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
    public partial class Clipboard : Page
    {
        private SkywriterUser _skywriterUser;

        private ClipboardModel _clipboardModel;

        private HubConnection _connection;
        private IHubProxy _proxy;

        public Clipboard()
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

                _proxy = _connection.CreateHubProxy("Clipboard");

                _proxy.On<String>("CopiedClipboardItem", (clipboardText) =>
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

                _clipboardModel = new ClipboardModel();
                DataContext = _clipboardModel;
            }
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            if (WritetoClipboardContent.Text.Length > 0)
            {
                SetClipboardText(WritetoClipboardContent.Text);

                _proxy.Invoke("CopyClipboardItem", _skywriterUser.Id, EncryptionHelper.Encrypt(WritetoClipboardContent.Text, "undeclared", true));

                WritetoClipboardContent.Text = String.Empty;
                WritetoClipboardContent.Focus();
            }
        }

        private void SetClipboardText(String text)
        {
            ClipboardHelper.CopyToClipboard(text);
            _clipboardModel._SharedClipboardContent = text;
        }
    }

    public class ClipboardModel : INotifyPropertyChanged
    {
        private String _sharedClipboardContent;
        private String _writetoClipboardContent;

        public String _SharedClipboardContent
        {
            get { return _sharedClipboardContent; }
            set
            {
                if (value == _sharedClipboardContent)
                    return;

                _sharedClipboardContent = value;
                OnPropertyChanged("_SharedClipboardContent");
            }
        }

        public String _WritetoClipboardContent
        {
            get { return _writetoClipboardContent; }
            set
            {
                if (value == _writetoClipboardContent)
                    return;

                _writetoClipboardContent = value;
                OnPropertyChanged("_WritetoClipboardContent");
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
