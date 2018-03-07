namespace RiddlesHackaton2017.MonteCarlo
{
	public class SimulationResult
	{
		public bool? Won { get; set; }

		/// <summary>Number of simulated generations till the end of the game</summary>
		public int GenerationCount { get; set; }

		/// <summary>Total number of player1's fields of all rounds durint the simulation</summary>
		public int MyScore { get; set; }

		/// <summary>Total number of player2's fields of all rounds durint the simulation</summary>
		public int OpponentScore { get; set; }

		public SimulationResult(bool? won, int generationCount, int myScore, int opponentScore)
		{
			Won = won;
			GenerationCount = generationCount;
			MyScore = myScore;
			OpponentScore = opponentScore;
		}
	}
}
