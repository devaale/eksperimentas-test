class ETreeMain extends Element {

	static URL_API_MAIN_TREE = Settings.URL_API_EXP + 'Tree';
	static URL_API_MAIN_TREE_ITEM = Settings.URL_API_EXP + 'TreeItem';

	static PROTOCOL_TYPE = {
		0: 'Unknown',
		10: 'Modbus',
		20: 'BACnet',
		30: 'MQTT',
		40: 'CoAP',
		50: 'OpenThread',
		100: 'API',
	};

	static DATAPOINT_TYPE = {
		0: 'unknown',
		1: 'normal',
		2: 'virtual',
	};

	static REGISTER_TYPE = {
		16000: '16 bit unsigned',
		16010: '16 bit signed',
		32001: '32 bit unsigned swapped',
		32010: '32 bit signed',
		32011: '32 bit signed swapped',
		32100: '32 bit real',
		32101: '32 bit real swapped',
		64000: '64 bit unsigned',
		64001: '64 bit unsigned swapped',
		64010: '64 bit signed',
		64011: '64 bit signed swapped',
		64100: '64 bit real',
		64101: '64 bit real swapped',
	};

	static FUNCTION_CODE = {
		1: 'Read Coils (01)',
		2: 'Discrete Inputs (02)',
		3: 'Holding Registers (03)',
		4: 'Input Registers (04)',
		5: 'Single Coin (05) [Write]',
		6: 'Single Register (06) [Write]',
	};

	static READ_WRITE_TYPE = {
		0: 'read',
		1: 'write',
	};

	static VDP_FORMULA = {
		10: 'multiplication',
		20: 'division',
		30: 'addition',
		40: 'subtraction',
		50: 'difference',
		60: 'minVal',
		70: 'avgVal',
		80: 'maxVal',
		90: 'sum',
		100: 'count',
		1010: 'environmentalImpact',
		1020: 'thermalComfort',
		1030: 'depreciation',
	};

	static DATE_PART_OR_INTERVAL = {
		0: 'none',
		1: 'millisecond',
		2: 'second',
		3: 'minute',
		4: 'hour',
		5: 'day',
		6: 'week',
		7: 'month',
		8: 'quarter',
		9: 'year',
	};

	leftPanel;
	rightPanel;
	tree;	// Tree control
	currentObject;
	currentDevice;
	currentDatapoint;

	constructor(parent) {
		super({
			tag: 'div',
			attrs: { 'id': 'e-main-inner' },
			parent: parent,
		});
		this.#_init();
	}

	#_init() {
		var _this = this;
		this.tree = new ETree({
			'key': 'e-main-tree',
			'jsTree': {
				'core': {
					'data': function (node, cb) {

						if (node && node.id == '#') {
							let url = ETreeMain.URL_API_MAIN_TREE;
							E.get(url, function (data) {
								//console.log(data);
								cb.call(_this, data);
							});
						}
					},
				},
				'plugins': [
					'contextmenu',
					'dnd',
					'search',
					'state',
					'types',
					'wholerow',
					//'checkbox'
				],
				'types': {
					'default': {
						'icon': Settings.URL_API_CONTENT_IMG_SVG + 'tree-datapoint-none01-active.svg'
					},
					'dev': {
						'icon': Settings.URL_API_CONTENT_IMG_SVG + 'tree-datapoint-ext01-active.svg'
					},
					'dtp': {
						'icon': Settings.URL_API_CONTENT_IMG_SVG + 'tree-device01-active.svg'
					},
				},
			},

			//cmdReset: _this.cmdRefresh,
			//txtFilter: _this.txtFilter,

			//afterTreeDataLoad: $.noop,				// tree data loaded pre-precessing event
			onTreeNodeCreate: _this.onTreeNodeCreate,	// node creation cb
			onTreeNodeChanged: _this.onTreeNodeChanged,	// tree node changed event
			onTreeCustomMenu: _this.onTreeCustomMenu,	// tree custom menu triggered event

			//onCheckCallback: _this.onCheckCallback,		// tree classic 'check_callback'
			//onMoveNode: _this.onMoveNode,
			//onCopyNode: _this.onCopyNode,
		});

		this.leftPanel = this.add({ 'tag': 'div', attrs: { 'id': 'e-main-left' }, children: [this.tree] });
		this.rightPanel = this.add({ 'tag': 'div', attrs: { 'id': 'e-main-right' } });
	}

	onTreeNodeCreate = () => {

	}

	onTreeNodeChanged = () => {
		//console.log(this.tree);
		var _this = this;
		var rawId = this.tree.selectedNode.id;
		var type = rawId.replace(/[^a-zA-Z]+/g, '');	// leaving only letters
		var id = rawId.replace(/\D+/g, '');				// leaving only numbers

		_this.rightPanel.clear();

		switch (type) {

			case 'obj':
				this.loadObject(id);
				break;

			case 'dev':
				this.loadDevice(id);
				break;

			case 'dtp':
				this.loadDatapoint(id)
				break;
		}
	}

	onTreeCustomMenu = () => {

	}

	loadObject(id) {
		var _this = this;
		var url = url = Settings.URL_API_OBJECT + '?objectId=' + id;

		E.get(url, function (data) {

			_this.currentObject = data;

			var params = $.extend(EUtils.getCeDefaultParams(), {
				'data': [
					{
						type: 'group',
						label: E.T('mainInfo'),
						items: [
							{
								name: 'name',
								label: E.T('name'),
								value: data.name,
								type: 'string',
								required: true,
								readOnly: true,
							},
						]
					},
				],
			});

			//console.log(data);
			_this.rightPanel.add(new ContentEngine(params));
		});
	}

	loadDevice(id) {
		var _this = this;
		var url = url = Settings.URL_API_DEVICE + '/' + id;

		E.get(url, function (data) {

			_this.currentDevice = data;

			var params = $.extend(EUtils.getCeDefaultParams(), {
				'data': [
					{
						type: 'group',
						label: E.T('mainInfo'),
						items: [
							{
								name: 'name',
								label: E.T('name'),
								value: data.name,
								type: 'string',
								required: true,
								readOnly: true,
							},
							{
								name: 'description',
								label: E.T('description'),
								value: data.name,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'protocol',
								label: E.T('protocol'),
								value: ETreeMain.PROTOCOL_TYPE[data.protocol],
								type: 'text',
								readOnly: true,
							},
							{
								name: 'url',
								label: E.T('url'),
								value: data.url,
								type: 'text',
								readOnly: true,
								visible: _this.isModbusProtocol(data),
							},
							{
								name: 'unitId',
								label: E.T('unit-id'),
								value: data.unitId,
								type: 'text',
								readOnly: true,
								visible: _this.isModbusProtocol(data),
							},
							{
								name: 'interval',
								label: E.T('interval'),
								value: data.interval,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'clientId',
								label: E.T('client-id'),
								value: data.clientId,
								type: 'text',
								readOnly: true,
								visible: _this.isOtherProtocol(data),
							},
							{
								name: 'topic',
								label: E.T('topic'),
								value: data.topic,
								type: 'text',
								readOnly: true,
								visible: _this.isOtherProtocol(data),
							},
						],
					},
					{
						type: 'group',
						label: E.T('depreciation'),
						items: [
							{
								name: 'deprGL',
								label: E.T('deprGL'),
								value: data.deprGL,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'deprA',
								label: E.T('deprA'),
								value: data.deprA,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'deprLIR',
								label: E.T('deprLIR'),
								value: data.deprLIR,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'deprRL',
								label: E.T('deprRL'),
								value: data.deprRL,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'deprC',
								label: E.T('deprC'),
								value: data.deprC,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'deprSD',
								label: E.T('deprSD'),
								value: data.deprSD,
								type: 'text',
								readOnly: true,
							},
						],
					},
					{
						type: 'group',
						label: E.T('metaInfo'),
						items: [
							{
								name: 'id',
								label: E.T('id'),
								value: data.id,
								type: 'id',
							},
							{
								name: 'objectId',
								label: E.T('objectId'),
								value: data.objectId,
								type: 'id',
							},
						]
					},
				],
			});

			//console.log(data);
			_this.rightPanel.add(new ContentEngine(params));
		});
	}

	loadDatapoint(id) {
		var _this = this;
		var url = Settings.URL_API_DATAPOINT + '/' + id;

		E.get(url, function (data) {

			_this.currentDatapoint = data;

			var params = $.extend(EUtils.getCeDefaultParams(), {
				'data': [
					{
						type: 'group',
						label: E.T('mainInfo'),
						items: _this.datapointAttributes(data),
					},
					{
						type: 'group',
						label: E.T('metaInfo'),
						items: [
							{
								name: 'id',
								label: E.T('id'),
								value: data.id,
								type: 'id',
							},
							{
								name: 'deviceId',
								label: E.T('deviceId'),
								value: data.deviceId,
								type: 'id',
							},
						]
					},
				],
				/*
				// Testing of commands during CE development
				'commands': [
					{
						'id': 'save',
						'label': E.T('save'),
						'click': function (data) {
							alert('1');
						},
					}
				],
				*/
			});

			//console.log(data);
			_this.rightPanel.add(new ContentEngine(params));
		});

	}

	datapointAttributes(data) {

		var retVal = [];
		if (data != undefined) {

			// DATAPOINT_TYPE:
			//	0: 'unknown',
			//	1: 'normal',
			//	2: 'virtual',
			//
			// if(dvm.DatapointType == DatapointType.Virtual)
			//
			if (data.datapointType == 2) {

				// Virtual datapoint
				retVal = [
					{
						name: 'name',
						label: E.T('name'),
						value: data.name,
						type: 'string',
						required: true,
						readOnly: true,
					},
					{
						name: 'datapointFormulaId',
						label: E.T('function'),
						value: E.T(ETreeMain.VDP_FORMULA[data.datapointFormulaId]),
						type: 'string',
						required: true,
						readOnly: true,
					},
					{
						name: 'intervalDatepart',
						label: E.T('interval'),
						value: E.T(ETreeMain.DATE_PART_OR_INTERVAL[data.intervalDatepart]),
						type: 'string',
						required: true,
						readOnly: true,
					},
				];

			} else {

				// Normal or unknown datapoint
				switch (data.deviceProtocol) {

					// case DeviceProtocol.Modbus:
					case 10:
						retVal = [
							{
								name: 'name',
								label: E.T('name'),
								value: data.name,
								type: 'string',
								required: true,
								readOnly: true,
							},
							{
								name: 'description',
								label: E.T('description'),
								value: data.name,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'type',
								label: E.T('type'),
								value: E.T(ETreeMain.DATAPOINT_TYPE[data.datapointType]),
								type: 'text',
								readOnly: true,
							},
							{
								name: 'measureUnit',
								label: E.T('measure-unit'),
								value: data.measureUnit,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'registerAddress',
								label: E.T('register-address'),
								value: data.registerAddress,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'registerType',
								label: E.T('register-type'),
								value: ETreeMain.REGISTER_TYPE[data.registerType],
								type: 'text',
								readOnly: true,
							},
							{
								name: 'functionCode',
								label: E.T('function-code'),
								value: ETreeMain.FUNCTION_CODE[data.functionCode],
								type: 'text',
								readOnly: true,
							},
							{
								name: 'multiplier',
								label: E.T('multiplier'),
								value: data.multiplier,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'offset',
								label: E.T('offset'),
								value: data.offset,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'readWrite',
								label: E.T('read-write'),
								value: E.T(ETreeMain.READ_WRITE_TYPE[data.readWrite]),
								type: 'text',
								readOnly: true,
							},
						];
						break;

					//case DeviceProtocol.BACnet:
					case 20:
					//case DeviceProtocol.MQTT:
					case 30:
					//case DeviceProtocol.CoAP:
					case 40:
					//case DeviceProtocol.OpenThread:
					case 50:
						retVal = [
							{
								name: 'name',
								label: E.T('name'),
								value: data.name,
								type: 'string',
								required: true,
								readOnly: true,
							},
							{
								name: 'description',
								label: E.T('description'),
								value: data.name,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'type',
								label: E.T('type'),
								value: E.T(ETreeMain.DATAPOINT_TYPE[data.datapointType]),
								type: 'text',
								readOnly: true,
							},
							{
								name: 'measureUnit',
								label: E.T('measure-unit'),
								value: data.measureUnit,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'alias',
								label: E.T('alias'),
								value: data.alias,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'multiplier',
								label: E.T('multiplier'),
								value: data.multiplier,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'offset',
								label: E.T('offset'),
								value: data.offset,
								type: 'text',
								readOnly: true,
							},
							{
								name: 'readWrite',
								label: E.T('read-write'),
								value: E.T(ETreeMain.READ_WRITE_TYPE[data.readWrite]),
								type: 'text',
								readOnly: true,
							},
						];
						break;

					default:
						break;

				}
			}
		}
		return retVal;
	}

	getProtocol(data) {

		if (data != null) {

			if (data.hasOwnProperty('protocol')) {
				return data.protocol;
			} else if (data.hasOwnProperty('deviceProtocol')) {
				return data.deviceProtocol;
			}
		}

		return null;
	}

	isModbusProtocol(data) {

		var protocol = this.getProtocol(data);
		var retVal = false;

		switch (protocol) {

			case 10:
				retVal = true;
				break;

			case 20:
			case 30:
			case 40:
			case 50:

				break;
		}

		return retVal;
	}

	isOtherProtocol(data) {

		var protocol = this.getProtocol(data);
		var retVal = false;

		switch (protocol) {

			case 10:
				break;

			case 20:
			case 30:
			case 40:
			case 50:
				retVal = true;
				break;
		}

		return retVal;
	}
}

class EReportsMain extends Element {

	static URL_DATE_RANGE = Settings.URL_API_EXP + 'DateRanges';
	static URL_DATAPOINT_VALUE_DOWNLOAD = Settings.URL_API_DATAPOINT_VALUE + 'Download';
	URL_API_DATAPOINT_VALUE

	labels = {
		checklist: {
			'selectAll': E.T('selectAll'),
			'selectNone': E.T('selectNone'),
			'invertSel': E.T('invertSel'),
		},
	};

	constructor(parent) {
		super({
			tag: 'div',
			attrs: {
				'id': 'e-main-inner',
				'class': 'e-reports flex-col-con',
				//'class': 'e-reports',
			},
			parent: parent,
		});
		this.initUi();
		this.initEvents();
	}

	initUi() {
		var _this = this;

		_this.initDatapointsFilter();
		_this.initOtherFilters();
	}

	// Initialization of Objects, Devices and Datapoints selection, datapoints at the end.
	initDatapointsFilter() {
		var _this = this;

		/// Objects
		// Objects checklist
		this.cklObjects = new Checklist({
			labels: this.labels.checklist,
			onSelChange: function (item) {
				var selectedItems = _this.cklObjects.selectedItems;
				if (selectedItems.length > 0) {

					var params = {
						ids: $.map(selectedItems, function (item) {
							return item.id;
						}).join(Settings.DEFAULT_SEPARATOR)
					};
					var url = Settings.URL_API_DEVICE + '/All?objectIds=' + params.ids;

					E.get(url, function (data) {
						//console.log(data);
						_this.cklDevices.load(data);
					});

				} else {
					_this.cklDevices.clear();
				}
			},
		});

		// Objects container
		this.conObjects = this.add({
			tag: 'div',
			attrs: {
				class: 'flex-col-con'
			},
			children: [
				{ tag: 'h3', text: E.T('select-objects') },//, attrs: {'class': 'block'} },
				this.cklObjects,
			],
		});

		/// Devices
		// Devices checklist
		this.cklDevices = new Checklist({
			labels: this.labels.checklist,
			onSelChange: function (item) {
				var selectedItems = _this.cklDevices.selectedItems;
				if (selectedItems.length > 0) {

					var params = {
						ids: $.map(selectedItems, function (item) {
							return item.id;
						}).join(Settings.DEFAULT_SEPARATOR)
					};
					var url = Settings.URL_API_DATAPOINT + '?deviceIds=' + params.ids;
					E.get(url, function (data) {
						//console.log(data);
						_this.cklDatapoints.load(data);
					});

				} else {
					_this.cklDatapoints.clear();
				}
			},
		});

		// Devices container
		this.conDevices = this.add({
			tag: 'div',
			attrs: {
				class: 'flex-col-con'
			},
			children: [
				{ tag: 'h3', text: E.T('select-devices') },//, attrs: {'class': 'block'} },
				this.cklDevices,
			],
		});

		/// Datapoints
		// Devices checklist
		this.cklDatapoints = new Checklist({
			labels: this.labels.checklist,
			onSelChange: function (item) {
			},
		});

		// Devices container
		this.conDatapoints = this.add({
			tag: 'div',
			attrs: {
				class: 'flex-col-con'
			},
			children: [
				{ tag: 'h3', text: E.T('select-datapoints') },
				this.cklDatapoints,
			],
		});
	}

	// Other filters
	initOtherFilters() {
		var _this = this;

		_this.cmbDateRange = new Dropdown({
			id: 'dateRange{instanceNo}',
			'onSelChange': function () {
				_this.onDateRangeChanged();
			},
		});

		_this.dtpFrom = new Element({
			tag: 'input',
			attrs: {
				type: 'date',
			},
		});

		_this.dtpTo = new Element({
			tag: 'input',
			attrs: {
				type: 'date',
			},
		});

		_this.cmdDownload = new Element({
			tag: 'button',
			attrs: {
				type: 'date',
			},
			text: E.T('download'),
		});

		_this.conOtherFilters = _this.add({
			tag: 'div',
			attrs: {
				class: 'flex-row-con',
				style: 'margin-top: 0.5em; padding: 0.2em; border: dashed 1px gray;',
			},
			children: [
				_this.coverCon(E.T('date-range'), _this.cmbDateRange, 'flex-row-con'),
				_this.coverCon(E.T('from'), _this.dtpFrom, 'flex-row-con'),
				_this.coverCon(E.T('to'), _this.dtpTo, 'flex-row-con'),
				_this.coverCon('&nbsp;', _this.cmdDownload, 'flex-row-con'),
				
			],
		});
	}

	initEvents() {
		var _this = this;

		// Objects
		E.get(Settings.URL_API_OBJECT, function (data) {
			//console.log(data);
			_this.cklObjects.load(data);
		});

		// Date ranges
		E.get(EReportsMain.URL_DATE_RANGE, function (data) {
			//console.log(data);

			// Translation as our received names are ML aliases
			for (var row of data)
				row.name = E.T(row.name);

			// Now just load data
			_this.cmbDateRange.load(data);

			// Select 1st element (actual only if previous data was available)
			//_this.cmbDateRange.val = data[0].id;

			// After load none date from normally set
			// This event triggering helps to initialize from to
			_this.onDateRangeChanged();
		});

		_this.cmdDownload.jqThis.on('click', function () {
			//console.log('download!');

			var url = EReportsMain.URL_DATAPOINT_VALUE_DOWNLOAD;
			var params = {
				'DateFrom': _this.dtpFrom.val,
				'DateTo': _this.dtpTo.val,
				'DatapointIds': $.map(_this.cklDatapoints.selectedItems, function (v, i) {
					return v.id;
				}),
				'MeasureUnit': 3,		// Minute = 3
				'AggregationType': 1,	// RealValue = 1
				'ValueType': 0,			// Value = 0
				'ChartType': 1,			// Points = 1
				//'ComparisonYears': null,
			};
			E.post(url, params, function (data) {
				// console.log(data);
				window.open(data);

			});

		});
	}

	// Cover with container
	coverCon(label, control, cssClass) {

		// _control should be instanceof Element!
		var retVal = new Element({
			tag: 'div',
			attrs: {
				class: 'flex-col-con',
				style: 'margin: 0.5em;',
			},
			children: [
				{
					tag: 'label',
					attrs: {
						for: control.id,
					},
					html: label,
				},
			],
		});
		retVal.add(control);
		return retVal;
	}

	onDateRangeChanged() {
		var _this = this;
		var url = EReportsMain.URL_DATE_RANGE + '?dateRange=' + _this.cmbDateRange.val;
		E.get(url, function (data) {
			console.log(data);
/*
			{
				aggregationType:
				measureUnit:
				range {
					from: bla,
					to: bla,
				}
			}
*/
			if (data != undefined) {
				if (data.range != undefined) {

					if (data.range.from != undefined) {
						// Debug way to check how it works
						var from = new Date(data.range.from);
						var isoStr = from.toISOString();
						var fromStr = isoStr.substring(0, 10);
						_this.dtpFrom.val = fromStr;
					}

					if (data.range.to != undefined) {
						_this.dtpTo.val = new Date(data.range.to).toISOString().substring(0, 10);
					}
				}

			}


		});
	}
}

class EUtils {

	static message(params) {
		var msg;

		if ($.isPlainObject(params)) {
			if (params.msg) {
				msg += params.msg + '\n';
			}

			if (params.alias)
				msg += E.T(params.alias) + '\n';

			if ($.isArray(params.data)) {

				// Adding additional new line if some message available
				if (msg !== undefined)
					msg += '\n';

				for (const dt of params.data) {
					msg += dt + '\n';
				}
			}

		} else if ($.type(params) === "string") {
			if (E.Words[params] !== undefined)
				msg = E.T(params);
		}

		if (msg !== undefined) {
			Utils.alert(msg);
		}
	}

	static getCeDefaultParams() {
		return {
			'utils': {
				'translate': function (alias) {
					return E.T(alias);
				},
				'alert': function (params) {
					EUtils.message(params);
				},
			},
		};
	}
}

class EUsers extends Element {

	static URL_API_USERS = Settings.URL_API_EXP + 'Users';
	static URL_API_UINFO = Settings.URL_API_EXP + 'User/Info?userId=';
	static URL_API_UUPDATE = Settings.URL_API_EXP + 'User/Update';

	#_selectedUser;

	set selectedUser(val) {
		this.#_selectedUser = val;

		if ($.isPlainObject(this.#_selectedUser)) {
			this.cmdNew.jqThis.removeAttr('disabled');
			this.cmdSave.jqThis.removeAttr('disabled');
			this.cmdCancel.jqThis.removeAttr('disabled');
		}
	}
	get selectedUser() {
		return this.#_selectedUser;
	}

	constructor(parent) {
		super({
			tag: 'div',
			attrs: {
				'id': 'e-main-inner',
				'class': 'ad-users-main',
				'attrs': {
					//width: '100%',
				},
				//'class': 'e-reports',
			},
			parent: parent,
		});
		this.#_initUi();
		this.#_initEvents();
	}

	#_initUi() {
		var _this = this;

		this.txtUsersFilter = new Element({
			tag: 'input',
			attrs: {
				//class: 'ad-users-filter',
				placeholder: E.T('searchText'),
				style: 'width: 16em;',
			},
		});

		this.lbxUsers = new Element({
			tag: 'select',
			attrs: {
				//class: 'ad-users-list',
				style: 'width: 16em;',
				size: '24',
			},
		});

		this.cmdNew = new Element({
			tag: 'button',
			attrs: {
				type: 'button',
				class: 'cmd small-margin',
				disabled: true,
			},
			text: E.T('new'),
		});

		this.cmdSave = new Element({
			tag: 'button',
			attrs: {
				type: 'button',
				class: 'cmd small-margin',
				disabled: true,
			},
			text: E.T('save'),
		});

		this.cmdCancel = new Element({
			tag: 'button',
			attrs: {
				type: 'button',
				class: 'cmd small-margin',
				disabled: true,
			},
			text: E.T('cancel'),
		});

		this.cProperties = new Element({
			tag: 'div',
			attrs: {
				class: 'ad-users-properties',
			},
		});

		this.add([
			{
				tag: 'div',
				attrs: {
					class: 'ad-users-list',
				},
				children: [
					this.txtUsersFilter,
					this.lbxUsers,
				]
			},
			{
				tag: 'div',
				attrs: {
					class: 'ad-users-control',
				},
				children: [
					{
						tag: 'div',
						children: [
							this.cmdNew,
							this.cmdSave,
							this.cmdCancel,
						],
						attrs: {
							class: 'block',
						}
					},
					this.cProperties,
				],
			},
		]);

		//console.log(this.children);
	}

	#_initEvents() {
		var _this = this;

		_this.txtUsersFilter.jqThis.on('change paste', function () {
			_this.filterUsers();
		})

		_this.lbxUsers.jqThis.on('change', function () {
			_this.selectUser();
		});

		_this.cmdNew.jqThis.on('click', function () {

			var currentDate = new Date();
			var license = {
				userId: _this.selectedUser.id,
				type: 0,
				validFrom: null,
				validUntil: null,
			};

			_this.addLicense(license);
		});

		_this.cmdSave.jqThis.on('click', function () {
			//console.log(_this.selectedUser);

			var isOk = _this.validateBeforeSave();
			if (isOk) {
				E.post(EUsers.URL_API_UUPDATE, _this.selectedUser, function (data) {
					//console.log(data);
					_this.selectUser();
				});
			}
		});

		_this.cmdCancel.jqThis.on('click', function () {
			_this.selectUser();
		});

		_this.loadData();
	}

	loadData() {
		var _this = this;
		E.get(EUsers.URL_API_USERS, function (users) {
			//console.log(users);
			_this.users = users;
			_this.filterUsers();
		});
	} // loadData()

	filterUsers() {
		var _this = this;
		_this.lbxUsers.clear();

		if ($.isArray(_this.users)) {
			// Retrieving filtered text
			var filerText = _this.txtUsersFilter.val;
			// Is null or empty?
			var isFilerTextEmpty = Utils.isNullOrEmpty(filerText);
			// Uppercasing if not empty
			if (!isFilerTextEmpty) {
				filerText = filerText.toUpperCase();
			}

			// Grepping/Filtering only filtered entities
			_this.filteredUsers = $.grep(_this.users, function (n, i) {
				// Filtering only objects
				if ($.isPlainObject(n)) {

					if (isFilerTextEmpty)
						return true;

					// Filtering only having name property (probably all will have)
					if (n.hasOwnProperty('name')) {
						// Uppercasing and searching for specific text inclusion
						return n.name.toUpperCase().includes(filerText);
					}
				}
				return false;
			});

			for (var user of _this.filteredUsers) {
				_this.lbxUsers.add({
					tag: 'option',
					attrs: {
						'value': user.id,
					},
					text: user.name,
				});
			} // for

		} // if ($.isArray(_this.users))
	} // filterUsers()

	selectUser() {
		var _this = this;

		// This clean up happens, because selection changed, no matter will be sucessful load or not.
		_this.cProperties.clear();
		_this.clearLicenses();

		var userId = _this.lbxUsers.val;
		//console.log(userId);
		var url = EUsers.URL_API_UINFO + userId;

		E.get(url, function (userInfo) {
			//console.log(data);
			_this.selectedUser = userInfo;
			// This clean up happens to prevent multiple lagged callbacks return (were such cases), as only last one should remain
			_this.cProperties.clear();
			_this.cProperties.add({
				tag: 'h3',
				text: _this.selectedUser.name,
			});

			_this.cLicenses = _this.cProperties.add({
				tag: 'div',
				attrs: {
					class: 'block',
				},
			});

			if ($.isArray(_this.selectedUser.licenses)) {

				for (var license of _this.selectedUser.licenses) {
					// Add new licenses (UI will be created and handled inside of this method)
					_this.addLicense(license);
				} 
			} 
		}); 
	} // selectUser()

	createLicenseUi(license) {
		var _this = this;

		var isNew = license.id == undefined;
		var licenseId = 'LIC_' + (isNew ? 'NEW' : license.id);
		var clValidFrom, clValidUntil;

		// Delete button
		var clClose = new Element({
			tag: 'label',
			text: 'X',
		});

		clClose.jqThis.on('click', function () {
			if (Utils.confirm(E.T('sure-delete'))) {
				_this.removeLicense(license);
			}
		});

		// ValidFrom DateTimePicker
		clValidFrom = new Element({
			tag: 'input',
			attrs: {
				type: 'datetime-local', // type: 'date',
				value: license.validFrom,
				disabled: !isNew,
				class: 'iblock',
			}
		});

		clValidFrom.jqThis.on('change', function () {
			license.validFrom = $(this).val();
			//console.log('clValidFrom: ' + license.validFrom);
		})

		// Valid Until DateTimePicker
		clValidUntil = new Element({
			tag: 'input',
			attrs: {
				type: 'datetime-local', // type: 'date',
				value: license.validUntil,
				disabled: !isNew,
				class: 'iblock',
			}
		});

		clValidUntil.jqThis.on('change', function () {
			license.validUntil = $(this).val();
			//console.log('clValidUntil: ' + license.clValidUntil);
		})

		// License Type Dropdown
		var clLicenseType = new Element({
			tag: 'select',
			attrs: {
				class: 'iblock',
				disabled: !isNew,
			},
			children: [
				{
					tag: 'option',
					text: E.T('none'),
					attrs: {
						value: 0,
					},
				},
				{
					tag: 'option',
					text: E.T('lic1desc'),
					attrs: {
						value: 1,
					},
				},
				{
					tag: 'option',
					text: E.T('lic2desc'),
					attrs: {
						value: 2,
					},
				},
				{
					tag: 'option',
					text: E.T('lic3desc'),
					attrs: {
						value: 3,
					},
				},

			],
		});
		clLicenseType.jqThis.on('change', function () {
			license.type = clLicenseType.val;
		});
		clLicenseType.val = license.type;

		// Active CheckBox
		var clActive = new Element({
			tag: 'input',
			attrs: {
				id: licenseId,
				type: 'checkbox',
				checked: license.active,
			},
		});

		clActive.jqThis.on('change', function () {
			license.active = clActive.prop('checked');
		});

		// The whole license
		return new Element({
			tag: 'div',
			attrs: {
				class: 'exp-license exp-license_t' + license.type,
			},
			children: [

				// Close button
				{
					tag: 'div',
					attrs: {
						class: 'exp-close',
					},
					children: [
						clClose,
					],
				},

				// Header of the license
				{
					tag: 'h4',
					text: E.T('lic' + license.type + 'desc'),
				},

				// Valid From
				{
					tag: 'div',
					attrs: {
						class: 'block',
					},
					children: [
						{
							tag: 'label',
							text: E.T('validFrom'),
							attrs: {
								class: 'iblock',
							}
						},
						clValidFrom,
					],
				},

				// Valid Until
				{
					tag: 'div',
					attrs: {
						class: 'block',
					},
					children: [
						{
							tag: 'label',
							text: E.T('validUntil'),
							attrs: {
								class: 'iblock',
							}
						},
						clValidUntil,
					],
				},

				// License type
				{
					tag: 'div',
					attrs: {
						class: 'block',
					},
					children: [
						{
							tag: 'label',
							text: E.T('license'),
						},
						clLicenseType,
					],
				},

				// Active
				{
					tag: 'div',
					attrs: {
						class: 'block exp-license-active',
					},
					children: [
						{
							tag: 'label',
							attrs: {
								for: licenseId,
							},
							text: E.T('active'),
						},
						clActive,
					],
				}

			],
		}); // return
	} // createLicenseUi(license)

	/*
	 * Clearing all the licenses (possible because different user selected)
	 */
	clearLicenses() {
		this.licenseMap = [];
	}

	/*
	 * Create/Add new license 
	 */
	addLicense(license) {

		// Creating license UI
		var ui = this.createLicenseUi(license);

		// Add license and its ui to licenses and their ui map, which will allow to remove them smoothly
		this.licenseMap.push({
			license: license,
			ui: ui,
		});

		// Add license to licenses UI container, that it was visible to an user
		this.cLicenses.add(ui); // cLicenses.add

		// Specific license can be already part of this array, if this is first load after new user selected
		if (!this.selectedUser.licenses.includes(license)) {
			// We need to add it to selected user's array only it if is not part of it, eg. newly created via UI by current user
			this.selectedUser.licenses.push(license);
		}
	}

	/*
	 * Remove specific license by its object
	 * 
	 * @param {any} license
	 */
	removeLicense(license) {
		var index;

		// Remove from selected user licenses array
		index = this.selectedUser.licenses.indexOf(license);
		if (index != -1) {
			var removed = this.selectedUser.licenses.splice(index, 1);
		}

		// Removing the license from license and its UI map, as it was removed
		index = this.licenseMap.findIndex(l => l.license == license);
		if (index != -1) {
			var map = this.licenseMap[index];
			var removed = this.licenseMap.splice(index, 1);
			if (map.ui instanceof Element)
				map.ui.remove();
		}

	}

	validateBeforeSave() {

		var errFrom = 0, errUntil = 0, errLicense = 0

		for (var license of this.selectedUser.licenses) {

			// Validate only newly created licenses
			if (license.id == undefined) {

				if (!Utils.isDate(license.validFrom))
					errFrom++;

				if (!Utils.isDate(license.validUntil))
					errUntil++;

				if (license.type < 1 || license.type > 3)
					errLicense++;
			}
		}

		var retVal = errFrom == 0 && errUntil == 0 && errLicense == 0;

		if (!retVal) {
			var errMsg = E.T('fillFields') + ":\r\n";

			if (errFrom > 0)
				errMsg += "\r\n[" + E.T('validFrom') + " x " + errFrom + "]";

			if (errUntil > 0)
				errMsg += "\r\n[" + E.T('validUntil') + " x " + errUntil + "]";

			if (errLicense > 0)
				errMsg += "\r\n[" + E.T('license') + " x " + errLicense + "]";

			Utils.alert(errMsg);
		}

		return retVal;
	}

}

