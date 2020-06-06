'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_body)
{
    const _account = _body.id;
    const _apiKey = _body.apiKey;
    const _method = _body.method;
    const _item = _body.item;
    var _job = {};

    if (_body.method == "c") {
        if (!_body.requirements || !_body.effects || !_body.item) return -1;
        _job.requirements = _body.requirements;
        _job.effects = _body.effects;
    } else if (_body.method == "u") {
        if (!_body.itemName) return -1;
        _job.itemName = _body.itemName;
    }

    if (!_account || !_apiKey || !_method || !_item) return -1;

    return {
        account: _account,
        apiKey: _apiKey,
        method: _method,
        item: _item,
        job: _job,
        requirements: _body.requirements,
        effects: _body.effects
    };
}

const modifyItem = async function(_body) {
    //console.log(`body: ${JSON.stringify(_body)}`);
    const _params = validateParams(_body);

    if (_params == -1) {
        return {
            error: "Invalid request body."
        }
    }

    const _sql = new SQL();    
    const _result = await _sql.modifyItem(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = modifyItem;