using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiddlesHackaton2017.Test.Models
{
	[TestClass]
	public class BoardTest : BoardTestBase
	{
		[TestMethod]
		public void CopyConstructor_Test()
		{
			var board = InitBoard();
			var newBoard = new Board(board);
			//Assert.AreEqual(new Position(3, 7).Index, newBoard.Player1Position, "newBoard.Player1Location");
			//Assert.AreEqual(new Position(12, 7).Index, newBoard.Player2Position, "newBoard.Player2Location");
			Assert.AreEqual(board.MyPlayer, newBoard.MyPlayer, "MyPlayer");
			Assert.AreEqual(board.Round, newBoard.Round, "Round");
			Assert.Fail();
		}

		[TestMethod]
		public void OpponentBoard_Test()
		{
			var board = new Board()
			{
				Player1Position = new Position(3, 7).Index,
				Player2Position = new Position(12, 7).Index,
				MyPlayer = Player.Player2
			};
			var opponentBoard = board.OpponentBoard;
			//Assert.AreEqual(new Position(3, 7).Index, opponentBoard.Player1Position, "newBoard.Player1Location");
			//Assert.AreEqual(new Position(12, 7).Index, opponentBoard.Player2Position, "newBoard.Player2Location");
			Assert.AreEqual(Player.Player1, opponentBoard.MyPlayer, "MyPlayer");
			Assert.Fail();
		}

		[TestMethod]
		public void CopyAndPlay_Player1_Test()
		{
			var board = new Board()
			{
				Player1Position = new Position(3, 7).Index,
				Player2Position = new Position(12, 7).Index,
				MyPlayer = Player.Player2,
				Round = 3
			};
			int move = new Position(3, 8).Index;
			var newBoard = Board.CopyAndPlay(board, Player.Player1, move);
			//Assert.AreEqual(new Position(3, 8).Index, newBoard.Player1Position, "newBoard.Player1Location");
			//Assert.AreEqual(new Position(12, 7).Index, newBoard.Player2Position, "newBoard.Player2Location");
			Assert.AreEqual(board.MyPlayer, newBoard.MyPlayer, "MyPlayer");
			Assert.AreEqual(3, newBoard.Round, "Round");
			Assert.Fail();
		}

		[TestMethod]
		public void CopyAndPlay_Player2_Test()
		{
			var board = new Board()
			{
				Player1Position = new Position(3, 7).Index,
				Player2Position = new Position(12, 7).Index,
				MyPlayer = Player.Player2,
				Round = 3
			};
			int move = new Position(3, 8).Index;
			var newBoard = Board.CopyAndPlay(board, Player.Player2, move);
			Assert.AreEqual(board.Player1Position, newBoard.Player1Position, "newBoard.Player1Location");
			Assert.AreEqual(move, newBoard.Player2Position, "newBoard.Player2Location");
			Assert.AreEqual(board.MyPlayer, newBoard.MyPlayer, "MyPlayer");
			Assert.AreEqual(4, newBoard.Round, "Round");
			Assert.Fail();
		}
	}
}
