namespace RiddlesHackaton2017.Bots
{
	class MonteCarloStatistics
	{
		public int Move { get; set; }

		/// <summary>
		/// Number of played games
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Number of won games
		/// </summary>
		public int Won { get; set; }

		public override string ToString()
		{
			return string.Format("Move: {0}: Count {1}, Won {2} ({3:P0})",
				Move, Count, Won, (double)Won / Count);
		}
	}
}
