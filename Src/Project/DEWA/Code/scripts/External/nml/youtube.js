
docReady(function() {
	jQuery(function ($) {
		var $frame = $('.youtube-frame');
		var id = $frame.attr('id');
		var channel = $frame.data('channel');
		var ytApiKey = $frame.data('api');

		var loadFrame = function (response) {
			//debugger;
			var latestVideo = response.items[0];
			if (!latestVideo) return;

			var thumbnail = latestVideo.snippet.thumbnails.medium;

			$frame.find('.m15-social-media-feed__image')
				.attr('data-src', thumbnail.url)
				.css('background-image', 'url(' + thumbnail.url + ')');

			var videoId = latestVideo.snippet.resourceId.videoId;
			var descr = latestVideo.snippet.description;
			var date = new Date(Date.parse(latestVideo.snippet.publishedAt));
			var length = descr.length;
			var shortened = (length > 80 ? descr.substr(0, 80) : descr) + '...';

			$frame.find('.m15-social-media-feed__text').text(shortened);
			$frame.find('.m15-social-media-feed__timestamp').text(timeAgo(date));
			$frame.find('.yt-link').attr('href', 'https://www.youtube.com/watch?v=' + videoId);
		}

		var handleChannelCall = function (response) {
			var playlistId = response.items[0].contentDetails.relatedPlaylists.uploads;

			return $.ajax({
				//beforeSend: function () {
				//	console.log('attaching spinner ' + id);
				//	window.attachSpinner(id);
				//},
				//complete: function () {
				//	console.log('detaching spinner ' + id);
				//	window.detachSpinner(id);
				//},
				url: 'https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&playlistId=' + playlistId + '&key=' + ytApiKey
			});
		};

		$.ajax({
			url: 'https://www.googleapis.com/youtube/v3/channels?part=contentDetails&forUsername=' + channel + '&key=' + ytApiKey
		})
		.then(handleChannelCall)
		.then(loadFrame);
	});
});
