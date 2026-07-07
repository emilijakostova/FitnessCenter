const { defineConfig } = require('cypress');

module.exports = defineConfig({
  e2e: {
    baseUrl: 'https://localhost:44392',
    chromeWebSecurity: false,
    viewportWidth: 1280,
    viewportHeight: 720,
  },
});