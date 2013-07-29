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
using System.IO;

namespace Skywriter
{
    public partial class SkywriterBoard : Page
    {
        public static Boolean IsClosing = false;

        private static readonly String MESSAGE_SALT = "sunny";

        private SkywriterUser _skywriterUser;

        private SkywriterBoardModel _skywriterBoardModel;

        private HubConnection _connection;
        private IHubProxy _proxy;

        public SkywriterBoard()
        {
            UserWebservices.CLIPBOARD_URL = Properties.Settings.Default.RESTServerLocation;

            IsClosing = false;

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

                _proxy = GenerateProxy();

                try
                {
                    _connection.Start().Wait();
                }
                catch (Exception ex)
                {

                }

                InitializeComponent();

                Application.Current.MainWindow.MinHeight = 405;
                Application.Current.MainWindow.Height = 405;
                Application.Current.MainWindow.Title = "Skywriter - " + _skywriterUser.Name;

                _skywriterBoardModel = new SkywriterBoardModel();
                DataContext = _skywriterBoardModel;

                _connection.Reconnected += () =>
                {
                    _proxy.Invoke("ReconnectClient", _skywriterUser.Id);
                };

                _connection.Closed += () =>
                {
                    if (!IsClosing)
                    {
                        try
                        {
                            _connection = new HubConnection(Properties.Settings.Default.SocketServerLocation, "userId=" + _skywriterUser.Id);

                            _proxy = GenerateProxy();

                            _connection.Start().Wait();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                };
            }
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            _skywriterBoardModel._SharedSkywriterContent = String.Empty;
            _proxy.Invoke("ClearSkywriterBoard", _skywriterUser.Id);

            SharedSkywriterContent.Focus();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.FileName = "Skywriter";
            dlg.DefaultExt = ".text";
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                String filename = dlg.FileName;

                File.WriteAllText(filename, _skywriterBoardModel._SharedSkywriterContent);
            }

            SharedSkywriterContent.Focus();
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            if (WriteToSkywriterContent.Text.Length > 0)
            {
                SetBoardText(WriteToSkywriterContent.Text);

                _proxy.Invoke("CopySkywriterItem", _skywriterUser.Id, EncryptionHelper.Encrypt(WriteToSkywriterContent.Text, MESSAGE_SALT, true));

                WriteToSkywriterContent.Text = String.Empty;
            }

            WriteToSkywriterContent.Focus();
        }

        private void SetBoardText(String text)
        {
            _skywriterBoardModel._SharedSkywriterContent = text;
        }

        private void refresh_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                IsClosing = true;
                
                _connection.Stop();

                _connection = new HubConnection(Properties.Settings.Default.SocketServerLocation, "userId=" + _skywriterUser.Id);

                _proxy = GenerateProxy();

                _connection.Start().Wait();

                IsClosing = false;

                MessageBox.Show("Your connection has been refreshed", "Success", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Your connection could not be refreshed", "Error", MessageBoxButton.OK);
            }
        }

        private void logout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SecurityHelper.DeleteUserDetails();

            UserLoginSelect userLoginSelect = new UserLoginSelect();
            this.NavigationService.Navigate(userLoginSelect);
        }

        private IHubProxy GenerateProxy()
        {
            IHubProxy proxy = _connection.CreateHubProxy("SkywriterBoard");

            proxy.On<String>("CopiedSkywriterItem", (clipboardText) =>
            {
                try
                {
                    clipboardText = EncryptionHelper.Decrypt(clipboardText, MESSAGE_SALT, true);

                    Thread newThread = new Thread(() => SetBoardText(clipboardText));
                    newThread.SetApartmentState(ApartmentState.STA);
                    newThread.Start();
                }
                catch (Exception ex)
                {
                }
            });

            proxy.On("ClearSkywriterBoard", () =>
            {
                _skywriterBoardModel._SharedSkywriterContent = String.Empty;
            });

            return proxy;
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
