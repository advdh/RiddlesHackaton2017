using RiddlesHackaton2017.Bots;
using System;
using System.IO;
using System.Xml.Serialization;

namespace GeneticSimulator
{
	class Program
	{
		static void Main(string[] args)
		{
			int n = 100;
			var path = @"d:\temp\GeneticSimulator.xml";
			var generator = new GenerationRandomizer(new Random());
			var list = new Generations();
			list.Add(new Generation(MonteCarloParameters.Life));
			for (int i = 1; i < n; i++)
			{
				list.Add(new Generation(generator.Generate()));
			}

			//Play
			var gameRunner = new GameRunner(new Random());
			for(int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					if (i != j)
					{
						var result = gameRunner.Run(list[i].Parameters, list[j].Parameters);
						list[i].Results1.Add(result);
						list[j].Results2.Add(result);
						Console.WriteLine($"{i} - {j}: {result}");
						list.Save(path);
					}
				}
			}


			Console.ReadLine();
		}
	}
}
