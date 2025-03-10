#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace MEP_Tools.ThoatNuoc
{
    [Transaction(TransactionMode.Manual)]
    public class Command_Drainage5 : WPFData, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            SingleData.Singleton.Instance = new SingleData.Singleton();
            SingleData.Singleton.Instance.RevitData.UIApplication = commandData.Application;
            if (cls_Reg.Login == "Login")
            {
                if (cls_Reg.value_slope != "")
                {
                    cls_ThoatNuoc.Slope = Convert.ToDouble(cls_Reg.value_slope);
                    Run();
                }
                else
                {
                    MessageBox.Show("Please enter slope value !");
                }
                //SingleData.Singleton.Instance.WFData.InputWindow_ThoatNuoc.ShowDialog();
            }
            return Result.Succeeded;
        }
        public void Run()
        {


            Reference r_nhanh = null;
            Reference r_chinh = null;
            cls_ThoatNuoc.Id_Tee5 = null;
            try
            {
                r_nhanh = SingleData.Singleton.Instance.RevitData.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Please choose branch pipe");
                if (r_nhanh != null)
                {
                    try
                    {
                        r_chinh = SingleData.Singleton.Instance.RevitData.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Please choose main pipe");
                        if (r_chinh != null)
                        {
                            try
                            {
                                Func_ThoatNuoc F = new Func_ThoatNuoc();
                                SingleData.Singleton.Instance.RevitData.Transaction.Start();

                                Pipe p_chinh = SingleData.Singleton.Instance.RevitData.Document.GetElement(r_chinh) as Pipe; // chinh
                                Element e = SingleData.Singleton.Instance.RevitData.Document.GetElement(r_nhanh);
                                Pipe p_nhanh = null;
                                LocationCurve lc = p_chinh.Location as LocationCurve;
                                XYZ stPoint = lc.Curve.GetEndPoint(0);
                                XYZ edPoint = lc.Curve.GetEndPoint(1);

                                ElementId _levelId = p_chinh.ReferenceLevel.Id;
                                PipeType _pipeType = p_chinh.PipeType;
                                ElementId _pipeSystemType = p_chinh.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();

                                if (e is Pipe)
                                {
                                    p_nhanh = e as Pipe; // phu
                                }
                                else if (e is FamilyInstance)
                                {
                                    Connector ConnectorFromFixtures = F.GetConnector_Fixtures(e as FamilyInstance);
                                    var dir = ConnectorFromFixtures.CoordinateSystem.BasisZ;
                                    if (dir.IsAlmostEqualTo(XYZ.BasisZ) || dir.IsAlmostEqualTo(-XYZ.BasisZ))
                                    {
                                        XYZ point_DrawPipe = F.GetIntersec(stPoint, edPoint, ConnectorFromFixtures);
                                        p_nhanh = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, _pipeType.Id, _levelId, ConnectorFromFixtures, ConnectorFromFixtures.Origin + dir * (500 / 304.8));
                                    }
                                    else if (dir.IsAlmostEqualTo(XYZ.BasisX) || dir.IsAlmostEqualTo(-XYZ.BasisX) || dir.IsAlmostEqualTo(-XYZ.BasisY) || dir.IsAlmostEqualTo(-XYZ.BasisY))
                                    {
                                        Pipe p0 = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, _pipeType.Id, _levelId, ConnectorFromFixtures, ConnectorFromFixtures.Origin + dir * ConnectorFromFixtures.Radius * 3);
                                        Connector ConnectorPipe0 = F.GetConnecterPipeNotConnected(p0);
                                        p_nhanh = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, _pipeSystemType, _pipeType.Id, _levelId, ConnectorPipe0.Origin, ConnectorPipe0.Origin - XYZ.BasisZ * (500 / 304.8));
                                        F.CopyParameters(p0, p_nhanh);
                                        SingleData.Singleton.Instance.RevitData.Document.Create.NewElbowFitting(ConnectorPipe0, F.GetConnectorFromPoint(p_nhanh, ConnectorPipe0.Origin));
                                    }
                                }
                                double d1 = p_chinh.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
                                double d2 = p_nhanh.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
                                if (d1 >= d2)
                                {                                    

                                    Connector Conn_Bottom = F.GetConnector(p_nhanh);
                                                                   
                                    XYZ Intersec = F.CheckIntersec(stPoint, edPoint, F.GetIntersec(stPoint, edPoint, Conn_Bottom));
                                    if (Intersec != null)
                                    {
                                        XYZ diem1 = new XYZ(Intersec.X, Intersec.Y, 0);
                                        XYZ diem2 = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, 0);
                                        double kc = diem1.DistanceTo(diem2);
                                        Conn_Bottom.Origin = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, Intersec.Z + kc * cls_ThoatNuoc.Slope / 100);                                      
                                        #region 'CT chinh'
                                        Pipe p_new = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, p_nhanh.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId(), p_nhanh.PipeType.Id, p_nhanh.ReferenceLevel.Id, Conn_Bottom.Origin, Intersec);
                                        F.CopyParameters(p_nhanh, p_new);
                                        double len = p_new.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                                        F.Case5(p_chinh, p_new, stPoint, edPoint, Intersec, len, _pipeSystemType, _pipeType.Id, _levelId, F.GetConnectorFromPoint(p_new, Intersec), Conn_Bottom);
                                        #endregion
                                        SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                                        if (cls_ThoatNuoc.Id_Tee5 != null)
                                        {
                                            SingleData.Singleton.Instance.RevitData.Transaction.Start();
                                            ElementTransformUtils.MoveElement(SingleData.Singleton.Instance.RevitData.Document, cls_ThoatNuoc.Id_Tee5, new XYZ(0.001 / 304.8, 0.001 / 304.8, 0));
                                            SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                                        }
                                    }
                                    
                                }
                            }
                            catch (Exception ex)
                            {
                                SingleData.Singleton.Instance.RevitData.Transaction.RollBack();
                                System.Windows.MessageBox.Show(ex.Message + "\n" + "Contact: " + cls_Contact.sdt + " or Email: " + cls_Contact.email);
                                return;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }

        }
    }
}
