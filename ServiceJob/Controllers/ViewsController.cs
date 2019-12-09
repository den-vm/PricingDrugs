using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceJob.Classes;
using ServiceJob.Models;

namespace ServiceJob.Controllers
{
    public class ViewsController : Controller
    {
        //private readonly IHostingEnvironment _appEnvironment;
        private readonly UploadFile _fileProcessing = new UploadFile();

        //public ViewsController(IHostingEnvironment appEnvironment)
        //{
        //    _appEnvironment = appEnvironment;
        //}

        [Route("/Jvnlp")]
        public IActionResult Jvnlp()
        {
            return View();
        }

        [HttpPost("Jvnlp")]
        public async Task<IActionResult> RequestJvnlp(IFormCollection form)
        {
            var processingNDrugs = new ProcessingNDrugs();
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
                    var path = "/TempUploadsFile/" + fileJvnlp.FileName;
                    // save temp faile to path catalog wwwroot
                    using (var fileStream = new FileStream(Directory.GetCurrentDirectory() + path, FileMode.Create))
                    {
                        await fileJvnlp.CopyToAsync(fileStream);
                    }
                    var responseRead = _fileProcessing.ReadFileJvnlp(Directory.GetCurrentDirectory() + path);
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
            var newmessages = new List<JsonResult>();
            if (form.Keys.Count > 0)
            {
                var keysform = form.ToDictionary(x => x.Key, x => x.Value).ToList();
                foreach (var keyform in keysform)
                    switch (keyform.Key)
                    {
                        case "narcoticDrugsView":
                            var listdrugs = processingNDrugs.Get();
                            if (listdrugs.Count == 0)
                                newmessages.Add(Json(new
                                {
                                    typemessage = "complite",
                                    message = "Таблица наркотических препаратов пуста"
                                }));
                            break;

                        case "narcoticDrugsAdd":
                            var newlistDrugs = keyform.Value;
                            var resultAdd = processingNDrugs.Add(new List<DrugNarcoticsModel>());
                            if (!resultAdd)
                                newmessages.Add(Json(new
                                {
                                    typemessage = "error",
                                    message = "Ошибка при сохранении наркотических препаратов"
                                }));
                            newmessages.Add(Json(new
                            {
                                typemessage = "complite",
                                message = "Новые позиции успешно внесены в список"
                            }));
                            break;

                        case "narcoticDrugsEdit":
                            var editlistDrugs = keyform.Value;
                            var resultEdit = processingNDrugs.Edit(new List<DrugNarcoticsModel>());
                            if (!resultEdit)
                                newmessages.Add(Json(new
                                {
                                    typemessage = "error",
                                    message = "Ошибка при изменении существующих позиций"
                                }));
                            newmessages.Add(Json(new
                            {
                                typemessage = "complite",
                                message = "Имеющие позиции успешно изменены"
                            }));
                            break;

                        default:
                            newmessages.Add(Json(new
                            {
                                typemessage = "error",
                                message = "Ошибка чтения запроса от формы наркотических препаратов"
                            }));
                            break;
                    }
                if (newmessages.Count > 0)
                    return Json(new {listmessages = newmessages});
            }
            newmessages.Add(Json(new
            {
                typemessage = "error",
                message = "Проверьте заполнение формы"
            }));
            return Json(new {listmessages = newmessages});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}