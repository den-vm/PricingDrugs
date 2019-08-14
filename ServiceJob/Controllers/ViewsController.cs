using System.Diagnostics;
using System.Threading.Tasks;
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

        [HttpPost]
        public IActionResult DownloadFile()
        {
            return Content("dfgdfgdfdfg");
        }
        //public IActionResult DownloadFile(IFormCollection uploadedFile)
        //{
        //    var con = Request;
        //    if (uploadedFile.Files.Count == 1) // complite download file
        //    {
        //        if (uploadedFile.Files[0].Length > 25000000 &&
        //            uploadedFile.Files[0].ContentType.Equals("application/vnd.ms-excel")) // check byte and type file
        //        {

        //        }
        //        else
        //        {
        //            var errorFile =
        //                $"Файл {uploadedFile.Files[0].FileName} не является государственным реестром предельных отпускных цен из сайта grls.rosminzdrav.ru!";
        //            return Content(errorFile);
        //        }
        //    }
        //    return null;
        //}

        //public IActionResult About()
        //{
        //    ViewData["Message"] = "Your application description page.";

        //    return View();
        //}

        //public IActionResult Contact()
        //{
        //    ViewData["Message"] = "Your contact page.";

        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}