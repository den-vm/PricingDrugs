using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

//using Microsoft.Office.Interop.Excel;

namespace ServiceJob.Models
{
    public class UploadFile
    {
        public IQueryable<JvnlpModel> Drugs { get; protected set; }

        public void ReadFileJvnlp(string filePath)
        {
            HSSFWorkbook bookExcelXls = null;
            XSSFWorkbook bookExcelXlsx = null;
            ISheet sheet = null;
            string fileExt = Path.GetExtension(filePath);
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (fileExt == ".xls")
                    bookExcelXls = new HSSFWorkbook(file);
                else if (fileExt == ".xlsx")
                    bookExcelXlsx = new XSSFWorkbook(file);
                File.Delete(filePath);
            }
            if (bookExcelXls != null)
            {
                sheet = bookExcelXls.GetSheetAt(0);
            }
            else if (bookExcelXlsx != null)
            {
                sheet = bookExcelXlsx.GetSheetAt(0);
            }


            ////запускаем цикл по строкам
            //for (int row = 0; row <= sheet.LastRowNum; row++)
            //{
            //    //получаем строку
            //    var currentRow = sheet.GetRow(row);
            //    if (currentRow != null) //null когда строка содержит только пустые ячейки
            //    {
            //        //запускаем цикл по столбцам
            //        for (int column = 0; column < 3; column++)
            //        {
            //            //получаем значение яейки
            //            var stringCellValue = currentRow.GetCell(column).StringCellValue;
            //            //Выводим сообщение
            //            MessageBox.Show(string.Format("Ячейка {0}-{1} значение:{2}", row, column, stringCellValue));
            //        }
            //    }
            //}

            ////Создаём приложение.
            //var objExcel = new Application();
            ////Открываем книгу.                                                                                                                                                        
            //var objWorkBook = objExcel.Workbooks.Open(filePath, 0, false, 5, "", "", false, XlPlatform.xlWindows, "",
            //    true, false, 0, true, false, false);
            ////Выбираем таблицу(лист).
            //var objWorkSheet = (Worksheet) objWorkBook.Sheets[1];
            //var telephons = new string[100];
            ////Выбираем первые сто записей из столбца.
            //for (var i = 1; i < 101; i++)
            //{
            //    //Выбираем область таблицы. (в нашем случае просто ячейку)
            //    var range = objWorkSheet.Range["F" + i, "F" + i];
            //    //Добавляем полученный из ячейки текст.
            //    telephons[i - 1] = range.Text.ToString();
            //}
        }
    }
}