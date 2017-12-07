var username = "Log In";

/**
 * The purpose of this function is to ensure that the user has entered valid login information before sending the information to the web server.
 */
function validateLoginInfo() {
    if (logInForm.textUsername.value === "") {
        alert("Username cannot be left blank");
        return false;
    }
    if (logInForm.textPassword.value === "") {
        alert("Password cannot be left blank");
        return false;
    }
    username = logInForm.textUsername.value;
    return true;
}


/**
 * The purpose of this function is to ensure that the user has entered valid information for a new account before sending the information to the web server.
 */
function validateCreateAccountInfo() {
    if (createAccountForm.textUsername.value === "") {
        alert("Username cannot be left blank");
        return false;
    }
    if (createAccountForm.textPassword.value === "") {
        alert("Password cannot be left blank");
        return false;
    }
    if (createAccountForm.textAddress.value === "") {
        alert("Address cannot be left blank");
        return false;
    }
    if (createAccountForm.textPhoneNumber.value === "") {
        alert("Phone Number cannot be left blank");
        return false;
    }
    if (createAccountForm.textEmail.value === "") {
        alert("Email cannot be left blank");
        return false;
    }
    username = createAccountForm.textUsername.value;
    return true;
}