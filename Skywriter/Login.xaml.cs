using Skywriter.Helpers;
using Skywriter.Model;
using Skywriter.Webservices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        private LoginModel _loginModel;

        public Login()
        {
            InitializeComponent();

            Application.Current.MainWindow.MinHeight = 210;
            Application.Current.MainWindow.Height = 210;

            UserWebservices.CLIPBOARD_URL = Properties.Settings.Default.RESTServerLocation;

            _loginModel = new LoginModel();
            DataContext = _loginModel;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            UserLoginSelect userLoginSelect = new UserLoginSelect();
            this.NavigationService.Navigate(userLoginSelect);
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            SkywriterUser authenticatedUser = UserWebservices.Authenticate(UserName.Text, Password.Text);

            if (authenticatedUser != null)
            {
                SecurityHelper.SerializeUserDetails(authenticatedUser);

                Clipboard clipboard = new Clipboard();
                this.NavigationService.Navigate(clipboard);
            }
        }
    }

    public class LoginModel : INotifyPropertyChanged
    {
        private String _usersName;
        private String _userPassword;

        public String UsersName
        {
            get { return _usersName; }
            set
            {
                if (value == _usersName)
                    return;

                _usersName = value;
                OnPropertyChanged("UsersName");
            }
        }

        public String UserPassword
        {
            get { return _userPassword; }
            set
            {
                if (value == _userPassword)
                    return;

                _userPassword = value;
                OnPropertyChanged("UserPassword");
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
