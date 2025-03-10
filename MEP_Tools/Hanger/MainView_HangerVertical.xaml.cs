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

namespace MEP_Tools.Hanger
{
    /// <summary>
    /// Interaction logic for MainView_HangerVertical.xaml
    /// </summary>
    public partial class MainView_HangerVertical : Window
    {
        public Hanger_Vertical MainViewModel { get; set; }
        public MainView_HangerVertical()
        {
            InitializeComponent();
            this.DataContext = MainViewModel = new Hanger_Vertical();
        }



        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            if (Grap_Width.IsChecked == true)
            {
                clsdata.grap_width = 1;
                clsdata.grap_height = 0;

            }
            else
            {
                clsdata.grap_width = 0;
                clsdata.grap_height = 1;
            }
            clsdata.Check_veri = "";

            if (tab_control.SelectedItem == tb_DSCD)
            {

                clsdata.index_page = 0;
                clsdata.Diameter_ublot = Convert.ToDouble(cbb_barmodel.Text);
                clsdata.Check_veri = "Over";
                if (single.IsChecked == true)
                {
                    clsdata.index_cbb = 0;
                }
                else
                {
                    clsdata.index_cbb = 2;
                }
            }
            else
            {
                if (txt_distancetowall.Text != "")
                {
                    if (txt_distancetowall.Text.All(char.IsDigit))
                    {
                        clsdata.index_page = 1;
                        clsdata.index_cbb = 1;
                        clsdata.distance_wall = Convert.ToDouble(txt_distancetowall.Text);
                        clsdata.Check_veri = "Over";
                    }
                    else
                    {
                        MessageBox.Show("Enter the distance between wall and hanger again !");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter the distance between wall and hanger !");
                }
            }
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

        private void Grap_Width_Checked(object sender, RoutedEventArgs e)
        {
            if (Grap_Width.IsChecked == true)
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri("C:\\ProgramData\\Autodesk\\Revit\\Addins\\"+cls_Contact.version +"\\DBIM Tools\\Images\\Onggiodunng1.png");
                logo.EndInit(); // Getting the exception here
                image.Source = logo;
            }
        }

        private void Grap_Height_Checked(object sender, RoutedEventArgs e)
        {
            if (Grap_Height.IsChecked == true)
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri("C:\\ProgramData\\Autodesk\\Revit\\Addins\\" + cls_Contact.version + "\\DBIM Tools\\Images\\Onggiodunng2.png");
                logo.EndInit(); // Getting the exception here
                image.Source = logo;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=aQ3VkDVdvsU&ab_channel=DbimConstructionCompany");
        }

        private void Button_MouseMove(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            ((ToolTip)btn.ToolTip).IsOpen = false;
        }
    }
}
