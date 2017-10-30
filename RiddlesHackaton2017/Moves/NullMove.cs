using System;
using System.Collections.Generic;
using RiddlesHackaton2017.Models;
using System.Linq;

namespace RiddlesHackaton2017.Moves
{
	public class NullMove : Move
	{
		public override IEnumerable<int> AffectedFields
		{
			get
			{
				return Enumerable.Empty<int>();
			}
		}

		public override Board Apply(Board board, Player player)
		{
			throw new NotImplementedException();
		}

		public override string ToOutputString()
		{
			return string.Empty;
		}

		public static bool TryParse(string s, out Move move)
		{
			move = new NullMove();
			return (s == string.Empty);
		}

		public override string ToString()
		{
			return "NullMove";
		}
	}
}
