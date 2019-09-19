using System.Linq;
using Microsoft.Office.Interop.Excel;

namespace ServiceJob.Models
{
    public class UploadFile
    {
        public IQueryable<JvnlpModel> Drugs { get; protected set; }

        public void ReaderFileJvnlp(string filePath)
        {
            //Создаём приложение.
            var objExcel = new Application();
            //Открываем книгу.                                                                                                                                                        
            var objWorkBook = objExcel.Workbooks.Open(filePath, 0, false, 5, "", "", false, XlPlatform.xlWindows, "",
                true, false, 0, true, false, false);
            //Выбираем таблицу(лист).
            var objWorkSheet = (Worksheet) objWorkBook.Sheets[1];
            var telephons = new string[100];
            //Выбираем первые сто записей из столбца.
            for (var i = 1; i < 101; i++)
            {
                //Выбираем область таблицы. (в нашем случае просто ячейку)
                var range = objWorkSheet.Range["F" + i, "F" + i];
                //Добавляем полученный из ячейки текст.
                telephons[i - 1] = range.Text.ToString();
            }
        }
    }
}