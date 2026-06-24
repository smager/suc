window.su = {

    async init() {
        await this.load("https://code.jquery.com/jquery-3.7.1.min.js");
        await this.load("https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/js/bootstrap.bundle.min.js");
        await this.load("js/su-core.js");
        await this.load("js/su-api.js");
        await this.load("js/route.js");

    },

    load(src) {

        return new Promise((resolve, reject) => {

            const s = document.createElement("script");
            s.src = `${src}?t=${Date.now()}`;
            s.onload = resolve;
            s.onerror = reject;
            document.body.appendChild(s);
        });

    }

};

su.init();
