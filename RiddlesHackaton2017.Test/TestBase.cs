using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using System;
using System.IO;
using System.Linq;

namespace RiddlesHackaton2017.Test
{
	public class TestBase
	{
		protected static string StartBoardString = @".,.,.,1,.,0,.,.,.,.,.,0,.,1,.,.,1,.,.,.,.,.,.,.,0,.,.,1,1,.,.,.,1,1,.,.,0,.,.,.,0,.,.,0,1,0,1,.,.,.,1,.,.,.,1,.,.,.,.,1,.,0,.,.,1,.,0,.,1,.,1,1,.,1,.,1,.,.,0,1,.,0,.,0,.,0,.,.,.,1,1,.,.,.,.,0,.,.,1,.,.,.,.,.,0,.,0,1,0,.,.,.,.,.,.,1,.,1,0,.,1,0,.,.,.,.,.,0,.,.,.,.,.,.,1,.,.,.,.,.,1,.,0,1,0,1,.,0,.,.,.,.,.,0,.,.,.,.,.,.,1,.,.,.,.,.,1,0,.,1,0,.,0,.,.,.,.,.,.,1,0,1,.,1,.,.,.,.,.,0,.,.,1,.,.,.,.,0,0,.,.,.,1,.,1,.,1,.,0,1,.,.,0,.,0,.,0,0,.,0,.,1,.,0,.,.,1,.,0,.,.,.,.,0,.,.,.,0,.,.,.,0,1,0,1,.,.,1,.,.,.,1,.,.,0,0,.,.,.,0,0,.,.,1,.,.,.,.,.,.,.,0,.,.,0,.,1,.,.,.,.,.,1,.,0,.,.,.";
		protected static string HumanStartBoardString = @"...1.0.....0.1..1.
......0..11...11..
0...0..0101...1...
1....1.0..1.0.1.11
.1.1..01.0.0.0...1
1....0..1.....0.01
0......1.10.10....
.0......1.....1.01
01.0.....0......1.
....10.10.0......1
01.1.....0..1....0
0...1.1.1.01..0.0.
00.0.1.0..1.0....0
...0...0101..1...1
..00...00..1......
.0..0.1.....1.0...
";

		/// <summary>
		/// Board from game de4f1949-1c06-46b2-afb8-bfd25ff3fc40
		/// https://starapple.riddles.io/competitions/game-of-life-and-death/matches/de4f1949-1c06-46b2-afb8-bfd25ff3fc40
		/// </summary>
		protected static string ExampleBoardString = @".,.,.,1,.,1,.,.,.,.,.,0,.,0,1,1,.,1,.,.,1,1,.,.,.,.,.,0,1,.,1,.,.,.,0,.,0,.,.,.,1,.,0,.,1,.,0,.,.,.,.,1,.,.,.,.,1,.,0,.,1,.,0,.,0,.,0,.,.,.,1,.,0,.,.,.,1,.,1,.,0,0,.,0,.,1,.,1,.,.,.,.,0,0,.,.,.,.,.,.,.,0,1,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,1,.,.,1,.,.,1,.,.,.,.,.,.,1,1,0,.,.,1,1,.,1,.,1,.,.,0,.,0,.,0,0,.,.,1,0,0,.,.,.,.,.,.,0,.,.,0,.,.,0,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,0,1,.,.,.,.,.,.,.,1,1,.,.,.,.,0,.,0,.,1,.,1,1,.,0,.,0,.,.,.,1,.,0,.,.,.,1,.,1,.,1,.,0,.,1,.,0,.,.,.,.,0,.,.,.,.,1,.,0,.,1,.,0,.,.,.,1,.,1,.,.,.,0,.,0,1,.,.,.,.,.,0,0,.,.,0,.,0,0,1,.,1,.,.,.,.,.,0,.,0,.,.,.";

		protected static Board InitBoard()
		{
			return InitBoard(Player.Player1, StartBoardString);
		}


		/// <summary>
		/// Board from game de4f1949-1c06-46b2-afb8-bfd25ff3fc40
		/// https://starapple.riddles.io/competitions/game-of-life-and-death/matches/de4f1949-1c06-46b2-afb8-bfd25ff3fc40
		/// </summary>
		protected static Board ExampleBoard()
		{
			return InitBoard(Player.Player1, ExampleBoardString);
		}

		protected static Board InitBoard(string boardString)
		{
			return InitBoard(Player.Player1, boardString);
		}

		protected static Board InitBoard(int width, int height, string boardString)
		{
			return InitBoard(width, height, Player.Player1, boardString);
		}

		protected static Board InitBoard(Player player, string boardString)
		{
			return InitBoard(Board.Width, Board.Height, player, boardString);
		}

		protected static Board InitBoard(int width, int height, Player player, string boardString)
		{
			var board = new Board()
			{
				MyPlayer = player,
				Field = BotParser.ParseBoard(width, height, boardString),
			};
			board.UpdateFieldCounts();

			return board;
		}

		protected Board GetBoard(string gameId, int round)
		{
			var lines = GetLines(gameId);
			int ix = Enumerable.Range(0, lines.Count()).First(i => lines[i].StartsWith($"update game round {round}"));
			var line = lines[ix + 1];
			var board = new Board();
			ParseBoard(line.Split(' '), board);
			board.UpdateFieldCounts();
			return board;
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

		private static string Folder { get { return @"D:\Ad\Golad\Games"; } }

		private string[] GetLines(string gameId)
		{
			var filename = Path.Combine(Folder, gameId + ".txt");
			return File.ReadAllLines(filename);
		}

		protected void AssertHumanBoardString(string expected, string actual)
		{
			Assert.AreEqual(expected.Trim(), actual.Trim());
		}
	}
}
