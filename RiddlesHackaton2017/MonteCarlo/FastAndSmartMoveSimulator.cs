using System;
using System.Linq;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.RandomGeneration;
using RiddlesHackaton2017.Bots;

namespace RiddlesHackaton2017.MonteCarlo
{
	public class FastAndSmartMoveSimulator : IMoveSimulator
	{
		private readonly IRandomGenerator Random;
		private readonly MonteCarloParameters Parameters;

		public FastAndSmartMoveSimulator(IRandomGenerator randomGenerator, MonteCarloParameters monteCarloParameters)
		{
			Random = Guard.NotNull(randomGenerator, nameof(randomGenerator));
			Parameters = Guard.NotNull(monteCarloParameters, nameof(monteCarloParameters));
		}

		public Tuple<Move, Board> GetRandomMove(Board board, Player player)
		{
			//If player has only a few cells left, then do only kill moves
			if (board.GetFieldCount(player) < Parameters.MinimumFieldCountForBirthMoves)
			{
				return new Tuple<Move, Board>(GetRandomKillMove(board, player), null);
			}

			Move move;
			int rnd = Random.Next(100);
			if (rnd < Parameters.PassMovePercentage)
			{
				//Pass move
				move = new PassMove();
			}
			else if (rnd < Parameters.PassMovePercentage + Parameters.KillMovePercentage)
			{
				//Kill move
				move = GetRandomKillMove(board, player);
			}
			else
			{
				//Birth move
				move = GetRandomBirthMove(board, player);
			}
			move.ApplyInline(board, player);
			return new Tuple<Move, Board>(move, board.NextGeneration);
		}

		public Move GetRandomKillMove(Board board, Player player)
		{
			//Select an opponent cell, which would not be killed anyway in the next turn
			var opponentCells = board.GetCells(player.Opponent())
				.Where(i => board.NextGeneration.Field[i] == board.Field[i])
				.ToArray();
			if (opponentCells.Length == 0) return new PassMove();
			//var positions = opponentCells.Select(c => new Position(c)).ToArray();
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
			//Pick an empty cell which would get birth in the next turn
			var empty = board.EmptyCells
				.Where(c => Board.NeighbourFields[c]
					.Any(nc => board.Field[nc] != 0))
				.ToArray();
			short opponent = (short)(board.OpponentPlayer == Player.Player1 ? 1 : 2);
			var opponentBirthCells = empty
				.Where(i => board.NextGeneration.Field[i] == opponent)
				.ToArray();
			//var birthPositions = opponentBirthCells.Select(c => new Position(c)).ToArray();
			if (opponentBirthCells.Length == 0) return GetRandomKillMove(board, player);

			int b = opponentBirthCells[Random.Next(opponentBirthCells.Length)];

			//Pick two cells of my own to sacrifice: pick cells which would die anyway
			var dyingOwnCells = mine
				.Where(i => board.NextGeneration.Field[i] == 0)
				.ToArray();
			//var positions = dyingOwnCells.Select(c => new Position(c)).ToArray();
			if (dyingOwnCells.Length < 2) return GetRandomKillMove(board, player);

			int s1, s2;
			if (dyingOwnCells.Length == 2)
			{
				s1 = dyingOwnCells.First();
				s2 = dyingOwnCells.Last();
			}
			else
			{
				s1 = dyingOwnCells[Random.Next(dyingOwnCells.Length)];
				do
				{
					s2 = dyingOwnCells[Random.Next(dyingOwnCells.Length)];
				}
				while (s2 == s1);
			}

			return new BirthMove(b, s1, s2);
		}
	}
}
