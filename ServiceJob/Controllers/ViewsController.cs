using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceJob.Models;

namespace ServiceJob.Controllers
{
    public class ViewsController : Controller
    {
        [Route("/Jvnlp")]
        public IActionResult Jvnlp()
        {
            return View();
        }

        [Route("/Jvnlp")]
        [HttpPost]
        public IActionResult DownloadFile(IFormCollection uploadedFile)
        {
            var message = "";
            if (uploadedFile.Files.Count == 1) // complite download file
                if (uploadedFile.Files[0].Length > 25000000 &&
                    uploadedFile.Files[0].ContentType.Equals("application/vnd.ms-excel")) // check byte and type file
                {
                }
                else
                {
                    message =
                        $"Файл '{uploadedFile.Files[0].FileName}' не является государственным реестром предельных отпускных цен из сайта grls.rosminzdrav.ru!";
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