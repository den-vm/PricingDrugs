using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceJob.Interface
{
    interface ICalculateDrugs
    {
        Task<string> ReadLastDateUpdate(string headerList);
        List<object>[] SearchIncludeDrugs(List<object>[] regDrugs, string lastDateUpdate);
        void Start(List<object>[] regDrugs);
        List<object>[] JvnlpCalculated { get; }
        List<object>[] IncludeCalculated { get; }
    }
}
