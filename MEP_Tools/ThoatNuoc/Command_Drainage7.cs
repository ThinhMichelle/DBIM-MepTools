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
    public class Command_Drainage7 : WPFData, IExternalCommand
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

            IList<Reference> r_nhanh = null;
            Reference r_chinh = null;
            cls_ThoatNuoc.list_Id_Tee7.Clear();
            cls_ThoatNuoc.list_Id_error.Clear();
            cls_ThoatNuoc.Id_pipe_new_1 = null;
            cls_ThoatNuoc.Id_pipe_new_2 = null;

            try
            {
                r_nhanh = SingleData.Singleton.Instance.RevitData.Selection.PickObjects(ObjectType.Element, "Please choose branch pipes");
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
                                List<Reference> list_r = new List<Reference>();
                                List<XYZ> list_point = new List<XYZ>();
                                List<double> list_kc = new List<double>();
                                foreach (Reference item in r_nhanh)
                                {
                                    Pipe pipe_nhanh_sx = SingleData.Singleton.Instance.RevitData.Document.GetElement(item) as Pipe;
                                    Connector Conn_Bottom_sx = F.GetConnector(pipe_nhanh_sx);
                                    list_point.Add(Conn_Bottom_sx.Origin);
                                    list_r.Add(item);
                                }

                                Pipe pipe_chinh_sx = SingleData.Singleton.Instance.RevitData.Document.GetElement(r_chinh) as Pipe;
                                Connector Conn_main = F.GetConnector(pipe_chinh_sx);
                                XYZ main = Conn_main.Origin;
                                foreach (var item in list_point)
                                {
                                    list_kc.Add(item.DistanceTo(main));
                                }
                                int i, j, min;
                                for (i = 0; i < list_kc.Count - 1; i++)
                                {
                                    min = i;
                                    for (j = i + 1; j < list_kc.Count; j++)
                                    {
                                        if (list_kc[j] < list_kc[min])
                                            min = j;
                                    }
                                    var tg = list_r[i];
                                    list_r[i] = list_r[min];
                                    list_r[min] = tg;
                                    var tgkc = list_kc[i];
                                    list_kc[i] = list_kc[min];
                                    list_kc[min] = tgkc;
                                }
                                for (i = 0; i < list_r.Count; i++)
                                {
                                    SingleData.Singleton.Instance.RevitData.Transaction.Start();
                                    Pipe pipe_nhanh = SingleData.Singleton.Instance.RevitData.Document.GetElement(list_r[i]) as Pipe;
                                    Connector Conn_Bottom = F.GetConnector(pipe_nhanh);
                                    if (cls_ThoatNuoc.Id_pipe_new_1 != null && cls_ThoatNuoc.Id_pipe_new_2 != null)
                                    {
                                        Pipe pipe_chinh_1 = SingleData.Singleton.Instance.RevitData.Document.GetElement(cls_ThoatNuoc.Id_pipe_new_1) as Pipe;

                                        ElementId _levelId_1 = pipe_chinh_1.ReferenceLevel.Id;
                                        PipeType _pipeType_1 = pipe_chinh_1.PipeType;
                                        ElementId _pipeSystemType_1 = pipe_chinh_1.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();

                                        LocationCurve lc_new_1 = pipe_chinh_1.Location as LocationCurve;
                                        XYZ stPoint_new_1 = lc_new_1.Curve.GetEndPoint(0);
                                        XYZ edPoint_new_1 = lc_new_1.Curve.GetEndPoint(1);

                                        XYZ Intersec = F.CheckIntersec(stPoint_new_1, edPoint_new_1, F.GetIntersec(stPoint_new_1, edPoint_new_1, Conn_Bottom));

                                        if (Intersec != null)
                                        {
                                            if (Conn_Bottom.Origin.Z > Intersec.Z)
                                            {
                                                XYZ diem1 = new XYZ(Intersec.X, Intersec.Y, 0);
                                                XYZ diem2 = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, 0);
                                                double kc = diem1.DistanceTo(diem2);
                                                double distance = Intersec.DistanceTo(new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, Intersec.Z + kc * cls_ThoatNuoc.Slope / 100));
                                                F.Case7(pipe_chinh_1, pipe_nhanh, stPoint_new_1, edPoint_new_1, Intersec, distance, _pipeSystemType_1, _pipeType_1.Id, _levelId_1, Conn_Bottom);
                                            }
                                            else
                                            {
                                                cls_ThoatNuoc.list_Id_error.Add(pipe_nhanh.Id);
                                            }
                                        }
                                        else
                                        {
                                            Pipe pipe_chinh_2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(cls_ThoatNuoc.Id_pipe_new_2) as Pipe;

                                            ElementId _levelId_2 = pipe_chinh_2.ReferenceLevel.Id;
                                            PipeType _pipeType_2 = pipe_chinh_2.PipeType;
                                            ElementId _pipeSystemType_2 = pipe_chinh_2.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();

                                            LocationCurve lc_new_2 = pipe_chinh_2.Location as LocationCurve;
                                            XYZ stPoint_new_2 = lc_new_1.Curve.GetEndPoint(0);
                                            XYZ edPoint_new_2 = lc_new_1.Curve.GetEndPoint(1);

                                            XYZ Intersec2 = F.CheckIntersec(stPoint_new_2, edPoint_new_2, F.GetIntersec(stPoint_new_2, edPoint_new_2, Conn_Bottom));

                                            if (Intersec2 != null)
                                            {
                                                if (Conn_Bottom.Origin.Z > Intersec2.Z)
                                                {
                                                    XYZ diem1 = new XYZ(Intersec2.X, Intersec2.Y, 0);
                                                    XYZ diem2 = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, 0);
                                                    double kc = diem1.DistanceTo(diem2);
                                                    double distance = Intersec2.DistanceTo(new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, Intersec2.Z + kc * cls_ThoatNuoc.Slope / 100));
                                                    F.Case7(pipe_chinh_2, pipe_nhanh, stPoint_new_2, edPoint_new_2, Intersec2, distance, _pipeSystemType_2, _pipeType_2.Id, _levelId_2, Conn_Bottom);
                                                }
                                                else
                                                {
                                                    cls_ThoatNuoc.list_Id_error.Add(pipe_nhanh.Id);
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {

                                        Pipe pipe_chinh = SingleData.Singleton.Instance.RevitData.Document.GetElement(r_chinh) as Pipe;

                                        ElementId _levelId = pipe_chinh.ReferenceLevel.Id;
                                        PipeType _pipeType = pipe_chinh.PipeType;
                                        ElementId _pipeSystemType = pipe_chinh.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();

                                        LocationCurve lc1 = pipe_chinh.Location as LocationCurve;
                                        XYZ stPoint = lc1.Curve.GetEndPoint(0);
                                        XYZ edPoint = lc1.Curve.GetEndPoint(1);

                                        XYZ Intersec = F.CheckIntersec(stPoint, edPoint, F.GetIntersec(stPoint, edPoint, Conn_Bottom));
                                        if (Intersec!=null)
                                        {
                                            if (Conn_Bottom.Origin.Z > Intersec.Z)
                                            {
                                                XYZ diem1 = new XYZ(Intersec.X, Intersec.Y, 0);
                                                XYZ diem2 = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, 0);
                                                double kc = diem1.DistanceTo(diem2);
                                                double distance = Intersec.DistanceTo(new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, Intersec.Z + kc * cls_ThoatNuoc.Slope / 100));
                                                F.Case7(pipe_chinh, pipe_nhanh, stPoint, edPoint, Intersec, distance, _pipeSystemType, _pipeType.Id, _levelId, Conn_Bottom);
                                            }
                                            else
                                            {
                                                cls_ThoatNuoc.list_Id_error.Add(pipe_nhanh.Id);
                                            }
                                        }
                                        
                                    }
                                    SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                                    #region 'Tao check'
                                    SingleData.Singleton.Instance.RevitData.Transaction.Start();
                                    Pipe p_check = SingleData.Singleton.Instance.RevitData.Document.GetElement(cls_ThoatNuoc.Id_pipe_new_case7) as Pipe;
                                    List<Connector> list_connector = F.GetConnectors(p_check);
                                    Connector Conn_Check = null;
                                    if (list_connector[0].Origin.DistanceTo(Conn_Bottom.Origin) < list_connector[1].Origin.DistanceTo(Conn_Bottom.Origin))
                                    {
                                        Conn_Check = list_connector[0];
                                    }
                                    else
                                    {
                                        Conn_Check = list_connector[1];
                                    }

                                    XYZ diem1check = new XYZ(Conn_Check.Origin.X, Conn_Check.Origin.Y, 0);
                                    XYZ diem2check = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, 0);
                                    double kccheck = diem1check.DistanceTo(diem2check);
                                    XYZ DiemMove = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, Conn_Check.Origin.Z + kccheck * cls_ThoatNuoc.Slope / 100);
                                    double kc_Move = Conn_Bottom.Origin.DistanceTo(DiemMove);
                                    XYZ axis = null;
                                    if (DiemMove.Z > Conn_Bottom.Origin.Z)
                                    {
                                        axis = new XYZ(0, 0, 1);
                                    }
                                    else
                                    {
                                        axis = -new XYZ(0, 0, 1);
                                    }
                                    ElementTransformUtils.MoveElement(SingleData.Singleton.Instance.RevitData.Document, pipe_nhanh.Id, kc_Move * axis);
                                    FamilyInstance Fitting0 = SingleData.Singleton.Instance.RevitData.Document.Create.NewElbowFitting(Conn_Check, Conn_Bottom);
                                    SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                                    #endregion
                                }
                                if (cls_ThoatNuoc.list_Id_Tee7.Count > 0)
                                {
                                    foreach (var item in cls_ThoatNuoc.list_Id_Tee7)
                                    {
                                        SingleData.Singleton.Instance.RevitData.Transaction.Start();
                                        ElementTransformUtils.MoveElement(SingleData.Singleton.Instance.RevitData.Document, item, new XYZ(0.001 / 304.8, 0.001 / 304.8, 0));
                                        SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                                    }

                                }
                                if (cls_ThoatNuoc.list_Id_error.Count > 0)
                                {
                                    string nd = string.Empty;
                                    foreach (var item in cls_ThoatNuoc.list_Id_error)
                                    {
                                        nd = nd + item + "\n";
                                    }
                                    MessageBox.Show("Does not apply to pipes with id: " + "\n" + nd);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
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
