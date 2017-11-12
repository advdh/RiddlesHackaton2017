using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;

namespace RiddlesHackaton2017.Test
{
	[TestClass]
	public class BotParserTest
	{
		string BoardString = ".,.,.,1,.,0,.,.,.,.,.,0,.,1,.,.,1,.,.,.,.,.,.,.,0,.,.,1,1,.,.,.,1,1,.,.,0,.,.,.,0,.,.,0,1,0,1,.,.,.,1,.,.,.,1,.,.,.,.,1,.,0,.,.,1,.,0,.,1,.,1,1,.,1,.,1,.,.,0,1,.,0,.,0,.,0,.,.,.,1,1,.,.,.,.,0,.,.,1,.,.,.,.,.,0,.,0,1,0,.,.,.,.,.,.,1,.,1,0,.,1,0,.,.,.,.,.,0,.,.,.,.,.,.,1,.,.,.,.,.,1,.,0,1,0,1,.,0,.,.,.,.,.,0,.,.,.,.,.,.,1,.,.,.,.,.,1,0,.,1,0,.,0,.,.,.,.,.,.,1,0,1,.,1,.,.,.,.,.,0,.,.,1,.,.,.,.,0,0,.,.,.,1,.,1,.,1,.,0,1,.,.,0,.,0,.,0,0,.,0,.,1,.,0,.,.,1,.,0,.,.,.,.,0,.,.,.,0,.,.,.,0,1,0,1,.,.,1,.,.,.,1,.,.,0,0,.,.,.,0,0,.,.,1,.,.,.,.,.,.,.,0,.,.,0,.,1,.,.,.,.,.,1,.,0,.,.,.";
		string BoardStringWithoutCommas = @"
...1.0.....0.1..1.
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

		[TestMethod]
		public void Parse_Test()
		{
			var board = new Board() { MyPlayer = Player.Player1, Round = 0 };
			board.Field = BotParser.ParseBoard(BoardString);

			Assert.AreEqual(0, board.Field[new Position(0, 0).Index]);
			Assert.AreEqual(2, board.Field[new Position(3, 0).Index]);
			Assert.AreEqual(1, board.Field[new Position(5, 0).Index]);
			Assert.AreEqual(1, board.Field[new Position(17, 12).Index]);
			Assert.AreEqual(0, board.Field[new Position(17, 15).Index]);

			Assert.AreEqual(BoardString, board.BoardString());
		}

		[TestMethod]
		public void Parse_NoCommas_Test()
		{
			var board = new Board() { MyPlayer = Player.Player1, Round = 0 };

			board.Field = BotParser.ParseBoard(BoardStringWithoutCommas);

			Assert.AreEqual(BoardString, board.BoardString());
		}

		[TestMethod]
		public void Parse_3x2_Test()
		{
			var board = new Board();
			board.Field = BotParser.ParseBoard(3, 2, @"
.0.
1.0
");
			Assert.AreEqual(".,0,.,1,.,0", board.BoardString(3, 2));
		}
	}
}
