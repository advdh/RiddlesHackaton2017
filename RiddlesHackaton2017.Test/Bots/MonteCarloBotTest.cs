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
		public void Test()
		{
			var bot = new MonteCarloBot(new NullConsole(), new FirstIndexGenerator());
			bot.Board = ExampleBoard();

			var myKills = bot.GetMyKills().OrderByDescending(kvp => kvp.Value).ToList();

			Assert.AreEqual(50, myKills.Count);
			Assert.AreEqual(184, myKills[0].Key);
			Assert.AreEqual(new BoardStatus(GameStatus.Busy, 7), myKills[0].Value);
			Assert.AreEqual(new BoardStatus(GameStatus.Busy, 6), myKills[1].Value);
			Assert.AreEqual(new BoardStatus(GameStatus.Busy, 6), myKills[2].Value);
			Assert.AreEqual(new BoardStatus(GameStatus.Busy, 6), myKills[3].Value);
			Assert.AreEqual(new BoardStatus(GameStatus.Busy, 6), myKills[4].Value);
			Assert.AreEqual(new BoardStatus(GameStatus.Busy, 5), myKills[5].Value);
		}
	}
}
