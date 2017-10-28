using System;
using RiddlesHackaton2017.Models;

namespace RiddlesHackaton2017.Moves
{
	public abstract class Move
	{
		public abstract Board Apply(Board board, Player player);

		public abstract string ToOutputString();

		internal static Move Parse(string v)
		{
			throw new NotImplementedException();
		}
	}
}
