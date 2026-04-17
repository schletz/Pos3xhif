// src/ssh2-mock.js
// This is an empty mock so that Webpack ignores the native ssh2 module.
// Since we only use the local Docker daemon, this code will never be called.
module.exports = {
    Client: function() {}
};