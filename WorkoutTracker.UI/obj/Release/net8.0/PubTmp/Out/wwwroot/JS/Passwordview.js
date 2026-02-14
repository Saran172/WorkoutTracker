window.TogglePass = function (id) {
    const e = document.getElementById(id);

    if (!e) {
        alert("TogglePass: element not found", id);
        return null;
    }

    e.type = e.type === "password" ? "text" : "password";
    return e.type;
}
