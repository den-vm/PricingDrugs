using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceJob.Models;

namespace ServiceJob.Controllers
{
    public class ViewsController : Controller
    {
        private readonly UploadFileModel _fileProcessing = new UploadFileModel();

        [Route("/Jvnlp")]
        public IActionResult Jvnlp()
        {
            return View();
        }

        [Route("/Jvnlp")]
        [HttpPost]
        public IActionResult DownloadFile(IFormFile fileJvnlp)
        {
            object message = null;
            if (fileJvnlp != null) // complite download file
                if (fileJvnlp.Length > 25000000 &&
                    fileJvnlp.ContentType.Equals("application/vnd.ms-excel")) // check byte and type file
                {
                    _fileProcessing.ReaderFileJvnlp(fileJvnlp);
                    message = new {typemessage = "complite", message = "Успешно загружен и обработан"};
                }
                else
                {
                    message = new
                    {
                        typemessage = "error",
                        message =
                        $"Файл '{fileJvnlp.FileName}' не является государственным реестром предельных отпускных цен из сайта grls.rosminzdrav.ru!"
                    };
                }
            return Json(message);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}