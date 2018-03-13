using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017;
using RiddlesHackaton2017.IntegrationTest;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticSimulator.Analysis
{
	[TestClass]
	public class OpponentAnalysis
	{
		/// <summary>
		/// Generates a matrix of opponents x opponents with average scores, including draw scores
		/// </summary>
		[TestMethod]
		public void AggregateScores_All()
		{
			var games = GetAllGames(new DateTime(2018, 2, 15), 5);

			var players = games.Select(g => g.Player0).Distinct().ToList();

			var statistics = new Dictionary<Game, double>();
			foreach (var game in games)
			{
				double score = 0;
				if (game.Winner.HasValue)
				{
					score = game.Winner.Value == 1 ? 1.0 : -1.0;
				}
				else
				{
					var scores = CalculateScore(game.GameData);
					score = 2.0 * scores[0] / (scores[0] + scores[1]) - 1.0;
				}
				statistics.Add(game, score);
			}

			//Write players row
			var sb = new StringBuilder();
			sb.Append("Player\t");
			foreach (var player in players)
			{
				sb.Append(player);
				sb.Append('\t');
			}
			sb.AppendLine();

			//Write on each line the player with average score against the other player in separate cell
			foreach (var player0 in players)
			{
				sb.Append(player0);
				sb.Append('\t');
				foreach (var player1 in players)
				{
					var relevantStatistics = statistics.Where(s =>
						(s.Key.Player0 == player0 && s.Key.Player1 == player1)
						|| (s.Key.Player1 == player0 && s.Key.Player0 == player1));
					if (relevantStatistics.Any())
					{
						double avgScore = relevantStatistics.Average(r => r.Value);
						sb.Append($"{avgScore:0.000}");
					}
					sb.Append('\t');
				}
				sb.AppendLine();
			}
			Console.WriteLine(sb);
		}

		/// <summary>
		/// Generates a matrix of average scores by opponents by my versions
		/// </summary>
		[TestMethod]
		public void AggregateScores_Anila8()
		{
			var games = GetMyGames(new DateTime(2018, 2, 15));

			var opponents = games.Select(g => g.Opponent).Distinct().ToList();
			var myVersions = games.Select(g => g.Version).Distinct().ToList();

			var statistics = new Dictionary<Anila8Game, double>();
			foreach (var game in games)
			{
				double score = 0;
				if (game.Won.HasValue)
				{
					score = game.Won.Value == 1 ? 1.0 : -1.0;
				}
				else
				{
					var scores = CalculateScore(game.GameData);
					score = (game.Player == 0 ? 1 : -1) * (2.0 * scores[0] / (scores[0] + scores[1]) - 1.0);
				}
				statistics.Add(game, score);
			}

			//Write versions
			var sb = new StringBuilder();
			sb.Append("Version\t");
			foreach (var myVersion in myVersions)
			{
				sb.Append(myVersion);
				sb.Append('\t');
			}
			sb.AppendLine();

			//Write on each line the opponent with average score per version in separate cell
			foreach (var opponent in opponents)
			{
				sb.Append(opponent);
				sb.Append('\t');
				foreach (var myVersion in myVersions)
				{
					var relevantStatistics = statistics.Where(s => s.Key.Opponent == opponent && s.Key.Version == myVersion);
					if (relevantStatistics.Any())
					{
						double avgDrawScore = relevantStatistics.Average(r => r.Value);
						sb.Append($"{avgDrawScore:0.000}");
					}
					sb.Append('\t');
				}
				sb.AppendLine();
			}
			Console.WriteLine(sb);
		}

		[TestMethod]
		public void CalculateDrawScores_Test()
		{
			var games = GetMyGames(new DateTime(2018, 3, 10));

			var statistics = new Dictionary<Anila8Game, double>();
			foreach (var game in games)
			{
				var scores = CalculateScore(game.GameData);
				double drawScore = (game.Player == 0 ? 1 : -1) * (2.0 * scores[0] / (scores[0] + scores[1]) - 1.0);
				statistics.Add(game, drawScore);
			}

			foreach (var statistic in statistics.OrderBy(kvp => kvp.Value))
			{
				var game = statistic.Key;
				var drawScore = statistic.Value;
				Console.WriteLine($"{game.Id} - {game.Opponent} - {game.PlayedDate} (v{game.Version}) - drawscore {drawScore:0.000}");
			}
		}

		private int[] CalculateScore(string gameData)
		{
			var result = new int[2];
			var board = new Board();
			var rawLines = gameData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			var boardString = rawLines.First();
			board.SetField(BotParser.ParseBoard(boardString));
			board.UpdateFieldCounts();
			int ix = 1;
			var player = Player.Player1;
			while (ix < rawLines.Length)
			{
				var line = rawLines[ix];
				if (line != "kill -1,-1")
				{
					var move = Move.Parse(rawLines[ix]);
					board = board.ApplyMoveAndNext(player, move, validateMove: false);
					result[0] += board.Player1FieldCount;
					result[1] += board.Player2FieldCount;
					player = player.Opponent();
				}
				ix++;
			}
			return result;
		}

		/// <summary>
		/// Returns top count games that player0 (red) won, ordered by played date desc
		/// </summary>
		private IEnumerable<Anila8Game> GetMyGames(DateTime minDate)
		{
			string where = $"PlayedDate >= '{minDate:yyyy-MM-dd}' AND Opponent IN (SELECT TOP 15 BotName FROM LeaderBoard ORDER BY Rank)";
			string orderBy = "(SELECT Rank FROM LeaderBoard WHERE BotName = Opponent)";

			using (var database = new Database())
			{
				database.Connect();
				return database.GetMyGames(where, orderBy).ToList();
			}
		}

		/// <summary>
		/// Returns top count games that player0 (red) won, ordered by played date desc
		/// </summary>
		private IEnumerable<Game> GetAllGames(DateTime minDate, int top)
		{
			string where = $"PlayedDate >= '{minDate:yyyy-MM-dd}' " +
				$"AND Player0 IN (SELECT TOP {top} BotName FROM LeaderBoard ORDER BY Rank) " +
				$"AND Player1 IN (SELECT TOP {top} BotName FROM LeaderBoard ORDER BY Rank)";
			string orderBy = "(SELECT Rank FROM LeaderBoard WHERE BotName = Player0)";

			using (var database = new Database())
			{
				database.Connect();
				return database.GetGames(where, orderBy).ToList();
			}
		}
	}
}
