'use strict'
const {Sequelize} = require('sequelize');

class SQLController {
    constructor() {
        this._sql = new Sequelize('mysql', 'admin', 'adminadmin', {
            host: 'stage-existence.c4rptbmpkq5n.us-east-1.rds.amazonaws.com',
            port: 3306,
            dialect: 'mysql',
            logging:console.log
        });
    }

    __log__(_msg) {
        console.log(`[SQL]: ${_msg}`);
    }

    async connect() {
        this.__log__("Attempting to authenticate.");
        try {
            await this._sql.authenticate();
            this.__log__("Successfully authenticated.");
            return true;
        } catch(_err) {
            this.__log__("Failed to authenticate: "+_err);
            return false;
        }
    }

    query(_query) {
        return {};
    }
}

module.exports = SQLController;