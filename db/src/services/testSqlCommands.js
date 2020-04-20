const SQL = require('../services/sql.js');

const _sql = new SQL();

const testUpdateStats = async function(_stats) {
    const _resp = await _sql.updateStats(_stats);
    return _resp;
}

const runTests = async function() {
    await _sql.connect();

    var _resp = await testUpdateStats({
        ID: 45,
        fortitude: 3,
        nanoResist: 12
    });
    if (_resp.error) console.log(`error on [testUpdateStats]: ${_resp.error}`)
}

runTests();