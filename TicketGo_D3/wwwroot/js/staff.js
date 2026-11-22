// ticket check with $.post
$(function () {
    $("#verifyForm").on("submit", function (e) {
        e.preventDefault();

        $.post($(this).attr("action"), $(this).serialize(), function (res) {
            var type = "warning";
            if (res.status === "valid") type = "success";
            if (res.status === "used") type = "danger";

            var alertBox = $("<div></div>").addClass("alert alert-" + type).text(res.message);
            $("#result").empty().append(alertBox);
            $("#ticketCode").val("").focus();
        }, "json").fail(function () {
            var alertBox = $("<div></div>").addClass("alert alert-danger").text("Could not check the ticket. Please try again.");
            $("#result").empty().append(alertBox);
        });
    });
});
