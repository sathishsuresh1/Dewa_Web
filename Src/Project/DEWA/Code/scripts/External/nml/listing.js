docReady(function () {


    (function () {

        if (performance.navigation.type == 2) {
            setTimeout(function () { changeFormFilterHandlerload(); },1000)
        } else {
            changeFormFilterHandlerload();
        }
    })();

  

    $(document).on('click', "#loadmore", loadMoreFormHandler);
    $(".form-filter").on('change', changeFormFilterHandler);

    function changeFormFilterHandler(event) {
        $(".m66-preloader-fullpage").show();
        $(".parent-container").html("");
        $(".parent-container").prop("id", "listing-children");
        event.preventDefault();
        $("#page-value").val(1); // Reset page
        var $form = $("#listing-form");
        var ajax = ajaxifiedForm($form);
        ajax.done(changeFormFilterSuccessHandler);
    }

    function changeFormFilterHandlerload() {
        $(".m66-preloader-fullpage").show();
        $("#page-value").val(1); // Reset page
        var $form = $("#listing-form");
        var ajax = ajaxifiedForm($form);
        ajax.done(changeFormFilterSuccessHandler);       
    }

    function loadMoreFormHandler(event) {
        $(".m66-preloader-fullpage").show();
        event.preventDefault();
        var $this = $(this);
        $this.text($this.data('text'));
        var page = parseInt($("#page-value").val());
        $("#page-value").val(page + 1);
        var $form = $("#listing-form");
        var ajax = ajaxifiedForm($form);
        ajax.done(olderStoriesSuccessHandler);
    }

    function olderStoriesSuccessHandler(response) {
        $("#loadmore").remove();
        $(".load-contianer").remove();
        $("#listing-children").append(response);
        $(".m66-preloader-fullpage").fadeOut();
    }

    function changeFormFilterSuccessHandler(response) {
        $("#loadmore").remove();
        $(".load-contianer").remove();
        $("#listing-children").html(response);
        $(".m66-preloader-fullpage").hide();
        window.initComponents('rs_area');
        jQuery(window).trigger('reinit_tooltip');
    }
});
