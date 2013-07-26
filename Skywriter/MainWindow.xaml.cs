using Skywriter.Helpers;
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

            if (SecurityHelper.DeserializeUserDetails() != null)
            {
                Clipboard clipboard = new Clipboard();
                this.NavigationService.Navigate(clipboard);
            }
        }

        private void OnClose(object sender, CancelEventArgs args)
        {
            args.Cancel = true;
            this.WindowState = WindowState.Minimized;
        }
    }
}
