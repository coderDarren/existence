'use strict'
const SQL = require('../services/sql.js');
const {encrypt} = require('../util/crypto.js');

const validateParams = function(_body)
{
    if (!_body) return -1;
    const _first_name = _body.first_name;
    const _last_name = _body.last_name;
    const _username = _body.username;
    const _password = _body.password;
    const _email = _body.email;
    if (!_first_name || !_last_name ||
        !_username || !_password || !_email) return -1;
    
    const _apiKey = encrypt(`${_username}&${_password}`);

    return {
        first_name: _first_name,
        last_name: _last_name,
        username: _username,
        password: _password,
        email: _email,
        apiKey: _apiKey
    };
}

const enforceParamRules = function(_params) {
    const _letterRegex = '^[a-zA-Z ]*$';
    const _emailRegex = /^\S+@\S+$/;

    // enforce username
    if (_params.username.length < 5) {
        return {error: 'Username must be at least 5 characters', code: 1400}
    }
    if (!_params.username[0].match(_letterRegex)) {
        return {error: 'Username cannot begin with a number or special character', code: 1401}
    }

    // enforce password
    if (_params.password.length < 8) {
        return {error: 'Password must be at least 8 characters', code: 1402}
    }
    if (!_params.password[0].match(_letterRegex)) {
        return {error: 'Password cannot begin with a special character or a number', code: 1403}
    }

    // enforce first and last name
    if (!_params.first_name.match(_letterRegex)) {
        return {error: 'First name cannot contain special characters or numbers', code: 1404}
    }
    if (!_params.last_name.match(_letterRegex)) {
        return {error: 'Last name cannot contain special characters or numbers', code: 1405}
    }

    // enforce email
    if (!_params.email.match(_emailRegex)) {
        return {error: 'Email is not supported', code: 1406}
    }

    return {}
}

const createAccount = async function(_body) {
    console.log(`body: ${JSON.stringify(_body)}`);
    const _params = validateParams(_body);

    if (_params == -1) {
        return {
            error: "Invalid request body."
        }
    }
    
    const _issue = enforceParamRules(_params);
    if (_issue.error) {
        return _issue;
    }

    const _sql = new SQL();
    const _result = await _sql.createAccount(_params);
    await _sql.close();
    if (_result.error) {
        return _result;
    }

    return {
        data: _result.data
    }
}

module.exports = createAccount