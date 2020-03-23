using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceJob.Models
{
    public class ListCriteriasModels
    {
        public CriteriaModel before50on { get; set; }
        public CriteriaModel after50before500on { get; set; }
        public CriteriaModel after500 { get; set; }
        public string nds { get; set; }
    }
}
