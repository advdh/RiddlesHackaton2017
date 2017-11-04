using RiddlesHackaton2017.Models;
using System;
using System.Collections.Generic;

namespace RiddlesHackaton2017.Moves
{
	public abstract class Move
	{
		public abstract Board Apply(Board board, Player player);

		public abstract string ToOutputString();

		/// <summary>Collection of affected fields of the move after one generation</summary>
		public abstract IEnumerable<int> AffectedFields { get; }

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
