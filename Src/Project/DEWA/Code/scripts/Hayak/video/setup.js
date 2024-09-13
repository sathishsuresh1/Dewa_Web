/**
 * Copyright Avaya Inc.
 * All rights reserved. Usage of this source is bound to the terms described in
 * the file Avaya SDK EULA.pdf, included in this SDK.
 * Avaya – Confidential & Proprietary. Use pursuant to your signed agreement or Avaya Policy.
 */
/**
 * webRTCSetup object contains functions to setup with the sample WebRTC call UI's
 * @class
 */
var webRTCSetup = {
    /**
     * appends a stylesheet to the head element
     * @param {string} stylesheet - the css stylesheet
     */
    attachStylesheet: function (stylesheet) {
        var ss = document.createElement("link");
        ss.type = "text/css";
        ss.rel = "stylesheet";
        ss.href = stylesheet;
        document.getElementsByTagName("head")[0].appendChild(ss);
    },
    /**
     * appends a script to the head element
     * @param {string} script - the script to append
     * @param {function} fn - the function to execute when script is loaded
     */
    attachScript: function (script, fn) {
        var s = document.createElement("script");
        s.src = script;
        s.onload = fn;
        document.getElementsByTagName("head")[0].appendChild(s);
    },
    /**
     * injects the provided partial into the provided element container
     * @param {string} elementId - the id of the container that will hold the view
     * @param {string} partial - the html template
     */
    injectView: function (elementId, partial) {
        $(elementId).load(partial);
    },
    /**
     * adds the necessary dependencies to the DOM for the voice caller
     * @param {function} fn - function to execute when voice control service is available
     */
    setupVoiceCaller: function (fn) {
        this.attachStylesheet('/scripts/hayak/video/voice/style.css');
        this.attachScript('/scripts/hayak/video/voice/voiceui.js');
        this.attachScript('/scripts/hayak/video/voice/voicecontrol.js', fn);
        this.injectView('#replaceAsVoiceWebRTCCaller', '/scripts/hayak/video/voice/voice.tpl.html');
    },
    /**
     * adds the necessary dependencies to the DOM for the video caller
     * @param {function} fn - function to execute when video control service is available
     */
    setupVideoCaller: function (fn) {
        this.attachStylesheet('/scripts/hayak/video/video/style.css');
        this.attachScript('/scripts/hayak/video/video/videoui.js');
        this.attachScript('/scripts/hayak/video/video/videocontrol.js', fn);
        this.injectView('#replaceAsVideoWebRTCCaller', '/scripts/hayak/video/video/video.tpl.html');
    }
};
