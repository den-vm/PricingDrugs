var visibleTableJvnlp = false;
var visibleFormTableDrugs = false;
var visibleFormDrugsPriceCriteria = false;
var visibleFormFile = false;
var fActive = "";

//Навигация по страницам таблицы
$(".viewlist li > input[type='button']").click("input",
    function(input) {
        var nametable = GetNameActiveTable();
        var namebutton = input["target"].name;
        var idlist = infotable[`${nametable}`]["value"];
        var inputfilters = [];
        $(`table[id='${nametable}'] > thead > tr > td`).find(":input[type='search']").each(
            function(i, input) {
                if (nametable === "exjvnlpTable" && i === 12 && $(input).val() !== "") {
                    var datetime = moment($(input).val()).format("DD.MM.YYYY");
                    inputfilters.push(datetime);
                } else
                    inputfilters.push($(input).val()); // записываем значение всех полей поиска
            });

        $.get("Jvnlp/Drugs/Navigate",
                {
                    nameTable: nametable,
                    nameButton: namebutton,
                    idList: idlist,
                    listFilter: JSON.stringify(inputfilters)
                })
            .always(function(data) {

                if (data !== null) {
                    $(`#${nametable} tbody`).html("");
                    GenerateTableJvnlpOriginal(data["navigateRowList"], `${nametable}`);

                    infotable[`${nametable}`]["value"] = data["idList"];
                    infotable[`${nametable}`]["text"] =
                        `${data["navigateListLength"]} из ${data["navigateListViewLength"]
                        } (Стр. ${infotable[`${nametable}`]["value"]})`;

                    UpdateInfotable(`${nametable}`);
                }
            });
    });

function GetNameActiveTable() {
    var jvnlp = $("div[name='jvnlp']").css("display");
    var exjvnlp = $("div[name='exjvnlp']").css("display");
    var newJvnlp = $("div[name='newJvnlp']").css("display");
    var includedJvnlp = $("div[name='includedJvnlp']").css("display");

    if (jvnlp === "block")
        return "tableDrugs";
    if (exjvnlp === "block")
        return "exjvnlpTable";
    if (newJvnlp === "block")
        return "tableDrugsNew";
    if (includedJvnlp === "block")
        return "tableDrugsIncluded";

}

$("button[name=buttonUploadFile],div[name='cl-btn']").click(function() {
    openFormFile();
});

$("button[name=buttonCalculate],div[name='cl-btn']").click(function() { // рассчёт цен
    $.get("Jvnlp/Drugs/Calculate")
        .always(function(data) {
            if (data.status === 500) {
                alertify.message(data.responseJSON["message"]);
                return;
            }
            alertify.message(data["message"]);
            $.ajax({
                type: "POST",
                url: "Jvnlp/Calculated",
                processData: false, // отключение преобразования строки запроса по contentType
                contentType:
                    false, // отключение преобразования контента в тип по умолчанию: "application/x-www-form-urlencoded;"
                statusCode: {
                    200: function(data) {
                        GenerateTableJvnlpToStart(data["calcJvnlp"]["drugs"], "tableDrugsNew");
                        GenerateTableJvnlpToStart(data["calcIncJvnlp"]["drugs"], "tableDrugsIncluded");

                        infotable["tableDrugsNew"]["value"] = 1;
                        infotable["tableDrugsNew"]["text"] =
                            `${data["calcJvnlp"]["drugsViewLength"]} из ${data["calcJvnlp"]["drugsLength"]} (Стр. ${
                            infotable[
                                "tableDrugsNew"]["value"]})`;

                        infotable["tableDrugsIncluded"]["value"] = 1;
                        infotable["tableDrugsIncluded"]["text"] =
                            `${data["calcIncJvnlp"]["drugsViewLength"]} из ${data["calcIncJvnlp"]["drugsLength"]
                            } (Стр. ${infotable[
                                "tableDrugsIncluded"]["value"]})`;

                        UpdateInfotable("tableDrugsNew");

                        AddEventFilteredOnTable("tableDrugsNew");
                        AddEventFilteredOnTable("tableDrugsIncluded");
                    },
                    500: function(data) {
                        alertify.error(data["message"]);
                    }
                }
            });
        });
});

$("button[name=buttonSaveToFileExcel],div[name='cl-btn']").click(
    function () { // сохранение рассчитанного реестра в файл Excel
        $("div[name='lockActionsDownload']").css("display", "block");
        $.ajax({
            type: "POST",
            url: "Jvnlp/SaveCalculated",
            processData: false, // отключение преобразования строки запроса по contentType
            contentType:
                false, // отключение преобразования контента в тип по умолчанию: "application/x-www-form-urlencoded;"
            dataType: 'binary',
            xhrFields: {
                'responseType': 'blob'
            },
            xhr: function () {
                var xhr = $.ajaxSettings.xhr(); // получаем объект XMLHttpRequest
                xhr.onprogress = function (event) {
                    if (event.lengthComputable) { // если известно количество байт
                        // высчитываем процент скачиваемого
                        var percentComplete = Math.ceil(event.loaded / event.total * 100);
                        // устанавливаем значение в атрибут value тега <progress>
                        // и это же значение альтернативным текстом для браузеров, не поддерживающих <progress>
                        $("#lockActionsDownloadCount").html(percentComplete + "%");
                        if (percentComplete === 100) {
                            $("#lockActionsDownloadCount").html("");
                            $("div[name='lockActionsDownload']").css("display", "none");
                        }
                    }
                };
                return xhr;
            },
            statusCode: {
                200: function (data, status, xhr) {
                    var link = document.createElement('a'),
                        filename = "";
                    if (xhr.getResponseHeader('Content-Disposition')){//имя файла
                        filename = xhr.getResponseHeader('Content-Disposition');
                         filename = filename.match(/filename="(.*?)"/)[1];
                        filename = decodeURIComponent(filename);
                        filename = filename.substring(filename.length-21);
                    }
                    link.href = URL.createObjectURL(data);
                    link.download = filename;
                    link.click();
                },
                500: function (data) {
                    alertify.error(data.responseJSON["message"]);
                }
            }
        });
    });

$("button[name=buttonSavePrevDate],div[name='cl-btn']").click(
    function () { // сохранение рассчитанного реестра в файл Excel
        $.ajax({
            type: "POST",
            url: "Jvnlp/SaveDateUpdate",
            processData: false, // отключение преобразования строки запроса по contentType
            contentType:
                false, // отключение преобразования контента в тип по умолчанию: "application/x-www-form-urlencoded;"
            statusCode: {
                200: function (data) {
                    alertify.message(data["message"]);
                },
                500: function (data) {
                    alertify.error(data.responseJSON["message"]);
                }
            }
        });
    });

$("li[name=openTableJvnlp]").click(function() {
    if (visibleFormTableDrugs) // закрыть форму наркотических препаратов
        FormTableDrugs(100);
    if (visibleFormDrugsPriceCriteria) // закрыть форму таблица цен
        FormCalculationCriteria(100);
    TableJvnlp(100);
    if ($("#tableDrugs").html() === "") {
        alertify.message("Загрузите реестр препаратов");
    }
});

$("li[name=openFormTableDrugs]").click(function() {
    if (visibleTableJvnlp) // закрыть таблицу препаратов
        TableJvnlp(100);
    if (visibleFormDrugsPriceCriteria) // закрыть форму таблица цен
        FormCalculationCriteria(100);
    FormTableDrugs(100); // открыть форму наркотических препаратов
});
$("li[name=openFormDrugsPriceCriteria]").click(function() {
    if (visibleTableJvnlp) // закрыть таблицу препаратов
        TableJvnlp(100);
    if (visibleFormTableDrugs) // закрыть форму наркотических препаратов
        FormTableDrugs(100);
    FormCalculationCriteria(100); // открыть форму таблица цен
});

//открытие таблицы исходного реестра ЛП
$("li[name=openTableOriginal]").click(function() {
    $("div[name='newJvnlp']").hide();
    $("div[name='includedJvnlp']").hide();
    $("div[name='ReadyJvnlp']").css("display", "none");
    $("li[name=openTableReady]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openIncludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    $("button[name='buttonCalculate']").css("display", "none");
    $("button[name='buttonSaveToFileExcel']").css("display", "none");
    $("button[name='buttonSavePrevDate']").css("display", "none");

    var activeTable = GetNameActiveTable();
    if (activeTable !== "tableDrugs" && activeTable !== "exjvnlpTable") {
        $("li[name=openTableOriginal]").css("background-color", "rgba(0, 222, 255, 0.29)");
        $("div[name='originalJvnlp']").css("display", "block");
        $("li[name=openOriginalReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
        $("li[name=openOriginalExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
        $("div[name='jvnlp']").show(1,
            function() {
                if ($("#tableDrugs").html() === "") {
                    alertify.message("Загрузите реестр препаратов");
                } else {
                    UpdateInfotable("tableDrugs");
                }
            });
    }
});

//открытие таблицы расcчитанного реестра ЛП
$("li[name=openTableReady]").click(function() {
    $("div[name='exjvnlp']").hide();
    $("div[name='jvnlp']").hide();
    $("div[name='originalJvnlp']").css("display", "none");
    $("li[name=openTableOriginal]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openOriginalReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openOriginalExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    $("button[name='buttonCalculate']").css("display", "block");
    $("button[name='buttonSaveToFileExcel']").css("display", "block");
    $("button[name='buttonSavePrevDate']").css("display", "block");

    var activeTable = GetNameActiveTable();
    if (activeTable !== "tableDrugsNew" && activeTable !== "tableDrugsIncluded") {
        $("li[name=openTableReady]").css("background-color", "rgba(0, 222, 255, 0.29)");
        $("div[name='ReadyJvnlp']").css("display", "block");
        $("li[name=openReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
        $("li[name=openIncludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
        $("li[name=openExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
        $("div[name='newJvnlp']").show(1,
            function() {
                if ($("#tableDrugs").html() === "") {
                    alertify.message("Загрузите реестр препаратов");
                } else {
                    UpdateInfotable("tableDrugsNew");
                }
            });
    }
});

//открытие таблицы ЖВНЛП
$("li[name=openOriginalReadyJvnlp]").click(function() {
    $("div[name='exjvnlp']").hide();
    $("li[name=openOriginalExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    $("li[name=openOriginalReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
    $("div[name='jvnlp']").show(1,
        function() {
            if ($("#tableDrugs").html() === "") {
                alertify.message("Загрузите реестр препаратов");
            } else {
                UpdateInfotable("tableDrugs");
            }
        });

});
//открытие таблицы Исключенные позиции
$("li[name=openOriginalExcludedJvnlp]").click(function() {
    $("div[name='jvnlp']").hide();
    $("li[name=openOriginalReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    $("li[name=openOriginalExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
    $("div[name='exjvnlp']").show(1,
        function() {
            if ($("#tableDrugs").html() === "") {
                alertify.message("Загрузите реестр препаратов");
            } else {
                UpdateInfotable("exjvnlpTable");
            }
        });
});

//открытие таблицы расчитанного ЖВНЛП
$("li[name=openReadyJvnlp]").click(function() {
    $("div[name='includedJvnlp']").hide();
    $("li[name=openIncludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    $("li[name=openReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
    $("div[name='newJvnlp']").show(1,
        function() {
            if ($("#tableDrugs").html() === "") {
                alertify.message("Загрузите реестр препаратов");
            } else {
                UpdateInfotable("tableDrugsNew");
            }
        });
});
//открытие таблицы расчитанных Включенных позиций
$("li[name=openIncludedJvnlp]").click(function() {
    $("div[name='newJvnlp']").hide();
    $("li[name=openReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    $("li[name=openIncludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
    $("div[name='includedJvnlp']").show(1,
        function() {
            if ($("#tableDrugs").html() === "") {
                alertify.message("Загрузите реестр препаратов");
            } else {
                UpdateInfotable("tableDrugsIncluded");
            }
        });
});
//открытие таблицы Исключенных позиций исходного реестра
$("li[name=openExcludedJvnlp]").click(function() {
    $("div[name='newJvnlp']").hide();
    $("div[name='includedJvnlp']").hide();
    $("li[name=openReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openIncludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    $("li[name=openExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
});


$("button[name=SaveCriteriaPrice]").click(function() {
    SaveCriteria();
});
$("button[name=LoadCriteriaPrice]").click(function() {
    LoadCriterias();
});

function openFormFile() {
    $("div[name=divJvnlp]").slideToggle({
        duration: 200,
        easing: "linear",
        always: function() {
            if ($("div[name=divJvnlp]").css("display") === "block") visibleFormFile = true;
            else visibleFormFile = false;
        }
    });
}

function TableJvnlp(speed = 200) {
    if (visibleFormFile === true)
        openFormFile();
    $("div[name=lockbody]").slideToggle({
        duration: (speed + 100),
        easing: "linear",
        always: function() {
            $("div[name=tableDrugs]").slideToggle({
                duration: speed,
                easing: "linear",
                always: function() {
                    if ($("div[name=tableDrugs]").css("display") === "block") {
                        visibleTableJvnlp = true;
                        $("div[name='jvnlp']").show(1);
                        $("li[name=openTableOriginal]").css("background-color", "rgba(0, 222, 255, 0.29)");
                        $("li[name=openOriginalReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
                    } else {
                        visibleTableJvnlp = false;
                        $("li[name=openTableOriginal]").css("background-color", "rgba(0, 222, 255, 0.29)");
                        $("li[name=openOriginalReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
                        $("div[name='originalJvnlp']").css("display", "block");
                        $("div[name='jvnlp']").css("display", "block");
                        $("div[name='exjvnlp']").css("display", "none");

                        $("li[name=openOriginalExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

                        $("li[name=openTableReady]").css("background-color", "rgba(0, 222, 255, 0)");
                        $("li[name=openReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
                        $("li[name=openIncludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

                        $("div[name='ReadyJvnlp']").css("display", "none");
                        $("div[name='newJvnlp']").css("display", "none");
                        $("div[name='includedJvnlp']").css("display", "none");

                    }
                }
            });
        }
    });
}

// получить список препаратов из списка при открытии формы
function FormTableDrugs(speed = 200) {
    $("div[name=lockbody]").slideToggle({
        duration: (speed + 100),
        easing: "linear",
        always: function() {
            $("div[name=divDrugsDownload]").slideToggle({
                duration: speed,
                easing: "linear",
                always: function() {
                    getDrugs();
                }
            });
        }
    });
}

// получить таблицу ценовых критериев на препараты
function FormCalculationCriteria(speed = 200) {
    $("div[name=lockbody]").slideToggle({
        duration: (speed + 100),
        easing: "linear",
        always: function() {
            $("div[name=divDrugsPriceCriteria]").slideToggle({
                duration: speed,
                easing: "linear",
                always: function() {
                    //получить коэфиценты для расчёта цен
                    if ($("div[name=divDrugsPriceCriteria]").css("display") === "block") {
                        visibleFormDrugsPriceCriteria = true;
                        LoadCriterias();
                    } else {
                        visibleFormDrugsPriceCriteria = false;
                    }
                }
            });
        }
    });
}

function getDrugs() {
    //получить список препаратов из сервера если форма открыта
    if ($("div[name=divDrugsDownload]").css("display") === "block") {
        visibleFormTableDrugs = true;
        var dataForm = new FormData();
        dataForm.append("narcoticDrugsView", null);
        RequestFormNPDrugs(dataForm);
    } else {
        visibleFormTableDrugs = false;
        $("#RowDrugs").html("");
    }
}

function RequestFormNPDrugs(dataForm) {
    $.ajax({
        url: "Jvnlp/DrugsExpensive",
        type: "POST",
        data: dataForm,
        dataType: "json",
        processData: false, // отключение преобразования строки запроса по contentType
        contentType:
            false, // отключение преобразования контента в тип по умолчанию: "application/x-www-form-urlencoded; charset=UTF-8"
        success: function(data) {
            $.each(data["listmessages"],
                function(key, message) {
                    var value = message.value;
                    if (value.typemessage === "error")
                        setTimeout(function() {
                                alertify.error(value["message"]);
                            },
                            key * 200);
                    if (value.typemessage === "complite")
                        setTimeout(function() {
                                alertify.message(value["message"]);
                            },
                            key * 200);
                    if (value.typemessage === "drugs") {
                        $("#RowDrugs").html(getDrugsHTML(value.message));
                    }
                });
            //Запускаем наблюдение за изменениями в HTML-элементе input JVNLP 
            var elementsDrugSave = $("tr[name=drugSave]")
                .find($("input[name = nameDrug],input[name = dataDrugAdd],input[name = dataDrugDel]"));
            elementsDrugSave.on("input",
                function() {
                    $(this).parents("td,tr").attr("name", "drugEdit");
                });
        }
    });
};