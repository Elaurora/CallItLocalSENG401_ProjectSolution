/**
*   This function automatically runs when this javascript file is loaded.
*   This function sets the on click function of the send button to be the validate and send message function below.
*/
$(function () {

    $("#SendMessageButton").click(validateAndSendMessage);
    $('#SumbitReviewButton').click(validateAndSendReview);
});

/**
 * Ensures that the user has entered a valid message, then sends it.
 * If successful, the user will be forewarded to their message inbox
 */
function validateAndSendMessage() {

    var userData = $("#textUserMessage").val();
    if ($.trim(userData) == "") {
        return;
    }

    var recipient = $("#CompanyNameDisplay").text();
    var timestamp = Math.round((new Date()).getTime() / 1000);

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

function validateAndSendReview() {
    var userReview = $("#reviewtextarea").val();
    if ($.trim(userReview) == "") {
        return;
    }

    var companyName = $("#CompanyNameDisplay").text();
    var timestamp = Math.round((new Date()).getTime() / 1000);
    var stars = $("#starselection").val();

    $.ajax({
        method: "POST",
        url: "/CompanyListings/WriteReview",
        data: {
            company: companyName,
            userReview: userReview,
            timestamp: timestamp,
            stars: stars
        },
        success: function () {
            window.location = companyName 
        }
    });
}