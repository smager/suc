(function () {
    const exports = {
         version: "1.0.0"
        ,config: {
             baseUrl    : window.location.origin  + "/#/"
            //,executeUrl : "/client/data/execute"            
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

        ,template: { //html templating 
            _cache: Object.create(null)
            ,render(template, data) {
                return template.replace(/\{\{\s*([\w.]+)\s*\}\}/g, (_, path) => {
                    const value = path
                        .split(".")
                        .reduce((obj, key) => obj?.[key], data);
                    return value ?? "";
                });
            }
            
        }

        ,async getHtmlTemplate(url) {
            const cache = this.template._cache;
            if (cache[url])
                return cache[url];
            const response = await fetch(url);
            if ( ! response.ok)
                throw new Error(`Unable to load template '${url}'.`);
            return cache[url] = await response.text();
        }

        ,async getTemplate(url, data) {
            var source = await su.getHtmlTemplate(url);
            var html = su.template.render(source,data);       
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