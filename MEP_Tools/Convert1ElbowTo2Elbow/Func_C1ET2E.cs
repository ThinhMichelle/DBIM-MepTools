using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEP_Tools
{
    class Func_C1ET2E
    {
        public List<Connector> List_Connector_Elbow_Old(Element Elbow_Old)
        {
            List<Connector> result = new List<Connector>();
            FamilyInstance p = Elbow_Old as FamilyInstance;
            ConnectorSetIterator csi = null;
            csi = p.MEPModel.ConnectorManager.Connectors.ForwardIterator();
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                result.Add(conn);
            }
            return result;
        }
        public List<Pipe> List_Pipe_Neighbor(Element Elbow_Old)
        {
            List<Pipe> result = new List<Pipe>();
            FamilyInstance p = Elbow_Old as FamilyInstance;
            ConnectorSetIterator csi = null;
            csi = p.MEPModel.ConnectorManager.Connectors.ForwardIterator();
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                var all_Ref = conn.AllRefs;
                List<Element> ConnectedElements = new List<Element>();
                foreach (Connector connector in all_Ref)
                {
                    ConnectedElements.Add(connector.Owner);
                }
                foreach (Element Ele in ConnectedElements)
                {
                    if (Ele is Pipe)
                    {
                        result.Add(Ele as Pipe);
                    }
                }
            }
            return result;
        }
        public List<Connector> Get_Connecter_Pipe_Neighbor(List<Pipe> List_Pipe_Neighbor, List<Connector> List_Connector_Elbow_Old)
        {
            List<Connector> result = new List<Connector>();
            foreach (Pipe p in List_Pipe_Neighbor)
            {
                ConnectorSetIterator csi = null;
                csi = p.ConnectorManager.Connectors.ForwardIterator();
                while (csi.MoveNext())
                {
                    Connector Con = csi.Current as Connector;
                    foreach (Connector Con_Old in List_Connector_Elbow_Old)
                    {
                        if (Con_Old.Origin.IsAlmostEqualTo(Con.Origin))
                        {
                            result.Add(Con);
                        }
                    }
                }
            }
            return result;
        }
        public ElementId GetPipeTypeId(Pipe Pipe_Old)
        {
            ElementId Ids = null;
            Ids = Pipe_Old.PipeType.Id;
            return Ids;
        }
        public ElementId GetLevelId(Pipe Pipe_Old)
        {
            ElementId Ids = null;
            Ids = Pipe_Old.ReferenceLevel.Id;
            return Ids;
        }
        public Pipe CreatePipe(Document doc, ElementId PipeTypeID, ElementId LvelID, List<Connector> lst_Con)
        {
            Pipe Pipe_New = null;
            if (lst_Con[0].Origin.Z > lst_Con[1].Origin.Z)
            {
                Pipe_New = Pipe.Create(doc, PipeTypeID, LvelID, lst_Con[0], lst_Con[1]);
            }
            else
            {
                Pipe_New = Pipe.Create(doc, PipeTypeID, LvelID, lst_Con[1], lst_Con[0]);
            }
            return Pipe_New;
        }
        public FamilyInstance CreateFitting(Document doc, Pipe Pipe_New, Connector Con_Old)
        {
            ConnectorSet c1 = Pipe_New.ConnectorManager.Connectors;
            double mindist = 100000;
            Connector conn1 = null;
            Connector conn2 = null;
            foreach (Connector con1 in c1)
            {
                double conndist = con1.Origin.DistanceTo(Con_Old.Origin);
                if (conndist < mindist)
                {
                    mindist = conndist;
                    conn1 = con1;
                    conn2 = Con_Old;
                }
            }
            FamilyInstance result = doc.Create.NewElbowFitting(conn1, conn2);
            return result;
        }
        public void Offset_Fitting(FamilyInstance Fitting1, FamilyInstance Fitting2)
        {
            double Elevation1 = Fitting1.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();
            double Elevation2 = Fitting2.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();
            double offset = 0;
            if (Elevation1 > Elevation2)
            {
                offset = Elevation1 + 60 / 304.8;
                Fitting1.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(offset);
                clsBien_C1ET2E.lst_IdFit.Add(Fitting1.Id);
            }
            else
            {
                offset = Elevation2 + 60 / 304.8;
                Fitting2.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(offset);
                clsBien_C1ET2E.lst_IdFit.Add(Fitting2.Id);
            }
        }
        public string DirOffset(List<Pipe> lst_Pipe_Input)
        {
            string result = "NotOffsetZ";
            LocationCurve LC0 = lst_Pipe_Input[0].Location as LocationCurve;
            LocationCurve LC1 = lst_Pipe_Input[1].Location as LocationCurve;
            XYZ Diem1 = LC0.Curve.GetEndPoint(0);
            XYZ Diem2 = LC1.Curve.GetEndPoint(0);
            if (Math.Round(Diem1.Z,5) == Math.Round(Diem2.Z, 5))
            {
                result = "NotOffsetZ";
            }
            else
            {
                foreach (Pipe p in lst_Pipe_Input)
                {
                    List<Connector> lst_Conn = new List<Connector>();
                    ConnectorSet ConnSet = p.ConnectorManager.Connectors;
                    foreach (Connector Conn in ConnSet)
                    {
                        lst_Conn.Add(Conn);
                    }
                    if (Math.Round(lst_Conn[0].Origin.X, 5) == Math.Round(lst_Conn[1].Origin.X, 5) && Math.Round(lst_Conn[0].Origin.Y, 5) == Math.Round(lst_Conn[1].Origin.Y, 5))
                    {
                        result = "OffsetZ";
                    }
                }
            }
            return result;
        }
        public string Offset_Fitting_NotZ(Document doc, List<Pipe> lst_Pipe_Input, FamilyInstance Fitting1)
        {
            string result = "";
            ConnectorSet ConnSet1 = lst_Pipe_Input[0].ConnectorManager.Connectors;
            ConnectorSet ConnSet = Fitting1.MEPModel.ConnectorManager.Connectors;
            List<Connector> lst_Conn_Pipe = new List<Connector>();
            foreach (Connector Conn in ConnSet1)
            {
                lst_Conn_Pipe.Add(Conn);
            }
            foreach (Connector Conn1 in ConnSet)
            {
                foreach (Connector Conn2 in ConnSet1)
                {
                    if (Math.Round(Conn1.Origin.Z, 5) == Math.Round(Conn2.Origin.Z, 5) && Math.Round(Conn1.Origin.X, 5) == Math.Round(Conn2.Origin.X, 5) && Math.Round(Conn1.Origin.Y, 5) == Math.Round(Conn2.Origin.Y, 5))
                    {
                        Pipe p = lst_Pipe_Input[0];

                        List<Connector> lst_Conn = new List<Connector>();
                        ConnectorSet Conn = p.ConnectorManager.Connectors;
                        foreach (Connector Con in Conn)
                        {
                            lst_Conn.Add(Con);
                        }
                        if (Math.Round(lst_Conn[0].Origin.X, 5) == Math.Round(lst_Conn[1].Origin.X, 5))
                        {
                            XYZ V1 = new XYZ();
                            if (Math.Round(lst_Conn_Pipe[0].Origin.Z, 5) == Math.Round(Conn2.Origin.Z, 5) && Math.Round(lst_Conn_Pipe[0].Origin.X, 5) == Math.Round(Conn2.Origin.X, 5) && Math.Round(lst_Conn_Pipe[0].Origin.Y, 5) == Math.Round(Conn2.Origin.Y, 5))
                            {
                                V1 = new XYZ(1, lst_Conn_Pipe[1].Origin.Y - lst_Conn_Pipe[0].Origin.Y, 1);
                            }
                            else if (Math.Round(lst_Conn_Pipe[1].Origin.Z, 5) == Math.Round(Conn2.Origin.Z, 5) && Math.Round(lst_Conn_Pipe[1].Origin.X, 5) == Math.Round(Conn2.Origin.X, 5) && Math.Round(lst_Conn_Pipe[1].Origin.Y, 5) == Math.Round(Conn2.Origin.Y, 5))
                            {
                                V1 = new XYZ(1, lst_Conn_Pipe[0].Origin.Y - lst_Conn_Pipe[1].Origin.Y, 1);
                            }
                            if (XYZ.BasisY.Y / V1.Y > 0)
                            {
                                XYZ V = XYZ.BasisY * 60 / 304.8;
                                ElementTransformUtils.MoveElement(doc, Fitting1.Id, V);
                                result = "1";
                            }
                            else
                            {
                                XYZ V = -XYZ.BasisY * 60 / 304.8;
                                ElementTransformUtils.MoveElement(doc, Fitting1.Id, V);
                                result = "2";
                            }
                        }
                        else if (Math.Round(lst_Conn[0].Origin.Y, 5) == Math.Round(lst_Conn[1].Origin.Y, 5))
                        {
                            XYZ V1 = new XYZ();
                            if (Math.Round(lst_Conn_Pipe[0].Origin.Z, 5) == Math.Round(Conn2.Origin.Z, 5) && Math.Round(lst_Conn_Pipe[0].Origin.X, 5) == Math.Round(Conn2.Origin.X, 5) && Math.Round(lst_Conn_Pipe[0].Origin.Y, 5) == Math.Round(Conn2.Origin.Y, 5))
                            {
                                V1 = new XYZ(lst_Conn_Pipe[1].Origin.X - lst_Conn_Pipe[0].Origin.X, 1, 1);
                            }
                            else if (Math.Round(lst_Conn_Pipe[1].Origin.Z, 5) == Math.Round(Conn2.Origin.Z, 5) && Math.Round(lst_Conn_Pipe[1].Origin.X, 5) == Math.Round(Conn2.Origin.X, 5) && Math.Round(lst_Conn_Pipe[1].Origin.Y, 5) == Math.Round(Conn2.Origin.Y, 5))
                            {
                                V1 = new XYZ(lst_Conn_Pipe[0].Origin.X - lst_Conn_Pipe[1].Origin.X, 1, 1);
                            }
                            if (XYZ.BasisX.X / V1.X > 0)
                            {
                                XYZ V = XYZ.BasisX * 60 / 304.8;
                                ElementTransformUtils.MoveElement(doc, Fitting1.Id, V);
                                result = "3";
                            }
                            else
                            {
                                XYZ V = -XYZ.BasisX * 60 / 304.8;
                                ElementTransformUtils.MoveElement(doc, Fitting1.Id, V);
                                result = "4";
                            }
                        }
                    }
                }
            }
            return result;

        }
        public void MoveElement(Document doc, List<string> lst_Dk, List<string> lst_Dkk, List<FamilyInstance> lst_F)
        {
            for (int i = 0; i < clsBien_C1ET2E.lst_IdFit.Count; i++)
            {
                if (lst_Dk[i] == "OffsetZ")
                {
                    XYZ V = new XYZ(0, 0, 0 - (1 / 304.8));
                    ElementTransformUtils.MoveElement(doc, clsBien_C1ET2E.lst_IdFit[i], V);
                }
            }
            for (int i = 0; i < lst_F.Count; i++)
            {
                if (lst_Dkk[i] == "1")
                {
                    XYZ V = new XYZ(0, (1 / 304.8), 0);
                    ElementTransformUtils.MoveElement(doc, lst_F[i].Id, V);
                }
                else if (lst_Dkk[i] == "2")
                {
                    XYZ V = new XYZ(0, -(1 / 304.8), 0);
                    ElementTransformUtils.MoveElement(doc, lst_F[i].Id, V);
                }
                else if (lst_Dkk[i] == "3")
                {
                    XYZ V = new XYZ((1 / 304.8), 0, 0);
                    ElementTransformUtils.MoveElement(doc, lst_F[i].Id, V);
                }
                else if (lst_Dkk[i] == "4")
                {
                    XYZ V = new XYZ(-(1 / 304.8), 0, 0);
                    ElementTransformUtils.MoveElement(doc, lst_F[i].Id, V);
                }

            }
        }

        public List<Connector> List_Connector_Pipe(Element pipe_input)
        {
            List<Connector> result = new List<Connector>();
            Pipe p = pipe_input as Pipe;
            ConnectorSetIterator csi = null;
            csi = p.ConnectorManager.Connectors.ForwardIterator();
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                result.Add(conn);
            }
            return result;
        }
        public void Connect2Pipe(Document doc, Pipe pipe1, Pipe pipe2)
        {
            clsBien_C1ET2E.lst_Connector_Pipe1 = List_Connector_Pipe(pipe1);
            clsBien_C1ET2E.lst_Connector_Pipe2 = List_Connector_Pipe(pipe2);
            double distance1 = clsBien_C1ET2E.lst_Connector_Pipe1[0].Origin.DistanceTo(clsBien_C1ET2E.lst_Connector_Pipe2[0].Origin);
            double distance2 = clsBien_C1ET2E.lst_Connector_Pipe1[1].Origin.DistanceTo(clsBien_C1ET2E.lst_Connector_Pipe2[0].Origin);
            double distance3 = clsBien_C1ET2E.lst_Connector_Pipe1[0].Origin.DistanceTo(clsBien_C1ET2E.lst_Connector_Pipe2[1].Origin);
            double distance4 = clsBien_C1ET2E.lst_Connector_Pipe1[1].Origin.DistanceTo(clsBien_C1ET2E.lst_Connector_Pipe2[1].Origin);
            double min = distance1;
            Connector connector1 = null;
            Connector connector2 = null;
            if (min > distance2)
            {
                min = distance2;
                if (min > distance3)
                {
                    min = distance3;
                    if (min > distance4)
                    {
                        min = distance4;
                        connector1 = clsBien_C1ET2E.lst_Connector_Pipe1[1];
                        connector2 = clsBien_C1ET2E.lst_Connector_Pipe2[1];
                    }
                    else
                    {
                        connector1 = clsBien_C1ET2E.lst_Connector_Pipe1[0];
                        connector2 = clsBien_C1ET2E.lst_Connector_Pipe2[1];
                    }
                }
                else
                {
                    if (min > distance4)
                    {
                        min = distance4;
                        connector1 = clsBien_C1ET2E.lst_Connector_Pipe1[1];
                        connector2 = clsBien_C1ET2E.lst_Connector_Pipe2[1];
                    }
                    else
                    {
                        connector1 = clsBien_C1ET2E.lst_Connector_Pipe1[1];
                        connector2 = clsBien_C1ET2E.lst_Connector_Pipe2[0];
                    }
                }
            }
            else
            {
                if (min > distance3)
                {
                    min = distance3;
                    if (min > distance4)
                    {
                        min = distance4;
                        connector1 = clsBien_C1ET2E.lst_Connector_Pipe1[1];
                        connector2 = clsBien_C1ET2E.lst_Connector_Pipe2[1];
                    }
                    else
                    {
                        connector1 = clsBien_C1ET2E.lst_Connector_Pipe1[0];
                        connector2 = clsBien_C1ET2E.lst_Connector_Pipe2[1];
                    }
                }
                else
                {
                    if (min > distance4)
                    {
                        min = distance4;
                        connector1 = clsBien_C1ET2E.lst_Connector_Pipe1[1];
                        connector2 = clsBien_C1ET2E.lst_Connector_Pipe2[1];
                    }
                    else
                    {
                        connector1 = clsBien_C1ET2E.lst_Connector_Pipe1[0];
                        connector2 = clsBien_C1ET2E.lst_Connector_Pipe2[0];
                    }
                }
            }
            if (connector1 != null && connector2 != null)
            {
                doc.Create.NewElbowFitting(connector1, connector2);
            }
        }
        public void Align2Pipe(Document doc, Pipe pipe1, Pipe pipe2)
        {
            LocationCurve lc1 = pipe1.Location as LocationCurve;
            LocationCurve lc2 = pipe2.Location as LocationCurve;
            XYZ direction1 = (lc1.Curve.GetEndPoint(0) - lc1.Curve.GetEndPoint(1));
            Line l1 = Line.CreateUnbound(lc1.Curve.GetEndPoint(0), direction1);

            XYZ direction2 = (lc2.Curve.GetEndPoint(0) - lc2.Curve.GetEndPoint(1));
            Line l2 = Line.CreateUnbound(lc2.Curve.GetEndPoint(0), direction2);

            if (l1.Intersect(l2, out IntersectionResultArray results) == SetComparisonResult.Overlap)
            {
                
            }
            else if (l1.Intersect(l2) == SetComparisonResult.Disjoint)
            {
                Align(doc, pipe1, pipe2);
            }
        }
        public void Align(Document doc,Pipe pipe1, Pipe pipe2)
        {
            LocationCurve lc1 = pipe1.Location as LocationCurve;
            LocationCurve lc2 = pipe2.Location as LocationCurve;
            if (pipe1.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() != 0 && pipe2.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() != 0)
            {
                if (pipe1.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() > 3 || pipe2.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() > 3)
                {
                    if (pipe1.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() > 3)
                    {
                        Line l1 = lc1.Curve as Line;
                        Line l2 = lc2.Curve as Line;

                        XYZ ul2 = l2.GetEndPoint(1) - new XYZ(l2.GetEndPoint(0).X, l2.GetEndPoint(0).Y, l2.GetEndPoint(1).Z);

                        XYZ normal = ul2.Normalize();
                        XYZ dir = new XYZ(0, 0, 1);
                        XYZ cross = normal.CrossProduct(dir);

                        Line ll1 = Line.CreateUnbound(l2.GetEndPoint(0), ul2);
                        XYZ point = new XYZ(l1.GetEndPoint(0).X, l1.GetEndPoint(0).Y, l2.GetEndPoint(0).Z);
                        Line ll2 = Line.CreateUnbound(point, cross);
                        ll1.Intersect(ll2, out IntersectionResultArray results);
                        double kc = 0;
                        foreach (IntersectionResult item in results)
                        {
                            //MessageBox.Show(item.XYZPoint.ToString());
                            XYZ u1 = (item.XYZPoint - point).Normalize();
                            XYZ u2 = cross.Normalize();
                            if (u1.IsAlmostEqualTo(u2))
                            {
                                kc = -point.DistanceTo(item.XYZPoint);
                            }
                            else
                            {
                                kc = point.DistanceTo(item.XYZPoint);
                            }
                        }
                        ElementTransformUtils.MoveElement(doc, pipe2.Id, kc * cross);
                    }
                    else if (pipe2.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() > 3)
                    {
                        Line l2 = lc1.Curve as Line;
                        Line l1 = lc2.Curve as Line;

                        XYZ ul2 = l2.GetEndPoint(1) - new XYZ(l2.GetEndPoint(0).X, l2.GetEndPoint(0).Y, l2.GetEndPoint(1).Z);

                        XYZ normal = ul2.Normalize();
                        XYZ dir = new XYZ(0, 0, 1);
                        XYZ cross = normal.CrossProduct(dir);

                        Line ll1 = Line.CreateUnbound(l2.GetEndPoint(0), ul2);
                        XYZ point = new XYZ(l1.GetEndPoint(0).X, l1.GetEndPoint(0).Y, l2.GetEndPoint(0).Z);
                        Line ll2 = Line.CreateUnbound(point, cross);

                        ll1.Intersect(ll2, out IntersectionResultArray results);
                        double kc = 0;
                        foreach (IntersectionResult item in results)
                        {
                            //MessageBox.Show(item.XYZPoint.ToString());
                            XYZ u1 = (item.XYZPoint - point).Normalize();
                            XYZ u2 = cross.Normalize();
                            if (u1.IsAlmostEqualTo(u2))
                            {
                                kc = -point.DistanceTo(item.XYZPoint);
                            }
                            else
                            {
                                kc = point.DistanceTo(item.XYZPoint);
                            }
                        }
                        ElementTransformUtils.MoveElement(doc, pipe1.Id, kc * cross);

                    }
                }
                else if (pipe1.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() < 3 && pipe2.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() < 3)
                {
                   
                    double caodo1 = pipe1.LookupParameter("Middle Elevation").AsDouble();
                    double caodo2 = pipe2.LookupParameter("Middle Elevation").AsDouble();
                    if (caodo1 >= caodo2)
                    {
                        Line l1 = lc1.Curve as Line;
                        Line l2 = lc2.Curve as Line;

                        Plane plane1 = Plane.CreateByThreePoints(l2.GetEndPoint(0), l2.GetEndPoint(1), new XYZ(l2.GetEndPoint(1).X, l2.GetEndPoint(1).Y, l2.GetEndPoint(1).Z + 10));
                        XYZ point1 = LinePlaneIntersection(l1, plane1, out double lineParameter1);

                        Plane plane2 = Plane.CreateByThreePoints(l1.GetEndPoint(0), l1.GetEndPoint(1), new XYZ(l1.GetEndPoint(1).X, l1.GetEndPoint(1).Y, l1.GetEndPoint(1).Z + 10));
                        XYZ point2 = LinePlaneIntersection(l2, plane2, out double lineParameter2);

                        double kc = 0;
                        XYZ u = (point1 - point2).Normalize();
                        if (u.IsAlmostEqualTo(XYZ.BasisZ))
                        {
                            kc = - point1.DistanceTo(point2);
                        }
                        else
                        {
                            kc = point1.DistanceTo(point2);
                        }
                        ElementTransformUtils.MoveElement(doc, pipe1.Id, kc * XYZ.BasisZ);
                    }
                    else 
                    {
                        Line l2 = lc1.Curve as Line;
                        Line l1 = lc2.Curve as Line;

                        Plane plane1 = Plane.CreateByThreePoints(l2.GetEndPoint(0), l2.GetEndPoint(1), new XYZ(l2.GetEndPoint(1).X, l2.GetEndPoint(1).Y, l2.GetEndPoint(1).Z + 10));
                        XYZ point1 = LinePlaneIntersection(l1, plane1, out double lineParameter1);

                        Plane plane2 = Plane.CreateByThreePoints(l1.GetEndPoint(0), l1.GetEndPoint(1), new XYZ(l1.GetEndPoint(1).X, l1.GetEndPoint(1).Y, l1.GetEndPoint(1).Z + 10));
                        XYZ point2 = LinePlaneIntersection(l2, plane2, out double lineParameter2);

                        double kc = 0;
                        XYZ u = (point1 - point2).Normalize();
                        if (u.IsAlmostEqualTo(XYZ.BasisZ))
                        {
                            kc = - point1.DistanceTo(point2);
                        }
                        else
                        {
                            kc =  point1.DistanceTo(point2);
                        }
                        ElementTransformUtils.MoveElement(doc, pipe2.Id, kc * XYZ.BasisZ);

                    }
                }
            }
            else if (pipe1.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() == 0 && pipe2.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() != 0)
            {
                if (pipe2.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() > 3)
                {
                    //1 ngang
                    //2 dung
                    Line l1 = lc1.Curve as Line;
                    Line l2 = lc2.Curve as Line;

                    XYZ normal = l1.Direction.Normalize();
                    XYZ dir = new XYZ(0, 0, 1);
                    XYZ cross = normal.CrossProduct(dir);

                    Line ll1 = Line.CreateUnbound(l1.GetEndPoint(0), l1.Direction);
                    XYZ point = new XYZ(l2.GetEndPoint(0).X, l2.GetEndPoint(0).Y, l1.GetEndPoint(0).Z);
                    Line ll2 = Line.CreateUnbound(point, cross);

                    ll1.Intersect(ll2, out IntersectionResultArray results);

                    double kc = 0;
                    foreach (IntersectionResult item in results)
                    {
                        XYZ u1 = (item.XYZPoint - point).Normalize();
                        XYZ u2 = cross.Normalize();
                        if (u1.IsAlmostEqualTo(u2))
                        {
                            kc = -point.DistanceTo(item.XYZPoint);
                        }
                        else
                        {
                            kc = point.DistanceTo(item.XYZPoint);
                        }   
                    }
                    ElementTransformUtils.MoveElement(doc, pipe1.Id, kc * cross);
                }
                else if(pipe2.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() < 3)
                {
                    //1 dung
                    //2 ngang

                    Line l1 = lc1.Curve as Line;
                    Line l2 = lc2.Curve as Line;

                    XYZ ul2 = l2.GetEndPoint(1) - new XYZ(l2.GetEndPoint(0).X, l2.GetEndPoint(0).Y, l2.GetEndPoint(1).Z);

                    XYZ normal = ul2.Normalize();
                    XYZ dir = new XYZ(0, 0, 1);
                    XYZ cross = normal.CrossProduct(dir);

                    Line ll1 = Line.CreateUnbound(l2.GetEndPoint(0), ul2);
                    XYZ point = new XYZ(l1.GetEndPoint(0).X, l1.GetEndPoint(0).Y, l2.GetEndPoint(0).Z);
                    Line ll2 = Line.CreateUnbound(point, cross);

                    ll1.Intersect(ll2, out IntersectionResultArray results);

                    double kc = 0;
                    foreach (IntersectionResult item in results)
                    {
                        XYZ u1 = (item.XYZPoint - point).Normalize();
                        XYZ u2 = cross.Normalize();
                        if (u1.IsAlmostEqualTo(u2))
                        {
                            kc = -point.DistanceTo(item.XYZPoint);
                        }
                        else
                        {
                            kc = point.DistanceTo(item.XYZPoint);
                        }
                    }
                    ElementTransformUtils.MoveElement(doc, pipe2.Id, kc * cross);
                }
            }
            else if (pipe1.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() != 0 && pipe2.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() == 0)
            {
                if (pipe1.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() < 3)
                {
                    //1 ngang
                    //2 dung
                    Line l1 = lc2.Curve as Line;
                    Line l2 = lc1.Curve as Line;

                    XYZ ul2 = l2.GetEndPoint(1) - new XYZ(l2.GetEndPoint(0).X, l2.GetEndPoint(0).Y, l2.GetEndPoint(1).Z);

                    XYZ normal = ul2.Normalize();
                    XYZ dir = new XYZ(0, 0, 1);
                    XYZ cross = normal.CrossProduct(dir);

                    Line ll1 = Line.CreateUnbound(l2.GetEndPoint(0), ul2);
                    XYZ point = new XYZ(l1.GetEndPoint(0).X, l1.GetEndPoint(0).Y, l2.GetEndPoint(0).Z);
                    Line ll2 = Line.CreateUnbound(point, cross);

                    ll1.Intersect(ll2, out IntersectionResultArray results);

                    double kc = 0;
                    foreach (IntersectionResult item in results)
                    {
                        XYZ u1 = (item.XYZPoint - point).Normalize();
                        XYZ u2 = cross.Normalize();
                        if (u1.IsAlmostEqualTo(u2))
                        {
                            kc = -point.DistanceTo(item.XYZPoint);
                        }
                        else
                        {
                            kc = point.DistanceTo(item.XYZPoint);
                        }
                    }
                    ElementTransformUtils.MoveElement(doc, pipe1.Id, kc * cross);
                }
                else if (pipe1.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() > 3)
                {
                    //1 dung
                    //2 ngang

                    Line l1 = lc1.Curve as Line;
                    Line l2 = lc2.Curve as Line;

                    XYZ ul2 = l2.GetEndPoint(1) - new XYZ(l2.GetEndPoint(0).X, l2.GetEndPoint(0).Y, l2.GetEndPoint(1).Z);

                    XYZ normal = ul2.Normalize();
                    XYZ dir = new XYZ(0, 0, 1);
                    XYZ cross = normal.CrossProduct(dir);

                    Line ll1 = Line.CreateUnbound(l2.GetEndPoint(0), ul2);
                    XYZ point = new XYZ(l1.GetEndPoint(0).X, l1.GetEndPoint(0).Y, l2.GetEndPoint(0).Z);
                    Line ll2 = Line.CreateUnbound(point, cross);

                    ll1.Intersect(ll2, out IntersectionResultArray results);

                    double kc = 0;
                    foreach (IntersectionResult item in results)
                    {
                        XYZ u1 = (item.XYZPoint - point).Normalize();
                        XYZ u2 = cross.Normalize();
                        if (u1.IsAlmostEqualTo(u2))
                        {
                            kc = -point.DistanceTo(item.XYZPoint);
                        }
                        else
                        {
                            kc = point.DistanceTo(item.XYZPoint);
                        }
                    }
                    ElementTransformUtils.MoveElement(doc, pipe2.Id, kc * cross);
                }
            }
            else if (pipe1.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() == 0 && pipe2.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() == 0)
            {
                if (Math.Round(lc1.Curve.GetEndPoint(0).Z,5) != Math.Round( lc1.Curve.GetEndPoint(1).Z,5) && Math.Round(lc2.Curve.GetEndPoint(0).Z, 5) == Math.Round(lc2.Curve.GetEndPoint(1).Z, 5))
                {
                    Line l1 = lc1.Curve as Line;
                    Line l2 = lc2.Curve as Line;    

                    XYZ normal = l2.Direction.Normalize();
                    XYZ dir = new XYZ(0, 0, 1);
                    XYZ cross = normal.CrossProduct(dir);

                    Line ll1 = Line.CreateUnbound(l2.GetEndPoint(0), l2.Direction);
                    XYZ point = new XYZ(l1.GetEndPoint(0).X, l1.GetEndPoint(0).Y, l2.GetEndPoint(0).Z);
                    Line ll2 = Line.CreateUnbound(point, cross);
                    ll1.Intersect(ll2, out IntersectionResultArray results);
                    double kc = 0;
                    foreach (IntersectionResult item in results)
                    {
                        //MessageBox.Show(item.XYZPoint.ToString());
                        XYZ u1 = (item.XYZPoint - point).Normalize();
                        XYZ u2 = cross.Normalize();
                        if (u1.IsAlmostEqualTo(u2))
                        {
                            kc = - point.DistanceTo(item.XYZPoint);
                        }
                        else
                        {
                            kc =  point.DistanceTo(item.XYZPoint);
                        }                     
                    }
                    
                        
                    ElementTransformUtils.MoveElement(doc, pipe2.Id, kc * cross);
                }
                else if (Math.Round(lc1.Curve.GetEndPoint(0).Z, 5) == Math.Round(lc1.Curve.GetEndPoint(1).Z, 5) && Math.Round(lc2.Curve.GetEndPoint(0).Z, 5) != Math.Round(lc2.Curve.GetEndPoint(1).Z, 5))
                {
                    Line l1 = lc2.Curve as Line;
                    Line l2 = lc1.Curve as Line;

                    XYZ normal = l2.Direction.Normalize();
                    XYZ dir = new XYZ(0, 0, 1);
                    XYZ cross = normal.CrossProduct(dir);

                    Line ll1 = Line.CreateUnbound(l2.GetEndPoint(0), l2.Direction);
                    XYZ point = new XYZ(l1.GetEndPoint(0).X, l1.GetEndPoint(0).Y, l2.GetEndPoint(0).Z);
                    Line ll2 = Line.CreateUnbound(point, cross);
                    ll1.Intersect(ll2, out IntersectionResultArray results);
                    double kc = 0;
                    foreach (IntersectionResult item in results)
                    {
                        //MessageBox.Show(item.XYZPoint.ToString());
                        XYZ u1 = (item.XYZPoint - point).Normalize();
                        XYZ u2 = cross.Normalize();
                        if (u1.IsAlmostEqualTo(u2))
                        {
                            kc = - point.DistanceTo(item.XYZPoint);
                        }
                        else
                        {
                            kc = point.DistanceTo(item.XYZPoint);
                        }
                    }
                    ElementTransformUtils.MoveElement(doc, pipe1.Id, kc * cross);

                }
            }
        }
        public static XYZ LinePlaneIntersection(Line line,Plane plane, out double lineParameter)
        {
            XYZ planePoint = plane.Origin;
            XYZ planeNormal = plane.Normal;
            XYZ linePoint = line.GetEndPoint(0);

            XYZ lineDirection = (line.GetEndPoint(1) - linePoint).Normalize();

            // Is the line parallel to the plane, i.e.,
            // perpendicular to the plane normal?

            if (planeNormal.DotProduct(lineDirection) == 0)
            {
                lineParameter = double.NaN;
                return null;
            }

            lineParameter = (planeNormal.DotProduct(planePoint)- planeNormal.DotProduct(linePoint))/ planeNormal.DotProduct(lineDirection);

            return linePoint + lineParameter * lineDirection;
        }
    }
}
