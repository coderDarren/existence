'use strict'
const {Sequelize, DataTypes} = require('sequelize');

class SQLController {
    constructor() {
        this._sql = new Sequelize(
            process.env.DB_NAME || 'existence', 
            process.env.DB_USER || 'admin', 
            process.env.DB_PASS || 'adminadmin', {
            host: process.env.DB_ENDPOINT || 'stage-existence.c4rptbmpkq5n.us-east-1.rds.amazonaws.com',
            port: process.env.DB_PORT || 3306,
            dialect: process.env.DB_DIALECT || 'mysql',
            logging:console.log
        });

        this.getPlayer = this.getPlayer.bind(this);
        this.__define_models__ = this.__define_models__.bind(this);
        this.__define_models__();
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

    async createArmor() {
        const _reqStat = await this._stat.create({
            strength: 12,
            dexterity: 12,
        })
        const _effStat = await this._stat.create({})
        const _item = await this._item.create({
            level: 5,
            name: "test item2",
            requirementsID: _reqStat.id,
            effectsID: _effStat.id,
            rarity: 1
        });
    }

    async getPlayer(_playerName) {
        try {
            const _player = await this._player.findAll({where: {name: _playerName}});
            if (_player.length == 0) { 
                return {
                    error: `No player found named ${_playerName}.`
                }; 
            }

            const _stats = await this._stat.findByPk(_player[0].dataValues.statsID);
            
            return {
                data: {
                    player: _player[0].dataValues,
                    stats: _stats
                }
            }
        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async createPlayer(_account, _playerName) {
        try {
            const _playerExists = (await this.getPlayer(_playerName)).error == null;
            if (_playerExists) {
                return {
                    error: `Player already exists with name ${_playerName}`
                }
            }

            const _stats = await this._stat.create({});
            if (_stats.id == null) {
                return {
                    error: `Failed to generate stats.`
                }
            }

            const _player = await this._player.create({name: _playerName, serverID: 1, accountID: _account, statsID: _stats.id});
            return {
                data: {
                    player: _player,
                    stats: _stats
                }
            }
        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async updateStats(_stats) {
        try {
            const _resp = await this._stat.update(_stats, {where: {id: _stats.ID}})
            console.log(JSON.stringify(_resp));
            return {
                data: _resp
            }
        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    __define_models__() {
        // ACCOUNTS
        this._account = this._sql.define('account', {
            first_name: DataTypes.CHAR(255),
            last_name: DataTypes.CHAR(255),
            apiKey: DataTypes.CHAR(255),
            username: DataTypes.CHAR(255)
        }, {
            timestamps: false
        });

        // ARMOR ITEMS
        this._armorItem = this._sql.define('armorItem', {
            armorType: DataTypes.INTEGER
        }, {
            timestamps: false
        });

        // AUGMENT SLOT TYPE
        this._augmentSlotType = this._sql.define('augmentSlotType', {
            name: DataTypes.CHAR(255)
        }, {
            timestamps: false
        });

        // EQUIPMENT ITEM TYPES
        this._equipmentItemType = this._sql.define('equipmentItemType', {
            name: DataTypes.CHAR(255)
        }, {
            timestamps: false
        });

        // EQUIPMENT SLOTS
        this._equipmentSlot = this._sql.define('equipmentSlot', {
            itemID: DataTypes.INTEGER,
            slotType: DataTypes.INTEGER,
            itemType: DataTypes.INTEGER
        }, {
            timestamps: false
        });

        // GEAR SLOT TYPES
        this._gearSlotType = this._sql.define('gearSlotType', {
            name: DataTypes.CHAR(255)
        }, {
            timestamps: false
        });

        // INVENTORY SLOTS
        this._inventorySlot = this._sql.define('inventorySlot', {
            playerID: DataTypes.INTEGER,
            itemID: DataTypes.INTEGER
        }, {
            timestamps: false
        });

        // ITEMS
        this._item = this._sql.define('item', {
            name: DataTypes.CHAR(255),
            requirementsID: DataTypes.INTEGER,
            effectsID: DataTypes.INTEGER,
            level: DataTypes.INTEGER,
            rarity: DataTypes.INTEGER,
            shopBuyable: DataTypes.TINYINT
        }, {
            timestamps: false
        });

        // PLAYERS
        this._player = this._sql.define('player', {
            name: DataTypes.CHAR(255),
            accountID: DataTypes.INTEGER,
            serverID: DataTypes.INTEGER,
            statsID: DataTypes.INTEGER,
            level: DataTypes.INTEGER
        }, {
            timestamps: false
        });

        // PROSTHETIC SLOT TYPES
        this._prostheticSlotType = this._sql.define('prostheticSlotType', {
            name: DataTypes.CHAR(255),
        }, {
            timestamps: false
        });

        // RARITY TYPES
        this._rarityType = this._sql.define('rarityType', {
            name: DataTypes.CHAR(255),
        }, {
            timestamps: false
        });

        // SERVERS
        this._server = this._sql.define('server', {
            name: DataTypes.CHAR(255),
            capacity: DataTypes.INTEGER
        }, {
            timestamps: false
        });

        // STATS
        this._stat = this._sql.define('stat', {
            strength: DataTypes.INTEGER,
            dexterity: DataTypes.INTEGER,
            intelligence: DataTypes.INTEGER,
            fortitude: DataTypes.INTEGER,
            nanoPool: DataTypes.INTEGER,
            nanoResist: DataTypes.INTEGER,
            treatment: DataTypes.INTEGER,
            firstAid: DataTypes.INTEGER,
            oneHandEdged: DataTypes.INTEGER,
            twoHandEdged: DataTypes.INTEGER,
            pistol: DataTypes.INTEGER,
            evades: DataTypes.INTEGER,
            crit: DataTypes.INTEGER,
            attackSpeed: DataTypes.INTEGER,
            hacking: DataTypes.INTEGER,
            engineering: DataTypes.INTEGER,
            programming: DataTypes.INTEGER,
            quantumMechanics: DataTypes.INTEGER,
            symbiotics: DataTypes.INTEGER,
            processing: DataTypes.INTEGER,
            runSpeed: DataTypes.INTEGER,
            melee: DataTypes.INTEGER
        }, {
            timestamps: false
        }); 

        // TRADESKILL JOBS
        this._tradeskillJob = this._sql.define('tradeskillJob', {
            itemID: DataTypes.INTEGER,
            combineItemID: DataTypes.INTEGER,
            outputItemID: DataTypes.INTEGER,
            requirementsID: DataTypes.INTEGER
        }, {
            timestamps: false
        });

        // WEAPON ITEMS
        this._weaponItem = this._sql.define('weaponItems', {
            itemID: DataTypes.INTEGER,
            weaponType: DataTypes.INTEGER,
            damageMin: DataTypes.INTEGER,
            damageMax: DataTypes.INTEGER
        }, {
            timestamps: false
        });
    }
}

module.exports = SQLController;