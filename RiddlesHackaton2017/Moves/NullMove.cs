using System;
using RiddlesHackaton2017.Models;

namespace RiddlesHackaton2017.Moves
{
	public class NullMove : Move
	{
		public override Board Apply(Board board, Player player)
		{
			throw new NotImplementedException();
		}

		public override string ToOutputString()
		{
			return string.Empty;
		}
	}
}
