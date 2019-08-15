$(function() {

    var ul = $("#upload ul");

    $("#drop a").click(function() {
        // Simulate a click on the file input button
        // to show the file browser dialog
        $(this).parent().find("input").click();
    });
    //$("#upload").submit(function (e) {
    //    //e.preventDefault(); // stop the standard form submission
    //    $.ajax({
    //        url: this.action,
    //        type: this.method,
    //        data: $(this).serialize(),
    //        success: function (data) {
    //            console.log(data); // the object returned from your Action will be displayed here.
    //        }
    //    });
    //});
    // Initialize the jQuery File Upload plugin
    $("#upload").fileupload({

        // This element will accept file drag/drop uploading
        dropZone: $("#drop"),

        // This function is called when a file is added to the queue;
        // either via the browse button, or via drag/drop:
        add: function(e, data) {

            var tpl = $(
                '<li class="working" style="height: 100%;"><input type="text" value="0" data-width="48" data-height="48"' +
                ' data-fgColor="#0788a5" data-readOnly="1" data-bgColor="#3e4043" /><div class="nameFile">' +
                data.files[0].name +
                '</div><i class="sizeFile">' +
                formatFileSize(data.files[0].size) +
                "</i></p><span class='del_fileDelete_working'></span></li>");
            $(".infoFile").html("");
            // Add the HTML to the UL element
            data.context = tpl.appendTo(ul);

            // Initialize the knob plugin
            tpl.find("input").knob();

            // Listen for clicks on the cancel icon
            tpl.find("span").click(function() {

                if (tpl.hasClass("working")) {
                    jqXhr.abort();
                    tpl.fadeOut(function() {
                        tpl.remove();
                    });
                }
            });

            // Automatically upload the file once it is added to the queue
            var jqXhr = data.submit(function (e) {
                e.preventDefault(); // stop the standard form submission
                $.ajax({
                    type: "POST",
                    data: $(this).serialize(),
                    success: function (data) {
                        console.log(data);
                    }
                });
            });
        },

        progress: function(e, data) {

            // Calculate the completion percentage of the upload
            var progress = parseInt(data.loaded / data.total * 100, 10);

            // Update the hidden input field and trigger a change
            // so that the jQuery knob plugin knows to update the dial
            data.context.find("input").val(progress).change();

            if (progress === 100) {
                data.context.removeClass("working");
                data.context.addClass("complete");

                var spanDelFile = "<span class='del_fileDelete_complete'></span>";
                //spanDelFile += "<div id='tosend'><a>Отправить</a><input type='submit' value='Отправить' name='sendFileJVNLP'/></div>";
                data.context.append(spanDelFile);
                data.context.find($(".del_fileDelete_complete")).on("click",
                    function(e) {
                        $(data.context).animate({
                                opacity: 0
                            },
                            300,
                            function() {
                                data.context.remove();
                            });
                    });
            }
        },
        fail: function(e, data) {
            // Something has gone wrong!
            data.context.addClass("error");
        }

    });


    // Prevent the default action when a file is dropped on the window
    $(document).on("drop dragover",
        function(e) {
            e.preventDefault();
        });

    // Helper function that formats the file sizes
    function formatFileSize(bytes) {
        if (typeof bytes !== "number") {
            return "";
        }

        if (bytes >= 1000000000) {
            return (bytes / 1000000000).toFixed(2) + " GB";
        }

        if (bytes >= 1000000) {
            return (bytes / 1000000).toFixed(2) + " MB";
        }

        return (bytes / 1000).toFixed(2) + " KB";
    }

});