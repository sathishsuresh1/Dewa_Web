// Via: http://stackoverflow.com/questions/21784134/add-or-update-query-string-parameter-url-remains-unchanged
function UpdateQueryString(key, value, url) {
    //this flag is to check if you want to change the current page url
    //var iscurrentpage = false;
    //if (!url) {
    //	url = window.location.href;
    //	iscurrentpage = true;
    //}
    var re = new RegExp("([?&])" + key + "=.*?(&|#|$)(.*)", "gi");

    //this variable would be used as the result for this function
    var result = null;

    if (re.test(url)) {
        if (typeof value !== 'undefined' && value !== null) result = url.replace(re, '$1' + key + "=" + value + '$2$3');
        else {
            var hash = url.split('#');
            url = hash[0].replace(re, '$1$3').replace(/(&|\?)$/, '');
            if (typeof hash[1] !== 'undefined' && hash[1] !== null) url += '#' + hash[1];
            result = url;
        }
    } else {
        if (typeof value !== 'undefined' && value !== null) {
            var separator = url.indexOf('?') !== -1 ? '&' : '?',
                hash = url.split('#');
            url = hash[0] + separator + key + '=' + value;
            if (typeof hash[1] !== 'undefined' && hash[1] !== null) url += '#' + hash[1];
            result = url;
        } else result = url;
    }
    //if this is current page update the current page url
    //if (iscurrentpage) window.location.href = result;
    return result;
}

function ajaxifiedForm($form) {
    var type = $form.attr('method');
    var url = $form.attr('action');
    var data = $form.serialize();
    return jQuery.ajax({ type: type, url: url, data: data });
}

function timeAgo(date) {
    var now = new Date();
    var utc = new Date(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds());
    var difference = utc - date;
    var seconds = parseInt(difference / 1000);
    var minutes = parseInt(seconds / 60);
    var hours = parseInt(minutes / 60);
    var days = parseInt(hours / 24);
    if (days > 0) return days + " days ago";
    if (hours > 0) return hours + "h ago";
    if (minutes > 0) return minutes + "min ago";
    if (seconds > 0) return seconds + "s ago";
    return "Just now";
}

window.ParsleyConfig = {
    validators: {
        cannotbebeforetoday: {
            fn: function (value, requirements) {
                if (value == '')
                    return true;
                var now = new Date();
                var today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                return Date.parse(value) >= today;
            },
            priority: 32
        },
        cannotbeafterthreemonth: {
            fn: function (value, requirements) {
                if (value == '')
                    return true;
                var now = new Date();
                var futureMonth = new Date(now.getFullYear(), now.getMonth() + 3, now.getDate());
                return futureMonth >= Date.parse(value);
            },
            priority: 32
        },
        mustbetchecked: {
            fn: function (value, requirements) {
                return requirements.$element.is(':checked');
            },
            priority: 32
        }
    }
};

AddForgeryToken = function (data, formid) {
    data.__RequestVerificationToken = $('#' + formid + ' input[name=__RequestVerificationToken]').val();
    return data;
};
GoogleRecaptchaCallback = function (containerid, textvalue) {
    var v = grecaptcha.getResponse();
    if (v.length == 0) {
        jQuery('#' + containerid).text(textvalue);
        return false;
    }
    else {
        jQuery('#' + containerid).text("");
        return true;
    }
};
function steptrackerwithinpage(total, current, txt, info) {
    $('.m38-step-tracker').attr('data-total-steps', total);
    $('.m38-step-tracker').attr('data-current-step', current);
    $('.m38-step-tracker__progressbar').attr('aria-valuetext', txt);
    $('.m38-step-tracker').attr('data-hasInfo', info);

    jQuery(window).trigger('reinit_m38');
}
function removeSpaces(string) {
    return string.split(' ').join('');
}
Icadetails = function (eid, eidexp, pcat, pcode, pno, psrc, ica, beforesendfunc, completefunc, successfunc, errorfunc) {
    var url = "/api/ICAUserDetails/";
    jQuery.ajax({
        type: 'GET',
        url: url,
        beforeSend: beforesendfunc,
        data: {
            eid: eid,
            eidexp: eidexp,
            pcat: pcat,
            pcode: pcode,
            pno: pno,
            psrc: psrc,
            ica: ica
        },
        complete: completefunc,
        dataType: 'json',
        method: 'GET',
        async: true,
        success: function (response) {
            successfunc(response);
        }, error: function (response) {
            errorfunc();
        }
    });
}

jQuery(function (n) { function i(i) { i.preventDefault(); var o = n("#hiform"); o.submit() } n(".fancybox").fancybox(), n("#hitrigger").on("click", i), n(".non_arabic_link").on("click", function (i) { return n(i.id).is("#modal2") || (n(".dewa_noar").trigger("click"), window.setTimeout(function () { window.location.href = i.target }, 3e3)), !1 }) });
var lang = jQuery('html').attr('dir') == 'ltr' ? 'en' : 'ar';
moment.locale(lang);
window.attachSpinner = function (el, options) {
    var defaults = {
        zIndex: 0,
        minHeight: 300,
        bgColor: 'transparent',
        bgPosition: 'center center',
        opacity: 1
    };
    var opts = jQuery.extend(defaults, options);
    if (typeof el === 'string') {
        el = jQuery(el);
    }
    jQuery(el).animate({ minHeight: opts.minHeight }, {
        duration: 0, // iPad Mini cannot handle the CPU intesiveness of animation. Disabling for the time-being.
        complete: function () {
            var $el = jQuery(this);
            var $overlay = jQuery(document.createElement('div'))
                .addClass('ajax-loader-overlay')
                .css({
                    padding: $el.css('padding'),
                    width: $el.width(),
                    height: $el.height(),
                    minWidth: $el.css('min-width'),
                    minHeight: $el.css('min-height'),
                    zIndex: opts.zIndex,
                    backgroundColor: opts.bgColor,
                    backgroundPosition: opts.bgPosition
                })
                .fadeTo(0, opts.opacity);
            $el.append($overlay);
        }
    });
};
window.detachSpinner = function (el) {
    if (typeof el === 'string') {
        el = document.getElementById(el.replace('#', ''));
    }
    jQuery(el).animate({ minHeight: 0 }, {
        duration: 0 // iPad Mini cannot handle the CPU intesiveness of animation. Disabling for the time-being.
    }).find('.ajax-loader-overlay').remove();
}

jQuery(document).ready(function () {
    if (jQuery("#site_search").length > 0 && jQuery("#site_search").val() != "") {
        jQuery(".m31-search--dm_linkTitle,.m31-search--dm_linkDesc").highlight(jQuery("#site_search").val(), {})
    }
});