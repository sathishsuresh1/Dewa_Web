var _evconsumptionGraphHandler = function () {
    _evcardslist();
    return true;
}
function evlistchange() {
    var evlistcards = jQuery("#form-field-evlistcards");
    evlistcards.on('change', function () {
        jQuery('.m29v3-chart--listitem--ev').removeClass('active');
        jQuery('.m29v3-chart--EV').find('.m29v3-chart--controls.active').removeClass('active');
        jQuery("#dp_month_ev1").val("");
        jQuery("#dp_month_ev2").val("");
        require(['vendor/jquery.monthpicker'], function () {
            jQuery('.m29v3-chart--EV').find('.form-field__input--monthpicker').each(function () {
                jQuery(this).datepicker().data('datepicker').destroy();
            });
        });
        ev_monthlyDataLoaded = false;
        ev_YearDataLoaded = false;
        GetEVDataYearly();
        jQuery('.m29v3-chart--listitem--ev[data-index="1"]').addClass('active');

    });
}
function HandleEVTabChange(e) {
    var tab_clicked = $(this).attr("data-index");
    var curr_date = new Date();
    switch (tab_clicked) {
        case "2":
            if ($(this).hasClass("active")) {
                break;
            }
            jQuery(dvEV).attr("data-x-axis-text", jQuery(dvEV).attr("data-x-axis-daytext"));
            ev_YearDataLoaded = false;
            if (ev_monthlyDataLoaded === false) {
                jQuery("#dp_month_ev1").val('');
                jQuery("#dp_month_ev2").val('');

                

                require(['vendor/jquery.monthpicker'], function () {
                    SetDateRangeEV(jQuery("#form-field-evlistcards option:selected").attr("data-activationdate"), jQuery("#form-field-evlistcards option:selected").attr("data-deactivationdate"));
                    
                });
                break;
            }
            EVInitialzeGraph(EVGetMonthXis(), ev_month_obj, 'Day');
            break;

        case "1":
            jQuery(dvEV).attr("data-x-axis-text", jQuery(dvEV).attr("data-x-axis-monthtext"));
            ev_monthlyDataLoaded = false;
            if (ev_YearDataLoaded == false) {
                GetEVDataYearly();
            }
            break;
    }
}

function HandleEVDateChange(e) {
    switch ($(this).attr("id")) {
        case dpMonth1_ev:
            setTimeout(function (e) {
                var m = parseInt(jQuery(e).attr("data-month")) + 1;
                var strmonth = m.toString();
                var stryear = jQuery(e).attr("data-year");
                if (stryear == 'NaN' && strmonth == 'NaN') {
                    ev_month_obj[0].data = [];
                    ev_month_obj[0].name = '';
                    EVInitialzeGraph(EVGetMonthXis(), ev_month_obj, 'Day')
                }
                else {
                    GetEVDataYearly(strmonth, stryear, 0);
                }
            }, 300, "#" + jQuery(this).attr("id"));
            break;
        case dpMonth2_ev:
            setTimeout(function (e) {
                var m = parseInt(jQuery(e).attr("data-month")) + 1;
                var strmonth = m.toString();
                var stryear = jQuery(e).attr("data-year");
                if (stryear == 'NaN' && strmonth == 'NaN') {
                    ev_month_obj[1].data = [];
                    ev_month_obj[1].name = '';
                    EVInitialzeGraph(EVGetMonthXis(), ev_month_obj, 'Day')
                }
                else {
                    GetEVDataYearly(strmonth, stryear, 1);
                }
            }, 300, '#' + $(this).attr("id"));
            break;
    }
}
var _evcardslist = function () {
    var accountNumber = jQuery("input[name='SelectedAccountNumber']:checked").val();
    var url = "/api/EVCards/Account/";
    jQuery.ajax({
        type: 'GET',
        url: url,
        beforeSend: function (jqXHR) {
            xhrPool.push(jqXHR);
            window.attachSpinner(dvEV, {
                bgColor: '#000',
                opacity: 0.3,
                minHeight: 480,
                zIndex: 1000
            });

            //jQuery('.m29v3-chart--controls').removeClass('active');
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
                    //window.detachSpinner(dvEV);
                    jQuery(dvEV).trigger(nodata_ev_el);
                }
                else if (response != null) {
                    renderEVCards(response);
                    GetEVDataYearly();
                }
            }
        },
        error: function (jqXHR, exception) {
            window.detachSpinner(dvEV);
            jQuery(dvEV).trigger(nodata_ev_el);
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

            window.detachSpinner(dvEV);
            //jQuery(dvEV).trigger(nodata_ev_el);
        }
    });
}
function renderEVCards(context) {
    var markup = jQuery('#evcards-select-template').html();
    var template = Handlebars.compile(markup);
    var rendering = template(context);
    jQuery('#selectevcardsdiv').html(rendering);
    evlistchange();
}

function GetEVDataYearly(month, year, count) {
    var acc = jQuery("input[name='SelectedAccountNumber']:checked").val();
    if (month == undefined)
        month = "";
    if (year == undefined)
        year = "";
    if (count == undefined)
        count = 0;
    var url = "/api/EVConsumptionGraph/Card/";
    jQuery.ajax({
        type: 'GET',
        url: url,
        beforeSend: function (jqXHR) {
            xhrPool.push(jqXHR);
            window.attachSpinner(dvEV, {
                bgColor: '#000',
                opacity: 0.3,
                minHeight: 480,
                zIndex: 1000
            });
            //jQuery('.m29v3-chart--controls').removeClass('active');
        },
        data: {
            accountnumber: accountNumber,
            cardid: jQuery("#form-field-evlistcards option:selected").val(),
            month: month,
            year: year
        },
        dataType: 'json',
        method: 'GET',
        async: true,
        success: function (response) {
            window.detachSpinner(dvEV);
            if (response != null) {
                if (month != "" && year != "") {
                    ev_monthlyDataLoaded = true;

                    if (response.series != null && response.series[0] != null && response.series[0].DataPoints != null && response.series[0].DataPoints[0] != null) {
                        ev_month_obj[count].data = response.series[0].DataPoints[0].dValues;
                        ev_month_obj[count].name = GetMonthName(month, false) + ' ' + year;
                        EVInitialzeGraph(EVGetMonthXis(), ev_month_obj, 'Day')
                    }
                    else if (ev_month_obj[0].data.length == 0 && ev_month_obj[1].data.length == 0) {
                        jQuery(dvEV).trigger(nodata_ev_el);
                    }
                    else if ((count == 1 && ev_month_obj[0].data.length > 0) || (count == 0 && ev_month_obj[1].data.length > 0)) {
                        ev_month_obj[count].data = [];
                        ev_month_obj[count].name = '';
                        EVInitialzeGraph(EVGetMonthXis(), ev_month_obj, 'Day')
                    }
                    else {
                        ev_month_obj[count].data = [];
                        ev_month_obj[count].name = '';
                        jQuery(dvEV).trigger(nodata_ev_el);
                    }
                }
                else if (response.series != null) {
                    ev_YearDataLoaded = true;
                    ev_monthlyDataLoaded = false;
                    EVGraphLoad(response);
                }
                else {
                    jQuery(dvEV).trigger(nodata_ev_el);
                }
            }
            else {
                jQuery(dvEV).trigger(nodata_ev_el);
            }
        },
        complete: function (response) {
            globalaccountnumber = acc;
            getGaguageLoadSettting("_evconsumptionGraph").isComplete = 1;
            _graphInitiate = false;
        },
        error: function (response) {
            window.detachSpinner(dvEV);
            jQuery(dvEV).trigger(nodata_ev_el);
        },
    });
    return false;
}

function EVGraphLoad(r) {

    window.attachSpinner(dvEV, {
        bgColor: '#000',
        opacity: 0.3,
        minHeight: 480,
        zIndex: 1000
    });
    var e = jQuery.grep(r.series, function (v) {
        return v.Utility === 2;
    });

    if (e) {
        EVformatYearData(e, ev_year_obj);
        EVInitialzeGraph(EVGetYearXaxis(), ev_year_obj, 'Month');
        window.detachSpinner(dvEV);
        return true;
    }
    window.detachSpinner(dvEV);
    jQuery(dvEV).trigger(nodata_ev_el);
}

function EVInitialzeGraph(xis, data, lbl) {
    var d = jQuery.grep(data, function (v) {
        return v.data.length > 0;
    });

    if (d === null || d.length === 0) {
        jQuery(dvEV).attr("data-x-axis-categories", "");
        jQuery(dvEV).attr("data-series", "");
        jQuery(dvEV).trigger(nodata_ev_el);
        return;
    }

    jQuery(dvEV).attr("data-x-axis-categories", xis);
    jQuery(dvEV).attr("data-series", JSON.stringify(data));

    jQuery(dvEV).attr("data-usagefor", EVGetLabelTranslation(lbl));
    //jQuery(dvEV).trigger('m29v3-chart-initEV');
    setTimeout(function () {

        jQuery(dvEV).trigger('m29v3-chart-initEV');
        setTimeout(function () {

            jQuery('.m29v3-chart--listitem--ev.active').click();
            window.detachSpinner(dvEV);
        }, 100);
        //jQuery('.m29v3-chart--listitem--ev.active').click();
        ////jQuery('.m29v3-chart--controls').addClass('active');
        //window.detachSpinner(dvEV);
    }, 200);
}

function EVGetMonthXis() {
    var m = '';
    for (i = 1; i < 32; i++) {
        m += i.toString();
        if (i !== 31) {
            m += ',';
        }
    }
    return m;
}

function allZeroes1(arr) {
    var r = true;
    $.each(arr, function (i, v) {
        if (v > 0)
            r = false;
        return r;
    });
    return r;
}

function EVformatYearData(e, o) {

    jQuery(e).each(function (index, obj) {
        var thisYear = jQuery.grep(e, function (v, i) {
            return (v.Legend === obj.Legend);
        });
        o[index].data = [];
        o[index].name = obj.Legend;
        if (thisYear && thisYear.length > 0) {
            var monthindex;
            for (monthindex = 1; monthindex <= 12; monthindex++) {
                var dataValue = null;
                jQuery.each(thisYear[0].DataPoints, function (i, v) {
                    if (monthindex == v.Month) {
                        dataValue = v.dValue;
                    }
                });
                o[index].data.push(dataValue);
            }

        }
    });

}

function EVGetLabelTranslation(str) {
    var dir = jQuery('html').attr('dir');
    switch (str) {
        case "Hour":
            return dir === "rtl" ? " الاستخدام في الساعة" : "Usage Per Hour";
        case "Day":
            return dir === "rtl" ? " الاستخدام اليومي" : "Usage Per Day";
        case "Month":
            return dir === "rtl" ? " الاستخدام الشهري" : "Usage Per Month";
        default:
            return dir === "rtl" ? "Usage Per " : "Usage Per ";
    }
}


function EVGetYearXaxis() {
    var dir = jQuery('html').attr('dir');

    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var months_arabic = ['يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو', 'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'];

    if (dir === 'rtl') {
        return months_arabic;
    } else {
        return monthNames;
    }
}

function SetDateRangeEV(min, max) {
    requirejs(['picker'], function () {
        var dtm = moment(min, 'YYYY-MM-DD HH:mm').toDate();
        var dtx = moment(max, 'YYYY-MM-DD HH:mm').toDate();
        jQuery('#' + dpMonth1_ev).attr('data-mindate', GetMonthDate(min));
        jQuery('#' + dpMonth2_ev).attr('data-mindate', GetMonthDate(min));
        if (max == '0000-00-00') {
            var newdate = new Date()
            jQuery('#' + dpMonth1_ev).attr('data-maxdate', GetMonthDate(newdate));
            jQuery('#' + dpMonth2_ev).attr('data-maxdate', GetMonthDate(newdate));
            
        }
        jQuery(window).trigger('changeMinMonth');
        var emid = "#" + dpMonth1_ev;
        jQuery(emid).datepicker().data('datepicker').selectDate(new Date());
    });
}