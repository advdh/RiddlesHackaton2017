using RiddlesHackaton2017.Models;
using System;
using System.Xml.Serialization;

namespace GeneticSimulator
{
	public class GameResult
	{
		/// <summary>Hashcode of parameter1</summary>
		public int Parameters1Hash { get; set; }
		/// <summary>Hashcode of parameter2</summary>
		public int Parameters2Hash { get; set; }

		public Player? Winner { get; set; }
		public double Player1Score { get; set; }
		public double Player2Score { get; set; }

		public int Rounds { get; set; }
		[XmlIgnore]
		public TimeSpan TimeBank1 { get; set; }
		public int TimeBank1ms { get { return (int)TimeBank1.TotalMilliseconds; } set { TimeBank1 = TimeSpan.FromMilliseconds(value); } }
		[XmlIgnore]
		public TimeSpan TimeBank2 { get; set; }
		public int TimeBank2ms { get { return (int)TimeBank2.TotalMilliseconds; } set { TimeBank2 = TimeSpan.FromMilliseconds(value); } }

		public bool Bot1TimedOut { get; set; }
		public bool Bot2TimedOut { get; set; }

		[XmlIgnore]
		public Board Board { get; set; }

		public override string ToString()
		{

			if (Winner == null)
			{
				return string.Format("Draw in {0}; Scores {5:0.000} / {6:0.000}; timebanks {1}{2} / {3}{4}",
					Rounds, 
					TimeBank1ms, Bot1TimedOut ? " (timedout)" : "", 
					TimeBank2ms, Bot2TimedOut ? " (timed out)" : "",
					Player1Score, Player2Score);
			}
			else
			{
				return string.Format("Won by {7} in {0}; Scores {5:0.000} / {6:0.000}; timebanks {1}{2} / {3}{4}",
					Rounds,
					TimeBank1ms, Bot1TimedOut ? " (timedout)" : "",
					TimeBank2ms, Bot2TimedOut ? " (timed out)" : "",
					Player1Score, Player2Score,
					Winner);
			}
		}
	}
}
