#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Controls;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Electrical;
#endregion

namespace MEP_Tools
{
    [Transaction(TransactionMode.Manual)]
    public class Command_HUD : WPFData, IExternalCommand
    {
        public ICommand ApplyCommand { get; set; }
        public ICommand CancleCommand { get; set; }
        public ICommand AddComments { get; set; }

        public string ofsetValue;
        public Command_HUD()
        {
            ApplyCommand = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                try
                {              
                    if (clsBien_HUD.AlphaElbow != "")
                    {
                        #region "Khoi tao gia tri"
                        clsBien_HUD.Id_Fitting1 = null;
                        clsBien_HUD.Id_Fitting2 = null;
                        clsBien_HUD.ConnectorWithUnion1 = null;
                        clsBien_HUD.ConnectorWithUnion2 = null;
                        clsBien_HUD.ConnectorUnion1 = null;
                        clsBien_HUD.ConnectorUnion2 = null;
                        clsBien_HUD.Id_old = null;
                        clsBien_HUD.LVId = null;
                        clsBien_HUD.List_Connect.Clear();
                        clsBien_HUD.List_DuctNew.Clear();
                        clsBien_HUD.List_PipeNew.Clear();
                        clsBien_HUD.List_CbTrayNew.Clear();
                        clsBien_HUD.List_CondiutNew.Clear();
                        clsBien_HUD.List_DuctNeightbor.Clear();
                        clsBien_HUD.List_PipeNeightbor.Clear();
                        clsBien_HUD.List_CbTrayNeightbor.Clear();
                        clsBien_HUD.List_ConduitNeightbor.Clear();
                        clsBien_HUD.FittingType = "";
                        #endregion
                        #region 'CT chinh'
                        p.Hide();
                        SingleData.Singleton.Instance.RevitData.Transaction.Start();
                        Reference r = null;
                        try
                        {
                            r = SingleData.Singleton.Instance.RevitData.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                        }
                        catch
                        {

                        }
                        if (r != null)
                        {
                            Element ele = SingleData.Singleton.Instance.RevitData.Document.GetElement(r);
                            clsBien_HUD.Id_old = ele.Id;
                            Func_HUD F = new Func_HUD();
                            string Ftype = F.CheckType(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.Id_old);
                            clsBien_HUD.LVId = F.GeLevelElement(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.Id_old);
                            clsBien_HUD.List_Connect = F.GetConnector(SingleData.Singleton.Instance.RevitData.Document, Ftype, clsBien_HUD.Id_old);
                            F.GetFitting();
                            if (clsBien_HUD.FittingType == "Union")
                            {
                                #region "Nang + Tao Ong"
                                if (clsBien_HUD.Id_Fitting1 != null && clsBien_HUD.Id_Fitting2 != null)
                                {
                                    clsBien_HUD.ConnectorUnion1 = F.Disconnector(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorWithUnion1, clsBien_HUD.Id_Fitting1);
                                    clsBien_HUD.ConnectorUnion2 = F.Disconnector(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorWithUnion2, clsBien_HUD.Id_Fitting2);
                                    F.GetElementNeighbor(clsBien_HUD.ConnectorUnion1);
                                    F.GetElementNeighbor(clsBien_HUD.ConnectorUnion2);
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            F.CreateDuct(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion1, clsBien_HUD.ConnectorWithUnion1, clsBien_HUD.DuctTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateDuct(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion2, clsBien_HUD.ConnectorWithUnion2, clsBien_HUD.DuctTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            break;
                                        case "Pipe":
                                            F.CreatePipe(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion1, clsBien_HUD.ConnectorWithUnion1, clsBien_HUD.PipeTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreatePipe(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion2, clsBien_HUD.ConnectorWithUnion2, clsBien_HUD.PipeTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            break;
                                        case "CableTray":
                                            F.CreateCbTray(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion1, clsBien_HUD.ConnectorWithUnion1, clsBien_HUD.CbTrayTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateCbTray(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion2, clsBien_HUD.ConnectorWithUnion2, clsBien_HUD.CbTrayTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            break;
                                        case "Conduit":
                                            F.CreateConduit(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion1, clsBien_HUD.ConnectorWithUnion1, clsBien_HUD.ConduitTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateConduit(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion2, clsBien_HUD.ConnectorWithUnion2, clsBien_HUD.ConduitTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            break;
                                    }
                                    SingleData.Singleton.Instance.RevitData.Document.Delete(clsBien_HUD.Id_Fitting1);
                                    SingleData.Singleton.Instance.RevitData.Document.Delete(clsBien_HUD.Id_Fitting2);
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_HUD.OffsetValue));
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            F.CreateDuctFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_DuctNew);
                                            break;
                                        case "Pipe":
                                            F.CreatePipeFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_PipeNew);
                                            break;
                                        case "CableTray":
                                            F.CreateCbTrayFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_CbTrayNew);
                                            break;
                                        case "Conduit":
                                            F.CreateConduitFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_CondiutNew);
                                            break;
                                    }
                                }
                                else if (clsBien_HUD.Id_Fitting1 != null && clsBien_HUD.Id_Fitting2 == null)
                                {
                                    clsBien_HUD.ConnectorUnion1 = F.Disconnector(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorWithUnion1, clsBien_HUD.Id_Fitting1);
                                    F.GetElementNeighbor(clsBien_HUD.ConnectorUnion1);
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            F.CreateDuct(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion1, clsBien_HUD.ConnectorWithUnion1, clsBien_HUD.DuctTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            break;
                                        case "Pipe":
                                            F.CreatePipe(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion1, clsBien_HUD.ConnectorWithUnion1, clsBien_HUD.PipeTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            break;
                                        case "CableTray":
                                            F.CreateCbTray(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion1, clsBien_HUD.ConnectorWithUnion1, clsBien_HUD.CbTrayTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            break;
                                        case "Conduit":
                                            F.CreateConduit(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion1, clsBien_HUD.ConnectorWithUnion1, clsBien_HUD.ConduitTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            break;
                                    }
                                    SingleData.Singleton.Instance.RevitData.Document.Delete(clsBien_HUD.Id_Fitting1);
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_HUD.OffsetValue));
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            F.CreateDuctFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_DuctNew);
                                            break;
                                        case "Pipe":
                                            F.CreatePipeFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_PipeNew);
                                            break;
                                        case "CableTray":
                                            F.CreateCbTrayFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_CbTrayNew);
                                            break;
                                        case "Conduit":
                                            F.CreateConduitFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_CondiutNew);
                                            break;
                                    }
                                }
                                else if (clsBien_HUD.Id_Fitting1 == null && clsBien_HUD.Id_Fitting2 != null)
                                {
                                    clsBien_HUD.ConnectorUnion2 = F.Disconnector(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorWithUnion2, clsBien_HUD.Id_Fitting2);
                                    F.GetElementNeighbor(clsBien_HUD.ConnectorUnion2);
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            F.CreateDuct(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion2, clsBien_HUD.ConnectorWithUnion2, clsBien_HUD.DuctTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            break;
                                        case "Pipe":
                                            F.CreatePipe(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion2, clsBien_HUD.ConnectorWithUnion2, clsBien_HUD.PipeTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            break;
                                        case "CableTray":
                                            F.CreateCbTray(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion2, clsBien_HUD.ConnectorWithUnion2, clsBien_HUD.CbTrayTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            break;
                                        case "Conduit":
                                            F.CreateConduit(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.ConnectorUnion2, clsBien_HUD.ConnectorWithUnion2, clsBien_HUD.ConduitTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.OffsetValue), Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            break;
                                    }
                                    SingleData.Singleton.Instance.RevitData.Document.Delete(clsBien_HUD.Id_Fitting2);
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_HUD.OffsetValue));
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            F.CreateDuctFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_DuctNew);
                                            break;
                                        case "Pipe":
                                            F.CreatePipeFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_PipeNew);
                                            break;
                                        case "CableTray":
                                            F.CreateCbTrayFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_CbTrayNew);
                                            break;
                                        case "Conduit":
                                            F.CreateConduitFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_CondiutNew);
                                            break;
                                    }
                                }
                                else if (clsBien_HUD.Id_Fitting1 == null && clsBien_HUD.Id_Fitting2 == null)
                                {
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_HUD.OffsetValue));
                                }
                                #endregion
                            }
                            else if (clsBien_HUD.FittingType == "Elbow")
                            {
                                #region "Xoa + Nam + Tao Ong"
                                // Xoa ong
                                if (clsBien_HUD.Id_Fitting1 != null && clsBien_HUD.Id_Fitting2 != null)
                                {
                                    F.DeleteFitting(SingleData.Singleton.Instance.RevitData.Document);
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_HUD.OffsetValue));
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            Duct dt_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old) as Duct;
                                            Duct dt_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old1) as Duct;
                                            Duct dt_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old2) as Duct;
                                            clsBien_HUD.List_DuctNeightbor.Add(dt_old1);
                                            clsBien_HUD.List_DuctNeightbor.Add(dt_old2);
                                            F.CreateDuct_New(SingleData.Singleton.Instance.RevitData.Document, dt_old1, dt_old, clsBien_HUD.DuctTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateDuct_New(SingleData.Singleton.Instance.RevitData.Document, dt_old2, dt_old, clsBien_HUD.DuctTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateDuctFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_DuctNew);
                                            break;
                                        case "Pipe":
                                            Pipe pi_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old) as Pipe;
                                            Pipe pi_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old1) as Pipe;
                                            Pipe pi_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old2) as Pipe;
                                            clsBien_HUD.List_PipeNeightbor.Add(pi_old1);
                                            clsBien_HUD.List_PipeNeightbor.Add(pi_old2);
                                            F.CreatePipe_New(SingleData.Singleton.Instance.RevitData.Document, pi_old1, pi_old, clsBien_HUD.PipeTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreatePipe_New(SingleData.Singleton.Instance.RevitData.Document, pi_old2, pi_old, clsBien_HUD.PipeTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreatePipeFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_PipeNew);
                                            break;
                                        case "CableTray":
                                            CableTray Cbt_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old) as CableTray;
                                            CableTray Cbt_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old1) as CableTray;
                                            CableTray Cbt_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old2) as CableTray;
                                            clsBien_HUD.List_CbTrayNeightbor.Add(Cbt_old1);
                                            clsBien_HUD.List_CbTrayNeightbor.Add(Cbt_old2);
                                            F.CreateCbTray_New(SingleData.Singleton.Instance.RevitData.Document, Cbt_old1, Cbt_old, clsBien_HUD.CbTrayTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateCbTray_New(SingleData.Singleton.Instance.RevitData.Document, Cbt_old2, Cbt_old, clsBien_HUD.CbTrayTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateCbTrayFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_CbTrayNew);
                                            break;
                                        case "Conduit":
                                            Conduit Cd_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old) as Conduit;
                                            Conduit Cd_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old1) as Conduit;
                                            Conduit Cd_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old2) as Conduit;
                                            clsBien_HUD.List_ConduitNeightbor.Add(Cd_old1);
                                            clsBien_HUD.List_ConduitNeightbor.Add(Cd_old2);
                                            F.CreateConduit_New(SingleData.Singleton.Instance.RevitData.Document, Cd_old1, Cd_old, clsBien_HUD.ConduitTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateConduit_New(SingleData.Singleton.Instance.RevitData.Document, Cd_old2, Cd_old, clsBien_HUD.ConduitTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateConduitFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_CondiutNew);
                                            break;
                                    }
                                }
                                else if (clsBien_HUD.Id_Fitting1 != null && clsBien_HUD.Id_Fitting2 == null)
                                {
                                    F.DeleteFitting(SingleData.Singleton.Instance.RevitData.Document);
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_HUD.OffsetValue));
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            Duct dt_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old) as Duct;
                                            Duct dt_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old1) as Duct;
                                            clsBien_HUD.List_DuctNeightbor.Add(dt_old1);
                                            F.CreateDuct_New(SingleData.Singleton.Instance.RevitData.Document, dt_old1, dt_old, clsBien_HUD.DuctTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateDuctFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_DuctNew);
                                            break;
                                        case "Pipe":
                                            Pipe pi_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old) as Pipe;
                                            Pipe pi_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old1) as Pipe;
                                            clsBien_HUD.List_PipeNeightbor.Add(pi_old1);
                                            F.CreatePipe_New(SingleData.Singleton.Instance.RevitData.Document, pi_old1, pi_old, clsBien_HUD.PipeTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreatePipeFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_PipeNew);
                                            break;
                                        case "CableTray":
                                            CableTray Cbt_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old) as CableTray;
                                            CableTray Cbt_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old1) as CableTray;
                                            clsBien_HUD.List_CbTrayNeightbor.Add(Cbt_old1);
                                            F.CreateCbTray_New(SingleData.Singleton.Instance.RevitData.Document, Cbt_old1, Cbt_old, clsBien_HUD.CbTrayTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateCbTrayFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_CbTrayNew);
                                            break;
                                        case "Conduit":
                                            Conduit Cd_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old) as Conduit;
                                            Conduit Cd_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old1) as Conduit;
                                            clsBien_HUD.List_ConduitNeightbor.Add(Cd_old1);
                                            F.CreateConduit_New(SingleData.Singleton.Instance.RevitData.Document, Cd_old1, Cd_old, clsBien_HUD.ConduitTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateConduitFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_CondiutNew);
                                            break;
                                    }
                                }
                                else if (clsBien_HUD.Id_Fitting1 == null && clsBien_HUD.Id_Fitting2 != null)
                                {

                                    F.DeleteFitting(SingleData.Singleton.Instance.RevitData.Document);
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_HUD.OffsetValue));
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            Duct dt_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old) as Duct;
                                            Duct dt_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old2) as Duct;
                                            clsBien_HUD.List_DuctNeightbor.Add(dt_old2);
                                            F.CreateDuct_New(SingleData.Singleton.Instance.RevitData.Document, dt_old2, dt_old, clsBien_HUD.DuctTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateDuctFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_DuctNew);
                                            break;
                                        case "Pipe":
                                            Pipe pi_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old) as Pipe;
                                            Pipe pi_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old2) as Pipe;
                                            clsBien_HUD.List_PipeNeightbor.Add(pi_old2);
                                            F.CreatePipe_New(SingleData.Singleton.Instance.RevitData.Document, pi_old2, pi_old, clsBien_HUD.PipeTypeOld.Id, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreatePipeFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_PipeNew);
                                            break;
                                        case "CableTray":
                                            CableTray Cbt_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old) as CableTray;
                                            CableTray Cbt_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old2) as CableTray;
                                            clsBien_HUD.List_CbTrayNeightbor.Add(Cbt_old2);
                                            F.CreateCbTray_New(SingleData.Singleton.Instance.RevitData.Document, Cbt_old2, Cbt_old, clsBien_HUD.CbTrayTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateCbTrayFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_CbTrayNew);
                                            break;
                                        case "Conduit":
                                            Conduit Cd_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old) as Conduit;
                                            Conduit Cd_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_HUD.Id_old2) as Conduit;
                                            clsBien_HUD.List_ConduitNeightbor.Add(Cd_old2);
                                            F.CreateConduit_New(SingleData.Singleton.Instance.RevitData.Document, Cd_old2, Cd_old, clsBien_HUD.ConduitTypeOldID, clsBien_HUD.LVId, Convert.ToDouble(clsBien_HUD.AlphaElbow));
                                            F.CreateConduitFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_HUD.List_CondiutNew);
                                            break;
                                    }
                                }
                                else if (clsBien_HUD.Id_Fitting1 == null && clsBien_HUD.Id_Fitting2 == null)
                                {
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_HUD.OffsetValue));
                                }
                                #endregion
                            }
                            else
                            {
                                F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_HUD.OffsetValue));
                            }                                          
                        }
                        SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                        p.ShowDialog();
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message + "\n" + "Contact: " + cls_Contact.sdt + " or Email: " + cls_Contact.email);
                }
            });
            CancleCommand = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                p.Close();
            });
            AddComments = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                p.Hide();
                SingleData.Singleton.Instance.RevitData.Transaction.Start();
                IList<Element> refs = new List<Element>();
                try
                {
                    refs = SingleData.Singleton.Instance.RevitData.Selection.PickElementsByRectangle();
                }
                catch (Exception)
                {

                }
                if (refs != null)
                {
                    foreach (Element r in refs)
                    {
                        r.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(r.Id.ToString());
                    }
                }               
                SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                p.ShowDialog();
            });
        }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            SingleData.Singleton.Instance = new SingleData.Singleton(); 
            SingleData.Singleton.Instance.RevitData.UIApplication = commandData.Application;
            if (cls_Reg.Login == "Login")
            {
                SingleData.Singleton.Instance.WFData.InputWindow_HUD.ShowDialog();
            }
                
            return Result.Succeeded;
        }
    }
}
