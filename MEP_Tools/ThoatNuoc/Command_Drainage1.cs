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
    public class Command_Drainage1 : WPFData, IExternalCommand
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
            }
            return Result.Succeeded;
        }
        public void Run()
        {
            cls_ThoatNuoc.Id_Tee1 = null;

            Reference r_nhanh = null;
            Reference r_chinh = null;
            try
            {
                r_nhanh = SingleData.Singleton.Instance.RevitData.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Please choose branch pipe or fixtures");
                if (r_nhanh != null)
                {
                    try
                    {
                        r_chinh = SingleData.Singleton.Instance.RevitData.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Please choose main pipe");
                        if (r_chinh != null)
                        {
                            try
                            {
                                SingleData.Singleton.Instance.RevitData.Transaction.Start();
                                Func_ThoatNuoc F = new Func_ThoatNuoc();
                                #region 'Get Value'
                                Element e = SingleData.Singleton.Instance.RevitData.Document.GetElement(r_nhanh);
                                Pipe p1 = null;
                                Pipe p2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(r_chinh) as Pipe; //chinh
                                LocationCurve lc = p2.Location as LocationCurve;
                                XYZ stPoint = lc.Curve.GetEndPoint(0);
                                XYZ edPoint = lc.Curve.GetEndPoint(1);

                                if (e is Pipe)
                                {
                                    p1 = e as Pipe; // phu
                                }
                                else if (e is FamilyInstance)
                                {
                                    Connector ConnectorFromFixtures = F.GetConnector_Fixtures(e as FamilyInstance);
                                    var dir = ConnectorFromFixtures.CoordinateSystem.BasisZ;
                                    if (dir.IsAlmostEqualTo(XYZ.BasisZ) || dir.IsAlmostEqualTo(- XYZ.BasisZ))
                                    {
                                        XYZ point_DrawPipe = F.GetIntersec(stPoint, edPoint, ConnectorFromFixtures);
                                        p1 = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, p2.PipeType.Id, p2.ReferenceLevel.Id, ConnectorFromFixtures, new XYZ(ConnectorFromFixtures.Origin.X, ConnectorFromFixtures.Origin.Y, point_DrawPipe.Z));
                                    }
                                    else if (dir.IsAlmostEqualTo(XYZ.BasisX) || dir.IsAlmostEqualTo(-XYZ.BasisX) || dir.IsAlmostEqualTo(-XYZ.BasisY) || dir.IsAlmostEqualTo(-XYZ.BasisY))
                                    {
                                        Pipe p0 = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, p2.PipeType.Id, p2.ReferenceLevel.Id, ConnectorFromFixtures, ConnectorFromFixtures.Origin + dir * ConnectorFromFixtures.Radius * 3);
                                        //Connector ConnectorPipe0 = F.GetConnecterNotConnected(p0);
                                        Connector ConnectorPipe0 = F.GetConnecterPipeNotConnected(p0);
                                        XYZ point_DrawPipe = F.GetIntersec(stPoint, edPoint, ConnectorPipe0);
                                        p1 = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, p2.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId() , p2.PipeType.Id, p2.ReferenceLevel.Id, ConnectorPipe0.Origin, new XYZ(ConnectorPipe0.Origin.X, ConnectorPipe0.Origin.Y, point_DrawPipe.Z));
                                        F.CopyParameters(p0, p1);
                                        SingleData.Singleton.Instance.RevitData.Document.Create.NewElbowFitting(ConnectorPipe0, F.GetConnectorFromPoint(p1, ConnectorPipe0.Origin));
                                    }
                                    
                                }   
                                if (p1 != null)
                                {
                                    ElementId _levelId = p1.ReferenceLevel.Id;
                                    PipeType _pipeType = p1.PipeType;
                                    ElementId _pipeSystemType = p1.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();
                                    Connector Conn_Origin = F.GetConnector(p1);

                                    #endregion
                                    #region 'Get Distance and Draw pipe and create fitting' 
                                    #region 'Get Distance and point'
                                    XYZ Intersec = F.CheckIntersec(stPoint, edPoint, F.GetIntersec1(stPoint, edPoint, Conn_Origin, p1));
                                    if (Intersec != null)
                                    {
                                        XYZ diem1 = new XYZ(Intersec.X, Intersec.Y, 0);
                                        XYZ diem2 = new XYZ(Conn_Origin.Origin.X, Conn_Origin.Origin.Y, 0);
                                        double kc = diem1.DistanceTo(diem2);

                                        Conn_Origin.Origin = new XYZ(Conn_Origin.Origin.X, Conn_Origin.Origin.Y, Intersec.Z + kc * cls_ThoatNuoc.Slope / 100);
                                        double d1 = p1.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
                                        double d2 = p2.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();

                                        XYZ splitpoint1 = (Intersec - Conn_Origin.Origin) * ((d1 * 1.2) / kc);
                                        XYZ splitpoint2 = (Conn_Origin.Origin - Intersec) * ((kc / 3.5) / kc);
                                        // point 2 elbow
                                        XYZ newpoint1 = Conn_Origin.Origin + splitpoint1;
                                        // point 45
                                        XYZ newpoint2 = Intersec + splitpoint2;
                                        #endregion
                                        #region 'Draw'
                                        //pipe 2 check
                                        XYZ diem11 = new XYZ(newpoint1.X, newpoint1.Y, 0);
                                        XYZ diem22 = new XYZ(Conn_Origin.Origin.X, Conn_Origin.Origin.Y, 0);
                                        double kc_100 = diem11.DistanceTo(diem22);
                                        Conn_Origin.Origin = new XYZ(Conn_Origin.Origin.X, Conn_Origin.Origin.Y, newpoint1.Z + kc_100);
                                        Pipe p_new_0 = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, _pipeSystemType, _pipeType.Id, _levelId, Conn_Origin.Origin, newpoint1);
                                        F.CopyParameters(p1, p_new_0);
                                        // pipe ngang
                                        Pipe p_new_1 = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, _pipeSystemType, _pipeType.Id, _levelId, newpoint1, newpoint2);
                                        F.CopyParameters(p1, p_new_1);
                                        // pipe 45
                                        Pipe p_new_2 = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, _pipeSystemType, _pipeType.Id, _levelId, newpoint2, Intersec);
                                        F.CopyParameters(p1, p_new_2);
                                        #endregion

                                        #region 'Tao Elbow + Tee'
                                        FamilyInstance Fitting0 = SingleData.Singleton.Instance.RevitData.Document.Create.NewElbowFitting(Conn_Origin, F.GetConnectorDifferentPoint(p_new_0, newpoint1));
                                        FamilyInstance Fitting1 = SingleData.Singleton.Instance.RevitData.Document.Create.NewElbowFitting(F.GetConnectorFromPoint(p_new_1, newpoint1), F.GetConnectorFromPoint(p_new_0, newpoint1));
                                        double distance = p_new_2.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                                        F.Case1(Intersec, stPoint, edPoint, _pipeType.Id, _levelId, p2, distance, F.GetConnectorDifferentPoint(p_new_2, newpoint2).Origin, F.GetConnectorDifferentPoint(p_new_2, newpoint2), p_new_0, p_new_1, p_new_2, F.GetConnectorFromPoint(p_new_2, newpoint2), newpoint2);

                                        #endregion
                                        SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                                        if (cls_ThoatNuoc.Id_Tee1 != null)
                                        {
                                            SingleData.Singleton.Instance.RevitData.Transaction.Start();
                                            ElementTransformUtils.MoveElement(SingleData.Singleton.Instance.RevitData.Document, cls_ThoatNuoc.Id_Tee1, new XYZ(0.001 / 304.8, 0.001 / 304.8, 0));
                                        }
                                        #endregion
                                    }
                                }                             
                                SingleData.Singleton.Instance.RevitData.Transaction.Commit();
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
