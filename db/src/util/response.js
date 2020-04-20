'use strict'

const response = function(_status, _message)
{
    const _resp = {
        body: JSON.stringify({
            message: JSON.stringify(_message),
            statusCode: _status,
        }),
        headers: {
            'Access-Control-Allow-Origin': '*'
        },
    };
    return _resp;
}

const requestSuccess = response(200, "Request succeeded.");
const requestAccepted = response(202, "Request accepted.");
const badRequest = response(400, "Bad request.");
const requestForbidden = response(403, "Request forbidden.");
const serverError = response(502, "Server error.");

module.exports = {
    response,
    requestSuccess,
    requestAccepted,
    badRequest,
    requestForbidden,
    serverError
};