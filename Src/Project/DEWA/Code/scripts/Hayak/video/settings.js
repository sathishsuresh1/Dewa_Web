/**
 * Copyright Avaya Inc.
 * All rights reserved. Usage of this source is bound to the terms described in
 * the file Avaya SDK EULA.pdf, included in this SDK.
 * Avaya – Confidential & Proprietary. Use pursuant to your signed agreement or Avaya Policy.
 */
/**
 * settings namespace, manages client and routable attribute settings for the Oceana reference client application.
 * @namespace
 */
var inputSettings = JSON.parse($('.j145-hayak').attr('data-settings'));
var lang = $('html').attr('lang')=='en' ? messageConfig.English : messageConfig.Arabic

var settings = {
    /**
     * variable to hold customer attributes
     */
    attributes: [        
        { name: messageConfig.Language, values: lang},
        { name: messageConfig.Service, values: messageConfig.Sales }
    ],
    /**
     * loads the persisted the reference client settings from local storage 
     * into the settings form tabs
     */
    load: function () {
        this.initListeners();
        this.loadConfig();
        this.loadWorkRequest();
        console.info('Reference Client: settings have been successfully loaded');
    },
    /**
     * persist the reference client settings to local storage
     * for use by the Oceana Customer Services SDK
     */
    save: function () {
        if (this.saveConfig() && this.saveWorkRequest()) {
            console.info('Reference Client: settings have been successfully saved');

            ui.showSnackbar(messageConfig.Settings_saved, 6000, messageConfig.Home, function () {
                utils.openHome();
            });
        } else {
            console.log('Error saving settings');
        }
    },
    /**
     * Settings cancel button event handler
     * returns the customer to the initial reference client screen
     */
    cancel: function () {
        utils.openHome();
    },

    initListeners: function(){
        $('#home_menu_item').on('click', function (event) {
            utils.openHome();
        });
    },
    /**
     * load the customer config object from local storage into the config form
     */
    loadConfig: function () {
        var aawgConfigString = localStorage.getItem('oceana.aawg.config');
        var amcConfigString = localStorage.getItem('oceana.config');
        var customerString = localStorage.getItem('oceana.customer');
        var optionalString = localStorage.getItem('oceana.optional');
        var tokenConfigString = localStorage.getItem('oceana.token.config');

        if (aawgConfigString) {
            var config = JSON.parse(aawgConfigString);
            inputSettings.aawgServerTf = config.webGatewayAddress;

            if (config.port) {
                inputSettings.aawgPortTf = config.port;
            }

            if (config.secure !== undefined) {
                //$('#aawgSecureCb').prop('checked', config.secure);
            }
        } else {
            inputSettings.aawgPortTf = utils.getDefaultAAWGPort();
        }
        if (amcConfigString) {
            var config = JSON.parse(amcConfigString);
            inputSettings.serverTf = config.restUrl;

            if (config.urlPath) {
                inputSettings.amcUrlPathTf = config.urlPath;
            }

            if (config.port) {
                inputSettings.portTf = config.port;
            }
        } else {
            inputSettings.portTf = utils.getDefaultPort();
        }

        if (customerString) {
            var customer = JSON.parse(customerString);
            inputSettings.displayNameTf = customer.displayName;
            inputSettings.fromTf = customer.fromAddress;
        } else {
            var fromAddress = Math.floor(Math.random() * 899999) + 100000;
            inputSettings.displayNameTf = messageConfig.UserHyphen + fromAddress;
            inputSettings.fromTf = fromAddress;
        }

        if (optionalString) {
            var optional = JSON.parse(optionalString);
            inputSettings.destinationTf = optional.destinationAddress;
            inputSettings.contextTf = optional.context;
            inputSettings.topicTf = optional.topic;
        }

        if (tokenConfigString) {
            var tokenConfig = JSON.parse(tokenConfigString);
            inputSettings.tokenServerTf = tokenConfig.tokenServiceAddress;
            inputSettings.tokenPortTf = tokenConfig.port;
            if (tokenConfig.secure !== undefined) {
                //$('#tokenSecureCb').prop('checked', tokenConfig.secure);
            }

            inputSettings.tokenUrlPathTf = tokenConfig.urlPath;
        } else {
            // Default values
            inputSettings.tokenPortTf = utils.getDefaultAAWGPort();
            inputSettings.tokenUrlPathTf = 'token-generation-service/token/getEncryptedToken';
        }
    },
    /**
     * save the customer config object to local storage from the values in the config form
     */
    saveConfig: function () {

        var amcServer = inputSettings.serverTf.trim();
        var amcPort = inputSettings.portTf.trim();
        var amcUrlPath = inputSettings.amcUrlPathTf.trim();
        var displayName = inputSettings.displayNameTf.trim();
        var fromAddress = inputSettings.fromTf.trim();
        var destinationAddress = inputSettings.destinationTf.trim();
        var context = inputSettings.contextTf.trim();
        var aawgServer = inputSettings.aawgServerTf.trim();
        var aawgPort = inputSettings.aawgPortTf.trim();
        var aawgSecure = true;
        var topic = inputSettings.topicTf.trim();
        var tokenServer = inputSettings.tokenServerTf.trim();
        var tokenPort = inputSettings.tokenPortTf.trim();
        var tokenUrlPath = inputSettings.tokenUrlPathTf.trim();
        var tokenSecure = true;

        var config = {
            restUrl: amcServer,
            port: amcPort,
            secure: true,
            urlPath: amcUrlPath
        };

        var aawgConfig = {
            webGatewayAddress: aawgServer,
            port: aawgPort,
            secure: aawgSecure
        };

        var customer = {
            displayName: displayName,
            fromAddress: fromAddress,
        };

        var optional = {
            destinationAddress: destinationAddress,
            context: $('.j145-hayak').attr('data-requestid'),
            topic: topic
        };

        var tokenService = {
            tokenServiceAddress: tokenServer,
            port: tokenPort,
            urlPath: tokenUrlPath,
            secure: tokenSecure
        }

        localStorage.setItem('oceana.aawg.config', JSON.stringify(aawgConfig));
        localStorage.setItem('oceana.config', JSON.stringify(config));
        localStorage.setItem('oceana.customer', JSON.stringify(customer));
        localStorage.setItem('oceana.optional', JSON.stringify(optional));
        localStorage.setItem('oceana.token.config', JSON.stringify(tokenService));
        return true;

    },
    /**
     * load the work request object from local storage
     */
    loadWorkRequest: function () {
        var workRequestString = localStorage.getItem('oceana.workRequest');

        if (workRequestString) {
            var workRequest = JSON.parse(workRequestString);
            inputSettings.priorityTf = workRequest.priority;
            inputSettings.localeTf = workRequest.locale;
            inputSettings.strategyTf = workRequest.strategy;
            this.attributes = workRequest.attributes;

            inputSettings.sourceNameTf = workRequest.sourceName;
            inputSettings.nativeResourceIdTf = workRequest.nativeResourceId;
        }

        // setup attribute grid
        //$('#attributes-grid').jsGrid({
            //width: '100%',
            //height: '70%',
//
            //inserting: true,
            //editing: true,
            //paging: true,
//
            //confirmDeleting: false,
            //updateOnResize: true,
            //noDataContent: messageConfig.NoAttributesConfigured,
            //data: this.attributes,
//
            //fields: [
                //{ name: messageConfig.name, title: messageConfig.Name, type: 'text', width: 150, validate: 'required' },
                //{ name: messageConfig.values, title: messageConfig.Values, type: 'text', width: 150, validate: 'required' },
                //{ type: 'control' }
            //]
        //});
    },
    /**
     * save the work request attribute object to local storage
     * @return {boolean}
     */
    saveWorkRequest: function () {
        var priority = inputSettings.priorityTf.trim();
        var locale = inputSettings.localeTf.trim();
        var strategy = inputSettings.strategyTf.trim();
        var sourceName = inputSettings.sourceNameTf.trim();
        var nativeResourceId = inputSettings.nativeResourceIdTf.trim();

        var workRequest = {
            attributes: this.attributes,
            priority: priority,
            locale: locale,
            strategy: strategy,
            sourceName: sourceName,
            nativeResourceId: nativeResourceId
        };
        localStorage.setItem('oceana.workRequest', JSON.stringify(workRequest));
        return true;
    },
    /**
     * retrieve the customer provided identity object from local storage
     * @return {object} customer
     */
    retrieveIdentity: function () {
        return JSON.parse(localStorage.getItem('oceana.customer'));
    },
    /**
     * retrieve the optional customer provided object from local storage
     * @return {object} optional
     */
    retrieveOptionalParams: function () {
        return JSON.parse(localStorage.getItem('oceana.optional'));
    },
    /**
     * retrieve the customer provided amc config object from local storage
     * @return {object} configuration
     */
    retrieveConfig: function () {
        var oceanaConfig = JSON.parse(localStorage.getItem('oceana.config'));
        var webGatewayConfig = JSON.parse(localStorage.getItem('oceana.aawg.config'));

        var config = oceanaConfig;

        if (webGatewayConfig) {
            config = {
                configuration: oceanaConfig,
                webGatewayConfiguration: webGatewayConfig
            }
        }
        return config;
    },
    /**
     * retrieve the work request object from local storage
     * @return {object} attributes
     */
    retrieveWorkRequest: function () {
        return JSON.parse(localStorage.getItem('oceana.workRequest'));
    },
    /**
     * retrieve the aawg config object from local storage
     * @return {object} configuration
     */
    retrieveAawgConfig: function () {
        return JSON.parse(localStorage.getItem('oceana.aawg.config'));
    },
    /**
     * retrieve the token service config object from local storage
     * @return {object} tokenServiceConfiguration
     */
    retrieveTokenServiceConfig: function () {
        return JSON.parse(localStorage.getItem('oceana.token.config'));
    },
    /**
     * clears customer settings objects from local storage
     */
    clearSettingsStore: function () {
        localStorage.removeItem('oceana.aawg.config');
        localStorage.removeItem('oceana.config');
        localStorage.removeItem('oceana.customer');
        localStorage.removeItem('oceana.workRequest');
        localStorage.removeItem('oceana.token.config');
    }
};
