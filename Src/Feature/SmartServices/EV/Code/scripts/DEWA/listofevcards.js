var accountNumberselected;
var firstTime;
docReady(function () {
    Handlebars.registerHelper('each_upto', function (ary, max, options) {
        if (!ary || ary.length == 0)
            return options.inverse(this);

        var result = [];
        for (var i = 0; i < max && i < ary.length; ++i)
            result.push(options.fn(ary[i]));
        return result.join('');
    });
    Handlebars.registerHelper('ifCond', function (v1, v2, options) {
        if (v1 == v2) {
            return options.fn(this);
        }
        return options.inverse(this);
    });
    Handlebars.registerHelper('bookmarked', function (arg1, options) {
        return (arg1 != null && arg1 != '' && arg1 != undefined && arg1 == 'X') ? options.fn(this) : options.inverse(this);
    });
});
function evlistofcardsselection(page) {
    firstTime = false;
    _listofevcardsHandler(accountNumberselected, page);
    return false;
}
function loadcustomfunc() {
    jQuery("#listofcardsearchbutton").on('click', function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();
        evlistofcardsselection(1);
    });
    jQuery(".evbookmark").on('click', function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();
        var cardid = jQuery(this).attr('data-cardid');
        var modalpopup = jQuery(this).attr('data-modal');
        if (cardid != null && cardid != undefined && cardid != "") {
            _clickevbookmark(cardid, modalpopup, "", true, false);
        }
        return false;
    });
    jQuery(".evrenamebutton").on('click', function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();
        var evrenametext = jQuery('#form-field-evrenametext').val();
        var modalpopup = jQuery(this).attr('data-modal');
        var cardid = jQuery(this).attr('data-cardid');
        if (evrenametext != null && evrenametext != undefined && evrenametext != "") {
            _clickevbookmark(cardid, undefined, evrenametext, false, true);
        }
        return false;
    });

}
function modalclose() {
    jQuery(".viewallclose").on('click', function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();
        firstTime = true;
        _listofevcardsHandler(accountNumberselected, 1);
        return false;
    });
}
function modalclose1() {
    jQuery('.viewallclose').off('click.m100cards').on('click.m100cards', function () {
        var $parent = jQuery(this).closest('.m39-modal__header');
        $parent.find('#form-field-listofcardmodal').val('');
        $parent.find('#listofcardsearchbutton').click();
    });
}


var _listofevcardsHandler = function (accountNumber, page) {
    if (accountNumber != null && accountNumber != undefined && accountNumber != "") {
        accountNumberselected = accountNumber;
        if (typeof (page) === 'undefined') {
            page = 1;
            firstTime = true;
        }
        var url = "/api/ListofEVCards/Card/";
        jQuery.ajax({
            type: 'GET',
            url: url,
            beforeSend: function (jqXHR) {
                xhrPool.push(jqXHR);
                window.attachSpinner("#listofcardsdiv", {
                    bgColor: '#000',
                    opacity: 0.3,
                    minHeight: 480,
                    zIndex: 1000
                });
                //beforesendfnc();
            },
            data: {
                id: accountNumber,
                page: page,
                keyword: jQuery('#form-field-listofcardmodal').val()
            },
            dataType: 'json',
            method: 'GET',
            async: true,
            success: function (response) {
                if (response != null) {
                    if (response.Error != null && response.Error != undefined) {
                        renderErrorList(response.Error)
                    }
                    else if (response != null && response.eVCardDetails != null) {
                        if (firstTime == 1) {
                            renderList(response.eVCardDetails);
                        }
                        renderModalList(response.eVCardDetails);
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
                            jQuery('#evlistpagination-list').html("");
                        }
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
                modalclose1()
                jQuery(window).trigger('reinitm100');
                window.detachSpinner('.listofcard-placeholder');
                loadcustomfunc();
                getGaguageLoadSettting("_listofevcards").isComplete = 1;
                _graphInitiate = false;
                window.detachSpinner("#listofcardsdiv");
                // completefunc();
            }
        });
    }
}


var _clickevbookmark = function (cardid, modalpopup, name, bookmark, renameclick) {
    var url = "/api/UpdateEVCard/Card/";
    jQuery.ajax({
        type: 'GET',
        url: url,
        beforeSend: function (jqXHR) {
            xhrPool.push(jqXHR);
            window.attachSpinner("#listofcardsdiv", {
                bgColor: '#000',
                opacity: 0.3,
                minHeight: 480,
                zIndex: 1000
            });
            //beforesendfnc();
        },
        data: {
            accountnumber: accountNumberselected,
            cardid: cardid,
            name: name,
            bookmark: bookmark,
        },
        dataType: 'json',
        method: 'GET',
        async: true,
        success: function (response) {
            if (modalpopup != null && modalpopup != undefined && modalpopup != '') {
                firstTime = false;
                modalclose();
                _listofevcardsHandler(accountNumberselected, 1);
            }
            else {
                firstTime = true;
                _listofevcardsHandler(accountNumberselected, 1);
                if (renameclick) {
                    jQuery(".renameclose").trigger('click');
                }
                else {

                }
            }
        },
        error: function () {

        }, complete: function (response) {
            jQuery(window).trigger('reinitm100');
            loadcustomfunc();
            window.detachSpinner("#listofcardsdiv");
            //completefunc();
        }
    });
}

function renderList(context) {
    var markup = jQuery('#listofcards-item-template').html();
    var template = Handlebars.compile(markup);
    var rendering = template(context);
    jQuery('#listofcard-placeholder').html(rendering);
    window.initComponents('listofcard-placeholder');
}
function renderModalList(context) {
    var markup = jQuery('#listofcards-modal-template').html();
    var template = Handlebars.compile(markup);
    var rendering = template(context);
    jQuery('#modallistofcards').html(rendering);
}
function renderErrorList(context) {
    var markup = jQuery('#listofcards-error-template').html();
    var template = Handlebars.compile(markup);
    var rendering = template(context);
    jQuery('#listofcard-placeholder').html(rendering);
}

function renderSearchPagination(context) {
    var markup = jQuery('#listofcards-pagination-template').html();
    var template = Handlebars.compile(markup);
    var rendering = template(context);
    jQuery('#evlistpagination-list').html(rendering);
}