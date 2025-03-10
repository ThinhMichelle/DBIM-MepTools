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
    /// Interaction logic for MainView_HangerHorizontal.xaml
    /// </summary>
    public partial class MainView_HangerHorizontal : Window
    {
        public Hanger_Horizontal MainViewModel { get; set; }
        public MainView_HangerHorizontal()
        {
            InitializeComponent();
            this.DataContext = MainViewModel = new Hanger_Horizontal();
        }



        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbb_options.SelectedIndex == 0 || cbb_options.SelectedIndex == 3)
            {
                txt_A.IsEnabled = false;
                txt_distance.IsEnabled = false;
            }
            else
            {
                txt_A.IsEnabled = true;
                txt_distance.IsEnabled = true;
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
            
            //clsdata.lst_select_link.Clear();
            //clsdata.lst_linkselect.Clear();

            list_box_DSCD.Items.Clear();
            list_box_DSCU.Items.Clear();
            foreach (var item in clsdata.lst_Link_revit)
            {
                CheckBox chk = new CheckBox();
                chk.Content = item;
                list_box_DSCD.Items.Add(chk);
            }
            foreach (var item in clsdata.lst_Link_revit)
            {
                CheckBox chk = new CheckBox();
                chk.Content = item;
                list_box_DSCU.Items.Add(chk);
            }

        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            clsdata.lst_select_link.Clear();
            clsdata.lst_linkselect.Clear();
            if (cbb_options.Text != "Place Pick Point" && cbb_options.Text != "Place Parallel Pick Point")
            {
                if (txt_A.Text != "")
                {
                    if (txt_A.Text.All(char.IsDigit))
                    {
                        if (txt_distance.Text != "")
                        {
                            if (txt_distance.Text.All(char.IsDigit))
                            {
                                    if (chk_floor.IsChecked == true || chk_framing.IsChecked == true)
                                    {
                                        clsdata.A = Convert.ToDouble(txt_A.Text);
                                        clsdata.distance = Convert.ToDouble(txt_distance.Text);
                                        clsdata.index_cbb = cbb_options.SelectedIndex;

                                        if (tab_control.SelectedItem == tab_rec)
                                        {
                                            clsdata.index_page = 0;
                                            foreach (CheckBox item in list_box_DSCD.Items)
                                            {
                                                if (item.IsChecked == true)
                                                {
                                                    clsdata.lst_select_link.Add(item.Content.ToString());
                                                }
                                            }
                                            if (clsdata.lst_select_link.Count == 0)
                                            {
                                                MessageBox.Show("Please select link !");
                                            }
                                        }
                                        else if (tab_control.SelectedItem == tab_cir)
                                        {
                                            clsdata.index_page = 1;
                                            foreach (CheckBox item in list_box_DSCU.Items)
                                            {
                                                if (item.IsChecked == true)
                                                {
                                                    clsdata.lst_select_link.Add(item.Content.ToString());
                                                }
                                            }
                                            if (clsdata.lst_select_link.Count == 0)
                                            {
                                                MessageBox.Show("Please select link !");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Choose host in which hager can be attached !");
                                    }                  
                            }
                            else
                            {
                                MessageBox.Show("Enter the distance from each other again !");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please enter the distance from each other (C) !");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Enter the distance to the edge again !");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter the distance to the edge (A) !");
                }
            }
            else if (cbb_options.Text == "Place Pick Point" || cbb_options.Text == "Place Parallel Pick Point")
            {
                clsdata.index_cbb = cbb_options.SelectedIndex;

                    clsdata.index_cbb = cbb_options.SelectedIndex;
                    if (chk_floor.IsChecked == true || chk_framing.IsChecked == true)
                    {
                        clsdata.index_cbb = cbb_options.SelectedIndex;

                        if (tab_control.SelectedItem == tab_rec)
                        {
                            clsdata.index_page = 0;
                            foreach (CheckBox item in list_box_DSCD.Items)
                            {
                                if (item.IsChecked == true)
                                {
                                    clsdata.lst_select_link.Add(item.Content.ToString());
                                }
                            }
                            if (clsdata.lst_select_link.Count == 0)
                            {
                                MessageBox.Show("Please select link !");
                            }
                        }
                        else if (tab_control.SelectedItem == tab_cir)
                        {
                            clsdata.index_page = 1;
                            foreach (CheckBox item in list_box_DSCU.Items)
                            {
                                if (item.IsChecked == true)
                                {
                                    clsdata.lst_select_link.Add(item.Content.ToString());
                                }
                            }
                            if (clsdata.lst_select_link.Count == 0)
                            {
                                MessageBox.Show("Please select link !");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Choose host in which hager can be attached !");
                    }

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=uLAg42JNSZs&ab_channel=DbimConstructionCompany");
        }

        private void Button_MouseMove(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            ((ToolTip)btn.ToolTip).IsOpen = false;
        }
    }
}
