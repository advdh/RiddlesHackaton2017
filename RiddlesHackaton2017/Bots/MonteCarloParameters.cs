namespace RiddlesHackaton2017.Bots
{
	public class MonteCarloParameters
	{
		/// <summary>Number of simulations per move</summary>
		public int SimulationCount { get; set; } = 10;

		/// <summary>Number of moves</summary>
		public int MoveCount { get; set; } = 10;

		/// <summary>Percentage of kill moves in random move and simulation</summary>
		public int KillMovePercentage { get; internal set; } = 49;
		/// <summary>Percentage of pass moves in random move and simulation</summary>
		public int PassMovePercentage { get; internal set; } = 1;
		/// <summary>Percentage of birth moves in random move and simulation</summary>
		public int BirthMovePercentage { get { return 100 - KillMovePercentage - PassMovePercentage; } }

		/// <summary>Debug flag: if true, then output all attempted moves</summary>
		public bool LogAllMoves { get; set; } = false;

		public static MonteCarloParameters Default
		{
			get
			{
				return new MonteCarloParameters();
			}
		}
	}
}
