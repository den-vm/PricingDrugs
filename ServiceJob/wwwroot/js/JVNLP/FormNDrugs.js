var visibleFormfile = false;
var visibleFormTableDrugs = false;
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
    $("#dynamicRowDrugs").html(
        "<tr>" +
        '<td style="padding: 2px">' +
        '<input type="text" name="nameDrugs" value="" required placeholder="Введите МНН препарата" title="" onKeyup="this.title=this.value">' +
        "</td>" +
        '<td name="dataAdd" style="padding: 2px; width: 115px;" align="center">' +
        "<script>" +
        "var currentDate = new Date();" +
        '$("[name=dataAdd]").html(currentDate.getDate() +' +
        '"." +' +
        "currentDate.getMonth() +" +
        '"." +' +
        "currentDate.getFullYear());" +
        "</script>" +
        "</td>" +
        '<td name="dataDel" style="padding: 2px; width: 123px;" align="center"></td>' +
        '<td style="padding: 2px; width: 159px;">' +
        '<button type="button" class="add">Добавить</button><button type="button" class="del">Исключить</button>' +
        "</td>" +
        "</tr>");
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
    var drugNarcoticTable = $("#dynamicRowDrugs > tr > td > label > input");
    if (drugNarcoticTable.length > 0) {
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
});

//DynamicTable
var DynamicTable = (function(GLOB) {
    var RID = 0;
    return function(tBody) {
        /* Если ф-цию вызвали не как конструктор фиксим этот момент: */
        if (!(this instanceof arguments.callee)) {
            return new arguments.callee.apply(arguments);
        }
        //Делегируем прослушку событий элементу tbody
        tBody.onclick = function(e) {
            var evt = e || GLOB.event,
                trg = evt.target || evt.srcElement;
            if (trg.className && trg.className.indexOf("add") !== -1) {
                _addRow(trg.parentNode.parentNode, tBody);
            } else if (trg.className && trg.className.indexOf("del") !== -1) {
                tBody.rows.length > 1 && _delRow(trg.parentNode.parentNode, tBody);
            }
        };
        var _rowTpl = tBody.rows[0].cloneNode(true);
        // Корректируем имена элементов формы
        var _correctNames = function(row) {
            var elements = row.getElementsByTagName("*");
            for (var i = 0; i < elements.length; i += 1) {
                if (elements.item(i).name) {
                    if (elements.item(i).type &&
                        elements.item(i).type === "radio" &&
                        elements.item(i).className &&
                        elements.item(i).className.indexOf("glob") !== -1) {
                        elements.item(i).value = RID;
                    } else {
                        elements.item(i).name = RID + "[" + elements.item(i).name + "]";
                    }
                }
            }
            RID++;
            return row;
        };
        var _addRow = function(before, tBody) {
            var newNode = _correctNames(_rowTpl.cloneNode(true));
            tBody.insertBefore(newNode, before.nextSibling);
        };
        var _delRow = function(row, tBody) {
            tBody.removeChild(row);
        };
        _correctNames(tBody.rows[0]);
    };
})(this);
new DynamicTable(document.getElementById("dynamicRowDrugs"));