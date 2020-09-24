'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_body)
{
    const _account = _body.id;
    const _apiKey = _body.apiKey;
    const _method = _body.method;
    const _table = _body.table;
    const _element = _body.element;
    const _elementKey = _body.elementKey;
    const _strictKeys = _body.strictKeys;

    if (!_account || !_apiKey || !_method || !_element || !_elementKey || !_table) return -1;

    return {
        account: _account,
        apiKey: _apiKey,
        method: _method,
        element: _element,
        elementKey: _elementKey,
        table: _table,
        strictKeys: _strictKeys
    };
}

const modifyElement = async function(_body) {
    //console.log(`body: ${JSON.stringify(_body)}`);
    const _params = validateParams(_body);

    if (_params == -1) {
        return {
            error: "Invalid request body."
        }
    }

    const _sql = new SQL();    
    const _result = await _sql.modifyElement(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = modifyElement;