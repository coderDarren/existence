'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_body)
{
    if (!_body) return -1;
    const _itemID = _body.itemID;
    const _playerID = _body.playerID;
    const _account = _body.id;
    const _apiKey = _body.apiKey;
    if (!_account || !_apiKey || _itemID == undefined || !_playerID) return -1;
    return {
        itemID: _itemID,
        playerID: _playerID,
        account: _account,
        apiKey: _apiKey,
    };
}

const addInventory = async function(_body) {
    console.log(`body: ${JSON.stringify(_body)}`);
    const _params = validateParams(_body);

    if (_params == -1) {
        return {
            error: "Invalid request body."
        }
    }

    const _sql = new SQL();    
    const _result = await _sql.addInventory(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = addInventory