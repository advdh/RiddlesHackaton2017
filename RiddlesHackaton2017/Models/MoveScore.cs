using RiddlesHackaton2017.Moves;

namespace RiddlesHackaton2017.Models
{
	public class MoveScore
	{
		public Move Move { get; private set; }
		public int Gain2 { get; private set; }

		public MoveScore(Move move, int score)
		{
			Move = move;
			Gain2 = score;
		}

		public override string ToString()
		{
			return $"Gain: {Gain2} : {Move}";
		}
	}
}
