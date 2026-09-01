class Settings {

	static URL_WEB = '/exp/';
	static URL_API = Settings.URL_WEB + 'api/';
	static URL_API_EXP = Settings.URL_API + 'Experiment/';					// ExperimentApi controller

	static URL_API_WORD = Settings.URL_API + 'Word?lang=current';
	static URL_API_OBJECT = Settings.URL_API + 'Object/';
	static URL_API_DEVICE = Settings.URL_API + 'Device/';
	static URL_API_DATAPOINT = Settings.URL_API + 'Datapoint/';
	static URL_API_DATAPOINT_VALUE = Settings.URL_API + 'DatapointValue/';

	// WWW
	static URL_API_CONTENT = Settings.URL_WEB + 'Content/';
	static URL_API_CONTENT_IMG = Settings.URL_API_CONTENT + 'img/';
	static URL_API_CONTENT_IMG_SVG = Settings.URL_API_CONTENT_IMG + 'svg/';

	// Pages
	static PAGE_TREE =				'Tree';
	static PAGE_REPORTS =			'Reports';
	static PAGE_USERS =				'Users';
	static PAGE_NO_AUTH =			'NO_AUTH';

	static A_PROT = 'bearer ';
	static A_AUTH_TOKEN = "accessToken";

	static DEVICE_PREFIX = 'dev';
	static DATAPOINT_PREFIX = 'dtp';

	static UTF8_BOM = '\uFEFF';		// @see https://simple.wikipedia.org/w/index.php?title=Byte_order_mark&oldid=6699577
	static DEFAULT_SEPARATOR = '|';

	static Page;

	// authToken
	static get authToken() {
		return sessionStorage.getItem(Settings.A_AUTH_TOKEN);
	}
	static set authToken(val) {
		sessionStorage.setItem(Settings.A_AUTH_TOKEN, val);
	}

	static get AuthHeader() {
		return { 'Authorization': Settings.A_PROT + ' ' + Settings.authToken };
	}
	

	static init() {

		// Not all pages should trigger auto login or attempt to login feature
		var noAuth = Settings.PAGE_NO_AUTH == ExpStartupPage;
		if (!noAuth && !Settings.authToken) {
			// The following code looks for a fragment in the URL to get the access token which will be
			// used to call the protected Web API resource
			var fragment = common.getFragment();

			//debugger;

			if (fragment.access_token) {
				// returning with access token, restore old hash, or at least hide token
				window.location.hash = fragment.state || '';
				Settings.authToken = fragment.access_token;
			} else {
				// Original code, which causes redirect to ROOT / folder after entering APP root for authorization
				//window.location = "Account/Authorize?client_id=web&response_type=token&state=" + encodeURIComponent(window.location.hash);

				// WORKAROUND START
				// no token - so bounce to Authorize endpoint in AccountController to sign in or register
				var url = Settings.URL_WEB + "Account/Authorize?client_id=web&response_type=token&state=" +
					encodeURIComponent(window.location.hash) +
					"&returnUrl=" + encodeURIComponent(window.location);
				window.location = url;
				// WORKAROUND END
			}

		} // IF

		// Multilang init
		return E.init();
	} // CTOR
}

class E {

	// Multilingual words asoc array
	static #_words;

	/**
	 * Initialization of multilingual words
	 */
	static init() {
		var def = $.Deferred();
		var promise = def.promise();

		var url = Settings.URL_API_WORD;

		$.ajax({
			url: url,
			type: 'GET',
			headers: Settings.AuthHeader,
			success: function (words) {
				//console.log(data);

				E.#_words = {};
				if ($.isArray(words)) {
					for (const word of words) {
						E.#_words[word.alias] = word.text;
					}
				}

				// Resolve promise after initialization of ML words is done
				def.resolve(true);
			},
		});

		return promise;
	}

	static T(alias) {
		if (E.#_words[alias] !== undefined) {
			return E.#_words[alias];
		} else {
			return 'ML:' + alias;
		}
	}

	static get(url, cb) {
		$.ajax({
			url: url,
			type: 'GET',
			headers: Settings.AuthHeader,
			success: function (data) {
				if ($.isFunction(cb)) {
					cb(data);
				}
			},
		});
	}

	static post(url, params, cb) {
		$.ajax({
			url: url,
			type: 'POST',
			data: params,
			headers: Settings.AuthHeader,
			success: function (data) {
				if ($.isFunction(cb)) {
					cb(data);
				}
			},
		});
	}

	static put(url, params, cb) {
		$.ajax({
			url: url,
			type: 'PUT',
			data: params,
			headers: Settings.AuthHeader,
			success: function (data) {
				if ($.isFunction(cb)) {
					cb(data);
				}
			},
		});
	}
}

/**
 * Startup - after all loaded
 */
$(document).ready(function () {

	//debugger;
	// If this page is one of our pages
	if (typeof ExpStartupPage != 'undefined') {
		// initialize settings, which should return promise
		// After promise is resolved, initialization ended
		Settings.init().then(function (result) {

			var main = $('#e-main-outer');
			// Starting UI
			switch (ExpStartupPage) {

				case Settings.PAGE_TREE:
					var m = new ETreeMain(main);
					break;

				case Settings.PAGE_REPORTS:
					var m = new EReportsMain(main);
					break;

				case Settings.PAGE_USERS:
					var m = new EUsers(main);
					break;

				default:
					break;
			}
		});
	}
});



