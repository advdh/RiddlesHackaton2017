using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiddlesHackaton2017.Moves
{
	public abstract class Move
	{
		public abstract Board Apply(Board board, Player player);

		public abstract string ToOutputString();

		/// <summary>Collection of affected fields of the move after one generation</summary>
		public abstract IEnumerable<int> AffectedFields { get; }

		public int DirectImpactForBoard(Board board)
		{
			//var boardAfterMove = Board.CopyAndPlay(board, board.MyPlayer, this);
			//var boardAfterPassMove = Board.CopyAndPlay(board, board.MyPlayer, new PassMove());
			var affectedFields = AffectedFields.ToArray();
			var boardAfterMove = Board.CopyAndPlayPartial(board, board.MyPlayer, this, affectedFields);
			var boardAfterPassMove = Board.CopyAndPlayPartial(board, board.MyPlayer, new PassMove(), affectedFields);
			return BoardEvaluator.Evaluate(boardAfterMove) - BoardEvaluator.Evaluate(boardAfterPassMove);
		}

		public static Move Parse(string moveString)
		{
			Move result;
			if (PassMove.TryParse(moveString, out result)) return result;
			if (KillMove.TryParse(moveString, out result)) return result;
			if (BirthMove.TryParse(moveString, out result)) return result;
			if (NullMove.TryParse(moveString, out result)) return result;
			throw new ArgumentException("moveString");
		}

		public override bool Equals(object obj)
		{
			return ToString().Equals(obj.ToString());
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}
}
