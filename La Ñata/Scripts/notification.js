$(document).ready(function () {
    if ($("#txtNotification").html() !== "") {
        setTimeout(function () {
            $("#notificationContainer").addClass("hidden");
        }, 8000)
        setTimeout(function () {
            $("#notificationContainer").addClass("d-none");
        }, 10000)
    } else {
        $("#notificationContainer").addClass("d-none");
    }
    $("#btnCloseNotification").on("click", function () {
        $("#notificationContainer").addClass("hidden");
        setTimeout(function () {
            $("#notificationContainer").addClass("d-none");
        }, 2000)
    });
});