using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneticSimulator.Analysis
{
	[TestClass]
	public class FromFileAnalysis
	{
		private const string Directory = @"D:\Temp\GeneticSimulator";

		[TestMethod]
		public void Results_UseFastAndSmartMoveSimulator()
		{
			AnalyzeResults(@"fromfile 0 25 UseFastAndSmartMoveSimulator_xml 2018-03-16_11_37_12.xml",
				ConfigurationGenerator.Parameters.UseFastAndSmartMoveSimulator);
		}

		[TestMethod]
		public void Results_1603()
		{
			AnalyzeResults(@"fromfile 0 25 MaxDuration_xml 2018-03-16_08_04_00.xml",
				ConfigurationGenerator.Parameters.MaxDuration);
			AnalyzeResults(@"fromfile 0 25 DoubleWinBonusCount_xml 2018-03-16_01_38_27.xml",
				ConfigurationGenerator.Parameters.DoubleWinBonusCount);
			AnalyzeResults(@"fromfile 0 25 SimulationFactor_xml 2018-03-16_02_09_51.xml",
				ConfigurationGenerator.Parameters.SimulationFactor);
			AnalyzeResults(@"fromfile 0 25 WinBonusWeight_xml 2018-03-16_01_06_57.xml",
				ConfigurationGenerator.Parameters.WinBonusWeight);
			AnalyzeResults(@"fromfile 0 25 CellCountWeight_xml 2018-03-16_12_35_59.xml",
				ConfigurationGenerator.Parameters.CellCountWeight);
			AnalyzeResults(@"fromfile 0 25 SmartMoveGenerationCount_xml 2018-03-16_12_04_30.xml",
				ConfigurationGenerator.Parameters.SmartMoveGenerationCount);
			AnalyzeResults(@"fromfile 0 25 MaxSimulationCount_xml 2018-03-15_10_11_13.xml",
				ConfigurationGenerator.Parameters.MaxSimulationCount);
			AnalyzeResults(@"fromfile 0 25 MoveCount_xml 2018-03-15_10_31_40.xml",
				ConfigurationGenerator.Parameters.MoveCount);
			AnalyzeResults(@"fromfile 0 25 SimulationMaxGenerationCount_xml 2018-03-15_11_33_09.xml",
				ConfigurationGenerator.Parameters.SimulationMaxGenerationCount);
			AnalyzeResults(@"fromfile 0 25 MaxWinBonus_xml 2018-03-15_11_01_53.xml",
				ConfigurationGenerator.Parameters.MaxWinBonus);
		}

		[TestMethod]
		public void Results_MaxDuration()
		{
			AnalyzeResults(@"fromfile 0 25 MaxDuration_xml 2018-03-17_01_26_09.xml",
				ConfigurationGenerator.Parameters.MaxDuration);
			AnalyzeResults(@"fromfile 0 25 MaxDuration_xml 2018-03-16_08_18_57.xml",
				ConfigurationGenerator.Parameters.MaxDuration);
			AnalyzeResults(@"fromfile 0 25 MaxDuration_xml 2018-03-16_07_47_11.xml",
				ConfigurationGenerator.Parameters.MaxDuration);
			AnalyzeResults(@"fromfile 0 25 MaxDuration_xml 2018-03-15_07_18_30.xml",
				ConfigurationGenerator.Parameters.MaxDuration);
			AnalyzeResults(@"fromfile 0 25 MaxDuration_xml 2018-03-16_08_04_00.xml",
				ConfigurationGenerator.Parameters.MaxDuration);
		}

		[TestMethod]
		public void Results_MaxWinBonus()
		{
			AnalyzeResults(@"varyone 9 1000 MaxWinBonus 2 2018-03-03_02_21_13.xml");
		}

		[TestMethod]
		public void Results_WinBonusDecrementFactor()
		{
			AnalyzeResults(@"varyone 9 100 WinBonusDecrementFactor 1_09 2018-03-03_10_35_57.xml");
		}

		[TestMethod]
		public void Results_WinBonusDecrementFactor_2()
		{
			AnalyzeResults(@"varyone 9 100 WinBonusDecrementFactor 1_09 2018-03-04_10_37_43.xml",
				ConfigurationGenerator.Parameters.WinBonusDecrementFactor,
				sortByScore: true);
		}

		[TestMethod]
		public void Results_MaxWinBonus2()
		{
			AnalyzeResults(@"fromfile 0 25 MaxWinBonus2_xml 2018-03-11_06_02_51.xml",
				new[] { ConfigurationGenerator.Parameters.MaxWinBonus,
					ConfigurationGenerator.Parameters.WinBonusDecrementFactor,
					ConfigurationGenerator.Parameters.MaxWinBonus2,
					ConfigurationGenerator.Parameters.WinBonusDecrementFactor2});
		}

		/// <summary>
		/// Results of battle between MaxWinbonus=128 and MaxWinbonus=2048, both with WinBonusDecrement=0.916
		/// </summary>
		[TestMethod]
		public void Results_Battle_128_2048_916()
		{
			AnalyzeResults(@"fromfile 0 100 ConfigurationsMaxWinBonus_xml 2018-03-04_04_35_39.xml", 
				ConfigurationGenerator.Parameters.MaxWinBonus);
		}

		/// <summary>
		/// Results of configurations with between MaxWinbonus=128,256,512,1024,2048, all with WinBonusDecrement=0.916
		/// </summary>
		[TestMethod]
		public void Results_Battle_128_256_512_1024_2048_916()
		{
			AnalyzeResults(@"fromfile 0 100 ConfigurationsMaxWinBonus0916_xml 2018-03-04_05_01_54.xml", 
				ConfigurationGenerator.Parameters.MaxWinBonus);
		}

		/// <summary>
		/// Results of configurations with between MaxWinbonus=64,128, both with WinBonusDecrement=0.916
		/// </summary>
		[TestMethod]
		public void Results_Battle_64_128_916()
		{
			AnalyzeResults(@"fromfile 0 100 ConfigurationsMaxWinBonus0916_64_xml 2018-03-04_09_58_47.xml",
				ConfigurationGenerator.Parameters.MaxWinBonus);
		}

		[TestMethod]
		public void Results_MaxDuration_200tm1000()
		{
			AnalyzeResults("fromfile 0 100 ConfigurationsMaxDuration_xml 2018-03-04_11_03_09.xml",
				ConfigurationGenerator.Parameters.MaxDuration);
		}

		[TestMethod]
		public void Results_MoveType()
		{
			AnalyzeResults("fromfile 0 100 ConfigurationsMoveType_xml 2018-03-05_11_35_21.xml",
				new[] { ConfigurationGenerator.Parameters.KillMovePercentage, ConfigurationGenerator.Parameters.PassMovePercentage });
		}

		[TestMethod]
		public void Results_MoveCount()
		{
			AnalyzeResults("fromfile 0 100 ConfigurationsMoveCount_xml 2018-03-05_03_23_05.xml",
				ConfigurationGenerator.Parameters.MoveCount);
		}

		/// <summary>
		/// MoveCount vs. SimulationCount
		/// </summary>
		[TestMethod]
		public void Results_MoveCountVsSimulations_2500()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations2500_xml 2018-03-08_05_49_04.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_1200()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations1200_xml 2018-03-08_02_17_06.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_0676()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations676_xml 2018-03-08_01_39_49.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_0324()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations324_xml 2018-03-08_01_19_36.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_0216()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations216_xml 2018-03-08_12_51_34.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_0144()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations144_xml 2018-03-07_10_13_57.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_0072()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations72_xml 2018-03-07_09_53_04.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_0036()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations36_xml 2018-03-07_09_47_09.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_0018()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations18_xml 2018-03-07_09_43_36.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_SimulationMaxGenerationCount()
		{
			AnalyzeResults("fromfile 0 25 SimulationMaxGenerationCount_xml 2018-03-05_10_43_15.xml",
				ConfigurationGenerator.Parameters.SimulationMaxGenerationCount);
		}

		[TestMethod]
		public void Results_SimulationMaxGenerationCount2()
		{
			AnalyzeResults("fromfile 0 25 SimulationMaxGenerationCount2_xml 2018-03-06_07_23_13.xml",
				ConfigurationGenerator.Parameters.SimulationMaxGenerationCount);
		}

		[TestMethod]
		public void Results_BinarySimulationResult()
		{
			AnalyzeResults("fromfile 0 25 BinarySimulationResult_xml 2018-03-06_11_01_10.xml",
				ConfigurationGenerator.Parameters.BinarySimulationResult);
		}

		[TestMethod]
		public void Results_BinarySimulationResult2()
		{
			AnalyzeResults("fromfile 0 25 BinarySimulationResult2_xml 2018-03-06_11_28_05.xml",
				new[] { ConfigurationGenerator.Parameters.BinarySimulationResult, ConfigurationGenerator.Parameters.SimulationMaxGenerationCount });
		}

		[TestMethod]
		public void Results_ParallelSimulation()
		{
			AnalyzeResults("fromfile 0 100 ParallelSimulation_xml 2018-03-14_07_47_44.xml",
				ConfigurationGenerator.Parameters.ParallelSimulation);
		}

		[TestMethod]
		public void Test()
		{
			AnalyzeResults("fromfile 0 1 parallelsimulation_xml 2018-03-09_10_49_45.xml",
				ConfigurationGenerator.Parameters.ParallelSimulation);
		}

		[TestMethod]
		public void Results_SimulationDecrementScore2Factor()
		{
			AnalyzeResults("fromfile 0 10 SimulationDecrementScore2Factor_xml 2018-03-07_08_57_12.xml",
				ConfigurationGenerator.Parameters.SimulationDecrementScore2Factor);
		}

		[TestMethod]
		public void Results_HardcodedSimulationCounts()
		{
			AnalyzeResults("fromfile 0 25 HardCodedSimulationCounts_xml 2018-03-08_09_16_04.xml",
				ConfigurationGenerator.Parameters.HardcodedSimulationCounts);
		}

		[TestMethod]
		public void Results_SimulationFactor()
		{
			AnalyzeResults("fromfile 0 25 SimulationFactor_xml 2018-03-08_10_25_53.xml",
				ConfigurationGenerator.Parameters.SimulationFactor);
		}

		[TestMethod]
		public void Results_SmartMoveGenerationCount()
		{
			AnalyzeResults("fromfile 0 25 SmartMoveGenerationCount_xml 2018-03-13_08_12_11.xml",
				new[] { ConfigurationGenerator.Parameters.SmartMoveGenerationCount,
					ConfigurationGenerator.Parameters.SmartMoveMinimumFieldCount});
			AnalyzeResults("fromfile 0 25 SmartMoveGenerationCount_xml 2018-03-13_10_45_51.xml",
				new[] { ConfigurationGenerator.Parameters.SmartMoveGenerationCount,
					ConfigurationGenerator.Parameters.SmartMoveMinimumFieldCount});
		}

		[TestMethod]
		public void Results_Identical()
		{
			AnalyzeResults("fromfile 0 25 Identical_xml 2018-03-13_08_49_13.xml",
				ConfigurationGenerator.Parameters.SmartMoveGenerationCount);
		}

		[TestMethod]
		public void Results_DoubleWinBonusCount()
		{
			AnalyzeResults("fromfile 0 100 DoubleWinBonusCount_xml 2018-03-14_12_13_56.xml",
				ConfigurationGenerator.Parameters.DoubleWinBonusCount);
		}

		[TestMethod]
		public void Results_ScoreBasedOnWinBonus()
		{
			AnalyzeResults("fromfile 0 25 ScoreBasedOnWinBonus_xml 2018-03-11_07_28_53.xml",
				ConfigurationGenerator.Parameters.ScoreBasedOnWinBonus);
		}

		[TestMethod]
		public void Results_MaxWinBonus2_64_128()
		{
			AnalyzeResults("fromfile 0 25 MaxWinBonus2_xml 2018-03-11_08_32_42.xml",
				new[] { ConfigurationGenerator.Parameters.MaxWinBonus,
					ConfigurationGenerator.Parameters.WinBonusDecrementFactor,
					ConfigurationGenerator.Parameters.MaxWinBonus2,
					ConfigurationGenerator.Parameters.WinBonusDecrementFactor2});
		}

		[TestMethod]
		public void Results_WinBonusDecrementFactor_3()
		{
			AnalyzeResults(@"fromfile 0 25 WinBonusDecrementFactor_xml 2018-03-11_10_46_55.xml",
				ConfigurationGenerator.Parameters.WinBonusDecrementFactor);
		}

		[TestMethod]
		public void Results_WinBonusDecrementFactor_93_94()
		{
			AnalyzeResults(@"fromfile 0 25 WinBonusDecrementFactor_xml 2018-03-12_03_35_48.xml",
				ConfigurationGenerator.Parameters.WinBonusDecrementFactor);
		}

		[TestMethod]
		public void Results_WinBonusDecrementFactor_94_95()
		{
			AnalyzeResults(@"fromfile 0 25 WinBonusDecrementFactor_xml 2018-03-12_04_06_19.xml",
				ConfigurationGenerator.Parameters.WinBonusDecrementFactor);
		}

		[TestMethod]
		public void Results_WinBonusDecrementFactor_95_96()
		{
			AnalyzeResults(@"fromfile 0 25 WinBonusDecrementFactor_xml 2018-03-12_04_37_26.xml",
				ConfigurationGenerator.Parameters.WinBonusDecrementFactor);
		}

		private void AnalyzeResults(string path)
		{
			string filename = Path.Combine(Directory, path);
			var results = Configurations.Load(filename)
				.OrderByDescending(r => r.Won - r.Lost)
				.ThenByDescending(r => r.Won);
			int ix = 1;
			foreach (var result in results)
			{
				Console.WriteLine($"{ix}.\tCount={result.Count}\tWon={result.Won}\tDraw={result.Draw}\tLost={result.Lost}\n{result.Parameters}");
				ix++;
			}
		}

		private void AnalyzeResults(string path, ConfigurationGenerator.Parameters varyingParameter, bool sortByScore = false)
		{
			AnalyzeResults(path, new[] { varyingParameter }, sortByScore);
		}

		private void AnalyzeResults(string path, IEnumerable<ConfigurationGenerator.Parameters> varyingParameters, bool sortByScore = false)
		{
			string filename = Path.Combine(Directory, path);
			var results = Configurations.Load(filename).OrderByDescending(r => r.AverageScore1);
			if (sortByScore)
			{
				results = results.OrderByDescending(r => r.Won - r.Lost);
			}

			int ix = 1;

			//Display results by total rank including the specified properties
			foreach (var result in results)
			{
				Console.WriteLine($"{ix}.\tCount={result.Count}\tWon={result.Won}\tDraw={result.Draw}\tLost={result.Lost}\tTotalScore={result.TotalScore1:0.000}\tAverageScore={result.AverageScore1:0.000}");
				foreach(var parm in varyingParameters)
				{
					var propertyValue = GetPropValue(result.Parameters, parm.ToString());
					Console.WriteLine($"\t{parm.ToString()} = {propertyValue}");
				}
				ix++;
			}

			//Display results by pair
			ix = 1;
			foreach(var result in results)
			{
				int ix2 = 1;
				foreach(var result2 in results)
				{
					if (!result2.Parameters.Equals(result.Parameters))
					{
						var games = result.Results1
							.Where(r => r.Parameters2Hash == result2.Parameters.GetHashCode());
						Console.WriteLine("{0} - {1} ({2}): {3} - {4} - {5} (score {6:0.000} = average {7:0.000})",
							ix, 
							ix2,
							games.Count(),
							games.Count(g => g.Winner == Player.Player1),
							games.Count(g => g.Winner == null),
							games.Count(g => g.Winner == Player.Player2),
							games.Sum(g => g.Player1Score),
							games.Any() ? games.Average(g => g.Player1Score) : 0);
					}
					ix2++;
				}
				ix++;
			}
		}

		public static object GetPropValue(object src, string propName)
		{
			return src.GetType().GetProperty(propName).GetValue(src, null);
		}
	}
}
