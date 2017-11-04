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
			var mine = Enumerable.Range(0, Board.Size).Where(i => Board.Field[i] == 1);
			var his = Enumerable.Range(0, Board.Size).Where(i => Board.Field[i] == 2);

			var bestScore = BoardStatus.MinValue;
			int bestIndex = -1;
			foreach (int i in mine.Union(his))
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
