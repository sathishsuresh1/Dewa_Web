function SetDateRangeE(min, max) {
    requirejs(['picker'], function () {
        var dtm = moment(min, 'YYYY-MM-DD HH:mm').toDate();
        var dtx = moment(max, 'YYYY-MM-DD HH:mm').toDate();
        var dp_d_e1 = jQuery('#' + dpDay1_e).pickadate('picker');
        var dp_d_e2 = jQuery('#' + dpDay2_e).pickadate('picker');

        dp_d_e1.set('min', dtm);
        dp_d_e2.set('min', dtm);

        jQuery('#' + dpMonth1_e).attr('data-mindate', GetMonthDate(min));
        jQuery('#' + dpMonth2_e).attr('data-mindate', GetMonthDate(min));
        jQuery(window).trigger('changeMinMonth')
        if (dtx.getFullYear() < 9999) {
            dp_d_e1.set('max', dtx);
            dp_d_e2.set('max', dtx);
            jQuery('#' + dpMonth1_e).attr('data-maxdate', GetMonthDate(max));
            jQuery('#' + dpMonth2_e).attr('data-maxdate', GetMonthDate(max));

            jQuery(window).trigger('changeMaxMonth')
        }

    });
}

function SetDateRangeW(min, max) {
    requirejs(['picker'], function () {
        var dtm = moment(min, 'YYYY-MM-DD HH:mm').toDate();
        var dtx = moment(max, 'YYYY-MM-DD HH:mm').toDate();

        var dp_d_w1 = jQuery('#' + dpDay1_w).pickadate('picker');
        var dp_d_w2 = jQuery('#' + dpDay2_w).pickadate('picker');

        dp_d_w1.set('min', dtm);
        dp_d_w2.set('min', dtm);


        jQuery('#' + dpMonth1_w).attr('data-mindate', GetMonthDate(min));
        jQuery('#' + dpMonth2_w).attr('data-mindate', GetMonthDate(min));
        jQuery(window).trigger('changeMinMonth')
        if (dtx.getFullYear() < 9999) {
            dp_d_w1.set('max', dtx);
            dp_d_w2.set('max', dtx);
            jQuery('#' + dpMonth1_w).attr('data-maxdate', GetMonthDate(max));
            jQuery('#' + dpMonth2_w).attr('data-maxdate', GetMonthDate(max));
            jQuery(window).trigger('changeMaxMonth')
        }
    });
}

var _consumptionGraphHandler = function () {
    jQuery(dvElectricity).find(tab_list_el).each(function (i, el) {
        jQuery(el).removeClass('active');
        if (jQuery(el).attr("data-index") === jQuery(dvElectricity).attr("data-default-tab")) {
            jQuery(el).addClass('active');
            e_day_obj = JSON.parse(JSON.stringify(e_day_obj_default));
            e_month_obj = JSON.parse(JSON.stringify(e_month_obj_default));
            e_year_obj = JSON.parse(JSON.stringify(e_year_obj_default));
            e_dailyDataLoaded = false;
            e_monthlyDataLoaded = false;
            e_yearlyDataLoaded = false;
            tab_change = false;
        }
    });
    jQuery(dvElectricity).find('.m29v3-chart--controls').each(function (i, el) {
        jQuery(el).removeClass('active');
        if (jQuery(el).attr("data-index") === jQuery(dvElectricity).attr("data-default-tab")) {
            jQuery(el).addClass('active');
        }
    });
    jQuery(dvElectricity).find('.m29v3-chart--typeitem').each(function (i, el) {
        jQuery(el).removeClass('m29v3-chart--typeitemactive');
        if (jQuery(el).attr("data-type") === "column") {
            jQuery(el).addClass('m29v3-chart--typeitemactive');
        }
    });

    jQuery(dvWater).find(tab_list_el).each(function (i, el) {
        jQuery(el).removeClass('active');
        if (jQuery(el).attr("data-index") === jQuery(dvWater).attr("data-default-tab")) {
            jQuery(el).addClass('active');
            w_day_obj = JSON.parse(JSON.stringify(w_day_obj_default));
            w_month_obj = JSON.parse(JSON.stringify(w_month_obj_default));
            w_year_obj = JSON.parse(JSON.stringify(w_year_obj_default));
            w_dailyDataLoaded = false;
            w_monthlyDataLoaded = false;
            w_yearlyDataLoaded = false;
            tab_change = false;
        }
    });
    jQuery(dvWater).find('.m29v3-chart--controls').each(function (i, el) {
        jQuery(el).removeClass('active');
        if (jQuery(el).attr("data-index") === jQuery(dvWater).attr("data-default-tab")) {
            jQuery(el).addClass('active');
        }
    });
    jQuery(dvWater).find('.m29v3-chart--typeitem').each(function (i, el) {
        jQuery(el).removeClass('m29v3-chart--typeitemactive');
        if (jQuery(el).attr("data-type") === "column") {
            jQuery(el).addClass('m29v3-chart--typeitemactive');
        }
    });
    GetDataYearly(null, null, dvElectricity, dvWater);
    //GetSmartMeterDetails();
    return true;
}

function ToggleTabs(rel, vl) {
    jQuery(rel).find(tab_list_el).each(function (i, el) {
        if (jQuery(el).hasClass("month_tab") || jQuery(el).hasClass("day_tab")) {
            if (vl === true) {
                jQuery(el).show();
            } else {
                jQuery(el).hide();
            }
        }
    });

}

function HandleTabChange(e) {

    var root_id = '#' + $(e.target).closest(".m29v3-chart").attr("id");
    var tab_clicked = $(this).attr("data-index");
    var curr_date = new Date();
    switch (tab_clicked) {
        case "0":
            if (e_dailyDataLoaded === false) {

                var d_id = "#" + dpDay1_e;

                var param_date = curr_date.getDate() + ' ' + GetMonthName(curr_date.getMonth() + 1, false) + ' ' + curr_date.getFullYear();
                var param_date_en = curr_date.getDate() + ' ' + GetMonthName(curr_date.getMonth() + 1, true) + ' ' + curr_date.getFullYear();

                jQuery(d_id).val(param_date);
                jQuery(d_id).attr("data-month", (curr_date.getMonth()).toString());
                jQuery(d_id).attr("data-year", curr_date.getFullYear().toString());

                jQuery(d_id).trigger("change");
                jQuery('input[name="' + dpDay1_e + '_submit"]').val(param_date_en);
                jQuery("#" + dpDay2_e).val("");

                break;
            }
            InitialzeGraph(e_day_x_axis, e_day_obj, 'Hour', root_id);
            break;
        case "3":
            if (w_dailyDataLoaded === false) {
                var param_date = curr_date.getDate() + ' ' + GetMonthName(curr_date.getMonth() + 1, false) + ' ' + curr_date.getFullYear();
                var param_date_en = curr_date.getDate() + ' ' + GetMonthName(curr_date.getMonth() + 1, true) + ' ' + curr_date.getFullYear();
                var dwid = "#" + dpDay1_w;

                jQuery(dwid).val(param_date);
                jQuery(dwid).attr("data-month", curr_date.getMonth());
                jQuery(dwid).attr("data-year", curr_date.getFullYear());

                jQuery(dwid).trigger("change");
                jQuery('[name="' + dpDay1_w + '_submit"]').val(param_date_en);
                jQuery("#" + dpDay2_w).val("");
                break;
            }
            InitialzeGraph(w_day_x_axis, w_day_obj, 'Hour', root_id);
            break;
        case "1":
            var messagetouse_e = $(this).attr('data-notfound-message');
            if (outage === false) {
                jQuery(root_id).find('.m29v3-chart__nodata-container .m29v3-chart__nodata').html(messagetouse_e);
                jQuery(root_id).attr("data-x-axis-text", jQuery(root_id).attr("data-x-axis-daytext"));
                if (e_monthlyDataLoaded === false) {

                    var emid = "#" + dpMonth1_e;

                    require(['vendor/jquery.monthpicker'], function () {
                        jQuery(emid).datepicker().data('datepicker').selectDate(new Date());
                    });
                    break;
                }
                InitialzeGraph(GetMonthXis(), e_month_obj, 'Day', root_id)
            }
            else {
                messagetouse_e = $(this).attr('data-outage-message');
                jQuery(root_id).find('.m29v3-chart__nodata-container .m29v3-chart__nodata').html(messagetouse_e);
                HandleNoData(root_id, 1);
            }
            break;
        case "4":
            var messagetouse_w = $(this).attr('data-notfound-message');
            if (outage === false) {
                jQuery(root_id).find('.m29v3-chart__nodata-container .m29v3-chart__nodata').html(messagetouse_w);
                jQuery(root_id).attr("data-x-axis-text", jQuery(root_id).attr("data-x-axis-daytext"));
                if (w_monthlyDataLoaded === false) {
                    var wmid = "#" + dpMonth1_w;
                    require(['vendor/jquery.monthpicker'], function () {
                        jQuery(wmid).datepicker().data('datepicker').selectDate(new Date());

                    })
                    break;
                }
                InitialzeGraph(GetMonthXis(), w_month_obj, 'Day', root_id)
            }
            else {
                messagetouse_w = $(this).attr('data-outage-message');
                jQuery(root_id).find('.m29v3-chart__nodata-container .m29v3-chart__nodata').html(messagetouse_w);
                HandleNoData(root_id, 4);
            }
            break;
        case "2":
            jQuery(root_id).attr("data-x-axis-text", jQuery(root_id).attr("data-x-axis-monthtext"));
            GetDataYearly(9, root_id);
            break;
        case "5":
            jQuery(root_id).attr("data-x-axis-text",jQuery(root_id).attr("data-x-axis-monthtext"));
            GetDataYearly(10, root_id);
            break;
    }

}

function HandleDateChange(e) {

    var root_id = '#' + jQuery(e.target).closest(".m29v3-chart").attr("id");

    if (outage === true) {
        jQuery(root_id).find('.m29v3-chart__nodata-container .m29v3-chart__nodata').html(jQuery(root_id).attr('data-outage-message'));
        jQuery(root_id).trigger(nodata_el);
        return;
    }

    var post_data = {
        __RequestVerificationToken: GetAFToken(),
        accountnumber: GetPremiseNumber(),
        date: '1',
        month: '1',
        year: '',
        usagetype: 'D',
        rtype: 'E'
    };

    post_data.date = jQuery(this).val();
    if (!$(this).attr("id") == dpMonth2_e || !$(this).attr("id") == dpMonth1_e) {
        if (!post_data.date.trim() || post_data.date.length < 1) {
            jQuery(root_id).trigger(nodata_el);
            return;
        }
    }
    
    
    switch ($(this).attr("id")) {

        case dpDay1_e:
            setTimeout(function (post_data, root_id, aid) {
                var submit_id = 'input[name="' + aid + '_submit"]';
                post_data.date = jQuery(submit_id).val();
                GetData(post_data, 1, root_id);
            }, 200, post_data, root_id, jQuery(this).attr("id"));
            break;
        case dpDay2_e:
            setTimeout(function (post_data, root_id, aid) {
                var submit_id = 'input[name="' + aid + '_submit"]';
                post_data.date = jQuery(submit_id).val();
                GetData(post_data, 2, root_id);
            }, 200, post_data, root_id, jQuery(this).attr("id"));

            break;
        case dpMonth1_e:

            setTimeout(function (o, e, r) {
                o.usagetype = "M";
                var m = parseInt(jQuery(e).attr("data-month")) + 1;
                o.month = m.toString();
                o.year = jQuery(e).attr("data-year");
                GetData(o, 3, r);
            }, 1000, post_data, "#" + jQuery(this).attr("id"), root_id);

            break;
        case dpMonth2_e:
            setTimeout(function (o, e, r) {
                o.usagetype = "M";
                var m = parseInt(jQuery(e).attr("data-month")) + 1;
                o.month = m.toString();
                o.year = jQuery(e).attr("data-year");

                GetData(o, 4, r);
            }, 500, post_data, '#' + $(this).attr("id"), root_id);
            break;
        case dpDay1_w:
            setTimeout(function (post_data, root_id, aid) {
                var submit_id = 'input[name="' + aid + '_submit"]';
                post_data.date = jQuery(submit_id).val();

                GetData(post_data, 5, root_id);
            }, 200, post_data, root_id, jQuery(this).attr("id"));

            break;
        case dpDay2_w:
            setTimeout(function (post_data, root_id, aid) {
                var submit_id = 'input[name="' + aid + '_submit"]';
                post_data.date = jQuery(submit_id).val();
                GetData(post_data, 6, root_id);
            }, 200, post_data, root_id, jQuery(this).attr("id"));

            break;
        case dpMonth1_w:
            setTimeout(function (o, e, r) {
                o.usagetype = "M";
                var m = parseInt(jQuery(e).attr("data-month")) + 1;
                o.month = m.toString();
                o.year = jQuery(e).attr("data-year");

                GetData(o, 7, r);
            }, 500, post_data, '#' + $(this).attr("id"), root_id);

            break;
        case dpMonth2_w:
            setTimeout(function (o, e, r) {
                o.usagetype = "M";
                var m = parseInt(jQuery(e).attr("data-month")) + 1;
                o.month = m.toString();
                o.year = jQuery(e).attr("data-year");
                GetData(o, 8, r);
            }, 500, post_data, '#' + $(this).attr("id"), root_id);
            break;
    }

}

function GetDataYearly(type, root_id, eroot_id, wroot_id) {
    var lbl = "Month";
    var acc = jQuery("input[name='SelectedAccountNumber']:checked").val();
    if (acc != null && acc != "") {
        if (type != null && type != undefined) {
            if (type === 9) {
                if (e_yearlyDataLoaded === true) {
                    window.attachSpinner(root_id, {
                        bgColor: '#000',
                        opacity: 0.3,
                        minHeight: 480,
                        zIndex: 1000
                    });
                    InitialzeGraph(GetYearXaxis(), e_year_obj, lbl, root_id);
                    window.detachSpinner(root_id);
                    return;
                }
            } else {
                if (w_yearlyDataLoaded === true) {
                    window.attachSpinner(root_id, {
                        bgColor: '#000',
                        opacity: 0.3,
                        minHeight: 480,
                        zIndex: 1000
                    });
                    InitialzeGraph(GetYearXaxis(), w_year_obj, lbl, root_id);
                    window.detachSpinner(root_id);
                    return;
                }
            }
        }
        if (consumptiongraphresponse == null || acc != globalaccountnumber) {
            var ajaxCallURL = '/api/ConsumptionStatistics/Consumption';
            jQuery.ajax({
                type: "POST",
                url: ajaxCallURL,
                traditional: true,
                data: {
                    id: acc,
                    __RequestVerificationToken: jQuery("#graphform input[name='__RequestVerificationToken']").val()
                },
                beforeSend: function () {
                    consumptiongraphresponse = null;
                    if (eroot_id == undefined && wroot_id == undefined) {
                        window.attachSpinner(root_id, {
                            bgColor: '#000',
                            opacity: 0.3,
                            minHeight: 480,
                            zIndex: 1000
                        });
                    } else {
                        window.attachSpinner(eroot_id, {
                            bgColor: '#000',
                            opacity: 0.3,
                            minHeight: 480,
                            zIndex: 1000
                        });
                        window.attachSpinner(wroot_id, {
                            bgColor: '#000',
                            opacity: 0.3,
                            minHeight: 480,
                            zIndex: 1000
                        });
                    }

                },
                success: function (r) {
                    consumptiongraphresponse = r;
                    if (eroot_id == undefined && wroot_id == undefined) {
                        window.detachSpinner(root_id);
                        GraphLoad(type, root_id, lbl, r);
                    } else {
                        GraphLoadBoth(eroot_id, wroot_id, lbl, r)
                    }

                },
                complete: function (response) {
                    globalaccountnumber = acc;
                    getGaguageLoadSettting("_consumptionGraph").isComplete = 1;
                    _graphInitiate = false;
                },
                error: function () {
                    if (eroot_id == undefined && wroot_id == undefined) {
                        window.detachSpinner(root_id);
                        jQuery(root_id).trigger(nodata_el);
                    } else {
                        window.detachSpinner(eroot_id);
                        jQuery(eroot_id).trigger(nodata_el);
                        window.detachSpinner(wroot_id);
                        jQuery(wroot_id).trigger(nodata_el);
                    }
                },
            });
        } else {
            GraphLoad(type, root_id, lbl, consumptiongraphresponse);
        }
    }
    return false;
}

function GraphLoad(type, root_id, lbl, r) {

    if (type === 9) {
        window.attachSpinner(root_id, {
            bgColor: '#000',
            opacity: 0.3,
            minHeight: 480,
            zIndex: 1000
        });
        var e = jQuery.grep(r.series, function (v) {
            return v.Utility === 0;
        });

        if (r.meter != null && r.meter.IsSmartElectricityMeter && r.meter.IsSmartElectricityMeter === true) {
            jQuery(root_id).find(".month_tab").show();
            jQuery(root_id).find(".consumptionmonthlytab").show();
            jQuery(root_id).find(".day_tab").show();
            if (r.meter.MoveInDate && r.meter.MoveOutDate) {
                SetDateRangeE(r.meter.MoveInDate, r.meter.MoveOutDate);
            }
            ToggleTabs(dvElectricity, true);
        } else {
            jQuery(root_id).find(".month_tab").hide();
            jQuery(root_id).find(".consumptionmonthlytab").hide();
            jQuery(root_id).find(".day_tab").hide();
            ToggleTabs(dvElectricity, false);
        }

        if (e) {
            formatYearData(e, e_year_obj);
            InitialzeGraph(GetYearXaxis(), e_year_obj, lbl, root_id);
            e_yearlyDataLoaded = true;
            window.detachSpinner(root_id);
            return true;
        }
    } else {
        window.attachSpinner(root_id, {
            bgColor: '#000',
            opacity: 0.3,
            minHeight: 480,
            zIndex: 1000
        });
        var w = jQuery.grep(r.series, function (v) {
            return v.Utility === 1;
        });
        if (r.meter != null && r.meter.IsSmartWaterMeter && r.meter.IsSmartWaterMeter === true) {
            jQuery(root_id).find(".month_tab").show();
            jQuery(root_id).find(".consumptionmonthlytab").show();
            jQuery(root_id).find(".day_tab").show();
            if (r.meter.MoveInDate && r.meter.MoveOutDate) {
                SetDateRangeW(r.meter.MoveInDate, r.meter.MoveOutDate);
            }
            ToggleTabs(dvWater, true);
        } else {
            jQuery(root_id).find(".month_tab").hide();
            jQuery(root_id).find(".consumptionmonthlytab").hide();
            jQuery(root_id).find(".day_tab").hide();
            ToggleTabs(dvWater, false);
        }

        if (w) {
            formatYearData(w, w_year_obj);
            InitialzeGraph(GetYearXaxis(), w_year_obj, lbl, root_id);
            w_yearlyDataLoaded = true;
            window.detachSpinner(root_id);
            return true;
        }

    }
    window.detachSpinner(root_id);
    $(root_id).trigger(nodata_el);
}

function GraphLoadBoth(eroot_id, wroot_id, lbl, r) {
    var e = jQuery.grep(r.series, function (v) {
        return v.Utility === 0;
    });

    if (r.meter != null && r.meter.IsSmartElectricityMeter && r.meter.IsSmartElectricityMeter === true) {
        jQuery(eroot_id).find(".month_tab").show();
        jQuery(eroot_id).find(".consumptionmonthlytab").show();
        jQuery(eroot_id).find(".day_tab").show();
        if (r.meter.MoveInDate && r.meter.MoveOutDate) {
            SetDateRangeE(r.meter.MoveInDate, r.meter.MoveOutDate);
        }
        ToggleTabs(dvElectricity, true);
    } else {
        jQuery(eroot_id).find(".month_tab").hide();
        jQuery(eroot_id).find(".consumptionmonthlytab").hide();
        jQuery(eroot_id).find(".day_tab").hide();
        ToggleTabs(dvElectricity, false);
    }

    if (e) {
        formatYearData(e, e_year_obj);
        InitialzeGraph(GetYearXaxis(), e_year_obj, lbl, eroot_id);
        e_yearlyDataLoaded = true;
        window.detachSpinner(eroot_id);
    } else {
        window.detachSpinner(eroot_id);
        $(eroot_id).trigger(nodata_el);
    }
    var w = jQuery.grep(r.series, function (v) {
        return v.Utility === 1;
    });
    if (r.meter != null && r.meter.IsSmartWaterMeter && r.meter.IsSmartWaterMeter === true) {
        jQuery(wroot_id).find(".month_tab").show();
        jQuery(wroot_id).find(".consumptionmonthlytab").show();
        jQuery(wroot_id).find(".day_tab").show();
        if (r.meter.MoveInDate && r.meter.MoveOutDate) {
            SetDateRangeW(r.meter.MoveInDate, r.meter.MoveOutDate);
        }
        ToggleTabs(dvWater, true);

    } else {
        jQuery(wroot_id).find(".month_tab").hide();
        jQuery(wroot_id).find(".consumptionmonthlytab").hide();
        jQuery(wroot_id).find(".day_tab").hide();
        ToggleTabs(dvWater, false);
    }

    if (w) {
        formatYearData(w, w_year_obj);
        InitialzeGraph(GetYearXaxis(), w_year_obj, lbl, wroot_id);
        w_yearlyDataLoaded = true;
        window.detachSpinner(wroot_id);
    } else {
        $(wroot_id).trigger(nodata_el);
    }
    return true;
}

function GetData(postData, type, root_id) {
    if (type < 5) {
        postData.rtype = 'E'
    } else {
        postData.rtype = 'W';
    }
    var lbl_day = "Day";
    var lbl_hour = "Hour";
    jQuery.ajax({
        type: "POST",
        url: api_url,
        data: postData,
        beforeSend: function () {
            window.attachSpinner(root_id, {
                bgColor: '#000',
                opacity: 0.3,
                minHeight: 480,
                zIndex: 1000
            });
        },
        complete: function () {
            window.detachSpinner(root_id);
        },
        success: function (res) {
            if (res.data && !allZeroes(res.data)) {
                switch (type) {
                    case 1:
                        e_day_obj[0].data = res.data;
                        e_day_obj[0].name = postData.date;
                        InitialzeGraph(e_day_x_axis, e_day_obj, lbl_hour, root_id);
                        e_dailyDataLoaded = true;
                        break;
                    case 2:
                        e_day_obj[1].data = res.data;
                        e_day_obj[1].name = postData.date;

                        InitialzeGraph(e_day_x_axis, e_day_obj, lbl_hour, root_id);

                        break;
                    case 3:
                        e_month_obj[0].data = res.data;
                        e_month_obj[0].name = GetMonthName(postData.month, false) + ' ' + postData.year;
                        InitialzeGraph(GetMonthXis(), e_month_obj, lbl_day, root_id)
                        e_monthlyDataLoaded = true;
                        break;
                    case 4:

                        e_month_obj[1].data = res.data;
                        e_month_obj[1].name = GetMonthName(postData.month, false) + ' ' + postData.year;

                        InitialzeGraph(GetMonthXis(), e_month_obj, lbl_day, root_id)

                        break;
                    case 5:
                        w_day_obj[0].data = res.data;
                        w_day_obj[0].name = postData.date;
                        InitialzeGraph(w_day_x_axis, w_day_obj, lbl_hour, root_id);
                        w_dailyDataLoaded = true;
                        break;
                    case 6:
                        w_day_obj[1].data = res.data;
                        w_day_obj[1].name = postData.date;

                        InitialzeGraph(w_day_x_axis, w_day_obj, lbl_hour, root_id);

                        break;
                    case 7:
                        w_month_obj[0].data = res.data;
                        w_month_obj[0].name = GetMonthName(postData.month, false) + ' ' + postData.year;
                        InitialzeGraph(GetMonthXis(), w_month_obj, lbl_day, root_id)
                        w_monthlyDataLoaded = true;
                        break;
                    case 8:

                        w_month_obj[1].data = res.data;
                        w_month_obj[1].name = GetMonthName(postData.month, false) + ' ' + postData.year;

                        InitialzeGraph(GetMonthXis(), w_month_obj, lbl_day, root_id)

                        break;
                }
            } else {
                HandleNoData(root_id, type);
            }
        }
    });
}

function InitialzeGraph(xis, data, lbl, graph_id) {
    var d = jQuery.grep(data, function (v) {
        return v.data.length > 0;
    });

    if (d === null || d.length === 0) {
        jQuery(graph_id).attr("data-x-axis-categories", "");
        jQuery(graph_id).attr("data-series", "");
        jQuery(graph_id).trigger(nodata_el);
        return;
    }

    jQuery(graph_id).attr("data-x-axis-categories", xis);
    jQuery(graph_id).attr("data-series", JSON.stringify(data));

    jQuery(graph_id).attr("data-usagefor", GetLabelTranslation(lbl));

    setTimeout(function () {
        jQuery(graph_id).trigger('m29v3-chart-init');
        if (w_yearlyDataLoaded === true) { } else {
            window.detachSpinner(graph_id);
        }
    }, 100);
}

function GetPremiseNumber() {
    return jQuery("input[name='SelectedAccountNumber']:checked").attr("data-legacy-acc-number");
}

function GetAFToken() {
    return jQuery("input[name='__RequestVerificationToken']").val()
}

function GetMonthName(monthNumber, eng) {
    var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
    if (eng === true) {
        return months[monthNumber - 1];
    }
    var months_arabic = ['يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو', 'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'];

    if (jQuery('html').attr('dir') == 'rtl') {
        return months_arabic[monthNumber - 1];
    } else {
        return months[monthNumber - 1];
    }
}

function GetMonthXis() {
    var m = '';
    for (i = 1; i < 32; i++) {
        m += i.toString();
        if (i !== 31) {
            m += ',';
        }
    }
    return m;
}

function HandleNoData(root_id, type) {
    switch (type) {
        case 1:
            e_day_obj[0].data = [];
            e_day_obj[0].name = '';
            if (e_day_obj[1].data.length > 0) {
                InitialzeGraph(e_day_x_axis, e_day_obj, '', root_id);
            } else {
                jQuery(root_id).trigger(nodata_el);
            }
            break;
        case 2:
            e_day_obj[1].data = [];
            e_day_obj[1].name = '';
            if (e_day_obj[0].data.length > 0) {
                InitialzeGraph(e_day_x_axis, e_day_obj, '', root_id);
            } else {
                jQuery(root_id).trigger(nodata_el);
            }

            break;
        case 5:
            w_day_obj[0].data = [];
            w_day_obj[0].name = ''
            if (w_day_obj[1].data.length > 0) {
                InitialzeGraph(w_day_x_axis, w_day_obj, '', root_id);
            } else {
                jQuery(root_id).trigger(nodata_el);
            }
            break;

        case 6:
            w_day_obj[1].data = [];
            w_day_obj[1].name = ''
            if (w_day_obj[0].data.length > 0) {
                InitialzeGraph(w_day_x_axis, w_day_obj, '', root_id);
            } else {
                jQuery(root_id).trigger(nodata_el);
            }

            break;
        case 3:
            e_month_obj[0].data = [];
            e_month_obj[0].name = ''
            if (e_month_obj[1].data.length > 0) {
                InitialzeGraph(GetMonthXis(), e_month_obj, 'day', root_id);
            } else {
                jQuery(root_id).trigger(nodata_el);
            }
            break;
        case 4:
            e_month_obj[1].data = [];
            e_month_obj[1].name = ''
            if (e_month_obj[0].data.length > 0) {
                InitialzeGraph(GetMonthXis(), e_month_obj, 'day', root_id);
            } else {
                jQuery(root_id).trigger(nodata_el);
            }
            break;
        case 7:
            w_month_obj[0].data = [];
            w_month_obj[0].name = ''
            if (w_month_obj[1].data.length > 0) {
                InitialzeGraph(GetMonthXis(), w_month_obj, 'day', root_id);
            } else {
                jQuery(root_id).trigger(nodata_el);
            }
            break;
        case 8:
            w_month_obj[1].data = [];
            w_month_obj[1].name = '';
            if (w_month_obj[0].data.length > 0) {
                InitialzeGraph(GetMonthXis(), w_month_obj, 'day', root_id);
            } else {
                jQuery(root_id).trigger(nodata_el);
            }
            break;
    }

}

function allZeroes(arr) {
    var r = true;
    $.each(arr, function (i, v) {
        if (v > 0)
            r = false;
        return r;
    });
    return r;
}

function formatYearData(e, o) {

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
                        dataValue = v.Value;
                    }
                });
                o[index].data.push(dataValue);
            }

        }
    });

}

function GetLabelTranslation(str) {
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

function GetMonthDate(date) {
    var d = moment(date, 'YYYY-MM-DD HH:mm').toDate();
    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    return d.getFullYear() + '-' + (d.getMonth()+1);
}

function GetYearXaxis() {
    var dir = jQuery('html').attr('dir');

    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var months_arabic = ['يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو', 'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'];

    if (dir === 'rtl') {
        return months_arabic;
    } else {
        return monthNames;
    }
}