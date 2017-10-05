
var currentSelectedChat = null;
username = "You";

/**
*   This function will set the on click functions for the send button and chat instances.
*/
$(function () {//This function is executed after the entire page is loaded
    $("#SendButton").click(sendMessage);
    $("#ChatInstancesList").children().each(function () {
        $(this).click(chatInstanceSelected);
    });
    var firstChatInstanceBox = $("#ChatInstancesList").children().first();

    firstChatInstanceBox.css("background", "rgba(255, 255, 255, 0.1)");
    currentSelectedChat = firstChatInstanceBox.attr("id");
});

/**
 *  Validates the message the user is trying to send, and sends it.
*   This function will reset the message box and append the message the user sent to the message display area
 */
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

    $.post("/Chat/SendMessage", {
        receiver: recipient,
        timestamp: timestamp,
        message: userData
    });
}

/**
 * When a user selects their chat history with a specific user, this function will load and display the chat history.
 */
function chatInstanceSelected() {
    if ($(this).attr("id") == currentSelectedChat) {
        return;
    }

    $("#" + currentSelectedChat).css("background", "initial");

    currentSelectedChat = $(this).attr("id");

    $("#" + currentSelectedChat).css("background", "rgba(255, 255, 255, 0.1)");

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