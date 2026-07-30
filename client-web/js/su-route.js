(function () {

    class Router {

        constructor() {
            this._routes = {};
            this._current = null;
        }
        get params() {
            return this._current?.params ?? {};
        }

        routes(routes) {
            this._routes = {};
            for (const route of routes) {
                this._routes[route.page] = route;
                if (route.default)
                    this._defaultPage = route.page;
            }

            this._defaultPage ??= "home";
            return this;
        }

        async load(url) {
            const response = await fetch(url);
            const routes = await response.json();
            this.routes(routes);
            return this;
        }

        current() {
            return this._current;
        }

        start() {
            window.addEventListener("hashchange", () => this.navigate());
            this.navigate();
        }

        async navigate(url = location.hash) {
            //console.log("Navigate:", url);
            this._current = this.parse(url);
            //console.log(this._current);
            await this.render();
        }

        async render() {
            const route = this._routes[this._current.page.name];
            if (!route) {
                console.error("Route not found:", this._current.page.name);
                return;
            }

            if (route && route.params) {
                this._current.params = {};
                route.params.forEach((name, index) => {
                    this._current.params[name] = this._current.page.params[index];
                });
            }

            //console.log("Render:", route);

            //let response = await fetch(`p/${route.view}.html`);
            let response = await fetch(`p/${route.view}.html?t=${Date.now()}`);     

            if (response.status === 404)
                response = await fetch("p/404.html");
            const html = await response.text();
            const app = document.getElementById("app");
            app.innerHTML = html;
            // Execute scripts inside loaded html
            app.querySelectorAll("script").forEach(oldScript => {
                const script = document.createElement("script");
                if (oldScript.src)
                    script.src = oldScript.src;
                else
                    script.textContent = oldScript.textContent;
                document.body.appendChild(script);
                script.remove();
            });

            // Scroll section
            if (this._current.sections.length) {
                const section = this._current.sections.at(-1).name;
                document
                    .getElementById(section)
                    ?.scrollIntoView({
                        behavior: "smooth"
                    });

            }
        }

        parse(url) {
            if (!url || url === "#") 
                url = "/";

            url = url.replace(/^#/, "");

            if (url === "")
                url = "/";

            const result = {
                page: null,
                params: {},
                sections: [],
                modals: [],
                tabs: []
            };

            const tokens = url
                .split("/")
                .filter(Boolean);

            // Default page
            if (tokens.length === 0) {

                result.page = {
                    name: this._defaultPage,
                    params: []
                };
                return result;
            }

            for (const token of tokens) {
                if (token.startsWith("modals:")) {
                    result.modals = this.parseItems(token.substring(7));
                    continue;
                }
                if (token.startsWith("tabs:")) {
                    result.tabs = this.parseItems(token.substring(5));
                    continue;
                }
                if (!result.page) {
                    result.page = this.parseNode(token);
                    continue;
                }
                result.sections.push(this.parseNode(token));
            }
            return result;
        }        
      
        parseNode(text) {

            const node = {
                name: "",
                params: []
            };
            const open = text.indexOf("[");
            if (open < 0) {
                node.name = text.replace(":", "");
                return node;
            }
            node.name = text.substring(0, open).replace(":", "");
            const close = text.lastIndexOf("]");
            if (close > open) {
                const value = text.substring(open + 1, close);
                if (value.length)
                    node.params = value.split("|");
            }
            return node;
        }
        parseItems(text) {
            if (!text)
                return [];
            return text
                .split(",")
                .map(x => this.parseNode(x));
        }

    }
    su.router = new Router();
})();