using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using xNet;
using System.Net.Http;
using System.IO;

namespace MEP_Tools.Register
{
    /// <summary>
    /// Interaction logic for MainView_Register.xaml
    /// </summary>
    /// 
    public partial class MainView_Register : Window
    {
        public Command_Register MainViewModel { get; set; }
        public MainView_Register()
        {
            InitializeComponent();
            this.DataContext = MainViewModel = new Command_Register();
        }
        public string GetCodeKey()
        {
            string output = RunCMD("wmic diskdrive get serialNumber");
            string output1 = RunCMD("ipconfig");

            string[] result1 = Regex.Split(output, @"\r\r\n|[ ]{2,}");
            string[] result2 = Regex.Split(output1, @"\r\r\n|\r\n|[ ]{2,}");

            string str1 = result1[2];
            string str2 = "";
            for (int i = 0; i < result2.Length; i++)
            {
                if (result2[i].Contains("Ethernet adapter Ethernet:"))
                {
                    if (result2[i + 3] == "Connection-specific DNS Suffix")
                    {
                        str2 = result2[i + 8].Split(new char[] { ':' })[1].Split(new char[] { ' ' })[1];
                    }
                    else if (result2[i + 3] == "Media State . . . . . . . . . . . : Media disconnected")
                    {

                    }
                }
                else if (result2[i].Contains("Ethernet adapter Ethernet 2:"))
                {
                    if (result2[i + 3] == "Connection-specific DNS Suffix")
                    {
                        str2 = result2[i + 8].Split(new char[] { ':' })[1].Split(new char[] { ' ' })[1];
                    }
                    else if (result2[i + 3] == "Media State . . . . . . . . . . . : Media disconnected")
                    {

                    }
                }
            }
            string str = "";
            str = str1 + str2;
            str = Encode(str);
            return str;
        }
        public string Encode(string input)
        {
            string result = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsNumber(input[i]) == true)
                {
                    string code = "";
                    switch (input[i])
                    {
                        case '0':
                            code = "0000";
                            break;
                        case '1':
                            code = "0001";
                            break;
                        case '2':
                            code = "0010";
                            break;
                        case '3':
                            code = "0011";
                            break;
                        case '4':
                            code = "0100";
                            break;
                        case '5':
                            code = "0101";
                            break;
                        case '6':
                            code = "0110";
                            break;
                        case '7':
                            code = "0111";
                            break;
                        case '8':
                            code = "1000";
                            break;
                        case '9':
                            code = "1001";
                            break;
                    }
                    result += code;
                }
                else
                {
                    result += input[i];
                }
            }
            return result;
        }
        private string RunCMD(string cmd)
        {
            Process cmdProcess = new Process();
            cmdProcess.StartInfo.FileName = "cmd.exe";
            cmdProcess.StartInfo.Arguments = "/c " + cmd;
            cmdProcess.StartInfo.RedirectStandardOutput = true;
            cmdProcess.StartInfo.UseShellExecute = false;
            cmdProcess.StartInfo.CreateNoWindow = true;
            cmdProcess.Start();
            string output = cmdProcess.StandardOutput.ReadToEnd();
            cmdProcess.WaitForExit();
            if (String.IsNullOrEmpty(output))
            {
                return "";
            }
            return output;
        }
        private void MainWPF_Loaded(object sender, RoutedEventArgs e)
        {
            cls_Reg.Str_Code = GetCodeKey();
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
        public string CheckCodeKey(string str)
        {
            HttpRequest Check = new HttpRequest();
            string text = "";
            string result = "KO";
            try
            {
                text = Check.Get(cls_Contact.web_date, null).ToString();
                Match match = Regex.Match(text.ToString(), str + ".*?(?=DBIMCompany)");
                if (match != Match.Empty)
                {
                    if (CheckDate(match) == "HH")
                    {
                        result = "KO";
                    }
                    else
                    {
                        result = "OK";
                    }

                }
                else
                {
                    result = "KO";
                }
            }
            catch (Exception)
            {
                result = "NO";
                MessageBox.Show("Please connect internet to Activate !!!");
            }
            return result;
        }
        public string CheckLogin(string str, string user, string pass)
        {
            HttpRequest Check = new HttpRequest();
            HttpClient httpClient = new HttpClient();
            string text = "";
            string result = "KO";
            try
            {
                text = Check.Get(cls_Contact.web_account, null).ToString();
                string text_str = str + ".*?(?=Login)";
                Match match = Regex.Match(text.ToString(), str + ".*?(?=Login)");
                if (match != Match.Empty)
                {
                    string tk = match.ToString().Split(new char[] { '|' })[1];
                    string mk = match.ToString().Split(new char[] { '|' })[2];
                    
                    if (tk == user)
                    {
                        if (mk == pass)
                        {
                            cls_Reg.User_Email = match.ToString().Split(new char[] { '|' })[1];
                            cls_Reg.User_Name = match.ToString().Split(new char[] { '|' })[3];
                            cls_Reg.User_PhoneNumber = match.ToString().Split(new char[] { '|' })[4];
                            cls_Reg.User_Password = match.ToString().Split(new char[] { '|' })[2];
                            cls_Reg.User_Company = match.ToString().Split(new char[] { '|' })[5];
                            string path = @"C:\\ProgramData\\Autodesk\\Revit\\Addins\\"+cls_Contact.version+"\\DBIM Tools\\Login.txt";
                            string[] noidung = { cls_Reg.User_Email, cls_Reg.User_Password };
                            File.WriteAllLines(path, noidung);
                            result = "Login";
                        }
                        else
                        {
                            result = "Pass";
                        }
                    }
                    else
                    {
                        result = "User";
                    }
                }
                else
                {
                    result = "KO";
                }
            }
            catch (Exception)
            {
                result = "NO";
                MessageBox.Show("Please connect internet to Activate !!!");
            }
            return result;
        }
        public string CheckDate(Match match)
        {
            string result = "";
            if (match != Match.Empty)
            {
                
                string[] date = match.ToString().Split(new char[] { '|' });
                string day = date[1].Split(new char[] { '/' })[0];
                string month = date[1].Split(new char[] { '/' })[1];
                string year = date[1].Split(new char[] { '/' })[2];
                System.DateTime now = new System.DateTime(Convert.ToInt32(cls_Reg.year_real), Convert.ToInt32(cls_Reg.month_real), Convert.ToInt32(cls_Reg.day_real));
                System.DateTime then = new System.DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
                System.TimeSpan diff1 = then.Subtract(now);

                int days = (int)Math.Ceiling(diff1.TotalDays);

                if (days <= 0)
                {
                    result = "HH";
                    cls_Reg.dayleft = "0";
                }
                else
                {
                    cls_Reg.dayleft = days.ToString();
                }
            }
            else
            {
                cls_Reg.dayleft = "0";
            }    
            return result;
        }
        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            if (Username.Text != "")
            {
                if (Password.Password != "")
                {
                    string checklogin = CheckLogin(cls_Reg.Str_Code, Username.Text, Password.Password);
                    if (checklogin == "Login")
                    {
                        string checkcode = CheckCodeKey(cls_Reg.Str_Code);
                        if (checkcode == "OK")
                        {
                            cls_Reg.Login = "Login";
                            MessageBox.Show("Login successfully !!!");
                            this.Close();
                        }
                        else
                        {
                            cls_Reg.Login = "Login";
                            MessageBox.Show("Account has not been activated !!!");
                            this.Close();
                        }
                    }
                    else if (checklogin == "User")
                    {
                        MessageBox.Show("Account does not exist, please register !!!");
                    }
                    else if (checklogin == "Pass")
                    {
                        MessageBox.Show("Incorrect password  !!!");
                    }
                    else if (checklogin == "KO")
                    {
                        MessageBox.Show("You logged into the wrong device. Please log into the registered device.!!!");
                    }
                    else if (checklogin == "NO")
                    {

                    }
                }
                else
                {
                    MessageBox.Show("Please enter password !!!");
                }
            }
            else
            {
                MessageBox.Show("Please enter email !!");
            }
        }
        private void btn_Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Username.Text != "")
                {
                    if (Password.Password != "")
                    {
                        string checklogin = CheckLogin(cls_Reg.Str_Code, Username.Text, Password.Password);
                        if (checklogin == "Login")
                        {
                            string checkcode = CheckCodeKey(cls_Reg.Str_Code);
                            if (checkcode == "OK")
                            {
                                cls_Reg.Login = "Login";
                                MessageBox.Show("Login successfully !!!");
                                this.Close();
                            }
                            else
                            {
                                cls_Reg.Login = "Login";
                                MessageBox.Show("Account has not been activated !!!");
                                this.Close();
                            }
                        }
                        else if (checklogin == "User")
                        {
                            MessageBox.Show("Account does not exist, please register !!!");
                        }
                        else if (checklogin == "Pass")
                        {
                            MessageBox.Show("Incorrect password  !!!");
                        }
                        else if (checklogin == "KO")
                        {
                            MessageBox.Show("You logged into the wrong device. Please log into the registered device.!!!");
                        }
                        else if (checklogin == "NO")
                        {

                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter password !!!");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter email !!");
                }
            }
        }
        private void btn_Register_Click(object sender, RoutedEventArgs e)
        {
            SingleData.Singleton.Instance = new SingleData.Singleton();
            SingleData.Singleton.Instance.WFData.InputWindow_RegisterNew.ShowDialog();
        }
        private void btn_ForgetPass_Click(object sender, RoutedEventArgs e)
        {
            SingleData.Singleton.Instance = new SingleData.Singleton();
            SingleData.Singleton.Instance.WFData.InputWindow_forgetPass.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=be_ofD82Lm0&ab_channel=DbimConstructionCompany");
        }

        private void Button_MouseMove(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            ((ToolTip)btn.ToolTip).IsOpen = false;
        }
    }
}
