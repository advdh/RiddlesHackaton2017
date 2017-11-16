using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.MoveGeneration;
using RiddlesHackaton2017.Test;
using System;
using System.Diagnostics;

namespace RiddlesHackaton2017.IntegrationTest
{
	[TestClass]
	public class PerformanceTest : TestBase
	{
		[TestMethod]
		public void Combined_PerformanceTest()
		{
			int count = 1000;

			var board = ExampleBoard();
			var parameters = new MonteCarloParameters() { WinBonus = new int[Board.Size] };
			var moveGenerator = new MoveGenerator(board, parameters);

			var stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < count; i++)
			{
				var board1 = board.NextGeneration;
				var board2 = board1.NextGeneration;
				var killBoard = new Board(board);
				var killBoard1 = new Board(board1);
				var killBoard2 = new Board(board2);
				moveGenerator.GetBirths(board1, board2, killBoard, killBoard1, killBoard2);
				moveGenerator.GetMyKills(board1, board2, killBoard, killBoard1, killBoard2);
				moveGenerator.GetOpponentKills(board1, board2, killBoard, killBoard1, killBoard2);
			}

			Console.WriteLine($"Average duration: {(double)stopwatch.ElapsedMilliseconds / count : 0.00} ms");
		}

		[TestMethod]
		public void Corniel_Test()
		{
			string s =  ".,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						"0,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.," +
						".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.";
			var board = new Board();
			board.Field = BotParser.ParseBoard(s);

			var moveBoard = new Board(board);
			moveBoard.Field[new Position(2, 2).Index] = 1;
			moveBoard.Player2FieldCount = moveBoard.CalculatedPlayer2FieldCount;

			var nextBoard = board.NextGeneration;
			var moveNextBoard = moveBoard.NextGeneration;

			Console.WriteLine(BoardEvaluator.Evaluate(moveNextBoard).Score);
			Console.WriteLine(BoardEvaluator.Evaluate(nextBoard).Score);
		}
	}
}
