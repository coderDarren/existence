'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_query)
{
    const _mobName = _query.mobName;

    if (!_mobName) return -1;

    return {
        mobName: _mobName
    };
}

const getMobLoot = async function(_query) {
    //console.log(`body: ${JSON.stringify(_body)}`);
    const _params = validateParams(_query);

    if (_params == -1) {
        return {
            error: "Invalid request query."
        }
    }

    const _sql = new SQL();    
    const _result = await _sql.getMobLoot(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = getMobLoot;