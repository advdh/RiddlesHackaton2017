using RiddlesHackaton2017.Bots;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticSimulator.Simulators
{
	public class ConfigFromFileSimulator2 : BaseSimulator
	{
		protected ConfigFromFileSimulator2(string commandLine) : base(commandLine)
		{
		}

		private string AllConfigurationsFile
		{
			get
			{
				return Path.Combine(Directory, "Results", "AllConfigurations.xml");
			}
		}

		private string AllResultsFile
		{
			get
			{
				return Path.Combine(Directory, "Results", "AllResults.xml");
			}
		}

		public override Configurations Generate(ConfigurationGenerator generator, int populationSize)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// All historic results between any pair of configurations
		/// </summary>
		private GameResults AllResults;

		/// <summary>
		/// All historic results between any pair of configurations
		/// </summary>
		private GameResults AllRelevantResults;

		private GameRunner GameRunner;

		public override void Simulate(Configurations configurations, int simulationCount)
		{
			//Add any new configurations to AllConfigurations file
			var allConfigurations = Configurations.Load(AllConfigurationsFile, createIfNotExists: true);
			foreach(var configuration in configurations)
			{
				if (!allConfigurations.Any(c => c.Equals(configuration)))
				{
					allConfigurations.Add(configuration);
				}
			}
			allConfigurations.Save(AllConfigurationsFile);

			//Read all results
			AllResults = GameResults.Load(AllResultsFile);
			var hashes = configurations.Select(c => c.Parameters.HashCode);
			AllRelevantResults = (GameResults)AllResults
				.Where(r => hashes.Contains(r.Parameters1Hash) && hashes.Contains(r.Parameters2Hash))
				.ToList();

			//Initialize counts
			int populationSize = configurations.Count;
			var pairs = new Dictionary<Tuple<int, int>, int>();
			for (int i = 0; i < populationSize; i++)
			{
				for (int j = 0; j < populationSize; j++)
				{
					if (i != j)
					{
						var key = new Tuple<int, int>(i, j);
						int value = AllRelevantResults
							.Count(r => r.Parameters1Hash == configurations[i].GetHashCode()
									&& r.Parameters2Hash == configurations[j].GetHashCode());
						pairs.Add(key, value);
					}
				}
			}

			GameRunner = new GameRunner(new Random());

			//Find next pair of configurations to simulate
			var pair = pairs.OrderByDescending(p => p.Value).ThenBy(p => p.Key.Item1).ThenBy(p => p.Key.Item2).First();

			while (pair.Value < simulationCount)
			{
				//Simulate
				var parm1 = pair.Key.Item1;
				var parm2 = pair.Key.Item2;
				var result = GameRunner.Run(configurations.ElementAt(parm1).Parameters, configurations.ElementAt(parm2).Parameters);

				//Add result to AllResults and save
				AllResults.Add(result);
				AllResults.Save(AllResultsFile);

				//Add result to AllRelevantResults
				AllRelevantResults.Add(result);

				//Update counts
				pairs[new Tuple<int, int>(parm1, parm2)]++;
				Console.WriteLine($"{parm1} - {parm2}: {result}");
			}
		}
	}
}
