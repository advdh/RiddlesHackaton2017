namespace RiddlesHackaton2017.MonteCarlo
{
	public class SimulationResult
	{
		public bool? Won { get; set; }

		/// <summary>Round number in which we win or loose, or MaxRounds if draw</summary>
		public int Round { get; set; }

		public SimulationResult(bool? won, int round)
		{
			Won = won;
			Round = round;
		}
	}
}
