using RiddlesHackaton2017.Bots;
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

		private readonly IMoveSimulator[] _SmartMoveSimulator;
		private readonly IMoveSimulator[] _SimpleMoveSimulator;

		public Simulator(IRandomGenerator randomGenerator,
			MonteCarloParameters monteCarloParameters)
		{
			Random = Guard.NotNull(randomGenerator, nameof(randomGenerator));
			Parameters = Guard.NotNull(monteCarloParameters, nameof(monteCarloParameters));
			_SmartMoveSimulator = new IMoveSimulator[Parameters.MaxSimulationCount];
			_SimpleMoveSimulator = new IMoveSimulator[Parameters.MaxSimulationCount];
		}

		/// <summary>
		/// Simulates the specified move a number of times using MonteCarlo simulation
		/// </summary>
		/// <returns>Statistics object for this move</returns>
		public MonteCarloStatistics SimulateMove(Board startBoard, TimeSpan maxDuration, Move move, int simulationCount)
		{
			StartBoard = Guard.NotNull(startBoard, nameof(startBoard));
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
					statistic.MyScore = int.MaxValue;
					return statistic;
				}
			}
			if (StartBoard.MyPlayerFieldCount == 0)
			{
				//Lost in 0
				statistic.Count = simulationCount;
				statistic.Lost = simulationCount;
				statistic.OpponentScore = int.MaxValue;
				return statistic;
			}

			if (Parameters.ParallelSimulation)
			{
				try
				{
					Parallel.ForEach(
						Enumerable.Range(0, simulationCount),
						() => new MonteCarloStatistics() { Move = move },
						DoSimulate,
						localSum => Aggregate(statistic, localSum)
					);
				}
				catch (AggregateException ex)
				{
					//Log the exception and ignore it
					Console.Error.WriteLine($"{ex.ToString()}");
				}
			}
			else
			{
				for (int i = 0; i < simulationCount; i++)
				{
					DoSimulate(0, statistic);
				}
			}

			return statistic;
		}

		private MonteCarloStatistics DoSimulate(int i, ParallelLoopState state, MonteCarloStatistics statistic)
		{
			return DoSimulate(i, statistic);
		}

		private MonteCarloStatistics DoSimulate(int i, MonteCarloStatistics statistic)
		{
			var result = SimulateRestOfGame(i);

			statistic.Count++;
			statistic.MyScore += result.MyScore;
			statistic.OpponentScore += result.OpponentScore;
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
			Interlocked.Add(ref statistic.MyScore, localSum.MyScore);
			Interlocked.Add(ref statistic.OpponentScore, localSum.OpponentScore);
		}

		private IMoveSimulator GetSmartMoveSimulator(int i)
		{
			if (_SmartMoveSimulator[i] == null)
			{
				_SmartMoveSimulator[i] = new SmartMoveSimulator(Random.Clone(i), Parameters);
			}
			return _SmartMoveSimulator[i];
		}

		private IMoveSimulator GetSimpleMoveSimulator(int i)
		{
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
			int myScore = board.MyPlayerFieldCount;
			int opponentScore = board.OpponentPlayerFieldCount;

			while (StartBoard.Round + generationCount / 2 <= Board.MaxRounds && generationCount < Parameters.SimulationMaxGenerationCount)
			{
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
				myScore += board.MyPlayerFieldCount;
				opponentScore += board.OpponentPlayerFieldCount;

				if (board.OpponentPlayerFieldCount == 0)
				{
					//Won
					myScore += (Parameters.SimulationMaxGenerationCount - generationCount) * 100;
					return new SimulationResult(won: true, generationCount: generationCount,
						myScore: (int)myScore, opponentScore: (int)opponentScore);
				}
				if (board.MyPlayerFieldCount == 0)
				{
					//Lost
					opponentScore += (Parameters.SimulationMaxGenerationCount - generationCount) * 100;
					return new SimulationResult(won: false, generationCount: generationCount,
						myScore: (int)myScore, opponentScore: (int)opponentScore);
				}

				//Next player
				player = player.Opponent();
				generationCount++;
			}

			bool? won = board.MyPlayerFieldCount > 2 * board.OpponentPlayerFieldCount ? true
				: board.OpponentPlayerFieldCount > 2 * board.MyPlayerFieldCount ? (bool?)false
				: null;
			return new SimulationResult(won, generationCount, (int)myScore, (int)opponentScore);
		}

	}
}
