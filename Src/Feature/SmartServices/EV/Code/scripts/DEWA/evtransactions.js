docReady(function () {
    Handlebars.registerHelper('discountedamount', function (v1, v2, options) {
        if (numeral(v1) == 0) {
            return options.inverse(this);
        }
        if (v1 == v2) {
            return options.fn(this);
        }
        return options.inverse(this);
    });
    Handlebars.registerHelper('ifselectedcardEquals', function (arg1, options) {
        return (arg1 == "") ?  options.inverse(this): options.fn(this);
    });
    Handlebars.registerHelper('toSelectedCard', function (v1, v2, options) {
        if (v1 == v2) {
            return "selected";
        }
        return "";
    });
    Handlebars.registerHelper('converttoconsumptionformat', function (str) {
        return numeral(str).format('0,0.000');
    });
});
var _evtransactionsHandler = function () {
    _evcardstransactionslist();
    return false;
}
var _evcardstransactionslist = function () {
    var accountNumber = jQuery("input[name='SelectedAccountNumber']:checked").val();
    var url = "/api/EVCards/Account/";
    jQuery.ajax({
        type: 'GET',
        url: url,
        beforeSend: function (jqXHR) {
            xhrPool.push(jqXHR);
            window.attachSpinner(dvtransactionsEV, {
                bgColor: '#000',
                opacity: 0.3,
                minHeight: 480,
                zIndex: 1000
            });
        },
        data: {
            id: accountNumber,
        },
        dataType: 'json',
        method: 'GET',
        async: true,
        success: function (response) {
            if (response != null) {
                if (response.Error != null && response.Error != undefined) {
                    renderEVTransactionErrorList(response.Error);
                }
                else if (response != null) {
                    renderEVTransactionsCards(response);
                    _evtransactionsListitems();
                }
            }
        },
        error: function (jqXHR, exception) {
            window.detachSpinner(dvtransactionsEV);
            jQuery(dvtransactionsEV).trigger(nodata_ev_el);
            var msg = '';
            if (jqXHR.status === 0) {
                msg = 'Not connect.\n Verify Network.';
            } else if (jqXHR.status == 404) {
                msg = 'Requested page not found. [404]';
            } else if (jqXHR.status == 500) {
                msg = 'Internal Server Error [500].';
            } else if (exception === 'parsererror') {
                msg = 'Requested JSON parse failed.';
            } else if (exception === 'timeout') {
                msg = 'Time out error.';
            } else if (exception === 'abort') {
                msg = 'Ajax request aborted.';
            } else {
                msg = 'Uncaught Error.\n' + jqXHR.responseText;
            }

        }, complete: function (response) {

            window.detachSpinner(dvtransactionsEV);
        }
    });
}
function renderEVTransactionsCards(context) {
    var markup = jQuery('#evcardsintransaction-select-template').html();
    var template = Handlebars.compile(markup);
    var modifiedcontext = {
        context: context,
        selectedcard: jQuery('#selectevtransactionscardsdiv').attr("data-selectedcardid")
    };
    var rendering = template(modifiedcontext);
    jQuery('#selectevtransactionscardsdiv').html(rendering);
    evtransactionlistchange();
}

function evtransactionsselection(page) {
    firstTime = false;
    _evtransactionsListitems(page);
    return false;
}
function evtransactionlistchange() {
    jQuery("#form-field-EVTransactionYearFilter, #form-field-evtransactionlistcards,#form-field-transactionssortby  ").on('change', function (e) {
        event.preventDefault();
        event.stopImmediatePropagation();
        _evtransactionsListitems();
    });
}
var _evtransactionsListitems = function (page) {
    var accountNumber = jQuery("input[name='SelectedAccountNumber']:checked").val();
    if (typeof (page) === 'undefined') {
        page = 1;
    }
    var url = "/api/EVTransactions/Account/";
    jQuery.ajax({
        type: 'GET',
        url: url,
        beforeSend: function (jqXHR) {
            xhrPool.push(jqXHR);
            window.attachSpinner("#EVtransactionsPlaceholder", {
                bgColor: '#000',
                opacity: 0.3,
                minHeight: 480,
                zIndex: 1000
            });
        },
        data: {
            accountnumber: accountNumber,
            cardid: jQuery("#form-field-evtransactionlistcards option:selected").val(),
            dashboardpage: (jQuery(".evtransactions").attr("data-dashboardpage") == 'True'),
            page: page,
            monthyear: jQuery("#form-field-EVTransactionYearFilter option:selected").val(),
            sortby: jQuery("#form-field-transactionssortby option:selected").val(),
        },
        dataType: 'json',
        method: 'GET',
        async: true,
        success: function (evresponse) {
            if (evresponse != null) {
                if (evresponse.Error != null && evresponse.Error != undefined) {
                    renderEVTransactionErrorList(evresponse.Error)
                }
                else if (evresponse != null && evresponse.data != null) {
                    var response = evresponse.data;
                    renderEVTransactionList(response);
                    if (response.pagination) {
                        var pagedata = {
                            previouspage: function () {
                                if (response.page > 1)
                                    return true;
                                else
                                    return false;
                            },
                            nextpage: function () {
                                if (response.page < response.totalpage)
                                    return true;
                                else
                                    return false;
                            },
                            page: response.page,
                            firstpagenumber: 1,
                            lastpagenumber: response.totalpage,
                            previouspagenumber: response.page - 1,
                            nextpagenumber: response.page + 1,
                            pagenumbers: response.pagenumbers,
                            totalpage: response.totalpage
                        }
                        renderSearchPagination(pagedata);
                    }
                    else {
                        jQuery('#pagination-list').html("");
                    }
                    window.initComponents('EVtransactionsPlaceholder');
                    jQuery(window).trigger("reinit_m23");
                }
            }
        },
        error: function (jqXHR, exception) {
            var msg = '';
            if (jqXHR.status === 0) {
                msg = 'Not connect.\n Verify Network.';
            } else if (jqXHR.status == 404) {
                msg = 'Requested page not found. [404]';
            } else if (jqXHR.status == 500) {
                msg = 'Internal Server Error [500].';
            } else if (exception === 'parsererror') {
                msg = 'Requested JSON parse failed.';
            } else if (exception === 'timeout') {
                msg = 'Time out error.';
            } else if (exception === 'abort') {
                msg = 'Ajax request aborted.';
            } else {
                msg = 'Uncaught Error.\n' + jqXHR.responseText;
            }

        }, complete: function (response) {
            jQuery(window).trigger('reinit_m23');
            window.detachSpinner('#EVtransactionsPlaceholder');
            getGaguageLoadSettting("_evtransactions").isComplete = 1;
            _graphInitiate = false;
        }
    });
}
function renderEVTransactionList(context) {
    var markup = jQuery('#evtransactions-item-template').html();
    var template = Handlebars.compile(markup);
    var rendering = template(context);
    jQuery('#EVtransactionsPlaceholder').html(rendering);
}
function renderEVTransactionErrorList(context) {
    var markup = jQuery('#evcardsintransaction-error-template').html();
    var template = Handlebars.compile(markup);
    var rendering = template(context);
    jQuery('#EVtransactionsPlaceholder').html(rendering);
}
function renderSearchPagination(context) {
    var markup = jQuery('#evtransaction-pagination-template').html();
    var template = Handlebars.compile(markup);
    var rendering = template(context);
    jQuery('#pagination-list').html(rendering);
}