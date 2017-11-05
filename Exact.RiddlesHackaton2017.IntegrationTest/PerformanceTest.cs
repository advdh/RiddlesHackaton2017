using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using RiddlesHackaton2017.Test;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiddlesHackaton2017.IntegrationTest
{
	[TestClass]
	public class PerformanceTest : TestBase
	{
		[TestMethod]
		public void Test()
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
				bot.GetMyKills();
			}
			TimeSpan improved = stopwatch.Elapsed;


			Console.WriteLine($"Old: {old}, Improved: {improved} ({(1 - (double)improved.Ticks / old.Ticks):P0} better)");
		}
	}
}
