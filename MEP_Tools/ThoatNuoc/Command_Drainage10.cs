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
    public class Command_Drainage10 : WPFData, IExternalCommand
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

            Reference r_nhanh = null;
            Reference r_chinh = null;
            cls_ThoatNuoc.Id_Tee10 = null;

            try
            {
                r_nhanh = SingleData.Singleton.Instance.RevitData.Selection.PickObject(ObjectType.Element, "Please choose branch pipe");
                if (r_nhanh != null)
                {
                    Func_ThoatNuoc F = new Func_ThoatNuoc();
                    try
                    {
                        r_chinh = SingleData.Singleton.Instance.RevitData.Selection.PickObject(ObjectType.Element, "Please choose main pipe");
                        try
                        {
                            if (r_chinh != null)
                            {

                                SingleData.Singleton.Instance.RevitData.Transaction.Start();
                                Pipe pipe_nhanh = SingleData.Singleton.Instance.RevitData.Document.GetElement(r_nhanh) as Pipe;
                                Connector Conn_Bottom_Nhanh = F.GetConnector(pipe_nhanh);

                                Pipe pipe_chinh = SingleData.Singleton.Instance.RevitData.Document.GetElement(r_chinh) as Pipe;

                                ElementId _levelId_1 = pipe_chinh.ReferenceLevel.Id;
                                PipeType _pipeType_1 = pipe_chinh.PipeType;
                                ElementId _pipeSystemType_1 = pipe_chinh.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();

                                LocationCurve lc_chinh = pipe_chinh.Location as LocationCurve;
                                XYZ stPoint_chinh = lc_chinh.Curve.GetEndPoint(0);
                                XYZ edPoint_chinh = lc_chinh.Curve.GetEndPoint(1);

                                LocationCurve lc_nhanh = pipe_nhanh.Location as LocationCurve;
                                XYZ stPoint_nhanh = lc_nhanh.Curve.GetEndPoint(0);
                                XYZ edPoint_nhanh = lc_nhanh.Curve.GetEndPoint(1);


                                XYZ Intersec = F.CheckIntersec(stPoint_chinh, edPoint_chinh, F.GetIntersec1(stPoint_chinh, edPoint_chinh, Conn_Bottom_Nhanh, pipe_nhanh));

                                if (Intersec != null)
                                {
                                    if (Conn_Bottom_Nhanh.Origin.Z > Intersec.Z)
                                    {
                                        //double diameter = pipe_nhanh.Diameter * 2.5;
                                        //XYZ diem11 = new XYZ(Conn_Bottom_Nhanh.Origin.X, Conn_Bottom_Nhanh.Origin.Y, 0);
                                        //XYZ diem22 = new XYZ(Intersec.X, Intersec.Y, 0);
                                        //double kc0 = diem11.DistanceTo(diem22);
                                        //double kc_keolaigan = kc0 - diameter;
                                        //XYZ u_keolaigan = null;
                                        //if ((stPoint_nhanh - edPoint_nhanh).Normalize().Z < 0)
                                        //{
                                        //    u_keolaigan = stPoint_nhanh - edPoint_nhanh;
                                        //}
                                        //else
                                        //{
                                        //    u_keolaigan = -stPoint_nhanh + edPoint_nhanh;
                                        //}

                                        //Conn_Bottom_Nhanh.Origin = Conn_Bottom_Nhanh.Origin + kc_keolaigan * u_keolaigan.Normalize();


                                        XYZ diem11 = new XYZ(Conn_Bottom_Nhanh.Origin.X, Conn_Bottom_Nhanh.Origin.Y, 0);
                                        XYZ diem22 = new XYZ(Intersec.X, Intersec.Y, 0);
                                        double kc0 = diem11.DistanceTo(diem22);


                                        XYZ diem1 = new XYZ(Conn_Bottom_Nhanh.Origin.X, Conn_Bottom_Nhanh.Origin.Y, Conn_Bottom_Nhanh.Origin.Z);
                                        XYZ diem2 = new XYZ(Intersec.X, Intersec.Y, Conn_Bottom_Nhanh.Origin.Z - kc0 * cls_ThoatNuoc.Slope / 100);
                                        double kc = diem1.DistanceTo(diem2);

                                        XYZ u = null;
                                        XYZ u1 = (edPoint_chinh - stPoint_chinh).Normalize();
                                        XYZ u2 = -(edPoint_chinh - stPoint_chinh).Normalize();

                                        XYZ pointslope1 = new XYZ(diem2.X, diem2.Y, diem1.Z - kc * cls_ThoatNuoc.Slope / 100);
                                        XYZ pointslope2 = null;
                                        XYZ pointtest1 = pointslope1 + u1 * kc;
                                        XYZ pointtest2 = pointslope1 + u2 * kc;

                                        if (pointtest1.Z < pointtest2.Z)
                                        {
                                            pointslope2 = pointtest1;
                                            u = u1;
                                        }
                                        else
                                        {
                                            pointslope2 = pointtest2;
                                            u = u2;
                                        }

                                        double kcslope = diem1.DistanceTo(pointslope2);


                                        XYZ point_check45 = new XYZ(pointslope2.X, pointslope2.Y, diem1.Z - kcslope * cls_ThoatNuoc.Slope / 100);
                                        double kcmove = point_check45.DistanceTo(new XYZ(Conn_Bottom_Nhanh.Origin.X, Conn_Bottom_Nhanh.Origin.Y, point_check45.Z));
                                        point_check45 = new XYZ(point_check45.X, point_check45.Y, Conn_Bottom_Nhanh.Origin.Z - kcmove * cls_ThoatNuoc.Slope / 100);

                                        Pipe pcheck = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, pipe_nhanh.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId(), pipe_nhanh.PipeType.Id, pipe_nhanh.ReferenceLevel.Id, Conn_Bottom_Nhanh.Origin, point_check45);
                                        F.CopyParameters(pipe_nhanh, pcheck);

                                        Connector Conn_Bottom_check = F.GetConnector(pcheck);
                                        Connector Conn_Top_check = F.GetConnectorDifferentPoint(pcheck, Conn_Bottom_check.Origin);

                                        FamilyInstance Fitting0 = SingleData.Singleton.Instance.RevitData.Document.Create.NewElbowFitting(Conn_Top_check, Conn_Bottom_Nhanh);
                                        XYZ point_along = Conn_Bottom_check.Origin + u * kc * 1.5;

                                        Pipe palong = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, pipe_nhanh.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId(), pipe_nhanh.PipeType.Id, pipe_nhanh.ReferenceLevel.Id, Conn_Bottom_check.Origin, point_along);
                                        F.CopyParameters(pipe_nhanh, palong);

                                        Connector Conn_Bottom_along = F.GetConnector(palong);
                                        Connector Conn_Top_along = F.GetConnectorDifferentPoint(palong, Conn_Bottom_along.Origin);
                                        FamilyInstance Fitting1 = SingleData.Singleton.Instance.RevitData.Document.Create.NewElbowFitting(Conn_Bottom_check, Conn_Top_along);
                                        XYZ Intersec_along = F.CheckIntersec(stPoint_chinh, edPoint_chinh, F.GetIntersec(stPoint_chinh, edPoint_chinh, Conn_Bottom_along));
                                        if (Intersec_along != null)
                                        {
                                            Pipe p_new = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, pipe_nhanh.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId(), pipe_nhanh.PipeType.Id, pipe_nhanh.ReferenceLevel.Id, Conn_Bottom_along.Origin, Intersec_along);
                                            F.CopyParameters(pipe_nhanh, p_new);
                                            double distance = Conn_Bottom_along.Origin.DistanceTo(Intersec_along);
                                            Connector Conn_p_new_Intersec = F.GetConnectorFromPoint(p_new, Intersec_along);
                                            Connector Conn_p_new_Top = F.GetConnectorDifferentPoint(p_new, Conn_p_new_Intersec.Origin);
                                            F.Case10(pipe_chinh, stPoint_chinh, edPoint_chinh, Intersec_along, distance, _pipeSystemType_1, _pipeType_1.Id, _levelId_1, p_new, Conn_p_new_Intersec, Conn_p_new_Top, Conn_Bottom_along);
                                        }

                                    }
                                }
                                SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                                if (cls_ThoatNuoc.Id_Tee10 != null)
                                {

                                    SingleData.Singleton.Instance.RevitData.Transaction.Start();
                                    ElementTransformUtils.MoveElement(SingleData.Singleton.Instance.RevitData.Document, cls_ThoatNuoc.Id_Tee10, new XYZ(0.001 / 304.8, 0.001 / 304.8, 0));
                                    SingleData.Singleton.Instance.RevitData.Transaction.Commit();
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
