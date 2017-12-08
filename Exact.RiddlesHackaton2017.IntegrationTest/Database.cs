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

		public class Game
		{
			public string Id { get; set; }
			public int Version { get; set; }
			public string Opponent { get; set; }
			public string Log { get; set; }
		}

		public IEnumerable<Game> GetMyGames()
		{
			string sql = "SELECT Id, Version, Opponent, Log FROM Anila8Games WHERE Log IS NOT NULL";
			using (var command = new SqlCommand(sql, Connection))
			{
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						yield return new Game()
						{
							Id  = reader.GetString(0),
							Version = reader.GetInt16(1),
							Opponent = reader.GetString(2),
							Log = reader.GetString(3),
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
