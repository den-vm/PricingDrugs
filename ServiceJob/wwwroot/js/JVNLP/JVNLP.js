var visibleTableJvnlp = false;
var visibleFormTableDrugs = false;
var visibleFormDrugsPriceCriteria = false;
var visibleFormFile = false;
var fActive = '';

$(window).on('resize', function () {
        UpdateStyleTableDrugs("tableDrugsNew");
        UpdateStyleTableDrugs("tableDrugsIncluded");
        UpdateStyleTableDrugs("tableDrugsExcluded", true);
});



$('#tableDrugsNew thead > tr > td > input').on('input', function (input) {
    var nameInput = input['target'].name;
    var text = input['target'].value;
    filterDrugs("newJvnlp", nameInput, text);
});

$("button[name=buttonUploadFile],div[name='cl-btn']").click(function() {
    openFormFile();
});

$("li[name=openTableJvnlp]").click(function() {
    if (visibleFormTableDrugs) // закрыть форму наркотических препаратов
        FormTableDrugs(100);
    if (visibleFormDrugsPriceCriteria) // закрыть форму таблица цен
        FormCalculationCriteria(100);
    TableJvnlp(100);
});

$("li[name=openTableOriginal]").click(function() {
    $("div[name='originalJvnlp']").css("display", "block");
    $("div[name='ReadyJvnlp']").css("display", "none");
});
$("li[name=openTableReady]").click(function() {
    $("div[name='originalJvnlp']").css("display", "none");
    $("div[name='ReadyJvnlp']").css("display", "block");
    $("div[name='newJvnlp']").show(1, function () {
        UpdateStyleTableDrugs("tableDrugsNew");
    });
});

$("li[name=openReadyJvnlp]").click(function () {
    $("div[name='includedJvnlp']").hide();
    $("div[name='excludedJvnlp']").hide();
    $("div[name='newJvnlp']").show(1, function () {
        UpdateStyleTableDrugs("tableDrugsNew");
    });
});
$("li[name=openIncludedJvnlp]").click(function () {
    $("div[name='newJvnlp']").hide();
    $("div[name='excludedJvnlp']").hide();
    $("div[name='includedJvnlp']").show(1, function () {
        UpdateStyleTableDrugs("tableDrugsIncluded");
    });
});
$("li[name=openExcludedJvnlp]").click(function () {
    $("div[name='newJvnlp']").hide();
    $("div[name='includedJvnlp']").hide();
    $("div[name='excludedJvnlp']").show(1, function () {
        UpdateStyleTableDrugs("tableDrugsExcluded", true);
    });
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
$("button[name=SaveCriteriaPrice]").click(function() {
    SaveCriteria();
});
$("button[name=LoadCriteriaPrice]").click(function() {
    LoadCriterias();
});

function filterDrugs(nameTable, nameInput, text) {
    if (nameTable === "newJvnlp") {
        var indexCell = nameInput.replace('filterTable', '');
        if (fActive != nameInput) {
            $('#tableDrugsNew tbody > tr').filter(function (index, element) {
                element[indexCell]
                return index % 2 === 0;
            }).slideDown();
        }
        alert(index);
    }
}

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
                    if ($("div[name=tableDrugs]").css("display") === "block") visibleTableJvnlp = true;
                    else visibleTableJvnlp = false;
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
    if (!excludedDrugs) {
        $theadCells.each(function (index, value) {
            if (value.innerHTML === "Предельная розничная цена с НДС") {
                $delElement = index;
            }
        });
        $theadCells.splice($delElement, 1);
    }

    // Set the width of tbody columns
    $table.find("tbody tr").children().each(function (i, v) {
        $(v).width($theadCells.map(function () {
            return $(this).width();
        }).get()[i]);
    });

    $("#" + nameTable + " tbody")
        .height($("div[name='newJvnlp']").height() - $("#" + nameTable + " thead").height() - 16);
}

function GenerateTableJvnlp(listDrugs, listRemovedDrugs) {

}


