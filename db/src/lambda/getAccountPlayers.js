'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_query)
{
    if (!_query) return -1;
    const _account = _query['account'];
    const _apiKey = _query['apiKey'];
    if (!_account || !_apiKey) return -1;
    return {
        account: _account,
        apiKey: _apiKey
    };
}

const getAccountPlayers = async function(_query) {
    console.log(`query: ${JSON.stringify(_query)}`);
    const _params = validateParams(_query);

    if (_params == -1) {
        return {
            error: "Invalid query params."
        }
    }

    const _sql = new SQL();    
    /*if (!(await _sql.connect())) {
        return {
            error: "Failed to connect to db."
        }
    }*/

    const _result = await _sql.getAccountPlayers(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = getAccountPlayers