using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneticSimulator
{
	[TestClass]
	public class Analysis
	{
		private const string Directory = @"D:\Temp\GeneticSimulator";

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
			AnalyzeResults(@"varyone 9 100 WinBonusDecrementFactor 1_09 2018-03-04_10_37_43.xml");
		}

		[TestMethod]
		public void Results_FromFile_128_2048_0707_916()
		{
			AnalyzeResults(@"fromfile 1282048 100 Configurations_xml 2018-03-04_02_50_57.xml",
				new[] { ConfigurationGenerator.Parameters.MaxWinBonus, ConfigurationGenerator.Parameters.WinBonusDecrementFactor });
		}

		/// <summary>
		/// Results of battle between MaxWinbonus=128 and MaxWinbonus=2048, both with WinBonusDecrement=0.916
		/// </summary>
		[TestMethod]
		public void Results_Battle_128_2048_916()
		{
			AnalyzeResults(@"fromfile 0 100 ConfigurationsMaxWinBonus_xml 2018-03-04_04_35_39.xml", 
				new[] { ConfigurationGenerator.Parameters.MaxWinBonus });
		}

		/// <summary>
		/// Results of configurations with between MaxWinbonus=128,256,512,1024,2048, all with WinBonusDecrement=0.916
		/// </summary>
		[TestMethod]
		public void Results_Battle_128_256_512_1024_2048_916()
		{
			AnalyzeResults(@"fromfile 0 100 ConfigurationsMaxWinBonus0916_xml 2018-03-04_05_01_54.xml", 
				new[] { ConfigurationGenerator.Parameters.MaxWinBonus });
		}

		/// <summary>
		/// Results of configurations with between MaxWinbonus=64,128, both with WinBonusDecrement=0.916
		/// </summary>
		[TestMethod]
		public void Results_Battle_64_128_916()
		{
			AnalyzeResults(@"fromfile 0 100 ConfigurationsMaxWinBonus0916_64_xml 2018-03-04_09_58_47.xml",
				new[] { ConfigurationGenerator.Parameters.MaxWinBonus });
		}

		[TestMethod]
		public void Results_MaxDuration_200tm1000()
		{
			AnalyzeResults("fromfile 0 100 ConfigurationsMaxDuration_xml 2018-03-04_11_03_09.xml",
				new[] { ConfigurationGenerator.Parameters.MaxDuration });
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
				new[] { ConfigurationGenerator.Parameters.MoveCount });
		}

		/// <summary>
		/// MoveCount vs. SimulationCount
		/// </summary>
		[TestMethod]
		public void Results_MoveCountVsSimulations_676()
		{
			AnalyzeResults("fromfile 0 100 ConfigurationsMoveCountVsSimulations676_xml 2018-03-05_04_28_09.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_324()
		{
			AnalyzeResults("fromfile 0 100 ConfigurationsMoveCountVsSimulations324_xml 2018-03-05_07_08_11.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_216()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations216_xml 2018-03-05_10_20_33.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_144()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations144_xml 2018-03-05_09_18_06.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_072()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations72_xml 2018-03-05_09_48_23.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_036()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations36_xml 2018-03-05_10_00_14.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_MoveCountVsSimulations_018()
		{
			AnalyzeResults("fromfile 0 25 ConfigurationsMoveCountVsSimulations18_xml 2018-03-05_10_13_39.xml",
				new[] { ConfigurationGenerator.Parameters.MoveCount, ConfigurationGenerator.Parameters.StartSimulationCount });
		}

		[TestMethod]
		public void Results_SimulationMaxGenerationCount()
		{
			AnalyzeResults("fromfile 0 25 SimulationMaxGenerationCount_xml 2018-03-05_10_43_15.xml",
				new[] { ConfigurationGenerator.Parameters.SimulationMaxGenerationCount });
		}

		[TestMethod]
		public void Results_SimulationMaxGenerationCount2()
		{
			AnalyzeResults("fromfile 0 25 SimulationMaxGenerationCount2_xml 2018-03-06_07_23_13.xml",
				new[] { ConfigurationGenerator.Parameters.SimulationMaxGenerationCount });
		}

		[TestMethod]
		public void Results_BinarySimulationResult()
		{
			AnalyzeResults("fromfile 0 25 BinarySimulationResult_xml 2018-03-06_11_01_10.xml",
				new[] { ConfigurationGenerator.Parameters.BinarySimulationResult });
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
			AnalyzeResults("fromfile 0 25 ParallelSimulation_xml 2018-03-07_08_12_49.xml",
				new[] { ConfigurationGenerator.Parameters.ParallelSimulation });
		}

		[TestMethod]
		public void Results_SimulationDecrementScore2Factor()
		{
			AnalyzeResults("fromfile 0 10 SimulationDecrementScore2Factor_xml 2018-03-07_08_57_12.xml",
				new[] { ConfigurationGenerator.Parameters.SimulationDecrementScore2Factor });
		}

		private void AnalyzeResults(string path)
		{
			string filename = Path.Combine(Directory, path);
			var results = Configurations.Load(filename)
				.OrderByDescending(r => 3 * r.Won + r.Draw)
				.ThenByDescending(r => r.Won);
			int ix = 1;
			foreach (var result in results)
			{
				Console.WriteLine($"{ix}.\tCount={result.Count}\tWon={result.Won}\tDraw={result.Draw}\tLost={result.Lost}\n{result.Parameters}");
				ix++;
			}
		}

		private void AnalyzeResults(string path, IEnumerable<ConfigurationGenerator.Parameters> varyingParameters)
		{
			string filename = Path.Combine(Directory, path);
			var results = Configurations.Load(filename)
				.OrderByDescending(r => 3 * r.Won + r.Draw)
				.ThenByDescending(r => r.Won);
			int ix = 1;

			//Display results by total rank including the specified properties
			foreach (var result in results)
			{
				Console.WriteLine($"{ix}.\tCount={result.Count}\tWon={result.Won}\tDraw={result.Draw}\tLost={result.Lost}");
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
						Console.WriteLine("{0} - {1} ({2}): {3} - {4} - {5}",
							ix, 
							ix2,
							games.Count(),
							games.Count(g => g.Winner == Player.Player1),
							games.Count(g => g.Winner == null),
							games.Count(g => g.Winner == Player.Player2));
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
