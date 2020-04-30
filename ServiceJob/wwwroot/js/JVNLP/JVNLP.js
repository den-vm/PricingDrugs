var visibleTableJvnlp = false;
var visibleFormTableDrugs = false;
var visibleFormDrugsPriceCriteria = false;
var visibleFormFile = false;
var fActive = "";

$(window).on("resize",
    function() {
        UpdateStyleTableDrugs("tableDrugs");
        UpdateStyleTableDrugs("exjvnlpTable");
        UpdateStyleTableDrugs("tableDrugsNew");
        UpdateStyleTableDrugs("tableDrugsIncluded");
        UpdateStyleTableDrugs("tableDrugsExcluded", true);
    });

///Навигация
$(".viewlist li > input[type='button']").click("input",
    function(input) {
        var nametable = GetActiveTable();
        var namebutton = input["target"].name;
        var idlist = $(".viewlist li > label[name='infotable']").data("idlist");
        alert(`${nametable}` + ` ${namebutton}` + ` ${idlist}`);

        $.get('Jvnlp/Drugs/Navigate', { nameTable: nametable, nameButton: namebutton, idList: idlist })
            .always(function (data) {
            alert("finished");
        });
    });

///Фильтрация
//$("div[name='ReadyJvnlp'],div[name='originalJvnlp'] div > table > thead > tr > td > input[type='search']").on("input",
//    function(input) {
//        var nametable = GetActiveTable();
//        var idFilter = input["target"].name.replace("filterTable", "");
//        var textFilter = input["target"].value;
//        //alert(`${nametable}` + ` ${idFilter}` + ` ${textFilter}`);

//        $.get('Jvnlp/Drugs/Filtered', { nameTable: nametable, idColumn: idFilter, text: textFilter })
//            .always(function (data) {
//                alert("finished");
//            });
//    });

function GetActiveTable() {
    var jvnlp = $("div[name='jvnlp']").css("display");
    var exjvnlp = $("div[name='exjvnlp']").css("display");
    var newJvnlp = $("div[name='newJvnlp']").css("display");
    var includedJvnlp = $("div[name='includedJvnlp']").css("display");
    var excludedJvnlp = $("div[name='excludedJvnlp']").css("display");

    if (jvnlp === "block")
        return "jvnlp";
    if (exjvnlp === "block")
        return "exjvnlp";
    if (newJvnlp === "block")
        return "newJvnlp";
    if (includedJvnlp === "block")
        return "includedJvnlp";
    if (excludedJvnlp === "block")
        return "excludedJvnlp";
}

$("button[name=buttonUploadFile],div[name='cl-btn']").click(function() {
    openFormFile();
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

///опен таблицы исходного реестра ЛП или расчитанного
$("li[name=openTableOriginal]").click(function() {
    $("div[name='newJvnlp']").hide();
    $("div[name='includedJvnlp']").hide();
    $("div[name='excludedJvnlp']").hide();
    $("div[name='ReadyJvnlp']").css("display", "none");
    $("li[name=openTableReady]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openIncludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    var activeTable = GetActiveTable();
    if (activeTable !== "jvnlp" && activeTable !== "exjvnlp") {
        $("li[name=openTableOriginal]").css("background-color", "rgba(0, 222, 255, 0.29)");
        $("div[name='originalJvnlp']").css("display", "block");
        $("li[name=openOriginalReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
        $("li[name=openOriginalExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
        $("div[name='jvnlp']").show(1,
            function() {
                UpdateStyleTableDrugs("tableDrugs");
                if ($("#tableDrugs").html() === "") {
                    alertify.message("Загрузите реестр препаратов");
                }
            });
    }
});

$("li[name=openTableReady]").click(function() {
    $("div[name='exjvnlp']").hide();
    $("div[name='jvnlp']").hide();
    $("div[name='originalJvnlp']").css("display", "none");
    $("li[name=openTableOriginal]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openOriginalReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openOriginalExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    var activeTable = GetActiveTable();
    if (activeTable !== "newJvnlp" && activeTable !== "includedJvnlp" && activeTable !== "excludedJvnlp") {
        $("li[name=openTableReady]").css("background-color", "rgba(0, 222, 255, 0.29)");
        $("div[name='ReadyJvnlp']").css("display", "block");
        $("li[name=openReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
        $("li[name=openIncludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
        $("li[name=openExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
        $("div[name='newJvnlp']").show(1,
            function() {
                UpdateStyleTableDrugs("tableDrugsNew");
                if ($("#tableDrugs").html() === "") {
                    alertify.message("Загрузите реестр препаратов");
                }
            });
    }
});
///------------------------------------------------------///

///опен таблицы для исходного реестра ЛП
$("li[name=openOriginalReadyJvnlp]").click(function() {
    $("div[name='exjvnlp']").hide();
    $("li[name=openOriginalExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    $("li[name=openOriginalReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
    $("div[name='jvnlp']").show(1,
        function() {
            UpdateStyleTableDrugs("tableDrugs");
            if ($("#tableDrugs").html() === "") {
                alertify.message("Загрузите реестр препаратов");
            }
        });
});
$("li[name=openOriginalExcludedJvnlp]").click(function() {
    $("div[name='jvnlp']").hide();
    $("li[name=openOriginalReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    $("li[name=openOriginalExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
    $("div[name='exjvnlp']").show(1,
        function() {
            UpdateStyleTableDrugs("exjvnlpTable");
            if ($("#tableDrugs").html() === "") {
                alertify.message("Загрузите реестр препаратов");
            }
        });
});
///------------------------------------------------------///

///опен таблицы для расчитанного реестра ЛП
$("li[name=openReadyJvnlp]").click(function() {
    $("div[name='includedJvnlp']").hide();
    $("div[name='excludedJvnlp']").hide();
    $("li[name=openIncludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    $("li[name=openReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
    $("div[name='newJvnlp']").show(1,
        function() {
            UpdateStyleTableDrugs("tableDrugsNew");
            if ($("#tableDrugs").html() === "") {
                alertify.message("Загрузите реестр препаратов");
            }
        });
});
$("li[name=openIncludedJvnlp]").click(function() {
    $("div[name='newJvnlp']").hide();
    $("div[name='excludedJvnlp']").hide();
    $("li[name=openReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    $("li[name=openIncludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
    $("div[name='includedJvnlp']").show(1,
        function() {
            UpdateStyleTableDrugs("tableDrugsIncluded");
            if ($("#tableDrugs").html() === "") {
                alertify.message("Загрузите реестр препаратов");
            }
        });
});
$("li[name=openExcludedJvnlp]").click(function() {
    $("div[name='newJvnlp']").hide();
    $("div[name='includedJvnlp']").hide();
    $("li[name=openReadyJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");
    $("li[name=openIncludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

    $("li[name=openExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0.29)");
    $("div[name='excludedJvnlp']").show(1,
        function() {
            UpdateStyleTableDrugs("tableDrugsExcluded", true);
            if ($("#tableDrugs").html() === "") {
                alertify.message("Загрузите реестр препаратов");
            }
        });
});
///------------------------------------------------------///


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
                        $("div[name='jvnlp']").show(1,
                            function() {
                                UpdateStyleTableDrugs("tableDrugs");
                            });
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
                        $("li[name=openExcludedJvnlp]").css("background-color", "rgba(0, 222, 255, 0)");

                        $("div[name='ReadyJvnlp']").css("display", "none");
                        $("div[name='newJvnlp']").css("display", "none");
                        $("div[name='includedJvnlp']").css("display", "none");
                        $("div[name='excludedJvnlp']").css("display", "none");

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
}

function UpdateStyleTableDrugs(nameTable, excludedDrugs = false) {
    // Change the selector if needed
    var $table = $("#" + nameTable + "");
    var $theadCells = $table.find("thead tr[name='headTable2'], tr[name='headTable3']").children();
    var $delElement = 0;
    if (nameTable !== "tableDrugs" && nameTable !== "exjvnlpTable" && !excludedDrugs) {
        $theadCells.each(function(index, value) {
            if (value.innerHTML === "Предельная розничная цена с НДС") {
                $delElement = index;
            }
        });
        $theadCells.splice($delElement, 1);
    }

    // Set the width of tbody columns
    $table.find("tbody tr").children().each(function(i, v) {
        $(v).width($theadCells.map(function() {
            return $(this).width();
        }).get()[i]);
    });

    if (nameTable === "tableDrugs")
        $("#" + nameTable + " tbody")
            .height($("div[name='jvnlp']").height() - $("#" + nameTable + " thead").height() - 16);

    if (nameTable === "exjvnlpTable")
        $("#" + nameTable + " tbody")
            .height($("div[name='exjvnlp']").height() - $("#" + nameTable + " thead").height() - 16);

    if (nameTable === "tableDrugsNew")
        $("#" + nameTable + " tbody")
            .height($("div[name='newJvnlp']").height() - $("#" + nameTable + " thead").height() - 16);

    if (nameTable === "tableDrugsIncluded")
        $("#" + nameTable + " tbody")
            .height($("div[name='includedJvnlp']").height() - $("#" + nameTable + " thead").height() - 16);

    if (nameTable === "tableDrugsExcluded")
        $("#" + nameTable + " tbody")
            .height($("div[name='excludedJvnlp']").height() - $("#" + nameTable + " thead").height() - 16);
}
