using Skywriter.Helpers;
using Skywriter.Model;
using Skywriter.Webservices;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Skywriter
{
    public partial class SkywriterBoard : Page
    {
        private SkywriterUser _skywriterUser;

        private SkywriterBoardModel _skywriterBoardModel;

        private HubConnection _connection;
        private IHubProxy _proxy;

        public SkywriterBoard()
        {
            UserWebservices.CLIPBOARD_URL = Properties.Settings.Default.RESTServerLocation;

            _skywriterUser = SecurityHelper.DeserializeUserDetails();

            if (_skywriterUser != null)
            {
                _skywriterUser = UserWebservices.GetUser(_skywriterUser.Id);
            }

            if (_skywriterUser != null)
            {
                try
                {
                    _connection = new HubConnection(Properties.Settings.Default.SocketServerLocation, "userId=" + _skywriterUser.Id);
                }
                catch (Exception ex)
                {
                }

                _proxy = _connection.CreateHubProxy("SkywriterBoard");

                _proxy.On<String>("CopiedSkywriterItem", (clipboardText) =>
                {
                    try
                    {
                        clipboardText = EncryptionHelper.Decrypt(clipboardText, "undeclared", true);

                        Thread newThread = new Thread(() => SetClipboardText(clipboardText));
                        newThread.SetApartmentState(ApartmentState.STA);
                        newThread.Start();
                    }
                    catch (Exception ex)
                    {
                    }
                });

                try
                {
                    _connection.Start().Wait();
                }
                catch (Exception ex)
                {

                }

                InitializeComponent();

                Application.Current.MainWindow.MinHeight = 390;
                Application.Current.MainWindow.Height = 390;
                Application.Current.MainWindow.Title = "Skywriter - " + _skywriterUser.Name;

                _skywriterBoardModel = new SkywriterBoardModel();
                DataContext = _skywriterBoardModel;
            }
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            if (WriteToSkywriterContent.Text.Length > 0)
            {
                SetClipboardText(WriteToSkywriterContent.Text);

                _proxy.Invoke("CopySkywriterItem", _skywriterUser.Id, EncryptionHelper.Encrypt(WriteToSkywriterContent.Text, "undeclared", true));

                WriteToSkywriterContent.Text = String.Empty;
                WriteToSkywriterContent.Focus();
            }
        }

        private void SetClipboardText(String text)
        {
            ClipboardHelper.CopyToClipboard(text);
            _skywriterBoardModel._SharedSkywriterContent = text;
        }
    }

    public class SkywriterBoardModel : INotifyPropertyChanged
    {
        private String _sharedSkywriterContent;
        private String _writeToSkywriterContent;

        public String _SharedSkywriterContent
        {
            get { return _sharedSkywriterContent; }
            set
            {
                if (value == _sharedSkywriterContent)
                    return;

                _sharedSkywriterContent = value;
                OnPropertyChanged("_SharedSkywriterContent");
            }
        }

        public String _WriteToSkywriterContent
        {
            get { return _writeToSkywriterContent; }
            set
            {
                if (value == _writeToSkywriterContent)
                    return;

                _writeToSkywriterContent = value;
                OnPropertyChanged("_WriteToSkywriterContent");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
