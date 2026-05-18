import { BackAddress, FrontAddress} from "common.js"

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
    let v1 = document.getElementById('value1');
    let v2 = document.getElementById('value2');
    v1.innerText = response[0];
    v2.innerText = response[1];
}

async function adminGetProfile() {
    let input = document.getElementById('get-profile-input');
    let output = document.getElementById('get-profile-output');
    let token = localStorage.getItem("token");

    output.innerText = "";
    try {
        const response = await fetch(`${BackAddress}/api/user/${input.value}`, {
            method: 'GET',
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            }
        });
        if (response.status === 403) {
            output.innerText = "Forbidden. You are not an admin ad tried lookig at someone else's profile.";
        }
        else if (response.ok) {
            const data = await response.json();
            output.innerText = JSON.stringify(data);
        }
        else if (response.status === 404) {
            output.innerText = "User not found.";
        }
    }
    catch (error) {
        output.innerText = error.message;
    }
}
document.getElementById("get-profile-btn").addEventListener("click", adminGetProfile);