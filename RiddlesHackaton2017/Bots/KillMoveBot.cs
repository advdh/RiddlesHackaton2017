using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.Output;
using System.Linq;

namespace RiddlesHackaton2017.Bots
{
	public class KillMoveBot : BaseBot
	{
		public KillMoveBot(IConsole consoleError) : base(consoleError)
		{
		}

		public override Move GetMove()
		{
			var bestScore = BoardStatus.MinValue;
			int bestIndex = -1;
			foreach (int i in Board.MyCells.Union(Board.OpponentCells))
			{
				var newBoard = Board.CopyAndPlay(Board, Board.MyPlayer, new KillMove(i));
				var score = BoardEvaluator.Evaluate(newBoard);
				if (score > bestScore)
				{
					bestScore = score;
					bestIndex = i;
				}
			}
			return new KillMove(bestIndex);
		}
	}
}
