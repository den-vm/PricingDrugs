using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceJob.Models;

namespace ServiceJob.Controllers
{
    public class ViewsController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly UploadFile _fileProcessing = new UploadFile();

        public ViewsController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        [Route("/Jvnlp")]
        public IActionResult Jvnlp()
        {
            return View();
        }

        [HttpPost("Jvnlp")]
        public async Task<IActionResult> RequestJvnlp(IFormCollection form)
        {
            var a = form.Keys;
            var fileJvnlp = Request?.Form.Files[0];
            object message = null;
            if (fileJvnlp != null) // complite download file
                if (fileJvnlp.Length > 25000000 &&
                    fileJvnlp.ContentType.Equals("application/vnd.ms-excel")) // check byte and type file
                {
                    // create path temp file
                    var path = @"\tempupload\" + fileJvnlp.FileName;
                    // save temp faile to path catalog wwwroot
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await fileJvnlp.CopyToAsync(fileStream);
                    }
                    _fileProcessing.ReadFileJvnlp(_appEnvironment.WebRootPath + path);
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


        public async Task<IActionResult> DownloadNarcoticDrugs()
        {
            var res = Request;
            object message;
            if (res != null) // complite save drugs

                message = new
                {
                    typemessage = "complite",
                    message = "Таблица наркотических препаратов сохранена!"
                };

            else
                message = new
                {
                    typemessage = "error",
                    message = "Проверьте заполнение таблицы наркотических препаратов!"
                };
            return Json(message);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}