using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
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
		/// Round 63: Birthmove (2,2), sacrifice = (7,1) and (10,9) (gain2 = 91): score = 100 %, moves = 18, win in 1, loose in 2147483647 - Used 97 ms, Timelimit 371 ms, Start 54.922, End 55.018
		/// </summary>
		[TestMethod]
		public void Replay_Test()
		{
			DoReplay("b29012e8-a732-4cce-ae10-bb1059e3b303"
				//, rounds: new[] { 99 }
				//, action: Replay_OwnKillMoves
				, bot: new MonteCarloBot(new TheConsole(), new RandomGenerator(new Random()))
				{
					Parameters = new MonteCarloParameters()
					{
						LogLevel = 1,
						//Debug = true,
						//MaxDuration = TimeSpan.FromSeconds(1),
						//MaxDuration = TimeSpan.FromDays(1)
					}
				});
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
				DoReplay(gameId, bot: new MonteCarloBot(new TheConsole(), new RandomGenerator(new Random()))
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
			BaseBot bot = null)
		{
			var filename = Path.Combine(Folder, gameId + ".txt");
			if (rounds == null)
			{
				rounds = Enumerable.Range(0, Board.Size).ToArray();
			}
			if (bot == null)
			{
				bot = new MonteCarloBot(new TheConsole(), new RandomGenerator(new Random()));
			}
			var lines = File.ReadAllLines(filename);
			DoReplayLines(lines, differenceOnly, action, rounds, bot);
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
	}
}
