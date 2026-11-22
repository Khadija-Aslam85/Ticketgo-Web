// seat selection and booking page
$(function () {
    var box = $("#seatBox");
    var eventId = box.data("event-id");
    var seatsUrl = box.data("seats-url");
    var bookUrl = box.data("book-url");
    var price = parseFloat(box.data("price"));
    var selected = [];

    function showMessage(type, text) {
        var alertBox = $("<div></div>").addClass("alert alert-" + type).text(text);
        $("#msg").empty().append(alertBox);
    }

    function updateSummary() {
        if (selected.length === 0) {
            $("#summary").text("No seats selected.");
        } else {
            var total = selected.length * price;
            $("#summary").text("Seats: " + selected.join(", ") + " | Total: Rs. " + total.toLocaleString());
        }
    }

    // $.get : load the seats from the server
    function loadSeats() {
        $.get(seatsUrl, function (data) {
            box.empty();
            selected = [];
            updateSummary();

            for (var i = 1; i <= data.total; i++) {
                var seat = String(i);
                var taken = data.booked.indexOf(seat) !== -1;
                var btn = $("<button type='button' class='btn seat'></button>").text(seat).data("seat", seat);

                if (taken) {
                    btn.addClass("btn-secondary").prop("disabled", true);
                } else {
                    btn.addClass("btn-outline-success");
                }

                box.append(btn);
            }
        }).fail(function () {
            showMessage("danger", "Could not load the seats.");
        });
    }

    // click on a free seat: select or unselect it
    box.on("click", ".seat", function () {
        var btn = $(this);
        var seat = btn.data("seat");
        var index = selected.indexOf(seat);

        if (index === -1) {
            selected.push(seat);
            btn.removeClass("btn-outline-success").addClass("btn-primary");
        } else {
            selected.splice(index, 1);
            btn.removeClass("btn-primary").addClass("btn-outline-success");
        }

        updateSummary();
    });

    // $.ajax : send the booking to the server
    $("#confirmBtn").on("click", function () {
        if (selected.length === 0) {
            showMessage("warning", "Please select at least one seat.");
            return;
        }

        $.ajax({
            url: bookUrl,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ eventId: eventId, seats: selected }),
            headers: { "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val() },
            success: function (res) {
                showMessage("success", res.message + " Your ticket codes: " + res.codes.join(", "));
                loadSeats();
            },
            error: function (xhr) {
                var message = xhr.responseJSON ? xhr.responseJSON.message : "Booking failed.";
                showMessage("danger", message);
                loadSeats();
            }
        });
    });

    loadSeats();
});
