(function (window) {

    const su = {

        version: "1.0.0",

        config: {
            baseUrl: "",
            apiUrl: "",
            tokenKey: "su_token"
        },

        ready(fn) {

            if (document.readyState === "loading") {
                document.addEventListener("DOMContentLoaded", fn);
            }
            else {
                fn();
            }

        },

        setConfig(options = {}) {

            Object.assign(
                this.config,
                options
            );

        }

    };

    window.su = su;

})(window);