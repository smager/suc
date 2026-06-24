(function (window) {
    const su = {
         version: "1.0.0"
        ,config: {
             baseUrl        : "/"
            ,apiUrl         : "https://localhost:7129"
            ,tokenKey       : "accessToken"
            ,getDataUrl     : "/client/data/getdata"
            ,executeCmdUrl  : "/client/data/executecmd"            
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

            this.config =
                await response.json();

            return this.config;
        }
        ,

        setConfig(options = {}) {

            Object.assign(
                this.config,
                options
            );

        }
        ,removeClient() {
            localStorage.removeItem("d257c9nzi2ke");
            localStorage.removeItem("t1685x3kw44p");
        }

        ,setClient(data) {
            const { ClientId, ApiKey } = data || {};
            localStorage.setItem(
                "d257c9nzi2ke",
                ClientId
            );
            localStorage.setItem(
                "t1685x3kw44p",
                ApiKey
            );
        }
        ,getClient() {
            return {
                clientId: localStorage.getItem("d257c9nzi2ke"),
                apiKey: localStorage.getItem("t1685x3kw44p")
            };
        }

    };

    window.su = su;
    const clientInfo = su.getClient();
    if( clientInfo.clientId == null || clientInfo.apiKey == null ){
        var config = su.loadConfig().then((config) => {
            su.setClient(config);
            console.log("Config loaded:", config);
        });
    };

})(window);