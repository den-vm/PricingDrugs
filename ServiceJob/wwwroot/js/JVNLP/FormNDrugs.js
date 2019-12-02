$("button[class = add]").click(function() {
    var newRowDrug = '<tr name="drugNew">' +
        '<td style="padding: 2px">' +
        '<input type="text" name="nameDrug" value="" required placeholder="Введите МНН препарата" title="" onKeyup="this.title=this.value">' +
        "</td>" +
        '<td style="padding: 2px; width: 115px;" align="center">' +
        '<input name="dataDrugAdd" type="date" />' +
        '<script type="text/javascript">' +
        "$(document).ready(function () {" +
        "var nowDate = new Date();" +
        '$("[name=dataDrugAdd]").val(new Date().toISOString().split("T")[0]);' +
        "})" +
        "</script>" +
        "</td>" +
        '<td style="padding: 2px; width: 123px;" align="center"><input type="date" name="dataDrugDel" /></td>' +
        "</tr>";
    $("#RowDrugs").append(newRowDrug);
});

$("button[class = del]").click(function() {
    $("tr[name = drugNew]:last").remove();
});

$("form[name=drugNarcoticForm]").submit(function(event) {
    event.preventDefault(); // отключить форму отправки события по умолчанию
    var dataForm = new FormData();
    dataForm.append("narcoticDrugsAdd", readRowDrugs($("tr[name = drugNew]")));
    dataForm.append("narcoticDrugsEdit", readRowDrugs($("tr[name = drugEdit]"), true));
    //RequestFormNPDrugs(dataForm);
});

// Чтение наркот. и псих. препаратов из формы
function readRowDrugs(nameRowDrug, outIdDrug = false) {
    var elemetsFormDrugs = nameRowDrug
        .find($("input[name = nameDrug],input[name = dataDrugAdd],input[name = dataDrugDel]"));
    var valueRowDrugs = elemetsFormDrugs.map(function() {
        return this.value;
    }).get();
    var listDrugs = [];
    var idDrugs = null;
    if (outIdDrug) { // read ID to saved drugs 
        idDrugs = nameRowDrug.map(function() {
            return this.dataset.id;
        }).get();
    }
    for (var i = 0, j = 0; i < valueRowDrugs.length; i += 3, j++) {
        if (outIdDrug) {
            listDrugs.push(idDrugs[j], valueRowDrugs.slice(i, i + 3));
        } else listDrugs.push(valueRowDrugs.slice(i, i + 3));
    }
    return listDrugs;
}

var mutationObserver = new MutationObserver(function(mutations) {
    mutations.forEach(function(mutation) {
        //console.log(mutation);
        alert(mutation);
    });
});
console.log($("#RowDrugs")[0]);
// Запускаем наблюдение за изменениями в HTML-элементе JVNLP
mutationObserver.observe($("#RowDrugs")[0],
    {
        attributes: true, //в атрибутах node
        characterData: true, //наблюдать ли за node.data (текстовое содержимое)
        childList: true, //изменения в непосредственных детях node
        subtree: true, //во всех потомках node
        attributeOldValue: true, //если true, будет передавать и старое и новое значение node.data в колбэк (см далее), иначе только новое (также требуется опция characterData)
        characterDataOldValue: true //если true, будет передавать и старое и новое старое значение атрибута в колбэк (см далее), иначе только новое (также требуется опция attributes)
    });