using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;

namespace ServiceJob.Models
{
    public class UploadFile
    {
        public DataTable Drugs { get; protected set; }
        public DataTable RemovedDrugs { get; protected set; }

        public List<object[]> ReadFileJvnlpAsync(StreamReader fileMemoryStream, string fileName)
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
                CreateTableJvnlp(tableJvnlp);

                if (Drugs == null || RemovedDrugs == null)
                    return new List<object[]>();

                var drugs = Drugs.Rows.Cast<DataRow>().Select(x => x.ItemArray).ToArray();
                var rmDrugs = RemovedDrugs.Rows.Cast<DataRow>().Select(x => x.ItemArray).ToArray();
                return new List<object[]>
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

        private void CreateTableJvnlp(DataSet dataJvnlp)
        {
            Drugs = dataJvnlp.Tables["Лист 1"];
            RemovedDrugs = dataJvnlp.Tables["Искл"];
        }
    }

    public class SaveNarcoticDrugs
    {
    }
}