(function (su) {

    if (!su)
        throw new Error(
            "su-core.js is required."
        );

    const buildHeaders = () => {

        const headers = {
            "Content-Type": "application/json"
        };

        const token =
            localStorage.getItem(
                su.config.tokenKey
            );

        if (token) {
            headers.Authorization =
                `Bearer ${token}`;
        }

        return headers;
    };

    su.api = {

        async post(url, data = {}) {

            const response =
                await fetch(url, {
                    method: "POST",
                    headers: buildHeaders(),
                    body: JSON.stringify(data)
                });

            if (!response.ok) {
                throw new Error(
                    `HTTP ${response.status}`
                );
            }

            return await response.json();
        },

        async get(url) {

            const response =
                await fetch(url, {
                    headers: buildHeaders()
                });

            if (!response.ok) {
                throw new Error(
                    `HTTP ${response.status}`
                );
            }

            return await response.json();
        },

        async getData(request) {

            return await this.post(
                su.config.getDataUrl,
                request
            );

        },

        async executeCmd(request) {

            return await this.post(
                su.config.executeCmdUrl,
                request
            );

        }

    };

})(window.su);