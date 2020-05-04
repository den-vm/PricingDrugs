alertify.set("notifier", "position", "top-center");
document.querySelector("html").classList.add("js");

var fileInput = $(".input-file"),
    fileSend = $(".file-send");

$("form[name=jvnlpForm]").submit(function(event) {
    event.preventDefault(); // отключить форму отправки события по умолчанию
    if (fileInput.val() !== "") {
        var dataForm = new FormData();
        for (var i = 0; i < fileInput[0].files.length; i++) {
            dataForm.append("fileJvnlp", fileInput[0].files[i]);
        }
        var progressBar = $("#progressbar");
        $.ajax({
            url: "Jvnlp/upload",
            type: "POST",
            data: dataForm,
            dataType: "json",
            processData: false, // отключение преобразования строки запроса по contentType
            contentType:
                false, // отключение преобразования контента в тип по умолчанию: "application/x-www-form-urlencoded; charset=UTF-8"
            xhr: function() {
                var xhr = $.ajaxSettings.xhr(); // получаем объект XMLHttpRequest
                xhr.upload.addEventListener("progress",
                    function(evt) { // добавляем обработчик события progress (onprogress)
                        if (evt.lengthComputable) { // если известно количество байт
                            // высчитываем процент загруженного
                            var percentComplete = Math.ceil(evt.loaded / evt.total * 100);
                            // устанавливаем значение в атрибут value тега <progress>
                            // и это же значение альтернативным текстом для браузеров, не поддерживающих <progress>
                            $("#textProgress").html("Загружено " + percentComplete + "%");
                            progressBar.val(percentComplete);
                        }
                    },
                    false);
                xhr.upload.addEventListener("load",
                    function() { // добавляем обработчик события progress (onprogress)
                        $("#textProgress").html("Загружен");
                        $("div[name='lockActions']").css("display", "block");
                    },
                    false);
                return xhr;
            },
            statusCode: {
                200: function(data) {
                    $("form[name='jvnlpForm'] > div > input[type='file']").val("");
                    $("form[name='jvnlpForm'] > div > label").html("Выберите файл");

                    $("div[name='lockActions']").css("display", "none");
                    openFormFile();
                    GenerateTableJvnlp(data["originalDrugs"], "tableDrugs");
                    $("label[name='infotable']").data("idlist", "1");
                    $("label[name='infotable']")
                        .html(`${data["originalDrugsViewLength"]} из ${data["originalDrugsLength"]} (Стр. ${$(
                            "label[name='infotable']").data("idlist")})`);

                    //очищаем все события в tableDrugs
                    $("table[id='tableDrugs'] > thead > tr > td > input[type='search']").unbind();

                    //событие фильтрации для tableDrugs с задержкой ввода
                    var timeOut;
                    $("table[id='tableDrugs'] > thead > tr > td > input[type='search']").on("input",
                        function(input) { // выполнение после прекращения ввода
                            clearTimeout(timeOut);
                            timeOut = setTimeout(function() {
                                    var nametable = GetActiveTable();
                                    var inputfilters = [];
                                    $("table[id='tableDrugs'] > thead > tr > td").find(":input[type='search']").each(
                                        function (i, input) {
                                            inputfilters.push($(input).val()); // записываем значение всех полей поиска
                                        });

                                    $.get("Jvnlp/Drugs/Filtered",
                                        { nameTable: nametable, listFilter: JSON.stringify(inputfilters) })
                                        .always(function(data) {
                                            var codeResponse = data["status"];
                                            if (codeResponse === 500) {
                                                var jsonMessage = JSON.parse(data.responseText);
                                                alertify.error(jsonMessage["message"]);
                                                return;
                                            }
                                            $("#tableDrugs tbody").html("");
                                            GenerateTableJvnlp(data["filterRowList"], "tableDrugs", true);
                                            $("label[name='infotable']").data("idlist", "1");
                                            $("label[name='infotable']")
                                                .html(`${data["filterListLength"]} из ${data["filterListViewLength"]
                                                    } (Стр. ${$("label[name='infotable']").data("idlist")})`);
                                        });
                                },
                                2000);

                        });
                    //$("table[id='tableDrugs'] > thead > tr > td > input[type='search']").on("keydown",
                    //    function() { // очищаем таймер когда вводит текст
                    //        clearTimeout(timeOut);
                    //    });
                },
                500: function(data) {
                    $("form[name='jvnlpForm'] > div > input[type='file']").val("");
                    $("form[name='jvnlpForm'] > div > label").html("Выберите файл");

                    $("div[name='lockActions']").css("display", "none");
                    var jsonMessage = JSON.parse(data.responseText);
                    alertify.error(jsonMessage["message"]);
                }
            }
        });
    } else {
        alertify.error("Выберите файл для загрузки");
    }
});

fileInput.change(function() {
    $(".input-file-trigger").html("Файл: " + this.files[0].name.toString());
    fileSend.html("Отправить");
});

function GenerateTableJvnlp(drugs, nameTable = "", filter = false) {
    var lenghtHeaderColumn = 0;
    var jsonDrugs = JSON.parse(drugs);
    jsonDrugs.forEach(function(item, i) { // clear null row Drugs
        var delRow = 0;
        item.forEach(function(item, i) {
            if (item !== null)
                delRow = 1;
        });

        if (delRow === 0) {
            delete jsonDrugs[i]; // удалям полностью пустые строки
            return;
        }

        if (i === 2) {
            for (var j = item.length - 1; item[j] === null; j--) {
                delete item[j]; // удаляем пустые столбцы в заголовке таблицы
                lenghtHeaderColumn++;
            }
            return;
        }
        for (var j = item.length - 1, headlenght = lenghtHeaderColumn; item[j] === null && headlenght !== 0; j--) {
            delete item[j];
            headlenght--;
        }

    });

    //если данные отфильтрованны
    if (filter) {
        if (nameTable === "newJvnlp" || nameTable === "includedJvnlp") {
            // тут будет построение таблицы для рассчитанного ЖПВНЛ и Включенных
        }

        var tbody = "";
        delete jsonDrugs[0]; // удаляем заголовок таблицы
        delete jsonDrugs[2]; // удаляем строку с заголовками столбцов
        jsonDrugs.forEach(function(item, i) { // обработка строк с препаратами
            var tr = '<tr name="drug">';
            item.forEach(function(item, i) {
                if (item === null)
                    item = "";
                tr += (`<td style='width: calc(100%/${jsonDrugs[3].length})'>${item}</td>`);
            });
            tr += ("</tr>");
            tbody += tr;
        });
        $("#tableDrugs tbody").html(tbody);
    }

    //если данные не отфильтрованны
    if (!filter) {
        if (nameTable === "newJvnlp" || nameTable === "includedJvnlp") {
            // тут будет построение таблицы для рассчитанного ЖПВНЛ и Включенных
        }

        // а тут будет построение таблицы для всех остальных типов таблиц
        var table = "";
        var thead = "<thead>";
        var tbody = '<tbody name="drugs" style="overflow-y: scroll;">';
        jsonDrugs.forEach(function(item, i) {
            if (i === 0) {
                thead += ('<tr name="headTable1">');
                thead += (`<td colspan="${jsonDrugs[2].length}" style="font-size: 21px;">${item[i]}</td>`);
                thead += ("</tr>");
                return;
            }
            if (i === 2) {
                var headColumn = ('<tr name="headTable2">');
                var headFilters = '<tr class="searchInput" name="headSearch">';
                item.forEach(function(item, i) {
                    headColumn += (`<td style='width: calc(100%/${jsonDrugs[2].length})'>${item}</td>`);
                    headFilters += ("<td>");
                    headFilters += (`<input name="filterTable${i}" type="search" value="" placeholder="Поиск">`);
                    headFilters += ("</td>");
                });
                headFilters += ("</tr>");
                headColumn += ("</tr>");
                thead += headColumn;
                thead += headFilters;
                thead += ("</thead>");
                return;
            }
            var tr = '<tr name="drug">';
            item.forEach(function(item, i) {
                if (item === null)
                    item = "";
                tr += (`<td style='width: calc(100%/${jsonDrugs[2].length})'>${item}</td>`);
            });
            tr += ("</tr>");
            tbody += tr;
        });
        table += thead;
        table += tbody;
        $(`#${nameTable}`).html(table);
    }
}