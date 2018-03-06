using System;
using System.IO;

namespace GeneticSimulator.Simulators
{
	public abstract class BaseSimulator
	{
		protected const string Directory = @"D:\Temp\GeneticSimulator";
		protected string Filename { get; private set; }

		protected BaseSimulator(string commandLine)
		{
			Filename = Path.Combine(Directory, string.Format("{0} {1:yyyy-MM-dd_hh_mm_ss}.xml",
				commandLine.Replace(".", "_"),
				DateTime.Now));
		}

		public abstract Configurations Generate(ConfigurationGenerator generator, int populationSize);

		public virtual void Simulate(Configurations configurations, int simulationCount)
		{
			configurations.Save(Filename);

			int populationSize = configurations.Count;

			//Play
			var gameRunner = new GameRunner(new Random());
			for (int n = 0; n < simulationCount; n++)
			{
				Console.WriteLine($"Start simulation {n + 1} / {simulationCount}");

				for (int i = 0; i < populationSize; i++)
				{
					for (int j = 0; j < populationSize; j++)
					{
						if (i != j)
						{
							var result = gameRunner.Run(configurations[i].Parameters, configurations[j].Parameters);
							configurations[i].Results1.Add(result);
							configurations[j].Results2.Add(result);
							Console.WriteLine($"{i} - {j}: {result}");
							configurations.Save(Filename);
						}
					}
				}
			}
		}
	}
}
