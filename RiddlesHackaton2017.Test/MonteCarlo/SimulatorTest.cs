using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.MonteCarlo;
using RiddlesHackaton2017.RandomGeneration;
using System;

namespace RiddlesHackaton2017.Test.MonteCarlo
{
	[TestClass]
	public class SimulatorTest : TestBase
	{
		/// <summary>
		/// Given that there is only one own kill with a positive score and at least one with a zero score
		/// Then GetRandomMove retuns a valid birth move
		/// </summary>
		[TestMethod]
		public void Test()
		{
			var random = new RandomGenerator(new Random(0));
			var board = new Board() {
				Field = BotParser.ParseBoard(".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,1,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,0,0,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,0,0,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,0,.,.,.,.,.,.,.,.,.,.,.,0,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,0,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,0,0,.,0,0,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.")
			};
			board.UpdateFieldCounts();
			Console.WriteLine(board.HumanBoardString());
			var simulator = new Simulator(board, random, new MonteCarloParameters());
			var move = simulator.GetRandomMove(board, Player.Player1);
			Assert.AreEqual("birth 12,1 16,6 13,10", move.ToOutputString());
		}

		/// <summary>
		/// Given that a pass move leads to winning the game
		/// Then GetRandomMove returns a pass move
		/// </summary>
		[TestMethod]
		public void Test2()
		{
			var random = new RandomGenerator(new Random(0));
			var board = new Board()
			{
				Field = BotParser.ParseBoard(".,.,.,.,.,.,.,0,0,0,0,.,.,.,.,.,.,.,.,.,.,.,1,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,.,0,.,.,0,0,0,.,.,.,0,.,.,.,.,.,.,.,.,.,0,.,.,0,.,.,.,0,.,.,.,.,.,.,.,.,.,.,0,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,0,0,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,0,.,.,0,0,0,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.")
			};
			board.UpdateFieldCounts();
			Console.WriteLine(board.HumanBoardString());
			var simulator = new Simulator(board, random, new MonteCarloParameters());
			var move = simulator.GetRandomMove(board, Player.Player1);
			Assert.AreEqual("pass", move.ToOutputString());
		}

		/// <summary>
		/// Given that there are 4 kills with a gain equal to 0, and no kills with a higher gain
		/// Then GetRandomMove returns a valid birth move
		/// </summary>
		[TestMethod]
		public void Test3()
		{
			var random = new RandomGenerator(new Random(0));
			var board = new Board()
			{
				Field = BotParser.ParseBoard(".,.,.,.,1,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,0,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,0,0,0,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.")
			};
			board.UpdateFieldCounts();
			Console.WriteLine(board.HumanBoardString());
			var simulator = new Simulator(board, random, new MonteCarloParameters());
			var move = simulator.GetRandomMove(board, Player.Player1);
			Assert.AreEqual("birth 14,2 16,5 15,4", move.ToOutputString());
		}

		/// <summary>
		/// Given that there are nog enough own kills with gain at least 0 to do a birth move
		/// Then GetRandomMove returns a kill move
		/// </summary>
		[TestMethod]
		public void Test4()
		{
			var random = new RandomGenerator(new Random(0));
			var board = new Board()
			{
				Field = BotParser.ParseBoard(".,1,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,.,.,1,.,.,.,.,.,.,.,.,.,0,0,.,.,.,.,.,.,.,1,1,1,.,.,.,.,.,.,0,0,.,.,.,.,.,.,1,.,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.")
			};
			board.UpdateFieldCounts();
			Console.WriteLine(board.HumanBoardString());
			var simulator = new Simulator(board, random, new MonteCarloParameters());
			var move = simulator.GetRandomMove(board, Player.Player2);
			Assert.AreEqual("kill 15,1", move.ToOutputString());
		}

		/// <summary>
		/// Given that there are nog enough opponent kills with gain at least 0 to do a kill move
		/// Then GetRandomMove returns a pass move
		/// </summary>
		[TestMethod]
		public void Test5()
		{
			var random = new RandomGenerator(new Random(0));
			var board = new Board()
			{
				Field = BotParser.ParseBoard(".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,.,.,.,1,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.")
			};
			board.UpdateFieldCounts();
			Console.WriteLine(board.HumanBoardString());
			var simulator = new Simulator(board, random, new MonteCarloParameters());
			var move = simulator.GetRandomMove(board, Player.Player2);
			Assert.AreEqual("pass", move.ToOutputString());
		}

		/// <summary>
		/// Given that there are no births with gain at least 0
		/// Then GetRandomMove returns a kill move
		/// </summary>
		[TestMethod]
		public void Test6()
		{
			var random = new RandomGenerator(new Random(0));
			var board = new Board()
			{
				Field = BotParser.ParseBoard(".,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,1,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,1,.,.,.,.,.,.,0,.,0,0,.,.,.,.,.,1,1,.,.,.,.,.,.,0,0,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,0,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,0,.,.")
			};
			board.UpdateFieldCounts();
			Console.WriteLine(board.HumanBoardString());
			var simulator = new Simulator(board, random, new MonteCarloParameters());
			var move = simulator.GetRandomMove(board, Player.Player1);
			Assert.AreEqual("kill 6,12", move.ToOutputString());
		}
	}
}
