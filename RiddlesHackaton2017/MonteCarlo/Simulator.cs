using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.RandomGeneration;

namespace RiddlesHackaton2017.MonteCarlo
{
	public class Simulator
	{
		public Board StartBoard { get; private set; }
		private readonly IRandomGenerator Random;
		public MonteCarloParameters Parameters { get; private set; }

		public Simulator(Board startBoard, IRandomGenerator randomGenerator, MonteCarloParameters monteCarloParameters)
		{
			StartBoard = Guard.NotNull(startBoard, nameof(startBoard));
			Random = Guard.NotNull(randomGenerator, nameof(randomGenerator));
			Parameters = Guard.NotNull(monteCarloParameters, nameof(monteCarloParameters));
		}

		/// <summary>
		/// Simulates the specified move a number of times using MonteCarlo simulation
		/// </summary>
		/// <returns>Score for this move</returns>
		public MonteCarloStatistics SimulateMove(Move move, int simulationCount)
		{
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

			for (int i = 0; i < simulationCount; i++)
			{
				var result = SimulateRestOfGame();

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
			}

			return statistic;
		}

		private IMoveSimulator _SmartMoveSimulator;
		private IMoveSimulator SmartMoveSimulator
		{
			get
			{
				if (_SmartMoveSimulator == null)
				{
					_SmartMoveSimulator = new SmartMoveSimulator(Random, Parameters);
				}
				return _SmartMoveSimulator;
			}
		}

		private IMoveSimulator _SimpleMoveSimulator;
		private IMoveSimulator SimpleMoveSimulator
		{
			get
			{
				if (_SimpleMoveSimulator == null)
				{
					_SimpleMoveSimulator = new SimpleMoveSimulator(Random, Parameters);
				}
				return _SimpleMoveSimulator;
			}
		}

		/// <summary>
		/// Simulates one game to the end
		/// </summary>
		public SimulationResult SimulateRestOfGame()
		{
			var board = new Board(StartBoard);

			var player = board.OpponentPlayer;
			int generationCount = 1;
			while (StartBoard.Round + generationCount / 2 < Board.MaxRounds && generationCount < Parameters.SimulationMaxGenerationCount)
			{
				//Bot play
				var simulator = generationCount < Parameters.SimulationMaxGenerationCount 
					&& (StartBoard.Player1FieldCount < Parameters.SmartMoveMinimumFieldCount || StartBoard.Player2FieldCount < Parameters.SmartMoveMinimumFieldCount) 
					? SmartMoveSimulator : SimpleMoveSimulator;
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
