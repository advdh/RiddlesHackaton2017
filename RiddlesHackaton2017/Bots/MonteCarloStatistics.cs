using RiddlesHackaton2017.Moves;

namespace RiddlesHackaton2017.Bots
{
	class MonteCarloStatistics
	{
		public Move Move { get; set; }

		/// <summary>
		/// Number of played games
		/// </summary>
		public int Count { get; set; }

		/// <summary>Number of won games</summary>
		public int Won { get; set; }

		/// <summary>Number of lost games</summary>
		public int Lost { get; set; }

		public int Draw { get { return Count - Won - Lost; } }

		public double Score { get {  return (double)(2 * Won + Draw) / Count / 2; } }

		/// <summary>Total number of rounds of all won simlations, counted from current board</summary>
		public int WonInRounds { get; internal set; }

		/// <summary>Total number of rounds of all lost simlations, counted from current board</summary>
		public int LostInRounds { get; internal set; }

		/// <summary>Average number of rounds until we win</summary>
		public int AverageWinRounds { get { return Won > 0 ? WonInRounds / Won : int.MaxValue; } }

		/// <summary>Average number of rounds until we loose</summary>
		public int AverageLooseRounds { get { return Lost > 0 ? LostInRounds / Lost : int.MaxValue; } }

		public override string ToString()
		{
			return $"Move: {Move}: Count {Count}, Won {Won}, Lost {Lost}, Draw {Draw}, Score = {Score:P0}";
		}
	}
}
