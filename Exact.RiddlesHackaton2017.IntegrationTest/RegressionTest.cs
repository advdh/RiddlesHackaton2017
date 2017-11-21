﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using RiddlesHackaton2017.Test;
using System;

namespace RiddlesHackaton2017.IntegrationTest
{
	[TestClass]
	public class RegressionTest : TestBase
	{
		/// <summary>
		/// v29, UnmanagedCode, c8d3239e-2688-4b02-99b9-888f17afc009
		/// Round 17: should do a birth 10,0 11,0 12,0
		/// </summary>
		[TestMethod]
		public void Test()
		{
			var board = InitBoard(Player.Player1, ".,.,.,.,.,.,.,.,.,0,.,0,0,.,.,.,.,.,.,.,.,.,.,.,.,.,.,0,.,.,.,.,.,.,.,.,.,.,.,.,1,1,.,.,.,.,.,.,.,.,1,1,1,.,.,.,.,1,.,.,1,.,.,.,.,0,.,1,.,1,1,.,.,.,.,.,1,.,.,.,.,.,.,.,1,1,.,.,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,1,.,.,.,.,.,.,.,.,.,.,.,.,1,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,1,.,.,.,1,1,.,.,.,.,.,.,.,.,.,.,1,1,.,.,.,.,1,1,.,.,.,.,.,.,.,.,.,1,1,.,.,1,.,.,1,1,.,.,.,.,.,.,.,.,.,.,.,.,.,1,.,1,.,.,.,.,.,.,.,.,.,.,.,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,.,.,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,1,1,1,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.,.");
			board.Round = 17;
			var bot = new MonteCarloBot(new TheConsole(), new RandomGenerator(new Random(0)));
			var move = bot.GetMove(board, TimeSpan.FromMilliseconds(758));
			Assert.AreEqual("birth 10,0 11,0 12,0", move.ToString());
		}
	}
}