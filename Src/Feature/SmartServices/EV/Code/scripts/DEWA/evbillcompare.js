var _fnevbillcompare = function () {
    var accountNumber = jQuery("input[name='SelectedAccountNumber']:checked").val();
    if (accountNumber != null && accountNumber != undefined && accountNumber != "") {
        var url = "/api/sitecore/EVDashboard/EVBillCompare";
        jQuery.ajax({
            type: 'POST',
            url: url,
            traditional: true,
            datatype: 'application/json',
            data: AddForgeryToken({
                account: accountNumber,
            }, "evbillcompare-component-form"),
            beforeSend: function (jqXHR) {
                xhrPool.push(jqXHR);
                jQuery('#evbill-compare-component').empty();
                window.attachSpinner('#evbill-compare-component', { bgColor: '#fff', opacity: 0.6, minHeight: 250 });
            },
            complete: function () {
                jQuery(window).trigger('resize');
                getGaguageLoadSettting("_evbillcompare").isComplete = 1;
                window.detachSpinner('#evbill-compare-component');
            },
            success: function (result) {
                if (result != null && result != "") {
                    jQuery('#evbill-compare-component').append(result);
                    jQuery(window).trigger('init_EVm84');
                    //jQuery(window).trigger('init_m84');
                    window.initComponents('receiptmodalslist');
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
            }
        });
    }
}