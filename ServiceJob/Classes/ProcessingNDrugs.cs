using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using ServiceJob.Interface;
using ServiceJob.Models;

namespace ServiceJob.Classes
{
    public class ProcessingNDrugs : IReadFileNPDrugs<DrugNarcoticsModel>
    {
        private static readonly string Path = Directory.GetCurrentDirectory() + "\\BaseDrugs\\NDrugsReestr.xml";

        public List<DrugNarcoticsModel> GetDrugs()
        {
            var drugsList = new List<DrugNarcoticsModel>();

            try
            {
                var xDoc = XDocument.Load(Path);
            }
            catch
            {
                var xDoc = new XDocument();
                xDoc.Add(new XElement("Drugs"));
                xDoc.Save(Path);
            }
            return drugsList;
        }

        public bool Add(List<DrugNarcoticsModel> listdrugs)
        {
            try
            {
                var xDoc = XDocument.Load(Path);
                var xDrugs = xDoc.Element("Drugs");

                foreach (var infoDrugs in listdrugs)
                {
                    var eDrug = new XElement("Drug");
                    var aId = new XAttribute("Id", infoDrugs.Id);
                    var aName = new XAttribute("name", infoDrugs.NameDrug);
                    var aInludeDate = infoDrugs.IncludeDate != null
                        ? new XAttribute("IncludeDate", infoDrugs.IncludeDate.Value.ToString("MM/dd/yyyy"))
                        : null;
                    var aOutDate = infoDrugs.OutDate != null
                        ? new XAttribute("OutDate", infoDrugs.OutDate.Value.ToString("MM/dd/yyyy"))
                        : null;
                    eDrug.Add(aId, aName, aInludeDate, aOutDate);

                    xDrugs.Add(eDrug);
                }
                if (xDrugs != null)
                {
                    xDoc.Save(Path);
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool Edit(List<DrugNarcoticsModel> listdrugs)
        {
            return true;
        }

        public int GetNewKey()
        {
            return 0;
        }
    }
}