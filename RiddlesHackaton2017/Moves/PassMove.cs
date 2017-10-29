using RiddlesHackaton2017.Models;

namespace RiddlesHackaton2017.Moves
{
	public class PassMove : Move
	{
		public override Board Apply(Board board, Player player)
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
