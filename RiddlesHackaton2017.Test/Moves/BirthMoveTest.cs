using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;

namespace RiddlesHackaton2017.Test.Moves
{
	[TestClass]
	public class BirthMoveTest : TestBase
	{
		[TestMethod]
		public void Apply_Valid_Test()
		{
			var board = InitBoard();
			var birthPosition = new Position(0, 0);
			var sacrificePosition1 = new Position(5, 0);
			var sacrificePosition2 = new Position(11, 0);
			var move = new BirthMove(birthPosition, sacrificePosition1, sacrificePosition2);

			var newBoard = move.Apply(board, Player.Player1);

			Assert.AreEqual(1, newBoard.Field[birthPosition.Index], "Born position");
			Assert.AreEqual(0, newBoard.Field[sacrificePosition1.Index], "SacrificePosition1");
			Assert.AreEqual(0, newBoard.Field[sacrificePosition2.Index], "SacrificePosition2");
			Assert.AreEqual(2, newBoard.Field[new Position(0, 3).Index], "Unchanged position");
		}

		[TestMethod, ExpectedException(typeof(InvalidBirthMoveException))]
		public void Apply_InvalidBirthPosition_Test()
		{
			var board = InitBoard();
			var birthPosition = new Position(0, 2);
			var sacrificePosition1 = new Position(5, 0);
			var sacrificePosition2 = new Position(11, 0);

			var move = new BirthMove(birthPosition, sacrificePosition1, sacrificePosition2);

			move.Apply(board, Player.Player1);
		}

		[TestMethod, ExpectedException(typeof(InvalidBirthMoveException))]
		public void Apply_InvalidSacrificePosition1_Test()
		{
			var board = InitBoard();
			var birthPosition = new Position(0, 0);
			var sacrificePosition1 = new Position(0, 0);
			var sacrificePosition2 = new Position(11, 0);

			var move = new BirthMove(birthPosition, sacrificePosition1, sacrificePosition2);

			move.Apply(board, Player.Player1);
		}

		[TestMethod, ExpectedException(typeof(InvalidBirthMoveException))]
		public void Apply_InvalidSacrificePosition2_Test()
		{
			var board = InitBoard();
			var birthPosition = new Position(0, 0);
			var sacrificePosition1 = new Position(5, 0);
			var sacrificePosition2 = new Position(0, 0);

			var move = new BirthMove(birthPosition, sacrificePosition1, sacrificePosition2);

			move.Apply(board, Player.Player1);
		}

		[TestMethod, ExpectedException(typeof(InvalidBirthMoveException))]
		public void Apply_DuplicateSacrificePositions_Test()
		{
			var board = InitBoard();
			var birthPosition = new Position(0, 0);
			var sacrificePosition1 = new Position(5, 0);
			var sacrificePosition2 = new Position(5, 0);

			var move = new BirthMove(birthPosition, sacrificePosition1, sacrificePosition2);

			move.Apply(board, Player.Player1);
		}

		[TestMethod]
		public void OutputString_Test()
		{
			var birthPosition = new Position(0, 0);
			var sacrificePosition1 = new Position(5, 0);
			var sacrificePosition2 = new Position(11, 0);

			var move = new BirthMove(birthPosition, sacrificePosition1, sacrificePosition2);

			Assert.AreEqual("birth 0,0 5,0 11,0", move.ToOutputString());
		}

		[TestMethod]
		public void TryParse_Parsable_Test()
		{
			Move move;
			Assert.IsTrue(BirthMove.TryParse("birth 1,0 5,1 11,12", out move));
			Assert.AreEqual(new BirthMove(new Position(1, 0), new Position(5, 1), new Position(11, 12)), move);
		}

		[TestMethod]
		public void TryParse_NotParsable_Test()
		{
			Move move;
			Assert.IsFalse(BirthMove.TryParse("foo", out move));
			Assert.AreEqual(new NullMove(), move);
		}

		[TestMethod]
		public void TryParse_NotParsable2_Test()
		{
			Move move;
			Assert.IsFalse(BirthMove.TryParse("birth", out move));
			Assert.AreEqual(new NullMove(), move);
		}

		[TestMethod]
		public void TryParse_NotParsable3_Test()
		{
			Move move;
			Assert.IsFalse(BirthMove.TryParse("birth x,y 5,0 6,0", out move));
			Assert.AreEqual(new NullMove(), move);
		}

		[TestMethod]
		public void TryParse_NotParsable4_Test()
		{
			Move move;
			Assert.IsFalse(BirthMove.TryParse("birth 5,0 6,0", out move));
			Assert.AreEqual(new NullMove(), move);
		}
	}
}
