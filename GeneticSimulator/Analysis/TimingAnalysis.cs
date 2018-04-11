using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.IntegrationTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticSimulator.Analysis
{
	[TestClass]
	public class TimingAnalysis
	{
		/// <summary>
		/// Aggregates opponent timings: averages per opponent per round
		/// </summary>
		[TestMethod]
		public void AnalyzeOpponentTimings()
		{
			using (var database = new Database())
			{
				database.Connect();

				var games = database.GetMyGames(@"Log IS NOT NULL AND Log != '' AND Version = 54
					AND Opponent IN(SELECT BotName FROM LeaderBoard WHERE Rank <= 10 OR BotName = 'Wapper')");
				AnalyzeTimings(games);
			}
		}

		private void AnalyzeTimings(IEnumerable<Anila8Game> games)
		{
			const string myself = "Anila8";
			var opponents = new Dictionary<string, OpponentRound[]>();

			foreach (var game in games)
			{
				var rounds = ParseLogRounds(game.Log, 54);
				if (rounds.Any())
				{
					//Find myself
					OpponentRound[] myList = null;
					if (!opponents.ContainsKey(myself))
					{
						myList = new OpponentRound[101];
						opponents.Add(myself, myList);
					}
					else
					{
						myList = opponents[myself];
					}

					//Find opponent
					OpponentRound[] list = null;
					if (!opponents.ContainsKey(game.Opponent))
					{
						list = new OpponentRound[101];
						opponents.Add(game.Opponent, list);
					}
					else
					{
						list = opponents[game.Opponent];
					}

					LogRound previousRound = null;
					foreach (var round in rounds)
					{
						//Own timings
						int ms = (int)(round.EndTime - round.StartTime).TotalMilliseconds;
						if (ms < 0) ms += (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
						if (myList[round.Round] == null) myList[round.Round] = new OpponentRound();
						myList[round.Round].Count++;
						myList[round.Round].TotalMs += ms;

						if (previousRound != null)
						{
							//Opponent timings
							ms = (int)(round.StartTime - previousRound.EndTime).TotalMilliseconds;
							if (ms < 0) ms += (int)TimeSpan.FromMinutes(1).TotalMilliseconds;

							if (list[round.Round] == null) list[round.Round] = new OpponentRound();
							list[round.Round].Count++;
							list[round.Round].TotalMs += ms;
						}
						previousRound = round;
					}
				}
			}

			//Header
			Console.WriteLine();
			foreach (var opponent in opponents.OrderByDescending(o => o.Value[2].Count))
			{
				Console.Write($"\t{opponent.Key} ({opponent.Value[2].Count})");
			}
			Console.WriteLine();

			//Rounds
			for (int round = 2; round <= 100; round++)
			{
				Console.Write(round);
				foreach (var opponent in opponents.OrderByDescending(o => o.Value[2].Count))
				{
					Console.Write($"\t");
					if (opponent.Value[round] != null)
					{
						Console.Write($"{opponent.Value[round].AvgMs}");
					}
				}
				Console.WriteLine();
			}
		}

		private class OpponentRound
		{
			public int Count { get; set; }
			public int TotalMs { get; set; }

			public int AvgMs { get { return TotalMs / Count; } }
		}

		public IEnumerable<LogRound> ParseLogRounds(string log, int version)
		{
			var roundLines = log
				.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
				.Where(line => line.StartsWith("Round "));
			return roundLines.Select(line => LogRound.Parse(line, version));
		}


	}
}
