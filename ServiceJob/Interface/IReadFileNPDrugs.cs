using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceJob.Interface
{
    /// <summary>
    /// Processing Narcotic and Psychotropic Drugs 
    /// </summary>
    /// <typeparam name="T">Model Narcotic and Psychotropic Drugs</typeparam>
    interface IReadFileNPDrugs <T>
    {
        /// <summary>
        /// Return list included drugs 
        /// </summary>
        /// <returns></returns>
        List<T> Get();
        /// <summary>
        /// Add drugs to file
        /// </summary>
        /// <param name="listdrugs"></param>
        /// <returns></returns>
        bool Add(List<T> listdrugs);
        /// <summary>
        /// Delete drugs the file
        /// </summary>
        void Delete();
        /// <summary>
        /// Change drugs the file 
        /// </summary>
        void Edit();
    }
}
