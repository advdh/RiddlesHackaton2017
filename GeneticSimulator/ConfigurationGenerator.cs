using RiddlesHackaton2017.Bots;
using System;

namespace GeneticSimulator
{
	public class ConfigurationGenerator
	{
		private Random Random;

		public ConfigurationGenerator(Random random)
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

		/// <summary>
		/// Generates a random set of MonteCarloParameters
		/// </summary>
		/// <remarks>Code duplication: TODO: reuse Vary method</remarks>
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
			parameters.MaxWinBonus = Range(0, 100000);
			parameters.WinBonusDecrementFactor = Range(0.33, 1.0);

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

		public enum Parameters
		{
			MinSimulationCount,
			MaxSimulationCount,
			StartSimulationCount,
			MoveCount,
			MaxWinBonus,
			WinBonusDecrementFactor,
			MaxWinBonus2,
			WinBonusDecrementFactor2,
			KillMovePercentage,
			PassMovePercentage,
			MinimumFieldCountForBirthMoves,
			MaxDuration,
			MaxRelativeDuration,
			SimulationMaxGenerationCount,
			SmartMoveGenerationCount,
			SmartMoveMinimumFieldCount,
			SmartMoveDurationThreshold,
			CellCountWeight,
			WinBonusWeight,
			BinarySimulationResult,
			ParallelSimulation,
			SimulationDecrementScore2Factor,
			HardcodedSimulationCounts,
			SimulationFactor,
			ScoreBasedOnWinBonus,
			DoubleWinBonusCount,
		}

		/// <summary>
		/// Generates a MonteCarloParameters object with speified one parameter randomly changed with respect to the specified MonteCarloParameters
		/// </summary>
		public MonteCarloParameters Vary(MonteCarloParameters original, Parameters varyingParameter)
		{
			var parameters = new MonteCarloParameters(original);

			switch (varyingParameter)
			{
				case Parameters.MinSimulationCount:
					parameters.MinSimulationCount = Range(1, 100);
					break;
				case Parameters.MaxSimulationCount:
					parameters.MaxSimulationCount = Range(parameters.MinSimulationCount, 100);
					break;
				case Parameters.StartSimulationCount:
					parameters.StartSimulationCount = Range(parameters.MinSimulationCount, parameters.MaxSimulationCount);
					break;
				case Parameters.MoveCount:
					parameters.MoveCount = Range(1, 100);
					break;
				case Parameters.MaxWinBonus:
					parameters.MaxWinBonus = Range(0, 100000);
					break;
				case Parameters.WinBonusDecrementFactor:
					parameters.WinBonusDecrementFactor = Range(0.33, 1.0);
					break;
				case Parameters.KillMovePercentage:
					parameters.KillMovePercentage = RangeRelative(0, 100);
					break;
				case Parameters.PassMovePercentage:
					parameters.PassMovePercentage = RangeRelative(0, 100 - parameters.KillMovePercentage);
					break;
				case Parameters.MinimumFieldCountForBirthMoves:
					parameters.MinimumFieldCountForBirthMoves = Range(2, 100);
					break;
				case Parameters.MaxDuration:
					parameters.MaxDuration = TimeSpan.FromMilliseconds(Range(0, 10000));
					break;
				case Parameters.MaxRelativeDuration:
					parameters.MaxRelativeDuration = Range(0.0, 1.0);
					break;
				case Parameters.SimulationMaxGenerationCount:
					parameters.SimulationMaxGenerationCount = Range(0, 100);
					break;
				case Parameters.SmartMoveGenerationCount:
					parameters.SmartMoveGenerationCount = Range(0, 100);
					break;
				case Parameters.SmartMoveMinimumFieldCount:
					parameters.SmartMoveMinimumFieldCount = Range(0, 100);
					break;
				case Parameters.SmartMoveDurationThreshold:
					parameters.SmartMoveDurationThreshold = TimeSpan.FromMilliseconds(Range(0, 1000));
					break;
				case Parameters.CellCountWeight:
					parameters.CellCountWeight = RangeRelative(0, 10);
					break;
				case Parameters.WinBonusWeight:
					parameters.WinBonusWeight = RangeRelative(0, 10);
					break;
				default:
					throw new ArgumentException($"Vary: parameter not supported: {varyingParameter}");
			}
			return parameters;
		}

		/// <summary>
		/// Generates a MonteCarloParameters object with speified one parameter value multiplied with the specified factor
		/// </summary>
		public MonteCarloParameters Vary(MonteCarloParameters original, Parameters varyingParameter, double factor)
		{
			var parameters = new MonteCarloParameters(original);

			switch (varyingParameter)
			{
				case Parameters.MinSimulationCount:
					parameters.MinSimulationCount = (int)(original.MinSimulationCount * factor);
					break;
				case Parameters.MaxSimulationCount:
					parameters.MaxSimulationCount = (int)(original.MinSimulationCount * factor);
					break;
				case Parameters.StartSimulationCount:
					parameters.StartSimulationCount = (int)(original.StartSimulationCount * factor);
					break;
				case Parameters.MoveCount:
					parameters.MoveCount = (int)(original.MoveCount * factor);
					break;
				case Parameters.MaxWinBonus:
					parameters.MaxWinBonus = (int)(original.MaxWinBonus * factor);
					break;
				case Parameters.WinBonusDecrementFactor:
					parameters.WinBonusDecrementFactor = original.WinBonusDecrementFactor * factor;
					break;
				case Parameters.KillMovePercentage:
					parameters.KillMovePercentage = (int)(original.KillMovePercentage * factor);
					break;
				case Parameters.PassMovePercentage:
					parameters.PassMovePercentage = (int)(original.PassMovePercentage * factor);
					break;
				case Parameters.MinimumFieldCountForBirthMoves:
					parameters.MinimumFieldCountForBirthMoves = (int)(original.MinimumFieldCountForBirthMoves* factor); ;
					break;
				case Parameters.MaxDuration:
					parameters.MaxDuration = TimeSpan.FromMilliseconds(original.MaxDuration.TotalMilliseconds * factor);
					break;
				case Parameters.MaxRelativeDuration:
					parameters.MaxRelativeDuration = (int)(original.MaxRelativeDuration * factor);
					break;
				case Parameters.SimulationMaxGenerationCount:
					parameters.SimulationMaxGenerationCount = (int)(original.SimulationMaxGenerationCount * factor);
					break;
				case Parameters.SmartMoveGenerationCount:
					parameters.SmartMoveGenerationCount = (int)(original.SmartMoveGenerationCount * factor); ;
					break;
				case Parameters.SmartMoveMinimumFieldCount:
					parameters.SmartMoveMinimumFieldCount = (int)(original.SmartMoveMinimumFieldCount * factor);
					break;
				case Parameters.SmartMoveDurationThreshold:
					parameters.SmartMoveDurationThreshold = TimeSpan.FromMilliseconds(original.SmartMoveDurationThreshold.TotalMilliseconds * factor);
					break;
				case Parameters.CellCountWeight:
					parameters.CellCountWeight = (int)(original.CellCountWeight * factor);
					break;
				case Parameters.WinBonusWeight:
					parameters.WinBonusWeight = (int)(original.WinBonusWeight* factor);
					break;
				default:
					throw new ArgumentException($"Vary: parameter not supported: {varyingParameter}");
			}
			return parameters;
		}

		public MonteCarloParameters CrossOver(MonteCarloParameters parameters1, MonteCarloParameters parameters2, double p)
		{
			var newParameters = new MonteCarloParameters();

			newParameters.CellCountWeight = Random.NextDouble() < p ? parameters1.CellCountWeight : parameters2.CellCountWeight;
			newParameters.KillMovePercentage = Random.NextDouble() < p ? parameters1.KillMovePercentage : parameters2.KillMovePercentage;
			newParameters.MaxDuration = Random.NextDouble() < p ? parameters1.MaxDuration : parameters2.MaxDuration;
			newParameters.MaxRelativeDuration = Random.NextDouble() < p ? parameters1.MaxRelativeDuration : parameters2.MaxRelativeDuration;
			newParameters.MaxSimulationCount = Random.NextDouble() < p ? parameters1.MaxSimulationCount : parameters2.MaxSimulationCount;
			newParameters.MaxWinBonus = Random.NextDouble() < p ? parameters1.MaxWinBonus : parameters2.MaxWinBonus;
			newParameters.MinimumFieldCountForBirthMoves = Random.NextDouble() < p ? parameters1.MinimumFieldCountForBirthMoves : parameters2.MinimumFieldCountForBirthMoves;
			newParameters.MinSimulationCount = Random.NextDouble() < p ? parameters1.MinSimulationCount : parameters2.MinSimulationCount;
			newParameters.MoveCount = Random.NextDouble() < p ? parameters1.MoveCount : parameters2.MoveCount;
			newParameters.PassMovePercentage = Random.NextDouble() < p ? parameters1.PassMovePercentage : parameters2.PassMovePercentage;
			newParameters.SimulationMaxGenerationCount = Random.NextDouble() < p ? parameters1.SimulationMaxGenerationCount : parameters2.SimulationMaxGenerationCount;
			newParameters.SmartMoveDurationThreshold = Random.NextDouble() < p ? parameters1.SmartMoveDurationThreshold : parameters2.SmartMoveDurationThreshold;
			newParameters.SmartMoveGenerationCount = Random.NextDouble() < p ? parameters1.SmartMoveGenerationCount : parameters2.SmartMoveGenerationCount;
			newParameters.SmartMoveMinimumFieldCount = Random.NextDouble() < p ? parameters1.SmartMoveMinimumFieldCount : parameters2.SmartMoveMinimumFieldCount;
			newParameters.StartSimulationCount = Random.NextDouble() < p ? parameters1.StartSimulationCount : parameters2.StartSimulationCount;
			newParameters.WinBonusDecrementFactor = Random.NextDouble() < p ? parameters1.WinBonusDecrementFactor : parameters2.WinBonusDecrementFactor;
			newParameters.WinBonusWeight = Random.NextDouble() < p ? parameters1.WinBonusWeight : parameters2.WinBonusWeight;

			return newParameters;
		}
	}
}
