using System;
using System.Linq;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.RandomGeneration;
using RiddlesHackaton2017.Bots;

namespace RiddlesHackaton2017.MonteCarlo
{
	public class SmartMoveSimulator : IMoveSimulator
	{
		private IRandomGenerator Random;
		private MonteCarloParameters Parameters;

		public SmartMoveSimulator(IRandomGenerator randomGenerator, MonteCarloParameters monteCarloParameters)
		{
			Random = Guard.NotNull(randomGenerator, nameof(randomGenerator));
			Parameters = Guard.NotNull(monteCarloParameters, nameof(monteCarloParameters));
		}

		public Tuple<Move, Board> GetRandomMove(Board board, Player player)
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

		public Tuple<Move, Board> GetRandomKillMove(Board board, Player player)
		{
			var moveGenerator = new SimulationMoveGenerator(board);
			var board1 = board.NextGeneration;
			var afterMoveBoard = new Board(board);
			var afterMoveBoard1 = new Board(board1);
			var opponentKillMoves = moveGenerator.GetKillsForPlayer(board1, afterMoveBoard, afterMoveBoard1, player.Opponent(), player);
			if (!opponentKillMoves.Any())
			{
				//No kill moves with positive gain: do a pass move
				return new Tuple<Move, Board>(new PassMove(), board1);
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
			var move = new KillMove(index);
			move.ApplyInline(afterMoveBoard, player);
			afterMoveBoard.GetNextGeneration(afterMoveBoard1, move.AffectedFields);
			return new Tuple<Move, Board>(move, afterMoveBoard1);
		}

		public Tuple<Move, Board> GetRandomBirthMove(Board board, Player player)
		{
			var moveGenerator = new SimulationMoveGenerator(board);
			var board1 = board.NextGeneration;
			if (board1.GetFieldCount(player.Opponent()) == 0)
			{
				//Pass leads to win
				return new Tuple<Move, Board>(new PassMove(), board1);
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
			var move = new BirthMove(birthIndex, killIndex1, killIndex2);
			move.ApplyInline(afterMoveBoard, player);
			afterMoveBoard.GetNextGeneration(afterMoveBoard1, move.AffectedFields);
			return new Tuple<Move, Board>(move, afterMoveBoard1);
		}
	}
}
