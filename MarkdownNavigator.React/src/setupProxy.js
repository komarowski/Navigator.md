﻿const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function (app) {
    const appProxy = createProxyMiddleware('/api', {
        target: 'https://localhost:7024',
        secure: false,
    });

    app.use(appProxy);
};

