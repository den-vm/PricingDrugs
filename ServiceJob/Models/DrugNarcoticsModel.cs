using System;

namespace ServiceJob.Models
{
    public class DrugNarcoticsModel
    {
        public int Id { get; set; }
        public string NameDrug { get; set; }
        public DateTime IncludeDate { get; set; }
        public DateTime? OutDate { get; set; }
    }
}