'use strict'
const SQL = require('../services/sql.js');

const validateParams = function(_body)
{
    if (!_body) return -1;
    const _name = _body.name;
    const _account = _body.account;
    if (!_name || !_account) return -1;
    return {
        name: _name,
        account: _account
    };
}

const createPlayer = async function(_body) {
    console.log(`body: ${JSON.stringify(_body)}`);
    const _params = validateParams(_body);

    if (_params == -1) {
        return {
            error: "Invalid request body. Expecting 'name'."
        }
    }

    const _sql = new SQL();    
    if (!(await _sql.connect())) {
        return {
            error: "Failed to connect to db."
        }
    }
    
    const _result = await _sql.createPlayer(_params.account, _params.name);
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = createPlayer