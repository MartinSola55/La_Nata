// --------------add active class-on another-page move----------
jQuery(document).ready(function ($) {
	// Get current path and find target link
	var path = window.location.pathname.split("/");

	// Account for home page with empty path
	if (path[1] == '') {
		path[1] = 'Home';
	}

	var target = $('#' + path[1] + 'Link');
	// Add active class to target link
	target.parent().addClass('active');
});


$(".navbar-toggler").click(function () {
	$(".navbar-collapse").slideToggle(300);
});