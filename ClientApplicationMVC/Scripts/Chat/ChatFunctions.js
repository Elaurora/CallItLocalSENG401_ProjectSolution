

username = "You";
$(function () {//This function is executed after the entire page is loaded

    $("#SendButton").click(sendMessage);



});


function sendMessage() {
    var userData = $("#textUserMessage").val();
    if ($.trim(userData) == "") {
        return;
    }
    $("#textUserMessage").val("");//Clear the chat box

    var convoPrevState = $("#ConversationDisplayArea").html();
    var htmlNewMessage =
        "<p class='message'>" +
        "<span class='username'>" + username + ": " + "</span>" + userData + "</p>";
    $("#ConversationDisplayArea").html(convoPrevState + htmlNewMessage);// Add the new message to the message display area.

    $("#ConversationDisplayArea").scrollTop($("#ConversationDisplayArea").prop("scrollHeight"));//Make the scrollbar scroll to the bottom

    //TODO: Implement recipient function
    var recipient = "TestCompany";
    var timestamp = Math.round((new Date()).getTime() / 1000);

    $.post("Chat/SendMessage", {
        receiver: recipient,
        timestamp: timestamp,
        message: userData
    });
}

function chatInstanceSelected() {
    //TODO: This function - Should load the selected chat instance from the database and display it in the chat display area
}