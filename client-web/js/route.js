 
class Router {
    constructor() {
        this.currentIndex=0;
        window.addEventListener(
            "hashchange",
            () => this.load()
        );

        window.addEventListener(
            "load",
            () => this.load()
        );

        //prevent from saving in history
        document.addEventListener(
            "click",
            e => this.handleScrollClick(e)
        );

    }
    handleScrollClick(e) {
        const id =e.target.dataset.scroll;

        if (!id) return;
        e.preventDefault();

        document
            .getElementById(id)
            ?.scrollIntoView({
                behavior: "smooth"
            });

    }    

    async load() {
        // Handle section hashes
        if (  ! location.hash.startsWith("#/")) {

            const section = location.hash.substring(1);

            const el = document.getElementById(section);

            if (el) {

                el.scrollIntoView({
                    behavior: "smooth"
                });

                return;
            } else{
                // console.log("history.state",history.state)
                history.go(-1);      
            }

        } 
    
        let route =location.hash.replace("#/", "");
    
        switch (route) {

            case "":
            case "home":
                await this.render("pages/home.html");
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

        try {


            
            const response =await fetch(url);
            const html =await response.text();
            const app = document.querySelector("#app");
            app.innerHTML = html;

            app.querySelectorAll("script")
                .forEach(oldScript => {
                    const script = document.createElement("script");
                    script.text = oldScript.textContent;
                    app.appendChild(script);
                    app.removeChild(script);

            });
                    

        }
        catch (err) {

            console.error(err);

            document
                .querySelector("#app")
                .innerHTML =
                `
                <div class="alert alert-danger">
                    ${err.message}
                </div>
                `;
        }
    }
}

new Router();

 