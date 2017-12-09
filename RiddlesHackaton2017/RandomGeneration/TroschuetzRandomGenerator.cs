using System;
using Troschuetz.Random.Generators;

namespace RiddlesHackaton2017.RandomGeneration
{
	public class TroschuetzRandomGenerator : IRandomGenerator
	{
		private readonly Random Random1;
		private readonly MT19937Generator Random;

		public TroschuetzRandomGenerator(Random random)
		{
			Random1 = Guard.NotNull(random, "random");
		}

		private TroschuetzRandomGenerator(MT19937Generator random)
		{
			Random = Guard.NotNull(random, "random");
		}

		public IRandomGenerator Clone(int i)
		{
			return new TroschuetzRandomGenerator(new MT19937Generator(Random1.Next(1000000)));
		}

		public int Next(int maxValue)
		{
			return Random.Next(maxValue);
		}
	}
}
