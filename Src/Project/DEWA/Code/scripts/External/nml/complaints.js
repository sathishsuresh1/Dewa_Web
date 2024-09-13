docReady(function () {
	jQuery(function ($) {
		$("#complaint-form").on('submit', submitFormHandler);

		function submitFormHandler(event) {
			var anyErrors = $(this).find(".parsley-custom-error-message").length > 0;
			console.log(anyErrors);
			if (anyErrors) event.preventDefault();

			$("#submit-form")
				.prop('disabled', true)
				.text('Submitting...');
		}
	});
});