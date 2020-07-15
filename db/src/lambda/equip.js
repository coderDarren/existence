'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_body)
{
    const _account = _body.id;
    const _apiKey = _body.apiKey;
    const _playerID = _body.playerID;
    const _itemID = _body.itemID;
    const _inventoryLoc = _body.inventoryLoc

    if (_account == undefined || !_apiKey || _playerID == undefined || _itemID == undefined || _inventoryLoc == undefined) return -1;

    return {
        account: _account,
        apiKey: _apiKey,
        playerID: _playerID,
        itemID: _itemID,
        inventoryLoc: _inventoryLoc
    };
}

const equip = async function(_body) {
    //console.log(`body: ${JSON.stringify(_body)}`);
    const _params = validateParams(_body);

    if (_params == -1) {
        return {
            error: "Invalid request body."
        }
    }

    const _sql = new SQL();    
    const _result = await _sql.equip(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = equip;