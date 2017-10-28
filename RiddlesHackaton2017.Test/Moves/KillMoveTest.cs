using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiddlesHackaton2017.Test.Moves
{
	[TestClass]
	public class KillMoveTest : TestBase
	{
		/// <summary>
		/// Kill own field should reset field
		/// </summary>
		[TestMethod]
		public void Apply_KillOwnField_Test()
		{
			var board = InitBoard();
			var move = new KillMove(new Position(0, 2));

			var newBoard = move.Apply(board, Player.Player1);

			Assert.AreEqual(0, newBoard.Field[new Position(0, 2).Index], "Killed field");
			Assert.AreEqual(2, newBoard.Field[new Position(0, 3).Index], "not killed field");
		}

		/// <summary>
		/// Kill opponent field should reset field
		/// </summary>
		[TestMethod]
		public void Apply_KillOpponentField_Test()
		{
			var board = InitBoard();
			var move = new KillMove(new Position(0, 3));

			var newBoard = move.Apply(board, Player.Player1);

			Assert.AreEqual(1, newBoard.Field[new Position(0, 2).Index], "non-Killed field");
			Assert.AreEqual(0, newBoard.Field[new Position(0, 3).Index], "Killed field");
		}

		/// <summary>
		/// Invalid kill move: nothing to kill
		/// </summary>
		[TestMethod, ExpectedException(typeof(InvalidKillMoveException))]
		public void Apply_KillEmptyField_Test()
		{
			var board = InitBoard();
			var move = new KillMove(new Position(0, 0));

			move.Apply(board, Player.Player1);
		}


		[TestMethod]
		public void OutputString_Test()
		{
			var killPosition = new Position(5, 0);

			var move = new KillMove(killPosition);

			Assert.AreEqual("kill 5,0", move.ToOutputString());
		}
	}
}
