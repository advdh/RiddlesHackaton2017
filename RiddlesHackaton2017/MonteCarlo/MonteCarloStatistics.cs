using RiddlesHackaton2017.Moves;

namespace RiddlesHackaton2017.MonteCarlo
{
	public class MonteCarloStatistics
	{
		public Move Move { get; set; }

		/// <summary>Number of played games</summary>
		public int Count { get; set; }

		/// <summary>Number of won games</summary>
		public int Won { get; set; }

		/// <summary>Number of lost games</summary>
		public int Lost { get; set; }

		public int Draw { get { return Count - Won - Lost; } }

		public double Score { get {  return (double)(2 * Won + Draw) / Count / 2; } }

		/// <summary>Total number of generations of all won simlations</summary>
		public int WonInGenerations { get; internal set; }

		/// <summary>Total number of generations of all lost simlations</summary>
		public int LostInGenerations { get; internal set; }

		/// <summary>Average number of generations until we win</summary>
		public int AverageWinRounds { get { return Won > 0 ? WonInGenerations / Won : int.MaxValue; } }

		/// <summary>Average number of generations until we loose</summary>
		public int AverageLooseRounds { get { return Lost > 0 ? LostInGenerations / Lost : int.MaxValue; } }

		public override string ToString()
		{
			return $"Move: {Move}: Count {Count}, Won {Won}, Lost {Lost}, Draw {Draw}, Score = {Score:P0}";
		}
	}
}
