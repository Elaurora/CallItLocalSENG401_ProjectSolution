
var currentSelectedChat = null;
username = "You";
$(function () {//This function is executed after the entire page is loaded
    $("#SendButton").click(sendMessage);
    $("#ChatInstancesList").children().each(function () {
        $(this).click(chatInstanceSelected);
    });
    var firstChatInstanceBox = $("#ChatInstancesList").children().first();

    firstChatInstanceBox.css("background", "rgba(255, 255, 255, 0.1)");
    currentSelectedChat = firstChatInstanceBox.attr("id");
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
    var recipient = currentSelectedChat;
    var timestamp = Math.round((new Date()).getTime() / 1000);

    //TODO AMIR: Here if where the web client sends messages to the the web server
    $.post("/Chat/SendMessage", {
        receiver: recipient,
        timestamp: timestamp,
        message: userData
    });
}

function chatInstanceSelected() {
    //TODO: This function - Should load the selected chat instance from the database and display it in the chat display area
    
    if ($(this).attr("id") == currentSelectedChat) {
        return;
    }

    $("#" + currentSelectedChat).css("background", "initial");

    currentSelectedChat = $(this).attr("id");

    $("#" + currentSelectedChat).css("background", "rgba(255, 255, 255, 0.1)");

     //TODO AMIR: Here if where the web client sends messages to the the web server
    $.ajax({
        method: "GET",
        url: "/Chat/Conversation",
        data: {
            otherUser: currentSelectedChat
        },
        success: function (data) {
            $("#ConversationDisplayArea").html(data);
        }
    });

}