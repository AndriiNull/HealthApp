const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
    ? env.ASPNETCORE_URLS.split(';')[0]
    : 'https://localhost:7212';

const PROXY_CONFIG = [
  {
    context: [
      "/api"  // ✅ This ensures all API routes go to the backend
    ],
    target,
    secure: false,
    changeOrigin: true,
    logLevel: "debug"  // ✅ Logs requests in the console for debugging
  }
];

module.exports = PROXY_CONFIG;
