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

            //Reading from a binary Excel file ('97-2003 format; *.xls)
            if (fileName.Contains(".xls"))
                bookExcel = ExcelReaderFactory.CreateBinaryReader(fileMemoryStream.BaseStream);

            //Reading from a OpenXml Excel file (2007 format; *.xlsx)
            if (fileName.Contains(".xlsx"))
                bookExcel = ExcelReaderFactory.CreateOpenXmlReader(fileMemoryStream.BaseStream);

            //DataSet - The result of each spreadsheet will be created in the result.Tables
            var tableJvnlp = bookExcel?.AsDataSet();
            CreateTableJvnlp(tableJvnlp);

            var drugs = Drugs.Rows.Cast<DataRow>().Select(x => x.ItemArray).ToArray();
            var rmDrugs = RemovedDrugs.Rows.Cast<DataRow>().Select(x => x.ItemArray).ToArray();

            var excelList = new List<object[]>
            {
                drugs,
                rmDrugs
            };

            return excelList;
        }

        private void CreateTableJvnlp(DataSet dataJvnlp)
        {
            Drugs = dataJvnlp.Tables[0];
            RemovedDrugs = dataJvnlp.Tables[1];
        }
    }

    public class SaveNarcoticDrugs
    {
    }
}