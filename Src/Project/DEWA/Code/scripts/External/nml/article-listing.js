$(function () {
	$(".form-filter").on('change', changeFormFilterHandler);

	function changeFormFilterHandler(event) {
		event.preventDefault();
		$("#page-value").val(1);	// Reset page
		var $form = $("#listing-form");
		var ajax = ajaxifiedForm($form);
		ajax.done(changeFormFilterSuccessHandler);
	}

	function changeFormFilterSuccessHandler(response) {
		$("#loadmore").remove();
		$("#listing-children").html(response);
	}
});