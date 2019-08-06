using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceJob.Models;

namespace ServiceJob.Controllers
{
    public class ViewsController : Controller
    {
        [Route("/Jvnlp")]
        public IActionResult Jvnlp(IFormCollection uploadedFile)
        {
            ViewBag.ErrorMassageFile =
                $"Файл  не является государственным реестром предельных отпускных цен из сайта grls.rosminzdrav.ru!";
            if (uploadedFile.Files.Count == 1) // complite download file
                if (uploadedFile.Files[0].Length > 25000000 &&
                    uploadedFile.Files[0].ContentType.Equals("application/vnd.ms-excel")) // check byte and type file
                {

                }
                else
                {
                    ViewBag.ErrorMassageFile =
                        $"Файл {uploadedFile.Files[0].Name} не является государственным реестром предельных отпускных цен из сайта grls.rosminzdrav.ru!";
                }
            return View();
        }

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