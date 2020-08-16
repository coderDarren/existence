'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_query)
{
    if (!_query) return -1;

    return {
    };
}

const getMobs = async function(_query) {
    //console.log(`query: ${JSON.stringify(_query)}`);
    const _params = validateParams(_query);

    if (_params == -1) {
        return {
            error: "Invalid query params."
        }
    }

    const _sql = new SQL();
    const _result = await _sql.getMobs(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = getMobs