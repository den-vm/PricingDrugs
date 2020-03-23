using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        [HttpPost]
        [Route("Jvnlp/upload")]
        public async Task<IActionResult> UploadFile(IFormCollection form)
        {
            if (form.Equals(null))
                return new JsonResult(new {typemessage = "error", message = "Ошибка обработки формы"})
                    {StatusCode = 500};

            // Form file Jvnlp
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
                return new JsonResult(responseRead) {StatusCode = 200};
            }

            return new JsonResult(new
            {
                typemessage = "error",
                message =
                    $"Файл '{fileJvnlp.FileName}' не является государственным реестром предельных отпускных цен из сайта grls.rosminzdrav.ru"
            }) {StatusCode = 500};
        }

        [HttpPost]
        [Route("Jvnlp/DrugsExpensive")]
        public async Task<IActionResult> DrugsExpensive(IFormCollection form)
        {
            var processingNDrugs = new ProcessingNDrugs();
            var messages = new List<JsonResult>();
            var keysform = form.ToDictionary(x => x.Key, x => x.Value).ToList();
            IEnumerable<IEnumerable<string>> splitDrugs;
            List<DrugNarcoticsModel> listDrugs;

            foreach (var information in keysform)
                switch (information.Key)
                {
                    case "narcoticDrugsView":
                        var drugs = await Task.Run(() => processingNDrugs.GetDrugs());
                        if (drugs != null)
                        {
                            if (drugs.Count == 0)
                                messages.Add(new JsonResult(new
                                    {
                                        typemessage = "complite",
                                        message = "Список наркотических препарат пуст"
                                    })
                                    {StatusCode = 200});

                            messages.Add(new JsonResult(new
                                {
                                    typemessage = "drugs",
                                    message = drugs
                                })
                                {StatusCode = 200});
                        }

                        break;

                    case "narcoticDrugsAdd":
                        var newKey = await Task.Run(() => processingNDrugs.GetNewKey());
                        if (newKey == -1)
                            messages.Add(new JsonResult(new
                                {
                                    typemessage = "error",
                                    message = "Ошибка чтении файла 'NDrugsReestr.xml' наркотических препаратов"
                                })
                                {StatusCode = 500});

                        splitDrugs = information.Value.ToString().Split(',').SplitArray(3);
                        listDrugs =
                            splitDrugs.Select(x =>
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


                        var resultAdd = await Task.Run(() => processingNDrugs.Add(listDrugs));
                        if (!resultAdd)
                            messages.Add(new JsonResult(new
                                {
                                    typemessage = "error",
                                    message = "Ошибка при сохранении наркотических препаратов"
                                })
                                {StatusCode = 500});

                        messages.Add(new JsonResult(new
                            {
                                typemessage = "complite",
                                message = "Новые препараты успешно внесены в список"
                            })
                            {StatusCode = 200});
                        break;

                    case "narcoticDrugsEdit":
                        splitDrugs = information.Value.ToString().Split(',').SplitArray(4);
                        listDrugs =
                            splitDrugs.Select(x =>
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

                        var resultEdit = await Task.Run(() => processingNDrugs.Edit(listDrugs));
                        if (!resultEdit)
                        {
                            messages.Add(new JsonResult(new
                                {
                                    typemessage = "error",
                                    message = "Ошибка при изменении препаратов"
                                })
                                {StatusCode = 500});
                            break;
                        }

                        messages.Add(new JsonResult(new
                        {
                            typemessage = "complite",
                            message = "Препараты успешно изменены"
                        }) {StatusCode = 200});
                        break;

                    default:
                        messages.Add(new JsonResult(new
                        {
                            typemessage = "error",
                            message = "Ошибка чтения несущестующего параметра формы"
                        }) {StatusCode = 500});
                        break;
                }

            return new JsonResult(new {listmessages = messages});
        }

        [HttpPost]
        [Route("Jvnlp/PriceCriteria/upload")]
        public async Task<IActionResult> SavePriceCriteria(IFormCollection formdata)
        {
            try
            {
                var priceCriteriaJson = formdata.ToDictionary(x => x.Key, x => x.Value)
                    .Where(element => element.Key.Equals("priceCriteria")).Select(key => key.Value).ToList()[0];
                var criterias = JsonConvert.DeserializeObject<ListCriteriasModels>(priceCriteriaJson);
                var priceCriteria = new PriceCriteria<ListCriteriasModels>();
                await priceCriteria.SavedAsync(criterias);
                return new JsonResult(new
                    {
                        message = "Сохранено"
                    })
                    {StatusCode = 200};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new JsonResult(new
                {
                    message = "Ошибка сохранения"
                })
                {StatusCode = 500};
        }

        [HttpPost]
        [Route("Jvnlp/PriceCriteria")]
        public async Task<IActionResult> LoadPriceCriteria()
        {
            try
            {
                var loadedCriteria = await new PriceCriteria<ListCriteriasModels>().LoadAsync();
                var serializeCriterias = JsonConvert.SerializeObject(loadedCriteria);
                return new JsonResult(new
                    {
                        message = serializeCriterias
                    })
                    {StatusCode = 200};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new JsonResult(new
                {
                    message = "Ошибка загрузки"
                })
                {StatusCode = 500};
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}