using RiddlesHackaton2017.Models;
using System.Collections.Generic;
using System;

namespace RiddlesHackaton2017.Evaluation
{
	public static class BoardEvaluator
	{
		public static BoardStatus Evaluate(Board board)
		{
			var status = board.MyPlayerFieldCount > 0 ?
				board.OpponentPlayerFieldCount > 0 ? GameStatus.Busy : GameStatus.Won :
				board.MyPlayerFieldCount > 0 ? GameStatus.Lost : GameStatus.Draw;
			return new BoardStatus(status, board.MyPlayerFieldCount - board.OpponentPlayerFieldCount);
		}

		/// <returns>My score - opponent score</returns>
		public static int Evaluate(Board board, IEnumerable<int> positions, 
			int cellCountWeight)
		{
			int score = 0;
			foreach (int i in positions)
			{
				switch (board.Field[i])
				{
					case 1: score += cellCountWeight; break;
					case 2: score -= cellCountWeight; break;
				}
			}
			int sign = board.MyPlayer == Player.Player1 ? 1 : -1;
			return sign * score;
		}
	}
}
