function saveCookie(cookieString) {
    console.log("Current cookies:" + decodeURIComponent(document.cookie));
    console.log("Saving: " + cookieString);
    document.cookie = cookieString;
    console.log("Cookies after save:" + decodeURIComponent(document.cookie));
}

function getCookie(name) {
    let match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
    return match ? match[2] : null;
}

async function postVoidAsync(url) {
    const betterUrl = url.replace("backend", "localhost"); // atsiprasau visu kas tai matys, nenaudokit blazoriaus.
    $.ajax({
        url: betterUrl,
        type: 'POST',
        contentType: 'application/json',
        xhrFields: {
            withCredentials: true
        },
        success: function (response) {
            console.log("Logout successful:", response);
        },
        error: function (xhr, status, error) {
            console.error("Logout failed:", status, error);
        }
    });
}

async function postLogin(url, email, password) {
    const formData =
    {
        email: email,
        password: password
    };

    const returnObj = {
        status: 401,
        message: ""
    }
    console.log("fetching from: ", url);

    const betterUrl = url.replace("backend", "localhost"); // atsiprasau visu kas tai matys, nenaudokit blazoriaus.

    return fetch(betterUrl, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(formData),
        credentials: "include"
    }).then(async response => {
        returnObj.message = await response.text();
        returnObj.status = response.status;
        return returnObj;
    }).catch(error => {
        console.error("Request failed", error);
        returnObj.message = "An unexpected error occured";
        return returnObj;
    });
}