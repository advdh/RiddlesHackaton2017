using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using System;

namespace GeneticSimulator.Analysis
{
	[TestClass]
	public class LogRoundTest
	{
		private const double Delta = 0.00001;

		[TestMethod]
		public void Test()
		{
			string s = @"Round 1: Birthmove (15,9), sacrifice = (14,0) and (0,5) (40-40, gain2 = 150): score = 62 %, score2 = 1552, moves = 100 (67), simulations = 25, win in 8.00, loose in Infinity - Used 751 ms, Timelimit 10000 ms, Start 45.129, End 45.880";
			var logRound = LogRound.Parse(s, 52);
			Assert.IsNotNull(logRound);
			Assert.AreEqual(1, logRound.Round, "Round");
			Assert.AreEqual(new BirthMove(new Position(15, 9), new Position(14, 0), new Position(0, 5)), logRound.move, "Move");
			Assert.AreEqual(40, logRound.MyPlayerFieldCount, "MyPlayerFieldCount");
			Assert.AreEqual(40, logRound.OpponentPlayerFieldCount, "OpponentPlayerFieldCount");
			Assert.AreEqual(150, logRound.Gain2, "Gain2");
			Assert.AreEqual(0.62, logRound.Score, "Score");
			Assert.AreEqual(1552, logRound.Score2, "Score2");
			Assert.AreEqual(100, logRound.MoveCount, "MoveCount");
			Assert.AreEqual(67, logRound.BestMoveIndex, "BestMoveIndex");
			Assert.AreEqual(25, logRound.SimulationCount, "SimulationCount");
			Assert.AreEqual(8, logRound.WinRounds, "WinRounds");
			Assert.AreEqual(double.PositiveInfinity, logRound.LooseRounds, "LooseRounds");
			Assert.AreEqual(751, logRound.UsedTime.TotalMilliseconds, "UsedTime");
			Assert.AreEqual(10000, logRound.TimeLimit.TotalMilliseconds, "TimeLimit");
			Assert.AreEqual(45.129, logRound.StartTime.Subtract(DateTime.MinValue).TotalSeconds, Delta, "StartTime");
			Assert.AreEqual(45.880, logRound.EndTime.Subtract(DateTime.MinValue).TotalSeconds, Delta, "EndTime");
		}
	}
}
