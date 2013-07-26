using System;
using System.Collections.Generic;
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
    /// Interaction logic for UserLoginSelect.xaml
    /// </summary>
    public partial class UserLoginSelect : Page
    {
        public UserLoginSelect()
        {
            InitializeComponent();

            Application.Current.MainWindow.MinHeight = 150;
            Application.Current.MainWindow.Height = 150;
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)NewAccountSelect.IsChecked)
            {
                CreateAccount createAccount = new CreateAccount();
                this.NavigationService.Navigate(createAccount);
            }
            else
            {
                Login login = new Login();
                this.NavigationService.Navigate(login);
            }
        }
    }
}
