// shows the QR code of the clicked ticket inside the modal
$(function () {
    $(".btn-qr").on("click", function () {
        $("#qrImg").attr("src", $(this).data("url"));
        $("#qrCodeText").text($(this).data("code"));
    });
});
