var visibleFormfile = false;
var visibleFormTableDrugs = false;

$("li[name=openFormFile]").click(function () {
    if (visibleFormTableDrugs)
        hide_visibleFormTableDrugs(100);
    hide_visibleFormfile();
});
$("li[name=openFormTableDrugs]").click(function () {
    if (visibleFormfile)
        hide_visibleFormfile(100);
    hide_visibleFormTableDrugs();
});

function hide_visibleFormfile(speed = 200) {

    $("div[name=lockbody]").slideToggle((speed + 100),
        "linear",
        function () {
            $("div[name=divJvnlp]").slideToggle(speed,
                "linear",
                function () {
                    if ($("div[name=divJvnlp]").css("display") === "block") visibleFormfile = true;
                    else visibleFormfile = false;
                });
        });
}

// получить список препаратов из списка при открытии формы
function hide_visibleFormTableDrugs(speed = 200) {
    $("div[name=lockbody]").slideToggle((speed + 100),
        "linear",
        function () {
            $("div[name=divDrugsDownload]").slideToggle(speed,
                "linear", getDrugs());
        });

}

function getDrugs() {
    //получить список препаратов из сервера если форма открыта
    if ($("div[name=divDrugsDownload]").css("display") === "block") {
        visibleFormTableDrugs = true;
        var dataForm = new FormData();
        dataForm.append("narcoticDrugsView", null);
        //RequestFormNPDrugs(dataForm);
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
            if (data["typemessage"] === "complite")
                alertify.message(data["message"]);
        }
    });
}