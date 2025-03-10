using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEP_Tools.CreateSiphon
{
    public class Func_Siphon
    {
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
        public void Align(Document doc, Pipe pipe1, Pipe pipe2)
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
                            kc = -point1.DistanceTo(point2);
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
                            kc = -point1.DistanceTo(point2);
                        }
                        else
                        {
                            kc = point1.DistanceTo(point2);
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
                else if (pipe2.get_Parameter(BuiltInParameter.RBS_CURVE_SLOPE).AsDouble() < 3)
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
                if (Math.Round(lc1.Curve.GetEndPoint(0).Z, 5) != Math.Round(lc1.Curve.GetEndPoint(1).Z, 5) && Math.Round(lc2.Curve.GetEndPoint(0).Z, 5) == Math.Round(lc2.Curve.GetEndPoint(1).Z, 5))
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
                            kc = -point.DistanceTo(item.XYZPoint);
                        }
                        else
                        {
                            kc = point.DistanceTo(item.XYZPoint);
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
        }
        public static XYZ LinePlaneIntersection(Line line, Plane plane, out double lineParameter)
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

            lineParameter = (planeNormal.DotProduct(planePoint) - planeNormal.DotProduct(linePoint)) / planeNormal.DotProduct(lineDirection);

            return linePoint + lineParameter * lineDirection;
        }

        public IList<Connector> FindConnectorofFamily(FamilyInstance fI)
        {
            ConnectorSet conns = null;           
            conns = fI.MEPModel.ConnectorManager.Connectors;
            List<Connector> results = new List<Connector>();
            foreach (Connector conn in conns)
            {
                results.Add(conn);
            }
            return results;
        }
        public Connector FindConnector(FamilyInstance fI, XYZ conXYZ)
        {
            ConnectorSet conns = null;
            conns = fI.MEPModel.ConnectorManager.Connectors;
            foreach (Connector conn in conns)
            {
                if (conn.Origin.IsAlmostEqualTo(conXYZ))
                {
                    return conn;
                }
            }
            return null;
        }
        public Connector FindConnectedTo(FamilyInstance fI, XYZ conXYZ)
        {
            Connector connItself = FindConnector(fI, conXYZ);
            ConnectorSet connSet = connItself.AllRefs;
            foreach (Connector conn in connSet)
            {
                if (conn.Owner.Id.IntegerValue != fI.Id.IntegerValue &&
                    conn.ConnectorType == ConnectorType.End)
                {
                    return conn;
                }
            }
            return null;
        }
        public void DisConnectedTo(FamilyInstance fI, XYZ conXYZ, Connector conn)
        {
            Connector connItself = FindConnector(fI, conXYZ);
            connItself.DisconnectFrom(conn);
        }
        public XYZ FindAxis(Connector conn1, Connector conn2, FamilyInstance fI)
        {
            //Connector connItself = FindConnector(fI, conXYZ);
            ConnectorSet connSet1 = conn1.AllRefs;
            LocationCurve lc1 = null;
            Pipe pipe1 = null, pipe2 = null;
            foreach (Connector conn in connSet1)
            {
                if (conn.Owner.Id.IntegerValue != fI.Id.IntegerValue &&
                    conn.ConnectorType == ConnectorType.End)
                {
                    pipe1 = conn.Owner as Pipe;
                }
            }

            ConnectorSet connSet2 = conn2.AllRefs;
            LocationCurve lc2 = null;
            foreach (Connector conn in connSet2)
            {
                if (conn.Owner.Id.IntegerValue != fI.Id.IntegerValue &&
                    conn.ConnectorType == ConnectorType.End)
                {
                    pipe2 = conn.Owner as Pipe;
                }
            }
            lc1 = pipe1.Location as LocationCurve;
            lc2 = pipe2.Location as LocationCurve;
            if (pipe1.LookupParameter("Slope").AsDouble() == 0)
            {
                if (pipe2.LookupParameter("Slope").AsDouble() == 0)
                {                   
                    if (Math.Round(lc1.Curve.GetEndPoint(0).Z,5) == Math.Round(lc1.Curve.GetEndPoint(1).Z,5))
                    {
                        Line l1 = lc1.Curve as Line;
                        return l1.Direction;
                    }
                    else if (Math.Round(lc2.Curve.GetEndPoint(0).Z,5) == Math.Round(lc2.Curve.GetEndPoint(1).Z,5))
                    {
                        Line l2 = lc2.Curve as Line;
                        return l2.Direction;
                    }                  
                }
                else if (pipe2.LookupParameter("Slope").AsDouble() < 3)
                {
                    Line l2 = lc2.Curve as Line;
                    return l2.Direction;
                }
            }
            else if (pipe1.LookupParameter("Slope").AsDouble() < 3)
            {
                Line l1 = lc2.Curve as Line;
                return l1.Direction;
            }
            return null;
        }
    }
}
