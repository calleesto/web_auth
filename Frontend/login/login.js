import { FrontAddress, BackAddress } from "../common.js"

async function login() {
    let login = document.getElementById('username');
    let password = document.getElementById('password');

    let errorMessage = document.getElementById('login-error-message');
    errorMessage.textContent = "";

    let l = login.value;
    let p = password.value;

    let url = BackAddress + "/api/login"

    const params = {
        Username: l,
        Password: p
    };

    const options = {
        method: 'POST',
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify( params )
    };

    fetch( url, options )
        .then( response => {
            if (!response.ok) {
                throw new Error("incorrect credentials!");
            }

            return response.json();
        } )
        .then( response => {
            localStorage.setItem("token", response.token);
            window.location.href = FrontAddress + "/main/index.html";
        } )
        .catch( error => {
            errorMessage.textContent = error.message;
        } );
}

function googleLogin() {
    window.location.href = BackAddress + "/auth/login-google";
}

document.getElementById("standard-login-btn").addEventListener("click", login);
document.getElementById("google-login-btn").addEventListener("click", googleLogin);