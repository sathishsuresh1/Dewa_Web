_besHandler = function (accountNumber) {
            var data = { service: 'info', account_id: "00" + accountNumber };
            jQuery.ajax("/api/bes/proxy", {
            dataType: 'json',
            data: data,
            method: 'POST',
            async: true,
            beforeSend: function (jqXHR) {
            xhrPool.push(jqXHR);
                },
                success:function(response) {
                    var result = response;
                    if (result != null && result.account != null) {
                        if (result.account.show_bes != null && result.account.show_bes == "true") {
                            jQuery(".bes-accountid").val(accountNumber);
                            $(".bes-accountid").trigger('change');
                            $("#BEScontainer").show();
                            jQuery(".bes-container").show();
                            jQuery("#outstandingHeaderDiv").show();
                            jQuery("#outstandingHeaderDiv").addClass("m17-sectiontitle");
                            setTimeout(function () {
                                jQuery(".BES-grid").show();
                            }, 1000)
                        }
                        else {
                            jQuery(".BES-grid").hide();
                        }

                    }
                },error:function (response) {
                },complete:function (response) {
                   getGaguageLoadSettting("_besHandler").isComplete = 1;
                }
            });

};

_homebesHandler = function (accountNumber) {

            var data = { service: 'info', account_id: "00" + accountNumber };
            jQuery.ajax("/api/bes/proxy", {
                dataType: 'json',
                data: data,
                method: 'POST',
                success:function(response) {
                    var result = response;
                    if (result != null && result.account != null) {

                        if (result.account.show_bes != null && result.account.show_bes == "true") {
                            $(".nodataavailable").hide();
                            $(".bes-container").show();
                            $(".bes-accountid").val(accountNumber);
                            $(".bes-accountid").trigger('change');
                            setTimeout(function () {
                                jQuery(".BES-grid").show();
                            }, 1000)
                        }
                        else {
                            //console.log("disply our layout");
                            $(".bes-container").hide();
                            $(".nodataavailable").show();
                        }
                    }
                    else {
                        $(".bes-container").hide();
                        $(".nodataavailable").show();
                    }
                },
                error: function (response) {
                    $(".bes-container").hide();
                    $(".nodataavailable").show();
                },complete:function (response) {
                   getGaguageLoadSettting("_homebesHandler").isComplete = 1;
                }
            });
        };

function loadwidget(component, elementid, endpoint, returnURL) {
    var accountnumber = "00" + jQuery(".bes-accountid").val();
    var s;
    var lang;
    if (jQuery('html').attr('lang') != "en") {
        lang = "ar";
    } else {
        lang = "en";
    }
    s = document.createElement('script');
    s.id = 'adv_main';
    s.src = '/api/bes/proxy?page=mainjs&account_id=' + accountnumber;
    s.async = true;
    s.onload = function () {
        var comp = new advComp();
        comp.create({
            "account_id": accountnumber, // currently selected account ID
            "component": component, // e.g. "component": "survey", "nc", "hist"
            "element_id": elementid, // id of element where to display the widget
            "api": endpoint, // in this case //staging.dewa.gove.ae/api/acp/proxy
            "lg": lang,
            "return_url": returnURL
        });
    };
    document.body.appendChild(s);
}
$("#lnkaccountPic").click(function () {
        var $input = $('#accountPic');
        $input.trigger('click');
    });