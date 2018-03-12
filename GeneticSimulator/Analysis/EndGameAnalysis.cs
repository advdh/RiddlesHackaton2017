using GeneticSimulator.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneticSimulator.Analysis
{
	[TestClass]
	public class EndGameAnalysis
	{
		private const string Directory = @"D:\Temp\GeneticSimulator";

		[TestMethod]
		public void EndGame_SmartMoveGenerationCount()
		{
			var varyingParameters = new[] { ConfigurationGenerator.Parameters.SmartMoveGenerationCount };
			AnalyzeResults(1, @"endgame 1 1000 SmartMoveGenerationCount_xml 2018-03-11_02_49_05.xml", varyingParameters);
			AnalyzeResults(2, @"endgame 2 1000 SmartMoveGenerationCount_xml 2018-03-11_02_55_39.xml", varyingParameters);
			AnalyzeResults(3, @"endgame 3 1000 SmartMoveGenerationCount_xml 2018-03-11_03_05_13.xml", varyingParameters);
			AnalyzeResults(4, @"endgame 4 1000 SmartMoveGenerationCount_xml 2018-03-11_03_17_57.xml", varyingParameters);
			AnalyzeResults(5, @"endgame 5 1000 SmartMoveGenerationCount_xml 2018-03-11_03_40_59.xml", varyingParameters);
			AnalyzeResults(6, @"endgame 6 1000 SmartMoveGenerationCount_xml 2018-03-11_04_06_44.xml", varyingParameters);
			AnalyzeResults(7, @"endgame 7 1000 SmartMoveGenerationCount_xml 2018-03-11_04_46_44.xml", varyingParameters);
			AnalyzeResults(8, @"endgame 8 1000 SmartMoveGenerationCount_xml 2018-03-12_01_51_27.xml", varyingParameters);
			AnalyzeResults(9, @"endgame 9 1000 SmartMoveGenerationCount_xml 2018-03-12_03_24_04.xml", varyingParameters);
			AnalyzeResults(10, @"endgame 10 1000 SmartMoveGenerationCount_xml 2018-03-12_05_44_13.xml", varyingParameters);
		}

		private void AnalyzeResults(int loosingFieldCount, string path, IEnumerable<ConfigurationGenerator.Parameters> varyingParameters)
		{
			string filename = Path.Combine(Directory, path);
			var endgameResults = EndGameResults.Load(filename);
			int ix = 1;

			Console.WriteLine($"Loosing field count = {loosingFieldCount}");

			//Display configurations with the specified properties
			foreach (var configuration in endgameResults.Configurations)
			{
				Console.Write($"{ix}. ");
				foreach (var parm in varyingParameters)
				{
					var propertyValue = GetPropValue(configuration.Parameters, parm.ToString());
					Console.WriteLine($"\t{parm.ToString()} = {propertyValue}");
				}
				ix++;
			}
			Console.WriteLine();

			//Display results by winner
			for (int i = 0; i < endgameResults.Configurations.Count(); i++)
			{
				var results = endgameResults.Results.Where(r => r.WinningId == i);
				int won = results.Count(r => r.Won.HasValue && r.Won.Value);
				int count = results.Count();
				int totalRounds = results.Sum(r => r.EndRound - r.StartRound);
				double avgRounds = results.Average(r => r.EndRound - r.StartRound);
				int minRounds = results.Min(r => r.EndRound - r.StartRound);
				int maxRounds = results.Max(r => r.EndRound - r.StartRound);
				Console.WriteLine($"Winner {i+1}: count = {count}, won = {won}, lost = {count - won}, rounds = {totalRounds} (avg. {avgRounds:0.00}), min {minRounds}, max {maxRounds}");
			}
			Console.WriteLine();

			//Display results by looser
			for (int j = 0; j < endgameResults.Configurations.Count(); j++)
			{
				var results = endgameResults.Results.Where(r => r.LoosingId == j);
				int won = results.Count(r => r.Won.HasValue && r.Won.Value);
				int count = results.Count();
				int totalRounds = results.Sum(r => r.EndRound - r.StartRound);
				double avgRounds = results.Average(r => r.EndRound - r.StartRound);
				int minRounds = results.Min(r => r.EndRound - r.StartRound);
				int maxRounds = results.Max(r => r.EndRound - r.StartRound);
				Console.WriteLine($"Looser {j+1}: count = {count}, won = {count - won}, lost = {won}, rounds = {totalRounds} (avg. {avgRounds:0.00}), min {minRounds}, max {maxRounds}");
			}
			Console.WriteLine();

			//Display results by pair
			for (int i = 0; i < endgameResults.Configurations.Count(); i++)
			{
				for (int j = 0; j < endgameResults.Configurations.Count(); j++)
				{
					var results = endgameResults.Results.Where(r => r.WinningId == i && r.LoosingId == j);
					int won = results.Count(r => r.Won.HasValue && r.Won.Value);
					int count = results.Count();
					int totalRounds = results.Sum(r => r.EndRound - r.StartRound);
					double avgRounds = results.Average(r => r.EndRound - r.StartRound);
					int minRounds = results.Min(r => r.EndRound - r.StartRound);
					int maxRounds = results.Max(r => r.EndRound - r.StartRound);
					Console.WriteLine($"{i+1} - {j+1}: count = {count}, won = {won}, rounds = {totalRounds} (avg. {avgRounds:0.00}), min {minRounds}, max {maxRounds}");
				}
			}
			Console.WriteLine();
			Console.WriteLine();
		}

		public static object GetPropValue(object src, string propName)
		{
			return src.GetType().GetProperty(propName).GetValue(src, null);
		}
	}
}
