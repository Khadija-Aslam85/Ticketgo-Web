// live search of events with $.get
$(function () {
    var timer = null;

    $("#searchBox").on("keyup", function () {
        var box = $(this);
        clearTimeout(timer);

        // wait a little until the user stops typing
        timer = setTimeout(function () {
            $.get(box.data("url"), { q: box.val() }, function (html) {
                $("#eventList").html(html);
            });
        }, 300);
    });
});
