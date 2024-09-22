const { createProxyMiddleware } = require('http-proxy-middleware');
const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:22536';

const context =  [
  "/api",
  "/graphql",
];

module.exports = function(app) {
  const appProxy = createProxyMiddleware(context, {
    target: target,
    ws: true, // Enable WebSocket proxying
    secure: false, // Set to true in production
    changeOrigin: true, // Necessary for virtual hosted sites
  });

  app.use(appProxy);
};
