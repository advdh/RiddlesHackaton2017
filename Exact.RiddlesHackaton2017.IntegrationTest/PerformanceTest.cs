using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using RiddlesHackaton2017.Test;
using System;
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
				bot.GetMyKillsOld();
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
				bot.GetOpponentKillsOld();
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
				bot.GetBirthsOld();
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
				bot.GetBirthsOld();
				bot.GetMyKillsOld();
				bot.GetOpponentKillsOld();
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
	}
}
