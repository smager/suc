(function (su) {

    if (!su) throw new Error("su-core.js is required.");

    const buildHeaders = () => {

        const headers = {
            "Content-Type": "application/json"
        };

        const token = localStorage.getItem(su.config.tokenKey);

        if (token) {
            headers.Authorization =
                `Bearer ${token}`;
        }

        return headers;
    };

    su.api = {

        async post(url, data = {}) {

            const res =
                await fetch(url, {
                    method: "POST",
                    headers: buildHeaders(),
                    body: JSON.stringify(data)
                });

            if (!res.ok) {
                throw new Error(
                    `HTTP ${res.status}`
                );
            }

            return await res.json();
        },

        async get(url) {

            const res =
                await fetch(url, {
                    headers: buildHeaders()
                });

            if (!res.ok) {
                throw new Error(
                    `HTTP ${res.status}`
                );
            }

            return await res.json();
        },



        async execute(request) {
            var clientInfo = su.getClientConfig();
            return await this.post(
                clientInfo.apiUrl + su.config.executeUrl,
                request
            );
        }

        ,async sendEmail(request ) {
            var clientInfo = su.getClientConfig();
            return su.api.post( clientInfo.apiUrl + "/email/send",request);
        }

        ,async getNewAccessToken( ) {
            var refreshToken =  localStorage.getItem("8dsfgjm3450f");
            if( refreshToken == null ) location.replace("/login.html");
            var clientInfo = su.getClientConfig();
            refreshToken = await this.post(
                clientInfo.apiUrl + "/client/refresh-token",
                { refreshToken: refreshToken }
            );
            localStorage.setItem("8dsfgjm3450f", refreshToken);
            return refreshToken;
        }

    }

})(window.su);