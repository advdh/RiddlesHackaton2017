using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;

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
			board.Player1FieldCount = board.CalculatedPlayer1FieldCount;
			board.Player2FieldCount = board.CalculatedPlayer2FieldCount;

			return board;
		}

		protected void AssertHumanBoardString(string expected, string actual)
		{
			Assert.AreEqual(expected.Trim(), actual.Trim());
		}
	}
}
