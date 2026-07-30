(function () {
    const exports = {
         version: "1.0.0"
        ,config: {
             baseUrl    : window.location.origin  + "/#/"
            ,executeUrl : "/client/data/execute"            
        }

        ,ready(fn) {

            if (document.readyState === "loading") {
                document.addEventListener("DOMContentLoaded", fn);
            }
            else {
                fn();
            }

        },
         async loadConfig() {
            const response =
                await fetch(
                    `config.json?v=${Date.now()}`
                );

            if (!response.ok) {
                throw new Error(
                    "Unable to load config.json"
                );
            }

            this.setConfig(await response.json());
            
            return this.config;
        }
        
        , setConfig(options = {}) {
            Object.assign(
                this.config,
                options
            );

        }
        ,removeClient() {
            localStorage.removeItem("t1685x3kw44p");
            localStorage.removeItem("d4d45f5f84kd");
        }

        ,setClientConfig(data) {
            const { ApiKey,ApiUrl } = data || {};
            localStorage.setItem(
                "t1685x3kw44p",
                ApiKey
            );
            localStorage.setItem(
                "d4d45f5f84kd",
                ApiUrl
            );            
        }
        ,getClientConfig() {
            return {
                  apiKey: localStorage.getItem("t1685x3kw44p"),
                  apiUrl: localStorage.getItem("d4d45f5f84kd")
            };
        }
        ,async getHtmlTemplate(url) {
              const response =await fetch(url);
             const html =await response.text();
             return html;
        }

        ,async getTemplate(url, data) {
            var source = await su.getHtmlTemplate(url);
            var template = Handlebars.compile(source);
       
            var html = template(data);
            return html;
        }        

    };

    Object.assign(su,exports);

    const clientInfo = su.getClientConfig();
    if( clientInfo.clientId == null || clientInfo.apiKey == null || clientInfo.apiUrl   == null ){
        var config = su.loadConfig().then((config) => {
            su.setClientConfig(config);
           // console.log("Config loaded:", config);
        });
    };

})();