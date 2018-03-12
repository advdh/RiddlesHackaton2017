using System;

namespace RiddlesHackaton2017.IntegrationTest
{
	public class Anila8Game
	{
		public string Id { get; set; }
		public int Version { get; set; }
		public string Opponent { get; set; }
		public string Log { get; set; }
		public DateTime PlayedDate { get; set; }
		public int? Won { get; set; }
		public int Rounds { get; set; }
		public string GameData { get; set; }

		public string WonString
		{
			get
			{
				if (Won.HasValue && Won.Value == 1) return "Won";
				if (Won.HasValue && Won.Value == 0) return "Lost";
				return "Draw";
			}
		}

		/// <summary>
		/// Player 0 or 1
		/// </summary>
		public int Player { get; set; }
	}
}
