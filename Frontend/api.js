// todo fix import should
//import { BackAddress } from "common"

const BackAddress = "http://localhost:9002"

async function logout() {
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

async function getPublic() {
    let url = BackAddress + "/api/public"
    let response = await fetch(url);
    return await response.json();
}

async function showPublic() {
    let response = await getPublic();
    console.log(response);
    let data = document.getElementById('data');
    data.innerText = response;
}