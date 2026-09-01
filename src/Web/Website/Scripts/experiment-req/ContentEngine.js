/**
 * ContentEngine.js Web content regarding JSON data construction engine 
 * Written By Arvydas Grigonis
 * 
 * This software is opensource under Apache 2.00 licence.
 * Legitimate for non-commercial and commercial use.
 *
 * Requires: jQuery, Element.js
 *
 * Project URL: https://github.com/arnvidhr/element.js
 */

class ContentEngine extends Element {
	
	// Constants
	static VISIBLE =						'visible';
	static READ_ONLY =						'readOnly';
	static REQUIRED =						'required';
	
	static SHOW_DISABLED_HTML_CONTROLS =	true;	// if this enabled, all items with type will be rendered as HTML controls, including disabled ones
	
	/**
	 * Content engine data types
	 */ 
	static STRUCT_TYPE_ID =					'id';
	static STRUCT_TYPE_STRING =				'string';
	static STRUCT_TYPE_PASSWORD	=			'password';
	static STRUCT_TYPE_TEXT =				'text';
	static STRUCT_TYPE_INTEGER =			'int';
	static STRUCT_TYPE_FLOAT =				'float';
	static STRUCT_TYPE_BOOL =				'bool';
	static STRUCT_TYPE_DATE =				'date';
	static STRUCT_TYPE_GROUP =				'group';
	static STRUCT_TYPE_FILE =				'file';
	static STRUCT_TYPE_FILE_IMAGE =			'file_image';
	
	static STRUCT_GROUP_ITEMS =				'items';		// STRUCT_TYPE_GROUP items
	
	static SHOW_IF =						'showIf';		// Show condition

	static ESE_SUBMIT_EFFECTS = true;

	/// Private attributes
	#_dateTimePickers = [];	// @TODO?

	// UI sections
	secMessages;
	secData;
	secCommands;

	/// Private static

	/// Public properties
	params;			// All content engine params structure

	// ContentEngine specific data
	get data() {
		if (this.params !== undefined)
			return this.params.data;
	}

	// Content engine messages
	get messages() {
		if (this.params !== undefined)
			return this.params.messages;
	}

	/**
	 * Constructor
	 * 
	 * @param object params
	 */
	constructor(params) {
		super({ 'tag': 'div', 'attrs': { 'class': 'ce-main'}});
		this.params = $.extend(true, {
			'messages': [],
			'data': [],
			'commands': [],
			'utils': {
				'translate': function (alias) {
					return ContentEngineUtils.translate(alias);
				},
				'alert': function (params) {
					ContentEngineUtils.alert(params);
				},
				'createGroup': function (label) {
					return ContentEngineUtils.createGroup(label);
				},
				'createField': function (params) {
					return ContentEngineUtils.createField(params);
				},
			},
			'options': {
				// If enabled, preventing any command activation if required fields are invalid or empty
				'enforceRequired': false,	
			},
		}, params);
		this.#_init();
	}

	#_init() {
		if (this.params !== undefined) {
			this.#_initMessages();
			this.#_initControls();
			this.#_initCommands();
			this.#_initShowConditions();
		}
	}

	#_initMessages() {

		if ($.isArray(this.params.messages) && this.params.messages.length > 0) {

			this.secMessages = this.add({ 'tag': 'div', 'attrs': { 'class': 'ce-messages' } });
			for (const msg of this.params.messages) {
				this.secMessages.add({
					tag: 'div',
					text: msg['message'],
					attrs: {
						'class': 'ce-msg ce-msg-lvl' + msg['message-level']
					},
				});
			}
		}
	}

	#_initControls() {
		this.secData = this.add({ 'tag': 'div', 'attrs': { 'class': 'ce-data'} });
		this.#_recursiveControlsInit(this.secData, this.data);
	}

	#_initCommands() {
		var _this = this;

		if ($.isArray(this.params.commands) && this.params.commands.length > 0) {

			this.secCommands = this.add({ 'tag': 'div', 'attrs': { 'class': 'ce-commands' } });

			for (const cmdInfo of this.params.commands) {
				var cmd = this.secCommands.add({
					'tag': 'button',
					'attrs': {
						'type': 'button',
						'class': 'ce-cmd'
					},
					'text': cmdInfo.label,
				});

				cmd.jqThis.on('click', function () {
					if ($.isFunction(cmdInfo.click)) {
						_this.#_beforeCommandExecution(cmdInfo.click);
					}
				});
			}
		}
	}

	/**
	 * showIf handling
	 * It should be initialized after all 
	 */
	#_initShowConditions() {
		var _this = this;

		// looking for showIf formulaes
		ContentEngine.recursiveDataIteration(this.data, function (aKey, aField, aGroup) {

			// If it has showIf property
			if (aField.hasOwnProperty(ContentEngine.SHOW_IF)) {

				// If this property is not empty
				if (aField[ContentEngine.SHOW_IF]) {

					// Condition
					var condition = aField[ContentEngine.SHOW_IF];
					// splitting it to nodes by space, so spaces are mandatory in such formulas
					var conditionNodes = condition.split(' ');
					var conditionValueNodes = conditionNodes[2].split('|');

					// Now searching for related to this formula field
					ContentEngine.recursiveDataIteration(_this.data, function (cKey, cField, cGroup) {

						if (cKey == conditionNodes[0]) {
							if (cField.uiValueField && cField.uiValueField.jqThis) {
								cField.uiValueField.jqThis.change(function () {

									var val;
									if ($(this).prop("tagName").toLowerCase() == 'div') {
										val = $(this).children('select').val();
									} else {
										val = $(this).val();
									}

									var evalFormula = 'false';
									for (var key in conditionValueNodes)
										evalFormula += '||' + val + conditionNodes[1] + conditionValueNodes[key];

									var show = eval(evalFormula);
									//_this.debug('evalFormula: '+ evalFormula +' from '+ cKey +' to '+ aKey +' equals to: '+ show);

									if (aField.uiContainer && aField.uiContainer.jqThis) {
										if (show) {
											aField.uiContainer.jqThis.show();
										} else {
											aField.uiContainer.jqThis.hide();
										}
									}
								});

								// let's call it once in order to initialize UI correctly
								cField.uiValueField.jqThis.trigger('change');
							}

							return false; // already found what we need
						} else {
							return true; // still not found what we need
						}

					});
				}
			}
			return true;
		});
	}

	#_recursiveControlsInit = (parentControl, data) => {

		// for each item in array
		for (const field of data) {
			var key = field.name;
			//var field = data[key];	// for easier access
			//console.log(field);

			var control = this.initField(key, field);
			parentControl.add(control);

			var isValidGroup = field['type'] == ContentEngine.STRUCT_TYPE_GROUP &&
				field.hasOwnProperty(ContentEngine.STRUCT_GROUP_ITEMS);
			if (isValidGroup) {
				this.#_recursiveControlsInit(control, field[ContentEngine.STRUCT_GROUP_ITEMS]);
			}
		} // for each
	}

	/**
	 * Generates single property with label and control
	 * @param {any} key db field name
	 * @param {any} field
	 */
	initField(key, field) {

		if (!ContentEngine.validateField(field))
			return '';

		var readOnly = false;
		var visible = true;
		var isThisId = field['type'] == ContentEngine.STRUCT_TYPE_ID;
		var isGroup = field['type'] == ContentEngine.STRUCT_TYPE_GROUP;

		var retVal = [];

		// Group initialization
		if (isGroup) {
			var group = this.params.utils.createGroup(field.label);
			field.uiControl = field.uiContainer = group;
			return group;
		}

		// visible?
		if (field.hasOwnProperty(ContentEngine.VISIBLE)) {
			if (field[ContentEngine.VISIBLE] !== undefined) {
				visible = field[ContentEngine.VISIBLE];
			}
		}

		// readOnly?
		if (field.hasOwnProperty(ContentEngine.READ_ONLY)) {
			if (field[ContentEngine.READ_ONLY] !== undefined) {
				readOnly = field[ContentEngine.READ_ONLY];
			}
		}

		var isHiddenField = readOnly || !visible || isThisId;
		var hiddenField;
		if (isHiddenField) {
			field.uiValueField = ContentEngine.generateHiddenField(key, ContentEngine.determineFieldValue(field));
			retVal.push(hiddenField);
		}

		if (!visible) {
			if (!field.uiControl)
				field.uiControl = field.uiValueField;
			return retVal;
		}

		// adding ce-unit
		var fieldId = 'ce-' + key + this.instanceNo;
		field.uiControl = this.generateControlForField(key, field, fieldId);	// automatic undefined handling inside
		if(!isHiddenField) {
			field.uiValueField = field.uiControl;
		}
		
		field.uiContainer = this.params.utils.createField({
			//id: contentEngineId,
			name: field['label'],
			value: field.uiControl,
			appendixClass: 'ce-type-' + field['type'],
		});

		retVal.push(field.uiContainer);
		return retVal;
	}

	/**
	 * Retuns genrated control Element class
	 * @param {any} key	name of control
	 * @param {any} field	content engine field info structure
	 * @param {any} id	id
	 */
	generateControlForField(key, field, id) {

		//console.log('ContentEngine::generateControlForField("'+ key+'", "'+ field +'", "'+ id + '")');
		var fieldLabel = field['label'];
		var fieldValue = ContentEngine.determineFieldValue(field);
		var required = field[ContentEngine.REQUIRED] == true;
		var retVal;

		//  Options have bigger priority than type of object.
		//  If they are defined, they override string, text and other types and will be rendered dropdown with options, key-value pairs,
		//  where their keys will be related with specific field value.
		//
		//  IMHO, basically, possible not to define type parameter, if options defined.
		var hasOptions = field.hasOwnProperty('options');
		var isReadOnly = field[ContentEngine.READ_ONLY] == true;

		if (hasOptions) {
			retVal = new EseDropdown({
				'id': id,
				'name': key,
				'value': fieldValue,
				'disabled': isReadOnly,
				//'size': 10,
				'options': field.options,
			});

			return retVal;
		}

		// Read only fields
		// If to comment this if case, all will be rendered as disabled HTML controls
		if (ContentEngine.SHOW_DISABLED_HTML_CONTROLS && !hasOptions && isReadOnly) {

			if (field['type'] == ContentEngine.STRUCT_TYPE_BOOL && (fieldValue == '0' || fieldValue == '1')) {
				// In case of read only bool field showing YES/NO
				fieldValue = (fieldValue == '1' ? this.params.utils.translate('yes') : this.params.utils.translate('no'));
			} else if (fieldValue === '') {
				// In case of any other empty
				fieldValue = 'N/A';	
			}
			retVal = new Element({ 'tag': 'div', 'text': fieldValue });
			return retVal;
		}

		// Min-Max handling
		var attributes = {};
		if (field['min'])
			attributes['min'] = field['min'];
		if (field['max'])
			attributes['max'] = field['max'];

		// Process non-readonly fields now
		switch (field['type']) {

			case ContentEngine.STRUCT_TYPE_FLOAT:
				attributes = $.extend(
					attributes,
					{
						id: id, type: 'number', name: key, value: fieldValue, placeholder: fieldLabel, 'required': required, disabled: isReadOnly,
						step: '0.0001', pattern: '^\d+(?:\.\d{1,4})?$'
					});
				retVal = new Element({ 'tag': 'input', 'attrs': attributes });
				break;

			case ContentEngine.STRUCT_TYPE_INTEGER:
				attributes = $.extend(
					attributes,
					{ id: id, type: 'number', name: key, value: fieldValue, placeholder: fieldLabel, 'required': required, disabled: isReadOnly });
				retVal = new Element({ 'tag': 'input', 'attrs': attributes });
				break;

			case ContentEngine.STRUCT_TYPE_DATE:
				retVal = new Element({ 'tag': 'input', 'attrs': { id: id, type: 'text', name: key, value: fieldValue, placeholder: fieldLabel, 'required': required, disabled: isReadOnly } });
				//this.#_dateTimePickers.push(retVal);
				break;

			case ContentEngine.STRUCT_TYPE_STRING:
				retVal = new Element({ 'tag': 'input', 'attrs': { id: id, type: 'text', name: key, value: fieldValue, placeholder: fieldLabel, 'required': required, disabled: isReadOnly } });
				break;

			case ContentEngine.STRUCT_TYPE_PASSWORD:

				retVal = new Element({ 'tag': 'input', 'attrs': { id: id, type: 'password', name: key, value: fieldValue, placeholder: fieldLabel, 'required': required, disabled: isReadOnly } });
				break;

			case ContentEngine.STRUCT_TYPE_TEXT:
				retVal = new Element({
					'tag': 'textarea', 
					'attrs': {
						'id': id,
						'rows': 3,
						'cols': 25,
						'name': key,
						'placeholder': fieldLabel,
						'class': 'ce-textarea',
						'required': required,
						'disabled': isReadOnly
					},
					'text': fieldValue,
				});
				break;

			case ContentEngine.STRUCT_TYPE_BOOL:
				var attributes = { id: id, type: 'checkbox', name: key, value: '1', disabled: isReadOnly, checked: fieldValue == 1 };
				retVal = new Element({ 'tag': 'input', 'attrs': attributes });
				break;

			case ContentEngine.STRUCT_TYPE_FILE:
				retVal = new Element({ 'tag': 'input', 'attrs': { id: id, type: 'file', name: key } });
				break;

			case ContentEngine.STRUCT_TYPE_IMAGE:
				retVal = new Element({ 'tag': 'input', 'attrs': { id: id, type: 'file', name: key, accept: 'image/*' } });
				break;

			case ContentEngine.STRUCT_TYPE_ID:
			default:
				// id is as well simple read only stuff, if shown
				retVal = new Element({ 'tag': 'div', 'text': fieldValue });
				break;
		}
		return retVal;
	}

	#_beforeCommandExecution(cb) {
		var _this = this;

		// By default this disabled
		if (_this.params.options.enforceRequired) {
			// Required fields enforced validation
			var fields = this.jqThis.find('input:invalid, select:invalid, textarea:invalid');
			var requiredFieldsOk = fields.length == 0;	// is nothing invalid was found?
			if (!requiredFieldsOk) {
				this.params.utils.alert('error-empty-req-fields');
				// Preventing further propagation
				return;
			}
		}

		var info = {};
		ContentEngine.recursiveDataIteration(this.data, function (key, field, group) {

			console.log(field);

			switch (field.type) {
				case ContentEngine.STRUCT_TYPE_ID:
					// @TODO
					console.log('id!');
					info[key] = field.uiValueField.val;
					break;

				case ContentEngine.STRUCT_TYPE_STRING:
				case ContentEngine.STRUCT_TYPE_PASSWORD:
				case ContentEngine.STRUCT_TYPE_TEXT:
				case ContentEngine.STRUCT_TYPE_INTEGER:
				case ContentEngine.STRUCT_TYPE_FLOAT:
				case ContentEngine.STRUCT_TYPE_DATE:
					info[key] = field.uiValueField.val;
					break;

				case ContentEngine.STRUCT_TYPE_BOOL:
					info[key] = field.uiValueField.prop('checked');
					break;

				case ContentEngine.STRUCT_TYPE_GROUP:
					break;

				case ContentEngine.STRUCT_TYPE_FILE:
					// @TODO
					break;

				case ContentEngine.STRUCT_TYPE_FILE_IMAGE:
					// @TODO
					break;
			}

			// proceed iteration
			return true;
		});

		// Calling command with ContentEngine data
		cb(info);
	}

	/**
	 * Field validation
	 * @param {any} field
	 * @returns bool
	 */
	static validateField(field) {
		return field != null && field !== undefined;
	}

	/**
	 * Determines field value from field itself (use only this function for it)
	 * It checking do value exist, if so, returning it.
	 * If no and if defaultValue exists, returning it.
	 * @param {any} field
	 * @returns 
	 */
	static determineFieldValue(field) {
		var retVal = '';

		if (field !== undefined) {
			var value = field['value'];

			if (value === undefined && field['defaultValue'] !== undefined) {
				// Only if no value, assigning default value
				retVal = field['defaultValue'];
			} else {
				// No default value, so setting up value or empty
				retVal = (value === undefined || value == 'undefined' || value == null ? '' : String(value));
			}
		} else {
			console.warn('ContentEngine::determineFieldValue: field is undefined!');
		}
		//console.log('field value: ' + retVal);
		return retVal;
	}

	/**
	 * Generates hidden form field for specific property
	 * @param {any} name Name of hidden field
	 * @param {any} value Value of hidden field
	 */
	static generateHiddenField(name, value) {
		return new Element({ 'tag': 'input', 'attrs': { type: 'hidden', name: name, value: value, } });
	}

	/**
	 * Recursive iteration via callback
	 * WARNING! iteration will be stopped if callback won't return the true
	 * 
	 * @param {any} data
	 * @param {any} cb
	 */
	static recursiveDataIteration(data, cb) {

		// for each in array
		for (const field of data) {
			var key = field.name;
			//var field = data[key];	// for easier access
			//console.log(field);

			var isGroup = field['type'] == ContentEngine.STRUCT_TYPE_GROUP;

			// if we have callback
			if (cb !== undefined) {
				// if we have callback, calling it as this is not group, but regular field
				if (cb(key, field, isGroup) != true) {
					// If callback returned false, we abandoning the cycle
					//this.debug('EseContentEngine::recursiveDataIteration: Abandoning callback at '+ key);
					return false;
				}
			}

			// if that was group and it has items
			if (isGroup && field.hasOwnProperty(ContentEngine.STRUCT_GROUP_ITEMS)) {
				// iterate those items recursivelly
				if (ContentEngine.recursiveDataIteration(field[ContentEngine.STRUCT_GROUP_ITEMS], cb) != true) {
					// If recursive function returned false, what can happen only if the callback returned false, we abandoning the cycle
					return false;
				}
			}
		}
		return true;
	}
}

class ContentEngineUtils {

	static alert(params) {
		var msg = params;
		if ($.isPlainObject(params)) {
			msg = JSON.stringify(params);
		}
		alert(params)
	}

	static translate(alias) {
		return alias;
	}

	static createGroup(label) {

		return new Element({
			'tag': 'div',
			'attrs': {
				'class': 'ce-group',
			},
			'children': [
				{
					'tag': 'label',
					'text': label
				},
			],
		});
	}

	static createField(params) {

		var options = $.extend({
			name: undefined,
			value: undefined,
			appendixClass: undefined,
			style: undefined,
		}, params);

		if (options.value instanceof Element) {
			if (options.value.id == undefined) {
				options.value.id = options.name + '{instanceNo}';
			}
		}

		return new Element({
			'tag': 'div',
			'attrs': {
				'class': 'ce-unit' + (options.appendixClass !== undefined ? ' ' + options.appendixClass : ''),
			},
			'children': [
				{
					'tag': 'label',
					'attrs': {
						'for': options.value.id,
					},
					'text': options.name,
				},
				{
					'tag': 'div',
					'children': [
						options.value
					],
				}
			]
		});
	}
}