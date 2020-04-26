'use strict'
const {
    response,
    requestSuccess,
    badRequest
} = require('../util/response.js');
const getPlayer = require('./getPlayer.js');
const createPlayer = require('./createPlayer.js');
const updateStats = require('./updateStats.js');
const updatePlayer = require('./updatePlayer.js');

const versionCode = 0.1;
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
        case "/api/getPlayer":
            _resp = await getPlayer(_req.query);
            break;
        case "/api/createPlayer":
            _resp = await createPlayer(_req.body);
            break;
        case "/api/updatePlayer":
            _resp = await updatePlayer(_req.body);
            break;
        case "/api/updateStats":
            _resp = await updateStats(_req.body);
            break;
    }

    handleResponse(_resp, _req.context);
}

const handleResponse = function(_resp, _context) {
    if (_resp.error) {
        _context.succeed(response(502, _resp.error));
    } else {
        _context.succeed(response(200, _resp.data))
    }
}

module.exports = api;