using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace GeneticSimulator
{
	public enum ConfigurationFor
	{
		Both,
		Red,
		Blue,
	}

	public class Configuration
	{
		public Configuration()
		{

		}

		public Configuration(MonteCarloParameters monteCarloParameters)
		{
			Parameters = monteCarloParameters;
		}

		[XmlAttribute]
		public ConfigurationFor For { get; set; } = ConfigurationFor.Both;

		public MonteCarloParameters Parameters { get; set; }

		/// <summary>Number of played games</summary>
		public int Count { get { return Results1.Count + Results2.Count; } }

		/// <summary>Number of won games</summary>
		public int Won { get { return Results1.Count(r => r.Winner == Player.Player1) + Results2.Count(r => r.Winner == Player.Player2); } }

		/// <summary>Number of lost games</summary>
		public int Lost { get { return Results1.Count(r => r.Winner == Player.Player2) + Results2.Count(r => r.Winner == Player.Player1); } }

		/// <summary>Number of drawn games</summary>
		public int Draw { get { return Results1.Count(r => r.Winner == null) + Results2.Count(r => r.Winner == null); } }

		public double TotalScore1 { get { return Results1.Sum(r => r.Player1Score) + Results2.Sum(r => r.Player2Score); } }
		public double TotalScore2 { get { return Results1.Sum(r => r.Player2Score) + Results2.Sum(r => r.Player1Score); } }

		public double AverageScore1 { get { return (Results1.Sum(r => r.Player1Score) + Results2.Sum(r => r.Player2Score)) / Count; } }
		public double AverageScore2 { get { return (Results1.Sum(r => r.Player2Score) + Results2.Sum(r => r.Player1Score)) / Count; } }

		/// <summary>Results of games as player 1</summary>
		public List<GameResult> Results1 { get; } = new List<GameResult>();

		/// <summary>Results of games as player 2</summary>
		public List<GameResult> Results2 { get; } = new List<GameResult>();
	}
}
