(function() {
    console.log("[custom-auth.js]: Running")
    
    const tokenKey = "jwt_token"; // Key to store the token in localStorage

    // Intercept API responses
    const originalFetch = window.fetch;
    window.fetch = async (url, options) => {
        const response = await originalFetch(url, options);

        if (url.includes("/api/v1/identity/login") || url.includes("/api/v1/identity/signup")) {
            const data = await response.clone().json();
            if (data.token) {
                localStorage.setItem(tokenKey, data.token); // Store token
                setSwaggerAuth(data.token); // Auto-set token in Swagger UI
            }
        }

        return response;
    };

    // Function to set token in Swagger UI
    function setSwaggerAuth(token) {
        const bearerToken = "Bearer " + token;
        window.localStorage.setItem("jwt_token", bearerToken); // Store in localStorage
        const ui = window.ui || window.swaggerUIBundle || window.swaggerUi;
        if (ui) {
            ui.preauthorizeApiKey("Bearer", bearerToken);
        }
    }

    // Set token from localStorage on page load
    document.addEventListener("DOMContentLoaded", () => {
        const savedToken = localStorage.getItem(tokenKey);
        if (savedToken) {
            setSwaggerAuth(savedToken);
        }
    });
})();
