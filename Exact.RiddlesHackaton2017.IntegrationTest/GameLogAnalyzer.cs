using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RiddlesHackaton2017.IntegrationTest
{
	[TestClass]
	public class GameLogAnalyzer
	{
		/// <summary>
		/// Find exceptions in log files
		/// </summary>
		[TestMethod]
		public void FindExceptions()
		{
			using (var database = new Database())
			{
				database.Connect();

				var games = database.GetMyGames();
				foreach (var game in games.Where(g => g.Version >= 50))
				{
					var log = game.Log;
					var lines = log.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					bool writeNextLine = false;
					foreach (var line in lines)
					{
						if (writeNextLine)
						{
							if (line.Contains("IO log")) break;
							//Console.WriteLine(line);
						}
						else if (line.Contains("Exception"))
						{
							Console.WriteLine($"Game {game.Id}, my version {game.Version}, {game.Opponent}, {game.WonString}, {game.Rounds} rounds");
							writeNextLine = true;
						}
					}
				}
			}
		}

		/// <summary>
		/// Analyzes which opponents steal my time
		/// </summary>
		[TestMethod]
		public void AnalyzeOpponentsStealingTime()
		{
			int maxRound = 1;

			using (var database = new Database())
			{
				database.Connect();
				var opponents = new Dictionary<string, Statistics>();
				var games = database.GetMyGames().Where(g => g.PlayedDate > new DateTime(2018, 4, 1, 14, 0, 0));
				foreach (var game in games)
				{
					var log = game.Log;
					var result = GetGameStatistics(log, maxRound);
					Statistics stats;
					if (opponents.ContainsKey(game.Opponent))
					{
						stats = opponents[game.Opponent];
					}
					else
					{
						stats = new Statistics();
						opponents.Add(game.Opponent, stats);
					}
					stats.GameCount++;
					stats.TotalRoundCount += result[0];
					stats.TotalMoveCount += result[1];
					stats.TotalUsedMs += result[2];
				}

				foreach (var de in opponents
					.Where(s => s.Value.GameCount >= 2 && s.Value.TotalUsedMs > 0)
					.OrderByDescending(s => s.Value.GameCount)
					.Take(15))
				{
					var opponent = de.Key;
					var stats = de.Value;
					Console.WriteLine($"{opponent}: Games: {stats.GameCount} - {1000 * stats.TotalMoveCount / stats.TotalUsedMs}");
				}
			}
		}

		private int[] GetGameStatistics(string log, int maxRound)
		{
			var lines = log.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			int rounds = 0;
			int movesCount = 0;
			int usedMs = 0;
			var pattern = @"Round\s(?<round>\d+).*\smoves\s=\s(?<moves>\d+).*\ssimulations\s=\s(?<simulations>\d+).*Used\s(?<used>\d+)";
			var regex = new Regex(pattern);
			foreach (var line in lines)
			{
				var match = regex.Match(line);
				if (match.Success)
				{
					rounds++;
					movesCount += int.Parse(match.Groups["moves"].Value) * int.Parse(match.Groups["simulations"].Value);
					usedMs += int.Parse(match.Groups["used"].Value);
					if (rounds >= maxRound)
					{
						return new int[] { rounds, movesCount, usedMs };
					}
				}
			}
			return new int[] { rounds, movesCount, usedMs };
		}

		private class Statistics
		{
			public string Opponent { get; set; }
			public int GameCount { get; set; }
			public int TotalRoundCount { get; set; }
			public int TotalMoveCount { get; set; }
			public int TotalUsedMs { get; set; }
		}
	}
}
