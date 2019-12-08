var visibleFormfile = false;
var visibleFormTableDrugs = false;

$("li[name=openFormFile]").click(function() {
    if (visibleFormTableDrugs)
        FormTableDrugs(100);
    Formfile();

});
$("li[name=openFormTableDrugs]").click(function() {
    if (visibleFormfile)
        Formfile(100);
    FormTableDrugs(100);
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
            if (data["typemessage"] === "error")
                alertify.error(data["message"]);
            if (data["typemessage"] === "complite") {
                alertify.message(data["message"]);

                //Запускаем наблюдение за изменениями в HTML-элементе input JVNLP 
                var elementsDrugSave = $("tr[name=drugSave]")
                    .find($("input[name = nameDrug],input[name = dataDrugAdd],input[name = dataDrugDel]"));
                elementsDrugSave.on("input",
                    function() {
                        $(this).parents("td,tr").attr("name", "drugEdit");
                    });
            }
        }
    });
}