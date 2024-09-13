var _manageaccountinfoHandler = function (accountNumber) {
    if (accountNumber) {
        //var showhide = false;
        //if (showhidebool == "False") {
        //    showhide = true;
        //}
        //if (typeof (showhidebool) === 'undefined')
        //    showhide = false;
        
        var url = "/api/sitecore/Account/ManageAccountV1Ajax";
        jQuery.ajax({
            type: 'POST',
            url: url,
            traditional: true,
            datatype: 'application/json',
            data: AddForgeryToken({
                account: accountNumber
            }, "form-account-selector"),
            beforeSend: function (jqXHR) {
                //if (showhide) {
                //    jQuery('.manageaccountsuccessdiv').hide();
                //}
                if (jQuery('.errordiv').html().trim() != "") {
                    if (jQuery('.errordiv').hasClass('errordivexists')) {
                        jQuery('.errordiv').removeClass('errordivexists');
                        jQuery('.errordiv').html('');
                    }
                    else {
                        jQuery('.errordiv').addClass('errordivexists');
                    }
                }
                
                jQuery('.j105-drrg--loader').show();
                jQuery('.j105-drrg--loader').css('top', $(window).scrollTop());
                jQuery('body').removeClass('unscrollable').addClass('unscrollable');
            },
            complete: function () {
                jQuery('.j105-drrg--loader').hide();
                jQuery('body').removeClass('unscrollable');
                setTimeout(function () {
                    require(['parsley'], function () {
                        (jQuery('#accountprofiledetail').find('.form').parsley());
                    })
                    window.initComponents('accountprofiledetail');
                    jQuery(window).trigger('form_reinit');
                }, 100);
               // getGaguageLoadSettting("_manageaccountinfo").isComplete = 1;
               // _graphInitiate = false;
            },
            success: function (response) {
                if (response != undefined && response != "") {
                    //if (showhide) {
                    //    jQuery('.manageaccountdiv').show();
                    //}

                    //if (showhidebool == "True") {
                    //    showhidebool = "False";
                    //}
                    jQuery('#accountprofiledetail').html('');
                    jQuery('#accountprofiledetail').html(response);
                    var evaccountselected = jQuery('input[name="SelectedAccountNumber"]:checked').attr("data-evaccount");
                    if (evaccountselected == 'False') {
                        jQuery('#setprimaryaccountdiv').show();
                    }
                    else {
                        jQuery('#setprimaryaccountdiv').hide();
                    }
                }
            },});

    }
    return false;
}
