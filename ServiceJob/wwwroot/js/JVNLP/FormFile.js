document.querySelector("html").classList.add("js");

var fileInput = $(".input-file"),
    filesend = $(".file-send");

$("form[name=jvnlpform]").submit(function(event) {
    event.preventDefault(); //disconnect default event submit form
    if (fileInput.val() !== "") {
        var dataForm = new FormData();
        for (var i = 0; i < fileInput[0].files.length; i++) {
            dataForm.append("fileJvnlp", fileInput[0].files[i]);
        }
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
        alertify.error("Выберите файл для загрузки!");
    }
});

fileInput.change(function() {
    $(".input-file-trigger").html("Файл: " + this.files[0].name.toString());
    filesend.html("Отправить");
});