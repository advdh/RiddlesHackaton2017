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
			board.Round = 3;
			var opponentBoard = board.OpponentBoard;
			Assert.AreEqual(board.MyPlayer, opponentBoard.OpponentPlayer, "OpponentPlayer");
			Assert.AreEqual(board.OpponentPlayer, opponentBoard.MyPlayer, "MyPlayer");
			Assert.AreEqual(board.Round, opponentBoard.Round, "Round");

			Assert.AreEqual(0, opponentBoard.Field[0]);
			Assert.AreEqual(2, opponentBoard.Field[2]);
			Assert.AreEqual(1, opponentBoard.Field[3]);
		}

		/// <summary>
		/// Start with an initial board
		/// Move to the next generation
		/// Player 1 does a birth move: position (0,13) is born, positions (16,0) and (16,4) are sacrificed
		/// Move to the next generation
		/// </summary>
		[TestMethod]
		public void NextGeneration2_Test()
		{
			var board = InitBoard(".,.,.,1,.,1,.,.,.,.,.,0,.,0,1,1,.,1,.,.,1,1,.,.,.,.,.,0,1,.,1,.,.,.,0,.,0,.,.,.,1,.,0,.,1,.,0,.,.,.,.,1,.,.,.,.,1,.,0,.,1,.,0,.,0,.,0,.,.,.,1,.,0,.,.,.,1,.,1,.,0,0,.,0,.,1,.,1,.,.,.,.,0,0,.,.,.,.,.,.,.,0,1,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,1,.,.,1,.,.,1,.,.,.,.,.,.,1,1,0,.,.,1,1,.,1,.,1,.,.,0,.,0,.,0,0,.,.,1,0,0,.,.,.,.,.,.,0,.,.,0,.,.,0,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,0,1,.,.,.,.,.,.,.,1,1,.,.,.,.,0,.,0,.,1,.,1,1,.,0,.,0,.,.,.,1,.,0,.,.,.,1,.,1,.,1,.,0,.,1,.,0,.,.,.,.,0,.,.,.,.,1,.,0,.,1,.,0,.,.,.,1,.,1,.,.,.,0,.,0,1,.,.,.,.,.,0,0,.,.,0,.,0,0,1,.,1,.,.,.,.,.,0,.,0,.,.,.");

			Console.WriteLine("Original:");
			Console.WriteLine(board.HumanBoardString());

			var newBoard = Board.NextGeneration(board);

			Console.WriteLine("After my move:");
			Console.WriteLine(newBoard.HumanBoardString());

			var opponentMove = new BirthMove(new Position(0, 13), new Position(16, 0), new Position(16, 4));
			newBoard = opponentMove.Apply(newBoard, Player.Player2);

			Console.WriteLine("After opponent apply move:");
			Console.WriteLine(newBoard.HumanBoardString());

			newBoard = Board.NextGeneration(newBoard);

			Console.WriteLine("After opponent move next generation:");
			Console.WriteLine(newBoard.HumanBoardString());

			var expectedBboard = InitBoard(".,.,1,.,1,.,.,.,.,0,0,.,.,.,1,1,.,.,.,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,1,.,.,1,.,.,1,1,.,0,.,1,.,.,.,.,1,0,.,.,.,0,.,.,.,0,.,0,0,.,.,.,.,.,.,.,0,0,.,1,1,.,.,.,.,.,0,.,.,.,.,.,.,.,.,0,0,.,.,.,0,.,.,.,.,1,1,.,.,1,.,.,.,.,.,.,.,0,.,.,.,1,1,.,.,.,.,1,.,.,.,.,.,0,0,1,0,0,.,1,.,1,1,1,1,.,.,.,.,0,0,0,0,.,0,.,1,1,0,1,1,.,.,.,.,.,0,.,.,.,.,0,0,.,.,.,1,.,.,.,.,.,.,0,.,.,.,0,0,.,.,.,.,1,.,.,.,1,1,.,.,0,.,.,.,.,.,1,.,.,.,.,.,0,0,.,1,1,.,.,.,.,.,.,.,1,1,.,1,.,.,.,1,.,.,.,1,1,.,.,.,.,0,.,1,.,0,0,.,.,0,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,0,0,0,.,.,.,1,1,.,.,.,.,0,.,0,.,.");
			Console.WriteLine("Expected:");
			Console.WriteLine(expectedBboard.HumanBoardString());

			Assert.AreEqual(expectedBboard.HumanBoardString(), newBoard.HumanBoardString());
			var sb = new StringBuilder();
			for (int i = 0; i < Board.Size; i++)
			{
				if (expectedBboard.Field[i] != newBoard.Field[i])
				{
					sb.AppendFormat("{0}: expected: {1}, actual: {2}",
						new Position(i), expectedBboard.Field[i], newBoard.Field[i]);
					sb.AppendLine();
				}
			}
			if (sb.Length > 0)
			{
				Console.WriteLine("Differences:");
				Console.WriteLine(sb.ToString());
			}
		}

		[TestMethod]
		public void CopyAndPlay_Test()
		{
			var board = InitBoard(".,.,.,1,.,1,.,.,.,.,.,0,.,0,1,1,.,1,.,.,1,1,.,.,.,.,.,0,1,.,1,.,.,.,0,.,0,.,.,.,1,.,0,.,1,.,0,.,.,.,.,1,.,.,.,.,1,.,0,.,1,.,0,.,0,.,0,.,.,.,1,.,0,.,.,.,1,.,1,.,0,0,.,0,.,1,.,1,.,.,.,.,0,0,.,.,.,.,.,.,.,0,1,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,1,.,.,1,.,.,1,.,.,.,.,.,.,1,1,0,.,.,1,1,.,1,.,1,.,.,0,.,0,.,0,0,.,.,1,0,0,.,.,.,.,.,.,0,.,.,0,.,.,0,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,0,1,.,.,.,.,.,.,.,1,1,.,.,.,.,0,.,0,.,1,.,1,1,.,0,.,0,.,.,.,1,.,0,.,.,.,1,.,1,.,1,.,0,.,1,.,0,.,.,.,.,0,.,.,.,.,1,.,0,.,1,.,0,.,.,.,1,.,1,.,.,.,0,.,0,1,.,.,.,.,.,0,0,.,.,0,.,0,0,1,.,1,.,.,.,.,.,0,.,0,.,.,.");

			Console.WriteLine("Original:");
			Console.WriteLine(board.HumanBoardString());

			var newBoard = Board.NextGeneration(board);

			Console.WriteLine("After my move:");
			Console.WriteLine(newBoard.HumanBoardString());

			var opponentMove = new BirthMove(new Position(0, 13), new Position(16, 0), new Position(16, 4));
			newBoard = Board.CopyAndPlay(newBoard, Player.Player2, opponentMove);

			Console.WriteLine("After copy and play");
			Console.WriteLine(newBoard.HumanBoardString());

			var expectedBboard = InitBoard(".,.,1,.,1,.,.,.,.,0,0,.,.,.,1,1,.,.,.,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,1,.,.,1,.,.,1,1,.,0,.,1,.,.,.,.,1,0,.,.,.,0,.,.,.,0,.,0,0,.,.,.,.,.,.,.,0,0,.,1,1,.,.,.,.,.,0,.,.,.,.,.,.,.,.,0,0,.,.,.,0,.,.,.,.,1,1,.,.,1,.,.,.,.,.,.,.,0,.,.,.,1,1,.,.,.,.,1,.,.,.,.,.,0,0,1,0,0,.,1,.,1,1,1,1,.,.,.,.,0,0,0,0,.,0,.,1,1,0,1,1,.,.,.,.,.,0,.,.,.,.,0,0,.,.,.,1,.,.,.,.,.,.,0,.,.,.,0,0,.,.,.,.,1,.,.,.,1,1,.,.,0,.,.,.,.,.,1,.,.,.,.,.,0,0,.,1,1,.,.,.,.,.,.,.,1,1,.,1,.,.,.,1,.,.,.,1,1,.,.,.,.,0,.,1,.,0,0,.,.,0,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,0,0,0,.,.,.,1,1,.,.,.,.,0,.,0,.,.");
			Console.WriteLine("Expected:");
			Console.WriteLine(expectedBboard.HumanBoardString());

			Assert.AreEqual(expectedBboard.HumanBoardString(), newBoard.HumanBoardString());
			Assert.AreEqual(1, newBoard.Round);
			var sb = new StringBuilder();
			for (int i = 0; i < Board.Size; i++)
			{
				if (expectedBboard.Field[i] != newBoard.Field[i])
				{
					sb.AppendFormat("{0}: expected: {1}, actual: {2}",
						new Position(i), expectedBboard.Field[i], newBoard.Field[i]);
					sb.AppendLine();
				}
			}
			if (sb.Length > 0)
			{
				Console.WriteLine("Differences:");
				Console.WriteLine(sb.ToString());
			}
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
