'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_body)
{
    if (!_body) return -1;
    if (!_body.ID) return -1;
    return _body;
}

const updatePlayer = async function(_body) {
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
    
    const _result = await _sql.updatePlayer(_params);
    if (_result.error) {
        return _result;
    }

    return {
        data: {status: "Success"}
    }
}

module.exports = updatePlayer