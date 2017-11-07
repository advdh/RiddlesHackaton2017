using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using RiddlesHackaton2017.Test;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RiddlesHackaton2017.IntegrationTest
{
	[TestClass]
	public class PerformanceTest : TestBase
	{
		[TestMethod]
		public void GetMyKills_PerformanceTest()
		{
			int count = 1000;

			var board = ExampleBoard();
			var bot = new MonteCarloBot(new NullConsole(), new FirstIndexGenerator())
			{
				Board = board
			};

			var stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < count; i++)
			{
				GetMyKillsOld(board);
			}
			TimeSpan old = stopwatch.Elapsed;

			stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < count; i++)
			{
				var board1 = board.NextGeneration;
				var board2 = board1.NextGeneration;
				var killBoard = new Board(board);
				var killBoard1 = new Board(board1);
				var killBoard2 = new Board(board2);
				bot.GetMyKills(board1, board2, killBoard, killBoard1, killBoard2);
			}
			TimeSpan improved = stopwatch.Elapsed;


			Console.WriteLine($"Old: {old}, Improved: {improved} ({(1 - (double)improved.Ticks / old.Ticks):P0} better)");
		}

		[TestMethod]
		public void GetOpponentKills_PerformanceTest()
		{
			int count = 1000;

			var board = ExampleBoard();
			var bot = new MonteCarloBot(new NullConsole(), new FirstIndexGenerator())
			{
				Board = board
			};

			var stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < count; i++)
			{
				GetOpponentKillsOld(board);
			}
			TimeSpan old = stopwatch.Elapsed;

			stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < count; i++)
			{
				var board1 = board.NextGeneration;
				var board2 = board1.NextGeneration;
				var killBoard = new Board(board);
				var killBoard1 = new Board(board1);
				var killBoard2 = new Board(board2);
				bot.GetOpponentKills(board1, board2, killBoard, killBoard1, killBoard2);
			}
			TimeSpan improved = stopwatch.Elapsed;


			Console.WriteLine($"Old: {old}, Improved: {improved} ({(1 - (double)improved.Ticks / old.Ticks):P0} better)");
		}

		[TestMethod]
		public void GetBirths_PerformanceTest()
		{
			int count = 1000;

			var board = ExampleBoard();
			var bot = new MonteCarloBot(new NullConsole(), new FirstIndexGenerator())
			{
				Board = board
			};

			var stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < count; i++)
			{
				GetBirthsOld(board);
			}
			TimeSpan old = stopwatch.Elapsed;

			stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < count; i++)
			{
				var board1 = board.NextGeneration;
				var board2 = board1.NextGeneration;
				var killBoard = new Board(board);
				var killBoard1 = new Board(board1);
				var killBoard2 = new Board(board2);
				bot.GetBirths(board1, board2, killBoard, killBoard1, killBoard2);
			}
			TimeSpan improved = stopwatch.Elapsed;


			Console.WriteLine($"Old: {old}, Improved: {improved} ({(1 - (double)improved.Ticks / old.Ticks):P0} better)");
		}

		[TestMethod]
		public void Combined_PerformanceTest()
		{
			int count = 1000;

			var board = ExampleBoard();
			var bot = new MonteCarloBot(new NullConsole(), new FirstIndexGenerator())
			{
				Board = board
			};

			var stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < count; i++)
			{
				GetBirthsOld(board);
				GetMyKillsOld(board);
				GetOpponentKillsOld(board);
			}
			TimeSpan old = stopwatch.Elapsed;

			stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < count; i++)
			{
				var board1 = board.NextGeneration;
				var board2 = board1.NextGeneration;
				var killBoard = new Board(board);
				var killBoard1 = new Board(board1);
				var killBoard2 = new Board(board2);
				bot.GetBirths(board1, board2, killBoard, killBoard1, killBoard2);
				bot.GetMyKills(board1, board2, killBoard, killBoard1, killBoard2);
				bot.GetOpponentKills(board1, board2, killBoard, killBoard1, killBoard2);
			}
			TimeSpan improved = stopwatch.Elapsed;


			Console.WriteLine($"Old: {old}, Improved: {improved} ({(1 - (double)improved.Ticks / old.Ticks):P0} better)");
		}

		public Dictionary<int, BoardStatus> GetBirthsOld(Board board)
		{
			var result = new Dictionary<int, BoardStatus>();

			foreach (int i in board.EmptyCells)
			{
				var newBoard = new Board(board);
				newBoard.Field[i] = (short)board.MyPlayer;
				newBoard.MyPlayerFieldCount++;
				newBoard = newBoard.NextNextGeneration;
				var score = BoardEvaluator.Evaluate(newBoard);
				result.Add(i, score);
			}
			return result;
		}

		/// <summary>Gets a dictionary of kills on one of my cells with their scores</summary>
		public Dictionary<int, BoardStatus> GetMyKillsOld(Board board)
		{
			var result = new Dictionary<int, BoardStatus>();

			foreach (int i in board.MyCells)
			{
				var newBoard = new Board(board);
				newBoard.Field[i] = 0;
				newBoard.MyPlayerFieldCount--;
				newBoard = newBoard.NextNextGeneration;
				var score = BoardEvaluator.Evaluate(newBoard);
				result.Add(i, score);
			}
			return result;
		}

		public Dictionary<int, BoardStatus> GetOpponentKillsOld(Board board)
		{
			var result = new Dictionary<int, BoardStatus>();

			foreach (int i in board.OpponentCells)
			{
				var newBoard = new Board(board);
				newBoard.Field[i] = 0;
				newBoard.MyPlayerFieldCount--;
				newBoard = newBoard.NextNextGeneration;
				var score = BoardEvaluator.Evaluate(newBoard);
				result.Add(i, score);
			}
			return result;
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
