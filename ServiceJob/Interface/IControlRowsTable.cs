using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceJob.Interface
{
    interface IControlRowsTable
    {
        List<object> FilterRows(string nameTable, string listFilter, List<List<object>[]> allTableJvnlp);
    }
}
