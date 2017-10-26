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

		public int Next(int maxValue)
		{
			return Random.Next(maxValue);
		}
	}
}
