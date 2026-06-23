class Router {

    constructor() {

        window.addEventListener(
            "hashchange",
            () => this.load()
        );

        window.addEventListener(
            "load",
            () => this.load()
        );

    }

    async load() {
        /*

        let route =
            location.hash || "#/dashboard";

        route = route.replace("#/", "");

        */

        let route = location.hash.replace("#/", "");

        switch(route){

            case "":
            case "home":
                await this.render("index.html");
                break;

            case "auth/login":
                await this.render("pages/auth/login.html");
                break;

            case "auth/register":
                await this.render("pages/auth/register.html");
                break;

            case "terms":
                await this.render("pages/terms.html");
                break;

            default:
                await this.render("pages/404.html");
                break;
        }        


 
    }

    async render(url, params) {

        let html = await $.get(url);

        $("#app").html(html);

        if (window.PageInit)
            window.PageInit(params);
    }

}

new Router();