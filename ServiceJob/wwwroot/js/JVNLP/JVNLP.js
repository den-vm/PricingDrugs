var visibleFormfile = false;
var visibleFormTableDrugs = false;
var visibleFormDrugsPriceCriteria = false;

$("li[name=openFormFile]").click(function() {
    if (visibleFormTableDrugs) // закрыть форму наркотических препаратов
        FormTableDrugs(100);
    if (visibleFormDrugsPriceCriteria) // закрыть форму таблица цен
        FormCalculationCriteria(100);
    Formfile();
});
$("li[name=openFormTableDrugs]").click(function() {
    if (visibleFormfile) // закрыть форму загрузки препаратов
        Formfile(100);
    if (visibleFormDrugsPriceCriteria) // закрыть форму таблица цен
        FormCalculationCriteria(100);
    FormTableDrugs(100);
});
$("li[name=openFormDrugsPriceCriteria]").click(function() {
    if (visibleFormfile) // закрыть форму загрузки препаратов
        Formfile(100);
    if (visibleFormTableDrugs) // закрыть форму наркотических препаратов
        FormTableDrugs(100);
    FormCalculationCriteria(100);
});
$("button[name=SaveCriteriaPrice]").click(function() {
    var criteris = {
        before50: {
            notnarcotik: {
                row1: $("input[name=before50NotNarcotik]").val(),
                row2: $("input[name=before50UUNotNarcotik]").val(),
                row3: $("input[name=before50NotAreaNotNarcotik]").val(),
                row4: $("input[name=before50AreaUUNotNarcotik]").val()
            },
            narcotik: {
                row1: $("input[name=before50Narcotik]").val(),
                row2: $("input[name=before50UUNarcotik]").val(),
                row3: $("input[name=before50NotAreaNarcotik]").val(),
                row4: $("input[name=before50AreaUUNarcotik]").val()
            }
        },
        after50before500: {
            notnarcotik: {
                row1: $("input[name=after50before500NotNarcotik]").val(),
                row2: $("input[name=after50before500UUNotNarcotik]").val(),
                row3: $("input[name=after50before500NotAreaNotNarcotik]").val(),
                row4: $("input[name=after50before500AreaUUNotNarcotik]").val()
            },
            narcotik: {
                row1: $("input[name=after50before500Narcotik]").val(),
                row2: $("input[name=after50before500UUNarcotik]").val(),
                row3: $("input[name=after50before500NotAreaNarcotik]").val(),
                row4: $("input[name=after50before500AreaUUNarcotik]").val()
            }
        },
        after500: {
            notnarcotik: {
                row1: $("input[name=after500NotNarcotik]").val(),
                row2: $("input[name=after500UUNotNarcotik]").val(),
                row3: $("input[name=after500NotAreaNotNarcotik]").val(),
                row4: $("input[name=after500AreaUUNotNarcotik]").val()
            },
            narcotik: {
                row1: $("input[name=after500Narcotik]").val(),
                row2: $("input[name=after500UUNarcotik]").val(),
                row3: $("input[name=after500NotAreaNarcotik]").val(),
                row4: $("input[name=after500AreaUUNarcotik]").val()
            }
        }
    };
    console.log(criteris);
});

function Formfile(speed = 200) {

    $("div[name=lockbody]").slideToggle({
        duration: (speed + 100),
        easing: "linear",
        always: function() {
            $("div[name=divJvnlp]").slideToggle({
                duration: speed,
                easing: "linear",
                always: function() {
                    if ($("div[name=divJvnlp]").css("display") === "block") visibleFormfile = true;
                    else visibleFormfile = false;
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
                        // обратиться на сервер и получить критерии из файла
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
                                console.error(value["message"]);
                                alertify.error(value["message"]);
                            },
                            key * 200);
                    if (value.typemessage === "complite")
                        setTimeout(function() {
                                console.log(value["message"]);
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