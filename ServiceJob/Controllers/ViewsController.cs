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
                return Json(new {typemessage = "error", message = "Ошибка обработки формы"});
            
            // Form file Jvnlp
            if (form.Files.Count == 1)
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
                    var responseRead = _fileProcessing.ReadFileJvnlp(_appEnvironment.WebRootPath + path);
                    return Json(responseRead);
                }
                return Json(new
                {
                    typemessage = "error",
                    message =
                    $"Файл '{fileJvnlp.FileName}' не является государственным реестром предельных отпускных цен из сайта grls.rosminzdrav.ru"
                });
            }
            
            // Form NDrugs
            if (form.Keys.Count > 0)
            {
                var keyForm = form.ToDictionary(x => x.Key, x => x.Value).ToList()[0];
                switch (keyForm.Key)
                {
                    case "narcoticDrugsView":
                        var a = new DrugNarcoticsModel().ReadFileDrugs();

                        goto case "MessageSaveDrugs";

                    case "narcoticDrugsAdd":
                        var b = new DrugNarcoticsModel().ReadFileDrugs();

                        goto case "MessageSaveDrugs";

                    case "narcoticDrugsDel":
                        var c = new DrugNarcoticsModel().ReadFileDrugs();

                        goto case "MessageSaveDrugs";

                    case "narcoticDrugsEdit":
                        var d = new DrugNarcoticsModel().ReadFileDrugs();

                        goto case "MessageSaveDrugs";

                    case "MessageSaveDrugs":
                        return Json(new
                        {
                            typemessage = "complite",
                            message = "Таблица препаратов сохранена"
                        });

                    default:
                        return Json(new
                        {
                            typemessage = "error",
                            message = "Ошибка сохранения таблицы препаратов"
                        });
                }
            }
            return Json(new {typemessage = "error", message = "Проверьте заполнение формы"});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}