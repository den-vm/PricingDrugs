using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExcelDataReader;

namespace ServiceJob.Models
{
    public class UploadFile
    {
        public DataTable Drugs { get; protected set; }
        public DataTable RemovedDrugs { get; protected set; }

        public List<List<object>[]> ReadFileJvnlpAsync(StreamReader fileMemoryStream, string fileName)
        {
            IExcelDataReader bookExcel = null;
            // register provide encoding 1252 to Excel
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            try
            {
                //Reading from a binary Excel file ('97-2003 format; *.xls)
                if (fileName.Split(".").LastOrDefault().Equals("xls"))
                    bookExcel = ExcelReaderFactory.CreateBinaryReader(fileMemoryStream.BaseStream);

                //Reading from a OpenXml Excel file (2007 format; *.xlsx)
                if (fileName.Split(".").LastOrDefault().Equals("xlsx"))
                    bookExcel = ExcelReaderFactory.CreateOpenXmlReader(fileMemoryStream.BaseStream);

                //DataSet - The result of each spreadsheet will be created in the result.Tables
                var tableJvnlp = bookExcel?.AsDataSet();
                CreateTablesJvnlp(tableJvnlp);

                if (Drugs == null || RemovedDrugs == null)
                    return new List<List<object>[]>();

                var drugs = Drugs.Rows.Cast<DataRow>().Select(x => x.ItemArray.ToList()).ToArray();
                var rmDrugs = RemovedDrugs.Rows.Cast<DataRow>().Select(x => x.ItemArray.ToList()).ToArray();
                RemoveNullColumns(drugs,rmDrugs);

                return new List<List<object>[]>
                {
                    drugs,
                    rmDrugs
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void CreateTablesJvnlp(DataSet dataJvnlp)
        {
            Drugs = dataJvnlp.Tables["Лист 1"];
            RemovedDrugs = dataJvnlp.Tables["Искл"];
        }

        private void RemoveNullColumns(List<object>[] drugs, List<object>[] rmDrugs)
        {
            try
            {
                for (var i = drugs[2].Count-1; i != 0; i--)
                {
                    var obj = drugs[2][i].ToString();
                    if (!obj.Equals("")) continue;
                    foreach (var drug in drugs)
                    {
                        drug.RemoveAt(i);
                    }
                }

                for (var i = rmDrugs[2].Count - 1; i != 0; i--)
                {
                    var obj = rmDrugs[2][i].ToString();
                    if (!obj.Equals("")) continue;
                    foreach (var rmDrug in rmDrugs)
                    {
                        rmDrug.RemoveAt(i);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}