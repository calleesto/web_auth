import { FrontAddress, BackAddress } from "./common.js"

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

export async function logout() {
    let token = localStorage.getItem("token");
    if (token) {
        let url = BackAddress + "/api/logout"
        const options = {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
                "Authorization": token
            }
        };
        fetch( url, options )
            .then( response => response.json() )
            .then( response => {
                console.log( response );
                localStorage.removeItem("token");
            } );
    }
}

function googleLogin() {
    window.location.href = BackAddress + "/auth/login-google";
}

const standardLoginBtn = document.getElementById("standard-login-btn");
const googleLoginBtn = document.getElementById("google-login-btn");
if (standardLoginBtn) {
    standardLoginBtn.addEventListener("click", login);
}
if (googleLoginBtn) {
    googleLoginBtn.addEventListener("click", googleLogin);
}