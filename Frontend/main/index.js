function loginButtonLogic() {
    const btn = document.getElementById("logInOrOutButton");

    const isLoggedIn = localStorage.getItem('token');
    if (isLoggedIn) {
        btn.innerHTML = "Log Out";
        logout();
    } else {
        btn.innerHTML = "Log In";
        window.location.href = "../login/login.html";
    }
}