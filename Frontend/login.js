import {  } from "./common"

async function login() {
    let login = document.getElementById('username');
    let password = document.getElementById('password');

    let l = login.value;
    let p = password.value;

    url = HostAddress + "/api/login"
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
        .then( response => response.json() )
        .then( response => {
            localStorage.setItem("token", response.token);
            window.location.href = FrontAddress + "index.html";
        } );
}