using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Output;
using System;
using System.IO;
using System.Linq;

namespace Exact.RiddlesHackaton2017.IntegrationTest
{
	[TestClass]
	public class Replayer
	{
		public static string Folder { get { return @"C:\tmp\RiddlesHackaton2017\Games"; } }

		[TestMethod]
		public void Replay_Test()
		{
			DoReplay("617e3420-1302-42e4-a34e-c362763e319f");
		}

		/// <summary>
		/// Replays the specified game
		/// </summary>
		/// <param name="gameId">Game ID</param>
		/// <param name="differenceOnly">Show differences with regards to original move only</param>
		/// <param name="action">Optional: action to execute on action command</param>
		private void DoReplay(string gameId, bool differenceOnly = true,
			Action<Board> action = null,
			int[] rounds = null)
		{
			var filename = System.IO.Path.Combine(Folder, gameId + ".txt");
			if (rounds == null)
			{
				rounds = Enumerable.Range(0, Board.Size2).ToArray();
			}
			var lines = File.ReadAllLines(filename);
			DoReplayLines(lines, differenceOnly, action, rounds);
		}

		private void DoReplayLines(string[] lines, bool differenceOnly, Action<Board> action, int[] rounds)
		{
			DoReplayLines(lines, differenceOnly, action, rounds, new TheConsole());
		}

		private void DoReplayLines(string[] lines, bool differenceOnly, Action<Board> action, int[] rounds, IConsole console)
		{
			var board = new Board();
			Player player = Player.Player1;
			var bot = new Bot(console);
			int round = 0;
			int originalMove;
			int newMove = -1;

			foreach (var command in lines)
			{
				string[] words = command.Split(' ');
				switch (words[0])
				{
					case "settings":
						ParseSettings(words, ref player);
						break;

					case "update":
						ParseBoard(words, player, ref board, ref round);
						break;

					case "action":
						if (rounds.Contains(round))
						{
							if (action == null)
							{
								int timelimit = int.Parse(words[2]);
								newMove = bot.Move(board, TimeSpan.FromMilliseconds(timelimit));
							}
							else
							{
								action.Invoke(board);
							}
						}
						break;

					case "Output":
						if (rounds.Contains(round))
						{
							string sMove = command.Split('"')[1];
							originalMove = ToMove(board.MyPosition, sMove);
							if (!differenceOnly || newMove != originalMove)
							{
								console.WriteLine("Round {0}: original move: {1}, new move: {2}",
									round, new Position(originalMove), new Position(newMove));
							}
						}
						break;
				}
			}
		}

		private int ToMove(int position, string sMove)
		{
			switch (sMove)
			{
				case "up":
					return position - Board.Size;
				case "down":
					return position + Board.Size;
				case "left":
					return position - 1;
				case "right":
					return position + 1;
			}

			//Example: invalid move
			return 0;
		}

		private static void ParseBoard(string[] words, Player player, ref Board board, ref int round)
		{
			switch (words[2])
			{
				case "field":
					board = BotParser.ParseBoard(words, player, round);
					break;
				case "round":
					round = int.Parse(words[3]);
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
