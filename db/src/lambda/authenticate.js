'use strict'
const SQL = require('../services/sql.js');
const {encrypt} = require('../util/crypto.js');

const validateParams = function(_params)
{
    if (!_params) return -1;
    const _username = _params['username'];
    const _password = _params['password'];
    if (!_username || !_password) return -1;

    const _apiKey = encrypt(`${_username}&${_password}`);

    return {
        username: _username,
        password: _password,
        apiKey: _apiKey
    };
}

const authenticate = async function(_body) {
    //console.log(`query: ${JSON.stringify(_body)}`);
    const _params = validateParams(_body);

    if (_params == -1) {
        return {
            error: "Invalid request body."
        }
    }

    const _sql = new SQL();    
    const _result = await _sql.authenticate(_params);
    await _sql.close();
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = authenticate