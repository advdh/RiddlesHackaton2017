using RiddlesHackaton2017.Bots;

namespace GeneticSimulator.Simulators
{
	public class RandomSimulator : BaseSimulator, ISimulator
	{
		public RandomSimulator(string commandLine) : base(commandLine)
		{
		}

		public Configurations Generate(ConfigurationGenerator generator, int populationSize)
		{
			var result = new Configurations
			{
				new Configuration(MonteCarloParameters.Life)
			};
			for (int i = 1; i < populationSize; i++)
			{
				result.Add(new Configuration(generator.Generate()));
			}
			return result;
		}
	}
}
