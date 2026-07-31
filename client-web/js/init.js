(function (global) {
    "use strict";
    const su = Object.create(null);
    // private
    const modules = Object.create(null);
    su.register = function (name, module) {

        if (modules[name])
            throw new Error(`Module '${name}' already exists.`);

        modules[name] = module;
        su[name] = module;
    };
    global.su = su;

})(globalThis);


(function (global) {    
    const exports = {
         version: "1.0.0"
        ,async init() {
            this.loadCss("https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css");
            this.loadCss("https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.7.2/css/all.min.css");
            this.loadCss("css/site.css");
            await this.loadScript("https://code.jquery.com/jquery-3.7.1.min.js");
            await this.loadScript("https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/js/bootstrap.bundle.min.js");
            await this.loadScript("js/su-core.js");
            await this.loadScript("js/su-uiBuilder.js");
            await this.loadScript("js/su-api.js");
            await this.loadScript("js/su-route.js");
            await su.router.load("js/su-routes.json");
            
            su.router.start();
            await this.loadScript("js/su-msgBox.js");

            this.applyTheme();
        }
        ,applyTheme() {
            const prefersDark = window.matchMedia("(prefers-color-scheme: dark)");
            document.documentElement.setAttribute("data-bs-theme", prefersDark.matches ? "dark" : "light");
            prefersDark.addEventListener("change", this.applyTheme.bind(this));
        }
        ,async loadScript(src) {
            return new Promise((resolve, reject) => {
                const s = document.createElement("script");
                s.src = `${src}?t=${Date.now()}`;
                s.onload = resolve;
                s.onerror = reject;
                document.body.appendChild(s);
            });
        },

        async loadCss(url) {
            return new Promise((resolve, reject) => {
                // Prevent duplicate loading
                const existing = document.querySelector(`link[href*="${url}"]`);

                if (existing) {
                    resolve();
                    return;
                }

                const link = document.createElement("link");
                link.rel = "stylesheet";
                link.href = url;

                link.onload = resolve;
                link.onerror = reject;

                document.head.appendChild(link);
            });
        }
    };
    global.loader = exports; 
})(su);

su.loader.init();
