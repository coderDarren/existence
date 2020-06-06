'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_body)
{
    const _account = _body.id;
    const _apiKey = _body.apiKey;
    const _method = _body.method;
    const _mob = _body.mob;
    var _job = {};

    if (_body.method == "c") {
        
    } else if (_body.method == "u") {
        if (!_body.mobName) return -1;
        _job.mobName = _body.mobName;
    }

    if (!_account || !_apiKey || !_method || !_mob) return -1;

    return {
        account: _account,
        apiKey: _apiKey,
        method: _method,
        mob: _mob,
        job: _job,
    };
}

const modifyMob = async function(_body) {
    //console.log(`body: ${JSON.stringify(_body)}`);
    const _params = validateParams(_body);

    if (_params == -1) {
        return {
            error: "Invalid request body."
        }
    }

    const _sql = new SQL();    
    const _result = await _sql.modifyMob(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = modifyMob;