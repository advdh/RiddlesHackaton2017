using RiddlesHackaton2017.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiddlesHackaton2017.Evaluation
{
	public static class BoardEvaluator
	{
		/// <summary>
		/// Bonus for score if we are (pretty) sure that we're going to win
		/// </summary>
		public const int WinBonus = 999999;

		/// <summary>
		/// Threshold, above which we assume that we win
		/// </summary>
		public const int WinThreshold = 499999;

		/// <returns>score of MyPlayer</returns>
		public static int Evaluate(Board board, 
			int optionalArgument1 = 0)
		{
			int mine = Enumerable.Range(0, Board.Size).Count(i => board.Field[i] == 1);
			int his = Enumerable.Range(0, Board.Size).Count(i => board.Field[i] == 2);
			return mine - his;
		}
	}
}
