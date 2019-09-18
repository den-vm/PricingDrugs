using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ServiceJob.Models
{
    public class UploadFile
    {
        public IQueryable<JvnlpModel> Drugs { get; protected set; }

        public void ReaderFileJvnlp(string filePath)
        {
            //Создаём приложение.
            Microsoft.Office.Interop.Excel.Application ObjExcel = new Microsoft.Office.Interop.Excel.Application();
            //Открываем книгу.                                                                                                                                                        
            Microsoft.Office.Interop.Excel.Workbook ObjWorkBook = ObjExcel.Workbooks.Open(filePath, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            //Выбираем таблицу(лист).
            Microsoft.Office.Interop.Excel.Worksheet ObjWorkSheet;
            ObjWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[1];
            string[] telephons = new string[100];
            //Выбираем первые сто записей из столбца.
            for (int i = 1; i < 101; i++)
            {
                //Выбираем область таблицы. (в нашем случае просто ячейку)
                Microsoft.Office.Interop.Excel.Range range = ObjWorkSheet.get_Range("F" + i.ToString(), "F" + i.ToString());
                //Добавляем полученный из ячейки текст.
                telephons[i - 1] = range.Text.ToString();

            }
        }
    }
}