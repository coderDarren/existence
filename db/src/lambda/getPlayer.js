'use strict'
const {
    response,
    requestSuccess,
    badRequest
} = require('../util/response.js');
const SQL = require('../services/sql.js');

const validateParams = function(_body)
{
    if (!_body) return -1;
    const _username = _body['username'];
    const _player = _body['player'];
    if (!_username || !_player) return -1;
    return {
        username: _username,
        player: _player
    };
}

const getPlayer = async function(_event, _context) {
    console.log(_event);
    console.log(`body: ${JSON.stringify(_event.body)}`);
    console.log(`query: ${JSON.stringify(_event.queryStringParameters)}`);
    const _params = validateParams(_event.queryStringParameters);

    if (_params == -1) {
        _context.succeed(badRequest);
        return;
    }

    const _sql = new SQL();
    const _didConnect = await _sql.connect();
    
    if (!_didConnect) {
        _context.succeed(response("1400", "Failed to connect to db."));
    }

    _context.succeed(requestSuccess);
}

const testGetPlayer = async function() {
    const _sql = new SQL();
    await _sql.connect();
}
testGetPlayer();

module.exports = getPlayer