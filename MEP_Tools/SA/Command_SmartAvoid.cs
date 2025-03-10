#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.IFC;
using xNet;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Electrical;
using System.Windows;
#endregion

namespace MEP_Tools
{
    [Transaction(TransactionMode.Manual)]
    public class Command_SmartAvoid : WPFData, IExternalCommand
    {
        public ICommand CommandRun { get; set; }
        public ICommand CommandCancel { get; set; }
        public ICommand CommandSelect { get; set; }
        public ICommand CommandAuto { get; set; }
        public ICommand CommandCheck { get; set; }
        public ICommand CommandSelectedWall { get; set; }
        public Command_SmartAvoid()
        {   
            CommandSelect = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                p.Hide();
                Reference r = null;
                try
                {
                    r = SingleData.Singleton.Instance.RevitData.Selection.PickObject(ObjectType.Element, "Select Wall to edit");
                }
                catch
                {

                }
                if (r != null)
                {
                    Element ele = SingleData.Singleton.Instance.RevitData.Document.GetElement(r);
                    if (ele is Wall)
                    {
                        cls_SA.WallSelcet = ele as Wall;
                        SingleData.Singleton.Instance.WFData.Inputwindow_SA.Run.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Please select Wall");
                    }
                }    
                p.ShowDialog();
            });
            CommandRun = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                SingleData.Singleton.Instance.RevitData.Transaction.Start();
                Wall WallSelect = cls_SA.WallSelcet;
                Func_SA F = new Func_SA();
                if (WallSelect != null)
                {                   
                    #region 'Get profile'
                    double width = cls_SA.WallSelcet.Width;
                    IList<Reference> sideFaces = HostObjectUtils.GetSideFaces(cls_SA.WallSelcet, ShellLayerType.Exterior);
                    Element e2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(sideFaces[0]);
                    Face face = e2.GetGeometryObjectFromReference(sideFaces[0]) as Face;

                    XYZ normal = face.ComputeNormal(new UV(0, 0));
                    Transform offset = Transform.CreateTranslation(normal * -width / 2);

                    IList<CurveLoop> curveLoops = face.GetEdgesAsCurveLoops();
                    IList<Curve> profile = new List<Curve>();
                    IList<IList<CurveLoop>> curveLoopLoop = ExporterIFCUtils.SortCurveLoops(curveLoops);
                    foreach (IList<CurveLoop> curveLoops2 in curveLoopLoop)
                    {
                        foreach (CurveLoop cl in curveLoops2)
                        {
                            bool isCCW = cl.IsCounterclockwise(normal);
                            foreach (Curve curve in cl)
                            {
                                profile.Add(curve.CreateTransformed(offset));
                            }
                        }
                    }
                    #endregion
                    cls_SA.List_PipeinLink.Clear();
                    cls_SA.List_DuctinLink.Clear();
                    cls_SA.List_CableTrayinLink.Clear();
                    #region 'Get Element'
                    foreach (RevitLinkType item in cls_SA.List_LinkRevitSelect)
                    {
                        foreach (Document LinkedDoc in SingleData.Singleton.Instance.RevitData.UIApplication.Application.Documents)
                        {
                            if (LinkedDoc.Title.Equals(item.Name.Split(new char[] { '.' })[0]))
                            {
                                FilteredElementCollector collLinked1 = new FilteredElementCollector(LinkedDoc);
                                IList<Element> linkedDuct = collLinked1.OfClass(typeof(Duct)).ToElements();
                                FilteredElementCollector collLinked2 = new FilteredElementCollector(LinkedDoc);
                                IList<Element> linkedPipe = collLinked2.OfClass(typeof(Pipe)).ToElements();
                                FilteredElementCollector collLinked3 = new FilteredElementCollector(LinkedDoc);
                                IList<Element> linkedCableTray = collLinked3.OfClass(typeof(CableTray)).ToElements();
                                if (cls_SA.CheckPipe == "Checked")
                                {
                                    if (linkedPipe.Count != 0)
                                    {
                                        foreach (Element elePipe in linkedPipe)
                                        {
                                            if (elePipe is Pipe)
                                            {
                                                Pipe pipe = elePipe as Pipe;
                                                if (pipe.Diameter > cls_SA.Limit_Dia)
                                                {
                                                    cls_SA.List_PipeinLink.Add(elePipe as Pipe);
                                                }
                                            }
                                        }
                                    }
                                }
                                if (cls_SA.CheckDuct == "Checked")
                                {
                                    if (linkedDuct.Count != 0)
                                    {
                                        foreach (Element eleDuct in linkedDuct)
                                        {
                                            if (eleDuct is Duct)
                                            {
                                                cls_SA.List_DuctinLink.Add(eleDuct as Duct);
                                            }
                                        }
                                    }
                                }
                                if (cls_SA.CheckCableTray == "Checked")
                                {
                                    if (linkedCableTray.Count != 0)
                                    {
                                        foreach (Element eleCableTray in linkedCableTray)
                                        {
                                            if (eleCableTray is CableTray)
                                            {
                                                cls_SA.List_CableTrayinLink.Add(eleCableTray as CableTray);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region 'Add shaft openning'
                    Curve location = (cls_SA.WallSelcet.Location as LocationCurve).Curve;
                    XYZ center = null;
                    XYZ xyAxis = new XYZ(location.GetEndPoint(0).X - location.GetEndPoint(1).X, location.GetEndPoint(0).Y - location.GetEndPoint(1).Y, 0).Normalize();
                    #region 'Pipe'
                    List<Element> List_PipeIntersec = new List<Element>();
                    List<XYZ> List_Intersec1 = new List<XYZ>();
                    if (cls_SA.List_PipeinLink.Count !=0 )
                    {
                        foreach (Pipe item in cls_SA.List_PipeinLink)
                        {
                            Curve PipeCurve = (item.Location as LocationCurve).Curve;
                            if (face.Intersect(PipeCurve, out IntersectionResultArray intersectionResultArray) == SetComparisonResult.Overlap)
                            {
                                foreach (IntersectionResult intersectresult in intersectionResultArray)
                                {
                                    List_PipeIntersec.Add(item);                                                                       
                                    center = intersectresult.XYZPoint;
                                    List_Intersec1.Add(center);                                   
                                }
                            }
                        }
                        if (List_PipeIntersec.Count == 1)
                        {
                            Pipe pipe = List_PipeIntersec[0] as Pipe;
                            Curve PipeCurve = (pipe.Location as LocationCurve).Curve;
                            double radius = pipe.Diameter / 2;
                            face.Intersect(PipeCurve, out IntersectionResultArray intersectionResultArray);
                            foreach (IntersectionResult intersectresult in intersectionResultArray)
                            {
                                center = intersectresult.XYZPoint;
                                F.AddShaft_Circle(xyAxis, profile[0].GetEndPoint(0), center, normal, location, radius, cls_SA.WallSelcet);
                            }
                        }
                        else if (List_PipeIntersec.Count > 1)
                        {
                            Element Ele = F.FindElementOutermost(List_PipeIntersec, List_Intersec1, location.GetEndPoint(0));
                            List<Element> SortElement = new List<Element>();
                            List<XYZ> SortXYZ = new List<XYZ>();
                            SortElement = F.SortElement(List_PipeIntersec, List_Intersec1, Ele);
                            SortXYZ = F.SortXYZ(List_PipeIntersec, List_Intersec1, Ele);
                            List<List<Pipe>> List_Group_Pipe = new List<List<Pipe>>();
                            List_Group_Pipe = F.DividePipe(SortElement, SortXYZ);

                            foreach (var item in List_Group_Pipe)
                            {
                                if (item.Count == 1)
                                {
                                    Curve PipeCurve = (item[0].Location as LocationCurve).Curve;
                                    double radius = item[0].Diameter / 2;
                                    face.Intersect(PipeCurve, out IntersectionResultArray intersectionResultArray);
                                    foreach (IntersectionResult intersectresult in intersectionResultArray)
                                    {
                                        center = intersectresult.XYZPoint;
                                        F.AddShaft_Circle(xyAxis, profile[0].GetEndPoint(0), center, normal, location, radius, cls_SA.WallSelcet);
                                    }
                                }
                                else if (item.Count > 1)
                                {
                                    XYZ center1 = null;
                                    XYZ center2 = null;
                                    Curve PipeCurve1 = (item[0].Location as LocationCurve).Curve;
                                    double radius1 = item[0].Diameter / 2;
                                    face.Intersect(PipeCurve1, out IntersectionResultArray intersectionResultArray1);
                                    foreach (IntersectionResult intersectresult in intersectionResultArray1)
                                    {
                                        center1 = intersectresult.XYZPoint;
                                    }
                                    Curve PipeCurve2 = (item[item.Count - 1].Location as LocationCurve).Curve;
                                    double radius2 = item[item.Count - 1].Diameter / 2;
                                    face.Intersect(PipeCurve2, out IntersectionResultArray intersectionResultArray2);
                                    foreach (IntersectionResult intersectresult in intersectionResultArray2)
                                    {
                                        center2 = intersectresult.XYZPoint;
                                    }
                                    Double max = item[0].Diameter / 2;
                                    foreach (var item1 in item)
                                    {
                                        if (item1.Diameter /2 > max)
                                        {
                                            max = item1.Diameter / 2;
                                        }
                                    }   
                                    F.AddShaft_Circles(xyAxis, profile[0].GetEndPoint(0), center1, center2, normal, location, radius1, radius2, max, cls_SA.WallSelcet);
                                }
                            }
                        }                      
                    }                                
                    #endregion
                    #region 'Duct' 
                    List<Element> List_DuctIntersec = new List<Element>();
                    List<XYZ> List_Intersec2 = new List<XYZ>();
                    if (cls_SA.List_DuctinLink.Count != 0)
                    {
                        foreach (Duct item in cls_SA.List_DuctinLink)
                        {
                            Curve DuctCurve = (item.Location as LocationCurve).Curve;
                            if (face.Intersect(DuctCurve, out IntersectionResultArray intersectionResultArray) == SetComparisonResult.Overlap)
                            {
                                foreach (IntersectionResult intersectresult in intersectionResultArray)
                                {
                                    List_DuctIntersec.Add(item);
                                    center = intersectresult.XYZPoint;
                                    List_Intersec2.Add(center);                                   
                                }
                            }
                        }
                        if (List_DuctIntersec.Count == 1)
                        {
                            Duct duct = List_DuctIntersec[0] as Duct;
                            Curve DuctCurve = (duct.Location as LocationCurve).Curve;
                            try
                            {
                                double radius = duct.Diameter / 2;
                                face.Intersect(DuctCurve, out IntersectionResultArray intersectionResultArray);
                                foreach (IntersectionResult intersectresult in intersectionResultArray)
                                {
                                    center = intersectresult.XYZPoint;
                                    F.AddShaft_Circle(xyAxis, profile[0].GetEndPoint(0), center, normal, location, radius, cls_SA.WallSelcet);
                                }
                            }
                            catch
                            {
                                double duct_h = duct.Height / 2;
                                double duct_w = duct.Width / 2;
                                face.Intersect(DuctCurve, out IntersectionResultArray intersectionResultArray);
                                foreach (IntersectionResult intersectresult in intersectionResultArray)
                                {
                                    center = intersectresult.XYZPoint;
                                    F.AddShaft_Rec(xyAxis, profile[0].GetEndPoint(0), center, normal, location, duct_h, duct_w, cls_SA.WallSelcet);
                                }
                            }
                        }
                        else if (List_DuctIntersec.Count > 1)
                        {
                            Element Ele2 = F.FindElementOutermost(List_DuctIntersec, List_Intersec2, location.GetEndPoint(0));
                            List<Element> SortElement2 = new List<Element>();
                            List<XYZ> SortXYZ2 = new List<XYZ>();
                            SortElement2 = F.SortElement(List_DuctIntersec, List_Intersec2, Ele2);
                            SortXYZ2 = F.SortXYZ(List_DuctIntersec, List_Intersec2, Ele2);
                            List<List<Duct>> List_Group_Duct = new List<List<Duct>>();
                            List_Group_Duct = F.DivideDuct(SortElement2, SortXYZ2);

                            foreach (var item in List_Group_Duct)
                            {
                                if (item.Count == 1)
                                {
                                    Curve DuctCurve = (item[0].Location as LocationCurve).Curve;
                                    try
                                    {
                                        double duct_h = item[0].Height / 2;
                                        double duct_w = item[0].Width / 2;
                                        face.Intersect(DuctCurve, out IntersectionResultArray intersectionResultArray);
                                        foreach (IntersectionResult intersectresult in intersectionResultArray)
                                        {
                                            center = intersectresult.XYZPoint;
                                            F.AddShaft_Rec(xyAxis, profile[0].GetEndPoint(0), center, normal, location, duct_h, duct_w, cls_SA.WallSelcet);
                                        }
                                    }
                                    catch
                                    {
                                        double duct_d = item[0].Diameter / 2;
                                        face.Intersect(DuctCurve, out IntersectionResultArray intersectionResultArray);
                                        foreach (IntersectionResult intersectresult in intersectionResultArray)
                                        {
                                            center = intersectresult.XYZPoint;
                                            F.AddShaft_Circle(xyAxis, profile[0].GetEndPoint(0), center, normal, location, duct_d, cls_SA.WallSelcet);
                                        }
                                    }
                                }
                                else if (item.Count > 1)
                                {
                                    XYZ center1 = null;
                                    XYZ center2 = null;
                                    double radius1 = 0, radius2 = 0, height1 = 0, height2 = 0, width1 = 0, width2 = 0;
                                    Curve DuctCurve1 = (item[0].Location as LocationCurve).Curve;
                                    face.Intersect(DuctCurve1, out IntersectionResultArray intersectionResultArray1);
                                    foreach (IntersectionResult intersectresult in intersectionResultArray1)
                                    {
                                        center1 = intersectresult.XYZPoint;
                                    }
                                    Curve DuctCurve2 = (item[item.Count - 1].Location as LocationCurve).Curve;
                                    face.Intersect(DuctCurve2, out IntersectionResultArray intersectionResultArray2);
                                    foreach (IntersectionResult intersectresult in intersectionResultArray2)
                                    {
                                        center2 = intersectresult.XYZPoint;
                                    }
                                    try
                                    {
                                        double max = item[0].Diameter/2;
                                        foreach (var item1 in item)
                                        {
                                            if (item1.Diameter /2 > max)
                                            {
                                                max = item1.Diameter / 2;
                                            }  
                                        }
                                        radius1 = item[0].Diameter / 2;
                                        radius2 = item[item.Count - 1].Diameter / 2;
                                        F.AddShaft_Circles(xyAxis, profile[0].GetEndPoint(0), center1, center2, normal, location, radius1, radius2, max, cls_SA.WallSelcet);

                                    }
                                    catch
                                    {
                                        double max = item[0].Height / 2;
                                        foreach (var item1 in item)
                                        {
                                            if (item1.Height / 2 > max)
                                            {
                                                max = item1.Height / 2;
                                            }
                                        }
                                        height1 = item[0].Height / 2;
                                        width1 = item[0].Width / 2;

                                        height2 = item[0].Height / 2;
                                        width2 = item[item.Count - 1].Width / 2;
                                        F.AddShaft_Recs(xyAxis, profile[0].GetEndPoint(0), center1, center2, normal, location, height1, width1, height2, width2, max, cls_SA.WallSelcet);
                                    }
                                }
                            }        
                        }
                    }
                    #endregion
                    #region 'CableTray'
                    List<Element> List_CableTrayIntersec = new List<Element>();
                    List<XYZ> List_Intersec3 = new List<XYZ>();
                    if (cls_SA.List_CableTrayinLink.Count != 0)
                    {
                        foreach (CableTray item in cls_SA.List_CableTrayinLink)
                        {
                            Curve CableTrayCurve = (item.Location as LocationCurve).Curve;
                            if (face.Intersect(CableTrayCurve, out IntersectionResultArray intersectionResultArray) == SetComparisonResult.Overlap)
                            {
                                foreach (IntersectionResult intersectresult in intersectionResultArray)
                                {
                                    List_CableTrayIntersec.Add(item);
                                    center = intersectresult.XYZPoint;
                                    List_Intersec3.Add(center);

                                    double CavleTray_h = item.Height / 2;
                                    double CavleTray_w = item.Width / 2;
                                    F.AddShaft_Rec(xyAxis, profile[0].GetEndPoint(0), center, normal, location, CavleTray_h, CavleTray_w, cls_SA.WallSelcet);
                                }
                            }
                        }
                        if (List_CableTrayIntersec.Count == 1)
                        {
                            CableTray cableTray = List_CableTrayIntersec[0] as CableTray;
                            Curve CableTrayCurve = (cableTray.Location as LocationCurve).Curve;
                            double CavleTray_h = cableTray.Height / 2;
                            double CavleTray_w = cableTray.Width / 2;
                            face.Intersect(CableTrayCurve, out IntersectionResultArray intersectionResultArray);
                            foreach (IntersectionResult intersectresult in intersectionResultArray)
                            {
                                center = intersectresult.XYZPoint;
                                F.AddShaft_Rec(xyAxis, profile[0].GetEndPoint(0), center, normal, location, CavleTray_h, CavleTray_w, cls_SA.WallSelcet);

                            }
                        }
                        else if (List_CableTrayIntersec.Count > 1)
                        {
                            Element Ele3 = F.FindElementOutermost(List_CableTrayIntersec, List_Intersec3, location.GetEndPoint(0));
                            List<Element> SortElement3 = new List<Element>();
                            List<XYZ> SortXYZ3 = new List<XYZ>();
                            SortElement3 = F.SortElement(List_CableTrayIntersec, List_Intersec3, Ele3);
                            SortXYZ3 = F.SortXYZ(List_CableTrayIntersec, List_Intersec3, Ele3);
                            List<List<CableTray>> List_Group_CableTray = new List<List<CableTray>>();
                            List_Group_CableTray = F.DivideCabTray(SortElement3, SortXYZ3);

                            foreach (var item in List_Group_CableTray)
                            {
                                if (item.Count == 1)
                                {
                                    Curve CableTrayCurve = (item[0].Location as LocationCurve).Curve;
                                    double CavleTray_h = item[0].Height / 2;
                                    double CavleTray_w = item[0].Width / 2;
                                    face.Intersect(CableTrayCurve, out IntersectionResultArray intersectionResultArray);
                                    foreach (IntersectionResult intersectresult in intersectionResultArray)
                                    {
                                        center = intersectresult.XYZPoint;
                                        F.AddShaft_Rec(xyAxis, profile[0].GetEndPoint(0), center, normal, location, CavleTray_h, CavleTray_w, cls_SA.WallSelcet);
                                    }
                                }
                                else if (item.Count > 1)
                                {
                                    XYZ center1 = null;
                                    XYZ center2 = null;
                                    double height1 = 0, height2 = 0, width1 = 0, width2 = 0;
                                    Curve CabTrayCurve1 = (item[0].Location as LocationCurve).Curve;
                                    face.Intersect(CabTrayCurve1, out IntersectionResultArray intersectionResultArray1);
                                    foreach (IntersectionResult intersectresult in intersectionResultArray1)
                                    {
                                        center1 = intersectresult.XYZPoint;
                                    }
                                    Curve CabTrayCurve2 = (item[item.Count - 1].Location as LocationCurve).Curve;
                                    face.Intersect(CabTrayCurve2, out IntersectionResultArray intersectionResultArray2);
                                    foreach (IntersectionResult intersectresult in intersectionResultArray2)
                                    {
                                        center2 = intersectresult.XYZPoint;
                                    }
                                    double max = item[0].Height / 2;
                                    foreach (var item1 in item)
                                    {
                                        if (item1.Height / 2 > max)
                                        {
                                            max = item1.Height / 2;
                                        }
                                    }
                                    height1 = item[0].Height / 2;
                                    width1 = item[0].Width / 2;

                                    height2 = item[0].Height / 2;
                                    width2 = item[item.Count - 1].Width / 2;
                                    F.AddShaft_Recs(xyAxis, profile[0].GetEndPoint(0), center1, center2, normal, location, height1, width1, height2, width2, max, cls_SA.WallSelcet);
                                }
                            }
                        }
                    }
                    #endregion
                    #endregion
                    SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                    
                }
                else
                {
                    MessageBox.Show("You have to choose the wall");
                }
            });
            CommandCancel = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {                
                p.Close();
            });
            CommandAuto = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                //p.Hide();
                SingleData.Singleton.Instance.RevitData.Transaction.Start();
                #region 'Get Wall'
                List<Wall> List_Wall = new List<Wall>();
                FilteredElementCollector collector_wall = new FilteredElementCollector(SingleData.Singleton.Instance.RevitData.Document);
                ICollection<Element> collection_wall = collector_wall.OfClass(typeof(Wall)).ToElements();
                foreach (Element W in collection_wall)
                {
                    if (W is Wall)
                    {
                        List_Wall.Add(W as Wall);
                    }
                }
                #endregion 
                cls_SA.List_PipeinLink.Clear();
                cls_SA.List_DuctinLink.Clear();
                cls_SA.List_CableTrayinLink.Clear();
                #region 'Get Element'
                foreach (RevitLinkType item in cls_SA.List_LinkRevitSelect)
                {
                    foreach (Document LinkedDoc in SingleData.Singleton.Instance.RevitData.UIApplication.Application.Documents)
                    {
                        if (LinkedDoc.Title.Equals(item.Name.Split(new char[] { '.' })[0]))
                        {
                            FilteredElementCollector collLinked = new FilteredElementCollector(LinkedDoc);
                            FilteredElementCollector collLinked1 = new FilteredElementCollector(LinkedDoc);
                            IList<Element> linkedDuct = collLinked1.OfClass(typeof(Duct)).ToElements();
                            FilteredElementCollector collLinked2 = new FilteredElementCollector(LinkedDoc);
                            IList<Element> linkedPipe = collLinked2.OfClass(typeof(Pipe)).ToElements();
                            FilteredElementCollector collLinked3 = new FilteredElementCollector(LinkedDoc);
                            IList<Element> linkedCableTray = collLinked3.OfClass(typeof(CableTray)).ToElements();
                            if (cls_SA.CheckPipe == "Checked")
                            {
                                if (linkedPipe.Count != 0)
                                {
                                    foreach (Element elePipe in linkedPipe)
                                    {
                                        if (elePipe is Pipe)
                                        {
                                            cls_SA.List_PipeinLink.Add(elePipe as Pipe);
                                        }
                                    }
                                }
                            }
                            if (cls_SA.CheckDuct == "Checked")
                            {
                                if (linkedDuct.Count != 0)
                                {
                                    foreach (Element eleDuct in linkedDuct)
                                    {
                                        if (eleDuct is Duct)
                                        {
                                            cls_SA.List_DuctinLink.Add(eleDuct as Duct);
                                        }
                                    }
                                }
                            }
                            if (cls_SA.CheckCableTray == "Checked")
                            {
                                if (linkedCableTray.Count != 0)
                                {
                                    foreach (Element eleCableTray in linkedCableTray)
                                    {
                                        if (eleCableTray is CableTray)
                                        {
                                            cls_SA.List_CableTrayinLink.Add(eleCableTray as CableTray);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                if (List_Wall != null || List_Wall.Count != 0)
                {
                    foreach (Wall Wall_In_File in List_Wall)
                    {
                        #region 'Get profile'
                        double width = Wall_In_File.Width;

                        IList<Reference> sideFaces = HostObjectUtils.GetSideFaces(Wall_In_File, ShellLayerType.Exterior);
                        Element e2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(sideFaces[0]);
                        Face face = e2.GetGeometryObjectFromReference(sideFaces[0]) as Face;

                        XYZ normal = face.ComputeNormal(new UV(0, 0));
                        Transform offset = Transform.CreateTranslation(normal * -width / 2);

                        IList<CurveLoop> curveLoops = face.GetEdgesAsCurveLoops();
                        IList<Curve> profile = new List<Curve>();
                        IList<IList<CurveLoop>> curveLoopLoop = ExporterIFCUtils.SortCurveLoops(curveLoops);
                        foreach (IList<CurveLoop> curveLoops2 in curveLoopLoop)
                        {
                            foreach (CurveLoop cl in curveLoops2)
                            {
                                bool isCCW = cl.IsCounterclockwise(normal);
                                foreach (Curve curve in cl)
                                {
                                    profile.Add(curve.CreateTransformed(offset));
                                }
                            }
                        }
                        #endregion
                        #region 'Add shaft openning'
                        Func_SA F = new Func_SA();
                        Curve location = (Wall_In_File.Location as LocationCurve).Curve;
                        XYZ center = null;
                        XYZ xyAxis = new XYZ(location.GetEndPoint(0).X - location.GetEndPoint(1).X, location.GetEndPoint(0).Y - location.GetEndPoint(1).Y, 0).Normalize();
                        #region 'Pipe'
                        List<Element> List_PipeIntersec = new List<Element>();
                        List<XYZ> List_Intersec1 = new List<XYZ>();
                        if (cls_SA.List_PipeinLink.Count != 0)
                        {
                            foreach (Pipe item in cls_SA.List_PipeinLink)
                            {
                                Curve PipeCurve = (item.Location as LocationCurve).Curve;
                                if (face.Intersect(PipeCurve, out IntersectionResultArray intersectionResultArray) == SetComparisonResult.Overlap)
                                {
                                    foreach (IntersectionResult intersectresult in intersectionResultArray)
                                    {
                                        List_PipeIntersec.Add(item);
                                        center = intersectresult.XYZPoint;
                                        List_Intersec1.Add(center);
                                    }
                                }
                            }
                            if (List_PipeIntersec.Count == 1)
                            {
                                Pipe pipe = List_PipeIntersec[0] as Pipe;
                                Curve PipeCurve = (pipe.Location as LocationCurve).Curve;
                                double radius = pipe.Diameter / 2;
                                face.Intersect(PipeCurve, out IntersectionResultArray intersectionResultArray);
                                foreach (IntersectionResult intersectresult in intersectionResultArray)
                                {
                                    center = intersectresult.XYZPoint;
                                    F.AddShaft_Circle(xyAxis, profile[0].GetEndPoint(0), center, normal, location, radius, Wall_In_File);
                                }
                            }
                            else if (List_PipeIntersec.Count > 1)
                            {
                                Element Ele = F.FindElementOutermost(List_PipeIntersec, List_Intersec1, location.GetEndPoint(0));
                                List<Element> SortElement = new List<Element>();
                                List<XYZ> SortXYZ = new List<XYZ>();
                                SortElement = F.SortElement(List_PipeIntersec, List_Intersec1, Ele);
                                SortXYZ = F.SortXYZ(List_PipeIntersec, List_Intersec1, Ele);
                                List<List<Pipe>> List_Group_Pipe = new List<List<Pipe>>();
                                List_Group_Pipe = F.DividePipe(SortElement, SortXYZ);

                                foreach (var item in List_Group_Pipe)
                                {
                                    if (item.Count == 1)
                                    {
                                        Curve PipeCurve = (item[0].Location as LocationCurve).Curve;
                                        double radius = item[0].Diameter / 2;
                                        face.Intersect(PipeCurve, out IntersectionResultArray intersectionResultArray);
                                        foreach (IntersectionResult intersectresult in intersectionResultArray)
                                        {
                                            center = intersectresult.XYZPoint;
                                            F.AddShaft_Circle(xyAxis, profile[0].GetEndPoint(0), center, normal, location, radius, Wall_In_File);
                                        }
                                    }
                                    else if (item.Count > 1)
                                    {
                                        XYZ center1 = null;
                                        XYZ center2 = null;
                                        Curve PipeCurve1 = (item[0].Location as LocationCurve).Curve;
                                        double radius1 = item[0].Diameter / 2;
                                        face.Intersect(PipeCurve1, out IntersectionResultArray intersectionResultArray1);
                                        foreach (IntersectionResult intersectresult in intersectionResultArray1)
                                        {
                                            center1 = intersectresult.XYZPoint;
                                        }
                                        Curve PipeCurve2 = (item[item.Count - 1].Location as LocationCurve).Curve;
                                        double radius2 = item[item.Count - 1].Diameter / 2;
                                        face.Intersect(PipeCurve2, out IntersectionResultArray intersectionResultArray2);
                                        foreach (IntersectionResult intersectresult in intersectionResultArray2)
                                        {
                                            center2 = intersectresult.XYZPoint;
                                        }
                                        double max = item[0].Diameter / 2;
                                        foreach (var item1 in item)
                                        {
                                            if (item1.Diameter / 2 > max)
                                            {
                                                max = item1.Diameter / 2;
                                            }
                                        }
                                        F.AddShaft_Circles(xyAxis, profile[0].GetEndPoint(0), center1, center2, normal, location, radius1, radius2, max, Wall_In_File);
                                    }
                                }
                            }
                        }
                        #endregion
                        #region 'Duct' 
                        List<Element> List_DuctIntersec = new List<Element>();
                        List<XYZ> List_Intersec2 = new List<XYZ>();
                        if (cls_SA.List_DuctinLink.Count != 0)
                        {
                            foreach (Duct item in cls_SA.List_DuctinLink)
                            {
                                Curve DuctCurve = (item.Location as LocationCurve).Curve;
                                if (face.Intersect(DuctCurve, out IntersectionResultArray intersectionResultArray) == SetComparisonResult.Overlap)
                                {
                                    foreach (IntersectionResult intersectresult in intersectionResultArray)
                                    {
                                        List_DuctIntersec.Add(item);
                                        center = intersectresult.XYZPoint;
                                        List_Intersec2.Add(center);
                                    }
                                }
                            }
                            if (List_DuctIntersec.Count == 1)
                            {
                                Duct duct = List_DuctIntersec[0] as Duct;
                                Curve DuctCurve = (duct.Location as LocationCurve).Curve;
                                try
                                {
                                    double radius = duct.Diameter / 2;
                                    face.Intersect(DuctCurve, out IntersectionResultArray intersectionResultArray);
                                    foreach (IntersectionResult intersectresult in intersectionResultArray)
                                    {
                                        center = intersectresult.XYZPoint;
                                        F.AddShaft_Circle(xyAxis, profile[0].GetEndPoint(0), center, normal, location, radius, Wall_In_File);
                                    }
                                }
                                catch
                                {
                                    double duct_h = duct.Height / 2;
                                    double duct_w = duct.Width / 2;
                                    face.Intersect(DuctCurve, out IntersectionResultArray intersectionResultArray);
                                    foreach (IntersectionResult intersectresult in intersectionResultArray)
                                    {
                                        center = intersectresult.XYZPoint;
                                        F.AddShaft_Rec(xyAxis, profile[0].GetEndPoint(0), center, normal, location, duct_h, duct_w, Wall_In_File);
                                    }
                                }
                            }
                            else if (List_DuctIntersec.Count > 1)
                            {
                                Element Ele2 = F.FindElementOutermost(List_DuctIntersec, List_Intersec2, location.GetEndPoint(0));
                                List<Element> SortElement2 = new List<Element>();
                                List<XYZ> SortXYZ2 = new List<XYZ>();
                                SortElement2 = F.SortElement(List_DuctIntersec, List_Intersec2, Ele2);
                                SortXYZ2 = F.SortXYZ(List_DuctIntersec, List_Intersec2, Ele2);
                                List<List<Duct>> List_Group_Duct = new List<List<Duct>>();
                                List_Group_Duct = F.DivideDuct(SortElement2, SortXYZ2);

                                foreach (var item in List_Group_Duct)
                                {
                                    if (item.Count == 1)
                                    {
                                        Curve DuctCurve = (item[0].Location as LocationCurve).Curve;
                                        try
                                        {
                                            double duct_h = item[0].Height / 2;
                                            double duct_w = item[0].Width / 2;
                                            face.Intersect(DuctCurve, out IntersectionResultArray intersectionResultArray);
                                            foreach (IntersectionResult intersectresult in intersectionResultArray)
                                            {
                                                center = intersectresult.XYZPoint;
                                                F.AddShaft_Rec(xyAxis, profile[0].GetEndPoint(0), center, normal, location, duct_h, duct_w, Wall_In_File);
                                            }
                                        }
                                        catch
                                        {
                                            double duct_d = item[0].Diameter / 2;
                                            face.Intersect(DuctCurve, out IntersectionResultArray intersectionResultArray);
                                            foreach (IntersectionResult intersectresult in intersectionResultArray)
                                            {
                                                center = intersectresult.XYZPoint;
                                                F.AddShaft_Circle(xyAxis, profile[0].GetEndPoint(0), center, normal, location, duct_d, Wall_In_File);
                                            }
                                        }
                                    }
                                    else if (item.Count > 1)
                                    {
                                        XYZ center1 = null;
                                        XYZ center2 = null;
                                        double radius1 = 0, radius2 = 0, height1 = 0, height2 = 0, width1 = 0, width2 = 0;
                                        Curve DuctCurve1 = (item[0].Location as LocationCurve).Curve;
                                        face.Intersect(DuctCurve1, out IntersectionResultArray intersectionResultArray1);
                                        foreach (IntersectionResult intersectresult in intersectionResultArray1)
                                        {
                                            center1 = intersectresult.XYZPoint;
                                        }
                                        Curve DuctCurve2 = (item[item.Count - 1].Location as LocationCurve).Curve;
                                        face.Intersect(DuctCurve2, out IntersectionResultArray intersectionResultArray2);
                                        foreach (IntersectionResult intersectresult in intersectionResultArray2)
                                        {
                                            center2 = intersectresult.XYZPoint;
                                        }
                                        try
                                        {
                                            double max = item[0].Diameter / 2;
                                            foreach (var item1 in item)
                                            {
                                                if (item1.Diameter / 2 > max)
                                                {
                                                    max = item1.Diameter / 2;
                                                }
                                            }
                                            radius1 = item[0].Diameter / 2;
                                            radius2 = item[item.Count - 1].Diameter / 2;
                                            F.AddShaft_Circles(xyAxis, profile[0].GetEndPoint(0), center1, center2, normal, location, radius1, radius2,max,Wall_In_File);

                                        }
                                        catch
                                        {
                                            double max = item[0].Height / 2;
                                            foreach (var item1 in item)
                                            {
                                                if (item1.Height / 2 > max)
                                                {
                                                    max = item1.Height / 2;
                                                }
                                            }
                                            height1 = item[0].Height / 2;
                                            width1 = item[0].Width / 2;

                                            height2 = item[0].Height / 2;
                                            width2 = item[item.Count - 1].Width / 2;
                                            F.AddShaft_Recs(xyAxis, profile[0].GetEndPoint(0), center1, center2, normal, location, height1, width1, height2, width2, max, Wall_In_File);
                                        }

                                    }
                                }
                            }
                        }
                        #endregion
                        #region 'CableTray'
                        List<Element> List_CableTrayIntersec = new List<Element>();
                        List<XYZ> List_Intersec3 = new List<XYZ>();
                        if (cls_SA.List_CableTrayinLink.Count != 0)
                        {
                            foreach (CableTray item in cls_SA.List_CableTrayinLink)
                            {
                                Curve CableTrayCurve = (item.Location as LocationCurve).Curve;
                                if (face.Intersect(CableTrayCurve, out IntersectionResultArray intersectionResultArray) == SetComparisonResult.Overlap)
                                {
                                    foreach (IntersectionResult intersectresult in intersectionResultArray)
                                    {
                                        List_CableTrayIntersec.Add(item);
                                        center = intersectresult.XYZPoint;
                                        List_Intersec3.Add(center);

                                        double CavleTray_h = item.Height / 2;
                                        double CavleTray_w = item.Width / 2;
                                        F.AddShaft_Rec(xyAxis, profile[0].GetEndPoint(0), center, normal, location, CavleTray_h, CavleTray_w, Wall_In_File);
                                    }
                                }
                            }
                            if (List_CableTrayIntersec.Count == 1)
                            {
                                CableTray cableTray = List_CableTrayIntersec[0] as CableTray;
                                Curve CableTrayCurve = (cableTray.Location as LocationCurve).Curve;
                                double CavleTray_h = cableTray.Height / 2;
                                double CavleTray_w = cableTray.Width / 2;
                                face.Intersect(CableTrayCurve, out IntersectionResultArray intersectionResultArray);
                                foreach (IntersectionResult intersectresult in intersectionResultArray)
                                {
                                    center = intersectresult.XYZPoint;
                                    F.AddShaft_Rec(xyAxis, profile[0].GetEndPoint(0), center, normal, location, CavleTray_h, CavleTray_w, Wall_In_File);

                                }
                            }
                            else if (List_CableTrayIntersec.Count > 1)
                            {
                                Element Ele3 = F.FindElementOutermost(List_CableTrayIntersec, List_Intersec3, location.GetEndPoint(0));
                                List<Element> SortElement3 = new List<Element>();
                                List<XYZ> SortXYZ3 = new List<XYZ>();
                                SortElement3 = F.SortElement(List_CableTrayIntersec, List_Intersec3, Ele3);
                                SortXYZ3 = F.SortXYZ(List_CableTrayIntersec, List_Intersec3, Ele3);
                                List<List<CableTray>> List_Group_CableTray = new List<List<CableTray>>();
                                List_Group_CableTray = F.DivideCabTray(SortElement3, SortXYZ3);

                                foreach (var item in List_Group_CableTray)
                                {
                                    if (item.Count == 1)
                                    {
                                        Curve CableTrayCurve = (item[0].Location as LocationCurve).Curve;
                                        double CavleTray_h = item[0].Height / 2;
                                        double CavleTray_w = item[0].Width / 2;
                                        face.Intersect(CableTrayCurve, out IntersectionResultArray intersectionResultArray);
                                        foreach (IntersectionResult intersectresult in intersectionResultArray)
                                        {
                                            center = intersectresult.XYZPoint;
                                            F.AddShaft_Rec(xyAxis, profile[0].GetEndPoint(0), center, normal, location, CavleTray_h, CavleTray_w, Wall_In_File);
                                        }
                                    }
                                    else if (item.Count > 1)
                                    {
                                        XYZ center1 = null;
                                        XYZ center2 = null;
                                        double height1 = 0, height2 = 0, width1 = 0, width2 = 0;
                                        Curve CabTrayCurve1 = (item[0].Location as LocationCurve).Curve;
                                        face.Intersect(CabTrayCurve1, out IntersectionResultArray intersectionResultArray1);
                                        foreach (IntersectionResult intersectresult in intersectionResultArray1)
                                        {
                                            center1 = intersectresult.XYZPoint;
                                        }
                                        Curve CabTrayCurve2 = (item[item.Count - 1].Location as LocationCurve).Curve;
                                        face.Intersect(CabTrayCurve2, out IntersectionResultArray intersectionResultArray2);
                                        foreach (IntersectionResult intersectresult in intersectionResultArray2)
                                        {
                                            center2 = intersectresult.XYZPoint;
                                        }

                                        double max = item[0].Height / 2;
                                        foreach (var item1 in item)
                                        {
                                            if (item1.Height / 2 > max)
                                            {
                                                max = item1.Height / 2;
                                            }
                                        }

                                        height1 = item[0].Height / 2;
                                        width1 = item[0].Width / 2;

                                        height2 = item[0].Height / 2;
                                        width2 = item[item.Count - 1].Width / 2;
                                        F.AddShaft_Recs(xyAxis, profile[0].GetEndPoint(0), center1, center2, normal, location, height1, width1, height2, width2, max, Wall_In_File);
                                    }
                                }
                            }
                        }
                        #endregion
                        #endregion
                    }
                    SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                }
                else
                {
                    MessageBox.Show("There are no walls");
                }               
            });
            CommandCheck = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                SingleData.Singleton.Instance.RevitData.Transaction.Start();
                if (cls_SA.List_LinkRevitSelect.Count != 0)
                {
                    List<Wall> List_Wall = new List<Wall>();
                    FilteredElementCollector collector_wall = new FilteredElementCollector(SingleData.Singleton.Instance.RevitData.Document);
                    ICollection<Element> collection_wall = collector_wall.OfClass(typeof(Wall)).ToElements();
                    foreach (Element W in collection_wall)
                    {
                        if (W is Wall)
                        {
                            List_Wall.Add(W as Wall);
                        }
                    }
                    cls_SA.List_PipeinLink.Clear();
                    cls_SA.List_DuctinLink.Clear();
                    cls_SA.List_CableTrayinLink.Clear();
                    #region 'Get Element'
                    foreach (RevitLinkType item in cls_SA.List_LinkRevitSelect)
                    {
                        foreach (Document LinkedDoc in SingleData.Singleton.Instance.RevitData.UIApplication.Application.Documents)
                        {
                            if (LinkedDoc.Title.Equals(item.Name.Split(new char[] { '.' })[0]))
                            {
                                FilteredElementCollector collLinked = new FilteredElementCollector(LinkedDoc);
                                FilteredElementCollector collLinked1 = new FilteredElementCollector(LinkedDoc);
                                IList<Element> linkedDuct = collLinked1.OfClass(typeof(Duct)).ToElements();
                                FilteredElementCollector collLinked2 = new FilteredElementCollector(LinkedDoc);
                                IList<Element> linkedPipe = collLinked2.OfClass(typeof(Pipe)).ToElements();
                                FilteredElementCollector collLinked3 = new FilteredElementCollector(LinkedDoc);
                                IList<Element> linkedCableTray = collLinked3.OfClass(typeof(CableTray)).ToElements();
                                if (cls_SA.CheckPipe == "Checked")
                                {
                                    if (linkedPipe.Count != 0)
                                    {
                                        foreach (Element elePipe in linkedPipe)
                                        {
                                            if (elePipe is Pipe)
                                            {
                                                cls_SA.List_PipeinLink.Add(elePipe as Pipe);
                                            }
                                        }
                                    }
                                }
                                if (cls_SA.CheckDuct == "Checked")
                                {
                                    if (linkedDuct.Count != 0)
                                    {
                                        foreach (Element eleDuct in linkedDuct)
                                        {
                                            if (eleDuct is Duct)
                                            {
                                                cls_SA.List_DuctinLink.Add(eleDuct as Duct);
                                            }
                                        }
                                    }
                                }
                                if (cls_SA.CheckCableTray == "Checked")
                                {
                                    if (linkedCableTray.Count != 0)
                                    {
                                        foreach (Element eleCableTray in linkedCableTray)
                                        {
                                            if (eleCableTray is CableTray)
                                            {
                                                cls_SA.List_CableTrayinLink.Add(eleCableTray as CableTray);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    Func_SA F = new Func_SA();
                    cls_SA.List_PipeIntersec.Clear();
                    cls_SA.List_DuctIntersec.Clear();
                    cls_SA.List_CableTrayIntersec.Clear();
                    cls_SA.List_WallIntersecPipe.Clear();
                    cls_SA.List_WallIntersecDuct.Clear();
                    cls_SA.List_WallIntersecCableTray.Clear();
                    if (List_Wall != null || List_Wall.Count != 0)
                    {
                        foreach (Wall Wall_In_File in List_Wall)
                        {
                            #region 'Get Face'
                            double width = Wall_In_File.Width;
                            IList<Reference> sideFaces = HostObjectUtils.GetSideFaces(Wall_In_File, ShellLayerType.Exterior);
                            Element e2 = SingleData.Singleton.Instance.RevitData.Document.GetElement(sideFaces[0]);
                            Face face = e2.GetGeometryObjectFromReference(sideFaces[0]) as Face;
                            #endregion
                            #region 'Check Intersec'                                                    
                            #region 'Pipe'
                            //List<Element> List_PipeIntersec = new List<Element>();                           
                            if (cls_SA.List_PipeinLink.Count != 0)
                            {
                                foreach (Pipe item in cls_SA.List_PipeinLink)
                                {
                                    Curve PipeCurve = (item.Location as LocationCurve).Curve;
                                    if (face.Intersect(PipeCurve, out IntersectionResultArray intersectionResultArray) == SetComparisonResult.Overlap)
                                    {
                                        foreach (IntersectionResult intersectresult in intersectionResultArray)
                                        {
                                            cls_SA.List_PipeIntersec.Add(item);
                                            cls_SA.List_WallIntersecPipe.Add(Wall_In_File);
                                        }
                                    }
                                }
                            }
                            #endregion
                            #region 'Duct' 
                            //List<Element> List_DuctIntersec = new List<Element>();
                            if (cls_SA.List_DuctinLink.Count != 0)
                            {
                                foreach (Duct item in cls_SA.List_DuctinLink)
                                {
                                    Curve DuctCurve = (item.Location as LocationCurve).Curve;
                                    if (face.Intersect(DuctCurve, out IntersectionResultArray intersectionResultArray) == SetComparisonResult.Overlap)
                                    {
                                        foreach (IntersectionResult intersectresult in intersectionResultArray)
                                        {
                                            cls_SA.List_DuctIntersec.Add(item);
                                            cls_SA.List_WallIntersecDuct.Add(Wall_In_File);
                                        }
                                    }
                                }
                            }
                            #endregion
                            #region 'CableTray'
                            List<Element> List_CableTrayIntersec = new List<Element>();
                            if (cls_SA.List_CableTrayinLink.Count != 0)
                            {
                                foreach (CableTray item in cls_SA.List_CableTrayinLink)
                                {
                                    Curve CableTrayCurve = (item.Location as LocationCurve).Curve;
                                    if (face.Intersect(CableTrayCurve, out IntersectionResultArray intersectionResultArray) == SetComparisonResult.Overlap)
                                    {
                                        foreach (IntersectionResult intersectresult in intersectionResultArray)
                                        {
                                            cls_SA.List_CableTrayIntersec.Add(item);
                                            cls_SA.List_WallIntersecCableTray.Add(Wall_In_File);
                                        }
                                    }
                                }
                            }
                            #endregion
                            #endregion
                        }
                        
                    }
                    if (cls_SA.List_PipeIntersec.Count == 0 && cls_SA.List_DuctIntersec.Count == 0 && cls_SA.List_CableTrayIntersec.Count == 0)
                    {
                        SingleData.Singleton.Instance.WFData.Inputwindow_SA.List_Collision.Items.Add("No collision");
                    }
                    else
                    {
                        for (int i = 0; i < cls_SA.List_PipeIntersec.Count; i++)
                        {
                            SingleData.Singleton.Instance.WFData.Inputwindow_SA.List_Collision.Items.Add("Pipe: " + cls_SA.List_PipeIntersec[i].Id.IntegerValue + " and Wall: " + cls_SA.List_WallIntersecPipe[i].Id.IntegerValue);
                            cls_SA.List_IDWall.Add(cls_SA.List_WallIntersecPipe[i].Id);
                        }
                        for (int i = 0; i < cls_SA.List_DuctIntersec.Count; i++)
                        {
                            SingleData.Singleton.Instance.WFData.Inputwindow_SA.List_Collision.Items.Add("Duct: " + cls_SA.List_DuctIntersec[i].Id.IntegerValue + " and Wall: " + cls_SA.List_WallIntersecDuct[i].Id.IntegerValue);
                            cls_SA.List_IDWall.Add(cls_SA.List_WallIntersecDuct[i].Id);
                        }
                        for (int i = 0; i < cls_SA.List_CableTrayIntersec.Count; i++)
                        {
                            SingleData.Singleton.Instance.WFData.Inputwindow_SA.List_Collision.Items.Add("CableTray: " + cls_SA.List_CableTrayIntersec[i].Id.IntegerValue + " and Wall: " + cls_SA.List_WallIntersecCableTray[i].Id.IntegerValue);
                            cls_SA.List_IDWall.Add(cls_SA.List_WallIntersecCableTray[i].Id);
                        }
                    }
                    
                }
                SingleData.Singleton.Instance.RevitData.Transaction.Commit();
            });
            CommandSelectedWall = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                try
                {
                    SingleData.Singleton.Instance.RevitData.Transaction.Start();
                    if (SingleData.Singleton.Instance.WFData.Inputwindow_SA.List_Collision.SelectedIndex != -1)
                    {
                        ElementId id = cls_SA.List_IDWall[SingleData.Singleton.Instance.WFData.Inputwindow_SA.List_Collision.SelectedIndex];
                        SingleData.Singleton.Instance.RevitData.UIDocument.ShowElements(id);
                    }
                    SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                }
                catch
                {

                }
                
            });
        }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {       
            SingleData.Singleton.Instance = new SingleData.Singleton();
            SingleData.Singleton.Instance.RevitData.UIApplication = commandData.Application;
            #region 'Get Link'
            cls_SA.List_LinkRevitFirst.Clear();
            IList<Element> source = new FilteredElementCollector(SingleData.Singleton.Instance.RevitData.Document).OfCategory(BuiltInCategory.OST_RvtLinks).OfClass(typeof(RevitLinkType)).ToElements();      
            if (source.Count() <= 0)
            {
            }
            else
            {
                foreach (Element ele in source)
                {
                    if (ele is RevitLinkType)
                    {
                        cls_SA.List_LinkRevitFirst.Add(ele);
                    }
                }
            }
            #endregion       
            if (cls_Reg.Login == "Login")
            {
                try
                {
                    SingleData.Singleton.Instance.WFData.Inputwindow_SA.ShowDialog();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message + "\n" + "Contact: " + cls_Contact.sdt + " or Email: " + cls_Contact.email);
                }
            }
            return Result.Succeeded;
        }
       
    }
}
