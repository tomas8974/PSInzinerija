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