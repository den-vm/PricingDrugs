var visibleFormfile = false;
var visibleFormTableDrugs = false;

var newRowDrug = '<tr name="drugNew">' +
    '<td style="padding: 2px">' +
    '<input type="text" name="nameDrug" value="" required placeholder="Введите МНН препарата" title="" onKeyup="this.title=this.value">' +
    "</td>" +
    '<td style="padding: 2px; width: 115px;" align="center">' +
    '<input name="dataDrugAdd" type="date" />' +
    '<script type="text/javascript">' +
    "$(document).ready(function () {" +
    "var nowDate = new Date();" +
    '$("[name=dataDrugAdd]").val(nowDate.getFullYear() + "-" + nowDate.getMonth() + "-" + nowDate.getDate());' +
    "})" +
    "</script>" +
    "</td>" +
    '<td style="padding: 2px; width: 123px;" align="center"><input type="date" name="dataDrugDel" /></td>' +
    "</tr>";

$("li[name=openFormFile]").click(function() {
    if (visibleFormTableDrugs)
        hide_visibleFormTableDrugs(100);
    hide_visibleFormfile();
});
$("li[name=openFormTableDrugs]").click(function() {
    if (visibleFormfile)
        hide_visibleFormfile(100);
    hide_visibleFormTableDrugs();
});

function hide_visibleFormfile(speed = 200) {

    $("div[name=lockbody]").slideToggle((speed + 100),
        "linear",
        function() {
            $("div[name=divJvnlp]").slideToggle(speed,
                "linear",
                function() {
                    if ($("div[name=divJvnlp]").css("display") === "block") visibleFormfile = true;
                    else visibleFormfile = false;
                });
        });
}


// получить список препаратов из списка при открытии формы
function hide_visibleFormTableDrugs(speed = 200) {
    $("div[name=lockbody]").slideToggle((speed + 100),
        "linear",
        function() {
            $("div[name=divDrugsDownload]").slideToggle(speed,
                "linear",
                function() {
                    //получить список препаратов из сервера если форма открыта
                    if ($("div[name=divDrugsDownload]").css("display") === "block") {
                        visibleFormTableDrugs = true;
                        var dataForm = new FormData();
                        dataForm.append("narcoticDrugsView", null);
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
                    } else {
                        visibleFormTableDrugs = false;
                        cleartbodyDrugs();
                    }
                });
        });

}

function cleartbodyDrugs() {
    $("#RowDrugs").html("");
}
//$("div[name=lockbody]").css({
//    "display": "none",
//    "position": "absolute",
//    "top": "66px",
//    "bottom": "0",
//    "right": "0",
//    "left": "0",
//    "width": "100%",
//    "height": "82%",
//    "background": "#000000cf"
//});

$("form[name=drugNarcoticForm]").submit(function(event) {
    event.preventDefault(); // отключить форму отправки события по умолчанию
    var drugNarcoticTable = $("#RowDrugs > tr > td > label > input");
    if (false) {
        var dataForm = new FormData();
        var rows = [];
        for (var i = 0; i < drugNarcoticTable.length; i++) {
            //rows.push(`{name: "${drugNarcoticTable[i].value}"`);
            rows.push(drugNarcoticTable[i].value);
        }
        dataForm.append("narcoticDrugsAdd", JSON.stringify(rows));

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
    readNewDrugs();
});

function readNewDrugs() {
    var elemetsFormDrugs = $("tr[name = drugNew]")
        .find($("input[name = nameDrug],input[name = dataDrugAdd],input[name = dataDrugDel]"));
    var valueRowDrugs = elemetsFormDrugs.map(function() {
        return this.value;
    }).get();
    var listDrugs = [];
    for (var i = 0; i < valueRowDrugs.length; i += 3) {
        listDrugs.push(valueRowDrugs.slice(i, i + 3));
    }
    //console.log(listDrugs);
}

$("button[class=add]").click(function() {
    //var newDrug = singlRowDrug.replace(new RegExp("drugSingl"), "newDrug");
    $("#RowDrugs").append(newRowDrug);
});

$("button[class=del]").click(function() {
    $("tr[name=drugNew]:last").remove();
});