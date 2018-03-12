using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
				result.Add(i, score);
				afterMoveBoard.Field[i] = 0;
			}
			return result;
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
			var moveScore = BoardEvaluator.Evaluate(afterMoveBoard2, neighbours2, Parameters.CellCountWeight);
			int myMoveScore = moveScore.Item1;
			int opponentMoveScore = moveScore.Item2;
			var score = BoardEvaluator.Evaluate(board2, neighbours2, Parameters.CellCountWeight);
			int myScore = score.Item1;
			int opponentScore = score.Item2;

			//Win bonus
			int winBonus = Parameters.WinBonus[afterMoveBoard1.OpponentPlayerFieldCount]
				- Parameters.WinBonus[afterMoveBoard1.MyPlayerFieldCount]
				+ Parameters.WinBonus2[afterMoveBoard2.OpponentPlayerFieldCount]
				- Parameters.WinBonus2[afterMoveBoard2.MyPlayerFieldCount];

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
			return myMoveScore - opponentMoveScore - (myScore - opponentScore) 
				+ Parameters.WinBonusWeight * winBonus;
		}
	}
}
