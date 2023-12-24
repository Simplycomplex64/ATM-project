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
using System.Windows.Shapes;

namespace AtmProject
{
    /// <summary>
    /// Interaction logic for closeAtm.xaml
    /// </summary>
    public partial class closeAtm : Window
    {
        public closeAtm()
        {
            InitializeComponent();
        }

        private void goBackBtn_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).RestartApplication();
        }
    }
}
