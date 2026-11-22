// code used on every page
$(function () {

    function getToken() {
        return $("input[name='__RequestVerificationToken']").first().val();
    }

    // delete an event with $.ajax and remove its table row
    $(document).on("click", ".js-delete", function () {
        var button = $(this);

        if (!confirm("Are you sure you want to delete this event?")) {
            return;
        }

        $.ajax({
            url: button.data("url"),
            type: "POST",
            headers: { "RequestVerificationToken": getToken() },
            success: function () {
                button.closest("tr").fadeOut(300, function () {
                    $(this).remove();
                });
            },
            error: function () {
                alert("Could not delete the event.");
            }
        });
    });

    // block or unblock a user with $.ajax
    $(document).on("click", ".js-toggle-block", function () {
        var button = $(this);
        var badge = button.closest("tr").find(".js-status");

        $.ajax({
            url: button.data("url"),
            type: "POST",
            headers: { "RequestVerificationToken": getToken() },
            success: function (res) {
                if (res.blocked) {
                    badge.removeClass("bg-success").addClass("bg-danger").text("Blocked");
                    button.removeClass("btn-danger").addClass("btn-success").text("Unblock");
                } else {
                    badge.removeClass("bg-danger").addClass("bg-success").text("Active");
                    button.removeClass("btn-success").addClass("btn-danger").text("Block");
                }
            },
            error: function (xhr) {
                var message = xhr.responseJSON ? xhr.responseJSON.message : "Something went wrong.";
                alert(message);
            }
        });
    });
});
