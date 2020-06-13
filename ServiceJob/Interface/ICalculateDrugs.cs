using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceJob.Interface
{
    interface ICalculateDrugs
    {
        Task<string> ReadLastDateUpdate();
    }
}
