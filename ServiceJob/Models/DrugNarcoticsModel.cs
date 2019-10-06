using System;
using System.Collections.Generic;

namespace ServiceJob.Models
{
    public class DrugNarcoticsModel
    {
        public string NameDrug { get; private set; }
        public DateTime InDateAdd { get; private set; }

        public Dictionary<int, DrugNarcoticsModel> ReadFileDrugs()
        {
            var a = new Dictionary<int, DrugNarcoticsModel>();
            var b = new DrugNarcoticsModel {NameDrug = "12343", InDateAdd = DateTime.Now};
            a.Add(1, b);
            return a;
        }
    }
}