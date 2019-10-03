using System.Diagnostics;
using System.IO;
using System.Linq;
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
            if (form.Equals(null))
                return Json(new {typemessage = "error", message = "Ошибка обработки формы!"});

            if (form.Files.Count > 0)
            {
                var fileJvnlp = form.Files[0];
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
                    return Json(new {typemessage = "complite", message = "Успешно загружен и обработан"});
                }
                return Json(new
                {
                    typemessage = "error",
                    message =
                    $"Файл '{fileJvnlp.FileName}' не является государственным реестром предельных отпускных цен из сайта grls.rosminzdrav.ru!"
                });
            }
            if (form.Keys.Count > 0)
            {
                var keyForm = form.ToDictionary(x => x.Key, x => x.Value).ToList()[0];
                switch (keyForm.Key)
                {
                    case "narcoticDrugs":
                        // модель данных
                        return Json(new
                        {
                            typemessage = "complite",
                            message = "Таблица наркотических препаратов сохранена!"
                        });
                        break;
                }
            }
            return Json(new {typemessage = "error", message = "Проверьте заполнение формы!"});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}