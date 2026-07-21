(function (global) {
    const exports = {
         version: "1.0.0"
        ,dialog: null,
        async init() {
            if (this.msgBox)
                return;
            const html = await su.getHtmlTemplate("p/templates/msg-box.html");
            document.body.insertAdjacentHTML("beforeend", html);
            this.msgBox = new bootstrap.Modal(
                document.getElementById("bsMsgBox")
            );
            this.title = document.getElementById("msgTitle");
            this.body = document.getElementById("msgBody");
        },

        async show(title, message) {
            await this.init();
            this.title.innerHTML = title;
            this.body.innerHTML = message;
            this.msgBox.show();

        },

        async success(message) {
            await this.show("Success", message);

        },

        async error(message) {
            await this.show("Error", message);

        },

        async info(message) {
            await this.show("Infomation", message);
        }
    };
    if (!global) throw new Error("su-core.js is required.");
    global.msgBox = exports;     
})(su);