const AWS = require('aws-sdk');

class DynamoDB
{
    constructor()
    {
        this._dynamo = new AWS.DynamoDB();
        this.batchDeleteFromTable = this.batchDeleteFromTable.bind(this);
    }

    async addToTable(_table, _data)
    {
        try {
            const _res = await this._dynamo.putItem({
                Item: _data,
                TableName: _table
            }).promise();
            return _res;
        } catch (_err) {
            console.log(_err);
            return -1;
        }
    }

    async removeFromTable(_table, _data)
    {
        try {
            const _res = await this._dynamo.deleteItem({
                Key: _data,
                TableName: _table
            }).promise();
            return _res;
        } catch (_err) {
            console.log(_err);
            return -1;
        }
    }

    async getFromTable(_table, _data)
    {
        try {
            const _res = await this._dynamo.getItem({
                Key: _data,
                TableName: _table
            }).promise();
            return _res;
        } catch (_err) {
            console.log(_err);
            return -1;
        }
    }

    async scanTable(_table, _data)
    {
        try {
            const _params = _data;
            _data.TableName = _table;
            const _res = await this._dynamo.scan(_params).promise();
            return _res;
        } catch (_err) {
            console.log(_err);
            return -1;
        }
    }

    async updateTable(_table, _data)
    {
        try {
            const _params = _data;
            _data.TableName = _table;
            const _res = await this._dynamo.updateItem(_params).promise();
            return _res;
        } catch (_err) {
            console.log(_err);
            return -1;
        }
    }

    async batchDeleteFromTable(_table, _data)
    {
        var _batch = _data;
        if (_batch.length > 25) {
            await this.batchDeleteFromTable(_table, _data.splice(25, _batch.length));
            _batch = _data.splice(0, 25);
        }

        try {
            var _params = {
                RequestItems: {}
            }
            _params.RequestItems[_table] = _batch;
            console.log(`Batch deleting with params ${JSON.stringify(_params)}`);
            const _res = await this._dynamo.batchWriteItem(_params).promise()
            return _res;
        } catch (_err) {
            console.log(_err);
            return -1;
        }
    }
}

module.exports = new DynamoDB();