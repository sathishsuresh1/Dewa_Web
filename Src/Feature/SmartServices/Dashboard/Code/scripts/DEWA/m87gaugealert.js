var _consumptiondetailsHandler = function (accountNumber, premiseNumber) {
    var d = [];
    if (accountNumber != null && accountNumber != undefined && accountNumber != "" && premiseNumber != null && premiseNumber != undefined && premiseNumber != "") {
        var url = "/api/ConsumptionDetailsStats/ConsumptionDetailsStats/" + accountNumber + "X" + premiseNumber;
        jQuery.ajax({
            type: 'POST',
            url: url,
            traditional: true,
            dataType: 'json',
            async: true,
            datatype: 'application/json',
            data: AddForgeryToken({
                id: accountNumber
            }, "billplaceholderdiv"),
            beforeSend: function (jqXHR) {
                xhrPool.push(jqXHR);
                jQuery('.gauge-component').empty();
                window.attachSpinner('.gauge-component', { bgColor: '#fff', opacity: 0.6, minHeight: 250 });
            },
            success: function (response) {
                if (response != null && response.dataseries != null) {
                    renderReadGraph(response.dataseries);
                    jQuery(window).trigger('reinit_m87-select');
                }
            }, complete: function (response) {
                window.detachSpinner('.gauge-component');
                getGaguageLoadSettting("_consumptiondetails").isComplete = 1;
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



function renderReadGraph(item) {
    var markup = jQuery('#dashboard-gaugegraph-item-template').html();
    var template = Handlebars.compile(markup);

    var rendering = template(item);
    jQuery('.gauge-component').empty().html(rendering);
}