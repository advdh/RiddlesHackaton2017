using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
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
		public void Replay_Test()
		{
			DoReplay("1a08e4e3-e2ec-4089-81de-b5d62ad61fff"
				, rounds: new[] { 22 }
				//, action: Replay_OwnKillMoves
				);
		}

		/// <summary>
		/// Shows direct impact of all own kill moves
		/// </summary>
		/// <param name="board"></param>
		void Replay_OwnKillMoves(Board board)
		{
			Console.WriteLine("Round {0}", board.Round);
			var mine = board.GetCells(Player.Player1);
			foreach(int i in mine)
			{
				var killMove = new KillMove(i);
				//var newBoard = Board.CopyAndPlay(board, Player.Player1, killMove);
				Console.WriteLine("{0}: direct impact = {1}", new Position(i), killMove.DirectImpactForBoard(board));
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
			int round = 0;
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
						ParseBoard(words, player, ref board, ref round);
						break;

					case "action":
						if (rounds.Contains(round))
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
						if (rounds.Contains(round))
						{
							string sMove = command.Split('"')[1];
							originalMove = Move.Parse(sMove);
							if (!differenceOnly || !newMove.Equals(originalMove))
							{
								Console.Error.WriteLine("Round {0}: original move: {1}, new move: {2}",
									round, originalMove, newMove);
							}
						}
						break;
				}
			}
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
