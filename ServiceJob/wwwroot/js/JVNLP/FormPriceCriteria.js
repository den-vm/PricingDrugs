function SaveCriteria() {
    var criteriasForm = {
        before50on: {
            nonarcotik: [
                $("input[name=before50NotNarcotik]").val(),
                $("input[name=before50UUNotNarcotik]").val(),
                $("input[name=before50NotAreaNotNarcotik]").val(),
                $("input[name=before50AreaUUNotNarcotik]").val()
            ],
            narcotik: [
                $("input[name=before50Narcotik]").val(),
                $("input[name=before50UUNarcotik]").val(),
                $("input[name=before50NotAreaNarcotik]").val(),
                $("input[name=before50AreaUUNarcotik]").val()
            ]
        },
        after50before500on: {
            nonarcotik: [
                $("input[name=after50before500NotNarcotik]").val(),
                $("input[name=after50before500UUNotNarcotik]").val(),
                $("input[name=after50before500NotAreaNotNarcotik]").val(),
                $("input[name=after50before500AreaUUNotNarcotik]").val()
            ],
            narcotik: [
                $("input[name=after50before500Narcotik]").val(),
                $("input[name=after50before500UUNarcotik]").val(),
                $("input[name=after50before500NotAreaNarcotik]").val(),
                $("input[name=after50before500AreaUUNarcotik]").val()
            ]
        },
        after500: {
            nonarcotik: [
                $("input[name=after500NotNarcotik]").val(),
                $("input[name=after500UUNotNarcotik]").val(),
                $("input[name=after500NotAreaNotNarcotik]").val(),
                $("input[name=after500AreaUUNotNarcotik]").val()
            ],
            narcotik: [
                $("input[name=after500Narcotik]").val(),
                $("input[name=after500UUNarcotik]").val(),
                $("input[name=after500NotAreaNarcotik]").val(),
                $("input[name=after500AreaUUNarcotik]").val()
            ]
        },
        nds: $("input[name=NDSPrice]").val()
    };
    var dataForm = new FormData();
    dataForm.append("priceCriteria", JSON.stringify(criteriasForm));
    $.ajax({
        type: "POST",
        url: "Jvnlp/PriceCriteria/upload",
        data: dataForm,
        dataType: "json",
        processData: false, // отключение преобразования строки запроса по contentType
        contentType:
            false, // отключение преобразования контента в тип по умолчанию: "application/x-www-form-urlencoded;"
        complete: function(data) {
            var requestServer = JSON.parse(data.responseText);
            if (data.status === 500)
                setTimeout(function() {
                        console.error(requestServer["message"]);
                        alertify.error(requestServer["message"]);
                    },
                    200);
            if (data.status === 200)
                setTimeout(function() {
                        console.log(requestServer["message"]);
                        alertify.success(requestServer["message"]);
                    },
                    200);
        }
    });
}

function LoadCriterias() {
    var criterias = {};

    $.ajax({
        type: "POST",
        url: "Jvnlp/PriceCriteria",
        processData: false, // отключение преобразования строки запроса по contentType
        contentType:
            false, // отключение преобразования контента в тип по умолчанию: "application/x-www-form-urlencoded;"
        complete: function (data) {
            var requestServer = JSON.parse(data.responseText);
            if (data.status === 500)
                setTimeout(function () {
                        console.error(requestServer["message"]);
                        alertify.error(requestServer["message"]);
                    },
                    200);
            if (data.status === 200)
                setTimeout(function () {
                        console.log(requestServer["message"]);
                    alertify.success("Загружено");
                    criterias = JSON.parse(requestServer["message"]);
                    console.log(criterias);
                },
                    200);
        }
    });
}