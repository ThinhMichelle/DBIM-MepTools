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
    public class Command_MUD : WPFData, IExternalCommand
    {
        public ICommand ApplyCommand { get; set; }
        public ICommand CancleCommand { get; set; }
        public ICommand AddComments { get; set; }

        public string ofsetValue;
        public Command_MUD()
        {
            ApplyCommand = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                try
                {              
                    if (clsBien_MUD.AlphaElbow != "")
                    {
                        #region "Khoi tao gia tri"
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = null;
                        clsBien_MUD.ConnectorWithUnion1 = null;
                        clsBien_MUD.ConnectorWithUnion2 = null;
                        clsBien_MUD.ConnectorUnion1 = null;
                        clsBien_MUD.ConnectorUnion2 = null;
                        clsBien_MUD.Id_old = null;
                        clsBien_MUD.LVId = null;
                        clsBien_MUD.List_Connect.Clear();
                        clsBien_MUD.List_DuctNew.Clear();
                        clsBien_MUD.List_PipeNew.Clear();
                        clsBien_MUD.List_CbTrayNew.Clear();
                        clsBien_MUD.List_CondiutNew.Clear();
                        clsBien_MUD.List_DuctNeightbor.Clear();
                        clsBien_MUD.List_PipeNeightbor.Clear();
                        clsBien_MUD.List_CbTrayNeightbor.Clear();
                        clsBien_MUD.List_ConduitNeightbor.Clear();
                        clsBien_MUD.FittingType = "";
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
                            clsBien_MUD.Id_old = ele.Id;
                            Func_MUD F = new Func_MUD();
                            string Ftype = F.CheckType(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.Id_old);
                            clsBien_MUD.LVId = F.GeLevelElement(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.Id_old);
                            clsBien_MUD.List_Connect = F.GetConnector(SingleData.Singleton.Instance.RevitData.Document, Ftype, clsBien_MUD.Id_old);
                            F.GetFitting();
                            if (clsBien_MUD.FittingType == "Union")
                            {
                                #region "Nang + Tao Ong"
                                if (clsBien_MUD.Id_Fitting1 != null && clsBien_MUD.Id_Fitting2 != null)
                                {
                                    clsBien_MUD.ConnectorUnion1 = F.Disconnector(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorWithUnion1, clsBien_MUD.Id_Fitting1);
                                    clsBien_MUD.ConnectorUnion2 = F.Disconnector(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorWithUnion2, clsBien_MUD.Id_Fitting2);
                                    F.GetElementNeighbor(clsBien_MUD.ConnectorUnion1);
                                    F.GetElementNeighbor(clsBien_MUD.ConnectorUnion2);
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            F.CreateDuct(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion1, clsBien_MUD.ConnectorWithUnion1, clsBien_MUD.DuctTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateDuct(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion2, clsBien_MUD.ConnectorWithUnion2, clsBien_MUD.DuctTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            break;
                                        case "Pipe":
                                            F.CreatePipe(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion1, clsBien_MUD.ConnectorWithUnion1, clsBien_MUD.PipeTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreatePipe(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion2, clsBien_MUD.ConnectorWithUnion2, clsBien_MUD.PipeTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            break;
                                        case "CableTray":
                                            F.CreateCbTray(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion1, clsBien_MUD.ConnectorWithUnion1, clsBien_MUD.CbTrayTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateCbTray(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion2, clsBien_MUD.ConnectorWithUnion2, clsBien_MUD.CbTrayTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            break;
                                        case "Conduit":
                                            F.CreateConduit(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion1, clsBien_MUD.ConnectorWithUnion1, clsBien_MUD.ConduitTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateConduit(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion2, clsBien_MUD.ConnectorWithUnion2, clsBien_MUD.ConduitTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            break;
                                    }
                                    SingleData.Singleton.Instance.RevitData.Document.Delete(clsBien_MUD.Id_Fitting1);
                                    SingleData.Singleton.Instance.RevitData.Document.Delete(clsBien_MUD.Id_Fitting2);
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_MUD.OffsetValue));
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            F.CreateDuctFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_DuctNew);
                                            break;
                                        case "Pipe":
                                            F.CreatePipeFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_PipeNew);
                                            break;
                                        case "CableTray":
                                            F.CreateCbTrayFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_CbTrayNew);
                                            break;
                                        case "Conduit":
                                            F.CreateConduitFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_CondiutNew);
                                            break;
                                    }
                                }
                                else if (clsBien_MUD.Id_Fitting1 != null && clsBien_MUD.Id_Fitting2 == null)
                                {
                                    clsBien_MUD.ConnectorUnion1 = F.Disconnector(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorWithUnion1, clsBien_MUD.Id_Fitting1);
                                    F.GetElementNeighbor(clsBien_MUD.ConnectorUnion1);
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            F.CreateDuct(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion1, clsBien_MUD.ConnectorWithUnion1, clsBien_MUD.DuctTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            break;
                                        case "Pipe":
                                            F.CreatePipe(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion1, clsBien_MUD.ConnectorWithUnion1, clsBien_MUD.PipeTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            break;
                                        case "CableTray":
                                            F.CreateCbTray(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion1, clsBien_MUD.ConnectorWithUnion1, clsBien_MUD.CbTrayTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            break;
                                        case "Conduit":
                                            F.CreateConduit(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion1, clsBien_MUD.ConnectorWithUnion1, clsBien_MUD.ConduitTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            break;
                                    }
                                    SingleData.Singleton.Instance.RevitData.Document.Delete(clsBien_MUD.Id_Fitting1);
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_MUD.OffsetValue));
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            F.CreateDuctFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_DuctNew);
                                            break;
                                        case "Pipe":
                                            F.CreatePipeFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_PipeNew);
                                            break;
                                        case "CableTray":
                                            F.CreateCbTrayFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_CbTrayNew);
                                            break;
                                        case "Conduit":
                                            F.CreateConduitFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_CondiutNew);
                                            break;
                                    }
                                }
                                else if (clsBien_MUD.Id_Fitting1 == null && clsBien_MUD.Id_Fitting2 != null)
                                {
                                    clsBien_MUD.ConnectorUnion2 = F.Disconnector(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorWithUnion2, clsBien_MUD.Id_Fitting2);
                                    F.GetElementNeighbor(clsBien_MUD.ConnectorUnion2);
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            F.CreateDuct(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion2, clsBien_MUD.ConnectorWithUnion2, clsBien_MUD.DuctTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            break;
                                        case "Pipe":
                                            F.CreatePipe(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion2, clsBien_MUD.ConnectorWithUnion2, clsBien_MUD.PipeTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            break;
                                        case "CableTray":
                                            F.CreateCbTray(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion2, clsBien_MUD.ConnectorWithUnion2, clsBien_MUD.CbTrayTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            break;
                                        case "Conduit":
                                            F.CreateConduit(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.ConnectorUnion2, clsBien_MUD.ConnectorWithUnion2, clsBien_MUD.ConduitTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.OffsetValue), Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            break;
                                    }
                                    SingleData.Singleton.Instance.RevitData.Document.Delete(clsBien_MUD.Id_Fitting2);
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_MUD.OffsetValue));
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            F.CreateDuctFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_DuctNew);
                                            break;
                                        case "Pipe":
                                            F.CreatePipeFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_PipeNew);
                                            break;
                                        case "CableTray":
                                            F.CreateCbTrayFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_CbTrayNew);
                                            break;
                                        case "Conduit":
                                            F.CreateConduitFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_CondiutNew);
                                            break;
                                    }
                                }
                                else if (clsBien_MUD.Id_Fitting1 == null && clsBien_MUD.Id_Fitting2 == null)
                                {
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_MUD.OffsetValue));
                                }
                                #endregion
                            }
                            else if (clsBien_MUD.FittingType == "Elbow")
                            {
                                #region "Xoa + Nam + Tao Ong"
                                // Xoa ong
                                if (clsBien_MUD.Id_Fitting1 != null && clsBien_MUD.Id_Fitting2 != null)
                                {
                                    F.DeleteFitting(SingleData.Singleton.Instance.RevitData.Document);
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_MUD.OffsetValue));
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            Duct dt_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old) as Duct;
                                            Duct dt_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old1) as Duct;
                                            Duct dt_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old2) as Duct;
                                            clsBien_MUD.List_DuctNeightbor.Add(dt_old1);
                                            clsBien_MUD.List_DuctNeightbor.Add(dt_old2);
                                            F.CreateDuct_New(SingleData.Singleton.Instance.RevitData.Document, dt_old1, dt_old, clsBien_MUD.DuctTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateDuct_New(SingleData.Singleton.Instance.RevitData.Document, dt_old2, dt_old, clsBien_MUD.DuctTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateDuctFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_DuctNew);
                                            break;
                                        case "Pipe":
                                            Pipe pi_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old) as Pipe;
                                            Pipe pi_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old1) as Pipe;
                                            Pipe pi_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old2) as Pipe;
                                            clsBien_MUD.List_PipeNeightbor.Add(pi_old1);
                                            clsBien_MUD.List_PipeNeightbor.Add(pi_old2);
                                            F.CreatePipe_New(SingleData.Singleton.Instance.RevitData.Document, pi_old1, pi_old, clsBien_MUD.PipeTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreatePipe_New(SingleData.Singleton.Instance.RevitData.Document, pi_old2, pi_old, clsBien_MUD.PipeTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreatePipeFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_PipeNew);
                                            break;
                                        case "CableTray":
                                            CableTray Cbt_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old) as CableTray;
                                            CableTray Cbt_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old1) as CableTray;
                                            CableTray Cbt_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old2) as CableTray;
                                            clsBien_MUD.List_CbTrayNeightbor.Add(Cbt_old1);
                                            clsBien_MUD.List_CbTrayNeightbor.Add(Cbt_old2);
                                            F.CreateCbTray_New(SingleData.Singleton.Instance.RevitData.Document, Cbt_old1, Cbt_old, clsBien_MUD.CbTrayTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateCbTray_New(SingleData.Singleton.Instance.RevitData.Document, Cbt_old2, Cbt_old, clsBien_MUD.CbTrayTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateCbTrayFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_CbTrayNew);
                                            break;
                                        case "Conduit":
                                            Conduit Cd_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old) as Conduit;
                                            Conduit Cd_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old1) as Conduit;
                                            Conduit Cd_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old2) as Conduit;
                                            clsBien_MUD.List_ConduitNeightbor.Add(Cd_old1);
                                            clsBien_MUD.List_ConduitNeightbor.Add(Cd_old2);
                                            F.CreateConduit_New(SingleData.Singleton.Instance.RevitData.Document, Cd_old1, Cd_old, clsBien_MUD.ConduitTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateConduit_New(SingleData.Singleton.Instance.RevitData.Document, Cd_old2, Cd_old, clsBien_MUD.ConduitTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateConduitFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_CondiutNew);
                                            break;
                                    }
                                }
                                else if (clsBien_MUD.Id_Fitting1 != null && clsBien_MUD.Id_Fitting2 == null)
                                {
                                    F.DeleteFitting(SingleData.Singleton.Instance.RevitData.Document);
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_MUD.OffsetValue));
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            Duct dt_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old) as Duct;
                                            Duct dt_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old1) as Duct;
                                            clsBien_MUD.List_DuctNeightbor.Add(dt_old1);
                                            F.CreateDuct_New(SingleData.Singleton.Instance.RevitData.Document, dt_old1, dt_old, clsBien_MUD.DuctTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateDuctFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_DuctNew);
                                            break;
                                        case "Pipe":
                                            Pipe pi_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old) as Pipe;
                                            Pipe pi_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old1) as Pipe;
                                            clsBien_MUD.List_PipeNeightbor.Add(pi_old1);
                                            F.CreatePipe_New(SingleData.Singleton.Instance.RevitData.Document, pi_old1, pi_old, clsBien_MUD.PipeTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreatePipeFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_PipeNew);
                                            break;
                                        case "CableTray":
                                            CableTray Cbt_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old) as CableTray;
                                            CableTray Cbt_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old1) as CableTray;
                                            clsBien_MUD.List_CbTrayNeightbor.Add(Cbt_old1);
                                            F.CreateCbTray_New(SingleData.Singleton.Instance.RevitData.Document, Cbt_old1, Cbt_old, clsBien_MUD.CbTrayTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateCbTrayFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_CbTrayNew);
                                            break;
                                        case "Conduit":
                                            Conduit Cd_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old) as Conduit;
                                            Conduit Cd_old1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old1) as Conduit;
                                            clsBien_MUD.List_ConduitNeightbor.Add(Cd_old1);
                                            F.CreateConduit_New(SingleData.Singleton.Instance.RevitData.Document, Cd_old1, Cd_old, clsBien_MUD.ConduitTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateConduitFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_CondiutNew);
                                            break;
                                    }
                                }
                                else if (clsBien_MUD.Id_Fitting1 == null && clsBien_MUD.Id_Fitting2 != null)
                                {

                                    F.DeleteFitting(SingleData.Singleton.Instance.RevitData.Document);
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_MUD.OffsetValue));
                                    switch (Ftype)
                                    {
                                        case "Duct":
                                            Duct dt_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old) as Duct;
                                            Duct dt_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old2) as Duct;
                                            clsBien_MUD.List_DuctNeightbor.Add(dt_old2);
                                            F.CreateDuct_New(SingleData.Singleton.Instance.RevitData.Document, dt_old2, dt_old, clsBien_MUD.DuctTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateDuctFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_DuctNew);
                                            break;
                                        case "Pipe":
                                            Pipe pi_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old) as Pipe;
                                            Pipe pi_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old2) as Pipe;
                                            clsBien_MUD.List_PipeNeightbor.Add(pi_old2);
                                            F.CreatePipe_New(SingleData.Singleton.Instance.RevitData.Document, pi_old2, pi_old, clsBien_MUD.PipeTypeOld.Id, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreatePipeFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_PipeNew);
                                            break;
                                        case "CableTray":
                                            CableTray Cbt_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old) as CableTray;
                                            CableTray Cbt_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old2) as CableTray;
                                            clsBien_MUD.List_CbTrayNeightbor.Add(Cbt_old2);
                                            F.CreateCbTray_New(SingleData.Singleton.Instance.RevitData.Document, Cbt_old2, Cbt_old, clsBien_MUD.CbTrayTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateCbTrayFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_CbTrayNew);
                                            break;
                                        case "Conduit":
                                            Conduit Cd_old = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old) as Conduit;
                                            Conduit Cd_old2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(clsBien_MUD.Id_old2) as Conduit;
                                            clsBien_MUD.List_ConduitNeightbor.Add(Cd_old2);
                                            F.CreateConduit_New(SingleData.Singleton.Instance.RevitData.Document, Cd_old2, Cd_old, clsBien_MUD.ConduitTypeOldID, clsBien_MUD.LVId, Convert.ToDouble(clsBien_MUD.AlphaElbow));
                                            F.CreateConduitFitting(SingleData.Singleton.Instance.RevitData.Document, clsBien_MUD.List_CondiutNew);
                                            break;
                                    }
                                }
                                else if (clsBien_MUD.Id_Fitting1 == null && clsBien_MUD.Id_Fitting2 == null)
                                {
                                    F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_MUD.OffsetValue));
                                }
                                #endregion
                            }
                            else
                            {
                                F.OffsetElement(SingleData.Singleton.Instance.RevitData.Document, ele, Ftype, Convert.ToDouble(clsBien_MUD.OffsetValue));
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
                SingleData.Singleton.Instance.WFData.InputWindow_MUD.ShowDialog();
            }
                
            return Result.Succeeded;
        }
    }
}
