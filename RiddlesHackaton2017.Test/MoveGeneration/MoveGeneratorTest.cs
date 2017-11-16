using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.MoveGeneration;

namespace RiddlesHackaton2017.Test.MoveGeneration
{
	[TestClass]
	public class MoveGeneratorTest : TestBase
	{
		[TestMethod]
		public void GetMyKills_Busy()
		{
			var board = new Board();
			board.Field[new Position(0, 0).Index] = 1;
			board.Field[new Position(1, 0).Index] = 1;
			board.Field[new Position(1, 1).Index] = 1;

			//Add stable blocks for player 1 and 2
			board.Field[new Position(4, 0).Index] = 1;
			board.Field[new Position(5, 0).Index] = 1;
			board.Field[new Position(4, 1).Index] = 1;
			board.Field[new Position(5, 1).Index] = 1;
			board.Field[new Position(14, 0).Index] = 2;
			board.Field[new Position(15, 0).Index] = 2;
			board.Field[new Position(14, 1).Index] = 2;
			board.Field[new Position(15, 1).Index] = 2;
			board.Player1FieldCount = board.CalculatedPlayer1FieldCount;
			board.Player2FieldCount = board.CalculatedPlayer2FieldCount;

			var parameters = new MonteCarloParameters() { WinBonus = new int[Board.Size] };
			var moveGenerator = new MoveGenerator(board, parameters);
			var board1 = board.NextGeneration;
			var board2 = board1.NextGeneration;
			var killBoard = new Board(board);
			var killBoard1 = new Board(board1);
			var killBoard2 = new Board(board2);
			var myKills = moveGenerator.GetMyKills(board1, board2, killBoard, killBoard1, killBoard2);

			var kill = myKills[new Position(0, 0).Index];
			Assert.AreEqual(-4, kill);

			kill = myKills[new Position(4, 0).Index];
			Assert.AreEqual(0, kill);
		}

		[TestMethod]
		public void GetMyKills_Lost()
		{
			var board = new Board();
			board.Field[new Position(0, 0).Index] = 1;
			board.Field[new Position(1, 0).Index] = 1;
			board.Field[new Position(1, 1).Index] = 1;

			//Add stable blocks for player 2
			board.Field[new Position(14, 0).Index] = 2;
			board.Field[new Position(15, 0).Index] = 2;
			board.Field[new Position(14, 1).Index] = 2;
			board.Field[new Position(15, 1).Index] = 2;
			board.Player1FieldCount = board.CalculatedPlayer1FieldCount;
			board.Player2FieldCount = board.CalculatedPlayer2FieldCount;

			var parameters = new MonteCarloParameters() { WinBonus = new int[Board.Size] };
			var moveGenerator = new MoveGenerator(board, parameters);
			var board1 = board.NextGeneration;
			var board2 = board1.NextGeneration;
			var killBoard = new Board(board);
			var killBoard1 = new Board(board1);
			var killBoard2 = new Board(board2);
			var myKills = moveGenerator.GetMyKills(board1, board2, killBoard, killBoard1, killBoard2);

			var kill0 = myKills[0];
			Assert.AreEqual(-4, kill0);
		}

		[TestMethod]
		public void GetMyKills_Won()
		{
			var board = new Board();
			board.Field[new Position(0, 0).Index] = 1;
			board.Field[new Position(1, 0).Index] = 2;
			board.Field[new Position(1, 1).Index] = 2;

			//Add stable blocks for player 2
			board.Field[new Position(4, 0).Index] = 1;
			board.Field[new Position(5, 0).Index] = 1;
			board.Field[new Position(4, 1).Index] = 1;
			board.Field[new Position(5, 1).Index] = 1;
			board.Player1FieldCount = board.CalculatedPlayer1FieldCount;
			board.Player2FieldCount = board.CalculatedPlayer2FieldCount;

			var parameters = new MonteCarloParameters() { WinBonus = new int[Board.Size] };
			var moveGenerator = new MoveGenerator(board, parameters);
			var board1 = board.NextGeneration;
			var board2 = board1.NextGeneration;
			var killBoard = new Board(board);
			var killBoard1 = new Board(board1);
			var killBoard2 = new Board(board2);
			var myKills = moveGenerator.GetMyKills(board1, board2, killBoard, killBoard1, killBoard2);

			var kill = myKills[0];
			Assert.AreEqual(2, kill);

			kill = myKills[new Position(4, 0).Index];
			Assert.AreEqual(0, kill);
		}

		[TestMethod]
		public void GetMyKills_Draw()
		{
			var board = new Board();
			board.Field[new Position(0, 0).Index] = 1;
			board.Field[new Position(1, 0).Index] = 2;
			board.Field[new Position(1, 1).Index] = 2;

			//Add stable blocks for player 2
			board.Field[new Position(4, 0).Index] = 1;
			board.Field[new Position(5, 1).Index] = 1;
			board.Field[new Position(6, 0).Index] = 1;
			board.Player1FieldCount = board.CalculatedPlayer1FieldCount;
			board.Player2FieldCount = board.CalculatedPlayer2FieldCount;

			var parameters = new MonteCarloParameters() { WinBonus = new int[Board.Size] };
			var moveGenerator = new MoveGenerator(board, parameters);
			var board1 = board.NextGeneration;
			var board2 = board1.NextGeneration;
			var killBoard = new Board(board);
			var killBoard1 = new Board(board1);
			var killBoard2 = new Board(board2);
			var myKills = moveGenerator.GetMyKills(board1, board2, killBoard, killBoard1, killBoard2);

			//Draw in two because we will both loose all in two turns
			var kill = myKills[new Position(0, 0).Index];
			Assert.AreEqual(2, kill);

			//If we kill the other one, then stable state with 1 of mine and two of his
			kill = myKills[new Position(4, 0).Index];
			Assert.AreEqual(0, kill);
		}

		/// <remarks>First round of f9474a9c-4252-443c-b652-095d2dcb0c5f (V15, Renegade)</remarks>
		[TestMethod]
		public void Test()
		{
			var board = InitBoard("update game field 1,.,.,.,0,.,.,.,1,.,.,.,1,0,.,.,1,.,0,.,.,.,.,.,.,.,.,.,0,.,1,0,.,1,0,.,.,1,.,.,.,1,1,1,.,1,.,.,0,.,.,0,0,1,1,.,.,.,.,0,.,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,1,.,.,.,.,0,.,.,0,1,0,.,.,.,.,0,1,1,1,.,0,.,.,.,.,0,.,1,.,.,.,0,.,.,.,.,1,0,.,.,0,1,.,.,.,0,0,0,.,0,.,.,.,.,.,.,0,.,1,0,.,.,0,.,1,0,.,1,.,.,1,0,.,1,.,.,.,.,.,.,1,.,1,1,1,.,.,.,0,1,.,.,1,0,.,.,.,.,1,.,.,.,0,.,1,.,.,.,.,1,.,0,0,0,1,.,.,.,.,1,0,1,.,.,1,.,.,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,1,.,.,.,.,0,0,1,1,.,.,1,.,.,0,.,0,0,0,.,.,.,0,.,.,1,0,.,1,0,.,1,.,.,.,.,.,.,.,.,.,1,.,0,.,.,1,0,.,.,.,0,.,.,.,1,.,.,.,0");

			var board1 = board.NextGeneration;
			var board2 = board1.NextGeneration;
			var afterMoveBoard = new Board(board);
			var afterMoveBoard1 = new Board(board1);
			var afterMoveBoard2 = new Board(board2);

			int i = new Position(5, 9).Index;
			var neighbours1 = Board.NeighbourFieldsAndThis[i];
			var neighbours2 = Board.NeighbourFields2[i];
			afterMoveBoard.Field[i] = 0;

			var checkBoard = new Board(afterMoveBoard);
			checkBoard.Player1FieldCount = checkBoard.CalculatedPlayer1FieldCount;
			checkBoard.Player2FieldCount = checkBoard.CalculatedPlayer2FieldCount;

			var parameters = new MonteCarloParameters() { WinBonus = new int[Board.Size] };
			var moveGenerator = new MoveGenerator(board, parameters);
			var score = moveGenerator.CalculateMoveScore(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
			afterMoveBoard.Field[i] = board.Field[i];

			Assert.AreEqual(board1.HumanBoardString(), afterMoveBoard1.HumanBoardString());
			Assert.AreEqual(board2.HumanBoardString(), afterMoveBoard2.HumanBoardString());
			Assert.AreEqual(0, BoardEvaluator.Evaluate(board2).Score);
			Assert.AreEqual(5, BoardEvaluator.Evaluate(checkBoard.NextGeneration.NextGeneration).Score);
			Assert.AreEqual(5, score);
		}

	}
}
