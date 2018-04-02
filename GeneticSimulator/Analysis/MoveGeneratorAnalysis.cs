using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.IntegrationTest;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.MoveGeneration;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticSimulator.Analysis
{
	[TestClass]
	public class MoveGeneratorAnalysis
	{
		[TestMethod]
		public void AnalyzeMovesTop10()
		{
			using (var database = new Database())
			{
				database.Connect();
				var games = database.GetGames(@"GameData IS NOT NULL AND PlayedDate > '2018-03-24' 
					AND Player0 IN(SELECT BotName FROM LeaderBoard WHERE Rank <= 10) 
					AND Player1 IN(SELECT BotName FROM LeaderBoard WHERE Rank <= 10)");
				AnalyzeMoves(games.Take(10));
			}
		}

		[TestMethod]
		public void AnalyzeMoves_Test()
		{
			//Game between ol0 and Anila8
			var gameId = "84f2e021-78df-4c1b-80bc-ae62ca233ac3";

			using (var database = new Database())
			{
				database.Connect();
				var game = database.GetGameById(gameId);
				AnalyzeMoves(new[] { game }, verboseLogging: true);
			}
		}

		private void AnalyzeMoves(IEnumerable<Game> games, bool verboseLogging = false)
		{
			Board.InitializeFieldCountChanges();
			var results = new List<GameReplayResult>();
			foreach (var game in games.Take(100))
			{
				var rawLines = game.GameData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				var result = SimulateGame(game, rawLines, verboseLogging);
				results.Add(result);
				Console.WriteLine(result);
			}

			var aggregateResults = new List<GameReplayAggregateResult>();
			var distinctPlayers = results.Select(r => r.Game.Player0).Union(results.Select(r => r.Game.Player1)).Distinct().ToArray();
			foreach (var player in distinctPlayers)
			{
				var results0 = results.Where(r => r.Game.Player0 == player);
				var results1 = results.Where(r => r.Game.Player1 == player);
				var aggregateResult = new GameReplayAggregateResult()
				{
					Bot = player,
					TotalRounds = results0.Sum(r => r.Game.Rounds) + results1.Sum(r => r.Game.Rounds),
					TotalBestMove = results0.Sum(r => r.BestMove[1]) + results1.Sum(r => r.BestMove[2]),
					TotalEqualBestMove = results0.Sum(r => r.EqualBestMove[1]) + results1.Sum(r => r.EqualBestMove[2]),
					TotalBestMoveRankHighEnough = results0.Sum(r => r.BestMoveRankHighEnough[1]) + results1.Sum(r => r.BestMoveRankHighEnough[2]),
					TotalNotInMoveCandidates = results0.Sum(r => r.MoveNotInMoveCandidates[1]) + results1.Sum(r => r.MoveNotInMoveCandidates[2]),
				};
				Console.WriteLine(aggregateResult);
				aggregateResults.Add(aggregateResult);
			}
		}

		private GameReplayResult SimulateGame(Game game, string[] rawLines, bool verboseLogging = false)
		{
			var gameReplayResult = new GameReplayResult(game);

			var bot = new Anila8Bot(new NullConsole());
			var boardString = rawLines.First();
			var board = new Board() { Round = 1 };
			board.SetField(BotParser.ParseBoard(boardString));
			board.UpdateFieldCounts();
			int ix = 1;
			var player = Player.Player1;
			while (ix < rawLines.Length)
			{
				var line = rawLines[ix];
				if (line != "kill -1,-1")
				{
					//What whould anila8 do?
					bot.Board = board;
					bot.TimeLimit = TimeSpan.FromMilliseconds(TimeLimits[board.Round]);

					//New movegenerator
					var moveGenerator = new MoveGenerator2(board, MonteCarloParameters.Life);
					var candidateMoves = moveGenerator.GetMoves().ToArray();

					var move = Move.Parse(rawLines[ix]);

					var index = Enumerable.Range(0, candidateMoves.Length).FirstOrDefault(i => candidateMoves[i].Move.Equals(move));
					if (index == 0 && !candidateMoves[0].Move.Equals(move)) index = -1;

					if (index == -1)
					{
						gameReplayResult.MoveNotInMoveCandidates[board.MyPlayer.Value()]++;
					}

					//Player's move would not be in Anila8's move candidates
					//Calculate gain2 and add player's move to the candidate moves
					int gain2 = bot.GetMoveScore(move);
					int rank = candidateMoves.Count(m => m.Gain2 > gain2);

					bool simulatedCandidate = rank < MoveCounts[board.Round];
					if (simulatedCandidate)
					{
						gameReplayResult.BestMoveRankHighEnough[board.MyPlayer.Value()]++;
					}

					//Now simulate with the average number of moves for this round + the player's move, 
					//and with the average number of simulation counts for this round
					var list = new List<MoveScore>(candidateMoves.Take(MoveCounts[board.Round]));
					if (!list.Any(m => m.Equals(move)))
					{
						list.Add(new MoveScore(move, gain2));
					}
					candidateMoves = list
						.OrderByDescending(m => m.Gain2)
						.ToArray();
					var simulationResults = bot.SimulateMoves(candidateMoves, null, TimeSpan.FromSeconds(10), SimulationCounts[board.Round]);
					var simulationResult = simulationResults.FirstOrDefault(sr => sr.Move.Equals(move));
					var rank2 = simulationResults.Count(sr => sr.Score2 > simulationResult.Score2);

					if (rank2 == 0)
					{
						if (index >= 0 && simulatedCandidate)
						{
							//Player's move is also found by Anila8
							gameReplayResult.EqualBestMove[board.MyPlayer.Value()]++;
						}
						else
						{
							gameReplayResult.BestMove[board.MyPlayer.Value()]++;
						}
					}

					if (verboseLogging)
					{
						Console.WriteLine($"Round {board.Round}, player {board.MyPlayer}: move {move} found: {index >= 0} - gain2 = {gain2}, rank = {rank} (high enough = {simulatedCandidate}, movecount={MoveCounts[board.Round]}), rank2 = {rank2}");
					}

					board = board.ApplyMoveAndNext(move, validateMove: false);
					player = player.Opponent();
				}
				ix++;
			}
			return gameReplayResult;
		}

		private class GameReplayAggregateResult
		{
			public string Bot { get; set; }

			public int TotalRounds { get; set; }
			public int TotalNotInMoveCandidates { get; set; }
			public int TotalBestMove { get; set; }
			public int TotalEqualBestMove { get; set; }
			public int TotalBestMoveRankHighEnough { get; set; }

			public override string ToString()
			{
				return string.Format("{0}: rounds {1} - not in move candidates {2} ({3:P0}) - best move {4} ({5:P0}) - equal best move {6} ({7:P0}) - best move rank high enough {8} ({9:P0})",
					Bot,
					TotalRounds,
					TotalNotInMoveCandidates,
					(double)TotalNotInMoveCandidates / TotalRounds,
					TotalBestMove,
					(double)TotalBestMove / TotalRounds,
					TotalEqualBestMove,
					(double)TotalEqualBestMove / TotalRounds,
					TotalBestMoveRankHighEnough,
					(double)TotalBestMoveRankHighEnough / TotalRounds);
			}
		}

		private class GameReplayResult
		{
			public Game Game { get; private set; }

			/// <summary>For eaach player: in how many rounds was the played move in the move candiates of Anila8</summary>
			public int[] MoveNotInMoveCandidates { get; } = new int[3];

			/// <summary>For eaach player: in how many rounds was the played move better than all Anila8's move candidates</summary>
			public int[] BestMove { get; } = new int[3];

			/// <summary>For eaach player: in how many rounds was the played move the best move and this move was also found and simualted by Anila8</summary>
			public int[] EqualBestMove { get; } = new int[3];

			/// <summary>
			/// For eaach player: in how many rounds was the played move better than all Anila8's move candidates
			/// and was its rank high enough to be simulated
			/// </summary>
			public int[] BestMoveRankHighEnough { get; } = new int[3];

			public GameReplayResult(Game game)
			{
				Game = game;
			}

			public override string ToString()
			{
				var sb = new StringBuilder();
				sb.AppendLine($"Game {Game.Id} - {Game.Player0} ({Game.Version0}) - {Game.Player1} ({Game.Version1}) - {Game.Rounds} rounds");
				sb.AppendLine($"	Player1: moves not in move candidates: {MoveNotInMoveCandidates[1]}, is better move: {BestMove[1]}, rank high enough: {BestMoveRankHighEnough[1]}, best move found by Anila8: {EqualBestMove[1]}");
				sb.AppendLine($"	Player2: moves not in move candidates: {MoveNotInMoveCandidates[2]}, is better move: {BestMove[2]}, rank high enough: {BestMoveRankHighEnough[2]}, best move found by Anila8: {EqualBestMove[2]}");
				return sb.ToString();
			}
		}

		/// <summary>
		/// Get average move counts and simulation counts per round, from the logs
		/// </summary>
		[TestMethod]
		public void GetMoveCountsPerRound()
		{
			int myVersion = 52;

			using (var database = new Database())
			{
				database.Connect();

				//totalUsedTime[i] = total used time in round i, added for all games
				const int size = 101;
				TimeSpan[] totalUsedTime = new TimeSpan[size];
				int[] totalRounds = new int[size];
				int[] totalMoveCount = new int[size];
				int[] totalSimulationCount = new int[size];
				TimeSpan[] totalTimeLimits = new TimeSpan[size];

				var games = database.GetMyGames($"Log IS NOT NULL AND Version = {myVersion}");
				foreach (var game in games)
				{
					var rounds = ParseLogRounds(game.Log);
					foreach (var round in rounds.Where(r => !r.DirectWinMove))
					{
						totalRounds[round.Round]++;
						totalUsedTime[round.Round] += round.UsedTime;
						totalMoveCount[round.Round] += round.MoveCount;
						totalSimulationCount[round.Round] += round.SimulationCount;
						totalTimeLimits[round.Round] += round.TimeLimit;
					}
				}

				Console.WriteLine("Round\tUsedMs\tMoveCount\tSimulationCount\tTimeLimit");

				var sbDurations = new StringBuilder("private int[] Durations = new int[] {0, ");
				var sbMoveCounts = new StringBuilder("private int[] MoveCounts = new int[] {0, ");
				var sbSimulationCounts = new StringBuilder("private int[] SimulationCounts = new int[] {0, ");
				var sbTimeLimits = new StringBuilder("private int[] TimeLimits = new int[] {0, ");

				for (int i = 1; i <= 100; i++)
				{
					//Write tab-separated output
					Console.WriteLine("{0}\t{1:0}\t{2:0}\t{3:0}",
						i,
						totalUsedTime[i].TotalMilliseconds / totalRounds[i],
						totalMoveCount[i] / totalRounds[i],
						totalSimulationCount[i] / totalRounds[i]);

					sbDurations.AppendFormat("{0:0}, ", totalUsedTime[i].TotalMilliseconds / totalRounds[i]);
					sbMoveCounts.AppendFormat("{0}, ", totalMoveCount[i] / totalRounds[i]);
					sbSimulationCounts.AppendFormat("{0}, ", totalSimulationCount[i] / totalRounds[i]);
					sbTimeLimits.AppendFormat("{0:0}, ", totalTimeLimits[i].TotalMilliseconds / totalRounds[i]);
				}

				sbDurations.AppendLine("};");
				sbMoveCounts.AppendLine("};");
				sbSimulationCounts.AppendLine("};");
				sbTimeLimits.AppendLine("};");

				Console.WriteLine(sbDurations);
				Console.WriteLine(sbMoveCounts);
				Console.WriteLine(sbSimulationCounts);
				Console.WriteLine(sbTimeLimits);
			}
		}

		private IEnumerable<LogRound> ParseLogRounds(string log)
		{
			var roundLines = log
				.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
				.Where(line => line.StartsWith("Round "));
			return roundLines.Select(line => LogRound.ParseV52(line));
		}

		private int[] Durations = new int[] { 0, 736, 806, 805, 783, 714, 649, 604, 557, 510, 469, 431, 397, 368, 340, 314, 294, 273, 256, 240, 224, 212, 200, 191, 181, 172, 165, 159, 152, 147, 142, 138, 133, 130, 126, 124, 121, 118, 116, 115, 113, 111, 110, 108, 107, 106, 106, 103, 103, 103, 102, 101, 102, 101, 101, 100, 101, 99, 99, 99, 99, 99, 99, 98, 98, 99, 98, 98, 98, 99, 98, 98, 98, 98, 98, 97, 97, 97, 97, 96, 97, 97, 98, 97, 97, 97, 97, 98, 97, 97, 97, 97, 97, 97, 97, 97, 97, 99, 96, 86, 61, };

		private int[] MoveCounts = new int[] { 0, 85, 65, 64, 62, 59, 52, 47, 45, 41, 38, 37, 38, 36, 34, 34, 32, 31, 30, 29, 28, 26, 27, 25, 24, 24, 23, 22, 21, 21, 19, 20, 20, 18, 20, 19, 19, 19, 19, 18, 18, 19, 18, 18, 18, 18, 18, 19, 18, 18, 17, 18, 17, 16, 17, 17, 17, 17, 17, 17, 16, 17, 17, 17, 17, 16, 16, 17, 16, 16, 16, 16, 16, 16, 16, 16, 16, 17, 17, 16, 16, 17, 16, 16, 15, 16, 16, 16, 15, 16, 16, 16, 16, 16, 16, 16, 16, 18, 28, 60, 93, };

		private int[] SimulationCounts = new int[] { 0, 25, 45, 45, 46, 45, 45, 44, 43, 41, 39, 37, 36, 34, 33, 32, 31, 29, 28, 27, 27, 26, 25, 24, 24, 23, 22, 22, 21, 21, 21, 20, 20, 20, 19, 19, 19, 19, 19, 18, 18, 18, 18, 18, 18, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, };

		private int[] TimeLimits = new int[] { 0, 10000, 9310, 8601, 7892, 7194, 6570, 6004, 5494, 5034, 4622, 4249, 3916, 3616, 3345, 3102, 2883, 2687, 2511, 2353, 2211, 2083, 1969, 1866, 1772, 1689, 1613, 1545, 1484, 1429, 1380, 1335, 1296, 1260, 1227, 1199, 1173, 1150, 1129, 1110, 1093, 1076, 1063, 1050, 1039, 1030, 1022, 1012, 1006, 1000, 993, 988, 983, 979, 975, 972, 969, 966, 962, 961, 958, 958, 955, 953, 952, 950, 949, 948, 948, 946, 945, 945, 945, 945, 943, 942, 943, 943, 942, 942, 941, 942, 939, 939, 938, 938, 938, 938, 937, 937, 938, 938, 938, 938, 937, 938, 938, 938, 936, 937, 948, };


	}
}
