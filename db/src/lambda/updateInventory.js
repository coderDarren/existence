'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_body)
{
    if (!_body) return -1;
    const _slotID = _body.slotID;
    const _slotLoc = _body.slotLoc;
    const _playerID = _body.playerID;
    const _account = _body.id;
    const _apiKey = _body.apiKey;
    if (!_account || !_apiKey || _slotID == undefined || _slotLoc == undefined || !_playerID) return -1;
    return {
        slotID: _slotID,
        slotLoc: _slotLoc,
        playerID: _playerID,
        account: _account,
        apiKey: _apiKey,
    };
}

const updateInventory = async function(_body) {
    console.log(`body: ${JSON.stringify(_body)}`);
    const _params = validateParams(_body);

    if (_params == -1) {
        return {
            error: "Invalid request body."
        }
    }

    const _sql = new SQL();    
    if (!(await _sql.connect())) {
        return {
            error: "Failed to connect to db."
        }
    }
    
    const _result = await _sql.updateInventory(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: {status: "Success"}
    }
}

module.exports = updateInventory