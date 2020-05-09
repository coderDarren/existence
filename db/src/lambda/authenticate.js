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

const authenticate = async function(_query) {
    console.log(`query: ${JSON.stringify(_query)}`);
    const _params = validateParams(_query);

    if (_params == -1) {
        return {
            error: "Invalid query params."
        }
    }

    const _sql = new SQL();    
    if (!(await _sql.connect())) {
        return {
            error: "Failed to connect to db."
        }
    }

    const _result = await _sql.authenticate(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = authenticate