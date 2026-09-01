/**
 * Element.js is Web Front-End development from real OOP perspective, where we develop real OOP classes.
 * Written By Arvydas Grigonis
 * 
 * This software is opensource under Apache 2.00 licence.
 * Legitimate for non-commercial and commercial use.
 * 
 * Requires: jQuery.
 * 
 * Project URL: https://github.com/arnvidhr/element.js
 */

class Element {

	/*
	 * Every element enforced and auto generated id attribute mechanics.
	 * By default: disabled
	 */
	static ENFORCE_ID = false; // Do we need ids for every DOM element
	static DEFAULT_ID_PREFIX = 'E';
	static INSTANCE_NO = '{instanceNo}';

	static DEFAULT_ELEMENT = 'div'; // In case if consturctor parameters are empty

	// HTML self-closing elements
	static SELF_CLOSING_TAGS = [
		'area',
		'base',
		'br',
		'embed',
		'hr',
		'iframe',
		'img',
		'input',
		'link',
		'meta',
		'param',
		'source',
		'track'
	];

	// HTML Attributes
	static ATTR_ID = 'id';
	static ATTR_NAME = 'name';
	static ATTR_CLASS = 'class';
	static ATTR_STYLE = 'style';
	static ATTR_VALUE = 'value';
	static ATTR_SIZE = 'size';

	static ATTR_REQUIRED = 'required';
	static ATTR_SELECTED = 'selected';
	static ATTR_DISABLED = 'disabled';
	static ATTR_CHECKED = 'checked';
	static ATTR_MULTIPLE = 'multiple';
	static ATTR_READONLY = 'readonly';

	/**
	 * Single HTML attributes need to add like this:
	 * 
	 * required: true
	 * 
	 * In case if it false, it won't be added. 
	 * This done to simplify engine's work as developer can specify condition during control's initialization. 
	 */
	static SINGLE_ATTRS = [
		Element.ATTR_REQUIRED,
		Element.ATTR_SELECTED,
		Element.ATTR_DISABLED,
		Element.ATTR_CHECKED,
		Element.ATTR_MULTIPLE,
		Element.ATTR_READONLY,
	];

	/// Private attributes
	#_params;					// Constructor's received raw params
	#_tag;						// This HTML element tag
	#_jqThis;					// This class jQuery representation
	#_parent;					// Element class parent
	#_jqParent;					// Parent jQuery representation
	#_instanceNo;				// Current class instance number, initialized in Element::#_init()

	/// Private static
	static #_sInstanceNum = 1;	// Class instance counter

	/// Public properties
	children = [];

	/// Events
	onClear;

	// HTMl Element tag
	get tag() {
		return this.prop("tagName");
	}
	set tag(val) {
		this.prop("tagName", val);
	}

	get isSelfClosingTag() {
		if (this.#_tag !== undefined) {
			return $.inArray(this.#_tag.toLowerCase(), Element.SELF_CLOSING_TAGS) != -1;
		}
		return false;
	}

	get html() {
		return this.jqThis.html();
	}

	set html(val) {
		this.jqThis.html(this.varProcessing(val));
	}

	get text() {
		return this.jqThis.text();
	}
	set text(val) {
		this.jqThis.text(this.varProcessing(val));
	}

	get val() {
		return this.jqThis.val();
	}

	set val(val) {
		this.jqThis.val(this.varProcessing(val));
	}

	// Regular bounds, like CSS and so on
	get width() {
		return this.jqThis.width();
	}
	set width(val) {
		this.jqThis.width(val);
	}

	get height() {
		return this.jqThis.height();
	}
	set height(val) {
		this.jqThis.height(val);
	}

	// HTML DOM element id
	get id() {
		return this.attr(Element.ATTR_ID);
	}
	set id(val) {
		this.attr(Element.ATTR_ID, val);
	}

	// Parent element class
	get parent() {
		return this.#_parent;
	}
	set parent(val) {

		if (val instanceof Element) {
			this.#_parent = val;
			val.jqThis.append(this.jqThis);

		} else if (val instanceof jQuery) {
			this.#_jqParent = val;
			val.append(this.jqThis);
		}
	}

	/*
	 * jQuery representation of this HTML element
	 * Or its entry point for various use, if needed.
	 */
	get jqThis() {

		if (this.#_jqThis === undefined) {
			// Forming jQuery element creation string
			var ts = '<' + this.#_tag;
			if (this.isSelfClosingTag) {
				ts += ' />';
			} else {
				ts += '></' + this.#_tag + '>';
			}
			// jQuery element creation
			this.#_jqThis = $(ts);
		}
		return this.#_jqThis;
	}

	// jQuery level parent
	get jqParent() {
		if (this.isElementParent) {
			return this.parent.jqThis;
		} else {
			return this.#_jqParent;
		}
	}
	set jqParent(val) {
		this.#_jqParent = val;
	}

	// Is parent is instance of an Element
	get isElementParent() {
		return this.#_parent instanceof Element;
	}

	// Is parent is instance of jQuery
	get isJQueryParent() {
		return this.#_parent instanceof jQuery;
	}

	// Current class name (with inheritance I think, not sure)
	get className() {
		// @see https://stackoverflow.com/a/30560581
		return this.constructor.name;
	}

	// Current class instance unique number
	get instanceNo() {
		return this.#_instanceNo;
	}

	/*
	 * Constructor
	 * 
	 * Accepts object with parameters. 
	 * We strongly recomend to add 'tag' attribute, which specify this element HTML tag. Elsewhere, what you developing at all? :-)
	 */
	constructor() {

		this.#_params = $.extend({
			// Html tag
			tag: Element.DEFAULT_ELEMENT,
			// Html tag attributes
			attrs: [],
			// Wanted child elements, if needed @TODO: implement
			children: [],
			// Parent Element class
			parent: undefined,
		}, arguments[0]);

		this.#_init();
	}

	#_init() {

		// Increasing this class instance number
		Element.#_sInstanceNum++;
		// Assigning instance number to current class instance attribute
		// Sort of unique indentifier of this class instance, acecssible via this.instanceNo for public use.
		this.#_instanceNo = Element.#_sInstanceNum;

		// HTML element tag fist. As it is crucial.
		this.#_tag = this.#_params.tag;

		for (const paraKey in this.#_params) {
			var val = this.#_params[paraKey];

			switch (paraKey) {

				case 'tag':
					// Already initialized
					break;

				case 'attrs':
					for (const key in val) {
						this.prop(key, val[key]);
					}
					break;

				case 'text':
					this.text = val;
					break;

				case 'html':
					this.html = val;
					break;

				case 'children':
					this.add(val);
					break;

				case 'parent':
					this.parent = val;
					break;
			}
		}
	}

	/*
	 * Set or get HTML element attribute
	 */
	attr(attr, value) {
		if (value === undefined)
			return this.jqThis.attr(attr);
		else
			this.jqThis.attr(attr, this.varProcessing(value));
	}

	/*
	 * Set or get HTML element property
	 */
	prop(prop, value) {
		if (value === undefined)
			return this.jqThis.prop(prop);
		else
			this.jqThis.prop(prop, this.varProcessing(value));
	}

	/**
	 * Processing of assignning variable to place in it unique instance ids and so on
	 * @param {any} val
	 */
	varProcessing(val) {
		if ($.type(val) === 'string') {
			return val.replace(Element.INSTANCE_NO, this.instanceNo);
		} else {
			return val;
		}
	}

	/*
	 * add array|object to this Element as its child
	 * @recursive
	 */
	add(params) {

		var element;

		// Element instance
		if (params instanceof Element) {

			// Element
			element = params;
			// Assign this instance as parent to its child
			element.parent = this;
			// Adding to children collection
			this.children.push(element);
		}
		// Object, but not Element, probably params
		else if ($.isPlainObject(params)) {

			// Element (parent from params will be overriden)
			element = new Element(params);
			// Add
			this.add(element);
		}
		// Array
		else if ($.isArray(params)) {
			// Add each element
			for (const item of params) {
				this.add(item);
			}
		}

		// retVal
		if (element !== undefined)
			return element;
	}

	addChildBr() {
		return this.add({ tag: 'br' });
	}

	addChildHr() {
		return this.add({ tag: 'hr' });
	}

	/*
	 * Remove method, has 3 work modes:
	 * 
	 * 1. subj.remove(); // removes itself 
	 * 
	 * 2. subj.renove(child); // removes specific its own child
	 * 
	 * 3. subj.remove(childrenArray); // removes its own children array
	 * 
	 */
	remove() {

		// If no arguments or remove itself
		if (arguments.length > 0) {
			var arg = arguments[0];

			// If this an array
			if ($.isArray(arg)) {

				for (const child of arg) {
					this.remove(child);
				}

			} else if (arg instanceof Element) {

				// Find this child between children
				var index = this.children.indexOf(arg);

				// If something found
				if (index != -1) {
					// Removing it from children
					var removed = this.children.splice(index, 1);
					arg.jqThis.remove();
				}

			}

		} else {

			// In case parent is an Element
			if (this.isElementParent) {
				this.parent.remove(this);

				// In case parent is an jQuery instances
			} else if (this.isJQueryParent) {
				this.jqThis.remove();
			}
		}

	}

	/**
	 * Clean up of children items
	*/
	clear() {

		// Start onClear, if it exists before any clean up
		// If developer wants to do something with still existing data and implemented specific method
		if ($.isFunction(this.onClear)) {
			this.onClear();
		}

		// Recursively cleaning up all children structure
		for (const child of this.children) {

			// clear command first for most farer childs
			if ($.isFunction(child.clear)) {
				child.clear();
			}

			// Unsubcribling from Element.js events
			if ($.isFunction(child.unsubscribe)) {
				child.unsubscribe();
			}
		}

		// Wiping out from garbage collector everywhere
		this.children = [];

		// Cleaning up jQuery object
		// TODO: Refactoring might be needed
		// Possible issue, as jQuery::empty() deletes only known for it jQuery elements
		if (this.jqThis !== undefined) {
			this.jqThis.empty();
		}
	}

	/**
	 * Is this single attribute, like selected, disabled, required
	 * @param {any} attr HTML attribute
	 */
	static isThisSingleAttribute(attr) {
		return $.inArray(attr.toLowerCase(), Element.SINGLE_ATTRS) != -1;
	}
}