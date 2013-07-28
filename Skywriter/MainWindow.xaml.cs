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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            this.ShowsNavigationUI = false;

            InitializeComponent();
            
            SkywriterUser clipUser = SecurityHelper.DeserializeUserDetails();

            if (clipUser != null)
            {
                UserWebservices.CLIPBOARD_URL = Properties.Settings.Default.RESTServerLocation;

                clipUser = UserWebservices.GetUser(clipUser.Id);

                if (clipUser != null)
                {
                    SkywriterBoard skywriterBoard = new SkywriterBoard();
                    this.NavigationService.Navigate(skywriterBoard);
                }
                else
                {
                    SecurityHelper.DeleteUserDetails();

                    UserLoginSelect userLoginSelect = new UserLoginSelect();
                    this.NavigationService.Navigate(userLoginSelect);
                }
            }
        }

        private void OnClose(object sender, CancelEventArgs args)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                args.Cancel = true;
                this.WindowState = WindowState.Minimized;
            }
        }
    }
}
