using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			Assert.AreEqual(board.Player1FieldCount, newBoard.Player1FieldCount);
			Assert.AreEqual(board.Player2FieldCount, newBoard.Player2FieldCount);
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
			Assert.AreEqual(board.Player1FieldCount, opponentBoard.Player2FieldCount);
			Assert.AreEqual(board.Player2FieldCount, opponentBoard.Player1FieldCount);
		}

		[TestMethod]
		public void NextGeneration_Test()
		{
			var board = InitBoard(4, 4, @"
1...
0...
.1..
1...");
			var next = board.NextGeneration;

			AssertHumanBoardString(@"
....
01..
11..
....
", board.NextGeneration.HumanBoardString(4, 4));
		}

		[TestMethod]
		public void NextNextGeneration_Test()
		{
			var board = InitBoard(4, 4, @"
1...
0...
.1..
1...");
			var next = board.NextGeneration.NextGeneration;

			AssertHumanBoardString(@"
....
01..
11..
....
", board.NextGeneration.HumanBoardString(4, 4));
		}

		/// <summary>
		/// Start with an initial board
		/// Move to the next generation
		/// Player 1 does a birth move: position (0,13) is born, positions (16,0) and (16,4) are sacrificed
		/// Move to the next generation
		/// </summary>
		[TestMethod]
		public void NextGeneration_Test2()
		{
			var board = InitBoard(".,.,.,1,.,1,.,.,.,.,.,0,.,0,1,1,.,1,.,.,1,1,.,.,.,.,.,0,1,.,1,.,.,.,0,.,0,.,.,.,1,.,0,.,1,.,0,.,.,.,.,1,.,.,.,.,1,.,0,.,1,.,0,.,0,.,0,.,.,.,1,.,0,.,.,.,1,.,1,.,0,0,.,0,.,1,.,1,.,.,.,.,0,0,.,.,.,.,.,.,.,0,1,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,1,.,.,1,.,.,1,.,.,.,.,.,.,1,1,0,.,.,1,1,.,1,.,1,.,.,0,.,0,.,0,0,.,.,1,0,0,.,.,.,.,.,.,0,.,.,0,.,.,0,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,0,1,.,.,.,.,.,.,.,1,1,.,.,.,.,0,.,0,.,1,.,1,1,.,0,.,0,.,.,.,1,.,0,.,.,.,1,.,1,.,1,.,0,.,1,.,0,.,.,.,.,0,.,.,.,.,1,.,0,.,1,.,0,.,.,.,1,.,1,.,.,.,0,.,0,1,.,.,.,.,.,0,0,.,.,0,.,0,0,1,.,1,.,.,.,.,.,0,.,0,.,.,.");

			Console.WriteLine("Original:");
			Console.WriteLine(board.HumanBoardString());

			var newBoard = board.NextGeneration;

			Console.WriteLine("After my move:");
			Console.WriteLine(newBoard.HumanBoardString());

			var opponentMove = new BirthMove(new Position(0, 13), new Position(16, 0), new Position(16, 4));
			newBoard = opponentMove.Apply(newBoard, validate: true);

			Console.WriteLine("After opponent apply move:");
			Console.WriteLine(newBoard.HumanBoardString());

			newBoard = newBoard.NextGeneration;

			Console.WriteLine("After opponent move next generation:");
			Console.WriteLine(newBoard.HumanBoardString());

			var expectedBboard = InitBoard(".,.,1,.,1,.,.,.,.,0,0,.,.,.,1,1,.,.,.,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,1,.,.,1,.,.,1,1,.,0,.,1,.,.,.,.,1,0,.,.,.,0,.,.,.,0,.,0,0,.,.,.,.,.,.,.,0,0,.,1,1,.,.,.,.,.,0,.,.,.,.,.,.,.,.,0,0,.,.,.,0,.,.,.,.,1,1,.,.,1,.,.,.,.,.,.,.,0,.,.,.,1,1,.,.,.,.,1,.,.,.,.,.,0,0,1,0,0,.,1,.,1,1,1,1,.,.,.,.,0,0,0,0,.,0,.,1,1,0,1,1,.,.,.,.,.,0,.,.,.,.,0,0,.,.,.,1,.,.,.,.,.,.,0,.,.,.,0,0,.,.,.,.,1,.,.,.,1,1,.,.,0,.,.,.,.,.,1,.,.,.,.,.,0,0,.,1,1,.,.,.,.,.,.,.,1,1,.,1,.,.,.,1,.,.,.,1,1,.,.,.,.,0,.,1,.,0,0,.,.,0,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,0,0,0,.,.,.,1,1,.,.,.,.,0,.,0,.,.");
			Console.WriteLine("Expected:");
			Console.WriteLine(expectedBboard.HumanBoardString());

			Assert.AreEqual(expectedBboard.HumanBoardString(), newBoard.HumanBoardString());

			Assert.AreEqual(newBoard.GetCalculatedPlayerFieldCount(Player.Player1), newBoard.Player1FieldCount);
			Assert.AreEqual(newBoard.GetCalculatedPlayerFieldCount(Player.Player2), newBoard.Player2FieldCount);

			var sb = new StringBuilder();
			for (int i = 0; i < Board.Size; i++)
			{
				if (expectedBboard.Field[i] != newBoard.Field[i])
				{
					sb.AppendLine($"{new Position(i)}: expected: {expectedBboard.Field[i]}, actual: {newBoard.Field[i]}");
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

			var newBoard = board.NextGeneration;

			Console.WriteLine("After my move:");
			Console.WriteLine(newBoard.HumanBoardString());

			var opponentMove = new BirthMove(new Position(0, 13), new Position(16, 0), new Position(16, 4));
			newBoard = newBoard.ApplyMoveAndNext(opponentMove, validateMove: true);

			Console.WriteLine("After copy and play");
			Console.WriteLine(newBoard.HumanBoardString());

			var expectedBboard = InitBoard(".,.,1,.,1,.,.,.,.,0,0,.,.,.,1,1,.,.,.,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,0,.,.,1,.,.,1,.,.,1,1,.,0,.,1,.,.,.,.,1,0,.,.,.,0,.,.,.,0,.,0,0,.,.,.,.,.,.,.,0,0,.,1,1,.,.,.,.,.,0,.,.,.,.,.,.,.,.,0,0,.,.,.,0,.,.,.,.,1,1,.,.,1,.,.,.,.,.,.,.,0,.,.,.,1,1,.,.,.,.,1,.,.,.,.,.,0,0,1,0,0,.,1,.,1,1,1,1,.,.,.,.,0,0,0,0,.,0,.,1,1,0,1,1,.,.,.,.,.,0,.,.,.,.,0,0,.,.,.,1,.,.,.,.,.,.,0,.,.,.,0,0,.,.,.,.,1,.,.,.,1,1,.,.,0,.,.,.,.,.,1,.,.,.,.,.,0,0,.,1,1,.,.,.,.,.,.,.,1,1,.,1,.,.,.,1,.,.,.,1,1,.,.,.,.,0,.,1,.,0,0,.,.,0,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,0,0,0,.,.,.,1,1,.,.,.,.,0,.,0,.,.");
			Console.WriteLine("Expected:");
			Console.WriteLine(expectedBboard.HumanBoardString());

			Assert.AreEqual(expectedBboard.HumanBoardString(), newBoard.HumanBoardString());
			Assert.AreEqual(1, newBoard.Round);

			Assert.AreEqual(newBoard.GetCalculatedPlayerFieldCount(Player.Player1), newBoard.Player1FieldCount);
			Assert.AreEqual(newBoard.GetCalculatedPlayerFieldCount(Player.Player2), newBoard.Player2FieldCount);

			var sb = new StringBuilder();
			for (int i = 0; i < Board.Size; i++)
			{
				if (expectedBboard.Field[i] != newBoard.Field[i])
				{
					sb.AppendLine($"{new Position(i)}: expected: {expectedBboard.Field[i]}, actual: {newBoard.Field[i]}");
				}
			}
			if (sb.Length > 0)
			{
				Console.WriteLine("Differences:");
				Console.WriteLine(sb.ToString());
			}
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

		[TestMethod, Ignore]
		public void NextGeneration2_PerformanceTest()
		{
			int n = 100000;

			var board = InitBoard();

			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			for (int i = 0; i < n; i++)
			{
				board.ResetNextGeneration();
				var nextBoard = board.NextGeneration;
				//Assert.AreEqual(nextBoard.HumanBoardString(), board.NextGeneration.HumanBoardString());
			}
			Console.WriteLine($"NextGeneration2: {stopwatch.ElapsedMilliseconds}");
			stopwatch.Restart();
			for (int i = 0; i < n; i++)
			{
				board.ResetNextGeneration();
				board.CalculateNeighbours();
				var nextBoard = board.NextGeneration;
			}
			Console.WriteLine($"Calculate + NextGeneration2: {stopwatch.ElapsedMilliseconds}");
			stopwatch.Restart();
		}

		[TestMethod]
		public void GetDeltaFieldCountForKill_Test()
		{
			var board = GetBoard("edb3825c-6629-4c2b-90cb-7c1e0aa71de9", 1);
			var nextGeneration = board.NextGeneration;

			//Round 1

			//Sacrifice position 1
			int i = new Position(1, 5).Index;
			int result = board.GetDeltaFieldCountForKill(i, Player.Player1, Player.Player1, validate: true);
			Assert.AreEqual(-1, result);

			//Sacrifice position 2
			i = new Position(8, 1).Index;
			result = board.GetDeltaFieldCountForKill(i, Player.Player1, Player.Player1, validate: true);
			Assert.AreEqual(-3, result);

			//Game 59f191a9-33d3-4f12-a38b-5a42346ba4c8, player2
			board = GetBoard("59f191a9-33d3-4f12-a38b-5a42346ba4c8", 1);
			nextGeneration = board.NextGeneration;

			i = new Position(7, 9).Index;
			result = board.GetDeltaFieldCountForKill(i, Player.Player2, Player.Player2, validate: true);
			Assert.AreEqual(3, result);
		}

		[TestMethod]
		public void GetDeltaFieldCountForBirth_Test()
		{
			var board = GetBoard("edb3825c-6629-4c2b-90cb-7c1e0aa71de9", 1);
			var nextGeneration = board.NextGeneration;

			//Round 1

			//Birth position: example of a birth move on a cell which neither of us would own in the next generation
			int i = new Position(14, 6).Index;
			int result = board.GetDeltaFieldCountForBirth(i, Player.Player1, Player.Player1, validate: true);
			Assert.AreEqual(2, result);

			//Round 2
			board = GetBoard("edb3825c-6629-4c2b-90cb-7c1e0aa71de9", 2);
			nextGeneration = board.NextGeneration;

			//Birth position: example of a birth move on a cell which the opponent would own in the next generation
			i = new Position(9, 9).Index;
			result = board.GetDeltaFieldCountForBirth(i, Player.Player1, Player.Player1, validate: true);
			Assert.AreEqual(5, result);

			//Round 43
			board = GetBoard("edb3825c-6629-4c2b-90cb-7c1e0aa71de9", 43);
			nextGeneration = board.NextGeneration;

			//Birth position: example of a birth move on a cell which we would own in the next generation
			i = new Position(10, 4).Index;
			result = board.GetDeltaFieldCountForBirth(i, Player.Player1, Player.Player1, validate: true);
			Assert.AreEqual(5, result);

			//Game 59f191a9-33d3-4f12-a38b-5a42346ba4c8, player2
			board = GetBoard("59f191a9-33d3-4f12-a38b-5a42346ba4c8", 1);
			nextGeneration = board.NextGeneration;

			//Birth
			i = new Position(16, 6).Index;
			result = board.GetDeltaFieldCountForBirth(i, Player.Player2, Player.Player2, validate: true);
			Assert.AreEqual(3, result);

		}
	}
}
