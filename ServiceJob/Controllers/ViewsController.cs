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
        /// <summary>
        ///     исходный реестр препаратов
        /// </summary>
        private static readonly List<List<object>[]> OriginalTableJvnlp = new List<List<object>[]>();

        //private readonly IHostingEnvironment _appEnvironment;
        private readonly UploadFile _fileProcessing = new UploadFile();

        [Route("/Jvnlp")]
        public IActionResult Jvnlp()
        {
            return View();
        }

        /// <summary>
        ///     Загрузка реестра препаратов
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Jvnlp/upload")]
        public IActionResult UploadFile(IFormCollection form)
        {
            try
            {
                if (form.Equals(null))
                    throw new Exception("Ошибка в обработке данных с формы");

                // Get file Jvnlp
                var fileJvnlp = form.Files.FirstOrDefault();

                // check byte and type file
                if (fileJvnlp == null)
                    throw new Exception("Файл не загружен");
                if (!fileJvnlp.ContentType.Equals("application/vnd.ms-excel") &&
                    !fileJvnlp.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                    throw new Exception("Неверный тип файла");

                using var memoryStream = new StreamReader(fileJvnlp.OpenReadStream());

                var responseRead = _fileProcessing.ReadFileJvnlpAsync(memoryStream, fileJvnlp.FileName);

                if (responseRead.Count == 0)
                    throw new Exception("Ошибка в чтении файла excel. Файл должен содержать листы 'Лист 1' и 'Искл'");

                OriginalTableJvnlp.Clear();
                OriginalTableJvnlp.Add(responseRead[(int) JvnlpLists.JVNLP]);
                OriginalTableJvnlp.Add(responseRead[(int) JvnlpLists.Excluded]);

                var jsonOriginalDrugs = JsonConvert.SerializeObject(responseRead[(int) JvnlpLists.JVNLP].Take(250));
                var jsonExcludedDrugs = JsonConvert.SerializeObject(responseRead[(int) JvnlpLists.Excluded].Take(250));

                return new JsonResult(new
                    {
                        original = new
                        {
                            drugs = jsonOriginalDrugs,
                            drugsLength = responseRead[(int) JvnlpLists.JVNLP].Length - 3,
                            drugsViewLength = responseRead[(int) JvnlpLists.JVNLP].Take(250).Count() - 3
                        },
                        excluded = new
                        {
                            drugs = jsonExcludedDrugs,
                            drugsLength = responseRead[(int) JvnlpLists.Excluded].Length - 3,
                            drugsViewLength = responseRead[(int) JvnlpLists.Excluded].Take(250).Count() - 3
                        }
                    })
                    {StatusCode = 200};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new JsonResult(new
                    {
                        message = e.Message
                    })
                    {StatusCode = 500};
            }
        }

        /// <summary>
        ///     Управление списком наркотических препаратов
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
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

        /// <summary>
        ///     Загрузка критерий для расчёта наркотических препаратов
        /// </summary>
        /// <param name="formdata"></param>
        /// <returns></returns>
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

        /// <summary>
        ///     Скачивание критерий для расчёта наркотических препаратов
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        ///     Фильтрация таблицы
        /// </summary>
        /// <param name="nameTable"></param>
        /// <param name="listFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Jvnlp/Drugs/Filtered")]
        public IActionResult GetRowsFiltered(string nameTable, string listFilter)
        {
            List<object> filterList;
            var filtresCast = JsonConvert.DeserializeObject<string[]>(listFilter);
            var emptyFilter = filtresCast.All(textFilter => textFilter == "");
            try
            {
                object[] activeTable = null;
                if (nameTable.Equals("tableDrugs"))
                    activeTable = OriginalTableJvnlp[(int) JvnlpLists.JVNLP];
                if (nameTable.Equals("exjvnlpTable"))
                    activeTable = OriginalTableJvnlp[(int) JvnlpLists.Excluded];

                if (emptyFilter) // если текст фильтра пуст то возращаем всю исходную таблицу 
                {
                    filterList = (activeTable ?? throw new InvalidOperationException("Реестр таблиц пуст")).ToList();
                }
                else
                {
                    if (activeTable == null)
                        throw new InvalidOperationException("Реестр таблиц пуст");

                    var tempItems = activeTable;
                    for (var i = 0; i < filtresCast.Length; i++)
                    {
                        var numColumn = i;
                        tempItems = tempItems.WhereIf(!filtresCast[i].Equals(""),
                            (item, j) =>
                            {
                                if (j <= 2)
                                    return true;
                                var itemArray = (List<object>) item;
                                var strItemArray = itemArray[numColumn].GetType().BaseType.Name.Equals("ValueType")
                                    ? itemArray[numColumn].ToString().Replace(",", ".")
                                    : itemArray[numColumn].ToString();
                                var isContains = strItemArray.ToUpper().Contains(filtresCast[numColumn].ToUpper());
                                return isContains;
                            }).ToArray();
                    }

                    filterList = tempItems.ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new JsonResult(new {message = e.Message})
                    {StatusCode = 500};
            }

            var jsonFilterList = JsonConvert.SerializeObject(filterList.Take(250));
            return new JsonResult(new
                {
                    filterRowList = jsonFilterList,
                    filterListLength = filterList.Take(250).Count() - 3,
                    filterListViewLength = filterList.Count - 3
                })
                {StatusCode = 200};
        }

        /// <summary>
        ///     Навигация по таблице
        /// </summary>
        /// <param name="nameTable"></param>
        /// <param name="nameButton"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Jvnlp/Drugs/Navigate")]
        public IActionResult GetRowsNavigate(string nameTable, string nameButton, int idList, string listFilter)
        {
            var filtresCast = JsonConvert.DeserializeObject<string[]>(listFilter);
            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}