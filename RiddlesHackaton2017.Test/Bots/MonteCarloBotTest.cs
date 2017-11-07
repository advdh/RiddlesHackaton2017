using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using System.Linq;

namespace RiddlesHackaton2017.Test.Bots
{
	[TestClass]
	public class MonteCarloBotTest : TestBase
	{
		[TestMethod]
		public void GetMyKills_Busy()
		{
			var board = new Board();
			board.Field[new Position(0, 0).Index] = 1;
			board.Field[new Position(1, 0).Index] = 1;
			board.Field[new Position(1, 1).Index] = 1;

			//Add stable blocks for playe r1 and 2
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

			var bot = new MonteCarloBot(new NullConsole(), new FirstIndexGenerator())
			{
				Board = board
			};
			var board1 = board.NextGeneration;
			var board2 = board1.NextGeneration;
			var killBoard = new Board(board);
			var killBoard1 = new Board(board1);
			var killBoard2 = new Board(board2);
			var myKills = bot.GetMyKills(board1, board2, killBoard, killBoard1, killBoard2);

			var kill = myKills[new Position(0, 0).Index];
			Assert.AreEqual(-4, kill.Score);
			Assert.AreEqual(GameStatus.Busy, kill.Status);

			kill = myKills[new Position(4, 0).Index];
			Assert.AreEqual(0, kill.Score);
			Assert.AreEqual(GameStatus.Busy, kill.Status);
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

			var bot = new MonteCarloBot(new NullConsole(), new FirstIndexGenerator())
			{
				Board = board
			};
			var board1 = board.NextGeneration;
			var board2 = board1.NextGeneration;
			var killBoard = new Board(board);
			var killBoard1 = new Board(board1);
			var killBoard2 = new Board(board2);
			var myKills = bot.GetMyKills(board1, board2, killBoard, killBoard1, killBoard2);

			var kill0 = myKills[0];
			Assert.AreEqual(-4, kill0.Score);
			Assert.AreEqual(GameStatus.Lost, kill0.Status);
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

			var bot = new MonteCarloBot(new NullConsole(), new FirstIndexGenerator())
			{
				Board = board
			};
			var board1 = board.NextGeneration;
			var board2 = board1.NextGeneration;
			var killBoard = new Board(board);
			var killBoard1 = new Board(board1);
			var killBoard2 = new Board(board2);
			var myKills = bot.GetMyKills(board1, board2, killBoard, killBoard1, killBoard2);

			var kill = myKills[0];
			Assert.AreEqual(2, kill.Score);
			Assert.AreEqual(GameStatus.Won, kill.Status);

			kill = myKills[new Position(4, 0).Index];
			Assert.AreEqual(0, kill.Score);
			Assert.AreEqual(GameStatus.Busy, kill.Status);
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

			var bot = new MonteCarloBot(new NullConsole(), new FirstIndexGenerator())
			{
				Board = board
			};
			var board1 = board.NextGeneration;
			var board2 = board1.NextGeneration;
			var killBoard = new Board(board);
			var killBoard1 = new Board(board1);
			var killBoard2 = new Board(board2);
			var myKills = bot.GetMyKills(board1, board2, killBoard, killBoard1, killBoard2);

			//Draw in two because we will both loose all in two turns
			var kill = myKills[new Position(0, 0).Index];
			Assert.AreEqual(2, kill.Score);
			Assert.AreEqual(GameStatus.Draw, kill.Status);

			//If we kill the other one, then stable state with 1 of mine and two of his
			kill = myKills[new Position(4, 0).Index];
			Assert.AreEqual(0, kill.Score);
			Assert.AreEqual(GameStatus.Busy, kill.Status);
		}
	}
}
