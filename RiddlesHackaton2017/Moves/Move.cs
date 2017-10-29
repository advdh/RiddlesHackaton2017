using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Evaluation;

namespace RiddlesHackaton2017.Moves
{
	public abstract class Move
	{
		public abstract Board Apply(Board board, Player player);

		public abstract string ToOutputString();

		public int DirectImpactForBoard(Board board)
		{
			var newBoard = Board.CopyAndPlay(board, board.MyPlayer, this);
			return BoardEvaluator.Evaluate(newBoard) - BoardEvaluator.Evaluate(board);
		}
	}
}
