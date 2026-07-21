(function (global) {
    const exports = {
     version: "1.0.0"
    ,clear(container) {

        container.querySelectorAll("input, textarea, select").forEach(el => {

            switch (el.type) {

                case "checkbox":
                case "radio":
                    el.checked = false;
                    break;

                case "file":
                    el.value = "";
                    break;

                default:

                    if (el.tagName === "SELECT")
                        el.selectedIndex = 0;
                    else
                        el.value = "";

                    break;
            }
        });

    }};

    if (!global) throw new Error("su-core.js is required.");
    global.form = exports; 
})(su);