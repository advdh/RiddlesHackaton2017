using RiddlesHackaton2017.Models;
using System.Collections.Generic;
using System.Linq;

namespace RiddlesHackaton2017.Moves
{
	public class PassMove : Move
	{
		public override IEnumerable<int> AffectedFields
		{
			get
			{
				return Enumerable.Empty<int>();
			}
		}

		public override Board Apply(Board board, Player player, bool validate = true)
		{
			return board;
		}

		public override string ToOutputString()
		{
			return "pass";
		}

		public static bool TryParse(string s, out Move move)
		{
			if (s == "pass")
			{
				move = new PassMove();
				return true;
			}
			move = new NullMove();
			return false;
		}

		public override string ToString()
		{
			return "PassMove";
		}
	}
}
