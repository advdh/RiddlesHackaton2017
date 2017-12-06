﻿using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.RandomGeneration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RiddlesHackaton2017.MonteCarlo
{
	public class Simulator
	{
		public Board StartBoard { get; set; }
		private readonly IRandomGenerator Random;
		public MonteCarloParameters Parameters { get; private set; }
		public TimeSpan MaxDuration { get; set; }

		public Simulator(IRandomGenerator randomGenerator, 
			MonteCarloParameters monteCarloParameters)
		{
			Random = Guard.NotNull(randomGenerator, nameof(randomGenerator));
			Parameters = Guard.NotNull(monteCarloParameters, nameof(monteCarloParameters));
		}

		/// <summary>
		/// Simulates the specified move a number of times using MonteCarlo simulation
		/// </summary>
		/// <returns>Score for this move</returns>
		public MonteCarloStatistics SimulateMove(Board startBoard, TimeSpan maxDuration, Move move, int simulationCount)
		{
			StartBoard = startBoard;
			MaxDuration = maxDuration;

			var statistic = new MonteCarloStatistics() { Move = move };
			if (StartBoard.OpponentPlayerFieldCount == 0)
			{
				if (StartBoard.MyPlayerFieldCount == 0)
				{
					//Draw in 0
					statistic.Count = simulationCount;
					return statistic;
				}
				else
				{
					//Won in 0
					statistic.Count = simulationCount;
					statistic.Won = simulationCount;
					return statistic;
				}
			}
			if (StartBoard.MyPlayerFieldCount == 0)
			{
				//Lost in 0
				statistic.Count = simulationCount;
				statistic.Lost = simulationCount;
				return statistic;
			}

			Parallel.ForEach(
				Enumerable.Range(0, simulationCount), 
				() => new MonteCarloStatistics() { Move = move }, 
				DoSimulate,
				localSum => Aggregate(statistic, localSum)
			);

			return statistic;
		}

		private MonteCarloStatistics DoSimulate(int i, ParallelLoopState state, MonteCarloStatistics statistic)
		{
			var result = SimulateRestOfGame(i);

			statistic.Count++;
			if (result.Won.HasValue && result.Won.Value)
			{
				statistic.Won++;
				statistic.WonInGenerations += result.GenerationCount;
			}
			if (result.Won.HasValue && !result.Won.Value)
			{
				statistic.Lost++;
				statistic.LostInGenerations += result.GenerationCount;
			}
			return statistic;
		}

		private void Aggregate(MonteCarloStatistics statistic, MonteCarloStatistics localSum)
		{
			Interlocked.Add(ref statistic.Count, localSum.Count);
			Interlocked.Add(ref statistic.Won, localSum.Won);
			Interlocked.Add(ref statistic.WonInGenerations, localSum.WonInGenerations);
			Interlocked.Add(ref statistic.Lost, localSum.Lost);
			Interlocked.Add(ref statistic.LostInGenerations, localSum.LostInGenerations);
		}

		private IMoveSimulator[] _SmartMoveSimulator;
		private IMoveSimulator GetSmartMoveSimulator(int i)
		{
			if (_SmartMoveSimulator == null)
			{
				_SmartMoveSimulator = new IMoveSimulator[Parameters.MaxSimulationCount];
			}
			if (_SmartMoveSimulator[i] == null)
			{
				_SmartMoveSimulator[i] = new SmartMoveSimulator(Random.Clone(i), Parameters);
			}
			return _SmartMoveSimulator[i];
		}

		private IMoveSimulator[] _SimpleMoveSimulator;
		private IMoveSimulator GetSimpleMoveSimulator(int i)
		{
			if (_SimpleMoveSimulator == null)
			{
				_SimpleMoveSimulator = new IMoveSimulator[Parameters.MaxSimulationCount];
			}
			if (_SimpleMoveSimulator[i] == null)
			{
				_SimpleMoveSimulator[i] = new SimpleMoveSimulator(Random.Clone(i), Parameters);
			}
			return _SimpleMoveSimulator[i];
		}

		/// <summary>
		/// Simulates one game to the end
		/// </summary>
		public SimulationResult SimulateRestOfGame(int i)
		{
			var board = new Board(StartBoard);

			var player = board.OpponentPlayer;
			int generationCount = 1;
			while (StartBoard.Round + generationCount / 2 < Board.MaxRounds && generationCount < Parameters.SimulationMaxGenerationCount)
			{
				//Bot play
				var simulator = generationCount < Parameters.SmartMoveGenerationCount
					&& (StartBoard.Player1FieldCount < Parameters.SmartMoveMinimumFieldCount || StartBoard.Player2FieldCount < Parameters.SmartMoveMinimumFieldCount) 
					&& MaxDuration > Parameters.SmartMoveDurationThreshold
					? GetSmartMoveSimulator(i) : GetSimpleMoveSimulator(i);
				var tuple = simulator.GetRandomMove(board, player);
				Move move = tuple.Item1;
				Board nextBoard = tuple.Item2;
				if (nextBoard == null)
				{
					move.ApplyInline(board, player);
					board = board.NextGeneration;
				}
				else
				{
					board = nextBoard;
					board.ResetNextGeneration();
				}
				if (board.OpponentPlayerFieldCount == 0) return new SimulationResult(won: true, generationCount: generationCount);
				if (board.MyPlayerFieldCount == 0) return new SimulationResult(won: false, generationCount: generationCount);

				//Next player
				player = player.Opponent();
				generationCount++;
			}

			bool? won = board.MyPlayerFieldCount > 2 * board.OpponentPlayerFieldCount ? true
				: board.OpponentPlayerFieldCount > 2 * board.MyPlayerFieldCount ? (bool?)false 
				: null;
			return new SimulationResult(won: won, generationCount: generationCount);
		}

	}
}
