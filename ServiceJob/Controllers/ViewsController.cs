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
                            var listdrugs = processingNDrugs.GetDrugs();
                            if (listdrugs != null)
                            {
                                if (listdrugs.Count == 0)
                                {
                                    newmessages.Add(Json(new
                                    {
                                        typemessage = "complite",
                                        message = "Список наркотических препарат пуст"
                                    }));
                                    break;
                                }
                                newmessages.Add(Json(new
                                {
                                    typemessage = "drugs",
                                    message = listdrugs
                                }));
                            }
                            else
                            {
                                newmessages.Add(Json(new
                                {
                                    typemessage = "error",
                                    message =
                                    "Ошибка в чтении файла 'NDrugsReestr.xml' наркотических препаратов. Параметры 'ID', 'Name', 'IncludeDate' обязательны для чтения."
                                }));
                            }
                            break;

                        case "narcoticDrugsAdd":
                            var newKey = processingNDrugs.GetNewKey();
                            if (newKey == -1)
                            {
                                newmessages.Add(Json(new
                                {
                                    typemessage = "error",
                                    message = "Ошибка чтении файла 'NDrugsReestr.xml' наркотических препаратов"
                                }));
                                break;
                            }
                            var newarrayDrugs = keyform.Value.ToString().Split(',').SplitArray(3);
                            var newlistDrugs =
                                newarrayDrugs.Select(x =>
                                {
                                    var enumX = x.ToList();
                                    return new DrugNarcoticsModel
                                    {
                                        Id = newKey++,
                                        NameDrug = enumX[0],
                                        IncludeDate = enumX[1].ToDateTime(),
                                        OutDate = enumX[2].ToDateTime()
                                    };
                                }).ToList();


                            var resultAdd = processingNDrugs.Add(newlistDrugs);
                            if (!resultAdd)
                            {
                                newmessages.Add(Json(new
                                {
                                    typemessage = "error",
                                    message = "Ошибка при сохранении наркотических препаратов"
                                }));
                                break;
                            }
                            newmessages.Add(Json(new
                            {
                                typemessage = "complite",
                                message = "Новые препараты успешно внесены в список"
                            }));
                            break;

                        case "narcoticDrugsEdit":
                            var editarrayDrugs = keyform.Value.ToString().Split(',').SplitArray(4);
                            var editlistDrugs =
                                editarrayDrugs.Select(x =>
                                {
                                    var enumX = x.ToList();
                                    return new DrugNarcoticsModel
                                    {
                                        Id = int.Parse(enumX[0]),
                                        NameDrug = enumX[1],
                                        IncludeDate = enumX[2].ToDateTime(),
                                        OutDate = enumX[3].ToDateTime()
                                    };
                                }).ToList();
                            var resultEdit = processingNDrugs.Edit(editlistDrugs);
                            if (!resultEdit)
                            {
                                newmessages.Add(Json(new
                                {
                                    typemessage = "error",
                                    message = "Ошибка при изменении существующих препаратов"
                                }));
                                break;
                            }
                            newmessages.Add(Json(new
                            {
                                typemessage = "complite",
                                message = "Препараты успешно изменены"
                            }));
                            break;

                        default:
                            newmessages.Add(Json(new
                            {
                                typemessage = "error",
                                message = "Ошибка чтения несущестующего параметра формы"
                            }));
                            break;
                    }
                if (newmessages.Count > 0)
                    return Json(new {listmessages = newmessages});
            }
            else
            {
                newmessages.Add(Json(new
                {
                    typemessage = "error",
                    message = "Ошибка чтения данных формы"
                }));
            }
            return Json(new {listmessages = newmessages});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}