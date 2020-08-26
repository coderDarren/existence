'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_body)
{
    const _account = _body.id;
    const _apiKey = _body.apiKey;
    const _playerID = _body.playerID;
    const _transactionId = _body.transactionId;
    const _sell = _body.sell != undefined ? _body.sell : [];
    const _buy = _body.buy != undefined ? _body.buy : [];

    if (!_account || !_apiKey || !_playerID || !_transactionId) return -1;

    return {
        account: _account,
        apiKey: _apiKey,
        playerID: _playerID,
        transactionId: _transactionId,
        sell: _sell,
        buy: _buy
    };
}

const shopTerminalTrade = async function(_body) {
    //console.log(`body: ${JSON.stringify(_body)}`);
    const _params = validateParams(_body);

    if (_params == -1) {
        return {
            error: "Invalid request body."
        }
    }

    const _sql = new SQL();    
    const _result = await _sql.shopTerminalTrade(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = shopTerminalTrade;