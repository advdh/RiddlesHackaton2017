using System;
using System.Linq;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.RandomGeneration;
using RiddlesHackaton2017.Bots;
using System.Collections.Generic;

namespace RiddlesHackaton2017.MonteCarlo
{
	public class SmartMoveSimulator : IMoveSimulator
	{
		private readonly IRandomGenerator Random;
		private readonly MonteCarloParameters Parameters;

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
			var opponentKills = board.OpponentKills ?? GetKills(board, player.Opponent());

			if (!opponentKills.Any())
			{
				//No kill moves with positive gain: do a pass move
				return new Tuple<Move, Board>(new PassMove(), board.NextGeneration);
			}
			int value = Random.Next(opponentKills.Last().Value);
			int index = 0;
			foreach (var kvp in opponentKills)
			{
				if (value < kvp.Value)
				{
					index = kvp.Key;
					break;
				}
			}
			var move = new KillMove(index);
			move.ApplyInline(board, player);
			return new Tuple<Move, Board>(move, board.NextGeneration);
		}

		public Tuple<Move, Board> GetRandomBirthMove(Board board, Player player)
		{
			var births = board.MyBirths ?? GetBirths(board, player);
			var myKills = board.MyKills ?? GetKills(board, player);

			if (board.NextGeneration.GetFieldCount(player.Opponent()) == 0)
			{
				//Pass leads to win
				return new Tuple<Move, Board>(new PassMove(), board.NextGeneration);
			}

			if (!births.Any())
			{
				//Not enough births: do a kill move anyway
				return GetRandomKillMove(board, player);
			}

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
			move.ApplyInline(board, player);
			return new Tuple<Move, Board>(move, board.NextGeneration);
		}

		public static Dictionary<int, int> GetKills(Board board, Player player)
		{
			var moveGenerator = new SimulationMoveGenerator(board);
			var board1 = board.NextGeneration;
			var afterMoveBoard = new Board(board);
			var afterMoveBoard1 = new Board(board1);
			return moveGenerator.GetKillsForPlayer(board1, afterMoveBoard, afterMoveBoard1, player, player);
		}

		public static Dictionary<int, int> GetBirths(Board board, Player player)
		{
			var moveGenerator = new SimulationMoveGenerator(board);
			var board1 = board.NextGeneration;
			var afterMoveBoard = new Board(board);
			var afterMoveBoard1 = new Board(board1);
			return moveGenerator.GetBirthsForPlayer(board1, afterMoveBoard, afterMoveBoard1, player);
		}
	}
}
