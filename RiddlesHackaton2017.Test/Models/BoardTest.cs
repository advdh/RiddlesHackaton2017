using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiddlesHackaton2017.Test.Models
{
	[TestClass]
	public class BoardTest : TestBase
	{
		[TestMethod]
		public void CopyConstructor_Test()
		{
			var board = InitBoard();
			var newBoard = new Board(board);
			Assert.AreEqual(board.MyPlayer, newBoard.MyPlayer, "MyPlayer");
			Assert.AreEqual(board.Round, newBoard.Round, "Round");
			for (int i = 0; i < Board.Size; i++)
			{
				Assert.AreEqual(board.Field[i], newBoard.Field[i]);
			}
		}

		[TestMethod]
		public void OpponentBoard_Test()
		{
			var board = InitBoard();
			var opponentBoard = board.OpponentBoard;
			Assert.AreEqual(Player.Player1, opponentBoard.MyPlayer, "MyPlayer");
			for (int i = 0; i < Board.Size; i++)
			{
				Assert.AreEqual(3 - board.Field[i], opponentBoard.Field[i]);
			}
		}

		[TestMethod]
		public void NextGeneration_Test()
		{
			var board = InitBoard();
			board.Field[new Position(11, 4).Index] = 0;

			var newBoard = Board.NextGeneration(board);

			Assert.AreEqual(ExampleBoard().HumanBoardString(), newBoard.HumanBoardString());
		}

		[TestMethod]
		public void CopyAndPlay_Player1_Test()
		{
			var board = InitBoard();
			var move = new PassMove();
			var newBoard = Board.CopyAndPlay(board, Player.Player1, move);
			Assert.AreEqual(board.MyPlayer, newBoard.MyPlayer, "MyPlayer");
			Assert.AreEqual(3, newBoard.Round, "Round");
			Assert.Fail();
		}

		[TestMethod]
		public void CopyAndPlay_Player2_Test()
		{
			var board = new Board()
			{
				MyPlayer = Player.Player2,
				Round = 3
			};
			var move = new PassMove();
			var newBoard = Board.CopyAndPlay(board, Player.Player2, move);

			Assert.AreEqual(board.MyPlayer, newBoard.MyPlayer, "MyPlayer");
			Assert.AreEqual(4, newBoard.Round, "Round");
			Assert.Fail();
		}

		[TestMethod]
		public void BoardString_Test()
		{
			var board = InitBoard();
			Assert.AreEqual(StartBoardString, board.BoardString());
		}

		[TestMethod]
		public void HumanBoardString_Test()
		{
			var board = InitBoard();
			Assert.AreEqual(HumanStartBoardString, board.HumanBoardString());
		}
	}
}
