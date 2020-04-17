const Cryptr = require('cryptr')
const secret = process.env.CRYPTO_SECRET || 'nfg-123';
const cryptr = new Cryptr(secret);

const encrypt = function(_data) {
    return cryptr.encrypt(_data);
}

const decrypt = function(_hash) {
    return cryptr.decrypt(_hash);
}

module.exports = {
    encrypt,
    decrypt
}