docReady(function () {
    Handlebars.registerHelper('converttoamountformat', function (str) {
        return numeral(str).format('0,0.00');
    });
    Handlebars.registerHelper('removezeroamount', function (v1, options) {
        return numeral(v1) == 0 ?  options.inverse(this): options.fn(this);
    });
    Handlebars.registerHelper('evtotalflag', function (v1, options) {
        return numeral(v1) > 0 ?  options.fn(this): options.inverse(this);
    });
});
var _evtotalamountdueHandler = function () {
    var accountNumber = jQuery("input[name='SelectedAccountNumber']:checked").val();
    if (accountNumber != null && accountNumber != undefined && accountNumber != "") {
        var url = "/api/EVBills/Totalamount/";
        jQuery.ajax({
            type: 'GET',
            url: url,
            beforeSend: function (jqXHR) {
                xhrPool.push(jqXHR);
                jQuery('.evbill-placeholder').empty();
                window.attachSpinner('.evbill-placeholder', { bgColor: '#fff', opacity: 0.6, minHeight: 250 });
            },
            data: {
                accountnumber: accountNumber,
            },
            complete: function () {
                window.detachSpinner('.evbill-placeholder');
            },
            dataType: 'json',
            method: 'GET',
            async: true,
            success: function (response) {
                renderEVBill(response.data);
            }, complete: function (response) {
                getGaguageLoadSettting("_evtotalamountdue").isComplete = 1;
                _graphInitiate = false;
            }
        });
    }
}
function renderEVBill(context) {
    var markup = jQuery('#evtotalamount-component-template').html();
    var template = Handlebars.compile(markup);
    var rendering = template(context);
    jQuery('.evbill-placeholder').empty().html(rendering);
}