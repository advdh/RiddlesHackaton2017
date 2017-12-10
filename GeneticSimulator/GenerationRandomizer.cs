using RiddlesHackaton2017.Bots;
using System;

namespace GeneticSimulator
{
	public class GenerationRandomizer
	{
		private Random Random;

		public GenerationRandomizer(Random random)
		{
			Random = random;
		}

		private int Range(int min, int max)
		{
			if (min <= 0) return (int)Math.Exp(Range(0.0, Math.Log((double)max)));
			return (int)Math.Exp(Range(Math.Log((double)min), Math.Log((double)max)));
		}

		private int RangeRelative(int min, int max)
		{
			return Random.Next(min, max);
		}

		private double Range(double min, double max)
		{
			return (max - min) * Random.NextDouble() + min;
		}

		public MonteCarloParameters Generate()
		{
			var parameters = new MonteCarloParameters();

			//Simulation count
			parameters.MinSimulationCount = Range(1, 100);
			parameters.MaxSimulationCount = Range(parameters.MinSimulationCount, 100);
			parameters.StartSimulationCount = Range(parameters.MinSimulationCount, parameters.MaxSimulationCount);

			//Move count
			parameters.MoveCount = Range(1, 100);

			//WinBonus
			double increment = Range(1.0, 3.0);
			int maxWinBonus = Range(0, 100000);
			parameters.WinBonus = new int[288];
			double value = maxWinBonus;
			for (int i = 0; i < parameters.WinBonus.Length; i++)
			{
				parameters.WinBonus[i] = (int)value;
				value /= increment;
			}

			//Percentages for move types
			parameters.KillMovePercentage = RangeRelative(0, 100);
			parameters.PassMovePercentage = RangeRelative(0, 100 - parameters.KillMovePercentage);

			parameters.MinimumFieldCountForBirthMoves = Range(2, 100);

			//Duration
			parameters.MaxDuration = TimeSpan.FromMilliseconds(Range(0, 10000));
			parameters.MaxRelativeDuration = Range(0.0, 1.0);

			//Simulation
			parameters.SimulationMaxGenerationCount = Range(0, 100);
			parameters.SmartMoveGenerationCount = Range(0, 100);

			//Smart move
			parameters.SmartMoveMinimumFieldCount = Range(0, 100);
			parameters.SmartMoveDurationThreshold = TimeSpan.FromMilliseconds(Range(0, 1000));

			//Weights
			parameters.CellCountWeight = RangeRelative(0, 10);
			parameters.WinBonusWeight = RangeRelative(0, 10);

			return parameters;
		}
	}
}
