using System;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RiddlesHackaton2017.Bots
{
	public class MonteCarloParameters
	{
		/// <summary>Minimum number of simulations per move</summary>
		public int MinSimulationCount { get; set; } = 5;

		/// <summary>Maximum number of simulations per move</summary>
		public int MaxSimulationCount { get; set; } = 50;

		/// <summary>Initial number of simulations per move</summary>
		public int StartSimulationCount { get; set; } = 25;


		/// <summary>Number of moves</summary>
		public int MoveCount { get; set; } = 100;

		public int[] WinBonus { get; set; } = new[] { 128, 91, 64, 45, 32, 23, 16, 11, 8, 6, 4, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

		/// <summary>Percentage of kill moves in random move and simulation</summary>
		public int KillMovePercentage { get; set; } = 49;
		/// <summary>Percentage of pass moves in random move and simulation</summary>
		public int PassMovePercentage { get; set; } = 1;
		/// <summary>Percentage of birth moves in random move and simulation</summary>
		public int BirthMovePercentage { get { return 100 - KillMovePercentage - PassMovePercentage; } }

		/// <summary>
		/// Minimum field count in order for players to execute birth moves in the monte-carlo simulation.
		/// If player has less than this minimum field count, then he only executes kill moves
		/// </summary>
		public int MinimumFieldCountForBirthMoves { get; set; } = 10;

		/// <summary>Maximum time to spend per turn</summary>
		[XmlIgnore]
		public TimeSpan MaxDuration { get; set; } = TimeSpan.FromMilliseconds(500);

		/// <summary>
		/// MaxDuration in milliseconds: only for serialization purposes
		/// </summary>
		public int MaxDurationMs
		{
			get
			{
				return (int)MaxDuration.TotalMilliseconds;
			}
			set
			{
				MaxDuration = TimeSpan.FromMilliseconds(value);
			}
		}

		/// <summary>Maximum fraction of timelimit to be used</summary>
		public double MaxRelativeDuration { get; set; } = 0.1;

		/// <summary>Set this to true for debugging the bot without any time limit constraints</summary>
		public bool Debug { get; set; } = false;

		public static MonteCarloParameters Default
		{
			get
			{
				return new MonteCarloParameters();
			}
		}

		public static MonteCarloParameters Life => new MonteCarloParameters()
		{
			MinSimulationCount = 25,
			MaxSimulationCount = 47,
			StartSimulationCount = 26,
			MoveCount = 46,
			WinBonus = new[] {67, 58, 50, 44, 38, 33, 28, 25, 21, 19, 16, 14, 12, 10, 9, 8, 7, 6, 5, 4, 4, 3, 3, 2, 2, 2, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
			CellCountWeight = 5,
			WinBonusWeight = 5,
			MaxDuration = TimeSpan.FromMilliseconds(1577),
			MaxRelativeDuration = 0.09,
			SmartMoveDurationThreshold = TimeSpan.FromMilliseconds(465),
			PassMovePercentage = 39,
			KillMovePercentage = 30,
			SimulationMaxGenerationCount = 13,
			SmartMoveGenerationCount = 58,
			SmartMoveMinimumFieldCount = 10,
			MinimumFieldCountForBirthMoves = 8,
			Debug = false,
			LogLevel = 0,
		};

		public int LogLevel{ get; set; } = 0;

		/// <summary>Maximum number of generations during MonteCarlo simulation</summary>
		public int SimulationMaxGenerationCount { get; set; } = 30;

		/// <summary>Number of generations in which we use the smart move simulator</summary>
		/// <remarks>After this number of rounds, we switch to the simple move simulator</remarks>
		public int SmartMoveGenerationCount { get; set; } = 4;

		/// <summary>
		/// Minimum field count for smart move simulator: 
		/// below this number of field count for any of the players
		/// we switch to the smart move simulator
		/// </summary>
		public int SmartMoveMinimumFieldCount { get; set; } = 15;

		/// <summary>Minimum allowed move duration in order to execute a smart move</summary>
		[XmlIgnore]
		public TimeSpan SmartMoveDurationThreshold { get; set; } = TimeSpan.FromMilliseconds(80);

		/// <summary>
		/// MaxDuration in milliseconds: only for serialization purposes
		/// </summary>
		public int SmartMoveDurationThresholdMs
		{
			get
			{
				return (int)SmartMoveDurationThreshold.TotalMilliseconds;
			}
			set
			{
				SmartMoveDurationThreshold = TimeSpan.FromMilliseconds(value);
			}
		}

		/// <summary>Relative weight of difference of cellcounts in score calcultion</summary>
		public int CellCountWeight { get; set; } = 10;

		/// <summary>Relative weight of difference of cellcounts in score calcultion</summary>
		public int WinBonusWeight { get; set; } = 10;

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"MinSimulationCount = {MinSimulationCount}");
			sb.AppendLine($"MaxSimulationCount = {MaxSimulationCount}");
			sb.AppendLine($"StartSimulationCount = {StartSimulationCount}");
			sb.AppendLine($"MoveCount = {MoveCount}");
			sb.AppendLine($"WinBonus = {string.Join(", ", WinBonus.Where(i => i > 0))}");
			sb.AppendLine($"CellCountWeight = {CellCountWeight}");
			sb.AppendLine($"WinBonusWeight = {WinBonusWeight}");
			sb.AppendLine($"MaxDuration = {MaxDuration.TotalMilliseconds:0} ms");
			sb.AppendLine($"MaxRelativeDuration = {MaxRelativeDuration:P0}");
			sb.AppendLine($"SmartMoveDurationThreshold = {SmartMoveDurationThreshold.TotalMilliseconds:0} ms");
			sb.AppendLine($"PassMovePercentage = {PassMovePercentage}");
			sb.AppendLine($"KillMovePercentage = {KillMovePercentage}");
			sb.AppendLine($"BirthMovePercentage = {BirthMovePercentage}");
			sb.AppendLine($"SimulationMaxGenerationCount = {SimulationMaxGenerationCount}");
			sb.AppendLine($"SmartMoveGenerationCount = {SmartMoveGenerationCount}");
			sb.AppendLine($"SmartMoveMinimumFieldCount = {SmartMoveMinimumFieldCount}");
			sb.AppendLine($"MinimumFieldCountForBirthMoves = {MinimumFieldCountForBirthMoves}");
			sb.AppendLine($"Debug = {Debug}");
			sb.AppendLine($"LogLevel = {LogLevel}");
			return sb.ToString();
		}
	}
}
