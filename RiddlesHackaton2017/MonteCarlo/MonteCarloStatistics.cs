using RiddlesHackaton2017.Moves;

namespace RiddlesHackaton2017.MonteCarlo
{
	public class MonteCarloStatistics
	{
		public Move Move { get; set; }

		/// <summary>Number of played games</summary>
		public int Count;

		/// <summary>Number of won games</summary>
		public int Won;

		/// <summary>Number of lost games</summary>
		public int Lost;

		public int Draw { get { return Count - Won - Lost; } }

		public double Score { get {  return (double)(2 * Won + Draw) / Count / 2; } }
		public double Score2 { get { return MyScore - OpponentScore; } }

		/// <summary>Total number of generations of all won simlations</summary>
		public int WonInGenerations;

		/// <summary>Total number of generations of all lost simlations</summary>
		public int LostInGenerations;

		/// <summary>Average number of generations until we win</summary>
		public double AverageWinGenerations { get { return Won > 0 ? (double)WonInGenerations / Won : double.PositiveInfinity; } }

		/// <summary>Average number of generations until we loose</summary>
		public double AverageLooseGenerations { get { return Lost > 0 ? (double)LostInGenerations / Lost : double.PositiveInfinity; } }

		/// <summary>Total of my fields during all simulations</summary>
		public int MyScore;
		/// <summary>Total of opponent fields during all simulations</summary>
		public int OpponentScore;

		public override string ToString()
		{
			return $"Move: {Move}: Count {Count}, Won {Won}, Lost {Lost}, Draw {Draw}, Score = {Score:P0}, MyScore = {MyScore}, OpponentScore = {OpponentScore}";
		}
	}
}
