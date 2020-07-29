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
                        alertify.error(requestServer["message"]);
                    },
                    200);
            if (data.status === 200)
                setTimeout(function() {
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
        complete: function(data) {
            var requestServer = JSON.parse(data.responseText);
            if (data.status === 500)
                setTimeout(function() {
                        alertify.error(requestServer["message"]);
                    },
                    200);
            if (data.status === 200)
                setTimeout(function() {
                        alertify.success("Загружено");
                        criterias = JSON.parse(requestServer["message"]);

                        // до 50 включительно
                        $("input[name=before50NotNarcotik]").val(criterias.before50on.nonarcotik[0]);
                        $("input[name=before50UUNotNarcotik]").val(criterias.before50on.nonarcotik[1]);
                        $("input[name=before50NotAreaNotNarcotik]").val(criterias.before50on.nonarcotik[2]);
                        $("input[name=before50AreaUUNotNarcotik]").val(criterias.before50on.nonarcotik[3]);
                        $("input[name=before50Narcotik]").val(criterias.before50on.narcotik[0]);
                        $("input[name=before50UUNarcotik]").val(criterias.before50on.narcotik[1]);
                        $("input[name=before50NotAreaNarcotik]").val(criterias.before50on.narcotik[2]);
                        $("input[name=before50AreaUUNarcotik]").val(criterias.before50on.narcotik[3]);

                        // свыше 50 до 500 включительно
                        $("input[name=after50before500NotNarcotik]").val(criterias.after50before500on.nonarcotik[0]);
                        $("input[name=after50before500UUNotNarcotik]").val(criterias.after50before500on.nonarcotik[1]);
                        $("input[name=after50before500NotAreaNotNarcotik]")
                            .val(criterias.after50before500on.nonarcotik[2]);
                        $("input[name=after50before500AreaUUNotNarcotik]")
                            .val(criterias.after50before500on.nonarcotik[3]);
                        $("input[name=after50before500Narcotik]").val(criterias.after50before500on.narcotik[0]);
                        $("input[name=after50before500UUNarcotik]").val(criterias.after50before500on.narcotik[1]);
                        $("input[name=after50before500NotAreaNarcotik]").val(criterias.after50before500on.narcotik[2]);
                        $("input[name=after50before500AreaUUNarcotik]").val(criterias.after50before500on.narcotik[3]);

                        // свыше 500
                        $("input[name=after500NotNarcotik]").val(criterias.after500.nonarcotik[0]);
                        $("input[name=after500UUNotNarcotik]").val(criterias.after500.nonarcotik[1]);
                        $("input[name=after500NotAreaNotNarcotik]").val(criterias.after500.nonarcotik[2]);
                        $("input[name=after500AreaUUNotNarcotik]").val(criterias.after500.nonarcotik[3]);
                        $("input[name=after500Narcotik]").val(criterias.after500.narcotik[0]);
                        $("input[name=after500UUNarcotik]").val(criterias.after500.narcotik[1]);
                        $("input[name=after500NotAreaNarcotik]").val(criterias.after500.narcotik[2]);
                        $("input[name=after500AreaUUNarcotik]").val(criterias.after500.narcotik[3]);

                        // НДС
                        nds: $("input[name=NDSPrice]").val(criterias.nds);
                    },
                    200);
        }
    });
}