'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_query)
{
    const _id = _query['id'];
    if (!_id) return -1;

    return {
        id: _id,
        simple: _query['simple']
    };
}

const getMob = async function(_query) {
    //console.log(`query: ${JSON.stringify(_query)}`);
    const _params = validateParams(_query);

    if (_params == -1) {
        return {
            error: "Invalid query params."
        }
    }

    const _sql = new SQL();
    const _result = await _sql.getMob(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = getMob;