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

namespace MEP_Tools.MEPUpdown
{
    /// <summary>
    /// Interaction logic for MainView_MUD.xaml
    /// </summary>
    public partial class MainView_MUD : Window
    {
        public Command_MUD MainViewModel { get; set; }
        public MainView_MUD()
        {
            InitializeComponent();
            this.DataContext = MainViewModel = new Command_MUD();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            clsBien_MUD.OffsetValue = Distance.Text;
            if (RdB90.IsChecked == true)
            {
                clsBien_MUD.AlphaElbow = "90";
            }
            else if (RdB45.IsChecked == true)
            {
                clsBien_MUD.AlphaElbow = "45";
            }
            else if (RdBCus.IsChecked == true)
            {
                if (Alpha.Text == "")
                {
                    MessageBox.Show("Please enter the angle of tilt !");
                }
                else if (Alpha.Text.All(char.IsDigit))
                {
                    clsBien_MUD.AlphaElbow = Alpha.Text;
                }
                else
                {
                    MessageBox.Show("The angle of tilt format isn't correct. Please enter again !");
                }
            }
        }
        private void RdBCus_Checked(object sender, RoutedEventArgs e)
        {
            Alpha.IsEnabled = true;
        }
        private void RdBCus_Unchecked(object sender, RoutedEventArgs e)
        {
            Alpha.IsEnabled = false;
        }

        private void MainWPF_Loaded(object sender, RoutedEventArgs e)
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=aWOhIce2o-8&ab_channel=DbimConstructionCompany");
        }

        private void Button_MouseMove(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            ((ToolTip)btn.ToolTip).IsOpen = false;
        }
    }
}
