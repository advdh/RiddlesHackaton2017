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

		public override string ToString()
		{
			return "PassMove";
		}
	}
}
