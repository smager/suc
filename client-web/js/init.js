window.su = {

    async init() {
        await this.loadCss("https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css");
        await this.loadCss("https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.7.2/css/all.min.css");
        await this.loadCss("css/site.css");

        await this.loadScript("https://cdnjs.cloudflare.com/ajax/libs/handlebars.js/4.7.8/handlebars.min.js");
        await this.loadScript("https://code.jquery.com/jquery-3.7.1.min.js");
        await this.loadScript("https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/js/bootstrap.bundle.min.js");
        await this.loadScript("js/su-core.js");
        await this.loadScript("js/su-api.js");
        await this.loadScript("js/su-route.js");
         await this.loadScript("js/su-msgbox.js");
    },
    async loadScript(src) {

        return new Promise((resolve, reject) => {

            const s = document.createElement("script");
            s.src = `${src}?t=${Date.now()}`;
            s.onload = resolve;
            s.onerror = reject;
            document.body.appendChild(s);
        });

    }
    ,async loadCss(url) {

        return new Promise((resolve, reject) => {

            // Prevent duplicate loading
            const existing =
                document.querySelector(
                    `link[href*="${url}"]`
                );

            if (existing) {
                resolve();
                return;
            }

            const link =document.createElement("link");

            link.rel = "stylesheet";
            link.href = url;

            link.onload = resolve;
            link.onerror = reject;

            document.head.appendChild(link);

        });

    },    

};

su.init();
