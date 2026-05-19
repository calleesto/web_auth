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

async function profileControl(method, inputId, outputId) {
    let input = document.getElementById(inputId);
    let output = document.getElementById(outputId);
    let token = localStorage.getItem("token");
    let url = BackAddress + "/api/user/" + input.value;

    output.innerText = "";

    try {
        const response = await fetch(url, {
            method: method,
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            }
        });

        if (response.status === 403) {
            output.innerText = "Forbidden. You are not allowed to do this.";
        }
        else if (response.status === 404) {
            output.innerText = "User not found.";
        }
        else if (response.ok) {
            if (method === "GET") {
                const data = await response.json();
                output.innerText = JSON.stringify(data);
            }
            else if (method === "DELETE") {
                output.innerText = "User deleted successfully.";
            }
        }
        else {
            output.innerText = "Something went wrong.";
        }
    }
    catch (error) {
        output.innerText = error.message;
    }
}

function getProfile() {
    profileControl("GET", "get-profile-input", "get-profile-output");
}

function deleteProfile() {
    profileControl("DELETE", "delete-profile-input", "delete-profile-output");
}

async function submitLogs() {

}

document.getElementById("delete-profile-btn").addEventListener("click", deleteProfile)
document.getElementById("get-profile-btn").addEventListener("click", getProfile);