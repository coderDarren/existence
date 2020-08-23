'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_query)
{
    const _simple = _query['simple'];
    const _shopBuyable = _query['shopBuyable'];
    const _itemType = _query['itemType'];

    return {
        simple: _simple,
        shopBuyable: _shopBuyable,
        itemType: _itemType
    };
}

const getItems = async function(_query) {
    //console.log(`query: ${JSON.stringify(_query)}`);
    const _params = validateParams(_query);

    if (_params == -1) {
        return {
            error: "Invalid query params."
        }
    }

    const _sql = new SQL();
    const _result = await _sql.getItems(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = getItems