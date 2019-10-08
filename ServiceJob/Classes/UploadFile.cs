using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;

namespace ServiceJob.Models
{
    public class UploadFile
    {
        public DataTable Drugs { get; protected set; }
        public DataTable RemovedDrugs { get; protected set; }

        public object ReadFileJvnlp(string filePath)
        {
            IExcelDataReader bookExcel = null;
            DataSet tableJvnlp;
            var fileExt = Path.GetExtension(filePath);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // register provide encoding 1252 to Excel
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                switch (fileExt)
                {
                    case ".xls":
                        bookExcel = ExcelReaderFactory
                            .CreateBinaryReader(stream); //Reading from a binary Excel file ('97-2003 format; *.xls)
                        break;
                    case ".xlsx":
                        bookExcel = ExcelReaderFactory
                            .CreateOpenXmlReader(stream); //Reading from a OpenXml Excel file (2007 format; *.xlsx)
                        break;
                }
                tableJvnlp =
                    bookExcel
                        ?.AsDataSet(); //DataSet - The result of each spreadsheet will be created in the result.Tables
            }
            File.Delete(filePath);
            CreateTableJvnlp(tableJvnlp);
            return new { typemessage = "complite", message = "Успешно загружен и обработан" };
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