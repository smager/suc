 
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

        if( route.indexOf("?") > -1 )
            route = route.substring( 0,route.indexOf("?"));

        if(route =="") route ="/p/home";        
        await this.render(route +".html");
    }
    
    async render(url, params) {


        try {
            var response =await fetch(url);
            if (response.status === 404) {
                response =await fetch("pages/404.html");                
            }
            console.log("response",response)

            var html =await response.text();
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

 