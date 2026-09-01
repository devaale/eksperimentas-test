//#define ENABLE_FAKE_PAYMENT_METHODS
#define FILE_LOGGING

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using EVP.WebToPay.ClientAPI;

using Experiment.Core;
using Experiment.Core.Enums;
using Experiment.Core.Metadata;
using Experiment.Core.Data;
using Experiment.Data.Enums;
using Experiment.Data.Metadata;
using Experiment.Core.IO;

using M = Experiment.Data.Models;
using Experiment.Data.Services.SuperHow.Data;

using Website.Data;
using Website.Models;

namespace Website.Controllers
{
	[Authorize]
	[RoutePrefix("api/Billing")]
	public class BillingApiController : ApiController
	{
		#region Const
		const string TYPE_NAME = nameof(BillingApiController);
		const string ALGORITHM_NAME_AI_HEATING_CONTROL = "AI šildymas (valdymas)";
		const string ALGORITHM_NAME_AI_HEATING_ALARM = "AI alarm (šildymas)";
		const string ALGORITHM_NAME_AI_COOLING_ALARM = "AI šaldymas alarm";
		const string ALGORITHM_NAME_AI_COOLING_CONTROL = "AI valdymas (šaldymas)";
		const string DEVICE_NAME_CALCULATIONS = "Calculations";
		const string DATAPOINT_NAME_ENVIRONMENTAL_IMPACT = "Environmental impact";
		const int DATAPOINT_FORMULA_ID_ENVIRONMENTAL_IMPACT = 1010;
		const string DATAPOINT_NAME_THERMAL_COMFORT = "Thermal comfort";
		const int DATAPOINT_FORMULA_ID_THERMAL_COMFORT = 1020;
		const string DATAPOINT_NAME_POWER_FLOW_DISTRIBUTION = "Power Flow Distribution";
		const int DATAPOINT_FORMULA_ID_POWER_FLOW_DISTRIBUTION = 2010;
		static readonly string[] AI_ALGORITHM_NAMES = new[]
		{
			ALGORITHM_NAME_AI_HEATING_CONTROL,
			ALGORITHM_NAME_AI_HEATING_ALARM,
			ALGORITHM_NAME_AI_COOLING_ALARM,
			ALGORITHM_NAME_AI_COOLING_CONTROL,
		};
		static readonly CalculationDatapointPreset[] CALCULATIONS_DEVICE_DATAPOINT_PRESETS = new[]
		{
			new CalculationDatapointPreset(DATAPOINT_NAME_ENVIRONMENTAL_IMPACT, DATAPOINT_FORMULA_ID_ENVIRONMENTAL_IMPACT),
			new CalculationDatapointPreset(DATAPOINT_NAME_THERMAL_COMFORT, DATAPOINT_FORMULA_ID_THERMAL_COMFORT),
			new CalculationDatapointPreset(DATAPOINT_NAME_POWER_FLOW_DISTRIBUTION, DATAPOINT_FORMULA_ID_POWER_FLOW_DISTRIBUTION),
		};

		#endregion

		#region Attributes
		private ApplicationDbContext db = new ApplicationDbContext();

		#endregion

		#region Properties

		#endregion

		#region Helpers
		ILogger CreateLogger ()
		{
#if FILE_LOGGING
			return new FileLogger(5, WebsiteDefaults.GetLogsPath(), nameof(BillingApiController));
#else
			return new DebugLogger(5);
#endif
		}

		private sealed class AlgorithmEnsureStats
		{
			public int Created { get; set; }
			public int Updated { get; set; }
		}

		private sealed class CallbackData
		{
			public string OrderNo { get; set; }
			public bool Success { get; set; }
			public string CallbackRawData { get; set; }
		}

		private sealed class CalculationDatapointPreset
		{
			public CalculationDatapointPreset(string name, int formulaId)
			{
				Name = name;
				FormulaId = formulaId;
			}

			public string Name { get; }
			public int FormulaId { get; }
		}

		private static Algorithm CreateDefaultAlgorithm(int objectId, string name, AlgorithmType type)
		{
			var now = DateTime.Now;
			return new Algorithm()
			{
				ObjectId = objectId,
				Name = name,
				Type = type,
				DateStart = DateTime.Today,
				DateEnd = DateTime.Today,
				TimeStart = now.TimeOfDay,
				TimeEnd = now.TimeOfDay,
				ValueFrom = 0m,
				ValueTo = 1m,
				AlarmId = 0,
				GroupId = 0,
				DatapointId = 0,
				ValueOff = 0m,
				ValueOn = 1m,
				Status = 0m,
				ReminderAfterHours = 24,
				OnMonday = false,
				OnTuesday = false,
				OnWednesday = false,
				OnThursday = false,
				OnFriday = false,
				OnSaturday = false,
				OnSunday = false,
			};
		}

		private static Device CreateDefaultCalculationsDevice(int objectId)
		{
			return new Device()
			{
				Name = DEVICE_NAME_CALCULATIONS,
				ObjectId = objectId,
				UnitId = 2,
				Protocol = DeviceProtocol.API,
				Interval = 3600,
			};
		}

		private static Datapoint CreateDefaultCalculationsDatapoint(int deviceId, CalculationDatapointPreset preset)
		{
			return new Datapoint()
			{
				DeviceId = deviceId,
				DatapointType = DatapointType.Virtual,
				Name = preset.Name,
				DatapointFormulaId = preset.FormulaId,
				IntervalDatepart = DatePartOrInterval.Day,
				Multiplier = 1m,
			};
		}

		private void EnsureAiLicenseAlgorithms(string userId, ILogger logger, string vLoc)
		{
			var objectIds = GetActiveObjectIds(userId);
			if (objectIds.Count == 0)
			{
				logger.WriteLine(5, $"{vLoc}, no active objects found for user [{userId}], skipping AI algorithm creation.");
				return;
			}

			var existing = GetExistingAiAlgorithms(objectIds);
			var stats = new AlgorithmEnsureStats();

			if (EnsureAiAlarmAlgorithms(objectIds, existing, stats))
			{
				db.SaveChanges();
			}

			if (EnsureAiTriggerAlgorithms(objectIds, existing, stats))
			{
				db.SaveChanges();
			}

			logger.WriteLine(5, $"{vLoc}, AI algorithms ensure result: created={stats.Created}, updated={stats.Updated}, objects={objectIds.Count}, user=[{userId}]");
		}

		private void EnsureImprovedLicenseDevices(string userId, ILogger logger, string vLoc)
		{
			var objectIds = GetActiveObjectIds(userId);
			if (objectIds.Count == 0)
			{
				logger.WriteLine(5, $"{vLoc}, no active objects found for user [{userId}], skipping calculations device creation.");
				return;
			}

			var calculationsDevices = db.Devices
				.Where(d =>
					objectIds.Contains(d.ObjectId) &&
					d.Name == DEVICE_NAME_CALCULATIONS)
				.ToList();

			var existingObjectIds = calculationsDevices
				.Select(d => d.ObjectId)
				.Distinct()
				.ToList();

			var missingObjectIds = objectIds
				.Where(objectId => !existingObjectIds.Contains(objectId))
				.ToList();

			if (missingObjectIds.Count == 0)
			{
				logger.WriteLine(5, $"{vLoc}, calculations devices already exist for all active objects. objects={objectIds.Count}, user=[{userId}]");
			}
			else
			{
				foreach (var objectId in missingObjectIds)
				{
					db.Devices.Add(CreateDefaultCalculationsDevice(objectId));
				}

				db.SaveChanges();

				calculationsDevices = db.Devices
					.Where(d =>
						objectIds.Contains(d.ObjectId) &&
						d.Name == DEVICE_NAME_CALCULATIONS)
					.ToList();
			}

			var calculationDeviceIds = calculationsDevices
				.Select(d => d.Id)
				.ToList();

			var calculationFormulaIds = CALCULATIONS_DEVICE_DATAPOINT_PRESETS
				.Select(preset => preset.FormulaId)
				.ToList();

			var existingCalculationDatapoints = db.Datapoints
				.Where(dp =>
					calculationDeviceIds.Contains(dp.DeviceId) &&
					dp.DatapointType == DatapointType.Virtual &&
					dp.DatapointFormulaId.HasValue &&
					calculationFormulaIds.Contains(dp.DatapointFormulaId.Value))
				.Select(dp => new
				{
					dp.DeviceId,
					FormulaId = dp.DatapointFormulaId.Value
				})
				.ToList();

			int createdDatapoints = 0;
			foreach (var calculationsDevice in calculationsDevices)
			{
				foreach (var preset in CALCULATIONS_DEVICE_DATAPOINT_PRESETS)
				{
					bool alreadyExists = existingCalculationDatapoints.Any(existing =>
						existing.DeviceId == calculationsDevice.Id &&
						existing.FormulaId == preset.FormulaId);

					if (!alreadyExists)
					{
						db.Datapoints.Add(CreateDefaultCalculationsDatapoint(calculationsDevice.Id, preset));
						createdDatapoints++;
					}
				}
			}

			if (createdDatapoints > 0)
			{
				db.SaveChanges();
			}

			logger.WriteLine(5, $"{vLoc}, calculations devices ensure result: devicesCreated={missingObjectIds.Count}, virtualDatapointsCreated={createdDatapoints}, objects={objectIds.Count}, user=[{userId}]");
		}

		private List<int> GetActiveObjectIds(string userId)
		{
			return db.Objects
				.Where(o => o.UserId.Equals(userId) && o.Deleted == null)
				.Select(o => o.Id)
				.ToList();
		}

		private List<Algorithm> GetExistingAiAlgorithms(ICollection<int> objectIds)
		{
			return db.Algorithms
				.Where(a =>
					objectIds.Contains(a.ObjectId) &&
					a.Deleted == null &&
					AI_ALGORITHM_NAMES.Contains(a.Name))
				.ToList();
		}

		private static Algorithm FindAlgorithm(List<Algorithm> existing, int objectId, string name)
		{
			return existing.FirstOrDefault(a => a.ObjectId == objectId && a.Name.Equals(name));
		}

		private bool EnsureAiAlarmAlgorithms(IEnumerable<int> objectIds, List<Algorithm> existing, AlgorithmEnsureStats stats)
		{
			var changed = false;

			foreach (var objectId in objectIds)
			{
				changed |= EnsureAiAlarmAlgorithm(objectId, ALGORITHM_NAME_AI_HEATING_ALARM, existing, stats);
				changed |= EnsureAiAlarmAlgorithm(objectId, ALGORITHM_NAME_AI_COOLING_ALARM, existing, stats);
			}

			return changed;
		}

		private bool EnsureAiAlarmAlgorithm(int objectId, string alarmName, List<Algorithm> existing, AlgorithmEnsureStats stats)
		{
			if (FindAlgorithm(existing, objectId, alarmName) != null)
			{
				return false;
			}

			var alarm = CreateDefaultAlgorithm(objectId, alarmName, AlgorithmType.Alarm);
			db.Algorithms.Add(alarm);
			existing.Add(alarm);
			stats.Created++;
			return true;
		}

		private bool EnsureAiTriggerAlgorithms(IEnumerable<int> objectIds, List<Algorithm> existing, AlgorithmEnsureStats stats)
		{
			var changed = false;

			foreach (var objectId in objectIds)
			{
				var heatingAlarmId = GetAlarmId(existing, objectId, ALGORITHM_NAME_AI_HEATING_ALARM);
				var coolingAlarmId = GetAlarmId(existing, objectId, ALGORITHM_NAME_AI_COOLING_ALARM);

				changed |= EnsureAiTriggerAlgorithm(
					objectId,
					ALGORITHM_NAME_AI_HEATING_CONTROL,
					heatingAlarmId,
					existing,
					stats);

				changed |= EnsureAiTriggerAlgorithm(
					objectId,
					ALGORITHM_NAME_AI_COOLING_CONTROL,
					coolingAlarmId,
					existing,
					stats);
			}

			return changed;
		}

		private static int GetAlarmId(List<Algorithm> existing, int objectId, string alarmName)
		{
			var alarm = FindAlgorithm(existing, objectId, alarmName);
			return alarm != null ? alarm.Id : 0;
		}

		private bool EnsureAiTriggerAlgorithm(
			int objectId,
			string triggerName,
			int relatedAlarmId,
			List<Algorithm> existing,
			AlgorithmEnsureStats stats)
		{
			var trigger = FindAlgorithm(existing, objectId, triggerName);
			if (trigger == null)
			{
				trigger = CreateDefaultAlgorithm(objectId, triggerName, AlgorithmType.AlarmTrigger);
				trigger.AlarmId = relatedAlarmId;
				db.Algorithms.Add(trigger);
				existing.Add(trigger);
				stats.Created++;
				return true;
			}

			if (relatedAlarmId > 0 && trigger.AlarmId <= 0)
			{
				trigger.AlarmId = relatedAlarmId;
				stats.Updated++;
				return true;
			}

			return false;
		}

		private static void AddCallbackParam(NameValueCollection namedCollection, StringBuilder sbData, string key, string value)
		{
			if (string.IsNullOrEmpty(key))
			{
				return;
			}

			namedCollection.Set(key, value);
			sbData.Append(string.Format("{0}	{1}{2}", key, value, Environment.NewLine));
		}

		private async Task<CallbackData> ReadCallbackDataAsync(
			Client client,
			ILogger logger,
			string vLoc)
		{
			logger.WriteLine(5, string.Format("{0}, Preparing callback params from [{1}] [{2}]...",
				vLoc, Request.Method, Request.RequestUri));
			var namedCollection = new NameValueCollection();
			var sbData = new StringBuilder();

			// Query string parameters (main callback shape)
			var queryKvps = Request.GetQueryNameValuePairs();
			foreach (var kvp in queryKvps)
			{
				AddCallbackParam(namedCollection, sbData, kvp.Key, kvp.Value);
			}

			// Some integrations send callback params as form-url-encoded body.
			if (Request.Content != null)
			{
				var mediaType = Request.Content.Headers?.ContentType?.MediaType;
				if (!string.IsNullOrEmpty(mediaType) &&
					mediaType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
				{
					var formData = await Request.Content.ReadAsFormDataAsync();
					foreach (var key in formData.AllKeys)
					{
						AddCallbackParam(namedCollection, sbData, key, formData[key]);
					}
				}
			}

			logger.WriteLine(5, string.Format("{0}, Prepared params [{1}]", vLoc, sbData));
			logger.WriteLine(5, string.Format("{0}, Analyzing Paysera return...", vLoc));
			var data = client.GetMacroCallbackData(namedCollection);

			return new CallbackData()
			{
				OrderNo = data.OrderId,
				Success = data.Status == 1,
				CallbackRawData = sbData.ToString(),
			};
		}

		private void ConfirmOrderInDatabase(string orderNo, bool success, string callbackRawData, ILogger logger, string vLoc)
		{
			logger.WriteLine(5, string.Format("{0}, EXEC prcOrderConfirm [{1}], [{2}], [{3}], [{4}]",
				vLoc,
				HttpContext.Current.Request.UserHostAddress,
				orderNo,
				callbackRawData,
				success));

			var dda = new Sql(db.Database.Connection, logger);
			var cmd = dda.CreateCommand();
			cmd.CommandText = "EXEC prcOrderConfirm @ip, @orderNo, @data, @success";
			cmd.Parameters.Add(new SqlParameter("@ip", HttpContext.Current.Request.UserHostAddress));
			cmd.Parameters.Add(new SqlParameter("@orderNo", orderNo));
			cmd.Parameters.Add(new SqlParameter("@data", callbackRawData));
			cmd.Parameters.Add(new SqlParameter("@success", success));
			dda.Execute(cmd);
		}

		private Order GetOrderByOrderNo(string orderNo, ILogger logger, string vLoc)
		{
			logger.WriteLine(5, string.Format("{0}, prcOrderConfirm: Done. Looking for tblOrder.OrderNo={1}...", vLoc, orderNo));
			var order = db.Orders.FirstOrDefault(o => o.OrderNo.Equals(orderNo));
			if (order == null)
			{
				logger.WriteLine(1, string.Format("{0}, Order with OrderNo={1} WASN'T FOUUND in tblOrder DB table!", vLoc, orderNo));
				return null;
			}

			logger.WriteLine(5, string.Format("{0}, OrderNo={1}, Id={2}, proceeding to blockchain processing...", vLoc, orderNo, order.Id));
			return order;
		}

		private void TryEnsureAiAlgorithmsForOrder(Order order, bool paymentSuccess, ILogger logger, string vLoc)
		{
			if (order == null || !paymentSuccess || !OrderContainsAiLicense(order.Id))
			{
				return;
			}

			try
			{
				EnsureAiLicenseAlgorithms(order.UserId, logger, vLoc);
			}
			catch (Exception ex)
			{
				logger.WriteLine(1, $"{vLoc}, failed to create AI algorithms for user [{order.UserId}]: {ex.Message}");
			}
		}

		private void TryEnsureImprovedLicenseDevicesForOrder(Order order, bool paymentSuccess, ILogger logger, string vLoc)
		{
			if (order == null || !paymentSuccess || !OrderContainsImprovedLicense(order.Id))
			{
				return;
			}

			try
			{
				EnsureImprovedLicenseDevices(order.UserId, logger, vLoc);
			}
			catch (Exception ex)
			{
				logger.WriteLine(1, $"{vLoc}, failed to create calculations devices for user [{order.UserId}]: {ex.Message}");
			}
		}

		private bool OrderContainsAiLicense(Guid orderId)
		{
			return db.OrderDetails.Any(od =>
				od.OrderId == orderId &&
				od.LicenseType == UserLicenseType.License3);
		}

		private bool OrderContainsImprovedLicense(Guid orderId)
		{
			return db.OrderDetails.Any(od =>
				od.OrderId == orderId &&
				od.LicenseType == UserLicenseType.License2);
		}

		private static async Task CompleteOrderOnBlockchainAsync(Order order)
		{
			if (order == null)
			{
				return;
			}

			var bc = new Blockchain();
			await bc.OrderCompleted(order.Id);
		}

		private static HttpResponseMessage BuildMacroCallbackOkResponse()
		{
			MacroCallbackResponse mcbResponse = new MacroCallbackResponse(
				MacroCallbackResponseStatus.Ok);

			return new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(mcbResponse.ToString(), Encoding.UTF8, "text/plain"),
			};
		}

		private async Task<HttpResponseMessage> ProcessConfirmOrderAsync(string methodName)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, methodName);
			var logger = CreateLogger();

			try
			{
				// Deciphering required for Paysera params
				var payseraProjectId = Defaults.PAYSERA_PROJECT_ID;
				var payseraSignPassword = Defaults.PAYSERA_SIGN_PASSWORD;

				// Initializing Paysera Client...
				logger.WriteLine(5, string.Format("{0}, Initializing Paysera Client...", vLoc));
				// @see https://github.com/evp/webtopay-lib-dotnet/blob/master/example/EVP.WebToPay.ClientAPIExample/MacroCallback.aspx.cs
				var client = new Client(payseraProjectId, payseraSignPassword);

				var callbackData = await ReadCallbackDataAsync(client, logger, vLoc);
				ConfirmOrderInDatabase(callbackData.OrderNo, callbackData.Success, callbackData.CallbackRawData, logger, vLoc);

				var order = GetOrderByOrderNo(callbackData.OrderNo, logger, vLoc);
				TryEnsureImprovedLicenseDevicesForOrder(order, callbackData.Success, logger, vLoc);
				TryEnsureAiAlgorithmsForOrder(order, callbackData.Success, logger, vLoc);
				await CompleteOrderOnBlockchainAsync(order);
			}
			catch (Exception ex)
			{
				logger.WriteLine(1, string.Format("{0}, FAILED: {1}{2}{3}", vLoc, ex.Message, Environment.NewLine, ex.StackTrace));
			}

			// In any case we returning OK that specific callback was processed
			return BuildMacroCallbackOkResponse();
		}

		#endregion

		#region Methods
		[Route("Products")]
		public M.LicenseProduct[] GetProducts()
		{
			var retVal = new M.LicenseProduct[]
			{
				new M.LicenseProduct() { LicenseType = UserLicenseType.License2, Price = 5},
				new M.LicenseProduct() { LicenseType = UserLicenseType.License3, Price = 10},
			};

			return retVal;
		}

		[Route("PaymentMethods")]
		public M.PaymentMethod[] GetPaymentMethods()
		{
			var retVal = new M.PaymentMethod[]
			{
				new M.PaymentMethod() { Id = "Paysera", Name = "Paysera"},
#if ENABLE_FAKE_PAYMENT_METHODS
				new M.PaymentMethod() { Id = "FAKE1", Name = "Fake Payment Method 1"},
				new M.PaymentMethod() { Id = "FAKE2", Name = "Fake Payment Method 2"},
				new M.PaymentMethod() { Id = "Krust&Co", Name = "Krust & Co"},
#endif
			};

			return retVal;
		}

		[Route("PostOrder")]
		[ResponseType(typeof(M.Order))]
		public IHttpActionResult PostOrder(M.Order po)
		{
			var vLoc = string.Format("{0}::{1}", TYPE_NAME, nameof(PostOrder));
			var logger = CreateLogger();
			logger.WriteLine(4, vLoc);

			using (var transaction = db.Database.BeginTransaction())
			{
				string currentUserId = User.Identity.GetUserId();
				var order = new Order()
				{
					UserId = currentUserId,
					FullPrice = po.FullPrice,
					UsedTokens = po.UsedTokens,
					Discount = po.Discount,
					FinalPrice = po.FinalPrice,
					PaymentMethodId = po.PaymentMethodId,
					PaymentMethod = po.PaymentMethod,
					Posted = DateTime.Now,
					PostedIp = HttpContext.Current.Request.UserHostAddress, // @see https://stackoverflow.com/a/49924501
					State = OrderState.Posted,
					OrderDetails = new List<OrderDetail>(),
				};

				db.Orders.Add(order);
				db.SaveChanges();

				foreach (var pod in po.OrderDetails)
				{
					var od = new OrderDetail()
					{
						OrderId = order.Id,
						LicenseType = pod.LicenseType,
						NumMonths = pod.NumMonths,
					};
					db.OrderDetails.Add(od);
				}
				db.SaveChanges();
				transaction.Commit();
				logger.WriteLine(5, string.Format("{0}, Order committed, OrderId=[{1}], OrderNo=[{2}], END!", vLoc, order.Id, order.OrderNo));

				po.OrderNo = order.OrderNo;
			}
			return Ok(po);
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("ConfirmOrder")]
		public async Task<HttpResponseMessage> GetConfirmOrder()
		{
			return await ProcessConfirmOrderAsync(nameof(GetConfirmOrder));
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("ConfirmOrder")]
		public async Task<HttpResponseMessage> PostConfirmOrder()
		{
			return await ProcessConfirmOrderAsync(nameof(PostConfirmOrder));
		}

		#endregion

	}
}
