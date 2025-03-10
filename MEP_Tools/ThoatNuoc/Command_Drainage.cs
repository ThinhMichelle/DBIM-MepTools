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
    public class Command_Drainage : WPFData, IExternalCommand
    {
        public ICommand Pick_Case1 { get; set; }
        public ICommand Pick_Case2 { get; set; }
        public ICommand Pick_Case3 { get; set; }
        public ICommand Pick_Case4 { get; set; }
        public ICommand Pick_Case5 { get; set; }
        public ICommand Cancel { get; set; }
        public Command_Drainage()
        {
            Pick_Case1 = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                if (cls_ThoatNuoc.Check_Error == "No")
                {
                    p.Hide();
                    SingleData.Singleton.Instance.RevitData.Transaction.Start();
                    IList<Reference> r = null;
                    try
                    {
                        r = SingleData.Singleton.Instance.RevitData.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);
                        if (r != null)
                        {
                            MEP_Tools.ThoatNuoc.Func_ThoatNuoc F = new MEP_Tools.ThoatNuoc.Func_ThoatNuoc();
                            #region 'Get Value'
                            Pipe p1 = F.GetElement1(r, SingleData.Singleton.Instance.RevitData.Document) as Pipe; //phu
                            Pipe p2 = F.GetElement2(r, SingleData.Singleton.Instance.RevitData.Document) as Pipe; //chinh

                            ElementId levelId = p1.ReferenceLevel.Id;
                            PipeType _pipeType = p1.PipeType;
                            ElementId _pipeSystemType = p1.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();
                            Connector Conn_Origin = F.GetConnector(p1);
                            LocationCurve lc = p2.Location as LocationCurve;
                            XYZ stPoint = lc.Curve.GetEndPoint(0);
                            XYZ edPoint = lc.Curve.GetEndPoint(1);
                            #endregion
                            #region 'Get Distance and Draw pipe and create fitting' 
                            #region 'Get Distance and point'
                            XYZ Intersec = F.GetIntersec(stPoint, edPoint, Conn_Origin);
                            XYZ diem1 = new XYZ(Intersec.X, Intersec.Y, 0);
                            XYZ diem2 = new XYZ(Conn_Origin.Origin.X, Conn_Origin.Origin.Y, 0);
                            double kc = diem1.DistanceTo(diem2);
                            Conn_Origin.Origin = new XYZ(Conn_Origin.Origin.X, Conn_Origin.Origin.Y, Intersec.Z + kc * cls_ThoatNuoc.Slope / 100);
                            double d1 = p1.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
                            double d = p2.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
                            XYZ splitpoint1 = (Intersec - Conn_Origin.Origin) * ((d1 * 1.2) / kc);
                            XYZ splitpoint2 = (Conn_Origin.Origin - Intersec) * ((kc / 3.5) / kc);
                            // point 2 elbow
                            XYZ newpoint1 = Conn_Origin.Origin + splitpoint1;
                            // point 45
                            XYZ newpoint2 = Intersec + splitpoint2;
                            #endregion
                            #region 'Draw'
                            //pipe 2 check
                            Pipe p_new_0 = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, _pipeSystemType, _pipeType.Id, levelId, Conn_Origin.Origin, newpoint1);
                            F.CopyParameters(p1, p_new_0);
                            // pipe ngang
                            Pipe p_new_1 = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, _pipeSystemType, _pipeType.Id, levelId, newpoint1, newpoint2);
                            F.CopyParameters(p1, p_new_1);
                            // pipe 45
                            Pipe p_new_2 = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, _pipeSystemType, _pipeType.Id, levelId, newpoint2, Intersec);
                            F.CopyParameters(p1, p_new_2);
                            #endregion
                            #region 'Rotate' 
                            double Corner = Math.PI / 4;
                            XYZ point_TamXoay = new XYZ(newpoint1.X, newpoint1.Y, newpoint1.Z);
                            XYZ u = new XYZ(edPoint.X - stPoint.X, edPoint.Y - stPoint.Y, newpoint1.Z).Normalize();
                            u = new XYZ(Math.Round(u.X), Math.Round(u.Y), 0);
                            //if (Math.Round(u.X) == Math.Round(u.X) && Math.Round(u.X) == Math.Round(u.X) && Math.Round(u.X) == 0)
                            //{
                                //SingleData.Singleton.Instance.RevitData.Transaction.RollBack();
                            //}
                            //else
                            //{
                                Line axis2 = Line.CreateBound(point_TamXoay, point_TamXoay + u);
                                ElementTransformUtils.RotateElement(SingleData.Singleton.Instance.RevitData.Document, p_new_0.Id, axis2, -Corner);
                                Connector Con = F.GetConnectorDifferentPoint(p_new_0, newpoint1);
                                Connector Conn = F.GetConnectorFromPoint(p_new_0, newpoint1);
                                if (Con.Origin.Z < Conn.Origin.Z)
                                {
                                    ElementTransformUtils.RotateElement(SingleData.Singleton.Instance.RevitData.Document, p_new_0.Id, axis2, Math.PI / 2);
                                }
                                #endregion
                                #region 'Tao Elbow + Tee'
                                Conn_Origin.Origin = new XYZ(Conn_Origin.Origin.X, Conn_Origin.Origin.Y, Con.Origin.Z + d);
                                FamilyInstance Fitting0 = SingleData.Singleton.Instance.RevitData.Document.Create.NewElbowFitting(Conn_Origin, F.GetConnectorDifferentPoint(p_new_0, newpoint1));
                                FamilyInstance Fitting1 = SingleData.Singleton.Instance.RevitData.Document.Create.NewElbowFitting(F.GetConnectorFromPoint(p_new_1, newpoint1), F.GetConnectorFromPoint(p_new_0, newpoint1));
                                double len = p_new_2.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                                //F.Case1(Intersec, stPoint, edPoint, _pipeType.Id, levelId, p2, len, F.GetConnectorDifferentPoint(p_new_2, newpoint2).Origin, F.GetConnectorDifferentPoint(p_new_2, newpoint2), p_new_1, p_new_2, newpoint2);

                                #endregion
                                SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                            //}
                            #endregion

                            //return Result.Succeeded;
                        }
                        else
                        {
                            //return Result.Failed;
                        }
                    }
                    catch (Exception ex)
                    {
                       
                        SingleData.Singleton.Instance.RevitData.Transaction.RollBack();
                        MessageBox.Show("Not applicable in this case !");
                        //return Result.Failed;
                    }
                    p.ShowDialog();
                }
                cls_ThoatNuoc.Check_Error = "";
            });
            Pick_Case2 = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                if (cls_ThoatNuoc.Check_Error == "No")
                {
                    p.Hide();
                    SingleData.Singleton.Instance.RevitData.Transaction.Start();
                    IList<Reference> r = null;
                    try
                    {
                        r = SingleData.Singleton.Instance.RevitData.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);
                        if (r != null)
                        {
                            MEP_Tools.ThoatNuoc.Func_ThoatNuoc F = new MEP_Tools.ThoatNuoc.Func_ThoatNuoc();
                            #region 'Get Value'
                            Element Ele1 = F.GetElement1(r, SingleData.Singleton.Instance.RevitData.Document); // phu
                            Element Ele2 = F.GetElement2(r, SingleData.Singleton.Instance.RevitData.Document); // chinh
                            Pipe p1 = Ele1 as Pipe;
                            Pipe p2 = Ele2 as Pipe;
                            ElementId levelId = p1.ReferenceLevel.Id;
                            PipeType _pipeType = p1.PipeType;
                            ElementId _pipeSystemType = p1.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();

                            Connector Conn_Origin = F.GetConnector(p1);

                            LocationCurve lc = p2.Location as LocationCurve;
                            XYZ stPoint = lc.Curve.GetEndPoint(0);
                            XYZ edPoint = lc.Curve.GetEndPoint(1);
                            //XYZ u = new XYZ(edPoint.X - stPoint.X, edPoint.Y - stPoint.Y, 0).Normalize();
                            //u = new XYZ(Math.Round(u.X), Math.Round(u.Y), 0);
                            //if (Math.Round(u.X) == Math.Round(u.X) && Math.Round(u.X) == Math.Round(u.X) && Math.Round(u.X) == 0)
                            //{
                            //    SingleData.Singleton.Instance.RevitData.Transaction.RollBack();
                            //}
                            //else
                            //{
                            #endregion
                            #region 'Get Distance and point'
                            XYZ Intersec = F.GetIntersec(stPoint, edPoint, Conn_Origin);
                            XYZ diem1 = new XYZ(Intersec.X, Intersec.Y, 0);
                            XYZ diem2 = new XYZ(Conn_Origin.Origin.X, Conn_Origin.Origin.Y, 0);
                            double kc = diem1.DistanceTo(diem2);
                            Conn_Origin.Origin = new XYZ(Conn_Origin.Origin.X, Conn_Origin.Origin.Y, Intersec.Z + kc * cls_ThoatNuoc.Slope / 100);
                            #endregion
                            #region 'CT chinh'
                            Pipe p_new = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, _pipeSystemType, _pipeType.Id, levelId, Conn_Origin.Origin, Intersec);
                            F.CopyParameters(p1, p_new);
                            double len = p_new.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                            F.Case2(p2, stPoint, edPoint, Intersec, len, _pipeSystemType, _pipeType.Id, levelId, p_new, F.GetConnectorFromPoint(p_new, Intersec), Conn_Origin);
                            #endregion
                            SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                            //}

                            //return Result.Succeeded;
                        }
                        else
                        {
                            //return Result.Failed;
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.ToString());
                        SingleData.Singleton.Instance.RevitData.Transaction.RollBack();
                        MessageBox.Show("Not applicable in this case !");
                        //return Result.Failed;
                    }
                    p.ShowDialog();
                }
               
                cls_ThoatNuoc.Check_Error = "";
            });
            Pick_Case3 = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                p.Hide();
                SingleData.Singleton.Instance.RevitData.Transaction.Start();
                IList<Reference> r = null;
                try
                {
                    r = SingleData.Singleton.Instance.RevitData.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);
                    if (r != null)
                    {
                        MEP_Tools.ThoatNuoc.Func_ThoatNuoc F = new MEP_Tools.ThoatNuoc.Func_ThoatNuoc();
                        #region 'Get Value'
                        Pipe p1 = F.GetElement1_Case3(r, SingleData.Singleton.Instance.RevitData.Document); // chinh
                        Pipe p2 = F.GetElement2_Case3(r, SingleData.Singleton.Instance.RevitData.Document); // phu

                        ElementId levelId = p1.ReferenceLevel.Id;
                        PipeType _pipeType = p1.PipeType;
                        ElementId _pipeSystemType = p1.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();

                        Connector Conn_Bottom = F.GetConnector(p2);

                        LocationCurve lc = p1.Location as LocationCurve;
                        XYZ stPoint = lc.Curve.GetEndPoint(0);
                        XYZ edPoint = lc.Curve.GetEndPoint(1);

                        XYZ u1 = new XYZ(edPoint.X - stPoint.X, edPoint.Y - stPoint.Y, 0).Normalize();
                        //u1 = new XYZ(Math.Round(u1.X), Math.Round(u1.Y), 0);
                        LocationCurve lc2 = p2.Location as LocationCurve;
                        XYZ stPoint2 = lc2.Curve.GetEndPoint(0);
                        XYZ edPoint2 = lc2.Curve.GetEndPoint(1);
                        XYZ u2 = new XYZ(edPoint2.X - stPoint2.X, edPoint2.Y - stPoint2.Y, 0).Normalize();
                        //u2 = new XYZ(Math.Round(u2.X), Math.Round(u2.Y), 0);
                        if (Math.Round(u1.X,5) == Math.Round(u2.X, 5) && Math.Round(u1.Y, 5) == Math.Round(u2.Y, 5))
                        {

                            #endregion
                            #region 'Get Distance and point'
                            XYZ Intersec = F.GetIntersec(stPoint, edPoint, Conn_Bottom);
                            XYZ diem1 = new XYZ(Intersec.X, Intersec.Y, 0);
                            XYZ diem2 = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, 0);
                            double kc = diem1.DistanceTo(diem2);
                            #endregion
                            #region 'CT chinh'
                            Pipe p_new = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, p2.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId(), p2.PipeType.Id, p2.ReferenceLevel.Id, Conn_Bottom.Origin, Intersec);
                            F.CopyParameters(p2, p_new);
                            double len = p_new.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                            F.Case3(p1, stPoint, edPoint, Intersec, len, _pipeSystemType, _pipeType.Id, levelId, p_new, F.GetConnectorFromPoint(p_new, Intersec), Conn_Bottom);
                            #endregion
                            SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                            //return Result.Succeeded;
                        }
                        else
                        {
                            SingleData.Singleton.Instance.RevitData.Transaction.RollBack();
                        }

                    }
                    else
                    {
                        //return Result.Failed;
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                    SingleData.Singleton.Instance.RevitData.Transaction.RollBack();
                    MessageBox.Show("Not applicable in this case !");
                    //return Result.Failed;
                }
                p.ShowDialog();
                
                
                cls_ThoatNuoc.Check_Error = "";
            });
            Pick_Case4 = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                
                    p.Hide();
                    SingleData.Singleton.Instance.RevitData.Transaction.Start();
                    IList<Reference> r = null;
                    try
                    {

                        r = SingleData.Singleton.Instance.RevitData.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);
                        if (r != null)
                        {
                            MEP_Tools.ThoatNuoc.Func_ThoatNuoc F = new MEP_Tools.ThoatNuoc.Func_ThoatNuoc();
                            #region 'Get Value'
                            Pipe p1 = F.GetElement1_Case3(r, SingleData.Singleton.Instance.RevitData.Document); // chinh
                            Pipe p2 = F.GetElement2_Case3(r, SingleData.Singleton.Instance.RevitData.Document); // phu

                            ElementId levelId = p1.ReferenceLevel.Id;
                            PipeType _pipeType = p1.PipeType;
                            ElementId _pipeSystemType = p1.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();

                            Connector Conn_Bottom = F.GetConnector(p2);

                            LocationCurve lc = p1.Location as LocationCurve;
                            XYZ stPoint = lc.Curve.GetEndPoint(0);
                            XYZ edPoint = lc.Curve.GetEndPoint(1);
                            LocationCurve lc2 = p2.Location as LocationCurve;
                            XYZ stPoint2 = lc2.Curve.GetEndPoint(0);
                            XYZ edPoint2 = lc2.Curve.GetEndPoint(1);
                        //XYZ u1 = null;
                        //u1 = new XYZ(edPoint.X - stPoint.X, edPoint.Y - stPoint.Y, 0).Normalize();
                        //XYZ u2 = null;
                        //u2 = new XYZ(edPoint2.X - stPoint2.X, edPoint2.Y - stPoint2.Y, 0).Normalize();
                        //if (Math.Round( u1.X*u2.X + u1.Y*u2.Y,5) == 0)
                        //{
                            XYZ u = null;
                            if (edPoint2.X - stPoint2.X > 0)
                            {
                                u = new XYZ(edPoint2.X - stPoint2.X, edPoint2.Y - stPoint2.Y, edPoint2.Z - stPoint2.Z).Normalize();
                            }
                            else
                            {
                                u = new XYZ(-edPoint2.X + stPoint2.X, -edPoint2.Y + stPoint2.Y, -edPoint2.Z + stPoint2.Z).Normalize();
                            }
                            #endregion
                            #region 'Get Distance and point'
                            XYZ Intersec = F.GetIntersec(stPoint, edPoint, Conn_Bottom);
                            XYZ diem1 = new XYZ(Intersec.X, Intersec.Y, 0);
                            XYZ diem2 = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, 0);
                            double kc = diem1.DistanceTo(diem2);

                            XYZ point1 = Conn_Bottom.Origin + kc * u;
                            XYZ point2 = Conn_Bottom.Origin - kc * u;

                            if (point1.Z > point2.Z)
                            {
                                Conn_Bottom.Origin = point2;
                            }
                            else
                            {
                                Conn_Bottom.Origin = point1;
                            }
                            //Conn_Bottom.Origin = new XYZ(Intersec.X, Intersec.Y, Conn_Bottom.Origin.Z);
                            #endregion
                            #region 'CT chinh'
                            Pipe p_new = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, p2.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId(), p2.PipeType.Id, p2.ReferenceLevel.Id, Conn_Bottom.Origin, Intersec);
                            F.CopyParameters(p2, p_new);
                            double len = p_new.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                            F.Case4(p1, p2, stPoint, edPoint, Intersec, len, _pipeSystemType, _pipeType.Id, levelId, p_new, F.GetConnectorFromPoint(p_new, Intersec), Conn_Bottom);
                            #endregion
                            SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                        //return Result.Succeeded;
                        //}
                        //else
                        //{
                        //    SingleData.Singleton.Instance.RevitData.Transaction.RollBack();

                        //}
                        if (cls_ThoatNuoc.Id_move != null)
                        {
                            SingleData.Singleton.Instance.RevitData.Transaction.Start();
                            ElementTransformUtils.MoveElement(SingleData.Singleton.Instance.RevitData.Document, cls_ThoatNuoc.Id_move, cls_ThoatNuoc.u_move*cls_ThoatNuoc.kc_move);
                            SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                        }

                    }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.ToString());
                        SingleData.Singleton.Instance.RevitData.Transaction.RollBack();
                    //MessageBox.Show("Not applicable in this case !");
                    //return Result.Failed;
                    }
                    p.ShowDialog();
                cls_ThoatNuoc.Check_Error = "";
            });
            Pick_Case5 = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                if (cls_ThoatNuoc.Check_Error == "No")
                {
                    p.Hide();
                    SingleData.Singleton.Instance.RevitData.Transaction.Start();
                    IList<Reference> r = null;
                    try
                    {

                        r = SingleData.Singleton.Instance.RevitData.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);
                        if (r != null)
                        {
                            MEP_Tools.ThoatNuoc.Func_ThoatNuoc F = new MEP_Tools.ThoatNuoc.Func_ThoatNuoc();
                            #region 'Get Value'
                            Pipe p1 = F.GetElement1_Case5(r, SingleData.Singleton.Instance.RevitData.Document); //chinh
                            Pipe p2 = F.GetElement2_Case5(r, SingleData.Singleton.Instance.RevitData.Document); //phu

                            double d1 = p1.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
                            double d2 = p2.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
                            if (d1 >= d2)
                            {
                                ElementId levelId = p1.ReferenceLevel.Id;
                                PipeType _pipeType = p1.PipeType;
                                ElementId _pipeSystemType = p1.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();

                                Connector Conn_Bottom = F.GetConnector(p2);

                                LocationCurve lc = p1.Location as LocationCurve;
                                XYZ stPoint = lc.Curve.GetEndPoint(0);
                                XYZ edPoint = lc.Curve.GetEndPoint(1);

                                #endregion
                                #region 'Get Distance and point'
                                XYZ Intersec = F.GetIntersec(stPoint, edPoint, Conn_Bottom);
                                XYZ diem1 = new XYZ(Intersec.X, Intersec.Y, 0);
                                XYZ diem2 = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, 0);
                                double kc = diem1.DistanceTo(diem2);
                                Conn_Bottom.Origin = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, Intersec.Z + kc * cls_ThoatNuoc.Slope / 100);
                                #endregion
                                #region 'CT chinh'
                                Pipe p_new = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, p2.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId(), p2.PipeType.Id, p2.ReferenceLevel.Id, Conn_Bottom.Origin, Intersec);
                                F.CopyParameters(p2, p_new);
                                double len = p_new.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                                F.Case5(p1, p_new, stPoint, edPoint, Intersec, len, _pipeSystemType, _pipeType.Id, levelId, F.GetConnectorFromPoint(p_new, Intersec), Conn_Bottom);
                                #endregion
                                SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                                //return Result.Succeeded;
                            }
                            else
                            {
                                SingleData.Singleton.Instance.RevitData.Transaction.RollBack();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.ToString());
                        SingleData.Singleton.Instance.RevitData.Transaction.RollBack();
                        MessageBox.Show("Not applicable in this case !");
                        //return Result.Failed;
                    }
                    p.ShowDialog();
                }
               
                cls_ThoatNuoc.Check_Error = "";
            });

            Cancel = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                p.Close();
            });
        }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            SingleData.Singleton.Instance = new SingleData.Singleton();
            SingleData.Singleton.Instance.RevitData.UIApplication = commandData.Application;
            if (cls_Reg.Login == "Login")
            {
                SingleData.Singleton.Instance.WFData.InputWindow_ThoatNuoc.ShowDialog();
            }
            return Result.Succeeded;
        }

    }
}
