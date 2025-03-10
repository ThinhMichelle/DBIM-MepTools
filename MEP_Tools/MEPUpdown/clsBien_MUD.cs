using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Electrical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEP_Tools
{
    public static class clsBien_MUD
    {
        public static List<Connector> List_Connect = new List<Connector>();
        public static ElementId Id_old = null;
        public static ElementId Id_old1 = null;
        public static ElementId Id_old2 = null;
        public static ElementId Id_Fitting1 = null;
        public static ElementId Id_Fitting2 = null;
        public static Connector ConnectorWithUnion1 = null;
        public static Connector ConnectorWithUnion2 = null;
        public static string OffsetValue;
        public static ElementId SystemTypeIDOld = null;
        public static string AlphaElbow = "";
        public static DuctType DuctTypeOld = null;
        public static PipeType PipeTypeOld = null;
        public static ElementId CbTrayTypeOldID = null;
        public static ElementId ConduitTypeOldID = null;
        public static ElementId LVId = null;
        public static Connector ConnectorUnion1 = null;
        public static Connector ConnectorUnion2 = null;
        public static List<Duct> List_DuctNew = new List<Duct>();
        public static List<Pipe> List_PipeNew = new List<Pipe>();
        public static List<CableTray> List_CbTrayNew = new List<CableTray>();
        public static List<Conduit> List_CondiutNew = new List<Conduit>();
        public static List<Duct> List_DuctNeightbor = new List<Duct>();
        public static List<Pipe> List_PipeNeightbor = new List<Pipe>();
        public static List<CableTray> List_CbTrayNeightbor = new List<CableTray>();
        public static List<Conduit> List_ConduitNeightbor = new List<Conduit>();
        public static string Diameter;
        public static string Width;
        public static string Height;
        public static string FittingType;
    }
}
