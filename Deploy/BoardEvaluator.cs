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
			throw new NotImplementedException();
			int score = 0;
			return score;
		}
	}
}
