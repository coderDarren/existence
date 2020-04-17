const Cryptr = require('cryptr')
const secret = process.env.CRYPTO_SECRET;

const encrypt = function(_data) {
    return new Cryptr(secret).encrypt(_data);
}

const decrypt = function(_hash) {
    return new Cryptr(secret).decrypt(_hash);
}

module.exports = {
    encrypt,
    decrypt
}