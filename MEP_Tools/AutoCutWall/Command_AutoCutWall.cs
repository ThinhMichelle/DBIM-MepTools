#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

#endregion

namespace MEP_Tools
{
    [Transaction(TransactionMode.Manual)]
    public class Command_AutoCutWall : WPFData, IExternalCommand
    {
        public ICommand CommandOK { get; set; }
        public ICommand CommandCancel { get; set; }
        public ICommand CommandTest { get; set; }
        public Command_AutoCutWall()
        {
            CommandOK = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                try
                {
                    Func_CutWall F = new Func_CutWall();
                    p.Hide();             
                    IList<Reference> items = SingleData.Singleton.Instance.RevitData.Selection.PickObjects(ObjectType.Element, "Please choose walls");
                    if (items != null)
                    {
                        foreach (var r in items)
                        {
                            SingleData.Singleton.Instance.RevitData.Transaction.Start();
                            Wall wall_Select = SingleData.Singleton.Instance.RevitData.Document.GetElement(r) as Wall;
                            BoundingBoxXYZ bb = SingleData.Singleton.Instance.RevitData.Document.GetElement(r).get_BoundingBox(SingleData.Singleton.Instance.RevitData.Document.ActiveView);
                            Outline outline = new Outline(bb.Min, bb.Max);
                            BoundingBoxIntersectsFilter bbfilter = new BoundingBoxIntersectsFilter(outline);

                            ICollection<ElementId> idsExclude = new List<ElementId>();
                            idsExclude.Add(SingleData.Singleton.Instance.RevitData.Document.GetElement(r).Id);

                            List<FamilyInstance> list_Framing = new List<FamilyInstance>();
                            List<Floor> list_Floor = new List<Floor>();
                            foreach (RevitLinkType item in cls_Arc.List_LinkRevitSelect)
                            {
                                foreach (Document LinkedDoc in SingleData.Singleton.Instance.RevitData.Application.Documents)
                                {
                                    if (LinkedDoc.Title.Equals(item.Name.Split(new char[] { '.' })[0]))
                                    {
                                        FilteredElementCollector collLinked = new FilteredElementCollector(LinkedDoc);
                                        collLinked.Excluding(idsExclude).WherePasses(bbfilter);

                                        foreach (Element e in collLinked)
                                        {
                                            if (e is FamilyInstance)
                                            {
                                                list_Framing.Add(e as FamilyInstance);
                                            }
                                            else if (e is Floor)
                                            {
                                                list_Floor.Add(e as Floor);
                                            }

                                        }
                                    }
                                }
                            }
                            double count = list_Floor.Count + list_Framing.Count;
                            double phantram = 100 / count;
                           
                            foreach (var item in list_Floor)
                            {
                                F.Intersec_With_Floor(SingleData.Singleton.Instance.RevitData.Document, wall_Select, item);
                                //SingleData.Singleton.Instance.WFData.Inputwindow_AN.PB.Value += phantram;
                            }
                            SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                            foreach (var item in list_Framing)
                            {
                                SingleData.Singleton.Instance.RevitData.Transaction.Start();
                                F.Intersec_With_Framing(SingleData.Singleton.Instance.RevitData.Document, wall_Select, item);
                                SingleData.Singleton.Instance.RevitData.Transaction.Commit();
                                //SingleData.Singleton.Instance.WFData.Inputwindow_AN.PB.Value += phantram;
                            }                          
                        }                      
                    }
                    p.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    SingleData.Singleton.Instance.RevitData.Transaction.RollBack();
                }
            });

            CommandCancel = new RelayCommand<System.Windows.Window>((p) => { return p == null ? false : true; }, (p) =>
            {
                p.Close();
            });
           
        }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            SingleData.Singleton.Instance = new SingleData.Singleton();
            SingleData.Singleton.Instance.RevitData.UIApplication = commandData.Application;
            #region 'Get Link'
            cls_Arc.List_LinkRevitFirst.Clear();
            IList<Element> source = new FilteredElementCollector(SingleData.Singleton.Instance.RevitData.Document).OfCategory(BuiltInCategory.OST_RvtLinks).OfClass(typeof(RevitLinkType)).ToElements();
            if (source.Count() > 0)
            {
                foreach (Element ele in source)
                {
                    if (ele is RevitLinkType)
                    {
                        cls_Arc.List_LinkRevitFirst.Add(ele);
                    }
                }
            }
            SingleData.Singleton.Instance.WFData.Inputwindow_AN.ShowDialog();
            return Result.Succeeded;
            #endregion


        }
    }
}
