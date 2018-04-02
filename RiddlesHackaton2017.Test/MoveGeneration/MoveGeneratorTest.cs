using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.MoveGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RiddlesHackaton2017.Test.MoveGeneration
{
	[TestClass]
	public class MoveGeneratorTest : TestBase
	{
		[TestMethod]
		public void Test()
		{
			Board.InitializeFieldCountChanges();
			var board = GetBoard("edb3825c-6629-4c2b-90cb-7c1e0aa71de9", 1);
			var board1 = board.NextGeneration;
			var board2 = board1.NextGeneration;
			var afterMoveBoard = new Board(board);
			var afterMoveBoard1 = new Board(board1);
			var afterMoveBoard2 = new Board(board2);

			var generator = new MoveGenerator(board, MonteCarloParameters.Life);
			var kills1 = generator.GetMyKills(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2);
			var kills2 = generator.GetMyKills2();

			Assert.AreEqual(kills1.Count, kills2.Count, "count");
			for (int i = 0; i < kills1.Count; i++)
			{
				Console.WriteLine($"{kills1.Keys.ElementAt(i)} = {kills1.Values.ElementAt(i)}");
				Console.WriteLine($"{kills2.Keys.ElementAt(i)} = {kills2.Values.ElementAt(i)}");
				Assert.AreEqual(kills1.Keys.ElementAt(i), kills2.Keys.ElementAt(i), $"key {i}");
				Assert.AreEqual(kills1.Values.ElementAt(i), kills2.Values.ElementAt(i), $"value {i}");
			}
		}

		[TestMethod]
		public void Test_oI0_Game()
		{
			Board.InitializeFieldCountChanges();
			var board = GetBoard("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4);
		}


		[TestMethod]
		public void PerformancTest()
		{
			int n = 100;

			Board.InitializeFieldCountChanges();
			var board = GetBoard("edb3825c-6629-4c2b-90cb-7c1e0aa71de9", 1);
			var generator = new MoveGenerator(board, MonteCarloParameters.Life);

			var stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < n; i++)
			{
				var board1 = board.NextGeneration;
				var board2 = board1.NextGeneration;
				var afterMoveBoard = new Board(board);
				var afterMoveBoard1 = new Board(board1);
				var afterMoveBoard2 = new Board(board2);

				var kills1 = generator.GetMyKills(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2);
			}
			Console.WriteLine($"GetKills1: {stopwatch.ElapsedMilliseconds}");

			stopwatch.Restart();
			for (int i = 0; i < n; i++)
			{
				var kills2 = generator.GetMyKills2();
			}
			Console.WriteLine($"GetKills2: {stopwatch.ElapsedMilliseconds}");
		}
	}
}
