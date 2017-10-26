using System;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;

namespace RiddlesHackaton2017.Bots
{
	/// <summary>
	/// Helper bot for monte carlo: plays simple moves for the rest of the game
	/// </summary>
	public class MonteCarloRestOfGameBot : BaseBot
	{
		private readonly IRandomGenerator RandomGenerator;

		public MonteCarloRestOfGameBot(IConsole consoleError, IRandomGenerator randomGenerator) : base(consoleError)
		{
			RandomGenerator = Guard.NotNull(randomGenerator, nameof(randomGenerator));
		}

		public override int GetMove()
		{
			throw new NotImplementedException();
		}
	}
}
