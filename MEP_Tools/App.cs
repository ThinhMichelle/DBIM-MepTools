#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using xNet;
using System.Net.Http;
using System.Collections;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Events;
using System.Globalization;
using System.Net;
using System.Windows.Controls;
using TextBox = Autodesk.Revit.UI.TextBox;
using Image = System.Drawing.Image;
using System.Windows;
using System.Text.Json;
#endregion

namespace MEP_Tools
{
    class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            //GetDate();
            const string Ribbon_tab = "D-BIM Tools";
            const string Ribbon_panel1 = "MEP Tools";
            const string Ribbon_panel2 = "ARC Tools";
            const string Ribbon_panel0 = "Login";
            const string Ribbon_panel3 = "About";

            string lang_in = "en";
            string lang_out = "en";

            #region "Create tab"
            //Create tab
            try
            {
                a.CreateRibbonTab(Ribbon_tab);

            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.ToString());
                return Result.Failed;
            }
            #endregion
            #region "Create Login"
            RibbonPanel panel0 = null;
            if (panel0 == null)
            {
                panel0 = a.CreateRibbonPanel(Ribbon_tab, Ribbon_panel0);
            }
            #region "Images"
            //get image
            Image imgr = MEP_Tools.Properties.Resources.icon_user;
            ImageSource imgSrc_r = Bmp_GetImage(imgr);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_r = new PushButtonData("nRb_Register", "Login", Assembly.GetExecutingAssembly().Location, "MEP_Tools.Command_Register")
            {
                ToolTip = TranslateText("Log in to use other functions", lang_in, lang_out),
                Image = imgSrc_r,
                LargeImage = imgSrc_r
            };
            #endregion
            #region "Add Butoon to the ribbon"
            PushButton btn_r = panel0.AddItem(btn_Data_r) as PushButton;
            btn_r.Enabled = true;
            #endregion
            #endregion
            #region "Create Holymep"
            #region "panel1"
            //Create panel file          

            //RibbonPanel panel1 = null;
            cls_Reg.panel_mep = null;
            if (cls_Reg.panel_mep == null)
            {
                cls_Reg.panel_mep = a.CreateRibbonPanel(Ribbon_tab, Ribbon_panel1);
            }

            #region "Images"
            //get image
            Image img = MEP_Tools.Properties.Resources.Holy_New;
            ImageSource imgSrc = Bmp_GetImage(img);
            #endregion
            #region "Create button"
            PushButtonData btn_Data = new PushButtonData("nRb_HolyUpdown", "MEP Updown", Assembly.GetExecutingAssembly().Location, "MEP_Tools.Command_HUD")
            {
                ToolTip = TranslateText("Change Elevation of MEP components", lang_in, lang_out),
                LongDescription = TranslateText("You can change Elevation of MEP components as you like. Auto create fittings and you can change corner of fittings as you like", lang_in, lang_out),
                Image = imgSrc,
                LargeImage = imgSrc
            };

            #endregion
            #region "Add Butoon to the ribbon"
            //cls_Reg.B1 = 
            PushButton btn_holymep =  cls_Reg.panel_mep.AddItem(btn_Data) as PushButton;
            //cls_Reg.B1.Visible = false;
            #endregion
            #endregion
            #endregion        
            #region "Create AutoSplit"
            #region "panel3"

            #region "Images"
            //get image
            Image img3 = MEP_Tools.Properties.Resources.Split_New;
            ImageSource imgSrc3 = Bmp_GetImage(img3);
            #endregion
            #region "Create button"
            PushButtonData btn_Data3 = new PushButtonData("nRb_SplitPipe", "Auto Split", Assembly.GetExecutingAssembly().Location, "MEP_Tools.Command_SP")
            {
                ToolTip = TranslateText("Divide the pipe into several small segments", lang_in, lang_out),
                LongDescription = TranslateText("You can divide the Pipe, Duct, Conduit, CableTray into several small segments and auto create Union", lang_in, lang_out),
                Image = imgSrc3,
                LargeImage = imgSrc3
            };

            #endregion
            #region "Add Butoon to the ribbon"
            //cls_Reg.B3 =
            PushButton btn_autosplit =  cls_Reg.panel_mep.AddItem(btn_Data3) as PushButton;
            //cls_Reg.B3.Visible = false;
            #endregion
            #endregion
            #endregion
            #region "Create HangerHorizontal"
            #region "panel1"
            //Create panel file          

            #region "Images"
            //get image
            Image img_hamger_h = MEP_Tools.Properties.Resources.hanger_hori;
            ImageSource imgSrc_hanger_h = Bmp_GetImage(img_hamger_h);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_hanger_h = new PushButtonData("nRb_Hozi", "Hanger Horizontal", Assembly.GetExecutingAssembly().Location, "MEP_Tools.Hanger.Hanger_Horizontal")
            {
                ToolTip = TranslateText("Create hanger for Duct, Pipe", lang_in, lang_out),
                LongDescription = TranslateText("You can place one hanger at Duct, Pipe Horizontal or multiple hanger along Duct, Pipe Horizontal", lang_in, lang_out),
                Image = imgSrc_hanger_h,
                LargeImage = imgSrc_hanger_h
            };

            #endregion
            #region "Add Butoon to the ribbon"
            //cls_Reg.B5 =
            PushButton btn_hangerhorizontal = cls_Reg.panel_mep.AddItem(btn_Data_hanger_h) as PushButton;
            //cls_Reg.B5.Visible = false;
            #endregion
            #endregion
            #endregion
            #region "Create Hanger Vertical"
            #region "panel1"
            //Create panel file          

            #region "Images"
            //get image
            Image img_hamger_v = MEP_Tools.Properties.Resources.hanger_vertical;
            ImageSource imgSrc_hanger_v = Bmp_GetImage(img_hamger_v);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_hanger_v = new PushButtonData("nRb_Vertical", "Hanger Vertical", Assembly.GetExecutingAssembly().Location, "MEP_Tools.Hanger.Hanger_Vertical")
            {
                ToolTip = TranslateText("Create hanger for Duct, Pipe", lang_in, lang_out),
                LongDescription = TranslateText("You can place hanger for one Duct, Pipe Vertical or hanger for multiple Duct, Pipe Vertical", lang_in, lang_out),
                Image = imgSrc_hanger_v,
                LargeImage = imgSrc_hanger_v
            };

            #endregion
            #region "Add Butoon to the ribbon"
            //cls_Reg.B6 =
            PushButton btn_hangerv = cls_Reg.panel_mep.AddItem(btn_Data_hanger_v) as PushButton;
            //cls_Reg.B6.Visible = false;
            #endregion
            #endregion
            #endregion
            // thoat nuoc
            const string Ribbon_panel4 = "Plumbing Tools";
            #region "Create Drainage"
            //Create panel file          
            //RibbonPanel panel4 = null;
            cls_Reg.panel_thoatnuoc = null;
            if (cls_Reg.panel_thoatnuoc == null)
            {
                cls_Reg.panel_thoatnuoc = a.CreateRibbonPanel(Ribbon_tab, Ribbon_panel4);
            }
            
            #region "Images"
            //get image
            Image img_thoatnuoc1 = MEP_Tools.Properties.Resources.tee;
            ImageSource imgSrc_thoatnuoc1 = Bmp_GetImage(img_thoatnuoc1);
            Image img_thoatnuoc1a = MEP_Tools.Properties.Resources.tee8;
            ImageSource imgSrc_thoatnuoc1a = Bmp_GetImage(img_thoatnuoc1a);
            Image img_tooltip1 = MEP_Tools.Properties.Resources._1;
            ImageSource imgSrc_tooltip1 = Bmp_GetImage(img_tooltip1);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_thoatnuoc1 = new PushButtonData("nRb_Drainage1", "Vertical branch pipe 90(1T+3E)", Assembly.GetExecutingAssembly().Location, "MEP_Tools.ThoatNuoc.Command_Drainage1")
            {
                Image = imgSrc_thoatnuoc1a,
                ToolTip = "Connect vertical branch pipe to horizontal main pipe",
                LongDescription = "Connect from the vertical branch pipe to the horizontal main pipe by 1 tee and 3 elbow",
                ToolTipImage = imgSrc_tooltip1,
                LargeImage = imgSrc_thoatnuoc1
            };
            #endregion
   
            #region "Images"
            //get image
            Image img_thoatnuoc2 = MEP_Tools.Properties.Resources.tee;
            ImageSource imgSrc_thoatnuoc2 = Bmp_GetImage(img_thoatnuoc2);
            Image img_thoatnuoc2a = MEP_Tools.Properties.Resources.tee8;
            ImageSource imgSrc_thoatnuoc2a = Bmp_GetImage(img_thoatnuoc2a);
            Image img_tooltip2 = MEP_Tools.Properties.Resources._2;
            ImageSource imgSrc_tooltip2 = Bmp_GetImage(img_tooltip2);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_thoatnuoc2 = new PushButtonData("nRb_Drainage2", "Vertical branch pipe 45(1T+2E)", Assembly.GetExecutingAssembly().Location, "MEP_Tools.ThoatNuoc.Command_Drainage2")
            {
                Image = imgSrc_thoatnuoc2a,
                ToolTip = "Connect vertical branch pipe to horizontal main pipe",
                LongDescription = "Connect from the vertical branch pipe to the horizontal main pipe by 1 tee and 2 elbow",
                ToolTipImage = imgSrc_tooltip2,
                LargeImage = imgSrc_thoatnuoc2
            };
            #endregion

            #region "Images"
            //get image
            Image img_thoatnuoc3 = MEP_Tools.Properties.Resources.tee;
            ImageSource imgSrc_thoatnuoc3 = Bmp_GetImage(img_thoatnuoc3);
            Image img_thoatnuoc3a = MEP_Tools.Properties.Resources.tee8;
            ImageSource imgSrc_thoatnuoc3a = Bmp_GetImage(img_thoatnuoc3a);
            Image img_tooltip3 = MEP_Tools.Properties.Resources._3;
            ImageSource imgSrc_tooltip3 = Bmp_GetImage(img_tooltip3);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_thoatnuoc3 = new PushButtonData("nRb_Drainage3", "Horizontal parallel pipe (1T+1E)", Assembly.GetExecutingAssembly().Location, "MEP_Tools.ThoatNuoc.Command_Drainage3")
            {
                Image = imgSrc_thoatnuoc3a,
                ToolTip = "Connect 2 horizontal parallel pipes together",
                LongDescription = "Connect from the branch pipe to the horizontal main pipe",
                ToolTipImage = imgSrc_tooltip3,                            
                LargeImage = imgSrc_thoatnuoc3
            };
            #endregion

            #region "Images"
            //get image
            Image img_thoatnuoc4 = MEP_Tools.Properties.Resources.tee;
            ImageSource imgSrc_thoatnuoc4 = Bmp_GetImage(img_thoatnuoc4);
            Image img_thoatnuoc4a = MEP_Tools.Properties.Resources.tee8;
            ImageSource imgSrc_thoatnuoc4a = Bmp_GetImage(img_thoatnuoc4a);
            Image img_tooltip4 = MEP_Tools.Properties.Resources._4;
            ImageSource imgSrc_tooltip4 = Bmp_GetImage(img_tooltip4);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_thoatnuoc4 = new PushButtonData("nRb_Drainage4", "Perpendicular pipe 2 (Vertical Tee + 2E)", Assembly.GetExecutingAssembly().Location, "MEP_Tools.ThoatNuoc.Command_Drainage4")
            {
                Image = imgSrc_thoatnuoc4a,
                ToolTip = "Connect 2 horizontal perpendicular pipes together",
                LongDescription = "Connect from horizontal branch pipe to the horizontal main pipe by 1 tee and 2 elbow (vertical tee)",
                ToolTipImage = imgSrc_tooltip4,
                LargeImage = imgSrc_thoatnuoc4
            };
            #endregion

            #region "Images"
            //get image
            Image img_thoatnuoc5 = MEP_Tools.Properties.Resources.tee;
            ImageSource imgSrc_thoatnuoc5 = Bmp_GetImage(img_thoatnuoc5);
            Image img_thoatnuoc5a = MEP_Tools.Properties.Resources.tee8;
            ImageSource imgSrc_thoatnuoc5a = Bmp_GetImage(img_thoatnuoc5a);
            Image img_tooltip5 = MEP_Tools.Properties.Resources._5;
            ImageSource imgSrc_tooltip5 = Bmp_GetImage(img_tooltip5);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_thoatnuoc5 = new PushButtonData("nRb_Drainage5", "Vertical parallel pipe (1T + 3E)", Assembly.GetExecutingAssembly().Location, "MEP_Tools.ThoatNuoc.Command_Drainage5")
            {
                Image = imgSrc_thoatnuoc5a,
                ToolTip = "Connect 2 vertical parallel pipes together",
                LongDescription = "Connect from the branch pipe below the equipment to the vertical main  pipe",
                ToolTipImage = imgSrc_tooltip5,
                LargeImage = imgSrc_thoatnuoc5
            };
            
            #endregion

            #region "Images"
            //get image
            Image img_thoatnuoc6 = MEP_Tools.Properties.Resources.tee;
            ImageSource imgSrc_thoatnuoc6 = Bmp_GetImage(img_thoatnuoc6);
            Image img_thoatnuoc6a = MEP_Tools.Properties.Resources.tee8;
            ImageSource imgSrc_thoatnuoc6a = Bmp_GetImage(img_thoatnuoc6a);
            Image img_tooltip6 = MEP_Tools.Properties.Resources._6;
            ImageSource imgSrc_tooltip6 = Bmp_GetImage(img_tooltip6);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_thoatnuoc6 = new PushButtonData("nRb_Drainage6", "Perpendicular pipe 1(Horizontal Tee + 1E)", Assembly.GetExecutingAssembly().Location, "MEP_Tools.ThoatNuoc.Command_Drainage6")
            {
                Image = imgSrc_thoatnuoc6a,
                ToolTip = "Connect 2 horizontal perpendicular pipes together",
                LongDescription = "Connect from horizontal branch pipe to the horizontal main pipe by 1 tee and 1 elbow (horizontal tee)",
                ToolTipImage = imgSrc_tooltip6,
                LargeImage = imgSrc_thoatnuoc6
            };
            #endregion

            #region "Images"
            //get image
            Image img_thoatnuoc7 = MEP_Tools.Properties.Resources.tee;
            ImageSource imgSrc_thoatnuoc7 = Bmp_GetImage(img_thoatnuoc7);
            Image img_thoatnuoc7a = MEP_Tools.Properties.Resources.tee8;
            ImageSource imgSrc_thoatnuoc7a = Bmp_GetImage(img_thoatnuoc7a);
            Image img_tooltip7 = MEP_Tools.Properties.Resources._7;
            ImageSource imgSrc_tooltip7= Bmp_GetImage(img_tooltip7);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_thoatnuoc7 = new PushButtonData("nRb_Drainage7", "Perpendicular pipe 1(Cross Tee + 1E)", Assembly.GetExecutingAssembly().Location, "MEP_Tools.ThoatNuoc.Command_Drainage7")
            {
                Image = imgSrc_thoatnuoc7a,
                ToolTip = "Connect 2 horizontal perpendicular pipes together",
                LongDescription = "Connect from horizontal branch pipe to the horizontal main pipe by 1 tee and 1 elbow (cross tee)",
                ToolTipImage = imgSrc_tooltip7,
                LargeImage = imgSrc_thoatnuoc7
            };
            #endregion

            #region "Images"
            //get image
            Image img_thoatnuoc8 = MEP_Tools.Properties.Resources.tee;
            ImageSource imgSrc_thoatnuoc8 = Bmp_GetImage(img_thoatnuoc8);
            Image img_thoatnuoc8a = MEP_Tools.Properties.Resources.tee8;
            ImageSource imgSrc_thoatnuoc8a = Bmp_GetImage(img_thoatnuoc8a);
            Image img_tooltip8 = MEP_Tools.Properties.Resources._8;
            ImageSource imgSrc_tooltip8 = Bmp_GetImage(img_tooltip8);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_thoatnuoc8 = new PushButtonData("nRb_Drainage8", "Perpendicular pipe 2(Cross Tee + 2E)", Assembly.GetExecutingAssembly().Location, "MEP_Tools.ThoatNuoc.Command_Drainage8")
            {
                Image = imgSrc_thoatnuoc8a,
                ToolTip = "Connect 2 horizontal perpendicular pipes together",
                LongDescription = "Connect from horizontal branch pipe to the horizontal main pipe by 1 tee and 2 elbow (cross tee)",
                ToolTipImage = imgSrc_tooltip8,
                LargeImage = imgSrc_thoatnuoc8
            };
            #endregion

            #region "Images"
            //get image
            Image img_thoatnuoc9 = MEP_Tools.Properties.Resources.tee;
            ImageSource imgSrc_thoatnuoc9 = Bmp_GetImage(img_thoatnuoc9);
            Image img_thoatnuoc9a = MEP_Tools.Properties.Resources.tee8;
            ImageSource imgSrc_thoatnuoc9a = Bmp_GetImage(img_thoatnuoc9a);
            Image img_tooltip9 = MEP_Tools.Properties.Resources._9;
            ImageSource imgSrc_tooltip9 = Bmp_GetImage(img_tooltip9);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_thoatnuoc9 = new PushButtonData("nRb_Drainage9", "Perpendicular pipe 3(Vertical Tee + 3E)", Assembly.GetExecutingAssembly().Location, "MEP_Tools.ThoatNuoc.Command_Drainage9")
            {
                Image = imgSrc_thoatnuoc9a,
                ToolTip = "Connect 2 horizontal perpendicular pipes together",
                LongDescription = "Connect from horizontal branch pipe to the horizontal main pipe by 1 tee and 3 elbow (vertical tee)",
                ToolTipImage = imgSrc_tooltip9,
                LargeImage = imgSrc_thoatnuoc9
            };
            #endregion

            #region "Images"
            //get image
            Image img_thoatnuoc10 = MEP_Tools.Properties.Resources.tee;
            ImageSource imgSrc_thoatnuoc10 = Bmp_GetImage(img_thoatnuoc10);
            Image img_thoatnuoc10a = MEP_Tools.Properties.Resources.tee8;
            ImageSource imgSrc_thoatnuoc10a = Bmp_GetImage(img_thoatnuoc10a);
            Image img_tooltip10 = MEP_Tools.Properties.Resources._10;
            ImageSource imgSrc_tooltip10 = Bmp_GetImage(img_tooltip10);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_thoatnuoc10 = new PushButtonData("nRb_Drainage10", "Perpendicular pipe 3(Cross Tee + 3E)", Assembly.GetExecutingAssembly().Location, "MEP_Tools.ThoatNuoc.Command_Drainage10")
            {
                Image = imgSrc_thoatnuoc10a,
                ToolTip = "Connect 2 horizontal perpendicular pipes together",
                LongDescription = "Connect from horizontal branch pipe to the horizontal main pipe by 1 tee and 3 elbow (cross tee)",
                ToolTipImage = imgSrc_tooltip10,
                LargeImage = imgSrc_thoatnuoc10
            };
            #endregion

            #region "Images"
            //get image
            Image img_thoatnuoc11 = MEP_Tools.Properties.Resources.tee;
            ImageSource imgSrc_thoatnuoc11 = Bmp_GetImage(img_thoatnuoc11);
            Image img_thoatnuoc11a = MEP_Tools.Properties.Resources.tee8;
            ImageSource imgSrc_thoatnuoc11a = Bmp_GetImage(img_thoatnuoc11a);
            Image img_tooltip11 = MEP_Tools.Properties.Resources._11;
            ImageSource imgSrc_tooltip11 = Bmp_GetImage(img_tooltip11);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_thoatnuoc11 = new PushButtonData("nRb_Drainage11", "Perpendicular pipe 1 (Vertical Tee + 1E)", Assembly.GetExecutingAssembly().Location, "MEP_Tools.ThoatNuoc.Command_Drainage11")
            {
                Image = imgSrc_thoatnuoc11a,
                ToolTip = "Connect 2 horizontal perpendicular pipes together",
                LongDescription = "Connect from vertical branch pipe to the horizontal main pipe by 1 tee and 1 elbow (vertical cross tee)",
                ToolTipImage = imgSrc_tooltip11,
                LargeImage = imgSrc_thoatnuoc11
            };
            #endregion
            #endregion

            SplitButtonData sb_data_6_7 = new SplitButtonData("splitButton6_7", "Split");
            SplitButtonData sb_data_1_2 = new SplitButtonData("splitButton1_2", "Split");
            SplitButtonData sb_data_4_8 = new SplitButtonData("splitButton4_8", "Split");
            SplitButtonData sb_data_9_10 = new SplitButtonData("splitButton9_10", "Split");

            #region "Create TextBox"
            IList< RibbonItem > stackedItems1 = cls_Reg.panel_thoatnuoc.AddStackedItems(sb_data_6_7, sb_data_1_2, sb_data_4_8);

            SplitButton sb6_7 = stackedItems1[0] as SplitButton;
            sb6_7.AddPushButton(btn_Data_thoatnuoc6);
            sb6_7.AddPushButton(btn_Data_thoatnuoc7);
            sb6_7.AddPushButton(btn_Data_thoatnuoc11);

            SplitButton sb1_2 = stackedItems1[1] as SplitButton;
            sb1_2.AddPushButton(btn_Data_thoatnuoc1);
            sb1_2.AddPushButton(btn_Data_thoatnuoc2);

            SplitButton sb4_8 = stackedItems1[2] as SplitButton;
            sb4_8.AddPushButton(btn_Data_thoatnuoc4);
            sb4_8.AddPushButton(btn_Data_thoatnuoc8);

            IList<RibbonItem> stackedItems2 = cls_Reg.panel_thoatnuoc.AddStackedItems(btn_Data_thoatnuoc3, btn_Data_thoatnuoc5, sb_data_9_10);

            SplitButton sb9_10 = stackedItems2[2] as SplitButton;
            sb9_10.AddPushButton(btn_Data_thoatnuoc9);
            sb9_10.AddPushButton(btn_Data_thoatnuoc10);

            TextBoxData Tb_Data = new TextBoxData("nTB_Slopes");
            cls_Reg.textbox_slope = cls_Reg.panel_thoatnuoc.AddItem(Tb_Data) as TextBox;
            cls_Reg.textbox_slope.Width = 100;
            cls_Reg.textbox_slope.PromptText = "Slope (%)";
            cls_Reg.textbox_slope.EnterPressed += CallbackOfTextBox;
            #endregion
            #region "Create C1et2e"
            #region "Images"
            //get image
            Image img2 = MEP_Tools.Properties.Resources.Elbow24;
            ImageSource imgSrc2 = Bmp_GetImage(img2);
            #endregion
            #region "Create button"
            PushButtonData btn_Data2 = new PushButtonData("nRb_Convert1ElbowTo2Elbow", "Create 2 Elbow", Assembly.GetExecutingAssembly().Location, "MEP_Tools.Command_C1ET2E")
            {
                ToolTip = TranslateText("Convert from 1 elbow to 2 elbow", lang_in, lang_out),
                LongDescription = TranslateText("You can change the elbow type of Pipe to 2 elbow type", lang_in, lang_out),
                Image = imgSrc2,
                LargeImage = imgSrc2
            };

            #endregion
            #region "Add Butoon to the ribbon"
            //cls_Reg.B2 =
            PushButton btn_c1et2e = cls_Reg.panel_thoatnuoc.AddItem(btn_Data2) as PushButton;
            //cls_Reg.B2.Visible = false;
            #endregion
            #endregion
            #region "Create Siphong"

            #region "Images"
            //get image
            Image img_siphon = MEP_Tools.Properties.Resources.siphon;
            ImageSource imgSrc_siphon = Bmp_GetImage(img_siphon);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_siphon = new PushButtonData("nRb_siphon", "Create Siphon", Assembly.GetExecutingAssembly().Location, "MEP_Tools.CreateSiphon.Command_Siphon")
            {
                //ToolTip = TranslateText("Convert from 1 elbow to 2 elbow", lang_in, lang_out),
                //LongDescription = TranslateText("You can change the elbow type of Pipe to 2 elbow type", lang_in, lang_out),
                Image = imgSrc_siphon,
                LargeImage = imgSrc_siphon
            };

            #endregion
            #region "Add Butoon to the ribbon"
            //cls_Reg.B2 =
            //PushButton btn_siphon = cls_Reg.panel_thoatnuoc.AddItem(btn_Data_siphon) as PushButton;
            //cls_Reg.B2.Visible = false;
            #endregion

            #endregion   
            // arc
            #region "Create SmartAvoid"
            #region "panel4"
            //Create panel file          
            //RibbonPanel panel4 = null;
            cls_Reg.panel_arc = null;
            if (cls_Reg.panel_arc == null)
            {
                cls_Reg.panel_arc = a.CreateRibbonPanel(Ribbon_tab, Ribbon_panel2);
            } 
            #region "Images"
            //get image
            Image img4 = MEP_Tools.Properties.Resources.SA;
            ImageSource imgSrc4 = Bmp_GetImage(img4);
            #endregion
            #region "Create button"
            PushButtonData btn_Data4 = new PushButtonData("nRb_SmartAvoid", "Smart Avoid", Assembly.GetExecutingAssembly().Location, "MEP_Tools.Command_SmartAvoid")
            {
                ToolTip = TranslateText("Create openings on wall avoid MEP component", lang_in, lang_out),
                LongDescription = TranslateText("You can create openings with custom size on wall to avoid MEP component", lang_in, lang_out),
                Image = imgSrc4,
                LargeImage = imgSrc4
            };
            #endregion
            #region "Add Butoon to the ribbon"
            //cls_Reg.B4 =
            PushButton btn_smartavoi = cls_Reg.panel_arc.AddItem(btn_Data4) as PushButton;
            //cls_Reg.B4.Visible = false;
            #endregion
            #endregion
            #endregion        

            #region "Create Auto Cut Walls"
            #region "panel4"
            #region "Images"
            //get image
            Image imgcutwall = MEP_Tools.Properties.Resources._335478;
            ImageSource imgSrccutwall = Bmp_GetImage(imgcutwall);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_cutwall = new PushButtonData("nRb_CutWall", "Auto Cut Walls", Assembly.GetExecutingAssembly().Location, "MEP_Tools.Command_AutoCutWall")
            {
                Image = imgSrccutwall,
                LargeImage = imgSrccutwall
            };
            #endregion
            #region "Add Butoon to the ribbon"
            //cls_Reg.B4 =
            PushButton btn_cutwall = cls_Reg.panel_arc.AddItem(btn_Data_cutwall) as PushButton;
            //cls_Reg.B4.Visible = false;
            #endregion
            #endregion
            #endregion        
            // About
            #region "Create About"
            #region "panel3"
            //Create panel file          

            RibbonPanel panel3 = null;
            if (panel3 == null)
            {
                panel3 = a.CreateRibbonPanel(Ribbon_tab, Ribbon_panel3);
            }

            #region "Images"
            //get image
            Image imga = MEP_Tools.Properties.Resources._2022_07_20_15_16_19;
            ImageSource imgSrc_a = Bmp_GetImage(imga);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_a = new PushButtonData("nRb_About", "About", Assembly.GetExecutingAssembly().Location, "MEP_Tools.Command_About")
            {
                Image = imgSrc_a,
                LargeImage = imgSrc_a
            };
            #endregion
            #region "Add Butoon to the ribbon"
            PushButton btn_a = panel3.AddItem(btn_Data_a) as PushButton;
            btn_a.Enabled = true;
            #endregion
            #endregion
            #endregion
            const string Ribbon_panel5 = "FeedBack";
            // FeedBack
            #region "Create FeedBack"
            #region "panel3"
            //Create panel file          

            RibbonPanel panel5 = null;
            if (panel5 == null)
            {
                panel5 = a.CreateRibbonPanel(Ribbon_tab, Ribbon_panel5);
            }

            #region "Images"
            //get image
            Image img_feedback = MEP_Tools.Properties.Resources.feedback;
            ImageSource imgSrc_feedback = Bmp_GetImage(img_feedback);
            #endregion
            #region "Create button"
            PushButtonData btn_Data_FeedBack = new PushButtonData("nRb_FeedBack", "FeedBack", Assembly.GetExecutingAssembly().Location, "MEP_Tools.Command_FeedBack")
            {
                Image = imgSrc_feedback,
                LargeImage = imgSrc_feedback
            };
            #endregion
            #region "Add Butoon to the ribbon"
            PushButton btn_feedback = panel5.AddItem(btn_Data_FeedBack) as PushButton;
            btn_feedback.Enabled = true;
            #endregion
            #endregion
            #endregion

            cls_Reg.panel_thoatnuoc.Visible = false;
            cls_Reg.panel_mep.Visible = false;
            cls_Reg.panel_arc.Visible = false;
            if (cls_Reg.User_Name == "DanhThangDBIM" && cls_Reg.User_Password == "Dbim2025")
            {
                cls_Reg.panel_mep.Visible = true;
                cls_Reg.panel_arc.Visible = true;
                cls_Reg.panel_thoatnuoc.Visible = true;
            }
            else
            {
                MessageBox.Show("Thông tin tài khoản và mật khẩu không chính xác!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
            }    
            //    string str = CheckCodeKey(GetCodeKey());
            //string path = @"C:\\ProgramData\\Autodesk\\Revit\\Addins\\" + cls_Contact.version + "\\DBIM Tools\\Login.txt";
            //if (File.Exists(path))
            //{
            //    string[] lines = File.ReadAllLines(path);
            //    if (lines.Length >= 2)
            //    {
            //        string tk = lines[0];
            //        string mk = lines[1];
            //        if (tk == "" || mk == "")
            //        {
            //            cls_Reg.Login = "";
            //        }
            //        else
            //        {
            //            cls_Reg.Login = CheckLogin(GetCodeKey(), tk, mk);
            //        }
            //    }
            //    else
            //    {
            //        cls_Reg.Login = "";
            //    }
            //}
            //else
            //{
            //    cls_Reg.Login = "";
            //}
            
            //if (cls_Reg.Login == "Login")
            //{
            //    if (str == "OK")
            //    {
            //        cls_Reg.panel_mep.Visible = true;
            //        cls_Reg.panel_arc.Visible = true;
            //        cls_Reg.panel_thoatnuoc.Visible = true;
            //    }
            //}
            return Result.Succeeded;
        }
        
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
        //private BitmapSource Bmp_GetImage(Image img)
        //{
        //    BitmapImage bmp = new BitmapImage();
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        img.Save(ms, ImageFormat.Png);               
        //        ms.Position = 0;
        //        bmp.BeginInit();
        //        bmp.CacheOption = BitmapCacheOption.OnLoad;
        //        bmp.UriSource = null;
        //        bmp.StreamSource = ms;
        //        bmp.EndInit();

        //    }
        //    return bmp;
        //}

        private BitmapSource Bmp_GetImage(Image img)
        {
            BitmapImage bmp = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png); // Lưu hình ảnh dưới dạng PNG
                ms.Position = 0; // Đặt lại vị trí luồng về đầu
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = null;
                bmp.StreamSource = ms;
                bmp.EndInit();
            }
            return bmp;
        }
        #region delete check internet and date
        //public string CheckLogin(string str, string user, string pass)
        //{
        //    HttpRequest Check = new HttpRequest();
        //    string text = "";
        //    string result = "KO";
        //    try
        //    {
        //        text = Check.Get(cls_Contact.web_account, null).ToString();
        //        string text_str = str + ".*?(?=Login)";
        //        Match match = Regex.Match(text.ToString(), str + ".*?(?=Login)");
        //        if (match != Match.Empty)
        //        {
        //            string tk = match.ToString().Split(new char[] { '|' })[1];
        //            string mk = match.ToString().Split(new char[] { '|' })[2];
        //            if (tk == user)
        //            {
        //                if (mk == pass)
        //                {
        //                    cls_Reg.User_Email = match.ToString().Split(new char[] { '|' })[1];
        //                    cls_Reg.User_Name = match.ToString().Split(new char[] { '|' })[3];
        //                    cls_Reg.User_PhoneNumber = match.ToString().Split(new char[] { '|' })[4];
        //                    cls_Reg.User_Password = match.ToString().Split(new char[] { '|' })[2];
        //                    cls_Reg.User_Company = match.ToString().Split(new char[] { '|' })[5];
        //                    result = "Login";
        //                }
        //                else
        //                {
        //                    result = "Pass";
        //                }
        //            }
        //            else
        //            {
        //                result = "User";
        //            }
        //        }
        //        else
        //        {
        //            result = "KO";
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        result = "NO";
        //        System.Windows.MessageBox.Show("Please connect internet to Activate !!!");
        //    }
        //    return result;
        //}
        //public string CheckCodeKey(string str)
        //{
        //    HttpRequest Check = new HttpRequest();
        //    string text = "";
        //    string result = "";
        //    try
        //    {
        //        text = Check.Get(cls_Contact.web_date, null).ToString();
        //        Match match = Regex.Match(text.ToString(), str + ".*?(?=DBIMCompany)");             
        //        if (match != Match.Empty)
        //        {
        //            if (CheckDate(match) == "HH")
        //            {
        //                result = "KO";
        //            }
        //            else
        //            {
        //                result = "OK";
        //            }                   
        //        }
        //        else
        //        {
        //            result = "KO";
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        result = "KO";
        //    }

        //    return result;
        //}
        //public void GetDate()
        //{
        //    try
        //    {
        //        HttpRequest Check = new HttpRequest();
        //        string text_date = Check.Get("https://clock.onlinealarmkur.com/vi/", null).ToString();
        //        string str = ",";
        //        Match match_date = Regex.Match(text_date.ToString(), str + ".*?(?=</div>)");
        //        string[] date_real = match_date.ToString().Split(new char[] { ',' });
        //        string[] date = date_real[1].Split(new char[] { ' ' });
        //        cls_Reg.day_real = date[1];
        //        cls_Reg.month_real = date[3];
        //        cls_Reg.year_real = date[5];
        //        DateTime time_real = GetDate_Real();
        //        cls_Reg.day_real = time_real.Day.ToString();
        //        cls_Reg.month_real = time_real.Month.ToString();
        //        cls_Reg.year_real = time_real.Year.ToString();
        //    }
        //    catch
        //    {
        //        cls_Reg.day_real = "";
        //        cls_Reg.month_real = "";
        //        cls_Reg.year_real = "";
        //    }
            
        //}

        //public DateTime GetDate_Real()
        //{
        //    try
        //    {
        //        using (var response =
        //          WebRequest.Create("http://www.google.com").GetResponse())
        //            //string todaysDates =  response.Headers["date"];
        //            return DateTime.ParseExact(response.Headers["date"],
        //                "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
        //                CultureInfo.InvariantCulture.DateTimeFormat,
        //                DateTimeStyles.AssumeUniversal);
        //    }
        //    catch (WebException)
        //    {
        //        return DateTime.Now; //In case something goes wrong. 
        //    }
        //}   
        //public string CheckDate(Match match)
        //{
        //    string result = "";
        //    if (match != Match.Empty)
        //    {
        //        SingleData.Singleton.Instance = new SingleData.Singleton();

        //        string[] date = match.ToString().Split(new char[] { '|' });
        //        string day = date[1].Split(new char[] { '/' })[0];
        //        string month = date[1].Split(new char[] { '/' })[1];
        //        string year = date[1].Split(new char[] { '/' })[2];


        //        if (cls_Reg.day_real == "")
        //        {
        //            result = "HH";
        //        }
        //        else
        //        {
        //            System.DateTime now = new System.DateTime(Convert.ToInt32(cls_Reg.year_real), Convert.ToInt32(cls_Reg.month_real), Convert.ToInt32(cls_Reg.day_real));
        //            System.DateTime then = new System.DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
        //            System.TimeSpan diff1 = then.Subtract(now);
        //            int days = (int)Math.Ceiling(diff1.TotalDays);
        //            Warnings FormWarning = new Warnings(days);
        //            if (days <= 0)
        //            {
        //                result = "HH";
        //                FormWarning.ShowDialog();
        //                SingleData.Singleton.Instance.WFData.ContactWindow.ShowDialog();
        //                cls_Reg.dayleft = "0";
        //            }
        //            else if (days <= 5)
        //            {
        //                FormWarning.ShowDialog();
        //                SingleData.Singleton.Instance.WFData.ContactWindow.ShowDialog();
        //                cls_Reg.dayleft = days.ToString();
        //            }
        //            else
        //            {
        //                cls_Reg.dayleft = days.ToString();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        cls_Reg.dayleft = "0";
        //    }


        //    return result;
        //}
        //public string GetCodeKey()
        //{
        //    string output = RunCMD("wmic diskdrive get serialNumber");
        //    string output1 = RunCMD("ipconfig");

        //    string[] result1 = Regex.Split(output, @"\r\r\n|[ ]{2,}");
        //    string[] result2 = Regex.Split(output1, @"\r\r\n|\r\n|[ ]{2,}");

        //    string str1 = result1[2];
        //    string str2 = "";
        //    for (int i = 0; i < result2.Length; i++)
        //    {
        //        if (result2[i].Contains("Ethernet adapter Ethernet:"))
        //        {
        //            if (result2[i + 3] == "Connection-specific DNS Suffix")
        //            {
        //                str2 = result2[i + 8].Split(new char[] { ':' })[1].Split(new char[] { ' ' })[1];
        //            }
        //            else if (result2[i + 3] == "Media State . . . . . . . . . . . : Media disconnected")
        //            {

        //            }
        //        }
        //        else if (result2[i].Contains("Ethernet adapter Ethernet 2:"))
        //        {
        //            if (result2[i + 3] == "Connection-specific DNS Suffix")
        //            {
        //                str2 = result2[i + 8].Split(new char[] { ':' })[1].Split(new char[] { ' ' })[1];
        //            }
        //            else if (result2[i + 3] == "Media State . . . . . . . . . . . : Media disconnected")
        //            {

        //            }
        //        }
        //    }
        //    string str = "";
        //    str = str1 + str2;
        //    str = Encode(str);
        //    return str;
        //}
        //public string Encode(string input)
        //{
        //    string result = "";
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        if (char.IsNumber(input[i]) == true)
        //        {
        //            string code = "";
        //            switch (input[i])
        //            {
        //                case '0':
        //                    code = "0000";
        //                    break;
        //                case '1':
        //                    code = "0001";
        //                    break;
        //                case '2':
        //                    code = "0010";
        //                    break;
        //                case '3':
        //                    code = "0011";
        //                    break;
        //                case '4':
        //                    code = "0100";
        //                    break;
        //                case '5':
        //                    code = "0101";
        //                    break;
        //                case '6':
        //                    code = "0110";
        //                    break;
        //                case '7':
        //                    code = "0111";
        //                    break;
        //                case '8':
        //                    code = "1000";
        //                    break;
        //                case '9':
        //                    code = "1001";
        //                    break;
        //            }
        //            result += code;
        //        }
        //        else
        //        {
        //            result += input[i];
        //        }
        //    }
        //    return result;
        //}
        //private string RunCMD(string cmd)
        //{
        //    Process cmdProcess = new Process();
        //    cmdProcess.StartInfo.FileName = "cmd.exe";
        //    cmdProcess.StartInfo.Arguments = "/c " + cmd;
        //    cmdProcess.StartInfo.RedirectStandardOutput = true;
        //    cmdProcess.StartInfo.UseShellExecute = false;
        //    cmdProcess.StartInfo.CreateNoWindow = true;
        //    cmdProcess.Start();
        //    string output = cmdProcess.StandardOutput.ReadToEnd();
        //    cmdProcess.WaitForExit();
        //    if (String.IsNullOrEmpty(output))
        //    {
        //        return "";
        //    }
        //    return output;
        //}
        #endregion

        public string TranslateText(string input, string lang_in, string lang_out)
        {
            string translation = "";
            try
            {
                string url = String.Format
            ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
             lang_in, lang_out, Uri.EscapeUriString(input));
                HttpClient httpClient = new HttpClient();
                string result = httpClient.GetStringAsync(url).Result;
                var jsonData = JsonSerializer.Deserialize<List<dynamic>>(result);
                var translationItems = jsonData[0];

                foreach (object item in translationItems)
                {
                    IEnumerable translationLineObject = item as IEnumerable;
                    IEnumerator translationLineString = translationLineObject.GetEnumerator();
                    translationLineString.MoveNext();
                    translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
                }
                if (translation.Length > 1) { translation = translation.Substring(1); };
                return translation;
            }
            catch
            {
                return translation;
            }
            
            
        }
        public void CallbackOfTextBox(object sender, TextBoxEnterPressedEventArgs args)
        {
            Autodesk.Revit.UI.TextBox textBox = sender as Autodesk.Revit.UI.TextBox;
            try
            {
                string value = textBox.Value.ToString();
                double value_slope = Convert.ToDouble(value);
                textBox.Value = value_slope;
                cls_Reg.value_slope = Convert.ToString(value_slope);
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter slope value again");
                textBox.Value = "";
                cls_Reg.value_slope = "";
            }       
        }
       
    }
}
