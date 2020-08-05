'use strict'
const {
    response,
} = require('../util/response.js');
const getPlayer = require('./getPlayer.js');
const createPlayer = require('./createPlayer.js');
const updateStats = require('./updateStats.js');
const updateSessionData = require('./updateSessionData.js');
const updatePlayer = require('./updatePlayer.js');
const updateInventory = require('./updateInventory.js');
const addInventory = require('./addInventory.js');
const createAccount = require('./createAccount.js');
const authenticate = require('./authenticate.js');
const getAccountPlayers = require('./getAccountPlayers.js');
const modifyItem = require('./modifyItem.js');
const getMobLoot = require('./getMobLoot.js');
const getItem = require('./getItem.js');
const modifyElement = require('./modifyElement.js');
const equip = require('./equip.js');
const unequip = require('./unequip.js');

const versionCode = '0.1';
const api = async function(_event, _context) {
    console.log(JSON.stringify(_event));
    const _method = _event.httpMethod;
    const _path = _event.path;
    const _query = _event.queryStringParameters;
    const _body = _event.body ? JSON.parse(_event.body) : {};
    const _req = {
        context: _context,
        method: _method,
        path: _path,
        query: _query,
        body: _body
    }
    
    await handleRoute(_req);
}

const handleRoute = async function(_req) {
    var _resp = {};

    switch (_req.path) {
        case "/api":
        case "/api/":
            _req.context.succeed(response(200, `Existence API v${versionCode}`));
            break;
        case "/api/createAccount": // POST
            _resp = await createAccount(_req.body);
            break;
        case "/api/authenticate": // POST
            _resp = await authenticate(_req.body);
            break;
        case "/api/getAccountPlayers": // GET
            _resp = await getAccountPlayers(_req.query);
            break;
        case "/api/getPlayer": // GET
            _resp = await getPlayer(_req.query);
            break;
        case "/api/createPlayer": // POST
            _resp = await createPlayer(_req.body);
            break;
        case "/api/updatePlayer": // POST
            _resp = await updatePlayer(_req.body);
            break;
        case "/api/updateStats": // POST
            _resp = await updateStats(_req.body);
            break;
        case "/api/updateSessionData": // POST
            _resp = await updateSessionData(_req.body);
            break;
        case "/api/updateInventory": // POST
            _resp = await updateInventory(_req.body);
            break;
        case "/api/addInventory": // POST
            _resp = await addInventory(_req.body);
            break;
        case "/api/modifyItem": // POST
            _resp = await modifyItem(_req.body);
            break;
        case "/api/getMobLoot": // GET
            _resp = await getMobLoot(_req.query);
            break;
        case "/api/getItem": // GET
            _resp = await getItem(_req.query);
            break;
        case "/api/modifyElement": // POST
            _resp = await modifyElement(_req.body);
            break;
        case "/api/equip": // POST
            _resp = await equip(_req.body);
            break;
        case "/api/unequip": // POST
            _resp = await unequip(_req.body);
            break;
    }

    handleResponse(_resp, _req.context);
}

const handleResponse = function(_resp, _context) {
    if (_resp.error) {
        const _code = _resp.code ? _resp.code : 502;
        _context.succeed(response(_code, _resp.error));
    } else {
        _context.succeed(response(200, _resp.data))
    }
}

module.exports = api;