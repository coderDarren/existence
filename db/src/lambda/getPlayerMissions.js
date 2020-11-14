'use strict'
const {map} = require('lodash');
const DynamoDB = require('../services/dynamodb.js');

const validateParams = function(_query)
{
    if (!_query) return -1;
    const _player = _query['player'];
    if (!_player) return -1;
    return {
        player: _player
    };
}

const getPlayerMissions = async function(_query) {
    console.log(`query: ${JSON.stringify(_query)}`);
    const _params = validateParams(_query);

    if (_params == -1) {
        return {
            error: "Invalid query params."
        }
    }

    const _dydb = new DynamoDB();
    var _result = await _dydb.queryTable('missions', _params);
    if (_result.error) {
        return _result;
    }
    _result = map(_result.Items, _i => { return JSON.parse(_i.data.S); });

    return {
        data: _result
    }
}

// const testGetPlayerMissions = async function() {
//     const _missions = await getPlayerMissions({player:'darren01'});
//     console.log(JSON.stringify(_missions));
// }
// testGetPlayerMissions();

module.exports = getPlayerMissions;