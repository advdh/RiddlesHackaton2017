using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.RandomGeneration;
using System.Linq;

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
		public MonteCarloStatistics SimulateMove(Move move)
		{
			var statistic = new MonteCarloStatistics() { Move = move };
			if (StartBoard.OpponentPlayerFieldCount == 0)
			{
				if (StartBoard.MyPlayerFieldCount == 0)
				{
					//Draw in 1
					statistic.Count = Parameters.SimulationCount;
					return statistic;
				}
				else
				{
					//Won in 1
					statistic.Count = Parameters.SimulationCount;
					statistic.Won = Parameters.SimulationCount;
					statistic.WonInRounds = Parameters.SimulationCount;
					return statistic;
				}
			}
			if (StartBoard.MyPlayerFieldCount == 0)
			{
				//Lost in 1
				statistic.Count = Parameters.SimulationCount;
				statistic.Lost = Parameters.SimulationCount;
				statistic.LostInRounds = Parameters.SimulationCount;
				return statistic;
			}

			for (int i = 0; i < Parameters.SimulationCount; i++)
			{
				var result = SimulateRestOfGame();

				statistic.Count++;
				if (result.Won.HasValue && result.Won.Value)
				{
					statistic.Won++;
					statistic.WonInRounds += (result.Round - StartBoard.Round);
				}
				if (result.Won.HasValue && !result.Won.Value)
				{
					statistic.Lost++;
					statistic.LostInRounds += (result.Round - StartBoard.Round);
				}
			}

			return statistic;
		}


		/// <summary>
		/// Simulates one game to the end
		/// </summary>
		/// <returns>true if won, false if lost, null if draw</returns>
		public SimulationResult SimulateRestOfGame()
		{
			var board = new Board(StartBoard);

			var player = board.OpponentPlayer;
			while (board.Round < Board.MaxRounds)
			{
				//Bot play
				Move move = GetRandomMove(board, player);
				board = board.ApplyMoveAndNext(player, move);
				if (board.OpponentPlayerFieldCount == 0) return new SimulationResult(won: true, round: board.Round);
				if (board.MyPlayerFieldCount == 0) return new SimulationResult(won: false, round: board.Round);

				//Next player
				player = player.Opponent();
			}

			return new SimulationResult(won: null, round: Board.MaxRounds);
		}

		private Move GetRandomMove(Board board, Player player)
		{
			//If player has only a few cells left, then do only kill moves
			if (board.GetFieldCount(player) < Parameters.MinimumFieldCountForBirthMoves)
			{
				return GetRandomKillMove(board, player);
			}

			int rnd = Random.Next(100);
			if (rnd < Parameters.PassMovePercentage)
			{
				//With probability 1% we do a pass move
				return new PassMove();
			}
			else if (rnd < Parameters.PassMovePercentage + Parameters.KillMovePercentage)
			{
				//With probability 49% we do a kill move
				return GetRandomKillMove(board, player);
			}
			else
			{
				//With probability 50% we do a birth move
				return GetRandomBirthMove(board, player);
			}
		}

		public KillMove GetRandomKillMove(Board board, Player player)
		{
			var opponentCells = board.GetCells(player.Opponent()).ToArray();
			return new KillMove(opponentCells[Random.Next(opponentCells.Length)]);
		}

		public Move GetRandomBirthMove(Board board, Player player)
		{
			var mine = board.GetCells(player).ToArray();
			if (mine.Count() < 2)
			{
				//Only one cell left: cannot do a birth move
				//Switch to pass move
				return new PassMove();
			}

			//Pick one empty cell for birth
			//Don't pick an empty cell without any neighbours
			var empty = board.EmptyCells
				.Where(c => Board.NeighbourFields[c]
					.Any(nc => board.Field[nc] != 0))
				.ToArray();
			int b = empty[Random.Next(empty.Length)];

			//Pick two cells of my own to sacrifice
			int s1, s2;
			if (mine.Length == 2)
			{
				s1 = mine.First();
				s2 = mine.Last();
			}
			else
			{
				s1 = mine[Random.Next(mine.Length)];
				do
				{
					s2 = mine[Random.Next(mine.Length)];
				}
				while (s2 == s1);
			}

			return new BirthMove(b, s1, s2);
		}
	}
}
