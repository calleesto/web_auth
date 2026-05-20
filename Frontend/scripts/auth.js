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
            localStorage.setItem("refreshToken", response.refreshToken);
            window.location.href = FrontAddress + "/index.html";
        } )
        .catch( error => {
            errorMessage.textContent = error.message;
        } );
}

export async function logout() {
    let token = localStorage.getItem("token");
    let refreshToken = localStorage.getItem("refreshToken");
    if (token && refreshToken) {
        let url = BackAddress + "/api/logout"
        const options = {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({
                "RefreshToken": refreshToken
            } )
        };
        fetch( url, options )
            .then( response => {
                localStorage.removeItem("token");
                localStorage.removeItem("refreshToken");
            } );
    }
}

export async function refresh(){
    let token = localStorage.getItem("token");
    let refreshToken = localStorage.getItem("refreshToken");
    if (token && refreshToken) {
        let url = BackAddress + "/api/refresh"
        const options = {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({
                "RefreshToken": refreshToken
            } )
        };
        fetch( url, options )
            .then( response => response.json() )
            .then( response => {
                console.log( response );
                localStorage.setItem("token", response.token);
                localStorage.setItem("refreshToken", response.refreshToken);
            })
            .catch( error => {
                errorMessage.textContent = error.message;
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