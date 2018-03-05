using RiddlesHackaton2017.Bots;
using System;
using System.Linq;

namespace GeneticSimulator.Simulators
{
	public class VaryingOneSimulator : BaseSimulator, ISimulator
	{
		private ConfigurationGenerator.Parameters VaryingParameter { get; }
		private double? RelativeIncrease { get; }

		public VaryingOneSimulator(string commandLine) : base(commandLine)
		{
			string[] words = commandLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			ConfigurationGenerator.Parameters varyingParameter;
			if (words.Length > 3 && Enum.TryParse(words[3], out varyingParameter))
			{
				VaryingParameter = varyingParameter;
				double relativeIncrease;
				if (words.Length > 4 && double.TryParse(words[4], out relativeIncrease))
				{
					RelativeIncrease = relativeIncrease;
				}
			}
			else
			{
				throw new ArgumentException("Error in command: varyingParameter not specified or not recognized. Example: varyone 10 MaxDuration");
			}
		}

		public Configurations Generate(ConfigurationGenerator generator, int populationSize)
		{
			var result = new Configurations
			{
				new Configuration(MonteCarloParameters.Life)
			};
			if (RelativeIncrease.HasValue)
			{
				//Relative variation
				//Half of the population will have increased values for the varying parameter
				double factor = 1.0;
				int i = 0;
				while (result.Count - 1 < (populationSize) / 2 && i < populationSize)
				{
					i++;
					factor *= RelativeIncrease.Value;
					var newParameters = generator.Vary(MonteCarloParameters.Life, VaryingParameter, factor);
					//if (!newParameters.Equals(MonteCarloParameters.Life))
					if (!result.Any(p => newParameters.Equals(p.Parameters)))
					{
						result.Add(new Configuration(newParameters));
					}
				}
				factor = 1.0;
				i = 0;
				//Half of the population will have decreased values for the varying parameter
				while (result.Count < populationSize && i < populationSize)
				{
					i++;
					factor /= RelativeIncrease.Value;
					var newParameters = generator.Vary(MonteCarloParameters.Life, VaryingParameter, factor);
					if (!result.Any(p => newParameters.Equals(p.Parameters)))
					{
						result.Add(new Configuration(newParameters));
					}
				}
			}
			else
			{ 
				//Random variation
				for (int i = 1; i < populationSize; i++)
				{
					result.Add(new Configuration(generator.Vary(MonteCarloParameters.Life, VaryingParameter)));
				}
			}
			return result;
		}
	}
}
