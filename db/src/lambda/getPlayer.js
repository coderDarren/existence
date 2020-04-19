'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_body)
{
    if (!_body) return -1;
    const _player = _body['player'];
    if (!_player) return -1;
    return {
        player: _player
    };
}

const getPlayer = async function(_query) {
    console.log(`query: ${JSON.stringify(_query)}`);
    const _params = validateParams(_query);

    if (_params == -1) {
        return {
            error: "Invalid query params. Expecting 'player'."
        }
    }

    const _sql = new SQL();    
    if (!(await _sql.connect())) {
        return {
            error: "Failed to connect to db."
        }
    }

    const _result = await _sql.getPlayer(_params.player);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

const testGetPlayer = async function() {
    const _sql = new SQL();
    //await _sql.connect();
    //await _sql.createArmor();
    //await _sql.getArmor();
}
//testGetPlayer();

module.exports = getPlayer