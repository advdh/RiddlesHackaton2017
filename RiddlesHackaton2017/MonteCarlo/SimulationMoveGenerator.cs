using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
using System.Collections.Generic;

namespace RiddlesHackaton2017.MonteCarlo
{
	public class SimulationMoveGenerator
	{
		public Board Board { get; private set; }

		public SimulationMoveGenerator(Board board)
		{
			Board = Guard.NotNull(board, nameof(board));
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

		/// <summary>Gets a dictionary of births (not birth moves) with their scores after one generation</summary>
		public Dictionary<int, int> GetBirthsForPlayer(Board board1, Board afterMoveBoard, Board afterMoveBoard1, Player player)
		{
			var result = new Dictionary<int, int>();

			var candidates = Board.EmptyCells;

			int total = 0;
			foreach (int i in candidates)
			{
				var neighbours1 = Board.NeighbourFieldsAndThis[i];
				afterMoveBoard.Field[i] = player.Value();
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
			int moveScore = BoardEvaluator.Evaluate(afterMoveBoard1, neighbours1AndThis, 1);
			var score = BoardEvaluator.Evaluate(board1, neighbours1AndThis, 1);

			//Reset after move boards
			foreach (int j in neighbours1AndThis)
			{
				afterMoveBoard1.Field[j] = board1.Field[j];
			}
			afterMoveBoard1.MyPlayerFieldCount = board1.MyPlayerFieldCount;
			afterMoveBoard1.OpponentPlayerFieldCount = board1.OpponentPlayerFieldCount;
			int sign = (Board.MyPlayer == player ? 1 : -1);
			return sign * (moveScore - score);
		}
	}
}
