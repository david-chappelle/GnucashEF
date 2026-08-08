using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace GncEF
{
	public class Database
	{
		public static DbConnection FromSqlite(string databaseFile)
		{
			return new SqliteConnection($"Data Source={databaseFile}");
		}
	}
}
