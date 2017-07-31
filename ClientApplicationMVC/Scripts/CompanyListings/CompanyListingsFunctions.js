$(function () {

    $("#SendButton").click(validateAndSendMessage);
});

function validateAndSendMessage() {

    var userData = $("#textUserMessage").val();
    if ($.trim(userData) == "") {
        return;
    }

    var recipient = $("#CompanyNameDisplay").text();
    var timestamp = Math.round((new Date()).getTime() / 1000);

     //TODO AMIR: Here if where the web client sends messages to the the web server
    $.ajax({
        method: "POST",
        url: "/Chat/SendMessage",
        data: {
            receiver: recipient,
            timestamp: timestamp,
            message: userData
        },
        success: function () {
            window.location = "/Chat/Index";
        }
    });
}