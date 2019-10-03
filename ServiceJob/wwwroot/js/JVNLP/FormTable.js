
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
        dataForm.append("narcoticDrugs", JSON.stringify(rows));
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