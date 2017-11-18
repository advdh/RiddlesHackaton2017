using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
using System.Collections.Generic;
using System;

namespace RiddlesHackaton2017.MoveGeneration
{
	public class MoveGenerator
	{
		public MonteCarloParameters Parameters { get; private set; }
		public Board Board { get; private set; }

		public MoveGenerator(Board board, MonteCarloParameters parameters)
		{
			Board = Guard.NotNull(board, nameof(board));
			Parameters = Guard.NotNull(parameters, nameof(parameters));
		}

		/// <summary>Gets a dictionary of kills on one of my cells with their scores after two generations</summary>
		public Dictionary<int, int> GetMyKills(Board board1, Board board2,
			Board afterMoveBoard, Board afterMoveBoard1, Board afterMoveBoard2)
		{
			var result = new Dictionary<int, int>();

			foreach (int i in Board.MyCells)
			{
				var neighbours1 = Board.NeighbourFieldsAndThis[i];
				var neighbours2 = Board.NeighbourFields2[i];
				afterMoveBoard.Field[i] = 0;
				var score = CalculateMoveScore(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
				result.Add(i, score);
				afterMoveBoard.Field[i] = Board.Field[i];
			}
			return result;
		}

		/// <summary>Gets a dictionary of kills on opponent's cells with their scores after one generation</summary>
		/// <param name="killedPlayer">The player whose cell is killed</param>
		/// <param name="playerPlayer">The player who is playing the kill</param>
		public Dictionary<int, int> GetKillsForPlayer(Board board1, Board afterMoveBoard, Board afterMoveBoard1, 
			Player killedPlayer, Player playerPlayer)
		{
			var result = new Dictionary<int, int>();
			int total = 0;
			foreach (int i in Board.GetCells(killedPlayer))
			{
				var neighbours1 = Board.NeighbourFieldsAndThis[i];
				afterMoveBoard.Field[i] = 0;
				var score = CalculateMoveScore(board1, afterMoveBoard, afterMoveBoard1, neighbours1, playerPlayer);
				if (score >= 0)
				{
					total += score * score + 1;
					result.Add(i, total);
				}
				afterMoveBoard.Field[i] = Board.Field[i];
			}
			return result;
		}

		/// <summary>Gets a dictionary of kills on opponent's cells with their scores after two generations</summary>
		public Dictionary<int, int> GetOpponentKills(Board board1, Board board2,
			Board afterMoveBoard, Board afterMoveBoard1, Board afterMoveBoard2)
		{
			var result = new Dictionary<int, int>();

			foreach (int i in Board.OpponentCells)
			{
				var neighbours1 = Board.NeighbourFieldsAndThis[i];
				var neighbours2 = Board.NeighbourFields2[i];
				afterMoveBoard.Field[i] = 0;
				var score = CalculateMoveScore(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
				result.Add(i, score);
				afterMoveBoard.Field[i] = Board.Field[i];
			}
			return result;
		}

		/// <summary>Gets a dictionary of births (not birth moves) with their scores after one generation</summary>
		public Dictionary<int, int> GetBirthsForPlayer(Board board1, Board afterMoveBoard, Board afterMoveBoard1, Player player)
		{
			var result = new Dictionary<int, int>();

			var candidates = Board.EmptyCells;

			int total = 0;
			foreach (int i in candidates)
			{
				var neighbours1 = Board.NeighbourFieldsAndThis[i];
				afterMoveBoard.Field[i] = (short)player;
				var score = CalculateMoveScore(board1, afterMoveBoard, afterMoveBoard1, neighbours1, player);
				if (score > 0)
				{
					total += score * score + 1;
					result.Add(i, total);
				}
				afterMoveBoard.Field[i] = 0;
			}
			return result;
		}

		/// <summary>Gets a dictionary of births (not birth moves) with their scores after two generations</summary>
		public Dictionary<int, int> GetBirths(Board board1, Board board2,
			Board afterMoveBoard, Board afterMoveBoard1, Board afterMoveBoard2)
		{
			var result = new Dictionary<int, int>();

			var candidates = Board.EmptyCells;

			foreach (int i in candidates)
			{
				var neighbours1 = Board.NeighbourFieldsAndThis[i];
				var neighbours2 = Board.NeighbourFields2[i];
				afterMoveBoard.Field[i] = (short)Board.MyPlayer;
				var score = CalculateMoveScore(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
				if (score > 0)
				{
					result.Add(i, score);
				}
				afterMoveBoard.Field[i] = 0;
			}
			return result;
		}

		/// <summary>
		/// Calculates score after next move
		/// </summary>
		/// <param name="board1">Next generation board after pass move</param>
		/// <param name="afterMoveBoard">Board after specific move</param>
		/// <param name="afterMoveBoard1">Next generation board after specific move</param>
		/// <param name="neighbours1">Neighbours of i</param>
		private int CalculateMoveScore(Board board1, Board afterMoveBoard, Board afterMoveBoard1, IEnumerable<int> neighbours1AndThis,
			Player player)
		{
			afterMoveBoard.GetNextGeneration(afterMoveBoard1, neighbours1AndThis);

			//Calculate
			var moveScore = BoardEvaluator.Evaluate(afterMoveBoard1, neighbours1AndThis);
			int myMoveScore = moveScore.Item1;
			int opponentMoveScore = moveScore.Item2;
			var score = BoardEvaluator.Evaluate(board1, neighbours1AndThis);
			int myScore = score.Item1;
			int opponentScore = score.Item2;

			//Reset after move boards
			foreach (int j in neighbours1AndThis)
			{
				afterMoveBoard1.Field[j] = board1.Field[j];
			}
			afterMoveBoard1.MyPlayerFieldCount = board1.MyPlayerFieldCount;
			afterMoveBoard1.OpponentPlayerFieldCount = board1.OpponentPlayerFieldCount;
			int sign = (Board.MyPlayer == player ? 1 : -1);
			return sign * ((myMoveScore - opponentMoveScore) - (myScore - opponentScore));
		}

		/// <summary>
		/// Calculates score after second move
		/// </summary>
		/// <param name="board1">Next generation board after pass move</param>
		/// <param name="board2">Next-next generation board after two pass moves</param>
		/// <param name="afterMoveBoard">Board after specific move</param>
		/// <param name="afterMoveBoard1">Next generation board after specific move</param>
		/// <param name="afterMoveBoard2">Next-next generation board after specific move</param>
		/// <param name="neighbours1">Neighbours of i</param>
		/// <param name="neighbours2">Neighbours of neighbours of i</param>
		public int CalculateMoveScore(Board board1, Board board2,
			Board afterMoveBoard, Board afterMoveBoard1, Board afterMoveBoard2,
			IEnumerable<int> neighbours1AndThis, IEnumerable<int> neighbours2)
		{
			afterMoveBoard.GetNextGeneration(afterMoveBoard1, neighbours1AndThis);
			afterMoveBoard1.GetNextGeneration(afterMoveBoard2, neighbours2);

			//Calculate
			var moveScore = BoardEvaluator.Evaluate(afterMoveBoard2, neighbours2);
			int myMoveScore = moveScore.Item1;
			int opponentMoveScore = moveScore.Item2;
			var score = BoardEvaluator.Evaluate(board2, neighbours2);
			int myScore = score.Item1;
			int opponentScore = score.Item2;

			//Win bonus
			int winBonus = Parameters.WinBonus[afterMoveBoard1.OpponentPlayerFieldCount]
				- Parameters.WinBonus[afterMoveBoard1.MyPlayerFieldCount]
				+ Parameters.WinBonus[afterMoveBoard2.OpponentPlayerFieldCount]
				- Parameters.WinBonus[afterMoveBoard2.MyPlayerFieldCount];

			//Reset after move boards
			foreach (int j in neighbours1AndThis)
			{
				afterMoveBoard1.Field[j] = board1.Field[j];
			}
			afterMoveBoard1.MyPlayerFieldCount = board1.MyPlayerFieldCount;
			afterMoveBoard1.OpponentPlayerFieldCount = board1.OpponentPlayerFieldCount;
			foreach (int j in neighbours2)
			{
				afterMoveBoard2.Field[j] = board2.Field[j];
			}
			afterMoveBoard2.MyPlayerFieldCount = board2.MyPlayerFieldCount;
			afterMoveBoard2.OpponentPlayerFieldCount = board2.OpponentPlayerFieldCount;
			return myMoveScore - opponentMoveScore - (myScore - opponentScore) + winBonus;
		}
	}
}
