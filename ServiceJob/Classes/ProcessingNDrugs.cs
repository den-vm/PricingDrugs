using System;
using System.Xml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;
using ServiceJob.Interface;
using ServiceJob.Models;

namespace ServiceJob.Classes
{
    public class ProcessingNDrugs : IReadFileNPDrugs<DrugNarcoticsModel>
    {
        private static readonly string Path = Directory.GetCurrentDirectory() + "/BaseDrugs/NDrugsReestr.xml";

        public List<DrugNarcoticsModel> Get()
        {
            var drugsList = new List<DrugNarcoticsModel>();

            try
            {
                var xDoc = new XDocument(Path);

            }
            catch
            {
                var xDoc = new XDocument();
                var drugs = new XElement("drugs");
                xDoc.Add(drugs);
                xDoc.Save(Path);
            }
            return drugsList;
        }

        public bool Add(List<DrugNarcoticsModel> listdrugs)
        {
            var xDoc = new XDocument(Path);
            var drugs = xDoc.Element("drugs");

            var iphone6 = new XElement("phone");
            var iphoneNameAttr = new XAttribute("name", "iPhone 6");
            var iphoneCompanyElem = new XElement("company", "Apple");
            var iphonePriceElem = new XElement("price", "40000");
            iphone6.Add(iphoneNameAttr);
            iphone6.Add(iphoneCompanyElem);
            iphone6.Add(iphonePriceElem);

            var galaxys5 = new XElement("phone");
            var galaxysNameAttr = new XAttribute("name", "Samsung Galaxy S5");
            var galaxysCompanyElem = new XElement("company", "Samsung");
            var galaxysPriceElem = new XElement("price", "33000");
            galaxys5.Add(galaxysNameAttr);
            galaxys5.Add(galaxysCompanyElem);
            galaxys5.Add(galaxysPriceElem);

            if (drugs != null)
            {
                drugs.Add(iphone6);
                drugs.Add(galaxys5);
                xDoc.Add(drugs);
                xDoc.Save(Path);
                return true;
            }
            return false;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void Edit()
        {
            throw new NotImplementedException();
        }
    }
}