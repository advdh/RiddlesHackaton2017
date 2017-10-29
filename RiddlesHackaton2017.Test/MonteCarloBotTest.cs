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
		/// <summary>
		/// Use board from game de4f1949-1c06-46b2-afb8-bfd25ff3fc40
		/// https://starapple.riddles.io/competitions/game-of-life-and-death/matches/de4f1949-1c06-46b2-afb8-bfd25ff3fc40
		/// </summary>
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
