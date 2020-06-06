'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_body)
{
    const _account = _body.id;
    const _apiKey = _body.apiKey;
    const _method = _body.method;
    const _mobLoot = _body.mobLoot;
    var _job = {};

    if (!_account || !_apiKey || !_method || !_mobLoot) return -1;

    return {
        account: _account,
        apiKey: _apiKey,
        method: _method,
        mobLoot: _mobLoot,
        job: _job,
    };
}

const modifyMobLoot = async function(_body) {
    //console.log(`body: ${JSON.stringify(_body)}`);
    const _params = validateParams(_body);

    if (_params == -1) {
        return {
            error: "Invalid request body."
        }
    }

    const _sql = new SQL();    
    const _result = await _sql.modifyMobLoot(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = modifyMobLoot;