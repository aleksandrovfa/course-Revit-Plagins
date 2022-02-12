using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateNumRooms
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class NumRooms : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document arDoc = commandData.Application.ActiveUIDocument.Document;

            var rooms = new FilteredElementCollector(arDoc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .Select(r => r as Room)
                .GroupBy(x => x.LevelId)
                .ToList();
           
            Transaction transaction = new Transaction(arDoc);
            transaction.Start("Нумерация пространств");
            foreach (var Levelid in rooms)
            {
                string lvlName = arDoc.GetElement(Levelid.Key).Name;
                lvlName = lvlName.Replace("Уровень ", "");
                int Num = 1;
                foreach (var item in Levelid)
                {
                    item.LookupParameter("Номер").Set(lvlName+ "_" + Num.ToString());
                    Num++;
                }

            }
            transaction.Commit();
            return Result.Succeeded;
        }
    }
}
