'use strict'
const {Sequelize, DataTypes} = require('sequelize');
const {decrypt} = require('../util/crypto.js');

class SQLController {
    constructor() {
        this._sql = new Sequelize(
            process.env.DB_NAME || 'existence', 
            process.env.DB_USER || 'admin', 
            process.env.DB_PASS || 'adminadmin', {
            /*host: process.env.DB_ENDPOINT || 'stage-existence-db-proxy.proxy-c4rptbmpkq5n.us-east-1.rds.amazonaws.com',*/
            host: process.env.DB_ENDPOINT || 'stage-existence.c4rptbmpkq5n.us-east-1.rds.amazonaws.com',
            port: process.env.DB_PORT || 3306,
            dialect: process.env.DB_DIALECT || 'mysql',
            logging:console.log
        });

        this.getPlayer = this.getPlayer.bind(this);
        this.__define_models__ = this.__define_models__.bind(this);
        this.__validate_account__ = this.__validate_account__.bind(this);
        this.getItem = this.getItem.bind(this);

        this.__define_models__();
    }

    __log__(_msg) {
        console.log(`[SQL]: ${_msg}`);
    }

    async __validate_account__(_params) {
        const _acct = await this._account.findByPk(_params.account);
        if (!_acct) {
            console.log(`Unable to verify account ${_params.account}`);
            return {
                error: `No account found for user '${_params.account}'.`,
                code: 1398
            }
        }

        // !! TODO
        // Alert suspicious behavior
        if (_acct.dataValues.apiKey != _params.apiKey) {
            console.log(`Invalid api key was attempted on account ${_params.account}`);
            return {
                error: `Invalid api key was attempted on account ${_params.account}`,
                code: 1399
            }
        }

        return {};
    }

    async connect() {
        this.__log__("Attempting to connect to server.");
        try {
            await this._sql.authenticate();
            this.__log__("Successfully connected.");
            return true;
        } catch(_err) {
            this.__log__("Failed to connect: "+_err);
            return false;
        }
    }

    async close() {
        await this._sql.close();
    }

    async createAccount(_params) {
        try {
            var _check = await this._account.findOne({where: {username: _params.username}});
            if (_check) {
                return {
                    error: `Account already exists for username '${_params.username}'.`,
                    code: 1407
                }
            }

            var _check = await this._account.findOne({where: {email: _params.email}});
            if (_check) {
                return {
                    error: `Account already exists for email '${_params.email}'.`,
                    code: 1408
                }
            }

            const _acct = await this._account.create(_params);

            return {
                data: _acct
            }
        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async authenticate(_params) {
        try {
            const _acct = await this._account.findOne({where: {username: _params.username}});
            if (!_acct) {
                return {
                    error: `No account found for user '${_params.username}'.`
                }
            }
            
            const _data = decrypt(_acct.dataValues.apiKey);
            const _verify = _data.split('&');
            if (!_data.includes('&') || _verify.length < 2) {
                return {
                    error: `Something is wrong with the user's account authentication.`
                }
            }

            if (_params.password != _verify[1]) {
                return {
                    error: `Incorrect password.`
                }
            }

            return {
                data: _acct.dataValues
            }
        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async getAccountPlayers(_params) {
        try {
            // verify account
            const _authCheck = await this.__validate_account__(_params);
            if (_authCheck.error) {
                return _authCheck;
            }

            const _players = await this._player.findAll({where: {accountID: _params.account}});
            console.log(_players);
            var _data = [];
            for (i in _players) {
                const _player = _players[i];
                const _stats = await this._stat.findByPk(_player.dataValues.statsID);
                const _sessionData = await this._sessionData.findByPk(_player.dataValues.sessionDataID);
                const _inventory = (await this._sql.query(`select items.*, inventorySlots.lvl as lvl, inventorySlots.ID as slotID, inventorySlots.loc as slotLoc from items
                    inner join inventorySlots on inventorySlots.playerID = ${_player.dataValues.id} and inventorySlots.itemID = items.ID`))[0];
                for (var i = 0; i < _inventory.length; i++) {
                    var _item = _inventory[i];
                    _item.requirements = await this._stat.findByPk(_item.requirementsID);
                    _item.effects = await this._stat.findByPk(_item.effectsID);
                    Object.keys(_item.requirements.dataValues).forEach((_key, _index) => {
                        if (_key.toLowerCase() != 'id') {
                            _item.requirements.dataValues[_key] *= _item.lvl;
                        }
                    });
                    Object.keys(_item.effects.dataValues).forEach((_key, _index) => {
                        if (_key.toLowerCase() != 'id') {
                            _item.effects.dataValues[_key] *= _item.lvl;
                        }
                    });
                    _item.level = _item.lvl;
                    delete _item["requirementsID"];
                    delete _item["effectsID"];
                }
                _data.push({
                    player: _player.dataValues,
                    sessionData: _sessionData,
                    stats: _stats,
                    inventory: _inventory
                });
            }

            return {
                data: _data
            }
            
        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async getPlayer(_playerName) {
        try {
            const _player = await this._player.findOne({where: {name: _playerName}});
            if (!_player) { 
                return {
                    error: `No player found named ${_playerName}.`
                }; 
            }

            const _playerId = _player.dataValues.id;
            const _stats = await this._stat.findByPk(_player.dataValues.statsID);
            const _sessionData = await this._sessionData.findByPk(_player.dataValues.sessionDataID);
            const _inventory = (await this._sql.query(`select items.*, inventorySlots.lvl as lvl, inventorySlots.ID as slotID, inventorySlots.loc as slotLoc from items
                inner join inventorySlots on inventorySlots.playerID = ${_playerId} and inventorySlots.itemID = items.ID`))[0];
            for (var i = 0; i < _inventory.length; i++) {
                var _item = _inventory[i];
                _item.requirements = await this._stat.findByPk(_item.requirementsID);
                _item.effects = await this._stat.findByPk(_item.effectsID);
                _item.slotID = _item.slotID;
                Object.keys(_item.requirements.dataValues).forEach((_key, _index) => {
                    if (_key.toLowerCase() != 'id') {
                        _item.requirements.dataValues[_key] *= _item.lvl;
                    }
                });
                Object.keys(_item.effects.dataValues).forEach((_key, _index) => {
                    if (_key.toLowerCase() != 'id') {
                        _item.effects.dataValues[_key] *= _item.lvl;
                    }
                });
                _item.level = _item.lvl;
                delete _item["requirementsID"];
                delete _item["effectsID"];
            }
            //console.log(JSON.stringify(_inventory));
            
            return {
                data: {
                    player: _player.dataValues,
                    sessionData: _sessionData,
                    stats: _stats,
                    inventory: _inventory
                }
            }
        } catch (_err) {
            console.log(_err);
            return {
                error: _err
            }
        }
    }

    async createPlayer(_params) {
        try {
            // verify account
            const _authCheck = await this.__validate_account__(_params);
            if (_authCheck.error) {
                return _authCheck;
            }

            const _playerExists = await this._player.findOne({where: {name: _params.name}});
            if (_playerExists) {
                return {
                    error: `Player already exists with name ${_params.name}`,
                    code: 1400
                }
            }

            const _statsInit = await this._stat.create({});
            const _stats = await this._stat.findByPk(_statsInit.dataValues.id);
            if (_stats.id == null) {
                return {
                    error: `Failed to generate stats.`,
                    code: 1401
                }
            }

            const _sessionInit = await this._sessionData.create({})
            const _sessionData = await this._sessionData.findByPk(_sessionInit.dataValues.id);
            if (_sessionData.id == null) {
                return {
                    error: `Failed to generate session data.`,
                    code: 1402
                }
            }

            const _player = await this._player.create({name: _params.name, serverID: 1, accountID: _params.account, statsID: _stats.id, sessionDataID: _sessionData.id});
            if (_player.id == null) {
                return {
                    error: `Failed to create player.`,
                    code: 1403
                }
            }

            return {
                data: {
                    player: _player,
                    sessionData: _sessionData,
                    stats: _stats
                }
            }
        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async updatePlayer(_player) {
        try {
            const _resp = await this._player.update(_player, {where: {id: _player.ID}})
            //console.log(JSON.stringify(_resp));
            return {
                data: _resp
            }
        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async updateInventory(_params) {
        try {
            // verify account
            const _authCheck = await this.__validate_account__(_params);
            if (_authCheck.error) {
                return _authCheck;
            }

            const _player = await this._player.findByPk(_params.playerID);
            if (!_player) {
                return {
                    error: `Player does not exist with id ${_params.playerID}`,
                    code: 1400
                }
            }

            const _resp = await this._inventorySlot.update({loc: _params.slotLoc}, {where: {id: _params.slotID}});

            return {
                data: _resp
            }
        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async addInventory(_params) {
        try {
            // verify account
            const _authCheck = await this.__validate_account__(_params);
            if (_authCheck.error) {
                return _authCheck;
            }

            const _player = await this._player.findByPk(_params.playerID);
            if (!_player) {
                return {
                    error: `Player does not exist with id ${_params.playerID}`,
                    code: 1400
                }
            }

            var _item = await this._item.findByPk(_params.itemID);
            if (!_item) {
                return {
                    error: `Item does not exist with id ${_params.itemID}`,
                    code: 1401
                }
            }

            _item.dataValues.requirements = (await this._stat.findByPk(_item.dataValues.requirementsID)).dataValues;
            _item.dataValues.effects = (await this._stat.findByPk(_item.dataValues.effectsID)).dataValues;
            Object.keys(_item.dataValues.requirements).forEach((_key, _index) => {
                if (_key.toLowerCase() != 'id') {
                    _item.dataValues.requirements[_key] *= _params.lvl;
                }
            });
            Object.keys(_item.dataValues.effects).forEach((_key, _index) => {
                if (_key.toLowerCase() != 'id') {
                    _item.dataValues.effects[_key] *= _params.lvl;
                }
            });
            _item.dataValues.requirements = _item.dataValues.requirements;
            _item.dataValues.effects = _item.dataValues.effects;
            delete _item.dataValues.requirementsID;
            delete _item.dataValues.effectsID;

            const _slot = await this._inventorySlot.create({playerID: _params.playerID, itemID: _params.itemID, lvl: _params.lvl});
            _item.dataValues.slotID = _slot.id;
            _item.dataValues.level = _params.lvl;

            return {
                data: _item.dataValues
            }
        } catch (_err) {
            console.log(_err);
            return {
                error: _err
            }
        }
    }

    async modifyItem(_params) {
        try {
            // verify account
            const _authCheck = await this.__validate_account__(_params);
            if (_authCheck.error) {
                return _authCheck;
            }

            switch (_params.method) {
                case "c": 
                    if (await this._item.findOne({where: {name: _params.item.name}})) {
                        return {
                            error: `Cannot create. Item ${_params.item.name} already exists.`,
                            code: 1401
                        }
                    }

                    const _reqStat = await this._stat.create(_params.job.requirements)
                    const _effStat = await this._stat.create(_params.job.effects)
                    const _newItem = await this._item.create({
                        ..._params.item,
                        requirementsID: _reqStat.id,
                        effectsID: _effStat.id,
                        itemType: _params.item.type == "weapon" ? 0 : _params.item.type == "armor" ? 1 : 2
                    });

                    // create the sub props type if applicable
                    if (_params.item.type) {
                        const _subProps = {
                            itemID: _newItem.dataValues.id,
                            ..._params.item.subProps
                        };

                        switch (_params.item.type) {
                            case "weapon": const _wep = await this._weaponItem.create(_subProps); break; 
                            case "armor": const _armor = await this._armorItem.create(_subProps); break; 
                            default: /* no-op */ break;
                        }
                    }

                    return {
                        data: _newItem
                    }
                case "d": 

                    break;
                case "u": 
                    const _check = await this._item.findOne({where: {name: _params.job.itemName}});
                    if (!_check) {
                        return {
                            error: `Cannot update. Item ${_params.job.itemName} does not exist.`,
                            code: 1402
                        }
                    }

                    const _item = await this._item.update(_params.item, {where: {name: _params.job.itemName}});
                    if (_params.requirements) {
                        await this._stat.update(_params.requirements, {where: {id: _check.dataValues.requirementsID}});
                    }
                    if (_params.effects) {
                        await this._stat.update(_params.effects, {where: {id: _check.dataValues.effectsID}});
                    }

                    switch (_check.dataValues.itemType) {
                        // weapons
                        case 0: await this._weaponItem.update(_params.item.subProps, {where: {itemID: _check.dataValues.id}}); break;
                        // armor
                        case 1: await this._armorItem.update(_params.item.subProps, {where: {itemID: _check.dataValues.id}}); break;
                        default: break;
                    }

                    return {
                        data: _item
                    }
                default:
                    return {
                        error: 'Unknown method',
                        code: 1400
                    }
            }

        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async modifyMob(_params) {
        try {
            // verify account
            const _authCheck = await this.__validate_account__(_params);
            if (_authCheck.error) {
                return _authCheck;
            }

            switch (_params.method) {
                case "c": 
                    if (await this._mob.findOne({where: {name: _params.mob.name}})) {
                        return {
                            error: `Cannot create. Mob ${_params.mob.name} already exists.`,
                            code: 1401
                        }
                    }

                    const _newMob = await this._mob.create(_params.mob);

                    return {
                        data: _newMob
                    }
                case "d": 

                    break;
                case "u": 
                    const _check = await this._mob.findOne({where: {name: _params.job.mobName}});
                    if (!_check) {
                        return {
                            error: `Cannot update. Mob ${_params.job.mobName} does not exist.`,
                            code: 1402
                        }
                    }

                    const _mob = await this._mob.update(_params.mob, {where: {name: _params.job.mobName}});

                    return {
                        data: _mob
                    }
                default:
                    return {
                        error: 'Unknown method',
                        code: 1400
                    }
            }

        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async modifyMobLoot(_params) {
        try {
            // verify account
            const _authCheck = await this.__validate_account__(_params);
            if (_authCheck.error) {
                return _authCheck;
            }

            const _check = await this._mobLootItem.findOne({where: {mobID: _params.mobLoot.mobID, itemID: _params.mobLoot.itemID}});

            switch (_params.method) {
                case "c": 
                    if (_check) {
                        return {
                            error: `Cannot create. Mob loot ${JSON.stringify(_params.mobLoot)} already exists.`,
                            code: 1401
                        }
                    }

                    const _newMobLoot = await this._mobLootItem.create(_params.mobLoot);

                    return {
                        data: _newMobLoot
                    }
                case "d": 

                    break;
                case "u": 
                    if (!_check) {
                        return {
                            error: `Cannot update. Mob loot ${JSON.stringify(_params.mobLoot)} does not exist.`,
                            code: 1402
                        }
                    }

                    const _mobLoot = await this._mobLootItem.update(_params.mobLoot, {where: {mobID: _params.mobLoot.mobID, itemID: _params.mobLoot.itemID}});

                    return {
                        data: _mobLoot
                    }
                default:
                    return {
                        error: 'Unknown method',
                        code: 1400
                    }
            }

        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async getMobLoot(_params) {
        try {

            const _mob = await this._mob.findOne({where: {name: _params.mobName}});
            if (!_mob) {
                return {
                    error: `No mob exists named ${_params.mobName}`,
                    code: 1400
                }
            }

            const _potential = await this._mobLootItem.findAll({where: {mobID: _mob.dataValues.id}});
            
            var _loot = [];
            var i = 0;
            while (i < _potential.length) {
                var _item = _potential[i].dataValues;
                
                if (Math.random() < _item.dropRate) {
                    // get the item 
                    const _lvl = _params.lvl - Math.round(_item.lvlRange / 2) + Math.round(Math.random()*_item.lvlRange);
                    const _itemData = await this.getItem({id: _item.itemID, ql: _lvl});
                    if (_itemData.error) {
                        return _itemData;
                    }
                    console.log(_itemData.data);
                    _loot.push(_itemData.data);
                    console.log(_loot);
                }

                i++;
            }
            
            return {
                data: _loot
            }

        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async getItem(_params) {
        try {
            const _item = await this._item.findByPk(_params.id);
            if (!_item) {
                return {
                    error: `Item does not exist with id ${_params.id}`,
                    code: 1400
                }
            }

            var _requirements = await this._stat.findByPk(_item.dataValues.requirementsID);
            var _effects = await this._stat.findByPk(_item.dataValues.effectsID);
            
            Object.keys(_requirements.dataValues).forEach((_key, _index) => {
                if (_key.toLowerCase() != 'id') {
                    _requirements.dataValues[_key] *= _params.ql;
                }
            });
            Object.keys(_effects.dataValues).forEach((_key, _index) => {
                if (_key.toLowerCase() != 'id') {
                    _effects.dataValues[_key] *= _params.ql;
                }
            });
            _item.dataValues.requirements = _requirements;
            _item.dataValues.effects = _effects;
            delete _item.dataValues.requirementsID;
            delete _item.dataValues.effectsID;

            var _subItem = null;
            switch (_item.dataValues.itemType) {
                //weapons
                case 0: _subItem = await this._weaponItem.findOne({where: {itemID: _item.dataValues.id}}); break;
                //armor
                case 1: _subItem = await this._armorItem.findOne({where: {itemID: _item.dataValues.id}}); break;
                default: break;
            }
            
            if (_subItem != null) {
                delete _subItem.dataValues.itemID;
                delete _subItem.dataValues.id;
            }

            const _ret = _subItem == null ? {..._item.dataValues, level: _params.ql} : {..._item.dataValues, ..._subItem.dataValues, level: _params.ql};

            return {
                data: _ret
            }
        } catch (_err) {
            console.log(_err);
            return {
                error: _err
            }
        }
    }

    async updateStats(_stats) {
        try {
            const _resp = await this._stat.update(_stats, {where: {id: _stats.ID}})
            //console.log(JSON.stringify(_resp));
            return {
                data: _resp
            }
        } catch (_err) {
            return {
                error: _err
            }
        }
    }

    async updateSessionData(_session) {
        try {
            const _resp = await this._sessionData.update(_session, {where: {id: _session.ID}})
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

    __define_models__() {
        // ACCOUNTS
        this._account = this._sql.define('account', {
            ID: {type:DataTypes.CHAR(255),primaryKey:true},
            first_name: DataTypes.CHAR(255),
            last_name: DataTypes.CHAR(255),
            apiKey: DataTypes.CHAR(255),
            username: DataTypes.CHAR(255),
            email: DataTypes.CHAR(255)
        }, {
            timestamps: false
        });

        // ARMOR ITEMS
        this._armorItem = this._sql.define('armorItem', {
            itemID: DataTypes.INTEGER,
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
            itemID: DataTypes.INTEGER,
            loc: DataTypes.INTEGER,
            lvl: DataTypes.INTEGER
        }, {
            timestamps: false
        });

        // ITEMS
        this._item = this._sql.define('item', {
            name: DataTypes.CHAR(255),
            description: DataTypes.CHAR(255),
            requirementsID: DataTypes.INTEGER,
            effectsID: DataTypes.INTEGER,
            rarity: DataTypes.INTEGER,
            shopBuyable: DataTypes.TINYINT,
            stackable: DataTypes.TINYINT,
            tradeskillable: DataTypes.TINYINT,
            icon: DataTypes.CHAR(255),
            itemType: DataTypes.INTEGER
        }, {
            timestamps: false
        });

        // PLAYERS
        this._player = this._sql.define('player', {
            //ID: {type:DataTypes.CHAR(255),primaryKey:true},
            name: DataTypes.CHAR(255),
            accountID: DataTypes.INTEGER,
            serverID: DataTypes.INTEGER,
            statsID: DataTypes.INTEGER,
            sessionDataID: DataTypes.INTEGER,
            level: DataTypes.INTEGER,
            xp: DataTypes.INTEGER,
            statPoints: DataTypes.INTEGER,
        }, {
            timestamps: false
        });

        // SESSION DATA
        this._sessionData = this._sql.define('sessionData', {
            //ID: {type:DataTypes.INTEGER,primaryKey:true},
            posX: DataTypes.INTEGER,
            posY: DataTypes.INTEGER,
            posZ: DataTypes.INTEGER,
            rotX: DataTypes.INTEGER,
            rotY: DataTypes.INTEGER,
            rotZ: DataTypes.INTEGER
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
            //ID:{type:DataTypes.INTEGER,primaryKey:true},
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
        this._weaponItem = this._sql.define('weaponItem', {
            itemID: DataTypes.INTEGER,
            weaponType: DataTypes.INTEGER,
            damageMin: DataTypes.INTEGER,
            damageMax: DataTypes.INTEGER,
            speed: DataTypes.INTEGER
        }, {
            timestamps: false
        });

        // MOBS
        this._mob = this._sql.define('mob', {
            name: DataTypes.CHAR(255),
        }, {
            timestamps: false
        });

        // MOB LOOT ITEMS
        this._mobLootItem = this._sql.define('mobLootItem', {
            mobID: DataTypes.INTEGER,
            itemID: DataTypes.INTEGER,
            dropRate: DataTypes.FLOAT,
            lvlRange: DataTypes.INTEGER
        }, {
            timestamps: false
        });
    }
}

module.exports = SQLController;