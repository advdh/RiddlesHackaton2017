using RiddlesHackaton2017.Bots;
using System;

namespace GeneticSimulator
{
	class Program
	{
		static void Main(string[] args)
		{
			int n = 2;
			var path = @"d:\temp\GeneticSimulator.xml";
			var generator = new ConfigurationGenerator(new Random());
			var configurations = new Configurations();
			configurations.Add(new Configuration(MonteCarloParameters.Life));
			for (int i = 1; i < n; i++)
			{
				configurations.Add(new Configuration(generator.Generate()));
			}

			//Play
			var gameRunner = new GameRunner(new Random());
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					if (i != j)
					{
						var result = gameRunner.Run(configurations[i].Parameters, configurations[j].Parameters);
						configurations[i].Results1.Add(result);
						configurations[j].Results2.Add(result);
						Console.WriteLine($"{i} - {j}: {result}");
						configurations.Save(path);
					}
				}
			}

			Console.ReadLine();
		}
	}
}
