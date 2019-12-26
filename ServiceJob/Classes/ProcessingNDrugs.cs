using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            try
            {
                var xRoot = XDocument.Load(Path).Element("Drugs");
                DateTime tempoutdate;
                var listDrugs = xRoot.Elements("Drug").Select(element => new DrugNarcoticsModel
                {
                    Id = int.Parse(element.Attribute("Id")?.Value),
                    NameDrug = element.Attribute("Name")?.Value,
                    IncludeDate = DateTime.Parse(element.Attribute("IncludeDate")?.Value),
                    OutDate = DateTime.TryParse(element.Attribute("OutDate")?.Value, out tempoutdate)
                        ? tempoutdate
                        : (DateTime?) null
                }).ToList();
                return listDrugs;
            }
            catch (Exception ex)
            {
                switch (ex.HResult)
                {
                    case -2147024894:
                        var xDoc = new XDocument();
                        xDoc.Add(new XElement("Drugs"));
                        xDoc.Save(Path);
                        return new List<DrugNarcoticsModel>();
                }
            }
            return null;
        }

        public bool Add(List<DrugNarcoticsModel> listdrugs)
        {
            try
            {
                var xDoc = XDocument.Load(Path);
                var xRoot = xDoc.Element("Drugs");

                foreach (var infoDrugs in listdrugs)
                {
                    var xNode = new XElement("Drug");
                    var aId = new XAttribute("Id", infoDrugs.Id);
                    var aName = new XAttribute("Name", infoDrugs.NameDrug);
                    var aInludeDate = infoDrugs.IncludeDate != null
                        ? new XAttribute("IncludeDate", infoDrugs.IncludeDate.Value.ToString("dd.MM.yyyy"))
                        : null;
                    var aOutDate = infoDrugs.OutDate != null
                        ? new XAttribute("OutDate", infoDrugs.OutDate.Value.ToString("dd.MM.yyyy"))
                        : null;
                    xNode.Add(aId, aName, aInludeDate, aOutDate);
                    xRoot.Add(xNode);
                }
                if (xRoot != null)
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
            try
            {
                var xDoc = XDocument.Load(Path);
                var xRoot = xDoc.Element("Drugs");
                var xElements = xRoot.Elements("Drug");
                foreach (var infoDrug in listdrugs)
                {
                    var drug = xElements.SingleOrDefault(element =>
                        element.Attribute("Id").Value.Equals(infoDrug.Id.ToString()));
                    drug.Attribute("Name").Value = infoDrug.NameDrug;
                    drug.Attribute("IncludeDate").Value = infoDrug.IncludeDate.Value.ToString("dd.MM.yyyy");
                    if (drug.Attribute("OutDate")?.Value != null)
                    {
                        if (infoDrug.OutDate != null)
                            drug.Attribute("OutDate").Value = infoDrug.OutDate.Value.ToString("dd.MM.yyyy");
                        else drug.Attribute("OutDate").Remove();
                    }
                    else
                    {
                        var aOutDate = infoDrug.OutDate != null
                            ? new XAttribute("OutDate", infoDrug.OutDate.Value.ToString("dd.MM.yyyy"))
                            : null;
                        if (aOutDate != null)
                            drug.Add(aOutDate);
                    }
                }

                if (xRoot != null)
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

        public int GetNewKey()
        {
            try
            {
                var xRoot = XDocument.Load(Path).Element("Drugs")?.Elements("Drug");
                if (!xRoot.Any())
                    return 0;
                var lastKey = int.Parse(xRoot.Last().Attribute("Id")?.Value ?? "0") + 1;
                return lastKey;
            }
            catch (Exception e)
            {
                return -1;
            }
        }
    }
}