using System;
using System.Linq;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.RandomGeneration;
using RiddlesHackaton2017.Bots;

namespace RiddlesHackaton2017.MonteCarlo
{
	public class SimpleMoveSimulator : IMoveSimulator
	{
		private readonly IRandomGenerator Random;
		private readonly MonteCarloParameters Parameters;

		public SimpleMoveSimulator(IRandomGenerator randomGenerator, MonteCarloParameters monteCarloParameters)
		{
			Random = Guard.NotNull(randomGenerator, nameof(randomGenerator));
			Parameters = Guard.NotNull(monteCarloParameters, nameof(monteCarloParameters));
		}

		public Tuple<Move, Board> GetRandomMove(Board board)
		{
			//If player has only a few cells left, then do only kill moves
			if (board.PlayerFieldCount[board.MyPlayer.Value()] < Parameters.MinimumFieldCountForBirthMoves)
			{
				return new Tuple<Move, Board>(GetRandomKillMove(board), null);
			}

			int rnd = Random.Next(100);
			if (rnd < Parameters.PassMovePercentage)
			{
				//Pass move
				return new Tuple<Move, Board>(new PassMove(), null);
			}
			else if (rnd < Parameters.PassMovePercentage + Parameters.KillMovePercentage)
			{
				//Kill move
				return new Tuple<Move, Board>(GetRandomKillMove(board), null);
			}
			else
			{
				//Birth move
				return new Tuple<Move, Board>(GetRandomBirthMove(board), null);
			}
		}

		public KillMove GetRandomKillMove(Board board)
		{
			var opponentCells = board.GetCells(board.OpponentPlayer).ToArray();
			return new KillMove(opponentCells[Random.Next(opponentCells.Length)]);
		}

		public Move GetRandomBirthMove(Board board)
		{
			var mine = board.GetCells(board.MyPlayer).ToArray();
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
