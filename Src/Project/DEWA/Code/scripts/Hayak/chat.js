
var myModel = {
    socketUrl: "", estimateWaitTimeUrl: "", activity: { type: "message", from: { id: "" }, channelData: { channelType: "Web-Directline" }, text: "" }, isServerError: false, fullName: "", email: "", phone: "", accno: "", language: ""
};

var socket = null, socketOpen = false, isUserLoggedIn = false;
var acc_selector = 'input[name="SelectedAccountNumber"]', agentTyping = false;
var a = { msg: "" }; var dv_container = "#dv_container";

jQuery(document).ready(function () {

    jQuery("#form0").submit(function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        myModel.accno = jQuery(acc_selector + ':checked').val();
        //btStartChat1
        UserInfoSubmit();
    });
    jQuery("#form1").submit(function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        myModel.fullName = jQuery("#txFN1").val().trim();
        myModel.email = jQuery("#txEmail1").val().trim();
        myModel.phone = jQuery("#MobileNumber1").val().trim();
        myModel.accno = jQuery("#txCAN1").val().trim();
        UserInfoSubmit();
    });
    jQuery("#form2").submit(function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        myModel.fullName = jQuery("#txFN2").val().trim();
        myModel.email = jQuery("#txEmail2").val().trim();
        myModel.phone = jQuery("#MobileNumber2").val().trim();

        UserInfoSubmit();
    });

    $('input[id="cid"]').on('input', function () { $(this).val($(this).val().replace(/[<>]/gi, '')); });
    $("#cid").on("keydown", function (e) {
        if (13 == e.which) {
            a.msg = $("#cid").val().trim();
            if (a.msg) {
                $("#cid").val("");
                SendMessage(a.msg);
            }
            $("#cid").val("");
        } else {
            MO.isTypingMessage();
        }
    });

    $("#btSend").click(function (e) {
        e.preventDefault();
        a.msg = $("#cid").val().trim();
        if (a.msg) {
            SendMessage(a.msg);
        }
        $("#cid").val("");
    });

    jQuery(acc_selector).on("change", function () { HandleSelectAccount(); });
    if (isUserLoggedIn) { setTimeout(HandleSelectAccount, 2000); }

    $(window).on("beforeunload", function () {
        if (socket.readyState != 3 && socketOpen == true) {
            var cjs = { "apiVersion": "1.0", "type": "request", "authToken": "", "body": { "method": "closeConversation" } };
            try {
                cjs.authToken = MO.root.authToken;
                socket.send(JSON.stringify(cjs));
            }
            catch (e) {
                //console.log(e);
            }
            socket.close();
            /*if (typeof e == 'undefined') {
               e = window.event;
           }
           if (e) {
               e.returnValue = CONST.closeChatConfirmationMessage;
           }
          return CONST.closeChatConfirmationMessage;
          $("#btnyes").on("click", function () {
             
           });
           $("#btnno").on("click", function () {
               jQuery("#dv_error").hide();
               jQuery("#dv_chat_console").show();
               jQuery("#dv_chat_footer").show();
               
           });*/
        }
    })
    setTimeout(function () {
        $('.j145-footer-draggable').resizable({
            handles: "n",
            maxHeight: 250,
            minHeight: 51,
            resize: function (event, ui) {
                var winHeight = $(window).height(),
                    chatWrapper = $('.j145-hayak'),
                    chatHeaderHeight = chatWrapper.find('.j145-header').height(),
                    chatObj = chatWrapper.find('.j145-chat-obj'),
                    chatHeight = winHeight - chatHeaderHeight - ui.size.height,
                    chatObjHeight = 0;

                if (chatObj.length) {
                    chatObj.each(function () {
                        if ($(this).is(":visible")) {
                            chatObjHeight += $(this).outerHeight();
                        }
                    });
                    chatHeight = chatHeight - chatObjHeight;
                }
                $('.j145-hayak-chat').css('height', chatHeight)
            }
        });
    }, 2000);
});

function HandleSelectAccount() {
    var sel_val = jQuery(acc_selector + ':checked').val();
    if (sel_val != undefined) {
        jQuery("#btStartChat1").prop("disabled", !(sel_val.length > 5));
    }

}

function UserInfoSubmit() {

    if (myModel.accno.length < 6 && (myModel.fullName.length <= 0 || myModel.email.length <= 0 || myModel.phone.length <= 0)) {
        //console.log("form is in invalid state");
        return;
    }

    jQuery("#dv_prechat").hide();

    MO.getEstimatedWaitTime();
}

function InitiateSocket() {
    if (myModel.isServerError) {
        //console.log("Server Error, breaking further process.."); 
        return;
    }

    if (!myModel.socketUrl) {
        //console.log("Socket Url is null.");
        return;
    }

    socket = new WebSocket(myModel.socketUrl);

    socket.onopen = function (e) {
        WriteCon("Connection established");
        //WriteCon("Sending to server");
        socketOpen = true;
        MO.initChatStep1();
        MO.lastAction = 1;
    };

    socket.onmessage = function (e) { MO.onMessage(e.data); };
    socket.onclose = function (e) {
        //console.log(JSON.stringify(e));
        RenderAgentInput(CONST.chatEnded);
        jQuery("#cid").prop("disabled", true);
		jQuery("#videoClickToCall").hide();
		ToggleTyping(false);
        socketOpen = false;
        /*switch (e.code) {
            case 1000:
                
                break;
            default:
                break;
        }*/
    };
    socket.onerror = function (error) {
        WriteCon(JSON.stringify(error));
    };
}

function WriteCon(text) {
    //console.log(text); 
    return true;
}

function SendMessage(d) {
    if (!navigator.onLine) { alert('You are offline!'); return false; };

    if (d && d.length > 0) {
        //var m = formatMyText(d);
        //jQuery(dv_inner).append(m);
        RenderUserInput(d);
        MO.userMessage(d, socket);
        //SetFocus();
        //AppendMessageDateTime(false, moment().locale("en-ae").format());
    }
}

function ToggleTyping(v) {
    if (v === true) {
        RenderAgentIsTyping();
        //jQuery(dv_container).append(jQuery(typingdiv).clone().removeClass("hide"));
        //jQuery(focusDiv).animate({ scrollTop: jQuery(focusDiv)[0].scrollHeight }, 800);
        agentTyping = true;
    }
    else {
        jQuery(".istyping").remove();
        //jQuery(typingdiv).last().remove();
        agentTyping = false;
    }
}

var MO = {
    lastAction: 0, socketOpen: false, lastMessageTime: new Date(), countDown: "", interval: null, isChatAvailable: true, userTyping: false, userTypingSent: false, chatClosedByAgent: false,
    contactDetails: { "FullName": "", "MobileNumber": "", "EmailAddress": "", "AccountNumber": "", "ChatBotRequired": false, __RequestVerificationToken: GetAFToken() },
    root: { authToken: "", apiVersion: "1.0", type: "request", "body": { method: "requestChat", workRequestId: '', requestTranscript: true } },
    constants: { s_guid: "sguid", s_key: "skey" },
    initChatStep2: function () {
        var copy = JSON.parse(JSON.stringify(MO.root));
        copy.body = {
            "DeviceType": navigator.userAgent,
            "intrinsics": {
                'channelAttribute': 'Chat',
                'textDirection': document.dir.toUpperCase(),
                'attributes': ["ServiceType.Hayak", "Language." + myModel.language],
                'email': myModel.email,
                'name': myModel.fullName,
                'lastName': "",
                'country': '',
                'area': '',
                'phoneNumber': myModel.phone,
                'topic': '',
                'customFields': []
            },
            "method": "requestChat",
            "priority": 5,
            "requestTranscript": false,
            "routePointIdentifier": "Default",
            "workFlowType": "", "customData": {}, "calledParty": window.location.href
        };
        copy.body.workRequestId = MO.root.body.workRequestId;
        copy.body.requestTranscript = true;
        copy.type = "request";
        socket.send(JSON.stringify(copy));

        MO.lastMessageTime = new Date();
    },
    initChatStep1: function () {

        $(".j145-hayak").attr("data-requestid", "");
        MO.contactDetails.FullName = myModel.fullName, MO.contactDetails.MobileNumber = myModel.phone, MO.contactDetails.EmailAddress = myModel.email, MO.contactDetails.AccountNumber = myModel.accno;

        $.ajax({
            url: CONST.actionUrl,
            type: "POST",
            dataType: "json",
            data: this.contactDetails,
            success: function (e) {
                try {
                    if (socket.readyState === 1) {
                        MO.root.body.workRequestId = e.requestId;
                        //myModel.referenceId = e.referenceId;
                        myModel.language = e.language;
                        $(".j145-hayak").attr("data-requestid", e.requestId);
                        MO.initChatStep2();

                    } else {
                        //console.log(JSON.stringify(e));
                        //console.log(socket.readyState);
                    }
                }
                catch (err) {
                    console.log(err.message);
                }
            },
            fail: function (e) {
                //console.log(JSON.stringify(e));
                alert('unable to reach server');
            }, statusCode: {
                404: function () {
                    alert('page not found');
                },
                500: function () {
                    //console.log("backend 500 error");
                    //location.reload(true);
                },
                410: function () {
                    //console.log('session timeout');
                    //location.reload(true);
                },
                417: function () {
                    //console.log('backend error');
                }
            }
        });
    },
    initVideo: function () {
        jQuery("#videoClickToCall").hide();
        try {
            if (utils.checkIfBrowserIsSupport()) {
                // TODO fix this, this was temporaily disabled to test locally
                if (utils.isSecure()) {
                    settings.saveConfig();
                    settings.saveWorkRequest();

                    settings.load();

                    //ui.setupVoice();
                    ui.setupVideo();
                    jQuery("#videoClickToCall").show();
                }
            } else {
                ui.updateConnectionState('Browser not supported');
            }
        } catch (e) {
            console.log(e);
        }
    },
    onMessage: function (e) {
        //console.log(e);
        var j = JSON.parse(e);
        switch (MO.lastAction) {
            case 0:
                WriteCon(e);
                break;
            case 1:
                var key = "authenticationKey";
                MO.root.authToken = j.body !== 'undefined' && j.body.hasOwnProperty(key) ? j.body.authenticationKey : '';

                if (MO.root.authToken.length > 0) {
                    MO.lastAction = 2;
                    //MO.setSessionItem(this.constants.s_guid, j.body.guid);
                    //MO.setSessionItem(this.constants.s_key, MO.root.authToken);
                }
                break;
            case 2:
                switch (j.type) {
                    case "acknowledgement":
                        //message received by backend;                           
                        break;
                    case "notification":
                        MO.processNotification(j.body);
                        break;
                    default:
                        //console.log(JSON.stringify(j));
                        break;
                }

                break;
            default:
                MO.processNotification(j.body);
                //console.log(JSON.stringify(e));
                break;
        }
    },
    userMessage: function (msg, soket) {
        var copy = JSON.parse(JSON.stringify(MO.root));
        copy.body.method = "newMessage";
        copy.body["message"] = msg;
        copy.body["type"] = "text";
        soket.send(JSON.stringify(copy));
        MO.lastMessageTime = new Date();
        MO.userTyping = false; MO.userTypingSent = false;
    },
    agentMessage: function (o) {
        RenderAgentInput(o.message);
        //SetFocus();
    },
    pingMessage: function () {
        socket.send('{"authToken": "","apiVersion": "1.0","type": "request","body": {"method": "ping"}}');
    },
    isTypingMessage: function () {
        if (MO.userTypingSent === false) {
            socket.send('{"authToken":"","apiVersion":"1.0","type":"request","body":{"method":"isTyping","isTyping":true}}');
            MO.userTypingSent = true;
            MO.lastMessageTime = new Date();
        }
    },
    processNotification: function (j) {
        switch (j.method) {
            case "isTyping":
                if (agentTyping === false)
                    ToggleTyping(true);
                //console.log('Agen is typing....');

                break;
            case "newMessage":
                if (agentTyping === true)
                    ToggleTyping(false);
                if (j.message && j.message.length > 0) { MO.agentMessage(j); }
                break;
            case "participantLeave":
                if (j.endChatFlag) {
                    MO.chatClosedByAgent = true;
                    //MO.agentMessage({ message: j.displayName + " left the Chat." });
                }

                break;
            case "newParticipant":

                if (j.displayName !== undefined && j.displayName.length > 0) {
                    DisplayAgentName(j.displayName);
                }

                if (MO.lastAction === 2) {
                    MO.ewtModal(false);
                    MO.toggleChatWindow(true);
                    //console.log("Agent joined the chat \r\n " + JSON.stringify(j));
                }
                MO.lastAction = 3;
                break;
            case "requestChat":
                if (j.displayName !== undefined && j.displayName.length > 0) {
                    DisplayAgentName(j.displayName);
                }
                var msgs = checkForValue(j);
                if (msgs.length > 0) { RenderAgentInput(msgs); }

                break;
            case "newAgentFileTransfer":

                var url = "";
                url = CONST.fileDownloadUrl.replace("{0}", j.uuid); url = url.replace("{1}", j.workRequestId);

                var ftm = CONST.fileTransfer.replace("{0}", j.agentName);
                ftm = ftm.replace("{1}", '<a href="' + url + '" target="_blank">');
                ftm = ftm + '</a>';

                RenderAgentInput(ftm);
                break;
            case "newCoBrowseSessionKeyMessage":
                //console.log(JSON.stringify(j));
                /*if (j.sessionKey && j.sessionKey.length > 0) {
                    MO.initCobrowse(j.sessionKey);
                    //setTimeout(MO.joinKeyPushCoBrowse, 3000, j.sessionKey);
                }*/
                break;
            case "newPushPageMessage":
                if (j.displayName !== undefined && j.displayName.length > 0) {
                    if (j.pagePushURL !== undefined && j.pagePushURL.length > 0) {
                        var lnk = '<a href="' + j.pagePushURL + '" target="_blank">' + j.displayName + '</a>';
                        RenderAgentInput(lnk);
                    }
                }
                break;
            case "pong":
                //this.pingMessage();
                break;

            default:
                //console.log(JSON.stringify(j));
                break;
        }

    },
    getEstimatedWaitTime: function () {
        MO.preEWTModal();
        var settings = {
            "url": myModel.estimateWaitTimeUrl,
            "method": "POST",
            "timeout": 0,
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify({ "serviceMap": { "1": { "attributes": { "Channel": ["Chat"], "Language": [myModel.language], "ServiceType": ["Hayak"] }, "priority": 5 } } }),
        };
        var err = 'EWT: empty response! This may be caused by CORS issues or by blocking JavaScript (e.g. NoScript)';

        jQuery.ajax(settings).done(function (response) {
            //console.log(response);
            if (response === err) {
                //console.error();
                return;
            }

            var mapId = '1';
            var json = JSON.parse(JSON.stringify(response));
            MO.processEstimatedWaitTime(json.serviceMetricsResponseMap[mapId]);

            setTimeout(function () { if (MO.isChatAvailable === true) InitiateSocket(); }, 3000);

        }).fail(function (response) {

            if (response === '') {
                //console.error(err + '. Response code: ' + this.status);
            } else {
                //console.error('EWT: ' + response);
            }

            MO.alertMessage(CONST.chatNotAvailableHeader);
        });
    },
    processEstimatedWaitTime: function (serviceMap) {
        var maxWaitTime = 720;
        var minAgentCount = 1;
        var minWaitTime = 0;
        var priority = 5;

        var services = { "1": {} };
        //var alertHeader = CONST.chatPossibleHeader;
        var alertMsg = CONST.chatPossibleMsg;
        var alertWaitMsg = CONST.chatPossibleWaitMsg;
        var chatAvailable = true, chatWaitTimeZero = false;
        var metrics = serviceMap.metrics;

        if (metrics !== undefined) {
            var waitTime = parseInt(metrics.EWT);
            var agentCount = parseInt(metrics.ResourceStaffedCount);
            //console.debug('EWT: wait time is ' + waitTime + '. Maximum wait time is ' + maxWaitTime);
            //console.debug('EWT: ' + agentCount + ' agents are logged in. Minimum allowed are ' + minAgentCount);
            if (waitTime < maxWaitTime && waitTime >= minWaitTime && agentCount >= minAgentCount) {
                chatWaitTimeZero = waitTime <= minWaitTime;
                var minutes = Math.floor(waitTime / 60);
                var seconds = waitTime - minutes * 60;
                MO.countDown = minutes + ":" + seconds;
                //var waitTimeInMins = Math.round(waitTime / 60);
                alertMsg = CONST.chatAvailableMsg.replace('{0}', '<span class="time" id="spTime">' + MO.countDown + '</span>');
                //alertHeader = CONST.chatAvailableHeader;
                alertWaitMsg = CONST.chatAvailableWaitMsg;
                chatAvailable = true;

            } else {
                chatAvailable = false;
                alertMsg = CONST.chatNotAvailableMsg;
                if (waitTime > maxWaitTime) {
                    //alertHeader = CONST.chatNotAvailableHeader;
                    alertWaitMsg = CONST.chatNotAvailableHeader
                } else {
                    alertMsg = CONST.noAgentsAvailableMsg;
                    alertWaitMsg = CONST.noAgentsAvailableMsg;
                }
            }
        }

        if (!chatAvailable) {
            this.isChatAvailable = false;
        }
        //jQuery("#alertheader").text(alertHeader);
        jQuery("#alertbody").html(alertMsg);
        //jQuery("#dvwaitmsg").text(alertWaitMsg);
        this.ewtModal(true, chatWaitTimeZero);

    },
    alertMessage: function (body) {
        jQuery("#dv_ewt1").hide();
        jQuery("#dv_ewt2").hide();
        jQuery("#dv_ewt3").hide();
        jQuery("#dv_chat_console").hide();
        jQuery("#dv_chat_footer").hide();

        jQuery("#dv_error").show();
        jQuery("#errorbody").text(body);

    },
    preEWTModal: function () {
        jQuery("#dv_ewt1").hide();
        jQuery("#dvwaitmsg").text(CONST.chatPossibleWaitMsg);
        jQuery("#dv_ewt2").show();
        jQuery("#dv_ewt3").show();

    },
    ewtModal: function (op, wt) {
        var id1 = "#dv_ewt1", id2 = "#dv_ewt2", id3 = "#dv_ewt3";
        if (op === true) {
            jQuery(id1).show();
            jQuery(id2).hide();
            jQuery(id3).hide();
            if (wt === false) { MO.startCountDown(); }
            else { jQuery(id2).show(); jQuery(id3).show(); }
            jQuery(window).trigger('on_resize');
        }
        else {
            jQuery(id1).hide();
            jQuery(id2).hide();
            jQuery(id3).hide();
        }
    },
    toggleChatWindow: function (op) {
        if (op === true) {
            MO.initVideo();
            clearInterval(MO.interval);
            MO.ewtModal(false, false);
            jQuery("#dv_chat_console").show();
            jQuery("#dv_chat_footer").show();
            jQuery("body").css({ "overflow": "hidden" });
            jQuery(window).trigger('on_resize');
        }
        else {
            jQuery("#dv_chat_console").hide();
            jQuery("#dv_chat_footer").hide();
        }
    },
    /*joinKeyPushCoBrowse: function (coBrowseKey) {
        'use strict';
        window.opener.joinSession(myModel.fullName, coBrowseKey, []);
        //coBrowseUI.hideCoBrowseLinkDiv();
    },*/
    initCobrowse: function (key) {
        'use strict';
        //window.opener.initCB(console, CONST.coBrowseHost, myModel.fullName, key);
    },
    startCountDown: function () {
        MO.interval = setInterval(function () {
            try {
                var timer = MO.countDown.split(':');
                var minutes = parseInt(timer[0], 10);
                var seconds = parseInt(timer[1], 10);
                --seconds;
                minutes = (seconds < 0) ? --minutes : minutes;
                if (minutes < 0) MO.clearCountDown();
                seconds = (seconds < 0) ? 59 : seconds;
                seconds = (seconds < 10) ? '0' + seconds : seconds;
                $('#spTime').html(minutes + ':' + seconds);
                MO.countDown = minutes + ':' + seconds;
                if (minutes < 1 && seconds < 1) MO.clearCountDown();
            } catch (e) {
                console.log("Countdown Error!");
                console.log(e);
            }
        }, 1000);
    },
    getSessionItem: function (key) {
        'use strict';

        if (typeof (Storage) !== 'undefined') {
            return sessionStorage.getItem(key);
        } else {
            return null;
        }
    },
    setSessionItem: function (key, value) {
        'use strict';

        if (typeof (Storage) !== 'undefined') {
            sessionStorage.setItem(key, value);
        } else {
            console.log('No session storage availabe.');
        }
    },
    clearSession: function () {

    },
    clearCountDown: function () {
        clearInterval(MO.interval);
        jQuery("#dvwaitmsg").text(CONST.chatAvailableWaitMsg);
        jQuery("#dv_ewt2").show();
        jQuery("#dv_ewt3").show();
    }
};

function RenderUserInput(txt) {
    var d = new Date();
    var c = "";
    var dir = document.body.style.direction;
    if (dir === "rtl") {
        c = myModel.fullName.slice(myModel.fullName.length - 1);
    } else {
        c = myModel.fullName.charAt(0).toUpperCase();
    }
    var dt = moment(d).format('hh:mm A');

    var n = '<div class="j145-hayak-chat-section-row"><div class="j145-hayak-user-chat"><div class="j145-hayak-chat-avatar"><span>{0}</span></div><div class="j145-hayak-chat-text"><p>{1}</p><p class="time" style="direction:ltr">{2}</p></div></div></div>'.format(c, HtmlSanitizer.SanitizeHtml(txt), dt);
    $(dv_container).append(n);
}

function RenderAgentInput(txt) {
    var d = new Date();
    var dt = moment(d).format('hh:mm A');
    var n = '<div class="j145-hayak-chat-section-row"><div class="j145-hayak-agent-chat"><div class="j145-hayak-chat-avatar"><img src="/images/hayak/agent-icon.png"></div><div class="j145-hayak-chat-text"><p>{0}</p><p class="time" style="direction:ltr">{1}</p></div></div></div>'.format(HtmlSanitizer.SanitizeHtml(txt), dt);
    $(dv_container).append(n);
}

function DisplayAgentName(nam) {
    var s = CONST.youAreNowChattingWith.format(nam);
    var dv_id = "#dv_agent_welcome";
    $(dv_id).find("h4").remove();
    $(dv_id).append('<h4 class="agent-name active">' + s + '<span class="icon icon-pagination_active"></span></h4>');
}

function RenderAgentIsTyping() {    
    var n = '<div class="j145-hayak-chat-section-row istyping"><div class="j145-hayak-agent-chat"><div class="j145-hayak-chat-avatar"><img src="/images/hayak/agent-icon.png"></div><div class="j145-hayak-chat-text"><p>' + CONST.agentIsTyping + '</p></div></div></div>';
    $(dv_container).append(n);
}

String.prototype.format = function () {
    var a = arguments;
    return this.replace(/{(\d+)}/g, function (e, t) {
        return void 0 !== a[t] ? a[t] : e;
    });
};

function checkForValue(json) {
    var v = "";
    var mainO = "webOnHoldComfortGroup", subO1 = "messages";
    if (json.hasOwnProperty(mainO) && json.hasOwnProperty(subO1)) {
        var a = json[mainO][subO1];
        for (var i = 0; i < a.length; i++) {
            if (i == 0) { v = a[i]["message"]; }
            else { v += " <br/>" + a[i]; }
        }
    }
    return v;
}

function GetAFToken() {
    return jQuery('input[name="__RequestVerificationToken"]').val();
}