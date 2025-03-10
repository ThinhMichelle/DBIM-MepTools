
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
using Autodesk.Revit.DB;
using Microsoft.Win32;

namespace MEP_Tools.CreateSiphon
{
    /// <summary>
    /// Interaction logic for MainView_CreateSiphon.xaml
    /// </summary>
    public partial class MainView_CreateSiphon : Window
    {
        public Command_Siphon MainViewModel { get; set; }
        public MainView_CreateSiphon()
        {
            InitializeComponent();
            this.DataContext = MainViewModel = new Command_Siphon();
        }
        private void btn_load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog OpenFD = new OpenFileDialog()
                {
                    Title = "Select Family Siphon",              
                    Filter = "Family Files (*rfa)|*.rfa",
                    RestoreDirectory = true
                };

                    if (OpenFD.ShowDialog() == true)
                    {
                        clsBien_Siphon.PathFamily = @OpenFD.FileName;
                        this.Hide();
                        string[] path = clsBien_Siphon.PathFamily.Split(new char[] { '\\'});
                        clsBien_Siphon.Name_Family = path[path.Count() - 1].Split(new char[] { '.' })[0];
                        

                        SingleData.Singleton.Instance.RevitData.Transaction.Start();

                        FilteredElementCollector Collector = new FilteredElementCollector(SingleData.Singleton.Instance.RevitData.Document);
                        IList<Element> familySymbols = Collector.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().ToElements();

                        string check = "No";
                        foreach (FamilySymbol sym in familySymbols)
                        {
                            if (sym.FamilyName == clsBien_Siphon.Name_Family)
                            {
                                clsBien_Siphon.FamilySymbol_Siphon = sym;
                                check = "Yes";
                                break;
                            }
                        }
                        if (check == "No")
                        {
                            SingleData.Singleton.Instance.RevitData.Document.LoadFamily(clsBien_Siphon.PathFamily);

                            FilteredElementCollector Collector1 = new FilteredElementCollector(SingleData.Singleton.Instance.RevitData.Document);
                            IList<Element> familySymbols1 = Collector.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().ToElements();

                            foreach (FamilySymbol sym1 in familySymbols1)
                            {
                                if (sym1.FamilyName == clsBien_Siphon.Name_Family)
                                {
                                    clsBien_Siphon.FamilySymbol_Siphon = sym1;
                                    break;
                                }
                            }
                            BitmapImage logo = new BitmapImage();
                            logo.BeginInit();
                            logo.UriSource = new Uri("C:\\ProgramData\\Autodesk\\Revit\\Addins\\" + cls_Contact.version + "\\DBIM Tools\\Images\\check.png");
                            logo.EndInit(); // Getting the exception here
                            image.Source = logo;
                        }
                        
                        SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                        text_familyname.Text = clsBien_Siphon.Name_Family  ;
                        this.ShowDialog();

                    }
                    else
                    {
                        MessageBox.Show("The family file has not been selected");
                    }

                
            }
            catch (Exception)
            {
                MessageBox.Show("The family file has not been selected");
            }
        }

        private void text_familyname_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilteredElementCollector Collector = new FilteredElementCollector(SingleData.Singleton.Instance.RevitData.Document);
            IList<Element> familySymbols = Collector.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().ToElements();
            string check = "No";
            foreach (FamilySymbol sym in familySymbols)
            {
                if (sym.FamilyName == text_familyname.Text)
                {
                    clsBien_Siphon.FamilySymbol_Siphon = sym;
                    check = "Yes";
                    break;
                }
            }
            if (check == "Yes")
            {
                status_load.Content = "Loaded";

                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri("C:\\ProgramData\\Autodesk\\Revit\\Addins\\" + cls_Contact.version + "\\DBIM Tools\\Images\\check.png");
                logo.EndInit(); // Getting the exception here
                image.Source = logo;
            }
            else
            {
                status_load.Content = "Not loaded yet";

                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri("C:\\ProgramData\\Autodesk\\Revit\\Addins\\" + cls_Contact.version + "\\DBIM Tools\\Images\\x.png");
                logo.EndInit(); // Getting the exception here
                image.Source = logo;
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
            this.Topmost = true;

            if (clsBien_Siphon.Name_Family != string.Empty)
            {
                text_familyname.Text = clsBien_Siphon.Name_Family;
            }   
        }

        private void btn_pick_Click(object sender, RoutedEventArgs e)
        {
            if (status_load.Content.ToString() == "Loaded")
            {
                clsBien_Siphon.Name_Family = text_familyname.Text ;
            }
            else
            {
                clsBien_Siphon.Name_Family = "";
            }
        }
    }

}
