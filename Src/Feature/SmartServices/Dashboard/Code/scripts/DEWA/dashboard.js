

function getGaguageLoadSettting(name, enabled, isComplete) {
    var d = null;
    for (var i = 0; i < gaguageLoadSettting.length; i++) {
        if (gaguageLoadSettting[i].name == name) {
            d = gaguageLoadSettting[i];
            break;
        }
    }
    return d;
}

function AbortAllAjaxCall() {
    $.each(xhrPool, function (idx, jqXHR) {
        jqXHR.abort();
    });
}

function fnLazyload(gaguageLoadSettting) {
    var doLazyLoad = false;
    if (gaguageLoadSettting != null) {
        for (var i = 0; i < gaguageLoadSettting.length; i++) {
            doLazyLoad = !gaguageLoadSettting[i].enabled;
            if (doLazyLoad) {
                break;
            }
        }
    }
    if (!doLazyLoad) {
        jQuery(".component-tigger").hide();
    }
    return doLazyLoad;
}


var isInViewport = function (eleCheck) {
    var elementTop = $(eleCheck).offset().top;
    var elementBottom = elementTop + $(eleCheck).outerHeight(true);

    var viewportTop = $(window).scrollTop();
    var viewportBottom = viewportTop + $(window).height();

    return elementBottom > viewportTop && elementTop < viewportBottom;
};

function SetAccountDetail(_target) {
    accountNumber = jQuery(_target).find('input[name="SelectedAccountNumber"]:checked').val();
    bpNumber = jQuery(_target).find('input[name="SelectedAccountNumber"]:checked').attr("data-legacy-bp-number");
    premiseNumber = jQuery(_target).find('input[name="SelectedAccountNumber"]:checked').attr("data-legacy-acc-number");
    evaccount = jQuery('input[name="SelectedAccountNumber"]:checked').attr("data-evaccount");
}

function setSelfEnergyLink() {
    var seas_el = 'a.selfenergy';
    if ($(seas_el) === undefined) return true;
    try {
        $el = jQuery('input[name="SelectedAccountNumber"]:checked');
        var acc_type = $el.attr("data-acc-cat");
        var selaccount = $el.val();
        if (acc_type.indexOf("N-Resi") !== -1) {
            jQuery(seas_el).show();
            $(seas_el)[0].search = "";
            $(seas_el).attr('href', function (i, h) {
                return h.replace("?", "") + "?a=" + selaccount;
            });
        }
        else {
            jQuery(seas_el).hide();
        }
    } catch (err) { }
}

function setGaguageLoadSettting(gaguageLoadSettting) {
    for (var i = 0; i < gaguageLoadSettting.length; i++) {
        gaguageLoadSettting[i].enabled = false;
        gaguageLoadSettting[i].isComplete = null;
    }
}

function handleAccountSelection(e, gaguageLoadSettting) {
    AbortAllAjaxCall();
    SetAccountDetail(e.target);
    if (accountNumber) {
        setGaguageLoadSettting(gaguageLoadSettting);
        gaguagesubmit(gaguageLoadSettting, evaccount);
        setSelfEnergyLink();
    }
    return false;
}

function fnInitializeDashBoardComponent(gaguageLoadSettting, evaccount) {
    //debugger;
    if (!fnLazyload(gaguageLoadSettting)) {
        return false;
    }
    setTimeout(function () {
        // if (isInViewport(".component-tigger")) {
        SetAccountDetail("#form-account-selector");
        gaguagesubmit(gaguageLoadSettting, evaccount);
    }, 500)

    return false;
}

function gaguagesubmit(gaguageLoadSettting, evaccount) {
    //debugger;
    if (gaguageLoadSettting != null) {
        setSelfEnergyLink();
        if (evaccount == undefined)
            evaccount = jQuery('input[name="SelectedAccountNumber"]:checked').attr("data-evaccount");
        if (evaccount == 'True') {
            jQuery('.nonevcomponentsdiv').hide();
            jQuery('.evcomponentsdiv').find('.j153-ev-map').attr('data-journey', 'j153-ev-map');
            jQuery('.evcomponentsdiv').find('.m32-map').attr('data-component', 'm32-map');
            window.initComponents('j69-dashboardEV--map');
            jQuery('.evcomponentsdiv').show();
            if (jQuery('#j69-dashboardEV--map') != undefined && jQuery('#j69-dashboardEV--map').find('[data-evmapload]') != undefined && jQuery('#j69-dashboardEV--map').find('[data-evmapload]').attr('data-evmapload') == 'false') {
                setTimeout(function () {
                    jQuery(window).trigger('evLoad');
                }, 100)
            }
        }
        else {
            jQuery('.nonevcomponentsdiv').show();
            jQuery('.evcomponentsdiv').hide();
        }
        for (var i = 0; i < gaguageLoadSettting.length; i++) {
            if (!gaguageLoadSettting[i].enabled && gaguageLoadSettting[i].isComplete == null &&
                ((gaguageLoadSettting[i].evapplicable == "3") || (evaccount == 'True' && gaguageLoadSettting[i].evapplicable == "2") || (evaccount == 'False' && gaguageLoadSettting[i].evapplicable == "1"))) {
                _graphInitiate = true;
                switch (gaguageLoadSettting[i].name) {
                    case "_totalamountdue":
                        if (eval("typeof _totalamountdueHandler") != "undefined") {
                            _totalamountdueHandler(accountNumber);
                        }
                        break;
                    case "_consumptiondetails":
                        if (eval("typeof _consumptiondetailsHandler") != "undefined") {
                            _consumptiondetailsHandler(accountNumber, premiseNumber);
                        }
                        break;
                    case "_consumptionGraph":
                        if (eval("typeof _consumptionGraphHandler") != "undefined") {
                            _consumptionGraphHandler();
                        }
                        break;
                    case "_dewaoffers":
                        if (eval("typeof _fndewaoffers") != "undefined") {
                            _fndewaoffers(accountNumber);
                        }
                        break;
                    case "_besHandler":
                        if (eval("typeof _besHandler") != "undefined") {
                            _besHandler(accountNumber);
                        }
                        break;
                    case "_paymenthistory":
                        if (eval("typeof transactionSelection") != "undefined") {
                            transactionSelection();
                        }
                        break;
                    case "_listofevcards":
                        if (eval("typeof _listofevcardsHandler") != "undefined") {
                            _listofevcardsHandler(accountNumber);
                        }
                        break;
                    case "_evconsumptionGraph":
                        if (eval("typeof _evconsumptionGraphHandler") != "undefined") {
                            _evconsumptionGraphHandler();
                        }
                        break;
                    case "_evtransactions":
                        if (eval("typeof _evtransactionsHandler") != "undefined") {
                            _evtransactionsHandler();
                        }
                        break;
                    case "_evtotalamountdue":
                        if (eval("typeof _evtotalamountdueHandler") != "undefined") {
                            _evtotalamountdueHandler();
                        }
                        break;
                    case "_evbillcompare":
                        if (eval("typeof _fnevbillcompare") != "undefined") {
                            _fnevbillcompare();
                        }
                        break;
                    case "_smartAlertReadGraph":
                        if (eval("typeof _smartAlertReadGraphHandler") != "undefined") {
                            _smartAlertReadGraphHandler(accountNumber);
                        }
                        break;
                    case "_smartAlertSlabreader":
                        if (eval("typeof _smartAlertSlabreaderHandler") != "undefined") {
                            _smartAlertSlabreaderHandler(accountNumber);
                        }
                        break;
                    case "_billcompare":
                        if (eval("typeof _fnbillcompare") != "undefined") {
                            _fnbillcompare(accountNumber);
                        }
                        break;
                    case "_manageaccountinfo":
                        if (eval("typeof _manageaccountinfoHandler") != "undefined") {
                            _manageaccountinfoHandler(accountNumber);
                        }
                        break;
                    case "_homebesHandler":
                        if (eval("typeof _homebesHandler") != "undefined") {
                            _homebesHandler(accountNumber);
                        }
                        break;
                    default:
                        break;
                }

                gaguageLoadSettting[i].enabled = true;
                gaguageLoadSettting[i].isComplete = 0;
            }
        }
        jQuery(".component-tigger").hide();
    }
}