using System;

namespace RiddlesHackaton2017.RandomGeneration
{
	public class RandomGenerator : IRandomGenerator
	{
		private readonly Random Random;

		public RandomGenerator(Random random)
		{
			Random = Guard.NotNull(random, "random");
		}

		public IRandomGenerator Clone(int i)
		{
			return new RandomGenerator(new Random(Random.Next(1000000)));
		}

		public int Next(int maxValue)
		{
			return Random.Next(maxValue);
		}
	}
}
