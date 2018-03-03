using RiddlesHackaton2017.Models;
using System;
using System.Xml.Serialization;

namespace GeneticSimulator
{
	public class GameResult
	{
		public Player? Winner { get; set; }
		public int Rounds { get; set; }
		[XmlIgnore]
		public TimeSpan TimeBank1 { get; set; }
		public int TimeBank1ms { get { return (int)TimeBank1.TotalMilliseconds; } set { TimeBank1 = TimeSpan.FromMilliseconds(value); } }
		[XmlIgnore]
		public TimeSpan TimeBank2 { get; set; }
		public int TimeBank2ms { get { return (int)TimeBank2.TotalMilliseconds; } set { TimeBank2 = TimeSpan.FromMilliseconds(value); } }

		public bool Bot1TimedOut { get; set; }
		public bool Bot2TimedOut { get; set; }

		public override string ToString()
		{

			if (Winner == null)
			{
				return string.Format("Draw in {0}; timebanks {1}{2} / {3}{4}",
					Rounds, 
					TimeBank1ms, Bot1TimedOut ? " (timedout)" : "", 
					TimeBank2ms, Bot2TimedOut ? " (timed out)" : "");
			}
			else
			{
				return string.Format("Won by {5} in {0}; timebanks {1}{2} / {3}{4}",
					Rounds,
					TimeBank1ms, Bot1TimedOut ? " (timedout)" : "",
					TimeBank2ms, Bot2TimedOut ? " (timed out)" : "",
					Winner);
			}
		}
	}
}
