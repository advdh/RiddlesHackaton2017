using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.MoveGeneration;
using RiddlesHackaton2017.Moves;
using System.Collections.Generic;
using System.Linq;

namespace RiddlesHackaton2017.IntegrationTest.MoveGeneration
{
	[TestClass]
	public class MoveGenerator2Test : IntegrationTestBase
	{
		[TestMethod]
		public void GetBirths_Test()
		{
			var parameters = MonteCarloParameters.Life;
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4, Player.Player1);
			var generator = new MoveGenerator2(board, parameters);
			var births = generator.GetBirths(generationCount: 8, top: 2);
			Assert.AreEqual(2, births.Count(), "count");
			Assert.AreEqual(new Position(7, 6), new Position(births.First()), "first");
			Assert.AreEqual(new Position(4, 10), new Position(births.Last()), "last");
		}

		[TestMethod]
		public void GetBirths_AsPlayer2_Test()
		{
			var parameters = MonteCarloParameters.Life;
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4, Player.Player2);
			var generator = new MoveGenerator2(board, parameters);
			var births = generator.GetBirths(generationCount: 8, top: 2);
			Assert.AreEqual(2, births.Count(), "count");
			Assert.AreEqual(new Position(5, 10), new Position(births.First()), "first");
			Assert.AreEqual(new Position(11,4), new Position(births.Last()), "last");
		}

		[TestMethod]
		public void GetKills_Test()
		{
			var parameters = MonteCarloParameters.Life;
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4, Player.Player1);
			var generator = new MoveGenerator2(board, parameters);
			var kills = generator.GetKills(generationCount: 8, top: 2);
			Assert.AreEqual(2, kills.Count(), "count");
			Assert.AreEqual(new Position(1, 6), new Position(kills.First()), "first");
			Assert.AreEqual(new Position(0, 6), new Position(kills.Last()), "last");
		}

		[TestMethod]
		public void GetKills_AsPlayer2_Test()
		{
			var parameters = MonteCarloParameters.Life;
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4, Player.Player2);
			var generator = new MoveGenerator2(board, parameters);
			var kills = generator.GetKills(generationCount: 8, top: 2);
			Assert.AreEqual(2, kills.Count(), "count");
			Assert.AreEqual(new Position(6, 12), new Position(kills.First()), "first");
			Assert.AreEqual(new Position(5, 13), new Position(kills.Last()), "last");
		}

		[TestMethod]
		public void GetKills_Round1_Red()
		{
			var parameters = MonteCarloParameters.Life;
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 1, Player.Player1);
			var generator = new MoveGenerator2(board, parameters);
			var kills = generator.GetKills(generationCount: 8, top: 2);
		}

		[TestMethod]
		public void GetKills_Round1_Blue()
		{
			var parameters = MonteCarloParameters.Life;
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 1, Player.Player1);
			board.MyPlayer = Player.Player2;
			var generator = new MoveGenerator2(board, parameters);
			var kills = generator.GetKills(generationCount: 8, top: 2);
		}

		[TestMethod]
		public void GetOpponentKills_Test()
		{
			var parameters = MonteCarloParameters.Life;
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4, Player.Player1);
			var generator = new MoveGenerator2(board, parameters);
			var kills = generator.GetOpponentKills(generationCount: 8, top: 2);
			Assert.AreEqual(2, kills.Count(), "count");
			Assert.AreEqual(new Position(7, 7), new Position(kills.First()), "first");
			Assert.AreEqual(new Position(7, 8), new Position(kills.Last()), "last");
		}

		[TestMethod]
		public void GetOpponentKills_AsPlayer2_Test()
		{
			var parameters = MonteCarloParameters.Life;
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4, Player.Player2);
			var generator = new MoveGenerator2(board, parameters);
			var kills = generator.GetOpponentKills(generationCount: 8, top: 2);
			Assert.AreEqual(2, kills.Count(), "count");
			Assert.AreEqual(new Position(5, 5), new Position(kills.First()), "first");
			Assert.AreEqual(new Position(5, 8), new Position(kills.Last()), "last");
		}

		[TestMethod]
		public void CombineBirthsAndMoves_Test()
		{
			var births = new List<int>(new[] { 1, 2 });
			var kills = new List<int>(new[] { 3, 4, 5 });
			var generator = new MoveGenerator2(new Board(), MonteCarloParameters.Life);
			var moves = generator.CombineBirthsAndMoves(births, kills).ToList();
			Assert.AreEqual(6, moves.Count, "count");
			Assert.IsTrue(moves.Contains(new BirthMove(1, 3, 4)));
			Assert.IsTrue(moves.Contains(new BirthMove(1, 3, 5)));
			Assert.IsTrue(moves.Contains(new BirthMove(1, 4, 5)));
			Assert.IsTrue(moves.Contains(new BirthMove(2, 3, 4)));
			Assert.IsTrue(moves.Contains(new BirthMove(2, 3, 5)));
			Assert.IsTrue(moves.Contains(new BirthMove(2, 4, 5)));
			Assert.IsFalse(moves.Contains(new BirthMove(1, 3, 3)));
		}

		[TestMethod]
		public void GetMoves_Test()
		{
			var parameters = MonteCarloParameters.Life;
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4, Player.Player1);
			var generator = new MoveGenerator2(board, parameters);
			var moves = generator.GetMoves();
			Assert.AreEqual(41, moves.Count(), "count");
		}
	}
}