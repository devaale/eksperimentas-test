using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Core.Metadata{
	public interface ISql : IErrorInfo
	{
		IDbConnection Connection { get; }

		bool Open();
		void Close();

		IDbCommand CreateCommand();
		void AddParameter(IDbCommand cmd, string name, object value, DbType type);
		void AddParameter(IDbCommand cmd, string name, object value, SqlDbType type);
		void AddParameter(IDbCommand cmd, string name, object value);

		DataTable Query(IDbCommand cmd, string tableName);
		DataTable Query(IDbCommand cmd);
		DataTable Query(string sql, string tableName);
		DataTable Query(string sql);
		DataSet QueryDs(IDbCommand cmd);
		DataSet QueryDs(string sql);
		string QueryScalar(string sql);
		string QueryScalar(IDbCommand cmd);

		int Execute(string sql, IDbTransaction transaction);
		int Execute(IDbCommand cmd, IDbTransaction transaction);

		IDbTransaction BeginTransaction();
	}
}
