using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiddlesHackaton2017.IntegrationTest
{
	[TestClass]
	public class GameLogGeneratorTest
	{
		private const string GamesFolder = @"D:\Ad\Golad\Games";

		[TestMethod]
		public void AsPlayer0()
		{
			var gameId = "76c85d7d-4c0c-48bf-9d6b-5fce8dd03445";
			var fromFile = File.ReadAllLines(Path.Combine(GamesFolder, gameId + ".txt"));
			string[] fromGameData;
			using (var database = new Database())
			{
				database.Connect();
				var gameData = database.GetGameData(gameId);
				fromGameData = GameLogGenerator.GenerateCommandLines(gameData, Player.Player1).ToArray();
			}

			for (int i = 0; i < Math.Min(fromFile.Length, fromGameData.Length); i++)
			{
				Assert.AreEqual(fromFile[i], fromGameData[i], i.ToString());
			}
			if (fromFile.Length < fromGameData.Length)
			{
				Assert.Fail("Too many lines: first: {0}", fromGameData[fromFile.Length]);
			}
			if (fromFile.Length < fromGameData.Length)
			{
				Assert.Fail("Too few lines: first: {0}", fromFile[fromFile.Length]);
			}
		}

		[TestMethod]
		public void AsPlayer1()
		{
			var gameId = "64290360-8630-4091-8310-92b82ebc7103";
			var fromFile = File.ReadAllLines(Path.Combine(GamesFolder, gameId + ".txt"));
			string[] fromGameData;
			using (var database = new Database())
			{
				database.Connect();
				var gameData = database.GetGameData(gameId);
				fromGameData = GameLogGenerator.GenerateCommandLines(gameData, Player.Player2).ToArray();
			}

			for (int i = 0; i < Math.Min(fromFile.Length, fromGameData.Length); i++)
			{
				Assert.AreEqual(fromFile[i], fromGameData[i], i.ToString());
			}
			if (fromFile.Length < fromGameData.Length)
			{
				Assert.Fail("Too many lines: first: {0}", fromGameData[fromFile.Length]);
			}
			if (fromFile.Length < fromGameData.Length)
			{
				Assert.Fail("Too few lines: first: {0}", fromFile[fromFile.Length]);
			}
		}
	}
}
