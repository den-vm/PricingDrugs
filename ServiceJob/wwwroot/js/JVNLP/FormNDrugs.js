$("button[class = add]").click(function() {
    var newRowDrug = '<tr name="drugNew"> style="visibility: visible;"' +
        '<td style="padding: 2px">' +
        '<input type="text" name="nameDrug" value="" required placeholder="Введите МНН препарата" title="" onKeyup="this.title=this.value">' +
        "</td>" +
        '<td style="padding: 2px; width: 115px;" align="center">' +
        '<input name="dataDrugAdd" type="date" />' +
        '<script type="text/javascript">' +
        "$(document).ready(function () {" +
        "var nowDate = new Date();" +
        '$("tr[name = drugNew]").find($("[name=dataDrugAdd]:last")).val(new Date().toISOString().split("T")[0]);' +
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
    if (readRowDrugs($("tr[name = drugNew]")).length > 0)
        dataForm.append("narcoticDrugsAdd", readRowDrugs($("tr[name = drugNew]")));
    if (readRowDrugs($("tr[name = drugEdit]")).length > 0)
        dataForm.append("narcoticDrugsEdit", readRowDrugs($("tr[name = drugEdit]"), true));
    RequestFormNPDrugs(dataForm);
});

$("input[name=invisDrug]").click(function() {
    var listExclude = $("#RowDrugs").find("tr");
    var checked = this.checked;
    $.each(listExclude,
        function() {
            var dateExclude = $(this).find("input[name=dataDrugDel]").val();
            if (dateExclude !== "" && checked) {
                $(this).css("visibility", "collapse");
            } else $(this).css("visibility", "visible");
        });
});

/*-----------------------------------------------
    Чтение наркот. и псих. препаратов из формы
-------------------------------------------------*/
function readRowDrugs(nameRowDrug, outIdDrug = false) {
    var elemetsFormDrugs = nameRowDrug
        .find($("input[name = nameDrug],input[name = dataDrugAdd],input[name = dataDrugDel]"));

    var valueRowDrugs = elemetsFormDrugs.map(function() {
        return this.value;
    }).get(); // return array value

    var listDrugs = [];
    var idDrugs = null;

    if (outIdDrug) { //drugEdit - read ID to saved drugs 
        idDrugs = nameRowDrug.map(function() {
            return this.dataset.id;
        }).get();
    }
    for (var i = 0, j = 0; i < valueRowDrugs.length; i += 3, j++) { // Add new drug
        if (outIdDrug) {
            listDrugs.push(idDrugs[j], valueRowDrugs.slice(i, i + 3));
        } else listDrugs.push(valueRowDrugs.slice(i, i + 3));
    }
    return listDrugs;
}

function getDrugsHTML(listdrugs) {
    var saveDrug = "";
    $.each(listdrugs,
        function(index, drug) {
            var id = drug["id"];
            var nameDrug = drug["nameDrug"];
            var includeDate = drug["includeDate"].substr(0, 10);
            var outDate = drug["outDate"] !== null ? drug["outDate"].substr(0, 10) : "";

            saveDrug += '<tr name="drugSave" data-id = "' +
                id +
                '" style="visibility: visible;">' +
                '<td style="padding: 2px">' +
                '<input type="text" name="nameDrug" value="' +
                nameDrug +
                '" required placeholder="Введите МНН препарата" title="' +
                nameDrug +
                '" onKeyup="this.title=this.value">' +
                "</td>" +
                '<td style="padding: 2px; width: 115px;" align="center">' +
                '<input name="dataDrugAdd" type="date" value="' +
                includeDate +
                '" />' +
                "</td>" +
                '<td style="padding: 2px; width: 123px;" align="center"><input type="date" name="dataDrugDel" value="' +
                outDate +
                '" /></td>' +
                "</tr>";
        });
    return saveDrug;
}