var _totalamountdueHandler = function (accountNumber) {
    if (accountNumber != null && accountNumber != undefined && accountNumber != "") {
        var url = "/api/bills/Totalamount/" + accountNumber;
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
                jQuery('.bill-placeholder').empty();
                window.attachSpinner('.bill-placeholder', { bgColor: '#fff', opacity: 0.6, minHeight: 250 });
            },
            complete: function () {
                window.detachSpinner('.bill-placeholder');
                getGaguageLoadSettting("_totalamountdue").isComplete = 1;
                _graphInitiate = false;
            },
            success: function (response) {
                renderBill(response);
                jQuery(window).trigger('m81-initGraph');
            },
        });


    }
}



function renderBill(bill) {
        var markup = jQuery('#totalamount-component-template').html();
        var template = Handlebars.compile(markup);

        var context = {
            account: bill.AccountNumber,
            total: numeral(bill.Balance),
            totalformat: numeral(bill.Balance).format('0,0.00'),
            totalflag: numeral(bill.Balance) > 0,
            dewaamt:numeral(numeral(bill.ElectricityBill) + numeral(bill.WaterBill)).format('0,0.00'),
            dewaamtformat:numeral(numeral(bill.ElectricityBill) + numeral(bill.WaterBill)).format('0.00'),
            dewaamtflag:numeral(numeral(bill.ElectricityBill) + numeral(bill.WaterBill)) == 0,
            electricity: numeral(bill.ElectricityBill).format('0,0.00'),
            electricityflag: numeral(bill.ElectricityBill) == 0,
            water: numeral(bill.WaterBill).format('0,0.00'),
            waterflag: numeral(bill.WaterBill) == 0,
            dubaimunicipality: numeral(numeral(bill.DMCharges) + numeral(bill.SewerageFees) + numeral(bill.HousingFees)).format('0,0.00'),
            dubaimunicipalityformat: numeral(numeral(bill.DMCharges) + numeral(bill.SewerageFees) + numeral(bill.HousingFees)).format('0.00'),
            dubaimunicipalityflag: numeral(numeral(bill.DMCharges) + numeral(bill.SewerageFees) + numeral(bill.HousingFees)) == 0,
            dmcharges: numeral(bill.DMCharges).format('0,0.00'),
            dmchargesflag: numeral(bill.DMCharges) == 0,
            seweragefees: numeral(bill.SewerageFees).format('0,0.00'),
            seweragefeesflag: numeral(bill.SewerageFees) == 0,
            housingfees: numeral(bill.HousingFees).format('0,0.00'),
            housingfeesflag: numeral(bill.HousingFees) == 0,
            nakheel: numeral(bill.CoolingCharges).format('0,0.00'),
            nakheelformat: numeral(bill.CoolingCharges).format('0.00'),
            nakheelflag: numeral(bill.CoolingCharges) == 0,
            others: numeral(bill.OtherCharges).format('0,0.00'),
            othersformat: numeral(bill.OtherCharges).format('0.00'),
            othersflag: numeral(bill.OtherCharges) == 0,
        };

        var rendering = template(context);
        jQuery('.bill-placeholder').empty().html(rendering);
    }