using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RiddlesHackaton2017.IntegrationTest
{
	public class Database : IDisposable
	{
		protected string ServerName { get { return @"LT-14-106\SQL2016"; } }
		protected string DatabaseName { get { return "GameOfLifeAndDeath"; } }
		protected SqlConnection Connection;

		public void Connect()
		{
			Connection = new SqlConnection();
			Connection.ConnectionString = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True;",
				ServerName, DatabaseName);
			Connection.Open();
		}

		public string GetGameLog(string gameId)
		{
			string sql = "SELECT Log FROM Games WHERE Id = @gameId";
			using (var command = new SqlCommand(sql, Connection))
			{
				command.Parameters.AddWithValue("gameId", gameId);
				using (var reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						return reader.GetNullableString(0);
					}
				}
			}
			return null;
		}

		public IEnumerable<Anila8Game> GetMyGames(int top, string where, string orderBy)
		{
			string sql = $"SELECT TOP {top} {SelectAnila8} FROM Anila8Games WHERE {where} ORDER BY {orderBy}";
			return GetMyGames(sql);
		}

		public IEnumerable<Anila8Game> GetMyGames(string where, string orderBy)
		{
			string sql = $"SELECT {SelectAnila8} FROM Anila8Games WHERE {where} ORDER BY {orderBy}";
			return GetMyGames(sql);
		}

		public IEnumerable<Anila8Game> GetMyGames()
		{
			string sql = $"SELECT {SelectAnila8} FROM Anila8Games WHERE Log IS NOT NULL";
			return GetMyGames(sql);
		}

		private string SelectAnila8 { get { return " Id, Version, Opponent, Log, PlayedDate, Won, Rounds, GameData, Player "; } }

		private IEnumerable<Anila8Game> GetMyGames(string sql)
		{
			using (var command = new SqlCommand(sql, Connection))
			{
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						yield return new Anila8Game()
						{
							Id = reader.GetString(0),
							Version = reader.GetInt16(1),
							Opponent = reader.GetString(2),
							Log = reader.GetString(3),
							PlayedDate = reader.GetDateTime(4),
							Won = reader.GetNullableInt32(5),
							Rounds = reader.GetInt16(6),
							GameData = reader.GetString(7),
							Player = reader.GetInt32(8),
						};
					}
				}
			}
		}

		public IEnumerable<Game> GetGames(int top, string where, string orderBy)
		{
			string sql = $"SELECT TOP {top} {Select} FROM Games WHERE {where} ORDER BY {orderBy}";
			return GetGames(sql);
		}

		public IEnumerable<Game> GetGames(string where, string orderBy)
		{
			string sql = $"SELECT {Select} FROM Games WHERE {where} ORDER BY {orderBy}";
			return GetGames(sql);
		}

		private string Select { get { return " Id, PlayedDate, Rounds, Winner, Player0, Version0, Player1, Version1, GameData "; } }

		private IEnumerable<Game> GetGames(string sql)
		{
			using (var command = new SqlCommand(sql, Connection))
			{
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						yield return new Game()
						{
							Id = reader.GetString(0),
							PlayedDate = reader.GetDateTime(1),
							Rounds = reader.GetInt16(2),
							Winner = reader.GetNullableTinyint(3),
							Player0 = reader.GetString(4),
							Version0 = reader.GetInt16(5),
							Player1 = reader.GetString(6),
							Version1 = reader.GetInt16(7),
							GameData = reader.GetString(8),
						};
					}
				}
			}
		}

		public string GetGameData(string gameId)
		{
			string sql = "SELECT GameData FROM Games WHERE Id = @gameId";
			using (var command = new SqlCommand(sql, Connection))
			{
				command.Parameters.AddWithValue("gameId", gameId);
				using (var reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						return reader.GetNullableString(0);
					}
				}
			}
			return null;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					//Dispose managed state (managed objects).
					Connection.Dispose();
				}

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
