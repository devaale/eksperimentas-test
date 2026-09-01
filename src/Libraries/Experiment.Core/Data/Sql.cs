#define ENDLESS_SQLCMD_TIMEOUT
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Experiment.Core;
using Experiment.Core.IO;
using Experiment.Core.Metadata;
using System.Data.SqlClient;


namespace Experiment.Core.Data{
	public class Sql : ErrorInfo, ISql
	{
		#region Constants
		const string TYPE_NAME = nameof(Sql);
		const string DEFAULT_TABLE_NAME = "r"; // short to get smaller XML objects

		#endregion

		#region Attributes


		#endregion

		#region Properties
		public IDbConnection Connection { get; protected set; }
		public ILogger Logger { get; protected set; }

		#endregion

		#region Ctor
		public Sql(IDbConnection connection, ILogger logger)
		{
			// FYI: CTOR a.k.a. constructor
			Validation.RequireValid(connection, "EXP.Core.Data.Sql::CTOR [connection] should be initialized and not NULL!");
			Validation.RequireValid(logger, "EXP.Core.Data.Sql::CTOR [logger] should be initialized and not NULL!");

			Connection = connection;
			Logger = logger;
		}

		#endregion

		#region Helpers
		public string GetScalarValue(DataTable table)
		{
			string retVal = null;

			if (table != null)
			{
				if (table.Rows.Count > 0)
				{
					retVal = (table.Rows[0][0] == DBNull.Value ? null : table.Rows[0][0].ToString());
				}
			}

			return retVal;
		}

		#endregion

		#region Static
		public static string Magic(string text)
		{
			return text.Replace("'", "''");
		}

		/// <summary>
		/// Used for cases when database field datetime can be null, 
		/// in order to return null or datetime
		/// 
		/// var result = ResolveDbDateTime(dataTable.Row[0][1]);
		/// if(result.hasValue) {
		///		// not null
		///		Debug.Print(result.Value);
		/// }
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static DateTime? ResolveDbDateTime(object o)
		{
			DateTime? retVal = null;
			if (!DBNull.Value.Equals(o))
			{
				retVal = Convert.ToDateTime(o);
			}
			return retVal;
		}

		/// <summary>
		/// Returns single DataTable column, by columnName as IList[object]
		/// </summary>
		/// <param name="table"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public static IList<object> DataTableColumnToList (DataTable table, string columnName)
		{
			List<object> retVal = null; //table.AsEnumerable().Select(r => r.Field<object>(columnName)).ToList();
			return retVal;
		}



		#endregion

		#region Methods

		#region Error handling
		//public void ReportError(Exception ex, string sql, string msg)
		public void ReportError(string vLoc, Exception ex)
		{
			Error(ex);

			if(Logger != null)
			{
				var logLevel = 0;
				if(ex is DivideByZeroException)
				{
					logLevel = 1;
				}

				Logger.WriteLine(logLevel, string.Format("{0}, Failed: {1}", vLoc, ex.Message));
			}
		}

		#endregion

		#region Connection Open/Close

		public bool Open()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(Open));
			bool retVal = false;

			try
			{
				if (Connection.State == ConnectionState.Closed)
					Connection.Open();
				retVal = true;
			}
			catch (Exception ex)
			{
				ReportError(vLoc, ex);
			}
			finally
			{
			}

			return retVal;
		}

		public void Close()
		{
			var vLoc = string.Format("{0}::{1}()", TYPE_NAME, nameof(Close));

			try
			{
				if (Connection.State != ConnectionState.Closed)
					Connection.Close();
			}
			catch (Exception ex)
			{
				ReportError(vLoc, ex);
			}
			finally
			{

			}
		}

		#endregion

		#region Commands and Parameters

		public IDbCommand CreateCommand()
		{
			IDbCommand cmd = Connection.CreateCommand();
#if ENDLESS_SQLCMD_TIMEOUT
			cmd.CommandTimeout = 0;
#endif
			return cmd;
		}

		public void AddParameter(IDbCommand cmd, string name, object value, DbType type)
		{
			IDbDataParameter param = cmd.CreateParameter();
			param.ParameterName = name;
			param.Value = value;
			param.DbType = type;

			cmd.Parameters.Add(param);
		}

		public void AddParameter(IDbCommand cmd, string name, object value, SqlDbType type)
		{
			IDbDataParameter param = cmd.CreateParameter();
			param.ParameterName = name;
			param.Value = value;

			if(param is SqlParameter)
			{
				((SqlParameter)param).SqlDbType = type;
			}

			cmd.Parameters.Add(param);
		}

		public void AddParameter(IDbCommand cmd, string name, object value)
		{
			IDbDataParameter param = cmd.CreateParameter();
			param.ParameterName = name;
			param.Value = value;

			cmd.Parameters.Add(param);
		}

		#endregion

		#region Query

		/// <summary>
		/// Query DataTable
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public DataTable Query(IDbCommand cmd, string tableName)
		{
			var vLoc = string.Format("{0}::{1}(IDbCommand::CommandText=[{2}])", 
				TYPE_NAME, nameof(Query), cmd.CommandText);

			DataTable retVal = new DataTable(tableName);
			IDataReader reader = null;

			try
			{
				Error();
				Open();
				if (!IsError)
				{
					Debug.Print("Sql:Query> " + cmd.CommandText);
					reader = cmd.ExecuteReader();
					retVal.Load(reader);
					return retVal;
				}

			}
			catch (Exception ex)
			{
				ReportError(vLoc, ex);
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}
				Close();
			}

			return retVal;

		}

		/// <summary>
		/// Query DataTable
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public DataTable Query(IDbCommand cmd)
		{
			return Query(cmd, cmd.GetHashCode().ToString());
		}

		/// <summary>
		/// Query DataTable
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public DataTable Query(string sql, string tableName)
		{
			IDbCommand cmd = CreateCommand();
			cmd.CommandText = sql;
			return Query(cmd, tableName);
		}

		/// <summary>
		/// Query DataTable
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public DataTable Query(string sql)
		{
			return Query(sql, sql.GetHashCode().ToString());
		}


		/// </summary>
		/// Query DataTable
		/// 
		/// RomKuc 2021-01-25 do not close connection (it is not the same as "use specific transaction")
		/// <param name="sql"></param>
		/// <param name="noClose"></param>
		/// <returns></returns>
		public DataTable Query(string sql, bool noClose)
		{
			var vLoc = string.Format("{0}::{1}(sql=[{2}], noClose={3})",
				TYPE_NAME, nameof(Query), sql, noClose);

			IDbCommand cmd = CreateCommand();
			cmd.CommandText = sql;			
			DataTable retVal = new DataTable(sql.GetHashCode().ToString());
			IDataReader reader = null;
			try
			{
				Error();
				Open();
				if (!IsError)
				{
					Debug.Print("Sql:Query> " + cmd.CommandText);
					reader = cmd.ExecuteReader();
					retVal.Load(reader);
					return retVal;
				}
			}
			catch (Exception ex)
			{
				ReportError(vLoc, ex);
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}
				if (!noClose)
					Close();
			}
			return retVal;
		}


		/// <summary>
		/// Query DataSet
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public DataSet QueryDs(IDbCommand cmd)
		{
			var vLoc = string.Format("{0}::{1}(IDbCommand::CommandText=[{2}])",
				TYPE_NAME, nameof(QueryDs), cmd.CommandText);

			DataSet retVal = new DataSet(cmd.GetHashCode().ToString());
			IDataReader reader = null;

			try
			{
				Error();
				Open();
				if (!IsError)
				{
					Debug.Print("Sql:QueryDs> " + cmd.CommandText);
					reader = cmd.ExecuteReader();

					while (!reader.IsClosed)
					{
						retVal.Tables.Add().Load(reader);
					}
					return retVal;
				}

			}
			catch (Exception ex)
			{
				ReportError(vLoc, ex);
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}
				Close();
			}

			return retVal;
		}

		/// <summary>
		/// Query DataSet
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public DataSet QueryDs(string sql)
		{
			IDbCommand cmd = CreateCommand();
			cmd.CommandText = sql;
			return QueryDs(cmd);
		}


		#endregion

		#region QueryScalar

		public string QueryScalar(IDbCommand cmd)
		{
			return GetScalarValue(Query(cmd));
		}

		public string QueryScalar(string sql)
		{
			return GetScalarValue(Query(sql));
		}

		#endregion

		#region Execute

		public int Execute(IDbCommand cmd, IDbTransaction transaction = null)
		{
			var vLoc = string.Format("{0}::{1}(IDbCommand::CommandText=[{2}], transaction={3})", 
				TYPE_NAME, nameof(Execute), cmd.CommandText, transaction);
			int retVal = 0;

			try
			{
				Error();
				Open();
				if (!IsError)
				{
					Debug.Print("Sql:Query> " + cmd.CommandText);
					if(transaction != null)
					{
						cmd.Transaction = transaction;
					}
					retVal = cmd.ExecuteNonQuery();
				}

			}
			catch (Exception ex)
			{
				ReportError(vLoc, ex);
			}
			finally
			{
				if(transaction == null)
				{
					Close();
				}
			}

			return retVal;
		}

		public int Execute(string sql, IDbTransaction transaction = null)
		{
			Debug.Print("Sql:Execute> " + sql);
			IDbCommand cmd = CreateCommand();
			cmd.CommandText = sql;
			return Execute(cmd, transaction);
		}

		///RomKuc 2021-01-25 do not close connection (it is not the same as "use specific transaction")
		public int Execute(string sql, bool noClose, IDbTransaction transaction = null)
		{
			var vLoc = string.Format("{0}::{1}(sql=[{2}], noClose={3}, transaction={4}",
				TYPE_NAME, nameof(Execute), sql, noClose, transaction);

			Debug.Print("Sql:Execute> " + sql);
			IDbCommand cmd = CreateCommand();
			cmd.CommandText = sql;			
			int retVal = 0;
			try
			{
				Error();
				Open();
				if (!IsError)
				{
					Debug.Print("Sql:Query> " + cmd.CommandText);
					if (transaction != null)
					{
						cmd.Transaction = transaction;
					}
					retVal = cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				ReportError(vLoc, ex);
			}
			finally
			{
				if (transaction == null)
				{
					if (!noClose)
						Close();
				}
			}
			return retVal;
		}
		#endregion

		#region Transacions

		public IDbTransaction BeginTransaction ()
		{
			return Connection.BeginTransaction();
		}


		#endregion

		#endregion
	}

}
