namespace RiddlesHackaton2017.MonteCarlo
{
	public class SimulationResult
	{
		public bool? Won { get; set; }

		/// <summary>Number of simulated generations till the end of the game</summary>
		public int GenerationCount { get; set; }

		public SimulationResult(bool? won, int generationCount)
		{
			Won = won;
			GenerationCount = generationCount;
		}
	}
}
