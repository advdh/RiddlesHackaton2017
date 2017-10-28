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
		public void Apply_DuplicateSacricePositions_Test()
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
	}
}
