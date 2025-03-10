using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEP_Tools
{
    class Func_MUD
    {
        public string CheckType(Document doc, ElementId ids)
        {
            string result = "";
            Element ele = doc.GetElement(ids);
            if (ele is Duct)
            {
                result = "Duct";
                Duct dt = ele as Duct;
                clsBien_MUD.DuctTypeOld = dt.DuctType;
                try
                {
                    clsBien_MUD.Width = dt.LookupParameter("Width").AsValueString();
                    clsBien_MUD.Height = dt.LookupParameter("Height").AsValueString();
                }
                catch
                {
                    clsBien_MUD.Diameter = dt.LookupParameter("Diameter").AsValueString();
                }
            }
            else if (ele is Pipe)
            {
                result = "Pipe";
                Pipe pi = ele as Pipe;
                clsBien_MUD.PipeTypeOld = pi.PipeType;
                clsBien_MUD.Diameter = pi.LookupParameter("Diameter").AsValueString();
            }
            else if (ele is CableTray)
            {
                result = "CableTray";
                CableTray cbt = ele as CableTray;
                clsBien_MUD.CbTrayTypeOldID = cbt.GetTypeId();
                clsBien_MUD.Width = cbt.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM).AsValueString();
                clsBien_MUD.Height = cbt.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM).AsValueString();
            }
            else if (ele is Conduit)
            {
                result = "Conduit";
                Conduit cd = ele as Conduit;
                clsBien_MUD.ConduitTypeOldID = cd.GetTypeId();
                clsBien_MUD.Diameter = cd.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).AsValueString();
            }
            return result;
        }
        public List<Connector> GetConnector(Document doc, string FType, ElementId ids)
        {
            Element ele = doc.GetElement(ids);
            Pipe p = null;
            Duct dt = null;
            CableTray Cbt = null;
            Conduit Cd = null;
            ConnectorSetIterator csi = null;
            List<Connector> result = new List<Connector>();
            if (FType == "Duct")
            {
                dt = ele as Duct;
                csi = dt.ConnectorManager.Connectors.ForwardIterator();
                while (csi.MoveNext())
                {
                    Connector conn = csi.Current as Connector;
                    var all_Ref = conn.AllRefs;
                    List<Element> ConnectedElements = new List<Element>();
                    foreach (Connector connector in all_Ref)
                    {
                        ConnectedElements.Add(connector.Owner);
                    }
                    FamilyInstance F1 = null;
                    foreach (Element Fa in ConnectedElements)
                    {
                        if (Fa is FamilyInstance)
                        {
                            F1 = Fa as FamilyInstance;
                            if (F1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsValueString() != "")
                            {
                                Parameter partTypeParam1 = F1.Symbol.Family.get_Parameter(BuiltInParameter.FAMILY_CONTENT_PART_TYPE);
                                if (partTypeParam1.AsValueString() == "Union" || partTypeParam1.AsValueString() == "Elbow")
                                {
                                    result.Add(conn);
                                }
                            }
                        }
                    }

                }
            }
            else if (FType == "Pipe")
            {
                p = ele as Pipe;
                csi = p.ConnectorManager.Connectors.ForwardIterator();

                while (csi.MoveNext())
                {
                    Connector conn = csi.Current as Connector;
                    var all_Ref = conn.AllRefs;
                    List<Element> ConnectedElements = new List<Element>();
                    foreach (Connector connector in all_Ref)
                    {
                        ConnectedElements.Add(connector.Owner);
                    }
                    FamilyInstance F1 = null;
                    foreach (Element Fa in ConnectedElements)
                    {
                        if (Fa is FamilyInstance)
                        {
                            F1 = Fa as FamilyInstance;
                            if (F1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsValueString() != "")
                            {
                                Parameter partTypeParam1 = F1.Symbol.Family.get_Parameter(BuiltInParameter.FAMILY_CONTENT_PART_TYPE);
                                if (partTypeParam1.AsValueString() == "Union" || partTypeParam1.AsValueString() == "Elbow")
                                {
                                    result.Add(conn);
                                }
                            }
                        }
                    }
                }
            }
            else if (FType == "CableTray")
            {
                Cbt = ele as CableTray;
                csi = Cbt.ConnectorManager.Connectors.ForwardIterator();
                while (csi.MoveNext())
                {
                    Connector conn = csi.Current as Connector;
                    var all_Ref = conn.AllRefs;
                    List<Element> ConnectedElements = new List<Element>();
                    foreach (Connector connector in all_Ref)
                    {
                        ConnectedElements.Add(connector.Owner);
                    }
                    FamilyInstance F1 = null;
                    foreach (Element Fa in ConnectedElements)
                    {
                        if (Fa is FamilyInstance)
                        {
                            F1 = Fa as FamilyInstance;
                            if (F1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsValueString() != "")
                            {
                                Parameter partTypeParam1 = F1.Symbol.Family.get_Parameter(BuiltInParameter.FAMILY_CONTENT_PART_TYPE);
                                if (partTypeParam1.AsValueString() == "Channel Union" || partTypeParam1.AsValueString() == "Channel Vertical Elbow" || partTypeParam1.AsValueString().Contains("Union") || partTypeParam1.AsValueString().Contains("Elbow"))
                                {
                                    result.Add(conn);
                                }
                            }
                        }
                    }
                }
            }
            else if (FType == "Conduit")
            {
                Cd = ele as Conduit;
                csi = Cd.ConnectorManager.Connectors.ForwardIterator();

                while (csi.MoveNext())
                {
                    Connector conn = csi.Current as Connector;
                    var all_Ref = conn.AllRefs;
                    List<Element> ConnectedElements = new List<Element>();
                    foreach (Connector connector in all_Ref)
                    {
                        ConnectedElements.Add(connector.Owner);
                    }
                    FamilyInstance F1 = null;
                    foreach (Element Fa in ConnectedElements)
                    {
                        if (Fa is FamilyInstance)
                        {
                            F1 = Fa as FamilyInstance;
                            if (F1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsValueString() != "" )
                            {
                                Parameter partTypeParam1 = F1.Symbol.Family.get_Parameter(BuiltInParameter.FAMILY_CONTENT_PART_TYPE);
                                if (partTypeParam1.AsValueString() == "Union" || partTypeParam1.AsValueString() == "Elbow")
                                {
                                    result.Add(conn);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
        public void GetFitting()
        {
            ElementId id1 = null;
            ElementId id2 = null;
            FamilyInstance F1 = null;
            FamilyInstance F2 = null;
            List<Element> ConnectedElements = new List<Element>();
            if (clsBien_MUD.List_Connect.Count == 0)
            {
                clsBien_MUD.Id_Fitting1 = null;
                clsBien_MUD.Id_Fitting2 = null;
            }
            else if(clsBien_MUD.List_Connect.Count == 1)
            {
                var allRef = clsBien_MUD.List_Connect[0].AllRefs;
                foreach (Connector connector in allRef)
                {
                    ConnectedElements.Add(connector.Owner);
                }
                foreach (Element ele in ConnectedElements)
                {
                    if (ele is FamilyInstance)
                    {
                        F1 = ele as FamilyInstance;
                        id1 = ele.Id;
                        clsBien_MUD.ConnectorWithUnion1 = clsBien_MUD.List_Connect[0];
                    }
                }
                clsBien_MUD.Id_Fitting2 = null;
            }
            else if (clsBien_MUD.List_Connect.Count == 2)
            {
                var allRef = clsBien_MUD.List_Connect[0].AllRefs;              
                foreach (Connector connector in allRef)
                {
                    ConnectedElements.Add(connector.Owner);
                }
                foreach (Element ele in ConnectedElements)
                {
                    if (ele is FamilyInstance)
                    {
                        F1 = ele as FamilyInstance;
                        id1 = ele.Id;
                        clsBien_MUD.ConnectorWithUnion1 = clsBien_MUD.List_Connect[0];
                    }
                }

                List<Element> ConnectedElements2 = new List<Element>();
                var allRef2 = clsBien_MUD.List_Connect[1].AllRefs;
                foreach (Connector connector in allRef2)
                {
                    ConnectedElements2.Add(connector.Owner);
                }
                foreach (Element ele in ConnectedElements2)
                {
                    if (ele is FamilyInstance)
                    {
                        F2 = ele as FamilyInstance;
                        id2 = ele.Id;
                        clsBien_MUD.ConnectorWithUnion2 = clsBien_MUD.List_Connect[1];
                    }
                }
            }
            if (id1 != null && id2 != null)
            {
                Parameter partTypeParam1 = F1.Symbol.Family.get_Parameter(BuiltInParameter.FAMILY_CONTENT_PART_TYPE);
                Parameter partTypeParam2 = F2.Symbol.Family.get_Parameter(BuiltInParameter.FAMILY_CONTENT_PART_TYPE);
                Parameter CommmentsParam1 = F1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                Parameter CommmentsParam2 = F2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                if (partTypeParam1.AsValueString() == "Union" && partTypeParam2.AsValueString() == "Union")
                {
                    clsBien_MUD.Id_Fitting1 = id1;
                    clsBien_MUD.Id_Fitting2 = id2;
                    clsBien_MUD.FittingType = "Union";
                }
                else if (partTypeParam1.AsValueString() == "Union" && partTypeParam2.AsValueString() != "Union")
                {
                    clsBien_MUD.Id_Fitting1 = id1;
                    clsBien_MUD.Id_Fitting2 = null;
                    clsBien_MUD.FittingType = "Union";
                }
                else if (partTypeParam1.AsValueString() != "Union" && partTypeParam2.AsValueString() == "Union")
                {
                    clsBien_MUD.Id_Fitting1 = null;
                    clsBien_MUD.Id_Fitting2 = id2;
                    clsBien_MUD.FittingType = "Union";
                }
                else if (partTypeParam1.AsValueString().Contains("Union") && partTypeParam2.AsValueString().Contains("Union"))
                {
                    clsBien_MUD.Id_Fitting1 = id1;
                    clsBien_MUD.Id_Fitting2 = id2;
                    clsBien_MUD.FittingType = "Union";
                }
                else if (partTypeParam1.AsValueString().Contains("Union") && !partTypeParam1.AsValueString().Contains("Union"))
                {
                    clsBien_MUD.Id_Fitting1 = id1;
                    clsBien_MUD.Id_Fitting2 = null;
                    clsBien_MUD.FittingType = "Union";
                }
                else if (!partTypeParam1.AsValueString().Contains("Union") && partTypeParam2.AsValueString().Contains("Union"))
                {
                    clsBien_MUD.Id_Fitting1 = null;
                    clsBien_MUD.Id_Fitting2 = id2;
                    clsBien_MUD.FittingType = "Union";
                }
                else if (partTypeParam1.AsValueString() == "Elbow" && partTypeParam2.AsValueString() == "Elbow")
                {
                    string st1 = CommmentsParam1.AsString();
                    string st2 = CommmentsParam2.AsString();
                    if (CommmentsParam1.AsString() != null && CommmentsParam1.AsString() != "")
                    {
                        if (CommmentsParam2.AsString() != null && CommmentsParam2.AsString() != "")
                        {
                            clsBien_MUD.Id_Fitting1 = id1;
                            clsBien_MUD.Id_Fitting2 = id2;
                        }
                        else
                        {
                            clsBien_MUD.Id_Fitting1 = id1;
                            clsBien_MUD.Id_Fitting2 = null;
                        }
                    }
                    else
                    {
                        if (CommmentsParam2.AsString() != null && CommmentsParam2.AsString() != "")
                        {
                            clsBien_MUD.Id_Fitting1 = null;
                            clsBien_MUD.Id_Fitting2 = id2;
                        }
                        else
                        {
                            clsBien_MUD.Id_Fitting1 = null;
                            clsBien_MUD.Id_Fitting2 = null;
                        }
                    }
                    clsBien_MUD.FittingType = "Elbow";
                }
                else if (partTypeParam1.AsValueString() == "Elbow" && partTypeParam2.AsValueString() != "Elbow")
                {
                    if (CommmentsParam1.AsString() != null && CommmentsParam1.AsString() != "")
                    {
                        clsBien_MUD.Id_Fitting1 = id1;
                        clsBien_MUD.Id_Fitting2 = null;
                    }
                    else
                    {
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = null;
                    }
                    clsBien_MUD.FittingType = "Elbow";
                }
                else if (partTypeParam1.AsValueString() != "Elbow" && partTypeParam2.AsValueString() == "Elbow")
                {
                    if (CommmentsParam2.AsString() != null && CommmentsParam2.AsString() != "")
                    {
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = id2;
                    }
                    else
                    {
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = null;
                    }
                    clsBien_MUD.FittingType = "Elbow";
                }
                else if (partTypeParam1.AsValueString().Contains("Elbow") && partTypeParam2.AsValueString().Contains("Elbow"))
                {
                    if (CommmentsParam1.AsString() != null && CommmentsParam1.AsString() != "")
                    {
                        if (CommmentsParam2.AsValueString() != "")
                        {
                            clsBien_MUD.Id_Fitting1 = id1;
                            clsBien_MUD.Id_Fitting2 = id2;
                        }
                        else
                        {
                            clsBien_MUD.Id_Fitting1 = id1;
                            clsBien_MUD.Id_Fitting2 = null;
                        }
                    }
                    else
                    {
                        if (CommmentsParam2.AsString() != null && CommmentsParam2.AsString() != "")
                        {
                            clsBien_MUD.Id_Fitting1 = null;
                            clsBien_MUD.Id_Fitting2 = id2;
                        }
                        else
                        {
                            clsBien_MUD.Id_Fitting1 = null;
                            clsBien_MUD.Id_Fitting2 = null;
                        }
                    }
                    clsBien_MUD.FittingType = "Elbow";
                }
                else if (partTypeParam1.AsValueString().Contains("Elbow") && !partTypeParam2.AsValueString().Contains("Elbow"))
                {
                    if (CommmentsParam1.AsString() != null && CommmentsParam1.AsString() != "")
                    {
                        clsBien_MUD.Id_Fitting1 = id1;
                        clsBien_MUD.Id_Fitting2 = null;
                    }
                    else
                    {
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = null;
                    }
                    clsBien_MUD.FittingType = "Elbow";
                }
                else if (!partTypeParam1.AsValueString().Contains("Elbow") && partTypeParam2.AsValueString().Contains("Elbow"))
                {
                    if (CommmentsParam2.AsString() != null && CommmentsParam2.AsString() != "")
                    {
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = id2;
                    }
                    else
                    {
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = null;
                    }
                    clsBien_MUD.FittingType = "Elbow";
                }
            }
            else if (id1 == null && id2 != null)
            {
                Parameter partTypeParam2 = F2.Symbol.Family.get_Parameter(BuiltInParameter.FAMILY_CONTENT_PART_TYPE);
                Parameter CommmentsParam2 = F2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                if (partTypeParam2.AsValueString() == "Union")
                {
                    clsBien_MUD.Id_Fitting1 = null;
                    clsBien_MUD.Id_Fitting2 = id2;
                    clsBien_MUD.FittingType = "Union";
                }
                else if (partTypeParam2.AsValueString() == "Channel Union")
                {
                    clsBien_MUD.Id_Fitting1 = null;
                    clsBien_MUD.Id_Fitting2 = id2;
                    clsBien_MUD.FittingType = "Union";
                }
                else if (partTypeParam2.AsValueString() == "Elbow")
                {

                    if (CommmentsParam2.AsString() != null && CommmentsParam2.AsString() != "")
                    {
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = id2;
                    }
                    else
                    {
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = null;
                    }
                    clsBien_MUD.FittingType = "Elbow";
                }
                else if (partTypeParam2.AsValueString() == "Channel Vertical Elbow")
                {
                    if (CommmentsParam2.AsString() != null && CommmentsParam2.AsString() != "")
                    {
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = id2;
                    }
                    else
                    {
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = null;
                    }
                    clsBien_MUD.FittingType = "Elbow";
                }
            }
            else if (id2 == null && id1 != null)
            {
                Parameter partTypeParam1 = F1.Symbol.Family.get_Parameter(BuiltInParameter.FAMILY_CONTENT_PART_TYPE);
                Parameter CommmentsParam1 = F1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                if (partTypeParam1.AsValueString() == "Union")
                {
                    clsBien_MUD.Id_Fitting1 = id1;
                    clsBien_MUD.Id_Fitting2 = null;
                    clsBien_MUD.FittingType = "Union";
                }
                else if (partTypeParam1.AsValueString() == "Channel Union")
                {
                    clsBien_MUD.Id_Fitting1 = id1;
                    clsBien_MUD.Id_Fitting2 = null;
                    clsBien_MUD.FittingType = "Union";
                }
                else if (partTypeParam1.AsValueString() == "Elbow")
                {
                    if (CommmentsParam1.AsString() != null && CommmentsParam1.AsString() != "")
                    {
                        clsBien_MUD.Id_Fitting1 = id1;
                        clsBien_MUD.Id_Fitting2 = null;
                    }
                    else
                    {
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = null;
                    }
                    clsBien_MUD.FittingType = "Elbow";
                }
                else if (partTypeParam1.AsValueString() == "Channel Vertical Elbow")
                {
                    if (CommmentsParam1.AsString() != null && CommmentsParam1.AsString() != "")
                    {
                        clsBien_MUD.Id_Fitting1 = id1;
                        clsBien_MUD.Id_Fitting2 = null;
                    }
                    else
                    {
                        clsBien_MUD.Id_Fitting1 = null;
                        clsBien_MUD.Id_Fitting2 = null;
                    }
                    clsBien_MUD.FittingType = "Elbow";
                }
            }
            else if (id2 == null && id1 == null)
            {
                clsBien_MUD.Id_Fitting1 = null;
                clsBien_MUD.Id_Fitting2 = null;
            }
        }
        public Connector Disconnector(Document doc, Connector con, ElementId Id_Fitting)
        {
            Element ele = doc.GetElement(Id_Fitting);
            ConnectorSetIterator csi = null;
            Connector conn = null;
            Connector connUnion = null;
            FamilyInstance F = ele as FamilyInstance;
            csi = F.MEPModel.ConnectorManager.Connectors.ForwardIterator();
            while (csi.MoveNext())
            {
                conn = csi.Current as Connector;
                if (Math.Round(conn.Origin.X, 5) == Math.Round(con.Origin.X, 5) && Math.Round(conn.Origin.Y, 5) == Math.Round(con.Origin.Y, 5))
                {
                    con.DisconnectFrom(conn);
                }
                else
                {
                    connUnion = conn;
                }
            }
            return connUnion;
        }
        public void OffsetElement(Document doc, Element ElementOld, string FType, double OffsetValue)
        {
            double ME = 0;
            switch (FType)
            {
                case "Duct":
                    Duct Dt = ElementOld as Duct;
                    ME = Dt.LookupParameter("Middle Elevation").AsDouble();
                    ME = ME + OffsetValue / 304.8;
                    Dt.LookupParameter("Middle Elevation").Set(ME);
                    break;
                case "Pipe":
                    Pipe P = ElementOld as Pipe;
                    ME = P.LookupParameter("Middle Elevation").AsDouble();
                    ME = ME + OffsetValue / 304.8;
                    P.LookupParameter("Middle Elevation").Set(ME);
                    break;
                case "CableTray":
                    CableTray Cbt = ElementOld as CableTray;
                    ME = Cbt.LookupParameter("Middle Elevation").AsDouble();
                    ME = ME + OffsetValue / 304.8;
                    Cbt.LookupParameter("Middle Elevation").Set(ME);
                    break;
                case "Conduit":
                    Conduit Cd = ElementOld as Conduit;
                    ME = Cd.LookupParameter("Middle Elevation").AsDouble();
                    ME = ME + OffsetValue / 304.8;
                    Cd.LookupParameter("Middle Elevation").Set(ME);
                    break;
            }
        }
        public ElementId GetDuctSystemTypeID(Document doc, string STName)
        {
            ElementId result = null;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> collection = collector.OfCategory(BuiltInCategory.OST_DuctSystem).ToElements();

            foreach (Element DSTT in collection)
            {
                if (DSTT.Name == STName)
                    result = DSTT.Id;
            }
            return result;
        }
        public void CreateDuct(Document doc, Connector Conn_Fiit, Connector Conn_Duct, ElementId DuctTypeID, ElementId lvID, double offset, double goc)
        {
            #region 'Tinh goc'
            Duct dt_corner = doc.GetElement(clsBien_MUD.Id_old) as Duct;
            LocationCurve Lc = dt_corner.Location as LocationCurve;
            XYZ DiemDau = Lc.Curve.GetEndPoint(0);
            XYZ DiemCuoi = Lc.Curve.GetEndPoint(1);
            string Cor = "";
            if (DiemDau.X > DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X > DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "GiuNguyen";
            }
            else if (DiemDau.X < DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X < DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "KhongGiuNguyen";
            }
            #endregion
            #region 'Tinh toa do'
            XYZ Diem1 = Conn_Fiit.Origin;
            XYZ Diem2 = Conn_Duct.Origin;
            XYZ u = new XYZ(Diem1.X - Diem2.X, Diem1.Y - Diem2.Y, 0);
            offset = offset / 304.8;
            double kc = (offset / (Math.Tan((goc * (Math.PI)) / 180)));
            XYZ result1 = null, result2 = null;
            string DK = "";
            if (Math.Round(u.X, 7) == 0)
            {
                result1 = new XYZ(Diem1.X, Diem1.Y + kc, Diem2.Z + offset);
                result2 = new XYZ(Diem1.X, Diem1.Y - kc, Diem2.Z + offset);
                DK = "KhongXoay";
            }
            else if (Math.Round(u.Y, 7) == 0)
            {
                result1 = new XYZ(Diem1.X + kc, Diem1.Y, Diem2.Z + offset);
                result2 = new XYZ(Diem1.X - kc, Diem1.Y, Diem2.Z + offset);
                DK = "KhongXoay";
            }
            else
            {
                double ba = (u.Y / u.X) * (u.Y / u.X);
                double m = Math.Sqrt((kc * kc) / (ba + 1));
                double Diemy1 = m + Diem1.Y;
                double Diemy2 = -m + Diem1.Y;
                double Diemx1 = Diem1.X - (u.Y / u.X) * m;
                double Diemx2 = Diem1.X + (u.Y / u.X) * m;
                result1 = new XYZ(Diemx1, Diemy1, Diem2.Z + offset);
                result2 = new XYZ(Diemx2, Diemy2, Diem2.Z + offset);
                DK = "Xoay";
            }
            #endregion
            #region 'Ve ong moi'
            XYZ u1 = new XYZ(Diem1.X - result1.X, Diem1.Y - result1.Y, Diem1.Z - result1.Z);
            XYZ u2 = new XYZ(Diem1.X - result2.X, Diem1.Y - result2.Y, Diem1.Z - result2.Z);
            Duct dtnew = null;
            Duct dtold = doc.GetElement(clsBien_MUD.Id_old) as Duct;
            if ((u.X / u1.X) > 0)
            {
                dtnew = Duct.Create(doc, DuctTypeID, lvID, Conn_Fiit, result1);
                try
                {
                    if (clsBien_MUD.AlphaElbow == "90")
                    {
                        dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Width);
                        dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Height);
                    }
                    else
                    {
                        dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                        dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                    }
                }
                catch
                {
                    dtnew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                }
                RotateOrNot_Duct(DK, u, Cor, Diem1, doc, dtnew, Conn_Duct, result1, Diem2, Conn_Fiit);
                dtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(dtnew.Id.ToString());                                              
            }
            else if ((u.X / u1.X) < 0)
            {
                dtnew = Duct.Create(doc, DuctTypeID, lvID, Conn_Fiit, result2);
                try
                {
                    if (clsBien_MUD.AlphaElbow == "90")
                    {
                        dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Width);
                        dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Height);
                        
                    }
                    else
                    {
                        dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                        dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                    }
                }
                catch
                {
                    dtnew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                }
                RotateOrNot_Duct(DK, u, Cor, Diem1, doc, dtnew, Conn_Duct, result2, Diem2, Conn_Fiit);
                dtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(dtnew.Id.ToString());
            }
            else if (Math.Round(u.X - u1.X, 5) == 0)
            {
                if ((u.Y / u1.Y) > 0)
                {
                    dtnew = Duct.Create(doc, DuctTypeID, lvID, Conn_Fiit, result1);
                    try
                    {
                        if (clsBien_MUD.AlphaElbow == "90")
                        {
                            dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                            dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                        }
                        else
                        {
                            dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Height);
                            dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Width);
                        }
                    }
                    catch
                    {
                        dtnew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                    }
                    RotateOrNot_Duct(DK, u, Cor, Diem1, doc, dtnew, Conn_Duct, result1, Diem2, Conn_Fiit);
                    dtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(dtnew.Id.ToString());                   
                    
                }
                else if ((u.Y / u1.Y) < 0)
                {
                    dtnew = Duct.Create(doc, DuctTypeID, lvID, Conn_Fiit, result2);
                    try
                    {
                        if (clsBien_MUD.AlphaElbow == "90")
                        {
                            dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Width);
                            dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Height);
                        }
                        else
                        {
                            dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                            dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                        }
                    }
                    catch
                    {
                        dtnew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                    }
                    RotateOrNot_Duct(DK, u, Cor, Diem1, doc, dtnew, Conn_Duct, result2, Diem2, Conn_Fiit);
                    dtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(dtnew.Id.ToString());                               
                }
            }
            clsBien_MUD.List_DuctNew.Add(dtnew);
            #endregion
        }
        public void RotateOrNot_Duct(string DK, XYZ u, string Cor, XYZ Diem1, Document doc, Duct dtnew, Connector Conn_Duct, XYZ result1, XYZ Diem2, Connector Conn_Fiit)
        {
            if (DK == "Xoay")
            {
                double Corner = CornerBetweenTwoVector(u, XYZ.BasisX, Cor, clsBien_MUD.AlphaElbow);
                Line axis = Line.CreateBound(new XYZ(Diem1.X, Diem1.Y, 1), new XYZ(Diem1.X, Diem1.Y, 5));
                ElementTransformUtils.RotateElement(doc, dtnew.Id, axis, Corner);
                XYZ res = GetPointDt(dtnew, Conn_Fiit);
                Conn_Duct.Origin = new XYZ(res.X, res.Y, Diem2.Z);
            }
            else
            {
                Conn_Duct.Origin = new XYZ(result1.X, result1.Y, Diem2.Z);
            }
        }
        public void RotateOrNot_Pipe(string DK, XYZ u, string Cor, XYZ Diem1, Document doc, Pipe pinew, Connector Conn_Pipe, XYZ result1, XYZ Diem2, Connector Conn_Fiit)
        {
            if (DK == "Xoay")
            {
                double Corner = CornerBetweenTwoVector(u, XYZ.BasisX, Cor, clsBien_MUD.AlphaElbow);
                Line axis = Line.CreateBound(new XYZ(Diem1.X, Diem1.Y, 1), new XYZ(Diem1.X, Diem1.Y, 5));
                ElementTransformUtils.RotateElement(doc, pinew.Id, axis, Corner);
                XYZ res = GetPointPi(pinew, Conn_Fiit);
                Conn_Pipe.Origin = new XYZ(res.X, res.Y, Diem2.Z);
            }
            else
            {
                Conn_Pipe.Origin = new XYZ(result1.X, result1.Y, Diem2.Z);
            }
        }
        public void RotateOrNot_CableTray(string DK, XYZ u, string Cor, XYZ Diem1, Document doc, CableTray CTnew, Connector Conn_CT, XYZ result1, XYZ Diem2, Connector Conn_Fiit)
        {
            if (DK == "Xoay")
            {
                double Corner = CornerBetweenTwoVector(u, XYZ.BasisX, Cor, clsBien_MUD.AlphaElbow);
                Line axis = Line.CreateBound(new XYZ(Diem1.X, Diem1.Y, 1), new XYZ(Diem1.X, Diem1.Y, 5));
                ElementTransformUtils.RotateElement(doc, CTnew.Id, axis, Corner);
                XYZ res = GetPointCT(CTnew, Conn_Fiit);
                Conn_CT.Origin = new XYZ(res.X, res.Y, Diem2.Z);
            }
            else
            {
                Conn_CT.Origin = new XYZ(result1.X, result1.Y, Diem2.Z);
            }
        }
        public void RotateOrNot_CableTray1(string DK, XYZ Diem1, Document doc, CableTray CTnew)
        {
            if (DK == "Xoay" && clsBien_MUD.AlphaElbow == "90")
            {
                Line axis = Line.CreateBound(new XYZ(Diem1.X, Diem1.Y, 1), new XYZ(Diem1.X, Diem1.Y, 5));
                ElementTransformUtils.RotateElement(doc, CTnew.Id, axis, Math.PI / 2);
            }
        }
        public void RotateOrNot_Conduit(string DK, XYZ u, string Cor, XYZ Diem1, Document doc, Conduit Cnnew, Connector Conn_Conduit, XYZ result1, XYZ Diem2, Connector Conn_Fiit)
        {
            if (DK == "Xoay")
            {
                double Corner = CornerBetweenTwoVector(u, XYZ.BasisX, Cor, clsBien_MUD.AlphaElbow);
                Line axis = Line.CreateBound(new XYZ(Diem1.X, Diem1.Y, 1), new XYZ(Diem1.X, Diem1.Y, 5));
                ElementTransformUtils.RotateElement(doc, Cnnew.Id, axis, Corner);
                XYZ res = GetPointCN(Cnnew, Conn_Fiit);
                Conn_Conduit.Origin = new XYZ(res.X, res.Y, Diem2.Z);
            }
            else
            {
                Conn_Conduit.Origin = new XYZ(result1.X, result1.Y, Diem2.Z);
            }
        }
        public void CreateDuct_New(Document doc, Duct DuctOld1, Duct DuctOld, ElementId DuctTypeID, ElementId lvID, double goc)
        {
            #region 'Tinh goc'
            Duct dt_corner = doc.GetElement(clsBien_MUD.Id_old) as Duct;
            LocationCurve Lc = dt_corner.Location as LocationCurve;
            XYZ DiemDau = Lc.Curve.GetEndPoint(0);
            XYZ DiemCuoi = Lc.Curve.GetEndPoint(1);
            string Cor = "";
            if (DiemDau.X > DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X > DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "GiuNguyen";
            }
            else if (DiemDau.X < DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X < DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "KhongGiuNguyen";
            }
            #endregion
            #region 'Khai bao'
            ConnectorSet conn_set1 = DuctOld1.ConnectorManager.Connectors;
            ConnectorSet conn_set_old = DuctOld.ConnectorManager.Connectors;

            Connector Conn_DuctOld1 = null;
            Connector Conn_DuctOld = null;

            List<Connector> list_dt1_dtold = GetConnectorNear(conn_set1, conn_set_old);
            Conn_DuctOld1 = list_dt1_dtold[0];
            Conn_DuctOld = list_dt1_dtold[1];
            XYZ Diem1 = Conn_DuctOld1.Origin;
            XYZ Diem2 = Conn_DuctOld.Origin;
            #endregion
            if (Math.Round(Diem1.Z, 5) == Math.Round(Diem2.Z, 5))
            {

            }
            else
            {
                string DK = "";
                #region 'Tinh toa do'
                double offset = Diem2.Z - Diem1.Z;
                XYZ u = new XYZ(Diem1.X - Diem2.X, Diem1.Y - Diem2.Y, 0);
                //offset = offset / 304.8;
                double kc = (offset / (Math.Tan((goc * (Math.PI)) / 180)));
                XYZ result1 = null, result2 = null;
                if (Math.Round(u.X, 7) == 0)
                {
                    result1 = new XYZ(Diem1.X, Diem1.Y + kc, Diem2.Z);
                    result2 = new XYZ(Diem1.X, Diem1.Y - kc, Diem2.Z);
                    DK = "KhongXoay";
                }
                else if (Math.Round(u.Y, 7) == 0)
                {
                    result1 = new XYZ(Diem1.X + kc, Diem1.Y, Diem2.Z);
                    result2 = new XYZ(Diem1.X - kc, Diem1.Y, Diem2.Z);
                    DK = "KhongXoay";
                }
                else
                {
                    double bien1 = (u.Y / u.X) * (u.Y / u.X);
                    double m = Math.Sqrt((kc * kc) / (bien1 + 1));
                    double Diemy1 = m + Diem1.Y;
                    double Diemy2 = -m + Diem1.Y;
                    double Diemx1 = Diem1.X - (u.Y / u.X) * m;
                    double Diemx2 = Diem1.X + (u.Y / u.X) * m;
                    result1 = new XYZ(Diemx1, Diemy1, Diem2.Z);
                    result2 = new XYZ(Diemx2, Diemy2, Diem2.Z);
                    DK = "Xoay";
                }
                XYZ u1 = new XYZ(Diem1.X - result1.X, Diem1.Y - result1.Y, Diem1.Z - result1.Z);
                XYZ u2 = new XYZ(Diem1.X - result2.X, Diem1.Y - result2.Y, Diem1.Z - result2.Z);
                Duct dtnew = null;
                #endregion
                #region 'Ve ong moi'
                if ((u.X / u1.X) > 0)
                {
                    dtnew = Duct.Create(doc, DuctTypeID, lvID, Conn_DuctOld1, result1);
                    try
                    {
                        if (clsBien_MUD.AlphaElbow == "90")
                        {
                            dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Width);
                            dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Height);
                        }
                        else
                        {
                            dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                            dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                        }
                    }
                    catch
                    {
                        dtnew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                    }
                    RotateOrNot_Duct(DK, u, Cor, Diem1, doc, dtnew, Conn_DuctOld, result1, Diem2, Conn_DuctOld1);
                    dtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(dtnew.Id.ToString());                                    
                }
                else if ((u.X / u1.X) < 0)
                {
                    dtnew = Duct.Create(doc, DuctTypeID, lvID, Conn_DuctOld1, result2);
                    try
                    {
                        if (clsBien_MUD.AlphaElbow == "90")
                        {
                            dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Width);
                            dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Height);
                        }
                        else
                        {
                            dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                            dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                        }
                    }
                    catch
                    {
                        dtnew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                    }
                    RotateOrNot_Duct(DK, u, Cor, Diem1, doc, dtnew, Conn_DuctOld, result2, Diem2, Conn_DuctOld1);
                    dtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(dtnew.Id.ToString());                                      
                }
                else if (Math.Round(u.X - u1.X, 5) == 0)
                {
                    if ((u.Y / u1.Y) > 0)
                    {
                        dtnew = Duct.Create(doc, DuctTypeID, lvID, Conn_DuctOld1, result1);
                        try
                        {
                            if (clsBien_MUD.AlphaElbow == "90")
                            {
                                dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Width);
                                dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Height);
                            }
                            else
                            {
                                dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                                dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                            }
                        }
                        catch
                        {
                            dtnew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                        }
                        dtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(dtnew.Id.ToString());
                        RotateOrNot_Duct(DK, u, Cor, Diem1, doc, dtnew, Conn_DuctOld, result1, Diem2, Conn_DuctOld1);
                    }
                    else if ((u.Y / u1.Y) < 0)
                    {
                        dtnew = Duct.Create(doc, DuctTypeID, lvID, Conn_DuctOld1, result2);
                        try
                        {
                            if (clsBien_MUD.AlphaElbow == "90")
                            {
                                dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Width);
                                dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Height);
                            }
                            else
                            {
                                dtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                                dtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                            }
                        }
                        catch
                        {
                            dtnew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                        }
                        dtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(dtnew.Id.ToString());
                        RotateOrNot_Duct(DK, u, Cor, Diem1, doc, dtnew, Conn_DuctOld, result2, Diem2, Conn_DuctOld1);
                    }
                }
                clsBien_MUD.List_DuctNew.Add(dtnew);
                #endregion
            }
        }
        public void CreatePipe(Document doc, Connector Conn_Fiit, Connector Conn_Pipe, ElementId PipeTypeID, ElementId lvID, double offset, double goc)
        {
            #region 'Tinh goc'
            Pipe pi_corner = doc.GetElement(clsBien_MUD.Id_old) as Pipe;
            LocationCurve Lc = pi_corner.Location as LocationCurve;
            XYZ DiemDau = Lc.Curve.GetEndPoint(0);
            XYZ DiemCuoi = Lc.Curve.GetEndPoint(1);
            string Cor = "";
            if (DiemDau.X > DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X > DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "GiuNguyen";
            }
            else if (DiemDau.X < DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X < DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "KhongGiuNguyen";
            }
            #endregion
            XYZ Diem1 = Conn_Fiit.Origin;
            XYZ Diem2 = Conn_Pipe.Origin;
            XYZ u = new XYZ(Diem1.X - Diem2.X, Diem1.Y - Diem2.Y, 0);
            offset = offset / 304.8;
            double kc = (offset / (Math.Tan((goc * (Math.PI)) / 180)));
            XYZ result1 = null, result2 = null;
            string DK = "";
            if (Math.Round(u.X, 7) == 0)
            {
                result1 = new XYZ(Diem1.X, Diem1.Y + kc, Diem2.Z + offset);
                result2 = new XYZ(Diem1.X, Diem1.Y - kc, Diem2.Z + offset);
                DK = "KhongXoay";
            }
            else if (Math.Round(u.Y, 7) == 0)
            {
                result1 = new XYZ(Diem1.X + kc, Diem1.Y, Diem2.Z + offset);
                result2 = new XYZ(Diem1.X - kc, Diem1.Y, Diem2.Z + offset);
                DK = "KhongXoay";
            }
            else
            {
                double bien1 = (u.Y / u.X) * (u.Y / u.X);
                double m = Math.Sqrt((kc * kc) / (bien1 + 1));
                double Diemy1 = m + Diem1.Y;
                double Diemy2 = -m + Diem1.Y;
                double Diemx1 = Diem1.X - (u.Y / u.X) * m;
                double Diemx2 = Diem1.X + (u.Y / u.X) * m;
                result1 = new XYZ(Diemx1, Diemy1, Diem2.Z + offset);
                result2 = new XYZ(Diemx2, Diemy2, Diem2.Z + offset);
                DK = "Xoay";
            }
            XYZ u1 = new XYZ(Diem1.X - result1.X, Diem1.Y - result1.Y, Diem1.Z - result1.Z);
            XYZ u2 = new XYZ(Diem1.X - result2.X, Diem1.Y - result2.Y, Diem1.Z - result2.Z);
            Pipe pinew = null;
            if ((u.X / u1.X) > 0)
            {
                pinew = Pipe.Create(doc, PipeTypeID, lvID, Conn_Fiit, result1);
                pinew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                RotateOrNot_Pipe(DK, u, Cor, Diem1, doc, pinew, Conn_Pipe, result1, Diem2, Conn_Fiit);
                pinew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(pinew.Id.ToString());
            }
            else if ((u.X / u1.X) < 0)
            {
                pinew = Pipe.Create(doc, PipeTypeID, lvID, Conn_Fiit, result2);
                pinew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                RotateOrNot_Pipe(DK, u, Cor, Diem1, doc, pinew, Conn_Pipe, result2, Diem2, Conn_Fiit);
                pinew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(pinew.Id.ToString());                
            }
            else if (Math.Round(u.X - u1.X, 5) == 0)
            {
                if ((u.Y / u1.Y) > 0)
                {
                    pinew = Pipe.Create(doc, PipeTypeID, lvID, Conn_Fiit, result1);
                    pinew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                    RotateOrNot_Pipe(DK, u, Cor, Diem1, doc, pinew, Conn_Pipe, result1, Diem2, Conn_Fiit);
                    pinew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(pinew.Id.ToString());                   
                }
                else if ((u.Y / u1.Y) < 0)
                {
                    pinew = Pipe.Create(doc, PipeTypeID, lvID, Conn_Fiit, result2);
                    pinew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                    RotateOrNot_Pipe(DK, u, Cor, Diem1, doc, pinew, Conn_Pipe, result2, Diem2, Conn_Fiit);
                    pinew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(pinew.Id.ToString());                    
                }
            }
            clsBien_MUD.List_PipeNew.Add(pinew);

        }
        public void CreatePipe_New(Document doc, Pipe PipeOld1, Pipe PipeOld, ElementId PipeTypeID, ElementId lvID, double goc)
        {
            #region 'Tinh goc'
            Pipe pi_corner = doc.GetElement(clsBien_MUD.Id_old) as Pipe;
            LocationCurve Lc = pi_corner.Location as LocationCurve;
            XYZ DiemDau = Lc.Curve.GetEndPoint(0);
            XYZ DiemCuoi = Lc.Curve.GetEndPoint(1);
            string Cor = "";
            if (DiemDau.X > DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X > DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "GiuNguyen";
            }
            else if (DiemDau.X < DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X < DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "KhongGiuNguyen";
            }
            #endregion
            ConnectorSet conn_set1 = PipeOld1.ConnectorManager.Connectors;
            ConnectorSet conn_set_old = PipeOld.ConnectorManager.Connectors;

            Connector Conn_PipeOld1 = null;
            Connector Conn_PipeOld = null;

            List<Connector> list_pi1_piold = GetConnectorNear(conn_set1, conn_set_old);
            Conn_PipeOld1 = list_pi1_piold[0];
            Conn_PipeOld = list_pi1_piold[1];

            XYZ Diem1 = Conn_PipeOld1.Origin;
            XYZ Diem2 = Conn_PipeOld.Origin;


            if (Math.Round(Diem1.Z, 5) == Math.Round(Diem2.Z, 5))
            {

            }
            else
            {
                double offset = Diem2.Z - Diem1.Z;
                XYZ u = new XYZ(Diem1.X - Diem2.X, Diem1.Y - Diem2.Y, 0);
                //offset = offset / 304.8;
                double kc = (offset / (Math.Tan((goc * (Math.PI)) / 180)));
                XYZ result1 = null, result2 = null;
                string DK = "";
                if (Math.Round(u.X, 7) == 0)
                {
                    result1 = new XYZ(Diem1.X, Diem1.Y + kc, Diem2.Z);
                    result2 = new XYZ(Diem1.X, Diem1.Y - kc, Diem2.Z);
                    DK = "KhongXoay";
                }
                else if (Math.Round(u.Y, 7) == 0)
                {
                    result1 = new XYZ(Diem1.X + kc, Diem1.Y, Diem2.Z);
                    result2 = new XYZ(Diem1.X - kc, Diem1.Y, Diem2.Z);
                    DK = "KhongXoay";
                }
                else
                {
                    double bien1 = (u.Y / u.X) * (u.Y / u.X);
                    double m = Math.Sqrt((kc * kc) / (bien1 + 1));
                    double Diemy1 = m + Diem1.Y;
                    double Diemy2 = -m + Diem1.Y;
                    double Diemx1 = Diem1.X - (u.Y / u.X) * m;
                    double Diemx2 = Diem1.X + (u.Y / u.X) * m;
                    result1 = new XYZ(Diemx1, Diemy1, Diem2.Z);
                    result2 = new XYZ(Diemx2, Diemy2, Diem2.Z);
                    DK = "Xoay";
                }
                XYZ u1 = new XYZ(Diem1.X - result1.X, Diem1.Y - result1.Y, Diem1.Z - result1.Z);
                XYZ u2 = new XYZ(Diem1.X - result2.X, Diem1.Y - result2.Y, Diem1.Z - result2.Z);
                Pipe pinew = null;

                if ((u.X / u1.X) > 0)
                {
                    pinew = Pipe.Create(doc, PipeTypeID, lvID, Conn_PipeOld1, result1);
                    pinew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                    RotateOrNot_Pipe(DK, u, Cor, Diem1, doc, pinew, Conn_PipeOld, result1, Diem2, Conn_PipeOld1);
                    pinew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(pinew.Id.ToString());                   
                }
                else if ((u.X / u1.X) < 0)
                {
                    pinew = Pipe.Create(doc, PipeTypeID, lvID, Conn_PipeOld1, result2);
                    pinew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                    RotateOrNot_Pipe(DK, u, Cor, Diem1, doc, pinew, Conn_PipeOld, result2, Diem2, Conn_PipeOld1);
                    pinew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(pinew.Id.ToString());                   
                }
                else if (Math.Round(u.X - u1.X, 5) == 0)
                {
                    if ((u.Y / u1.Y) > 0)
                    {
                        pinew = Pipe.Create(doc, PipeTypeID, lvID, Conn_PipeOld1, result1);
                        pinew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                        RotateOrNot_Pipe(DK, u, Cor, Diem1, doc, pinew, Conn_PipeOld, result1, Diem2, Conn_PipeOld1);
                        pinew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(pinew.Id.ToString());                       
                    }
                    else if ((u.Y / u1.Y) < 0)
                    {
                        pinew = Pipe.Create(doc, PipeTypeID, lvID, Conn_PipeOld1, result2);
                        pinew.LookupParameter("Diameter").SetValueString(clsBien_MUD.Diameter);
                        RotateOrNot_Pipe(DK, u, Cor, Diem1, doc, pinew, Conn_PipeOld, result2, Diem2, Conn_PipeOld1);
                        pinew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(pinew.Id.ToString());                       
                    }
                }
                clsBien_MUD.List_PipeNew.Add(pinew);
            }
        }
        public void CreateCbTray(Document doc, Connector Conn_Fiit, Connector Conn_CbTray, ElementId CableTrayTypeID, ElementId lvID, double offset, double goc)
        {
            #region 'Tinh goc'
            CableTray Ct_corner = doc.GetElement(clsBien_MUD.Id_old) as CableTray;
            LocationCurve Lc = Ct_corner.Location as LocationCurve;
            XYZ DiemDau = Lc.Curve.GetEndPoint(0);
            XYZ DiemCuoi = Lc.Curve.GetEndPoint(1);
            string Cor = "";
            if (DiemDau.X > DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X > DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "GiuNguyen";
            }
            else if (DiemDau.X < DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X < DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "KhongGiuNguyen";
            }
            #endregion

            XYZ Diem1 = Conn_Fiit.Origin;
            XYZ Diem2 = Conn_CbTray.Origin;
            XYZ u = new XYZ(Diem1.X - Diem2.X, Diem1.Y - Diem2.Y, 0);
            offset = offset / 304.8;
            double kc = (offset / (Math.Tan((goc * (Math.PI)) / 180)));
            XYZ result1 = null, result2 = null;
            string DK = "";
            string DK1 = "";
            if (Math.Round(u.X, 7) == 0)
            {
                result1 = new XYZ(Diem1.X, Diem1.Y + kc, Diem2.Z + offset);
                result2 = new XYZ(Diem1.X, Diem1.Y - kc, Diem2.Z + offset);
                DK = "KhongXoay";
                DK1 = "KhongXoay";
            }
            else if (Math.Round(u.Y, 7) == 0)
            {
                result1 = new XYZ(Diem1.X + kc, Diem1.Y, Diem2.Z + offset);
                result2 = new XYZ(Diem1.X - kc, Diem1.Y, Diem2.Z + offset);
                DK = "KhongXoay";
                DK1 = "Xoay";
            }
            else
            {
                double bien1 = (u.Y / u.X) * (u.Y / u.X);
                double m = Math.Sqrt((kc * kc) / (bien1 + 1));
                double Diemy1 = m + Diem1.Y;
                double Diemy2 = -m + Diem1.Y;
                double Diemx1 = Diem1.X - (u.Y / u.X) * m;
                double Diemx2 = Diem1.X + (u.Y / u.X) * m;
                result1 = new XYZ(Diemx1, Diemy1, Diem2.Z + offset);
                result2 = new XYZ(Diemx2, Diemy2, Diem2.Z + offset);
                DK = "Xoay";
                DK1 = "Xoay";
            }
            XYZ u1 = new XYZ(Diem1.X - result1.X, Diem1.Y - result1.Y, Diem1.Z - result1.Z);
            XYZ u2 = new XYZ(Diem1.X - result2.X, Diem1.Y - result2.Y, Diem1.Z - result2.Z);
            CableTray Cbtnew = null;
            if ((u.X / u1.X) > 0)
            {
                Cbtnew = CableTray.Create(doc, CableTrayTypeID, Conn_Fiit.Origin, result1, lvID);
                Cbtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                Cbtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                Cbtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Cbtnew.Id.ToString());
                RotateOrNot_CableTray(DK, u, Cor, Diem1, doc, Cbtnew, Conn_CbTray, result1, Diem2, Conn_Fiit);
                RotateOrNot_CableTray1(DK1, Diem1, doc, Cbtnew);
            }
            else if ((u.X / u1.X) < 0)
            {
                Cbtnew = CableTray.Create(doc, CableTrayTypeID, Conn_Fiit.Origin, result2, lvID);
                Cbtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                Cbtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                Cbtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Cbtnew.Id.ToString());
                RotateOrNot_CableTray(DK, u, Cor, Diem1, doc, Cbtnew, Conn_CbTray, result2, Diem2, Conn_Fiit);
                RotateOrNot_CableTray1(DK1, Diem1, doc, Cbtnew);
            }
            else if (Math.Round(u.X - u1.X, 5) == 0)
            {
                if ((u.Y / u1.Y) > 0)
                {
                    Cbtnew = CableTray.Create(doc, CableTrayTypeID, Conn_Fiit.Origin, result1, lvID);
                    Cbtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                    Cbtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                    Cbtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Cbtnew.Id.ToString());
                    RotateOrNot_CableTray(DK, u, Cor, Diem1, doc, Cbtnew, Conn_CbTray, result1, Diem2, Conn_Fiit);
                    RotateOrNot_CableTray1(DK1, Diem1, doc, Cbtnew);
                }
                else if ((u.Y / u1.Y) < 0)
                {
                    Cbtnew = CableTray.Create(doc, CableTrayTypeID, Conn_Fiit.Origin, result2, lvID);
                    Cbtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                    Cbtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                    Cbtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Cbtnew.Id.ToString());
                    RotateOrNot_CableTray(DK, u, Cor, Diem1, doc, Cbtnew, Conn_CbTray, result2, Diem2, Conn_Fiit);
                    RotateOrNot_CableTray1(DK1, Diem1, doc, Cbtnew);
                }
            }
            clsBien_MUD.List_CbTrayNew.Add(Cbtnew);

        }
        public void CreateCbTray_New(Document doc, CableTray CbtOld1, CableTray CbtOld, ElementId CableTrayTypeID, ElementId lvID, double goc)
        {
            #region 'Tinh goc'
            CableTray Ct_corner = doc.GetElement(clsBien_MUD.Id_old) as CableTray;
            LocationCurve Lc = Ct_corner.Location as LocationCurve;
            XYZ DiemDau = Lc.Curve.GetEndPoint(0);
            XYZ DiemCuoi = Lc.Curve.GetEndPoint(1);
            string Cor = "";
            if (DiemDau.X > DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X > DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "GiuNguyen";
            }
            else if (DiemDau.X < DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X < DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "KhongGiuNguyen";
            }
            #endregion

            ConnectorSet conn_set1 = CbtOld1.ConnectorManager.Connectors;
            ConnectorSet conn_set_old = CbtOld.ConnectorManager.Connectors;

            Connector Conn_CbtOld1 = null;
            Connector Conn_CbtOld = null;

            List<Connector> list_Cbt1_Cbtold = GetConnectorNear(conn_set1, conn_set_old);
            Conn_CbtOld1 = list_Cbt1_Cbtold[0];
            Conn_CbtOld = list_Cbt1_Cbtold[1];

            XYZ Diem1 = Conn_CbtOld1.Origin;
            XYZ Diem2 = Conn_CbtOld.Origin;


            if (Math.Round(Diem1.Z, 5) == Math.Round(Diem2.Z, 5))
            {

            }
            else
            {
                double offset = Diem2.Z - Diem1.Z;
                XYZ u = new XYZ(Diem1.X - Diem2.X, Diem1.Y - Diem2.Y, 0);
                //offset = offset / 304.8;
                double kc = (offset / (Math.Tan((goc * (Math.PI)) / 180)));
                XYZ result1 = null, result2 = null;
                string DK = "";
                string DK1 = "";
                if (Math.Round(u.X, 7) == 0)
                {
                    result1 = new XYZ(Diem1.X, Diem1.Y + kc, Diem2.Z);
                    result2 = new XYZ(Diem1.X, Diem1.Y - kc, Diem2.Z);
                    DK = "KhongXoay";
                    DK1 = "KhongXoay";
                }
                else if (Math.Round(u.Y, 7) == 0)
                {
                    result1 = new XYZ(Diem1.X + kc, Diem1.Y, Diem2.Z);
                    result2 = new XYZ(Diem1.X - kc, Diem1.Y, Diem2.Z);
                    DK = "KhongXoay";
                    DK1 = "Xoay";
                }
                else
                {
                    double bien1 = (u.Y / u.X) * (u.Y / u.X);
                    double m = Math.Sqrt((kc * kc) / (bien1 + 1));
                    double Diemy1 = m + Diem1.Y;
                    double Diemy2 = -m + Diem1.Y;
                    double Diemx1 = Diem1.X - (u.Y / u.X) * m;
                    double Diemx2 = Diem1.X + (u.Y / u.X) * m;
                    result1 = new XYZ(Diemx1, Diemy1, Diem2.Z);
                    result2 = new XYZ(Diemx2, Diemy2, Diem2.Z);
                    DK = "Xoay";
                    DK1 = "Xoay";
                }
                XYZ u1 = new XYZ(Diem1.X - result1.X, Diem1.Y - result1.Y, Diem1.Z - result1.Z);
                XYZ u2 = new XYZ(Diem1.X - result2.X, Diem1.Y - result2.Y, Diem1.Z - result2.Z);
                CableTray Cbtnew = null;

                if ((u.X / u1.X) > 0)
                {
                    Cbtnew = CableTray.Create(doc, CableTrayTypeID, Conn_CbtOld1.Origin, result1, lvID);
                    Cbtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                    Cbtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                    Cbtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Cbtnew.Id.ToString());
                    RotateOrNot_CableTray(DK, u, Cor, Diem1, doc, Cbtnew, Conn_CbtOld, result1, Diem2, Conn_CbtOld1);
                    RotateOrNot_CableTray1(DK1, Diem1, doc, Cbtnew);
                }
                else if ((u.X / u1.X) < 0)
                {
                    Cbtnew = CableTray.Create(doc, CableTrayTypeID, Conn_CbtOld1.Origin, result2, lvID);
                    Cbtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                    Cbtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                    Cbtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Cbtnew.Id.ToString());
                    RotateOrNot_CableTray(DK, u, Cor, Diem1, doc, Cbtnew, Conn_CbtOld, result2, Diem2, Conn_CbtOld1);
                    RotateOrNot_CableTray1(DK1, Diem1, doc, Cbtnew);
                }
                else if (Math.Round(u.X - u1.X, 5) == 0)
                {
                    if ((u.Y / u1.Y) > 0)
                    {
                        Cbtnew = CableTray.Create(doc, CableTrayTypeID, Conn_CbtOld1.Origin, result1, lvID);
                        Cbtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                        Cbtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                        Cbtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Cbtnew.Id.ToString());
                        RotateOrNot_CableTray(DK, u, Cor, Diem1, doc, Cbtnew, Conn_CbtOld, result1, Diem2, Conn_CbtOld1);
                        RotateOrNot_CableTray1(DK1, Diem1, doc, Cbtnew);
                    }
                    else if ((u.Y / u1.Y) < 0)
                    {
                        Cbtnew = CableTray.Create(doc, CableTrayTypeID, Conn_CbtOld1.Origin, result2, lvID);
                        Cbtnew.LookupParameter("Width").SetValueString(clsBien_MUD.Width);
                        Cbtnew.LookupParameter("Height").SetValueString(clsBien_MUD.Height);
                        Cbtnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Cbtnew.Id.ToString());
                        RotateOrNot_CableTray(DK, u, Cor, Diem1, doc, Cbtnew, Conn_CbtOld, result2, Diem2, Conn_CbtOld1);
                        RotateOrNot_CableTray1(DK1, Diem1, doc, Cbtnew);
                    }
                }
                clsBien_MUD.List_CbTrayNew.Add(Cbtnew);
            }
        }
        public void CreateConduit(Document doc, Connector Conn_Fiit, Connector Conn_Cd, ElementId ConduitTypeID, ElementId lvID, double offset, double goc)
        {
            #region 'Tinh goc'
            Conduit Cn_corner = doc.GetElement(clsBien_MUD.Id_old) as Conduit;
            LocationCurve Lc = Cn_corner.Location as LocationCurve;
            XYZ DiemDau = Lc.Curve.GetEndPoint(0);
            XYZ DiemCuoi = Lc.Curve.GetEndPoint(1);
            string Cor = "";
            if (DiemDau.X > DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X > DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "GiuNguyen";
            }
            else if (DiemDau.X < DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X < DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "KhongGiuNguyen";
            }
            #endregion

            XYZ Diem1 = Conn_Fiit.Origin;
            XYZ Diem2 = Conn_Cd.Origin;
            XYZ u = new XYZ(Diem1.X - Diem2.X, Diem1.Y - Diem2.Y, 0);
            offset = offset / 304.8;
            double kc = (offset / (Math.Tan((goc * (Math.PI)) / 180)));
            XYZ result1 = null, result2 = null;
            string DK = "";
            if (Math.Round(u.X, 7) == 0)
            {
                result1 = new XYZ(Diem1.X, Diem1.Y + kc, Diem2.Z + offset);
                result2 = new XYZ(Diem1.X, Diem1.Y - kc, Diem2.Z + offset);
                DK = "KhongXoay";
            }
            else if (Math.Round(u.Y, 7) == 0)
            {
                result1 = new XYZ(Diem1.X + kc, Diem1.Y, Diem2.Z + offset);
                result2 = new XYZ(Diem1.X - kc, Diem1.Y, Diem2.Z + offset);
                DK = "KhongXoay";
            }
            else
            {
                double bien1 = (u.Y / u.X) * (u.Y / u.X);
                double m = Math.Sqrt((kc * kc) / (bien1 + 1));
                double Diemy1 = m + Diem1.Y;
                double Diemy2 = -m + Diem1.Y;
                double Diemx1 = Diem1.X - (u.Y / u.X) * m;
                double Diemx2 = Diem1.X + (u.Y / u.X) * m;
                result1 = new XYZ(Diemx1, Diemy1, Diem2.Z + offset);
                result2 = new XYZ(Diemx2, Diemy2, Diem2.Z + offset);
                DK = "Xoay";
            }
            XYZ u1 = new XYZ(Diem1.X - result1.X, Diem1.Y - result1.Y, Diem1.Z - result1.Z);
            XYZ u2 = new XYZ(Diem1.X - result2.X, Diem1.Y - result2.Y, Diem1.Z - result2.Z);
            Conduit CdNew = null;
            if ((u.X / u1.X) > 0)
            {
                CdNew = Conduit.Create(doc, ConduitTypeID, Conn_Fiit.Origin, result1, lvID);
                CdNew.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).SetValueString(clsBien_MUD.Diameter);
                CdNew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(CdNew.Id.ToString());
                RotateOrNot_Conduit(DK, u, Cor, Diem1, doc, CdNew, Conn_Cd, result1, Diem2, Conn_Fiit);
            }
            else if ((u.X / u1.X) < 0)
            {
                CdNew = Conduit.Create(doc, ConduitTypeID, Conn_Fiit.Origin, result2, lvID);
                CdNew.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).SetValueString(clsBien_MUD.Diameter);
                CdNew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(CdNew.Id.ToString());
                RotateOrNot_Conduit(DK, u, Cor, Diem1, doc, CdNew, Conn_Cd, result2, Diem2, Conn_Fiit);
            }
            else if (Math.Round(u.X - u1.X, 5) == 0)
            {
                if ((u.Y / u1.Y) > 0)
                {
                    CdNew = Conduit.Create(doc, ConduitTypeID, Conn_Fiit.Origin, result1, lvID);
                    CdNew.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).SetValueString(clsBien_MUD.Diameter);
                    CdNew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(CdNew.Id.ToString());
                    RotateOrNot_Conduit(DK, u, Cor, Diem1, doc, CdNew, Conn_Cd, result1, Diem2, Conn_Fiit);
                }
                else if ((u.Y / u1.Y) < 0)
                {
                    CdNew = Conduit.Create(doc, ConduitTypeID, Conn_Fiit.Origin, result2, lvID);
                    CdNew.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).SetValueString(clsBien_MUD.Diameter);
                    CdNew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(CdNew.Id.ToString());
                    RotateOrNot_Conduit(DK, u, Cor, Diem1, doc, CdNew, Conn_Cd, result2, Diem2, Conn_Fiit);
                }
            }
            clsBien_MUD.List_CondiutNew.Add(CdNew);

        }
        public void CreateConduit_New(Document doc, Conduit CdOld1, Conduit CdOld, ElementId CableTrayTypeID, ElementId lvID, double goc)
        {

            #region 'Tinh goc'
            Conduit Cn_corner = doc.GetElement(clsBien_MUD.Id_old) as Conduit;
            LocationCurve Lc = Cn_corner.Location as LocationCurve;
            XYZ DiemDau = Lc.Curve.GetEndPoint(0);
            XYZ DiemCuoi = Lc.Curve.GetEndPoint(1);
            string Cor = "";
            if (DiemDau.X > DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X > DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "GiuNguyen";
            }
            else if (DiemDau.X < DiemCuoi.X && DiemDau.Y > DiemCuoi.Y || DiemCuoi.X < DiemDau.X && DiemCuoi.Y > DiemDau.Y)
            {
                Cor = "KhongGiuNguyen";
            }
            #endregion

            ConnectorSet conn_set1 = CdOld1.ConnectorManager.Connectors;
            ConnectorSet conn_set_old = CdOld.ConnectorManager.Connectors;

            Connector Conn_CdOld1 = null;
            Connector Conn_CdOld = null;

            List<Connector> list_Cd1_Cdold = GetConnectorNear(conn_set1, conn_set_old);
            Conn_CdOld1 = list_Cd1_Cdold[0];
            Conn_CdOld = list_Cd1_Cdold[1];

            XYZ Diem1 = Conn_CdOld1.Origin;
            XYZ Diem2 = Conn_CdOld.Origin;


            if (Math.Round(Diem1.Z, 5) == Math.Round(Diem2.Z, 5))
            {

            }
            else
            {
                double offset = Diem2.Z - Diem1.Z;
                XYZ u = new XYZ(Diem1.X - Diem2.X, Diem1.Y - Diem2.Y, 0);
                //offset = offset / 304.8;
                double kc = (offset / (Math.Tan((goc * (Math.PI)) / 180)));
                XYZ result1 = null, result2 = null;
                string DK = "";
                if (Math.Round(u.X, 7) == 0)
                {
                    result1 = new XYZ(Diem1.X, Diem1.Y + kc, Diem2.Z);
                    result2 = new XYZ(Diem1.X, Diem1.Y - kc, Diem2.Z);
                    DK = "KhongXoay";
                }
                else if (Math.Round(u.Y, 7) == 0)
                {
                    result1 = new XYZ(Diem1.X + kc, Diem1.Y, Diem2.Z);
                    result2 = new XYZ(Diem1.X - kc, Diem1.Y, Diem2.Z);
                    DK = "KhongXoay";
                }
                else
                {
                    double bien1 = (u.Y / u.X) * (u.Y / u.X);
                    double m = Math.Sqrt((kc * kc) / (bien1 + 1));
                    double Diemy1 = m + Diem1.Y;
                    double Diemy2 = -m + Diem1.Y;
                    double Diemx1 = Diem1.X - (u.Y / u.X) * m;
                    double Diemx2 = Diem1.X + (u.Y / u.X) * m;
                    result1 = new XYZ(Diemx1, Diemy1, Diem2.Z);
                    result2 = new XYZ(Diemx2, Diemy2, Diem2.Z);
                    DK = "Xoay";
                }
                XYZ u1 = new XYZ(Diem1.X - result1.X, Diem1.Y - result1.Y, Diem1.Z - result1.Z);
                XYZ u2 = new XYZ(Diem1.X - result2.X, Diem1.Y - result2.Y, Diem1.Z - result2.Z);
                Conduit Cdnew = null;

                if ((u.X / u1.X) > 0)
                {
                    Cdnew = Conduit.Create(doc, CableTrayTypeID, Conn_CdOld1.Origin, result1, lvID);
                    Cdnew.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).SetValueString(clsBien_MUD.Diameter);
                    Cdnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Cdnew.Id.ToString());
                    RotateOrNot_Conduit(DK, u, Cor, Diem1, doc, Cdnew, Conn_CdOld, result1, Diem2, Conn_CdOld1);
                }
                else if ((u.X / u1.X) < 0)
                {
                    Cdnew = Conduit.Create(doc, CableTrayTypeID, Conn_CdOld1.Origin, result2, lvID);
                    Cdnew.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).SetValueString(clsBien_MUD.Diameter);
                    Cdnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Cdnew.Id.ToString());
                    RotateOrNot_Conduit(DK, u, Cor, Diem1, doc, Cdnew, Conn_CdOld, result2, Diem2, Conn_CdOld1);
                }
                else if (Math.Round(u.X - u1.X, 5) == 0)
                {
                    if ((u.Y / u1.Y) > 0)
                    {
                        Cdnew = Conduit.Create(doc, CableTrayTypeID, Conn_CdOld1.Origin, result1, lvID);
                        Cdnew.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).SetValueString(clsBien_MUD.Diameter);
                        Cdnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Cdnew.Id.ToString());
                        RotateOrNot_Conduit(DK, u, Cor, Diem1, doc, Cdnew, Conn_CdOld, result1, Diem2, Conn_CdOld1);
                    }
                    else if ((u.Y / u1.Y) < 0)
                    {
                        Cdnew = Conduit.Create(doc, CableTrayTypeID, Conn_CdOld1.Origin, result2, lvID);
                        Cdnew.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).SetValueString(clsBien_MUD.Diameter);
                        Cdnew.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Cdnew.Id.ToString());
                        RotateOrNot_Conduit(DK, u, Cor, Diem1, doc, Cdnew, Conn_CdOld, result2, Diem2, Conn_CdOld1);
                    }
                }
                clsBien_MUD.List_CondiutNew.Add(Cdnew);
            }
        }
        public ElementId GeLevelElement(Document doc, ElementId ids)
        {
            Element ele = doc.GetElement(ids);
            ElementId result = null;
            if (ele is Duct)
            {
                Duct dt = ele as Duct;
                result = dt.ReferenceLevel.Id;
            }
            else if (ele is Pipe)
            {
                Pipe pi = ele as Pipe;
                result = pi.ReferenceLevel.Id;
            }
            else if (ele is CableTray)
            {
                CableTray CbTr = ele as CableTray;
                result = CbTr.ReferenceLevel.Id;
            }
            else if (ele is Conduit)
            {
                Conduit Cd = ele as Conduit;
                result = Cd.ReferenceLevel.Id;
            }
            return result;
        }
        public void CreateDuctFitting(Document doc, List<Duct> lst_ele1)
        {
            if (lst_ele1.Count == 1)
            {
                Duct dt1 = lst_ele1[0];
                Duct dtold = doc.GetElement(clsBien_MUD.Id_old) as Duct;

                ConnectorSet conn_set1 = dt1.ConnectorManager.Connectors;
                ConnectorSet conn_set_old = dtold.ConnectorManager.Connectors;
                ConnectorSet conn_set_neighbor = clsBien_MUD.List_DuctNeightbor[0].ConnectorManager.Connectors;

                List<Connector> list_dt1_dtold = GetConnectorNear(conn_set1, conn_set_old);
                FamilyInstance Fitting1 = doc.Create.NewElbowFitting(list_dt1_dtold[0], list_dt1_dtold[1]);
                Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());

                List<Connector> list_dt1_neighbor = GetConnectorNear(conn_set1, conn_set_neighbor);
                FamilyInstance Fitting2 = doc.Create.NewElbowFitting(list_dt1_neighbor[0], list_dt1_neighbor[1]);
                Fitting2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting2.Id.ToString());

            }
            else if (lst_ele1.Count == 2)
            {
                Duct dt1 = lst_ele1[0];
                Duct dt2 = lst_ele1[1];
                Duct dtold = doc.GetElement(clsBien_MUD.Id_old) as Duct;

                ConnectorSet conn_set1 = dt1.ConnectorManager.Connectors;
                ConnectorSet conn_set2 = dt2.ConnectorManager.Connectors;
                ConnectorSet conn_set_old = dtold.ConnectorManager.Connectors;
                ConnectorSet conn_set_neighbor1 = clsBien_MUD.List_DuctNeightbor[0].ConnectorManager.Connectors;
                ConnectorSet conn_set_neighbor2 = clsBien_MUD.List_DuctNeightbor[1].ConnectorManager.Connectors;


                List<Connector> list_dt1_dtold = GetConnectorNear(conn_set1, conn_set_old);
                FamilyInstance Fitting1 = doc.Create.NewElbowFitting(list_dt1_dtold[0], list_dt1_dtold[1]);
                Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());


                List<Connector> list_dt1_neightbor = GetConnectorNear(conn_set1, conn_set_neighbor1);
                FamilyInstance Fitting2 = doc.Create.NewElbowFitting(list_dt1_neightbor[0], list_dt1_neightbor[1]);
                Fitting2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting2.Id.ToString());


                List<Connector> list_dt2_dtold = GetConnectorNear(conn_set2, conn_set_old);
                FamilyInstance Fitting3 = doc.Create.NewElbowFitting(list_dt2_dtold[0], list_dt2_dtold[1]);
                Fitting3.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting3.Id.ToString());


                List<Connector> list_dt2_neightbor = GetConnectorNear(conn_set2, conn_set_neighbor2);
                FamilyInstance Fitting4 = doc.Create.NewElbowFitting(list_dt2_neightbor[0], list_dt2_neightbor[1]);
                Fitting4.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting4.Id.ToString());
            }
            else if (lst_ele1.Count == 0)
            {
                if (clsBien_MUD.Id_old2 == null)
                {
                    Duct dt1 = doc.GetElement(clsBien_MUD.Id_old1) as Duct;
                    Duct dtold = doc.GetElement(clsBien_MUD.Id_old) as Duct;
                    ConnectorSet conn_set1 = dt1.ConnectorManager.Connectors;
                    ConnectorSet conn_set_old = dtold.ConnectorManager.Connectors;

                    List<Connector> list_dt1_dtold = GetConnectorNear(conn_set1, conn_set_old);
                    FamilyInstance Fitting1 = doc.Create.NewUnionFitting(list_dt1_dtold[0], list_dt1_dtold[1]);
                    Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());
                }
                else
                {
                    Duct dt1 = doc.GetElement(clsBien_MUD.Id_old1) as Duct;
                    Duct dt2 = doc.GetElement(clsBien_MUD.Id_old2) as Duct;
                    Duct dtold = doc.GetElement(clsBien_MUD.Id_old) as Duct;

                    ConnectorSet conn_set1 = dt1.ConnectorManager.Connectors;
                    ConnectorSet conn_set2 = dt2.ConnectorManager.Connectors;
                    ConnectorSet conn_set_old = dtold.ConnectorManager.Connectors;

                    List<Connector> list_dt1_dtold = GetConnectorNear(conn_set1, conn_set_old);
                    FamilyInstance Fitting1 = doc.Create.NewUnionFitting(list_dt1_dtold[0], list_dt1_dtold[1]);
                    Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());


                    List<Connector> list_dt2_dtold = GetConnectorNear(conn_set2, conn_set_old);
                    FamilyInstance Fitting2 = doc.Create.NewUnionFitting(list_dt2_dtold[0], list_dt2_dtold[1]);
                    Fitting2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting2.Id.ToString());
                }
            }
        }
        public void CreatePipeFitting(Document doc, List<Pipe> lst_ele1)
        {
            if (lst_ele1.Count == 1)
            {
                Pipe pi1 = lst_ele1[0];
                Pipe piold = doc.GetElement(clsBien_MUD.Id_old) as Pipe;

                ConnectorSet conn_set1 = pi1.ConnectorManager.Connectors;
                ConnectorSet conn_set_old = piold.ConnectorManager.Connectors;
                ConnectorSet conn_set_neighbor = clsBien_MUD.List_PipeNeightbor[0].ConnectorManager.Connectors;

                List<Connector> list_pi1_piold = GetConnectorNear(conn_set1, conn_set_old);
                FamilyInstance Fitting1 = doc.Create.NewElbowFitting(list_pi1_piold[0], list_pi1_piold[1]);
                Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());

                List<Connector> list_pi1_pineightbor = GetConnectorNear(conn_set1, conn_set_neighbor);
                FamilyInstance Fitting2 = doc.Create.NewElbowFitting(list_pi1_pineightbor[0], list_pi1_pineightbor[1]);
                Fitting2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting2.Id.ToString());
            }
            else if (lst_ele1.Count == 2)
            {
                Pipe pi1 = lst_ele1[0];
                Pipe pi2 = lst_ele1[1];
                Pipe piold = doc.GetElement(clsBien_MUD.Id_old) as Pipe;

                ConnectorSet conn_set1 = pi1.ConnectorManager.Connectors;
                ConnectorSet conn_set2 = pi2.ConnectorManager.Connectors;
                ConnectorSet conn_set_old = piold.ConnectorManager.Connectors;
                ConnectorSet conn_set_neighbor1 = clsBien_MUD.List_PipeNeightbor[0].ConnectorManager.Connectors;
                ConnectorSet conn_set_neighbor2 = clsBien_MUD.List_PipeNeightbor[1].ConnectorManager.Connectors;


                List<Connector> list_pi1_piold = GetConnectorNear(conn_set1, conn_set_old);
                FamilyInstance Fitting1 = doc.Create.NewElbowFitting(list_pi1_piold[0], list_pi1_piold[1]);
                Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());

                List<Connector> list_pi1_pineightbor1 = GetConnectorNear(conn_set1, conn_set_neighbor1);
                FamilyInstance Fitting2 = doc.Create.NewElbowFitting(list_pi1_pineightbor1[0], list_pi1_pineightbor1[1]);
                Fitting2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting2.Id.ToString());

                List<Connector> list_pi2_piold = GetConnectorNear(conn_set2, conn_set_old);
                FamilyInstance Fitting3 = doc.Create.NewElbowFitting(list_pi2_piold[0], list_pi2_piold[1]);
                Fitting3.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting3.Id.ToString());

                List<Connector> list_pi2_pineightbor2 = GetConnectorNear(conn_set2, conn_set_neighbor2);
                FamilyInstance Fitting4 = doc.Create.NewElbowFitting(list_pi2_pineightbor2[0], list_pi2_pineightbor2[1]);
                Fitting4.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting4.Id.ToString());
            }
            else if (lst_ele1.Count == 0)
            {
                if (clsBien_MUD.Id_old2 == null)
                {
                    Pipe pi1 = doc.GetElement(clsBien_MUD.Id_old1) as Pipe;
                    Pipe piold = doc.GetElement(clsBien_MUD.Id_old) as Pipe;
                    ConnectorSet conn_set1 = pi1.ConnectorManager.Connectors;
                    ConnectorSet conn_set_old = piold.ConnectorManager.Connectors;

                    List<Connector> list_pi1_piold = GetConnectorNear(conn_set1, conn_set_old);
                    FamilyInstance Fitting1 = doc.Create.NewUnionFitting(list_pi1_piold[0], list_pi1_piold[1]);
                    Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());
                }
                else
                {
                    Pipe pi1 = doc.GetElement(clsBien_MUD.Id_old1) as Pipe;
                    Pipe pi2 = doc.GetElement(clsBien_MUD.Id_old2) as Pipe;
                    Pipe piold = doc.GetElement(clsBien_MUD.Id_old) as Pipe;

                    ConnectorSet conn_set1 = pi1.ConnectorManager.Connectors;
                    ConnectorSet conn_set2 = pi2.ConnectorManager.Connectors;
                    ConnectorSet conn_set_old = piold.ConnectorManager.Connectors;

                    List<Connector> list_pi1_piold = GetConnectorNear(conn_set1, conn_set_old);
                    FamilyInstance Fitting1 = doc.Create.NewUnionFitting(list_pi1_piold[0], list_pi1_piold[1]);
                    Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());


                    List<Connector> list_pi2_piold = GetConnectorNear(conn_set2, conn_set_old);
                    FamilyInstance Fitting2 = doc.Create.NewUnionFitting(list_pi2_piold[0], list_pi2_piold[1]);
                    Fitting2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting2.Id.ToString());
                }
            }
        }
        public void CreateCbTrayFitting(Document doc, List<CableTray> lst_ele1)
        {
            if (lst_ele1.Count == 1)
            {
                CableTray Cbt1 = lst_ele1[0];
                CableTray Cbtold = doc.GetElement(clsBien_MUD.Id_old) as CableTray;

                ConnectorSet conn_set1 = Cbt1.ConnectorManager.Connectors;
                ConnectorSet conn_set_old = Cbtold.ConnectorManager.Connectors;
                ConnectorSet conn_set_neighbor = clsBien_MUD.List_CbTrayNeightbor[0].ConnectorManager.Connectors;

                List<Connector> list_Cbt1_Cbtold = GetConnectorNear(conn_set1, conn_set_old);
                FamilyInstance Fitting1 = doc.Create.NewElbowFitting(list_Cbt1_Cbtold[0], list_Cbt1_Cbtold[1]);
                Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());

                List<Connector> list_Cbt1_Cbtneighbor = GetConnectorNear(conn_set1, conn_set_neighbor);
                FamilyInstance Fitting2 = doc.Create.NewElbowFitting(list_Cbt1_Cbtneighbor[0], list_Cbt1_Cbtneighbor[1]);
                Fitting2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting2.Id.ToString());
            }
            else if (lst_ele1.Count == 2)
            {
                CableTray Cbt1 = lst_ele1[0];
                CableTray Cbt2 = lst_ele1[1];
                CableTray Cbtold = doc.GetElement(clsBien_MUD.Id_old) as CableTray;

                ConnectorSet conn_set1 = Cbt1.ConnectorManager.Connectors;
                ConnectorSet conn_set2 = Cbt2.ConnectorManager.Connectors;
                ConnectorSet conn_set_old = Cbtold.ConnectorManager.Connectors;
                ConnectorSet conn_set_neighbor1 = clsBien_MUD.List_CbTrayNeightbor[0].ConnectorManager.Connectors;
                ConnectorSet conn_set_neighbor2 = clsBien_MUD.List_CbTrayNeightbor[1].ConnectorManager.Connectors;


                List<Connector> list_Cbt1_Cbtold = GetConnectorNear(conn_set1, conn_set_old);
                FamilyInstance Fitting1 = doc.Create.NewElbowFitting(list_Cbt1_Cbtold[0], list_Cbt1_Cbtold[1]);
                Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());

                List<Connector> list_Cbt1_Cbtneighbor1 = GetConnectorNear(conn_set1, conn_set_neighbor1);
                FamilyInstance Fitting2 = doc.Create.NewElbowFitting(list_Cbt1_Cbtneighbor1[0], list_Cbt1_Cbtneighbor1[1]);
                Fitting2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting2.Id.ToString());

                List<Connector> list_Cbt2_Cbtold = GetConnectorNear(conn_set2, conn_set_old);
                FamilyInstance Fitting3 = doc.Create.NewElbowFitting(list_Cbt2_Cbtold[0], list_Cbt2_Cbtold[1]);
                Fitting3.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting3.Id.ToString());

                List<Connector> list_Cbt2_Cbtneighbor2 = GetConnectorNear(conn_set2, conn_set_neighbor2);
                FamilyInstance Fitting4 = doc.Create.NewElbowFitting(list_Cbt2_Cbtneighbor2[0], list_Cbt2_Cbtneighbor2[1]);
                Fitting4.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting4.Id.ToString());
            }
            else if (lst_ele1.Count == 0)
            {
                if (clsBien_MUD.Id_old2 == null)
                {
                    CableTray Cbt1 = doc.GetElement(clsBien_MUD.Id_old1) as CableTray;
                    CableTray Cbtold = doc.GetElement(clsBien_MUD.Id_old) as CableTray;
                    ConnectorSet conn_set1 = Cbt1.ConnectorManager.Connectors;
                    ConnectorSet conn_set_old = Cbtold.ConnectorManager.Connectors;

                    List<Connector> list_Cbt1_Cbtold = GetConnectorNear(conn_set1, conn_set_old);
                    FamilyInstance Fitting1 = doc.Create.NewUnionFitting(list_Cbt1_Cbtold[0], list_Cbt1_Cbtold[1]);
                    Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());
                }
                else
                {
                    CableTray Cbt1 = doc.GetElement(clsBien_MUD.Id_old1) as CableTray;
                    CableTray Cbt2 = doc.GetElement(clsBien_MUD.Id_old2) as CableTray;
                    CableTray Cbtold = doc.GetElement(clsBien_MUD.Id_old) as CableTray;

                    ConnectorSet conn_set1 = Cbt1.ConnectorManager.Connectors;
                    ConnectorSet conn_set2 = Cbt2.ConnectorManager.Connectors;
                    ConnectorSet conn_set_old = Cbtold.ConnectorManager.Connectors;

                    List<Connector> list_Cbt1_Cbtold = GetConnectorNear(conn_set1, conn_set_old);
                    FamilyInstance Fitting1 = doc.Create.NewUnionFitting(list_Cbt1_Cbtold[0], list_Cbt1_Cbtold[1]);
                    Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());


                    List<Connector> list_Cbt2_Cbtold = GetConnectorNear(conn_set2, conn_set_old);
                    FamilyInstance Fitting2 = doc.Create.NewUnionFitting(list_Cbt2_Cbtold[0], list_Cbt2_Cbtold[1]);
                    Fitting2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting2.Id.ToString());
                }
            }
        }
        public void CreateConduitFitting(Document doc, List<Conduit> lst_ele1)
        {
            if (lst_ele1.Count == 1)
            {
                Conduit Cd1 = lst_ele1[0];
                Conduit Cdold = doc.GetElement(clsBien_MUD.Id_old) as Conduit;

                ConnectorSet conn_set1 = Cd1.ConnectorManager.Connectors;
                ConnectorSet conn_set_old = Cdold.ConnectorManager.Connectors;
                ConnectorSet conn_set_neighbor = clsBien_MUD.List_ConduitNeightbor[0].ConnectorManager.Connectors;

                List<Connector> list_Cd1_Cdold = GetConnectorNear(conn_set1, conn_set_old);
                FamilyInstance Fitting1 = doc.Create.NewElbowFitting(list_Cd1_Cdold[0], list_Cd1_Cdold[1]);
                Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());


                List<Connector> list_Cd1_Cdneighbor = GetConnectorNear(conn_set1, conn_set_neighbor);
                FamilyInstance Fitting2 = doc.Create.NewElbowFitting(list_Cd1_Cdneighbor[0], list_Cd1_Cdneighbor[1]);
                Fitting2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting2.Id.ToString());
            }
            else if (lst_ele1.Count == 2)
            {
                Conduit Cd1 = lst_ele1[0];
                Conduit Cd2 = lst_ele1[1];
                Conduit Cdold = doc.GetElement(clsBien_MUD.Id_old) as Conduit;

                ConnectorSet conn_set1 = Cd1.ConnectorManager.Connectors;
                ConnectorSet conn_set2 = Cd2.ConnectorManager.Connectors;
                ConnectorSet conn_set_old = Cdold.ConnectorManager.Connectors;
                ConnectorSet conn_set_neighbor1 = clsBien_MUD.List_ConduitNeightbor[0].ConnectorManager.Connectors;
                ConnectorSet conn_set_neighbor2 = clsBien_MUD.List_ConduitNeightbor[1].ConnectorManager.Connectors;


                List<Connector> list_Cd1_Cdold = GetConnectorNear(conn_set1, conn_set_old);
                FamilyInstance Fitting1 = doc.Create.NewElbowFitting(list_Cd1_Cdold[0], list_Cd1_Cdold[1]);
                Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());

                List<Connector> list_Cd1_Cdneighbor1 = GetConnectorNear(conn_set1, conn_set_neighbor1);
                FamilyInstance Fitting2 = doc.Create.NewElbowFitting(list_Cd1_Cdneighbor1[0], list_Cd1_Cdneighbor1[1]);
                Fitting2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting2.Id.ToString());

                List<Connector> list_Cd2_Cdold = GetConnectorNear(conn_set2, conn_set_old);
                FamilyInstance Fitting3 = doc.Create.NewElbowFitting(list_Cd2_Cdold[0], list_Cd2_Cdold[1]);
                Fitting3.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting3.Id.ToString());

                List<Connector> list_Cd1_Cdneighbor2 = GetConnectorNear(conn_set2, conn_set_neighbor2);
                FamilyInstance Fitting4 = doc.Create.NewElbowFitting(list_Cd1_Cdneighbor2[0], list_Cd1_Cdneighbor2[1]);
                Fitting4.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting4.Id.ToString());


            }
            else if (lst_ele1.Count == 0)
            {
                if (clsBien_MUD.Id_old2 == null)
                {
                    Conduit Cd1 = doc.GetElement(clsBien_MUD.Id_old1) as Conduit;
                    Conduit Cdold = doc.GetElement(clsBien_MUD.Id_old) as Conduit;
                    ConnectorSet conn_set1 = Cd1.ConnectorManager.Connectors;
                    ConnectorSet conn_set_old = Cdold.ConnectorManager.Connectors;

                    List<Connector> list_Cd1_Cdold = GetConnectorNear(conn_set1, conn_set_old);
                    FamilyInstance Fitting1 = doc.Create.NewUnionFitting(list_Cd1_Cdold[0], list_Cd1_Cdold[1]);
                    Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());
                }
                else
                {
                    Conduit Cd1 = doc.GetElement(clsBien_MUD.Id_old1) as Conduit;
                    Conduit Cd2 = doc.GetElement(clsBien_MUD.Id_old2) as Conduit;
                    Conduit Cdold = doc.GetElement(clsBien_MUD.Id_old) as Conduit;

                    ConnectorSet conn_set1 = Cd1.ConnectorManager.Connectors;
                    ConnectorSet conn_set2 = Cd2.ConnectorManager.Connectors;
                    ConnectorSet conn_set_old = Cdold.ConnectorManager.Connectors;

                    List<Connector> list_Cd1_Cdold = GetConnectorNear(conn_set1, conn_set_old);
                    FamilyInstance Fitting1 = doc.Create.NewUnionFitting(list_Cd1_Cdold[0], list_Cd1_Cdold[1]);
                    Fitting1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting1.Id.ToString());

                    List<Connector> list_Cd2_Cdold = GetConnectorNear(conn_set2, conn_set_old);
                    FamilyInstance Fitting2 = doc.Create.NewUnionFitting(list_Cd2_Cdold[0], list_Cd2_Cdold[1]);
                    Fitting2.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(Fitting2.Id.ToString());
                }
            }
        }
        public void GetElementNeighbor(Connector Conn_Input)
        {
            List<Element> ConnectedElements = new List<Element>();
            var allRef = Conn_Input.AllRefs;
            foreach (Connector connector in allRef)
            {
                ConnectedElements.Add(connector.Owner);
            }
            foreach (Element ele in ConnectedElements)
            {
                if (ele is Duct)
                {
                    clsBien_MUD.List_DuctNeightbor.Add(ele as Duct);
                }
                else if (ele is Pipe)
                {
                    clsBien_MUD.List_PipeNeightbor.Add(ele as Pipe);
                }
                else if (ele is CableTray)
                {
                    clsBien_MUD.List_CbTrayNeightbor.Add(ele as CableTray);
                }
                else if (ele is Conduit)
                {
                    clsBien_MUD.List_ConduitNeightbor.Add(ele as Conduit);
                }
            }

        }
        public void DeleteFitting(Document doc)
        {
            if (clsBien_MUD.List_Connect.Count == 1)
            {
                clsBien_MUD.Id_old1 = GetFitingAndElement(doc, clsBien_MUD.List_Connect[0]);

            }
            else if (clsBien_MUD.List_Connect.Count == 2)
            {
                clsBien_MUD.Id_old1 = GetFitingAndElement(doc, clsBien_MUD.List_Connect[0]);
                clsBien_MUD.Id_old2 = GetFitingAndElement(doc, clsBien_MUD.List_Connect[1]);
            }
        }
        public ElementId GetFitingAndElement(Document doc, Connector InputCon)
        {
            ElementId result = null;
            List<Element> ConnectedElements1 = new List<Element>();
            var allRef1 = InputCon.AllRefs;
            foreach (Connector connector in allRef1)
            {
                ConnectedElements1.Add(connector.Owner);
            }
            FamilyInstance Fit1 = null;
            foreach (Element ele in ConnectedElements1)
            {
                if (ele is FamilyInstance)
                {
                    Fit1 = ele as FamilyInstance;
                }
            }
            if (Fit1 != null)
            {
                if (Fit1.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsValueString() != "")
                {

                    ConnectorSetIterator csi = null;
                    csi = Fit1.MEPModel.ConnectorManager.Connectors.ForwardIterator();
                    Connector Conn_Fit1 = null;
                    while (csi.MoveNext())
                    {
                        Connector conn = csi.Current as Connector;
                        if (Math.Round(conn.Origin.Z, 5) != Math.Round(clsBien_MUD.List_Connect[0].Origin.Z, 5))
                        {
                            Conn_Fit1 = conn;
                        }
                    }
                    if (Conn_Fit1 != null)
                    {
                        var allRef_Fit1 = Conn_Fit1.AllRefs;
                        List<Element> ConnectedElements_Fit1 = new List<Element>();
                        foreach (Connector connector in allRef_Fit1)
                        {
                            ConnectedElements_Fit1.Add(connector.Owner);
                        }
                        foreach (Element ele in ConnectedElements_Fit1)
                        {
                            if (ele is Duct)
                            {
                                Duct dt1 = ele as Duct;
                                ConnectorSetIterator csi1 = null;
                                csi1 = dt1.ConnectorManager.Connectors.ForwardIterator();
                                result = DelFaE(doc, csi1, Conn_Fit1, Fit1.Id, dt1.Id);
                            }
                            else if (ele is Pipe)
                            {
                                Pipe pi1 = ele as Pipe;
                                ConnectorSetIterator csi1 = null;
                                csi1 = pi1.ConnectorManager.Connectors.ForwardIterator();
                                result = DelFaE(doc, csi1, Conn_Fit1, Fit1.Id, pi1.Id);
                            }
                            else if (ele is CableTray)
                            {
                                CableTray Cbt1 = ele as CableTray;
                                ConnectorSetIterator csi1 = null;
                                csi1 = Cbt1.ConnectorManager.Connectors.ForwardIterator();
                                result = DelFaE(doc, csi1, Conn_Fit1, Fit1.Id, Cbt1.Id);
                            }
                            else if (ele is Conduit)
                            {
                                Conduit Cd = ele as Conduit;
                                ConnectorSetIterator csi1 = null;
                                csi1 = Cd.ConnectorManager.Connectors.ForwardIterator();
                                result = DelFaE(doc, csi1, Conn_Fit1, Fit1.Id, Cd.Id);
                            }
                        }
                    }

                }

            }
            return result;
        }
        public ElementId DelFaE(Document doc, ConnectorSetIterator csi, Connector Conn_Fit1, ElementId Id1, ElementId Id2)
        {
            ElementId result = null;
            Connector Conn_Elem1 = null;
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                if (Math.Round(conn.Origin.Z, 5) != Math.Round(Conn_Fit1.Origin.Z, 5))
                {
                    Conn_Elem1 = conn;
                }
            }
            var allRef_Elem1 = Conn_Elem1.AllRefs;
            List<Element> ConnectedElements_Elem1 = new List<Element>();
            foreach (Connector connector in allRef_Elem1)
            {
                ConnectedElements_Elem1.Add(connector.Owner);
            }
            foreach (Element ele1 in ConnectedElements_Elem1)
            {
                if (ele1 is FamilyInstance)
                {
                    FamilyInstance Fit2 = ele1 as FamilyInstance;
                    ConnectorSetIterator csi_Fit2 = null;
                    csi_Fit2 = Fit2.MEPModel.ConnectorManager.Connectors.ForwardIterator();
                    Connector Conn_Fit2 = null;
                    while (csi_Fit2.MoveNext())
                    {
                        Connector conn = csi_Fit2.Current as Connector;
                        if (Math.Round(conn.Origin.Z, 5) != Math.Round(Conn_Elem1.Origin.Z, 5))
                        {
                            Conn_Fit2 = conn;
                        }
                    }
                    var allRef_Fit2 = Conn_Fit2.AllRefs;
                    List<Element> ConnectedElements_Fit2 = new List<Element>();
                    foreach (Connector connector in allRef_Fit2)
                    {
                        ConnectedElements_Fit2.Add(connector.Owner);
                    }
                    result = ConnectedElements_Fit2[0].Id;
                    doc.Delete(Id1);
                    doc.Delete(Id2);
                    doc.Delete(ele1.Id);
                }
            }


            return result;
        }
        public List<Connector> GetConnectorNear(ConnectorSet c1, ConnectorSet c2)
        {

            Connector conn1 = null;
            Connector conn2 = null;

            double mindist = 100000;

            foreach (Connector con1 in c1)
            {
                foreach (Connector con2 in c2)
                {
                    double conndist = con1.Origin.DistanceTo(con2.Origin);
                    if (conndist < mindist)
                    {
                        mindist = conndist;
                        conn1 = con1;
                        conn2 = con2;
                    }
                }
            }
            List<Connector> listresult = new List<Connector>();
            listresult.Add(conn1);
            listresult.Add(conn2);
            return listresult;
        }
        public double CornerBetweenTwoVector(XYZ Vector1, XYZ Vector2, string Cor, string DK_input)
        {
            double result = 0;
            if (DK_input == "90")
            {
                double tu = Vector1.X * Vector2.X + Vector1.Y * Vector2.Y;
                //+ Vector1.Z * Vector2.Z;
                double mau = Math.Sqrt(Vector1.X * Vector1.X + Vector1.Y * Vector1.Y) * Math.Sqrt(Vector2.X * Vector2.X + Vector2.Y * Vector2.Y);
                result = Math.Acos(Math.Abs(tu / mau));
                if (Cor == "GiuNguyen")
                {
                    return result;
                }
                else
                {
                    return - result;
                }
            }
            else
            {
                if (Cor == "GiuNguyen")
                {
                    return Math.PI/2;
                }
                else
                {
                    return -Math.PI / 2; ;
                }
            }                        
        }
        public XYZ GetPointDt(Duct dt_input, Connector Conn_Input)
        {
            XYZ result = new XYZ();
            ConnectorSet Conset = dt_input.ConnectorManager.Connectors;
            foreach(Connector Con in Conset)
            {
                if(Con.Origin.IsAlmostEqualTo(Conn_Input.Origin))
                {

                }
                else
                {
                    result = Con.Origin;
                }    
            }
            return result;
        }
        public XYZ GetPointPi(Pipe pi_input, Connector Conn_Input)
        {
            XYZ result = new XYZ();
            ConnectorSet Conset = pi_input.ConnectorManager.Connectors;
            foreach (Connector Con in Conset)
            {
                if (Con.Origin.IsAlmostEqualTo(Conn_Input.Origin))
                {

                }
                else
                {
                    result = Con.Origin;
                }
            }
            return result;
        }
        public XYZ GetPointCT(CableTray CT_input, Connector Conn_Input)
        {
            XYZ result = new XYZ();
            ConnectorSet Conset = CT_input.ConnectorManager.Connectors;
            foreach (Connector Con in Conset)
            {
                if (Con.Origin.IsAlmostEqualTo(Conn_Input.Origin))
                {

                }
                else
                {
                    result = Con.Origin;
                }
            }
            return result;
        }
        public XYZ GetPointCN(Conduit Cn_input, Connector Conn_Input)
        {
            XYZ result = new XYZ();
            ConnectorSet Conset = Cn_input.ConnectorManager.Connectors;
            foreach (Connector Con in Conset)
            {
                if (Con.Origin.IsAlmostEqualTo(Conn_Input.Origin))
                {

                }
                else
                {
                    result = Con.Origin;
                }
            }
            return result;
        }

    }
}
