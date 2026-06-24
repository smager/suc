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

        async getData(request) {

            return await this.post(
                su.config.apiUrl + su.config.getDataUrl,
                request
            );

        },

        async executeCmd(request) {

            return await this.post(
                su.config.apiUrl + su.config.executeCmdUrl,
                request
            );

        }

    };

})(window.su);