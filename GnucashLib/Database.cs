using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace GnucashLib
{
	public class Database
	{
		public static DbConnection FromSqlite(string databaseFile)
		{
			return new SqliteConnection($"Data Source={databaseFile}");
		}
	}
}
