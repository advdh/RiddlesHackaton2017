using System;

namespace RiddlesHackaton2017.IntegrationTest
{
	public class Game
	{
		public string Id { get; set; }
		public string Player0 { get; set; }
		public int Version0 { get; set; }
		public string Player1 { get; set; }
		public int Version1 { get; set; }
		public DateTime PlayedDate { get; set; }
		public int? Winner { get; set; }
		public int Rounds { get; set; }
		public string GameData { get; set; }
	}
}
