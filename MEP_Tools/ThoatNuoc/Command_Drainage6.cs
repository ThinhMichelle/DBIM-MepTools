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
    public class Command_Drainage6 : WPFData, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            SingleData.Singleton.Instance = new SingleData.Singleton();
            SingleData.Singleton.Instance.RevitData.UIApplication = commandData.Application;
            if (cls_Reg.Login == "Login")
            {
                Run();
            }
            return Result.Succeeded;
        }
        public void Run()
        {
            
            IList<Reference> r_nhanh = null;
            Reference r_chinh = null;
            cls_ThoatNuoc.list_Id_Tee6.Clear();
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
                                List<Connector> list_con_sx = new List<Connector>();
                                List<double> list_kc = new List<double>();
                                foreach (Reference item in r_nhanh)
                                {
                                    Pipe pipe_nhanh_sx = SingleData.Singleton.Instance.RevitData.Document.GetElement(item) as Pipe;
                                    Connector Conn_Bottom_sx = F.GetConnector(pipe_nhanh_sx);
                                    list_point.Add(Conn_Bottom_sx.Origin);
                                    list_con_sx.Add(Conn_Bottom_sx);
                                    list_r.Add(item);

                                }

                                Pipe pipe_chinh_sx = SingleData.Singleton.Instance.RevitData.Document.GetElement(r_chinh) as Pipe;
                                Connector Conn_main = F.GetConnector(pipe_chinh_sx);
                                XYZ main = Conn_main.Origin;
                                LocationCurve lc_sx = pipe_chinh_sx.Location as LocationCurve;
                                XYZ stPoint_sx = lc_sx.Curve.GetEndPoint(0);
                                XYZ edPoint_sx = lc_sx.Curve.GetEndPoint(1);
                                foreach (var item in list_con_sx)
                                {
                                    XYZ Intersec = F.GetIntersec(stPoint_sx, edPoint_sx, item);
                                    list_kc.Add(Intersec.DistanceTo(main));
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

                                    LocationCurve lc_nhanh = pipe_nhanh.Location as LocationCurve;
                                    XYZ stPoint_nhanh = lc_nhanh.Curve.GetEndPoint(0);
                                    XYZ edPoint_nhanh = lc_nhanh.Curve.GetEndPoint(1);

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
                                                double diameter = pipe_nhanh.Diameter * 2;
                                                XYZ diem11 = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, 0);
                                                XYZ diem22 = new XYZ(Intersec.X, Intersec.Y, 0);
                                                double kc0 = diem11.DistanceTo(diem22);
                                                double kc_keolaigan = kc0 - diameter;
                                                XYZ u_keolaigan = null;
                                                if ((stPoint_nhanh - edPoint_nhanh).Normalize().Z < 0)
                                                {
                                                    u_keolaigan = stPoint_nhanh - edPoint_nhanh;
                                                }
                                                else
                                                {
                                                    u_keolaigan = -stPoint_nhanh + edPoint_nhanh;
                                                }

                                                Conn_Bottom.Origin = Conn_Bottom.Origin + kc_keolaigan * u_keolaigan.Normalize();
                                                Intersec = F.GetIntersec(stPoint_new_1, edPoint_new_1, Conn_Bottom);

                                                Pipe p_new = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, pipe_nhanh.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId(), pipe_nhanh.PipeType.Id, pipe_nhanh.ReferenceLevel.Id, Conn_Bottom.Origin, Intersec);
                                                F.CopyParameters(pipe_nhanh, p_new);
                                                double len = p_new.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                                                F.Case6(pipe_chinh_1, pipe_nhanh, stPoint_new_1, edPoint_new_1, Intersec, len, _pipeSystemType_1, _pipeType_1.Id, _levelId_1, p_new, F.GetConnectorFromPoint(p_new, Intersec), Conn_Bottom);
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
                                                    double diameter = pipe_nhanh.Diameter * 2;
                                                    XYZ diem11 = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, 0);
                                                    XYZ diem22 = new XYZ(Intersec2.X, Intersec2.Y, 0);
                                                    double kc0 = diem11.DistanceTo(diem22);
                                                    double kc_keolaigan = kc0 - diameter;
                                                    XYZ u_keolaigan = null;
                                                    if ((stPoint_nhanh - edPoint_nhanh).Normalize().Z < 0)
                                                    {
                                                        u_keolaigan = stPoint_nhanh - edPoint_nhanh;
                                                    }
                                                    else
                                                    {
                                                        u_keolaigan = -stPoint_nhanh + edPoint_nhanh;
                                                    }

                                                    Conn_Bottom.Origin = Conn_Bottom.Origin + kc_keolaigan * u_keolaigan.Normalize();
                                                    Intersec2 = F.GetIntersec(stPoint_new_2, edPoint_new_2, Conn_Bottom);

                                                    Pipe p_new = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, pipe_nhanh.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId(), pipe_nhanh.PipeType.Id, pipe_nhanh.ReferenceLevel.Id, Conn_Bottom.Origin, Intersec2);
                                                    F.CopyParameters(pipe_nhanh, p_new);
                                                    double len = p_new.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                                                    F.Case6(pipe_chinh_2, pipe_nhanh, stPoint_new_2, edPoint_new_2, Intersec2, len, _pipeSystemType_2, _pipeType_2.Id, _levelId_2, p_new, F.GetConnectorFromPoint(p_new, Intersec2), Conn_Bottom);
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
                                        if (Intersec != null)
                                        {
                                            if (Conn_Bottom.Origin.Z > Intersec.Z)
                                            {
                                                double diameter = pipe_nhanh.Diameter * 2;
                                                XYZ diem11 = new XYZ(Conn_Bottom.Origin.X, Conn_Bottom.Origin.Y, 0);
                                                XYZ diem22 = new XYZ(Intersec.X, Intersec.Y, 0);
                                                double kc0 = diem11.DistanceTo(diem22);
                                                double kc_keolaigan = kc0 - diameter;
                                                XYZ u_keolaigan = null;
                                                if ((stPoint_nhanh - edPoint_nhanh).Normalize().Z < 0)
                                                {
                                                    u_keolaigan = stPoint_nhanh - edPoint_nhanh;
                                                }
                                                else
                                                {
                                                    u_keolaigan = -stPoint_nhanh + edPoint_nhanh;
                                                }

                                                Conn_Bottom.Origin = Conn_Bottom.Origin + kc_keolaigan * u_keolaigan.Normalize();
                                                Intersec = F.GetIntersec(stPoint, edPoint, Conn_Bottom);

                                                Pipe p_new = Pipe.Create(SingleData.Singleton.Instance.RevitData.Document, pipe_nhanh.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId(), pipe_nhanh.PipeType.Id, pipe_nhanh.ReferenceLevel.Id, Conn_Bottom.Origin, Intersec);
                                                F.CopyParameters(pipe_nhanh, p_new);
                                                double len = p_new.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                                                F.Case6(pipe_chinh, pipe_nhanh, stPoint, edPoint, Intersec, len, _pipeSystemType, _pipeType.Id, _levelId, p_new, F.GetConnectorFromPoint(p_new, Intersec), Conn_Bottom);
                                            }
                                            else
                                            {
                                                cls_ThoatNuoc.list_Id_error.Add(pipe_nhanh.Id);
                                            }
                                        }
                                       
                                    }
                                    SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                                }
                                if (cls_ThoatNuoc.list_Id_Tee6.Count > 0)
                                {
                                    foreach (var item in cls_ThoatNuoc.list_Id_Tee6)
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
                    catch (Exception )
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
