using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.MoveGeneration;
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
					//Draw in 0
					statistic.Count = Parameters.SimulationCount;
					return statistic;
				}
				else
				{
					//Won in 0
					statistic.Count = Parameters.SimulationCount;
					statistic.Won = Parameters.SimulationCount;
					return statistic;
				}
			}
			if (StartBoard.MyPlayerFieldCount == 0)
			{
				//Lost in 0
				statistic.Count = Parameters.SimulationCount;
				statistic.Lost = Parameters.SimulationCount;
				return statistic;
			}

			for (int i = 0; i < Parameters.SimulationCount; i++)
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


		/// <summary>
		/// Simulates one game to the end
		/// </summary>
		public SimulationResult SimulateRestOfGame()
		{
			var board = new Board(StartBoard);

			var player = board.OpponentPlayer;
			int generationCount = 1;
			while (StartBoard.Round + generationCount / 2 < Board.MaxRounds)
			{
				//Bot play
				Move move = GetRandomMove(board, player);
				move.ApplyInline(board, player);
				board = board.NextGeneration;
				if (board.OpponentPlayerFieldCount == 0) return new SimulationResult(won: true, generationCount: generationCount);
				if (board.MyPlayerFieldCount == 0) return new SimulationResult(won: false, generationCount: generationCount);

				//Next player
				player = player.Opponent();
				generationCount++;
			}

			return new SimulationResult(won: null, generationCount: generationCount);
		}

		public Move GetRandomMove(Board board, Player player)
		{
			//If player has only a few cells left, then do a kill move
			if (board.GetFieldCount(player) < Parameters.MinimumFieldCountForBirthMoves)
			{
				//Do a kill move
				return GetRandomKillMove(board, player);
			}
			else
			{
				//Do a birth move
				return GetRandomBirthMove(board, player);
			}
		}

		public Move GetRandomKillMove(Board board, Player player)
		{
			var moveGenerator = new SimulationMoveGenerator(board);
			var board1 = board.NextGeneration;
			var afterMoveBoard = new Board(board);
			var afterMoveBoard1 = new Board(board1);
			var opponentKillMoves = moveGenerator.GetKillsForPlayer(board1, afterMoveBoard, afterMoveBoard1, player.Opponent(), player);
			if (!opponentKillMoves.Any())
			{
				//No kill moves with positive gain: do a pass move
				return new PassMove();
			}
			int value = Random.Next(opponentKillMoves.Last().Value);
			int index = 0;
			foreach (var kvp in opponentKillMoves)
			{
				if (value < kvp.Value)
				{
					index = kvp.Key;
					break;
				}
			}
			return new KillMove(index);
		}

		public Move GetRandomBirthMove(Board board, Player player)
		{
			var moveGenerator = new SimulationMoveGenerator(board);
			var board1 = board.NextGeneration;
			if (board1.GetFieldCount(player.Opponent()) == 0)
			{
				//Pass leads to win
				return new PassMove();
			}
			var afterMoveBoard = new Board(board);
			var afterMoveBoard1 = new Board(board1);
			var births = moveGenerator.GetBirthsForPlayer(board1, afterMoveBoard, afterMoveBoard1, player);
			if (!births.Any())
			{
				//Not enough births: do a kill move anyway
				return GetRandomKillMove(board, player);
			}
			var myKills = moveGenerator.GetKillsForPlayer(board1, afterMoveBoard, afterMoveBoard1, player, player);

			if (myKills.Count < 2)
			{
				//Not enough own kills: do a kill move anyway
				return GetRandomKillMove(board, player);
			}

			int birthValue = Random.Next(births.Last().Value);
			int birthIndex = -1;
			foreach (var kvp in births)
			{
				if (birthValue < kvp.Value)
				{
					birthIndex = kvp.Key;
					break;
				}
			}
			int myKill1 = Random.Next(myKills.Last().Value);
			int killIndex1 = -1;
			foreach (var kvp in myKills)
			{
				if (myKill1 < kvp.Value)
				{
					killIndex1 = kvp.Key;
					break;
				}
			}
			int myKill2 = Random.Next(myKills.Last().Value);
			int killIndex2 = -1;
			foreach (var kvp in myKills)
			{
				if (myKill2 < kvp.Value)
				{
					if (killIndex1 == kvp.Key)
					{
						if (killIndex1 == myKills.First().Key)
						{
							killIndex2 = myKills.ElementAt(1).Key;
						}
						else
						{
							killIndex2 = myKills.First().Key;
						}
					}
					else
					{
						killIndex2 = kvp.Key;
					}
					break;
				}
			}
			return new BirthMove(birthIndex, killIndex1, killIndex2);
		}
	}
}
