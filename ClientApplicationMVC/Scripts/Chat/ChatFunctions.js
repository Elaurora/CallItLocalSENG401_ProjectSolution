
var currentSelectedChat = null;
var myHubProxy = null;

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

    initializeSignalRProxy();
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

    addTextToChatBox(userData, "You");
    var recipient = currentSelectedChat;
    var timestamp = Math.round((new Date()).getTime() / 1000);


    myHubProxy.server.sendMessageTo(userData, recipient, timestamp);

    /*//This is the old way of sending the message to the server.
    $.post("/Chat/SendMessage", {
        receiver: recipient,
        timestamp: timestamp,
        message: userData
    });*/
}

function addTextToChatBox(text, sender) {
    var newMessageHtml =
        "<p class='message'>" +
        "<span class='username'";

    if (sender === "You") {
        newMessageHtml += ">You: ";
    }
    else {
        newMessageHtml += " style='color:aqua;'>" + sender + ": ";
    }

    newMessageHtml += "</span>" + text + "</p>";

    $("#ConversationDisplayArea").html(// Add the new message to the message display area.
        $("#ConversationDisplayArea").html() + newMessageHtml);

    $("#ConversationDisplayArea").scrollTop($("#ConversationDisplayArea").prop("scrollHeight"));//Make the scrollbar scroll to the bottom
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

function initializeSignalRProxy() {
    
    myHubProxy = $.connection.hub.createHubProxy("ChatHub");
    myHubProxy.client.receiveMessage = receiveMessage;
    $.connection.hub.start().done(function () {
        myHubProxy.server.hello($("#UsernameDisplay").text());
    });
}


function receiveMessage(message, sender) {
    if (currentSelectedChat === sender) {
        addTextToChatBox(message, sender);
    }
}