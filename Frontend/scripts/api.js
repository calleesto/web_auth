import { BackAddress} from "./common.js";

async function getPublic() {
    let url = BackAddress + "/api/public"
    let response = await fetch(url);
    return await response.json();
}

async function showPublic() {
    let response = await getPublic();
    let v1 = document.getElementById('value1');
    let v2 = document.getElementById('value2');
    v1.innerText = response[0];
    v2.innerText = response[1];
}

async function executeResourceAction({ method, endpoint, inputId, outputId, successMessage }) {
    const output = document.getElementById(outputId);
    const token = localStorage.getItem("token");

    let url = BackAddress + endpoint;
    if (inputId) {
        const input = document.getElementById(inputId);
        url += input.value;
    }

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
            output.innerText = inputId ? "User not found." : "Resource not found.";
        }
        else if (response.ok) {
            if (method === "GET") {
                const data = await response.json();
                output.innerText = JSON.stringify(data);
            } else {
                output.innerText = successMessage;
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

document.getElementById("get-profile-btn").addEventListener("click", async () => {
    await executeResourceAction({
        method: "GET",
        endpoint: "/api/user/",
        inputId: "get-profile-input",
        outputId: "get-profile-output"
    });
});

document.getElementById("delete-profile-btn").addEventListener("click", async () => {
    await executeResourceAction({
        method: "DELETE",
        endpoint: "/api/user/",
        inputId: "delete-profile-input",
        outputId: "delete-profile-output",
        successMessage: "User deleted successfully."
    });
});

document.getElementById("submit-logs-btn").addEventListener("click", async () => {
    await executeResourceAction({
        method: "POST",
        endpoint: "/api/logs",
        outputId: "logs-output",
        successMessage: "Logs submitted successfully."
    });
});

await showPublic();