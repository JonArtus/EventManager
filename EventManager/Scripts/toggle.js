﻿$("button").click(function () {
    $.ajax({
        url: "events/filter?type=" + this.value,
        dataType: "html",
        success: function (data) {
            $("#events").children(".table").remove();
            $("#events").append(data);
        }
    });
});

$(".btn-toggle").click(function () {
    $(this).find(".btn").toggleClass("active");

  
    if ($(this).find(".btn-warning").size() > 0) {
        $(this).find(".btn").toggleClass("btn-warning");
    }

    $(this).find(".btn").toggleClass("btn-default");
});
