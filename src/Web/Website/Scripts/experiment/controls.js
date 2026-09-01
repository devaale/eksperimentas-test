
class ETree extends Element {

	static copiedElementType;
	//static var copiedElementId = [];
	copiedElementId = [];
	//static copiedElementId = new Array();

	// Settings
	static ALWAYS_COPY_ON_DRAG = false;
	static ENFORCE_CHILDREN = false;	// Enforce that all tree nodes had children (could involve the mess)

	// search
	static CASE_SENSITIVE = false;
	static SHOW_ONLY_MATCHES = true;

	// Private vars
	#_params;

	// Public vars
	jsTree;
	lastUpdatedTreeNodeId;

	/**
	 * @parent means should have [renderContent] and [renderTree] methods
	 * @options various jsTree options
	 */
	constructor(params) {
		super({ tag: 'div', attrs: { 'class': params.key } });
		this.#_params = params;
		this.#_initETree();
		this.#_initETreeEvents();
	}

	#_initETree() {
		let _this = this;
		this.options = $.extend(true, {
			'jsTree': {
				'core': {
					'check_callback': function (operation, node, node_parent, node_position, more) {
						return _this.checkCallback(operation, node, node_parent, node_position, more);
					},
				},
				// used for state plugin, to indentify which tree it is, during saving or loading its state
				'plugins': [
					'contextmenu',
					//'dnd',
					'search',
					'state',
					//'types',
					'wholerow',
					//'checkbox'
				],
				'types': {
				},
				'search': {
					'case_insensitive': ETree.CASE_SENSITIVE,
					'show_only_matches': ETree.SHOW_ONLY_MATCHES,
				},
				'state': {
					'key': this.#_params.key,
				},
			},

			// @deprecated
			// REST webservice URL
			//treeUrl: undefined,
			// @deprecated
			//jqThis: undefined,

			cmdRefresh: undefined,
			txtFilter: undefined,

			afterTreeDataLoad: $.noop,	// tree data loaded pre-precessing event
			onTreeNodeCreate: $.noop,	// callback to create tree node, which will be assigned to ETree::selectedTreeNode
			onTreeNodeChanged: $.noop,	// tree node changed event
			onTreeCustomMenu: $.noop,	// tree custom menu triggered event
			onCheckCallback: $.noop,	// tree classic 'check_callback'
			onMoveNode: $.noop,			// after node moved
			onCopyNode: $.noop,			// after node copied
			onLoaded: $.noop,			// on tree loaded
		}, this.#_params);

		/// Settings
		// Drag and drop plugin
		if (ETree.ALWAYS_COPY_ON_DRAG) {
			_this.options.jsTree['dnd'] = {
				'always_copy': true,	// always copy, no moving
			};
		}

		// Initializing JSTREE
		_this.jsTree = _this.jqThis.jstree(_this.options.jsTree);
		if (!_this.jsTree) {
			_this.error("ETree::jsTree wasn't initialized successfuly!");
		}
	}

	#_initETreeEvents() {
		var _this = this;

		// custom menu
		if (_this.options.onTreeCustomMenu !== $.noop) {
			_this.options.jsTree['contextmenu'] = {
				items: _this.options.onTreeCustomMenu,
			};
		}

		if (_this.options.onTreeNodeChanged !== $.noop) {
			_this.jqThis.on('changed.jstree', function (e, data) {
				_this.treeNodeChanged(e, data);
			});
		}

		// after node moved
		if (_this.options.onMoveNode !== $.noop) {
			_this.jqThis.on('move_node.jstree', function (e, data) {
				_this.moveNode(e, data);
			});
		}

		// after node copied
		if (_this.options.onCopyNode !== $.noop) {
			_this.jqThis.on('copy_node.jstree', function (e, data) {
				_this.copyNode(e, data);
			});
		}

		// on tree loaded
		_this.jqThis.on('refresh.jstree', function (e, data) {

			if (_this.lastUpdatedTreeNodeId !== undefined) {
				_this.selectTreeNode(_this.lastUpdatedTreeNodeId);
				_this.lastUpdatedTreeNodeId = undefined;
			}

			if (_this.options.onLoaded !== $.noop) {
				_this.loaded(e, data);
			}
		});

		// Filtering
		var to = false;
		if (_this.options.txtFilter !== undefined) {
			_this.options.txtFilter.jqThis.keyup(function () {
				if (to) { clearTimeout(to); }
				to = setTimeout(function () {
					var v = _this.options.txtFilter.jqThis.val();
					_this.jqThis.jstree(true).search(v);
				}, 250);
			});
		}

		// Buttons
		if (_this.options.cmdRefresh !== undefined) {
			_this.options.cmdRefresh.jqThis.click(function () {
				_this.refresh();
			});
		}
	}

	/**
	 * Refresh whole JsTree
	 */
	refresh() {
		this.jqThis.jstree(true).refresh();
	}

	getTreeNodeById(id) {
		return this.jsTree.jstree(true).get_node(id);
	}

	/**
	 * Called to select specific jsTree node by its id
	 */
	selectTreeNode(id) {
		this.deselectAll();
		this.jqThis.jstree(true).select_node(id);
	}

	deselectAll() {
		this.jqThis.jstree(true).deselect_all(true);
	}

	/**
	 * After tree data loaded, if you want to do some processing or received data modifications
	 */
	afterTreeDataLoad = (data) => {

		var retVal = data;
		if (this.options.afterTreeDataLoad !== $.noop) {
			retVal = this.options.afterTreeDataLoad(data);
		}

		if ($.isArray(data)) {
			for (const o of retVal) {
				if (ETree.ENFORCE_CHILDREN) {
					o.children = true;
				} else if (o.children != undefined && o.children != 'boolean') {
					o.children = o.children == 1;
				}
			}
		}

		return retVal;
	}

	treeNodeCreate = (data) => {
		let retVal = this.options.onTreeNodeCreate(data);
		if (retVal !== undefined) {
			retVal.tree = this;
		}
		return retVal;
	}

	/**
	 * After tree node just changed
	 */
	treeNodeChanged = (e, data) => {
		//this.debug('ETree::callOnTreeNodeChanged', {e: e, data: data});

		if (data && data.node) {
			// We might need id verification, if this one works poor
			// But this variant will work even if this.selectedNode is undefined
			// Previously and now selected nodes are same object?
			this.selectedNewNode = this.selectedNode != data.node;

			// Single jsTree node (not eg. ETreeNode)
			this.selectedNode = data.node;

			// Several selected jsTree nodes (not eg. ETreeNode)
			this.selectedNodes = [];
			if (data && data.instance) {
				this.selectedNodes = data.instance.get_selected();
			}

			// Calling tree node create callback
			//this.selectedTreeNode = this.options.onTreeNodeCreate(data);
			this.selectedTreeNode = this.treeNodeCreate(data);

			// In normal work case [lastUpdatedTreeNodeId] is undefined
			// It only available of new tree node created.
			// But in such case after tree reload firstly selected always its parent tree node, which further execution need to prevent.
			// In order to avoid loading of dictionaries and so on, as anyway newly created object after this instantly will be selected.
			var execute = this.lastUpdatedTreeNodeId == undefined ||
				this.lastUpdatedTreeNodeId == this.selectedTreeNode.node.id;
			if (execute) {
				this.options.onTreeNodeChanged(this.selectedTreeNode);
			}
		}
	}

	/**
	 * When need to return context menu, which will be shown after this
	 */
	treeCustomMenu = () => {
		if (this.options.onTreeCustomMenu !== $.noop) {
			return this.options.onTreeCustomMenu();
		}
	}

	/**
	 * Native for JsTree 'check_callback'
	 */
	checkCallback = (operation, node, parent, position, more) => {

		var retVal = true;
		if (this.options.onCheckCallback !== $.noop) {
			retVal = this.options.onCheckCallback(operation, node, parent, position, more);
		}
		return retVal;
	}

	moveNode = (e, data) => {
		//console.log('ETree::callOnMoveNode');	// DEBUG
		if (this.options.onMoveNode !== $.noop) {
			return this.options.onMoveNode(e, data);
		}
	}

	copyNode = (e, data) => {
		//console.log('ETree::callOnCopyNode');	// DEBUG
		if (this.options.onCopyNode !== $.noop) {
			return this.options.onCopyNode(e, data);
		}
	}

	loaded = (e, data) => {
		//console.log('ETree::callOnLoaded');	// DEBUG
		if (this.options.onLoaded !== $.noop) {
			return this.options.onLoaded(e, data);
		}
	}

} // class ETree
