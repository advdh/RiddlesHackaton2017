using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.MonteCarlo;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using System;
using System.IO;
using System.Linq;

namespace RiddlesHackaton2017.IntegrationTest
{
	[TestClass]
	public class Replayer
	{
		public static string Folder { get { return @"D:\Ad\Golad\Games"; } }

		/// <summary>
		/// Should play kill moves in last 4 rounds
		/// </summary>
		[TestMethod]
		public void Regression_KillMovesToWin()
		{
			string gameId = "edb3825c-6629-4c2b-90cb-7c1e0aa71de9"; // Hakamo_bot

			var parms = MonteCarloParameters.Life;
			parms.Debug = false;

			DoReplay(gameId, differenceOnly: false
				, rounds: new[] { 63, 64, 65, 66 }
				, action: AssertKillMove
				, bot: new Anila8Bot(new TheConsole()) { Parameters = parms }
				, source: LogSource.File);
			}

		private void AssertKillMove(Board board, BaseBot bot, TimeSpan timelimit)
		{
			var moveString = bot.GetMove(board, timelimit);
			var move = Move.Parse(moveString);
			Assert.IsInstanceOfType(move, typeof(KillMove), $"Round {board.Round}: {move}");
		}

		[TestMethod]
		public void Replay_Test()
		{
			string gameId = "edb3825c-6629-4c2b-90cb-7c1e0aa71de9";	// "3c3dbc15-c316-434e-886b-fbad287e6d10";     //Player1, UnManagedCode
																		//string gameId = "59f191a9-33d3-4f12-a38b-5a42346ba4c8";		//Player2
			var parms = MonteCarloParameters.Life;

			parms.Debug = false;
			parms.LogLevel = 0;
			parms.UseFastAndSmartMoveSimulator = false;

			DoReplay(gameId, differenceOnly: false
				//, rounds: new[] { 65 }
				//, action: Replay_OwnKillMoves
				, bot: new Anila8Bot(new TheConsole())
				{
					Parameters = parms
				},
				source: LogSource.File);
		}

		[TestMethod]
		public void Replay_Test_Shazbot()
		{
			string gameId = "9d21efa0-453b-4951-87e5-0c0ed56b1248";
			var parms = MonteCarloParameters.Life;
			parms.ParallelSimulation = false;
			parms.Debug = true;
			parms.LogLevel = 2;
			parms.MaxDuration = TimeSpan.FromDays(1);
			parms.MoveCount = 1000;
			parms.MaxRelativeDuration = 1.0;
			parms.CellCountWeight = 1;
			parms.WinBonusWeight = 1;
			parms.SmartMoveGenerationCount = 4;
			DoReplay(gameId, differenceOnly: false
				, rounds: new[] { 91 }
				, bot: new Anila8Bot(new TheConsole())
				{
					Parameters = parms
				},
				source: LogSource.File);
		}

		[TestMethod]
		public void FastAndSmartMoveSimulator_Test()
		{
			var board = GetBoard("edb3825c-6629-4c2b-90cb-7c1e0aa71de9", 1);
			var parameters = MonteCarloParameters.Life;
			parameters.KillMovePercentage = 100;
			parameters.PassMovePercentage = 0;
			var simulator = new FastAndSmartMoveSimulator(new RandomGenerator(new Random()), parameters);
			//var move = simulator.GetRandomKillMove(board, Player.Player1);
			//var move = simulator.GetRandomBirthMove(board, Player.Player1);
			var move = simulator.GetRandomMove(board, Player.Player1);
		}

		private Board GetBoard(string gameId, int round)
		{
			var lines = GetLines(gameId, LogSource.File);
			int ix = Enumerable.Range(0, lines.Count()).First(i => lines[i].StartsWith($"update game round {round}"));
			var line = lines[ix + 1];
			var board = new Board();
			ParseBoard(line.Split(' '), board);
			board.UpdateFieldCounts();
			return board;
		}

		[TestMethod]
		public void Test()
		{
			var board = GetBoard("3c3dbc15-c316-434e-886b-fbad287e6d10", 100);
			board.UpdateFieldCounts();
			Assert.AreEqual(4, board.MyPlayerFieldCount);
			Assert.AreEqual(66, board.OpponentPlayerFieldCount);

			var move = new BirthMove(new Position(4, 5).Index, new Position(4, 2).Index, new Position(7, 4).Index);
			//var move = new KillMove(new Position(3,9).Index);
			var newBoard = board.ApplyMoveAndNext(Player.Player1, move, validateMove: true);

			Assert.AreEqual(4, newBoard.MyPlayerFieldCount);
		}

		/// <summary>
		/// Quick scan for exceptions of a big number of logged files
		/// </summary>
		[TestMethod]
		public void QuickExceptionScan()
		{
			DoQuickExceptionScan(maxCount: 100, maxDuration: TimeSpan.FromMilliseconds(50));
		}

		private void DoQuickExceptionScan(int maxCount, TimeSpan maxDuration)
		{
			foreach (var filename in Directory.GetFiles(Folder)
				.OrderByDescending(file => File.GetLastWriteTime(file))
				.Take(maxCount))
			{
				string gameId = Path.GetFileNameWithoutExtension(filename);
				Console.WriteLine(gameId);
				DoReplay(gameId, bot: new Anila8Bot(new TheConsole())
				{
					Parameters = new MonteCarloParameters() { MaxDuration = maxDuration }
				});
			}
		}

		/// <summary>
		/// Replays the specified game
		/// </summary>
		/// <param name="gameId">Game ID</param>
		/// <param name="differenceOnly">Show differences with regards to original move only</param>
		/// <param name="action">Optional: action to execute on action command</param>
		private void DoReplay(string gameId, bool differenceOnly = true,
			Action<Board, BaseBot, TimeSpan> action = null,
			int[] rounds = null,
			BaseBot bot = null,
			LogSource source = LogSource.File)
		{
			if (rounds == null)
			{
				rounds = Enumerable.Range(0, Board.Size).ToArray();
			}
			if (bot == null)
			{
				bot = new Anila8Bot(new TheConsole());
			}
			var lines = GetLines(gameId, source);
			DoReplayLines(lines, differenceOnly, action, rounds, bot);
		}

		private string[] GetLines(string gameId, LogSource logSource)
		{
			switch(logSource)
			{
				case LogSource.DatabaseLog:
					using (var database = new Database())
					{
						database.Connect();
						return database.GetGameLog(gameId).Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					}
				case LogSource.GameDataPlayer1:
					using (var database = new Database())
					{
						database.Connect();
						return GameLogGenerator.GenerateCommandLines(database.GetGameData(gameId), Player.Player1).ToArray();
					}
				case LogSource.GameDataPlayer2:
					using (var database = new Database())
					{
						database.Connect();
						return GameLogGenerator.GenerateCommandLines(database.GetGameData(gameId), Player.Player2).ToArray();
					}
				default:
					var filename = Path.Combine(Folder, gameId + ".txt");
					return File.ReadAllLines(filename);
			}
		}

		private void DoReplayLines(string[] lines, bool differenceOnly, 
			Action<Board, BaseBot, TimeSpan> action, int[] rounds, BaseBot bot)
		{
			var board = new Board();
			Move originalMove;
			Move newMove = new NullMove();

			foreach (var command in lines)
			{
				string[] words = command.Split(' ');
				switch (words[0])
				{
					case "settings":
						ParseSettings(words, board);
						break;

					case "update":
						ParseBoard(words, board);
						break;

					case "action":
						if (rounds.Contains(board.Round))
						{
							var timelimit = TimeSpan.FromMilliseconds(int.Parse(words[2]));
							if (action == null)
							{
								newMove = Move.Parse(bot.GetMove(board, timelimit));
							}
							else
							{
								action.Invoke(board, bot, timelimit);
							}
						}
						break;

					case "Output":
						if (rounds.Contains(board.Round))
						{
							string sMove = command.Split('"')[1];
							originalMove = Move.Parse(sMove);
							if (!differenceOnly || !newMove.Equals(originalMove))
							{
								Console.Error.WriteLine($"Round {board.Round}: original move: {originalMove}, new move: {newMove}");
							}
						}
						break;
				}
			}
		}

		private static void ParseBoard(string[] words, Board board)
		{
			switch (words[2])
			{
				case "field":
					board.SetField(BotParser.ParseBoard(words[3]));
					break;
				case "round":
					board.Round = int.Parse(words[3]);
					break;
				case "living_cells":
					if (words[1] == "player0")
					{
						board.Player1FieldCount = int.Parse(words[3]);
					}
					else
					{
						board.Player2FieldCount = int.Parse(words[3]);
					}
					break;
			}
		}

		private static void ParseSettings(string[] words, Board board)
		{
			switch (words[1])
			{
				case "your_botid":
					int value = int.Parse(words[2]);
					board.MyPlayer = (Player)Enum.Parse(typeof(Player), (value + 1).ToString());
					break;
			}
		}

		private enum LogSource
		{
			File,
			DatabaseLog,
			GameDataPlayer1,
			GameDataPlayer2,
		}
	}
}
