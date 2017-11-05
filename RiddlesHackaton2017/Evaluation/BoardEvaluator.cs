using RiddlesHackaton2017.Models;

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
		public static BoardStatus Evaluate(Board board, int baseScore = 0)
		{
			var status = board.MyPlayerFieldCount > 0 ?
				board.OpponentPlayerFieldCount > 0 ? GameStatus.Busy : GameStatus.Won :
				board.MyPlayerFieldCount > 0 ? GameStatus.Lost : GameStatus.Draw;
			return new BoardStatus(status, board.MyPlayerFieldCount - board.OpponentPlayerFieldCount - baseScore);
		}
	}
}
