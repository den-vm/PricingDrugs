using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceJob.Interface;

namespace ServiceJob.Classes
{
    public class ControlTable : IControlRowsTable
    {
        public List<object> FilterRows(string nameTable, string listFilter, List<List<object>[]> allTableJvnlp)
        {
            var filterList = new List<object>();
            var filtresCast = JsonConvert.DeserializeObject<string[]>(listFilter);
            var emptyFilter = filtresCast.All(textFilter => textFilter == "");
            try
            {
                object[] activeTable = null;
                if (nameTable.Equals("tableDrugs"))
                    activeTable = allTableJvnlp[(int)JvnlpLists.JVNLP];
                if (nameTable.Equals("exjvnlpTable"))
                    activeTable = allTableJvnlp[(int)JvnlpLists.Excluded];

                if (emptyFilter) // если текст фильтра пуст то возращаем всю исходную таблицу 
                {
                    filterList = (activeTable ?? throw new InvalidOperationException("Реестр таблиц пуст")).ToList();
                }
                else
                {
                    if (activeTable == null)
                        throw new InvalidOperationException("Реестр таблиц пуст");

                    var tempItems = activeTable;
                    for (var i = 0; i < filtresCast.Length; i++)
                    {
                        var numColumn = i;
                        tempItems = tempItems.WhereIf(!filtresCast[i].Equals(""),
                            (item, j) =>
                            {
                                if (j <= 2)
                                    return true;
                                var itemArray = (List<object>)item;
                                var strItemArray = itemArray[numColumn].GetType().BaseType.Name.Equals("ValueType")
                                    ? itemArray[numColumn].ToString().Replace(",", ".")
                                    : itemArray[numColumn].ToString();
                                var isContains = strItemArray.ToUpper().Contains(filtresCast[numColumn].ToUpper());
                                return isContains;
                            }).ToArray();
                    }

                    filterList = tempItems.ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return filterList;
        }
    }
}
