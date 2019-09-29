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
                            progressBar.val(percentComplete).text("Загружено " + percentComplete + "%");
                        }
                    },
                    false);
                return xhr;
            },
            success: function(data) {
                if (data["typemessage"] === "error")
                    alertify.error(data["message"]);
                if (data["typemessage"] === "complite")
                    alertify.message(data["message"]);
            }
        });
    } else {
        alertify.error("Выберите файл для загрузки!");
    }
});

fileInput.change(function () {
    $(".input-file-trigger").html("Файл: " + this.files[0].name.toString());
    fileSend.html("Отправить");
});