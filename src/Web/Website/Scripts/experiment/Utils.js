class Utils {
	
	static ASSERTION_LEVEL = 'error';
	
	/**
	 * Returns file extension from given URL
	 * 
	 * @url url of the file
	 */
	static getFileExt(url) {
		var retVal;
		var lastDotIndex = url.lastIndexOf('.'); 
		if(lastDotIndex != -1) {
			retVal = url.substring(lastDotIndex + 1);
		}
		return retVal;
	}
	
	static getIsImageByExt(ext) {
		return $.inArray(ext, [
			'jpg', 'jpeg', 'jpe', 'gif', 'png', 'apng', 'svg',
		]) != -1;
	}
	
	
	/**
	 * Returns A HREF element with base64 encoded data for download
	 */
	static downloadData(data, content, fileName) {
		return $('<a>', {
			href: 'data:text/plain;base64,' + Utils.utf8ToBase64(data),
			download: fileName
		}).append(content);
		//return '<a href="data:text/plain;base64,'+ btoa(data) +'" download="'+ fileName +'">'+ content +'</a>';
	}
	
	/**
	 * Specific CSV converter, usable mostly for Chart series
	 */
	static jsonArrayToCsv (serie, delimiter) {
		var retVal = '';

		if(!delimiter)
			delimiter = "\t";
		
		if(serie !== undefined) {
			for(var i = 0; i < serie.length; i++) {
				retVal += String(serie[i][0]) + delimiter + String(serie[i][1]) + '\n';
			}
		}
		return retVal;
	}

	/**
	 * Converts an empty object to empty string, instead of returning null or undefined
	 */
	static nullToEmptyStr(value) {
		if(value !== undefined)
			return value;
		else
			return '';
	}
	
	/**
	 * Converting an array to CSV formatted line with line ending
	 */
	static toCsvRow(arr, delimiter) {
		var retVal = '';
		
		if(!delimiter)
			delimiter = ";";
		
		$.each(arr, function(i, v){
			if(retVal.length >0) {
				retVal += delimiter;
			}
			retVal += Utils.nullToEmptyStr(v);
		});
		retVal += "\n";
		return retVal;
	}

	/**
	 * Converts given value to uppercase string, if it not undefined
	 */
	static toUpperString(obj) {
		if(obj === undefined)
			return obj;
		
		return String(obj).toUpperCase();
	}

	// Utf8 BOM to Base64
	static utf8ToBase64(str) {
		return window.btoa(unescape(encodeURIComponent(Settings.UTF8_BOM + str)));
	}

	static base64ToUtf8(str) {
		return decodeURIComponent(escape(window.atob(str)));
	}
	
	/**
	 * Leave only unique elements in array. Works not only with DOM elements.
	 * (Similar like SQL DISTINCT)
	 * 
	 * @Used in content-engine.js validation mechanics
	 * 
	 * @param arr
	 * @returns
	 */
	static uniqueArray(arr) {
		return $.grep(arr, function(v, k) {
			return $.inArray(v, arr) === k;
		});
	}
	
	/**
	 * Determine how many days has each month (unused)
	 * 
	 * @param month numeric value
	 * @param year numeric value
	 * @returns number of days in specific month
	 */
	static getDaysInMonth(month, year) {
		return getDaysInMonth(month, year);
	}
	
	/**
	 * Used to fill leading zeroes on values less than 10
	 * 
	 * @param val
	 * @returns
	 */
	static appendZeroFill(n) {
		if (n <= 9) {
			return "0" + n;
		}
		return n
	}
	
	/**
	 * Object clonning (sort of it)
	 */
	static clone(obj) {
		// Handle the 3 simple types, and null or undefined
		if (null == obj || "object" != typeof obj)
			return obj;

		// Handle Date
		if (obj instanceof Date) {
			var copy = new Date();
			copy.setTime(obj.getTime());
			return copy;
		}

		// Handle Array
		if (obj instanceof Array) {
			var copy = [];
			for (var i = 0, len = obj.length; i < len; i++) {
				copy[i] = Utils.clone(obj[i]);
			}
			return copy;
		}

		// Handle Object
		if (obj instanceof Object) {
			var copy = {};
			for ( var attr in obj) {
				if (obj.hasOwnProperty(attr))
					copy[attr] = Utils.clone(obj[attr]);
			}
			return copy;
		}

		throw new Error("Unable to copy obj! Its type isn't supported.");
	}
	
	/**
	 * Functionality like browser's alert
	 * 
	 * Don't call directly browser's alert function, but use this one, as later we'll impelemnt some fancy stuff here. 
	 */
	static alert(msg) {
		alert(msg);
	}
	
	/**
	 * Functionality like browser's confirm
	 * 
	 * Don't call directly browser's confirm function, but use this one, as later we'll impelemnt some fancy stuff here.
	 */
	static confirm(msg) {
		return confirm(msg);
	}
	
	/**
	 * Prompts for user's input
	 * 
	 * Don't call directly browser's confirm function, but use this one, as later we'll impelemnt some fancy stuff here.
	 */
	static prompt(msg, defaultValue) {
		if(defaultValue)
			return prompt(msg, defaultValue);
		else 
			return prompt(msg);
	}
	
	/**
	 * Creates URL with parametters
	 * 
	 * @url base URL
	 * @params with data, eg. { id: 1, name: 'Dummy name', }
	 * @exceptions array of fields which should be not included to url, eg. [ '_id', 'name', ] 
	 */
	static Url(url, params, includeOnly) {
		
		var retVal = url;
		var first = true;
		if(params) {
			
			for(var key in params) {

				var add = true;
				if(includeOnly) {
					if($.isArray(includeOnly)) {
						add = $.inArray(key, includeOnly) != -1;
					}
				}
				
				if(add) {
					if(first) {
						retVal += '?'+ key +'='+ params[key];
						first = false;
					} else {
						retVal += '&'+ key +'='+ params[key];
					}
				}
			}
		}
		return retVal;
	}
	
	/**
	 * Makes string with delimited string data from specific property from the array
	 * 
	 * For instance we have an array:
	 * 
	 * var data = [
	 * 		{ _id: 1, name: 'Abc' },
	 * 		{ _id: 2, name: 'Abu' }, 
	 * ];
	 * 
	 * var concatData = Utils.concatArrayColumnAsString(data, '_id', '|');
	 * 
	 * concatData should look like: "1|2"
	 * 
	 */
	static concatArrayColumnAsString(data, columnName, delimiter) {
		var retVal = '';
		if($.isArray(data)) {
			for(const item of data) {
				// if such column exists
				if(item[columnName] !== undefined) {
					if (retVal != '')
						retVal += delimiter;
					retVal += String(item[columnName]);
				}
			}
		}
		return retVal;
	}
	
	static collectOnlyRequiredParams(allParams, requiredParamsList) {
		var retVal = {};
		
		for(const requiredParam of requiredParamsList) {
			if(allParams[requiredParam] !== undefined) {
				retVal[requiredParam] = allParams[requiredParam]; 
			} else {
				console.warn('Utils::collectOnlyRequiredParams: requiredParam['+ requiredParam +'] was not found in [allParams].', allParams);
			}
		}
		
		return retVal;
	}
	
	static getPropertyValue(property, valueIfEmpty) {
		if(property !== undefined) {
			if($.isFunction(property)) {
				return property();
			} else {
				return property;
			}
		}
		return valueIfEmpty;
	}
	
	static randomColor() {
		return '#'+ ('000000' + Math.floor(Math.random()*16777215).toString(16)).slice(-6);
	}
	
	/**
	 * Checks is tring value is null or empty
	 * 
	 * Borrowed from: https://stackoverflow.com/a/33672308/14315045
	 */
	static isNullOrEmpty(value) {
		return typeof value == 'string' && !value.trim() || typeof value == 'undefined' || value === null;
	}
	
	/**
	 * This function designed to return the value, no matter what type of "beast" is value:
	 * 
	 * regular value;
	 * function;
	 * 
	 * Warning: promises are non-supported via their asynchronous nature.
	 */
	static handleValue(subject) {
		Utils.assert(!$.isFunction(subject.then), 'Utils::handleValue(subject), promises are non-supported via their asynchronous nature.');
		
		if($.isFunction(subject)) {
			// Function
			return subject();
		} else {
			// Regular value
			return subject;
		}
	}
	
	/**
	 * Iterate some array selectivelly
	 * 
	 * @param array an array which needs to iterate
	 * @param params plain object with parameters for this function, including params.cb callback
	 */
	static iterate (params) {
		let wLoc = 'Utils.iterate(array, params)';
		//console.log(wLoc +'...', params);
		
		let isParamsOk = $.isPlainObject(params);
		let isArrayOk = $.isArray(params.array);
		let hasCallback = $.isFunction(params.cb);
		
		// Minimum needed for work of this method
		let isOk = isArrayOk && isParamsOk && hasCallback; 
		
		// Dumping errors, if available
		Utils.assert(isArrayOk, wLoc +', [array] should be an array, not something else');
		Utils.assert(isParamsOk, wLoc +', [params] should be plain object!');
		Utils.assert(hasCallback, wLoc +', Should be defined callback as [params.cb]!');
		
		// And exiting if something wasn't right
		if(!isOk)
			return;
		
		let hasFrom = $.isNumeric(params.from);
		let hasTo = $.isNumeric(params.to);
		
		// iterating an array
		let index = 0, callAlways = !hasFrom && !hasTo, call = false;
		for(let item of params.array) {
			
			// Do we call it always?
			call = callAlways;
			
			// If we call it not always
			if(!call) {
				
				if(hasFrom && hasTo) { 
					// if it has from and to and it match
					if(index >= params.from && index <= params.to) {
						call = true;
					}
				} else if (hasFrom) {
					// if it has only from and it match
					if(index >= params.from) {
						call = true;
					}
					
				} else if (hasTo) {
					// if it has only to and it match
					if(index <= params.to) {
						call = true;
					}
				} 
			}
			
			// if flag is set to call
			if(call) {
				params.cb(item, index);
			}
			
			// next
			index++;
		}
	}
	
	static assert(condition, text, obj) {
		if(!condition) {
			if(obj !== undefined) {
				
				switch(Utils.ASSERTION_LEVEL) {
				
					case 'warning':
						console.warn(text, obj);
						break;
						
					case 'error':
						console.error(text, obj);
						break;
				}
				
			}
			else {
				switch(Utils.ASSERTION_LEVEL) {
					case 'warning':
						console.warn(text);
						break;
					
					case 'error':
						console.error(text);
						break;
				}
				
			}
		}
	}

	static isDate(date) {

		if (Utils.isNullOrEmpty(date))
			return false;

		return (new Date(date) !== "Invalid Date") && !isNaN(new Date(date));
	}

}