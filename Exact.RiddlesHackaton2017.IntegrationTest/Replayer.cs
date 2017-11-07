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

		[TestMethod]
		public void Test()
		{
			var board = new Board();
			ParseBoard(new[] { "update", "game", "field", "1,.,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,0,1,.,.,0,.,.,1,.,.,1,.,.,.,.,.,0,.,0,.,.,1,.,0,1,.,.,0,.,.,.,.,.,.,.,.,1,1,1,.,.,0,1,1,.,0,1,0,0,0,.,.,.,.,.,.,1,0,0,1,1,.,.,.,.,.,.,.,.,1,.,.,.,0,.,.,.,.,1,1,0,.,.,.,0,.,0,.,0,.,0,.,.,.,1,.,.,.,0,1,.,0,1,.,.,0,1,.,.,.,.,1,.,.,0,.,.,.,.,1,.,.,1,.,.,0,.,.,0,.,.,.,.,1,.,.,0,.,.,.,.,0,1,.,.,0,1,.,0,1,.,.,.,0,.,.,.,1,.,1,.,1,.,1,.,.,.,1,0,0,.,.,.,.,1,.,.,.,0,.,.,.,.,.,.,.,.,0,0,1,1,0,.,.,.,.,.,.,1,1,1,0,1,.,0,0,1,.,.,0,0,0,.,.,.,.,.,.,.,.,1,.,.,0,1,.,0,.,.,1,.,1,.,.,.,.,.,0,.,.,0,.,.,1,.,.,0,1,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,.,0" }, board);
			var move = new BirthMove(new Position(12, 10), new Position(13, 11), new Position(9, 13));
			var newBoard = board.ApplyMoveAndNext(Player.Player1, move);
			var nextNextBoard = newBoard.NextGeneration;
			Console.WriteLine(BoardEvaluator.Evaluate(board));
			Console.WriteLine(BoardEvaluator.Evaluate(nextNextBoard));
		}

		[TestMethod]
		public void Replay_Test()
		{
			DoReplay("988fe0cd-2193-4ab6-8f80-ebe6b69bffad"
				//, rounds: new[] { 25 }
				//, action: Replay_OwnKillMoves
				//, parameters: new MonteCarloParameters() { Debug = true, MaxDuration = TimeSpan.FromDays(1) }
				//, parameters: new MonteCarloParameters() { LogAllMoves = true }
				);
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
				DoReplay(gameId, parameters: new MonteCarloParameters()
				{
					MaxDuration = maxDuration
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
			MonteCarloParameters parameters = null)
		{
			var filename = Path.Combine(Folder, gameId + ".txt");
			if (rounds == null)
			{
				rounds = Enumerable.Range(0, Board.Size).ToArray();
			}
			if (parameters == null)
			{
				parameters = MonteCarloParameters.Default;
			}
			var lines = File.ReadAllLines(filename);
			DoReplayLines(lines, differenceOnly, action, rounds, new TheConsole(), parameters);
		}

		private void DoReplayLines(string[] lines, bool differenceOnly, 
			Action<Board> action, int[] rounds, IConsole console, MonteCarloParameters parameters)
		{
			var board = new Board();
			Player player = Player.Player1;
			var bot = new MonteCarloBot(console, new RandomGenerator(new Random()))
			{
				Parameters = parameters
			};
			Move originalMove;
			Move newMove = new NullMove();

			foreach (var command in lines)
			{
				string[] words = command.Split(' ');
				switch (words[0])
				{
					case "settings":
						ParseSettings(words, ref player);
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
					board.Field = BotParser.ParseBoard(words[3]);
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

		private static void ParseSettings(string[] words, ref Player player)
		{
			switch (words[1])
			{
				case "your_botid":
					int value = int.Parse(words[2]);
					player = (Player)Enum.Parse(player.GetType(), (value + 1).ToString());
					break;
			}
		}
	}
}
