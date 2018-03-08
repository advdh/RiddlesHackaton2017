using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using System;
using System.IO;
using System.Linq;
using Troschuetz.Random.Generators;

namespace RiddlesHackaton2017.IntegrationTest
{
	[TestClass]
	public class Replayer
	{
		public static string Folder { get { return @"D:\Ad\Golad\Games"; } }

		/// <summary>
		/// Round 63: Birthmove (2,2), sacrifice = (7,1) and (10,9) (gain2 = 91): score = 100 %, moves = 18, win in 1, loose in 2147483647 - Used 97 ms, Timelimit 371 ms, Start 54.922, End 55.018
		/// </summary>
		[TestMethod]
		public void Replay_Test()
		{
			var parms = MonteCarloParameters.Life;
			parms.LogLevel = 0;
			parms.Debug = false;
			//parms.StartSimulationCount = 13;
			//parms.MinSimulationCount = 13;
			//parms.MaxSimulationCount = 13;
			//parms.MoveCount = 192;
			DoReplay("9b87a3e8-cafa-48cf-9380-3fd46bddc434"
				//, rounds: new[] { 1 }
				//, action: Replay_OwnKillMoves
				, bot: new Anila8Bot(new TheConsole())
				{
					Parameters = parms
				},
				source: LogSource.File);
		}

		[TestMethod]
		public void Test()
		{
			string[] linesData = File.ReadAllLines(@"c:\tmp\game.txt");
			string[] linesLog = File.ReadAllLines(@"D:\Ad\Golad\Games\a734cae9-d395-4a00-8563-8bbb946d1bf3.txt")
				.Where(l => l.StartsWith("update game field"))
				.Select(s => s.Substring(18))
				.ToArray();
			string boardStringFromData = linesData.First();
			string boardStringFromLog = linesLog.First();
			Assert.AreEqual(boardStringFromLog, boardStringFromData, "Start board");
			var boardFromData = new Board() { Field = BotParser.ParseBoard(boardStringFromData) };
			for(int i = 0; i < 100; i++)
			{
				var move = Move.Parse(linesData[2 * i + 1]);
				boardFromData = boardFromData.ApplyMoveAndNext(Player.Player1, move);
				move = Move.Parse(linesData[2 * i + 2]);
				boardFromData = boardFromData.ApplyMoveAndNext(Player.Player2, move);
				var boardFromLog = new Board() { Field = BotParser.ParseBoard(linesLog[i + 1]) };
				Assert.AreEqual(boardFromLog.HumanBoardString(), boardFromData.HumanBoardString(), @"Round {i}");
			}
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
			Action<Board> action = null,
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
			Action<Board> action, int[] rounds, BaseBot bot)
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
							if (action == null)
							{
								int timelimit = int.Parse(words[2]);
								newMove = Move.Parse(bot.GetMove(board, TimeSpan.FromMilliseconds(timelimit)));
							}
							else
							{
								action.Invoke(board);
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
