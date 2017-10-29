using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using System;

namespace RiddlesHackaton2017.Test
{
	[TestClass]
	public class MonteCarloBotTest : TestBase
	{
		[TestMethod]
		public void MonteCarlo_Test()
		{
			var bot = new MonteCarloBot(new TheConsole(), new RandomGenerator(new Random(0)))
			{
				Parameters = new MonteCarloParameters()
				{
					SimulationCount = 10,
					MoveCount = 10,
					LogAllMoves = true,
				}
			};
			var board = ExampleBoard();

			var move = bot.GetMove(board);
		}
	}
}
