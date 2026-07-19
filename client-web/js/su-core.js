(function (window) {
    const su = {
         version: "1.0.0"
        ,config: {
             baseUrl        : window.location.origin 
            ,executeUrl  : "/client/data/execute"            
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
        
        , setConfig(options = {}) {
            Object.assign(
                this.config,
                options
            );

        }
        ,removeClient() {
            localStorage.removeItem("d257c9nzi2ke");
            localStorage.removeItem("t1685x3kw44p");
            localStorage.removeItem("d4d45f5f84kd");
        }

        ,setClientConfig(data) {
            const { ClientId, ApiKey,ApiUrl } = data || {};
            localStorage.setItem(
                "d257c9nzi2ke",
                ClientId
            );
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
                clientId: localStorage.getItem("d257c9nzi2ke"),
                  apiKey: localStorage.getItem("t1685x3kw44p"),
                  apiUrl: localStorage.getItem("d4d45f5f84kd")
            };
        }
        ,getUrlParamValue(variable) {
        	var source = window.location.href; 
        	variable = variable.toLowerCase();
        	var qLoc =  source.indexOf("?");
        	if (qLoc >-1){
        			var param = source.split("?");
        		var result = param[1];  //right parameters  
        		if (result.indexOf("&") > -1){ 
        	 
        			var vars = result.split("&");
        			for (var i=0;i<vars.length;i++) {
        				var pair = vars[i].split("=");
        				if (pair[0].toLowerCase() == variable.toLowerCase()) {
        	
        					return pair[1];
        				}
        			}
        		}
        		else{
        			var pair = result.split("=");
        			if (pair[0].toLowerCase() == variable.toLowerCase()) {
        				return pair[1];
        			}
        		}
        	}
        	return "";
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

    window.su = su;
    const clientInfo = su.getClientConfig();
    if( clientInfo.clientId == null || clientInfo.apiKey == null || clientInfo.apiUrl   == null ){
        var config = su.loadConfig().then((config) => {
            su.setClientConfig(config);
            console.log("Config loaded:", config);
        });
    };

})(window);