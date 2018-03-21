using GeneticSimulator.Simulators;
using RiddlesHackaton2017.Models;
using System;

namespace GeneticSimulator
{
	class Program
	{
		static void Main(string[] args)
		{
			var generator = new ConfigurationGenerator(new Random());
			BaseSimulator simulator = null;
			int populationSize;
			int simulationCount;
			Board.InitializeFieldCountChanges();

			//Read console
			Console.Write(">");
			string commandLine = Console.ReadLine();
			while (true)
			{
				simulator = null;

				//Parse input
				if (commandLine.Length == 0) break;
				string[] words = commandLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				string command = words[0];
				populationSize = words.Length > 1 ? int.Parse(words[1]) : 2;
				simulationCount = words.Length > 2 ? int.Parse(words[2]) : 1;

				//Initialize simulator
				try
				{
					switch (command.ToLowerInvariant())
					{
						case "exit":
							return;
						case "?":
						case "help":
							Console.WriteLine("random <population size> <simulation count> (example: random 10 5): runs simulations between all pairs of the population. configuration 0 is the life MonteCarloParameters, the others are random");
							Console.WriteLine("varyone <population size> <simulation count> <varying parameter> <increase> (example: varyone 10 5 MaxDuration 1.5): runs simulations between all pairs of the population. configuration 0 is the life MonteCarloParameters, the others are variations to the specified parameter, either random (if argument increase is not specified), or increased with the specified increase rate)");
							Console.WriteLine("fromfile <not used> <simulation count> <filename> (example: fromfile 0 5 D:\\Temp\\GeneticSimulator\\foo.xml): runs simulations between all pairs of the configurations from the specified file.");
							Console.WriteLine("exit: exits the program");
							Console.WriteLine("?: displays help");
							Console.WriteLine("Help: displays help");
							break;
						case "random":
							simulator = new RandomSimulator(commandLine);
							break;
						case "varyone":
							simulator = new VaryingOneSimulator(commandLine);
							break;
						case "fromfile":
							simulator = new ConfigFromFileSimulator(commandLine);
							break;
						case "endgame":
							simulator = new EndGameSimulator(commandLine);
							break;
						default:
							Console.WriteLine($"unknown command {command}");
							break;
					}
				}
				catch (ArgumentException ex)
				{
					Console.WriteLine(ex.Message);
				}

				//Run simulator
				if (simulator != null)
				{
					var populations = simulator.Generate(generator, populationSize);
					simulator.Simulate(populations, simulationCount);
				}

				//Read console
				Console.Write(">");
				commandLine = Console.ReadLine();
			}
		}
	}
}
