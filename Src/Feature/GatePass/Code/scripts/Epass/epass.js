
var defaultOpts = {
    closeOnSelect: true,
    format: 'd mmmm yyyy',
    formatSubmit: 'd mmmm yyyy',
    onStart: function () {
        // set default date
        var $dp = this.$node,
            date;

        if ($dp.data('initial-date')) {
            date = new Date($dp.data('initial-date'));
            this.set('select', [date.getFullYear(), date.getMonth(), date.getDate()]);
        }
    },
    onOpen: function () {
        if ($dp.attr('data-scroll') == 'false') {
            return
        } else if ($dp.attr('data-scroll') != undefined) {
            _this.scrollIntoView(this.$node, $dp.attr('data-scroll'));
        } else {
            _this.scrollIntoView(this.$node, false);
        };
    },
    onClose: function () {
        this.component.$node.parsley().validate();
    }
};
function LoadSinglePassValidation() {
    jQuery("#form-field-EmiratesIDExpiry").on("change", function () {
        checkIDValidation();
    });
    $emiratesid.on('focusout', function () {
        checkIDValidation();
    });
    jQuery("#form-field-emirates").on("change", function () {
        checkRTAValidation();
    });
    $platenumberid.on('focusout', function () {
        checkRTAValidation();
    });
    $platecodeid.on('focusout', function () {
        checkRTAValidation();
    });
    $platecategoryid.change(function () {
        checkRTAValidation();
    });
}
function checkIDValidation() {
    if (jQuery($emiratesid).parsley().isValid()) {
        if ($emiratesid.val() != "" && $emiratesid.val() != undefined && $emiratesidexpirydate.val() != "" && $emiratesidexpirydate.val() != undefined) {
            var response = Icadetails($emiratesid.val(), $emiratesidexpirydate.val(), "", "", "", "", true, epassbeforesendfunc, epasscompletefunc, successfunc, errorfunc);
        }
    }
}

function checkRTAValidation() {
    var selVal = jQuery("#form-field-emirates option:selected").val();
    if (selVal === "DXB") {
        if (jQuery($platenumberid).parsley().isValid()) {
            if ($platenumberid.val() != "" && $platenumberid.val() != undefined && jQuery("#form-field-categorycode option:selected").val() != "") {
                var response = Icadetails("", "", jQuery("#form-field-categorycode option:selected").val(), $platecodeid.val(), $platenumberid.val(), jQuery("#form-field-emirates option:selected").val(), false, epassbeforesendfunc, epasscompletefunc, successrtafunc, errorrtafunc);
            }
        }
    }
    else {
        errorrtafunc();
    }
}
function epassbeforesendfunc() {
    jQuery('#Errormessage').html('');
    jQuery('.j113-epass--loader').show();
    jQuery('.j113-epass--loader').css('top', $(window).scrollTop());
    $('body').removeClass('unscrollable').addClass('unscrollable');
}
function epasscompletefunc() {
    jQuery('.j113-epass--loader').hide();
    $('body').removeClass('unscrollable');
}
function successfunc(response) {
    if (response != null && response.result != null && response.result.icadetails != null) {
        $eidattachment.hide();
        //$fullname.val(response.result.icadetails.fullname);
        //$fullname.attr('readonly', true);
        if (response.result.icadetails.fullname != null && response.result.icadetails.fullname != "") {
            $fullname.val(response.result.icadetails.fullname);
            $fullname.addClass('disabled');
            $fullname.prop('readonly', true);
        }
        else {
            $fullname.removeClass('disabled');
            $fullname.prop('readonly', false);
        }
        $profession.val(response.result.icadetails.profession);
        $companyname.val(response.result.icadetails.companyname);
        if (response.result.icadetails.passportnumber != null && response.result.icadetails.passportnumber != "" && response.result.icadetails.passportnumber != undefined) {
            $passportattachment.hide();
            $passport.val(response.result.icadetails.passportnumber);
            $passportexpiry.val(response.result.icadetails.formatpassportexpirydate);
            $passport.addClass('disabled');
            $passport.prop('readonly', true);
            $passportexpiry.addClass('disabled');
            require(['picker'], function () {
                jQuery("#form-field-Passportexpiry").pickadate().pickadate('picker').stop();
                $passportexpiry.prop('readonly', true);
            });
        }
        else {
            $passport.removeClass('disabled');
            $passport.prop('readonly', false);
            $passportexpiry.removeClass('disabled');
            $passportexpiry.prop('readonly', false);
            $passportattachment.show();
        }

        if (response.result.icadetails.formatvisanumber != null && response.result.icadetails.formatvisanumber != "" && response.result.icadetails.formatvisanumber != undefined) {
            $visaattachment.hide();
            $visa.val(response.result.icadetails.formatvisanumber);
            $visaexpiry.val(response.result.icadetails.formatvisaexpirydate);
            $visa.addClass('disabled');
            $visa.prop('readonly', true);
            $visaexpiry.addClass('disabled');
            require(['picker'], function () {
                jQuery("#form-field-visaexpiry").pickadate().pickadate('picker').stop();
                $visaexpiry.prop('readonly', true);
            });
        }
        else {
            $visa.removeClass('disabled');
            $visa.prop('readonly', false);
            $visaexpiry.removeClass('disabled');
            revivePicker('form-field-visaexpiry');
            $visaattachment.show();
        }
        if (response.result.icadetails.nationality != null && response.result.icadetails.nationality != "") {
            $nationality.val(response.result.icadetails.nationality);
            jQuery("#form-field-nationality").trigger('change');
            $nationality.closest('.form-field').hide();
            $('#form-field-nationalityh').val($nationality.val());
            $('#form-field-nationalityh').closest('.form-field').show();
        }
        else {
            $nationality.removeClass('disabled');
            $nationality.prop('readonly', false);
        }
    }
    else {
        errorfunc();
    }
}
function errorfunc() {
    $fullname.val("");
    $fullname.removeAttr('readonly');
    $profession.val("");
    $companyname.val("");
    $visa.val("");
    $visa.removeAttr('readonly');
    $visaexpiry.val("");
    $visaexpiry.removeClass('disabled');
    revivePicker('form-field-visaexpiry');
    $passport.val("");
    $passport.removeAttr('readonly');
    $passportexpiry.val("");
    $passportexpiry.removeClass('disabled');
    revivePicker('form-field-Passportexpiry');
    $nationality.val("");
    $nationality.closest('.form-field').show();
    $('#form-field-nationalityh').closest('.form-field').hide();
    jQuery("#form-field-nationality").trigger('change');
}
function successrtafunc(response) {
    if (response != null && response.result != null && response.result.rtadetails != null) {
        $dlattachment.hide();
        $vhattachment.hide();
        if (response.result.rtadetails.formatinsuranceexpirydate != null && response.result.rtadetails.formatinsuranceexpirydate != "" && response.result.rtadetails.formatinsuranceexpirydate != undefined) {
            $vehregis.val(response.result.rtadetails.formatinsuranceexpirydate);
            $vehregis.addClass('disabled');
            require(['picker'], function () {
                jQuery("#form-field-vehregis").pickadate().pickadate('picker').stop();
                $vehregis.prop('readonly', true);
            });
        }
        else {
            $vehregis.removeClass('disabled');
            $vehregis.prop('readonly', false);
        }
    }
    else {
        errorrtafunc();
    }
}
function errorrtafunc() {
    $dlattachment.show();
    $vhattachment.show();
    $vehregis.removeClass('disabled');
    $vehregis.prop('readonly', false);
}
function revivePicker(id) {
    require(['picker'], function () {
        if (true) {
            dpOptions = jQuery('#' + id).data('picker-options');
            dpLabelId = '#' + jQuery('#' + id).parents('.form-field').attr('id');
            if (typeof dpOptions === 'string') {
                dpOptions = JSON.parse(dpOptions);
            } else if (typeof dpOptions !== 'object') {
                dpOptions = {};
            }
            jQuery('#' + id).pickadate(
                $.extend(
                    true, {
                        container: dpLabelId,
                        min: new Date()
                    },
                    dpOptions,
                    defaultOpts
                )
            );
            if (jQuery('#' + id).data('ref')) {
                jQuery('#' + id).on('change', _this.setConstraints);
            }
            jQuery('#' + id).after('<span data-clicker="' + dpLabelId + '" class="form-field__datepicker-icon-clicker"></span>');
        } else {
            jQuery('#' + id).attr('type', 'date');
            if (jQuery('#' + id).data('ref')) {
                jQuery('#' + id).on('change', _this.setConstraintsMobile);
            }
        }
        jQuery('#' + id).prop('readonly', false);
    })
}

function loadvalidations() {
    $epassid = jQuery("#form-field-epassid");
    $icavalid = jQuery("#form-field-icavalid");
    $emiratesid = jQuery("#form-field-emiratesID");
    $emiratesidexpirydate = jQuery("#form-field-eiddate");
    $fullname = jQuery("#form-field-fullname");
    $profession = jQuery("#form-field-profession");
    $companyname = jQuery("#CompanyName");
    $visa = jQuery("#form-field-visa");
    $visaexpiry = jQuery("#form-field-visadate");
    $passport = jQuery("#form-field-passport");
    $passportexpiry = jQuery("#form-field-passportdate");
    $nationality = jQuery("#form-field-nationality");
    $eidattachment = jQuery("#eiddocumentdiv");
    $passportattachment = jQuery("#passportdocumentdiv");
    $visaattachment = jQuery("#visadocumentdiv");
    $dlattachment = jQuery("#drivinglicensedocumentdiv");
    $vhattachment = jQuery("#vehicleregistrationdocumentdiv");
    $platenumberid = jQuery("#form-field-PlateNumber");
    $platecodeid = jQuery("#form-field-PlateCode");
    $platecategoryid = jQuery("#form-field-categorycode");
    $plateemiratesid = jQuery("#form-field-emirates");
    $vehregis = jQuery("#form-field-vehregis");
    onlydxb = ".onlydxb";
    jQuery("#successfullysubmittedokbtn").off('click.nextentry').on('click.nextentry', function () {
        $('.j113-epass--records_item').not('.j113-epass--records_item--done').each(function (index) {
            if (index == 0) {
                $(this).click();
                return false;
            };
        });
    });
    $plateemiratesid.change(function () {
        OnEmrtChange();
    });
    function OnEmrtChange() {
        var selVal = $plateemiratesid.val();
        $(".ct_platenumber").attr("data-parsley-type", "alphanum");
        if (selVal === "DXB") {
            $(onlydxb).show();
            $(".ct_platenumber").attr("data-parsley-type", "number");
        } else {
            $(onlydxb).hide();
        }
    }
    jQuery("#form-field-eiddate").on("change", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();
        checkMultiIDValidation();
    });
    $emiratesid.on('focusout', function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();
        checkMultiIDValidation();
    });
    jQuery("#form-field-emirates").on("change", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation(); checkMultiRTAValidation();
    });
    $platenumberid.on('focusout', function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();
        checkMultiRTAValidation();
    });
    $platecodeid.on('focusout', function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();
        checkMultiRTAValidation();
    });
    $platecategoryid.change(function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();
        checkMultiRTAValidation();
    });
}
function checkMultiIDValidation() {
    setTimeout(function () {
        var $mainvalue = jQuery("#multilist-placeholder").find('.j113-epass--records_item[data-id=' + jQuery("#form-field-epassid").val() + ']');
        if ($mainvalue != null) {
            var icavalue = $mainvalue.attr("data-icavalid");
            var eidvalue = $mainvalue.attr("data-emiratesid");
            var eidexpiryvalue = $mainvalue.attr("data-eidexpdate");

            if (icavalue != "" && icavalue != undefined && icavalue != "true") {
                if ($emiratesid.parsley().isValid()) {
                    if (eidvalue != "" && eidvalue != undefined && eidexpiryvalue != "" && eidexpiryvalue != undefined) {
                        Icadetails(eidvalue, eidexpiryvalue, "", "", "", "", true, epassbeforesendfunc, epasscompletefunc, successmultifunc, errormultifunc);
                    }
                }
            }
        }
    }, 250);
}
function checkMultiRTAValidation() {
    var selVal = jQuery("#form-field-emirates option:selected").val();
    if (selVal === "DXB") {
        if (jQuery($platenumberid).parsley().isValid()) {
            if ($platenumberid.val() != "" && $platenumberid.val() != undefined && jQuery("#form-field-categorycode option:selected").val() != "") {
                var response = Icadetails("", "", jQuery("#form-field-categorycode option:selected").val(),
                    $platecodeid.val(), $platenumberid.val(),
                    jQuery("#form-field-emirates option:selected").val(),
                    false, epassbeforesendfunc,
                    epasscompletefunc,
                    successmultirtafunc, errormultirtafunc);
            }
        }
    }
    else {
        errorrtafunc();
    }
}
function ICAreadonly() {
    if ($mainvalue.data().icavalid == 'true') {
        $fullname.addClass('disabled');
        $fullname.prop('disabled', true);
        $visa.addClass('disabled');
        $visa.prop('disabled', true);
        $visaexpiry.addClass('disabled');
        $visaexpiry.prop('disabled', true);
        $passport.addClass('disabled');
        $passport.prop('disabled', true);
        $passportexpiry.addClass('disabled');
        $passportexpiry.prop('disabled', true);
        $nationality.addClass('disabled');
        $nationality.prop('disabled', true);
    } else {
        $fullname.removeClass('disabled');
        $fullname.prop('disabled', false);
        $visa.removeClass('disabled');
        $visa.prop('disabled', false);
        $visaexpiry.removeClass('disabled');
        $visaexpiry.prop('disabled', false);
        $passport.removeClass('disabled');
        $passport.prop('disabled', false);
        $passportexpiry.removeClass('disabled');
        $passportexpiry.prop('disabled', false);
        $nationality.removeClass('disabled');
        $nationality.prop('disabled', false);
    }
}
function successmultifunc(response) {
    if (response != null && response.result != null && response.result.icadetails != null) {
        $eidattachment.hide();
        $icavalid.val("true");
        var $mainvalue = jQuery("#multilist-placeholder").find('.j113-epass--records_item[data-id=' + jQuery("#form-field-epassid").val() + ']');
        if ($mainvalue != null) {
            $mainvalue.data().full_name = response.result.icadetails.fullname;
            $mainvalue.find('.j113-epass--records_name').html(response.result.icadetails.fullname);
            $mainvalue.data().profession = response.result.icadetails.profession;
            $mainvalue.data().visa = response.result.icadetails.formatvisanumber;
            $mainvalue.data().passport = response.result.icadetails.passportnumber;
            $mainvalue.data().passportexpdate = response.result.icadetails.formatpassportexpirydate;
            $mainvalue.data().visaexpdate = response.result.icadetails.formatvisaexpirydate;
            $mainvalue.data().nationality = response.result.icadetails.nationality;
            $mainvalue.data().icavalid = "true";
        }

        if (response.result.icadetails.fullname != null && response.result.icadetails.fullname != "") {
            $fullname.val(response.result.icadetails.fullname);
            $fullname.addClass('disabled');
            $fullname.prop('disabled', true);
        }
        else {
            $fullname.removeClass('disabled');
            $fullname.prop('disabled', false);
        }
        //$fullname.attr('readonly', true);

        $profession.val(response.result.icadetails.profession);
        $companyname.val(response.result.icadetails.companyname);
        if (response.result.icadetails.passportnumber != null && response.result.icadetails.passportnumber != "" && response.result.icadetails.passportnumber != undefined) {
            $passportattachment.hide();
            $passport.val(response.result.icadetails.passportnumber);
            $passportexpiry.val(response.result.icadetails.formatpassportexpirydate);
            $passport.addClass('disabled');
            $passport.prop('disabled', true);
            $passportexpiry.addClass('disabled');
            $passportexpiry.prop('disabled', true);
        }
        else {
            $passport.removeClass('disabled');
            $passport.prop('disabled', false);
            $passportexpiry.removeClass('disabled');
            $passportexpiry.prop('disabled', false);
            $passportattachment.show();
        }
        if (response.result.icadetails.formatvisanumber != null && response.result.icadetails.formatvisanumber != "" && response.result.icadetails.formatvisanumber != undefined) {
            $visaattachment.hide();
            $visa.val(response.result.icadetails.formatvisanumber);
            $visaexpiry.val(response.result.icadetails.formatvisaexpirydate);
            $visa.addClass('disabled');
            $visa.prop('disabled', true);
            $visaexpiry.addClass('disabled');
            $visaexpiry.prop('disabled', true);
        }
        else {
            $visa.removeClass('disabled');
            $visa.prop('disabled', false);
            $visaexpiry.removeClass('disabled');
            $visaexpiry.prop('disabled', false);
            $visaattachment.show();
        }
        if (response.result.icadetails.nationality != null && response.result.icadetails.nationality != "") {
            $nationality.val(response.result.icadetails.nationality);
            jQuery("#form-field-nationality").trigger('change');
            $nationality.addClass('disabled');
            $nationality.prop('disabled', true);
        }
        else {
            $nationality.removeClass('disabled');
            $nationality.prop('disabled', false);
        }

    }
    else {
        errormultifunc();
    }
}
function errormultifunc() {
    $visaattachment.show();
    $passportattachment.show();
    $eidattachment.show();
}
function successmultirtafunc(response) {
    if (response != null && response.result != null && response.result.rtadetails != null) {
        $dlattachment.hide(); 
        $vhattachment.hide();
        if (response.result.rtadetails.formatinsuranceexpirydate != null && response.result.rtadetails.formatinsuranceexpirydate != "" && response.result.rtadetails.formatinsuranceexpirydate != undefined) {
            $vehregis.val(response.result.rtadetails.formatinsuranceexpirydate);
            $vehregis.addClass('disabled');
            require(['picker'], function () {
                jQuery("#form-field-vehregis").pickadate().pickadate('picker').stop();
                $vehregis.prop('readonly', true);
            });
        }
        else {
            $vehregis.removeClass('disabled');
            $vehregis.prop('readonly', false);
        }
    }
    else {
        errormultirtafunc();
    }
}
function errormultirtafunc() {
    $dlattachment.show(); 
    $vhattachment.show();
    $vehregis.removeClass('disabled');
    $vehregis.prop('readonly', false);
}



function wploadvalidations() {
    $emiratesid = jQuery("#form-field-EmiratesID");
    $emiratesidexpirydate = jQuery("#form-field-EmiratesIDExpiry");
    $fullname = jQuery("#form-field-fullname");
    $profession = jQuery("#form-field-profession");
    $visa = jQuery("#form-field-visa");
    $visaexpiry = jQuery("#form-field-visaexpiry");
    $passport = jQuery("#form-field-Passportnumber");
    $passportexpiry = jQuery("#form-field-Passportexpiry");
    $nationality = jQuery("#form-field-nationality");
    $eidattachment = jQuery("#eiddocumentdiv");
    $passportattachment = jQuery("#passportdocumentdiv");
    $visaattachment = jQuery("#visadocumentdiv");
    $dlattachment = jQuery("#drivinglicensedocumentdiv");
    $platenumberid = jQuery("#form-field-PlateNumber");
    $platecodeid = jQuery("#form-field-PlateCode");
    $platecategoryid = jQuery("#form-field-categorycode");
    $plateemiratesid = jQuery("#form-field-emirates");
}

function wpcheckIDValidation() {
    if (jQuery($emiratesid).parsley().isValid()) {
        if ($emiratesid.val() != "" && $emiratesid.val() != undefined && $emiratesidexpirydate.val() != "" && $emiratesidexpirydate.val() != undefined) {
            Icadetails($emiratesid.val(), $emiratesidexpirydate.val(), "", "", "", "", true, wpepassbeforesendfunc, wpepasscompletefunc, wpsuccessfunc, wperrorfunc);
        }
    }
}

function wpcheckRTAValidation() {
    var selVal = jQuery("#form-field-emirates option:selected").val();
    if (selVal === "DXB") {
        if (jQuery($platenumberid).parsley().isValid()) {
            if ($platenumberid.val() != "" && $platenumberid.val() != undefined && jQuery("#form-field-categorycode option:selected").val() != "") {
                var response = Icadetails("", "", jQuery("#form-field-categorycode option:selected").val(), $platecodeid.val(), $platenumberid.val(), jQuery("#form-field-emirates option:selected").val(), false, wpepassbeforesendfunc, wpepasscompletefunc, wpsuccessrtafunc, wperrorrtafunc);
            }
        }
    }
    else {
        wperrorrtafunc();
    }
}


jQuery('.j113-epass--select_input').on('change', function () {
    jQuery('#SubContractorID').val(jQuery(this).val());
});
function wpepassbeforesendfunc() {
    jQuery('#Errormessage').html('');
    jQuery('.j113-epass--loader').show();
    jQuery('.j113-epass--loader').css('top', $(window).scrollTop());
    $('body').removeClass('unscrollable').addClass('unscrollable');
}
function wpepasscompletefunc() {
    jQuery('.j113-epass--loader').hide();
    $('body').removeClass('unscrollable');
}
function wpsuccessfunc(response) {
    if (response != null && response.result != null && response.result.icadetails != null) {
        $eidattachment.hide();
        if (response.result.icadetails.fullname != null && response.result.icadetails.fullname != "") {
            $fullname.val(response.result.icadetails.fullname);
            $fullname.addClass('disabled');
            $fullname.prop('readonly', true);
        }
        else {
            $fullname.removeClass('disabled');
            $fullname.prop('readonly', false);
        }
        $profession.val(response.result.icadetails.profession);
        if (response.result.icadetails.passportnumber != null && response.result.icadetails.passportnumber != "" && response.result.icadetails.passportnumber != undefined) {
            $passportattachment.hide();
            $passport.val(response.result.icadetails.passportnumber);
            $passportexpiry.val(response.result.icadetails.formatpassportexpirydate);
            $passport.addClass('disabled');
            $passport.prop('readonly', true);
            $passportexpiry.addClass('disabled');
            require(['picker'], function () {
                jQuery("#form-field-Passportexpiry").pickadate().pickadate('picker').stop();
                $passportexpiry.prop('readonly', true);
            });
        }
        else {
            $passport.removeClass('disabled');
            $passport.prop('readonly', false);
            $passportexpiry.removeClass('disabled');
            $passportexpiry.prop('readonly', false);
            $passportattachment.show();
        }

        if (response.result.icadetails.formatvisanumber != null && response.result.icadetails.formatvisanumber != "" && response.result.icadetails.formatvisanumber != undefined) {
            $visaattachment.hide();
            $visa.val(response.result.icadetails.formatvisanumber);
            $visaexpiry.val(response.result.icadetails.formatvisaexpirydate);
            $visa.addClass('disabled');
            $visa.prop('readonly', true);
            $visaexpiry.addClass('disabled');
            require(['picker'], function () {
                jQuery("#form-field-visaexpiry").pickadate().pickadate('picker').stop();
                $visaexpiry.prop('readonly', true);
            });
        }
        else {
            $visa.removeClass('disabled');
            $visa.prop('readonly', false);
            $visaexpiry.removeClass('disabled');
            revivePicker('form-field-visaexpiry');
            $visaattachment.show();
        }
        if (response.result.icadetails.nationality != null && response.result.icadetails.nationality != "") {
            $nationality.val(response.result.icadetails.nationality);
            jQuery("#form-field-nationality").trigger('change');
            $nationality.closest('.form-field').hide();
            $('#form-field-nationalityh').val($nationality.val());
            $('#form-field-nationalityh').closest('.form-field').show();
        }
        else {
            $nationality.removeClass('disabled');
            $nationality.prop('readonly', false);
        }
    }
    else {
        wperrorfunc();
    }
}
function wperrorfunc() {
    $fullname.val("");
    $fullname.removeAttr('readonly');
    $visa.val("");
    $profession.val("");
    $visa.removeAttr('readonly');
    $visaexpiry.val("");
    $visaexpiry.removeClass('disabled');
    revivePicker('form-field-visaexpiry');
    $passport.val("");
    $passport.removeAttr('readonly');
    $passportexpiry.val("");
    $passportexpiry.removeClass('disabled');
    revivePicker('form-field-Passportexpiry');
    $nationality.val("");
    $nationality.closest('.form-field').show();
    $('#form-field-nationalityh').closest('.form-field').hide();
    jQuery("#form-field-nationality").trigger('change');
}
function wpsuccessrtafunc(response) {
    if (response != null && response.result != null && response.result.rtadetails != null) {
        $dlattachment.hide();
    }
    else {
        wperrorrtafunc();
    }
}
function wperrorrtafunc() {
    $dlattachment.show();
}
