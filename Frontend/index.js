async function logout() {
    let token = localStorage.getItem("token");
    if (token) {
        url = BackAddress + "/api/logout"
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
                // todo do smth more
                console.log( response );
                localStorage.removeItem("token");
            } );
    }
}