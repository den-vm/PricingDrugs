using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using ServiceJob.Classes;
using ServiceJob.Interface;
using ServiceJob.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ServiceJob.Controllers
{
    public class ViewsController : Controller
    {
        private const int VisibleLines = 253;

        static ViewsController()
        {
            AllTableJvnlp = new List<List<object>[]>();
            CalcDrugs = new List<List<object>[]>();
        }

        /// <summary>
        ///     исходный реестр препаратов
        /// </summary>
        private static List<List<object>[]> AllTableJvnlp { get; }

        /// <summary>
        ///     рассчитанный список препаратов
        /// </summary>
        private static List<List<object>[]> CalcDrugs { get; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private static string NewDateUpdate { get; set; }

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
        public async Task<IActionResult> UploadFile(IFormCollection form)
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
                var fileProcessing = new UploadFile();

                AllTableJvnlp.Clear();
                var responseRead = fileProcessing.ReadFileJvnlp(memoryStream, fileJvnlp.FileName);
                if (responseRead.Count == 0)
                    throw new Exception("Ошибка в чтении файла excel. Файл должен содержать листы 'Лист 1' и 'Искл'");
                AllTableJvnlp.Add(responseRead[(int) JvnlpLists.JVNLP]);
                AllTableJvnlp.Add(responseRead[(int) JvnlpLists.Excluded]);
                NewDateUpdate = fileProcessing.NewDateUpdate;

                var jsonOriginalDrugs =
                    JsonConvert.SerializeObject(responseRead[(int) JvnlpLists.JVNLP].Take(VisibleLines));
                var jsonExcludedDrugs =
                    JsonConvert.SerializeObject(responseRead[(int) JvnlpLists.Excluded].Take(VisibleLines));

                return new JsonResult(new
                    {
                        original = new
                        {
                            drugs = jsonOriginalDrugs,
                            drugsLength = responseRead[(int) JvnlpLists.JVNLP].Length - 3,
                            drugsViewLength = responseRead[(int) JvnlpLists.JVNLP].Take(VisibleLines).Count() - 3
                        },
                        excluded = new
                        {
                            drugs = jsonExcludedDrugs,
                            drugsLength = responseRead[(int) JvnlpLists.Excluded].Length - 3,
                            drugsViewLength = responseRead[(int) JvnlpLists.Excluded].Take(VisibleLines).Count() - 3
                        }
                    })
                    {StatusCode = 200};
            }
            catch (Exception e)
            {
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
            catch
            {
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
            catch
            {
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
            var filterList = new List<object>();
            try
            {
                List<List<object>[]> table;
                IControlRowsTable controlTable = new ControlTable();
                if (nameTable.Equals("tableDrugs") || nameTable.Equals("exjvnlpTable"))
                    table = AllTableJvnlp;
                else table = CalcDrugs;

                filterList = controlTable.FilterRows(nameTable, listFilter, table);
            }
            catch (Exception e)
            {
                return new JsonResult(new {message = e.Message})
                    {StatusCode = 500};
            }

            var jsonFilterList = JsonConvert.SerializeObject(filterList.Take(VisibleLines));
            return new JsonResult(new
                {
                    filterRowList = jsonFilterList,
                    filterListLength = filterList.Take(VisibleLines).Count() - 3,
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
        /// <param name="listFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Jvnlp/Drugs/Navigate")]
        public IActionResult GetRowsNavigate(string nameTable, string nameButton, int idList, string listFilter)
        {
            try
            {
                List<List<object>[]> table;
                IControlRowsTable controlTable = new ControlTable();
                if (nameTable.Equals("tableDrugs") || nameTable.Equals("exjvnlpTable"))
                    table = AllTableJvnlp;
                else table = CalcDrugs;

                var filterList = controlTable.FilterRows(nameTable, listFilter, table);
                var nextCount = idList * VisibleLines;
                switch (nameButton)
                {
                    case "startlist":
                        idList = 1;
                        nextCount = 0;
                        break;

                    case "nextlist":
                        if (nextCount > filterList.Count)
                            return new JsonResult(null);
                        idList++;
                        break;

                    case "prevlist":
                        if (nextCount <= VisibleLines)
                            return new JsonResult(null);

                        idList--;
                        nextCount = (idList - 1) * VisibleLines;
                        break;
                }

                var viewList = filterList.Skip(nextCount).Take(VisibleLines);

                var jsonFilterList = JsonConvert.SerializeObject(viewList);

                return new JsonResult(new
                    {
                        navigateRowList = jsonFilterList,
                        navigateListLength = VisibleLines * (idList - 1) + viewList.Count() - 3,
                        navigateListViewLength = filterList.Count - 3,
                        IdList = idList
                    })
                    {StatusCode = 200};
            }
            catch (Exception e)
            {
                return new JsonResult(new {message = e.Message})
                    {StatusCode = 500};
            }
        }

        /// <summary>
        ///     Расчёт цен на препараты ЖВНЛП
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Jvnlp/Drugs/Calculate")]
        public IActionResult CalculateDrugs()
        {
            if (AllTableJvnlp.Count == 0)
                return new JsonResult(new {message = "Список препаратов пуст"}) {StatusCode = 500};

            try
            {
                CalcDrugs.Clear();
                ICalculateDrugs calculate = new CalculateDrugs();
                calculate.Start(AllTableJvnlp[(int) JvnlpLists.JVNLP]);
                CalcDrugs.Add(calculate.JvnlpCalculated);
                CalcDrugs.Add(calculate.IncludeCalculated);
            }
            catch (Exception e)
            {
                return new JsonResult(new {message = e.Message}) {StatusCode = 500};
            }

            return new JsonResult(new {message = "Реестр ЖВНЛП рассчитан"}) {StatusCode = 200};
        }

        /// <summary>
        ///     Вывод рассчитанного реестра препаратов ЖВНЛП
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Jvnlp/Calculated")]
        public IActionResult GetCalculated()
        {
            var jsonCalcDrugs =
                JsonConvert.SerializeObject(CalcDrugs[(int) CalcJvnlp.CalcDrugs].Take(VisibleLines));
            var jsonIncCalcDrugs =
                JsonConvert.SerializeObject(CalcDrugs[(int) CalcJvnlp.CalcIncDrugs].Take(VisibleLines));
            return new JsonResult(new
                {
                    calcJvnlp = new
                    {
                        drugs = jsonCalcDrugs,
                        drugsLength = CalcDrugs[(int) CalcJvnlp.CalcDrugs].Length - 3,
                        drugsViewLength = CalcDrugs[(int) CalcJvnlp.CalcDrugs].Take(VisibleLines).Count() - 3
                    },
                    calcIncJvnlp = new
                    {
                        drugs = jsonIncCalcDrugs,
                        drugsLength = CalcDrugs[(int) CalcJvnlp.CalcIncDrugs].Length - 3,
                        drugsViewLength = CalcDrugs[(int) CalcJvnlp.CalcIncDrugs].Take(VisibleLines).Count() - 3
                    }
                })
                {StatusCode = 200};
        }

        [HttpPost]
        [Route("Jvnlp/SaveCalculated")]
        public IActionResult SaveCalculatedAsExcel()
        {
            var nameFile = "";

            if (CalcDrugs.Count == 0)
                return new JsonResult(new {message = "Для сохранения в файл необходимо выполнить рассчёт"})
                    {StatusCode = 500};

            var drugsJvnlp = new List<object[]>();
            var drugsIncJvnlp = new List<object[]>();
            var drugsExJvnlp = new List<object[]>();

            var notNumHeader = 0;
            foreach (var rowDrug in CalcDrugs[(int) CalcJvnlp.CalcDrugs])
            {
                if (notNumHeader < 3)
                {
                    notNumHeader++;
                    continue;
                }

                if (rowDrug[0].ToString().Equals(""))
                    rowDrug[0] = "-";
                drugsJvnlp.Add(rowDrug.ToArray());
            }

            notNumHeader = 0;
            foreach (var rowDrug in CalcDrugs[(int) CalcJvnlp.CalcIncDrugs])
            {
                if (notNumHeader < 3)
                {
                    notNumHeader++;
                    continue;
                }

                if (rowDrug[0].ToString().Equals(""))
                    rowDrug[0] = "-";
                drugsIncJvnlp.Add(rowDrug.ToArray());
            }

            notNumHeader = 0;
            foreach (var rowDrug in AllTableJvnlp[(int) JvnlpLists.Excluded])
            {
                if (notNumHeader < 3)
                {
                    notNumHeader++;
                    continue;
                }

                if (rowDrug[0].ToString().Equals(""))
                    rowDrug[0] = "-";
                drugsExJvnlp.Add(rowDrug.ToArray());
            }

            try
            {
                drugsJvnlp = drugsJvnlp.OrderBy(row => row[0]).ToList();
                drugsIncJvnlp = drugsIncJvnlp.OrderBy(row => row[0]).ToList();
                drugsExJvnlp = drugsExJvnlp.OrderBy(row => row[0]).ToList();

                var headerText = AllTableJvnlp[(int) JvnlpLists.JVNLP][0][0];
                var matchResult = Regex.Match((string) headerText,
                    @"(0[1-9]|[12][0-9]|3[01])[- .](0[1-9]|1[012])[- .](19|20)\d\d");
                var dateUpdateDrugs = matchResult.Value.Split('.');
                var savePath = Directory.GetCurrentDirectory() + "\\";
                nameFile = @$"{savePath}JVNLP_{dateUpdateDrugs[0]}_{dateUpdateDrugs[1]}_{dateUpdateDrugs[2]}.xlsx";

                // Is used lib EPPlus to save Excel

                using var newBookExcel =
                    new ExcelPackage(
                        new FileInfo(nameFile),
                        new FileInfo(@$"{Directory.GetCurrentDirectory()}\JVNLP_.xlsx"));
                var sheetJvnlp = newBookExcel.Workbook.Worksheets[1];
                sheetJvnlp.Cells["A1"].Value = headerText;
                sheetJvnlp.Cells["A4"].LoadFromArrays(drugsJvnlp.ToArray());

                var sheetIncJvnlp = newBookExcel.Workbook.Worksheets[2];
                sheetIncJvnlp.Cells["A1"].Value = headerText;
                sheetIncJvnlp.Cells["A4"].LoadFromArrays(drugsIncJvnlp.ToArray());

                var sheetExJvnlp = newBookExcel.Workbook.Worksheets[3];
                sheetExJvnlp.Cells["A1"].Value = headerText;
                sheetExJvnlp.Cells["A3"].LoadFromArrays(drugsExJvnlp.ToArray());

                const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.ContentType = contentType;
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

                var fileContentResult = new FileContentResult(newBookExcel.GetAsByteArray(), contentType)
                {
                    FileDownloadName = nameFile
                };
                return fileContentResult;
            }
            catch (Exception e)
            {
                return new JsonResult(new {message = e.Message}) {StatusCode = 500};
            }
        }


        [HttpPost]
        [Route("Jvnlp/SaveDateUpdate")]
        public async Task<IActionResult> SaveDateUpdate()
        {
            try
            {
                string controlMessage;
                if (NewDateUpdate != null)
                {
                    using var fs = new FileStream("lastdateupdate.json", FileMode.OpenOrCreate);
                    await JsonSerializer.SerializeAsync(fs, NewDateUpdate);
                    controlMessage = $"Дата обновления реестра '{NewDateUpdate}' сохранена в файле 'lastdateupdate.json'";
                }
                else controlMessage = "Загрузите реестр препаратов";
                return new JsonResult(new {message = controlMessage}) {StatusCode = 200};
            }
            catch (Exception e)
            {
                return new JsonResult(new {message = e.Message}) {StatusCode = 500};
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}