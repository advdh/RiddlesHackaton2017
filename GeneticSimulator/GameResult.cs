using RiddlesHackaton2017.Models;
using System;

namespace GeneticSimulator
{
	public class GameResult
	{
		public Player? Winner { get; set; }
		public int Rounds { get; set; }
		public TimeSpan TimeBank1 { get; set; }
		public TimeSpan TimeBank2 { get; set; }

		public override string ToString()
		{
			if (Winner == null)
			{
				return $"Draw in {Rounds}; timebanks {TimeBank1.TotalMilliseconds} / {TimeBank2.TotalMilliseconds}";
			}
			else
			{
				return $"Won by {Winner.Value} in {Rounds}; timebanks {TimeBank1.TotalMilliseconds} / {TimeBank2.TotalMilliseconds}";
			}
		}
	}
}
