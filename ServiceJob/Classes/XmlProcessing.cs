using System;
using System.Xml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using ServiceJob.Models;

namespace ServiceJob.Classes
{
    public class ProcessingNDrugs
    {
        public static List<DrugNarcoticsModel> Drugs { get; private set; }
        public static void AddNDrugs()
        {
        }

        public static void DeleteNDrugs()
        {
        }

        public static void EditNDrugs()
        {
        }

        public static List<DrugNarcoticsModel> GetNDrugs()
        {
            Drugs = new List<DrugNarcoticsModel>();
            var path = Directory.GetCurrentDirectory() + "/BaseDrugs/drugsDoc.xml";
            var xDoc = new XmlDocument();
            try
            {
                xDoc.Load(path);
            }
            catch (Exception e)
            {
                var xdoc = new XDocument();
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

                var phones = new XElement("phones");
                phones.Add(iphone6);
                phones.Add(galaxys5);
                xdoc.Add(phones);
                xdoc.Save(path);
            }
            return Drugs;
        }
    }
}