using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ServiceJob.Models;

namespace ServiceJob.Controllers
{
    public class ViewsController : Controller
    {
        private const string Path = "Pages";

        [Route("/Jvnlp")]
        public IActionResult Jvnlp()
        {
            return View($"~/{Path}/Jvnlp.cshtml");
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