using System;
using System.Linq;
using System.Text;

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
		public int KillMovePercentage { get; internal set; } = 49;
		/// <summary>Percentage of pass moves in random move and simulation</summary>
		public int PassMovePercentage { get; internal set; } = 1;
		/// <summary>Percentage of birth moves in random move and simulation</summary>
		public int BirthMovePercentage { get { return 100 - KillMovePercentage - PassMovePercentage; } }

		/// <summary>
		/// Minimum field count in order for players to execute birth moves in the monte-carlo simulation.
		/// If player has less than this minimum field count, then he only executes kill moves
		/// </summary>
		public int MinimumFieldCountForBirthMoves { get; internal set; } = 10;

		/// <summary>Maximum time to spend per turn</summary>
		public TimeSpan MaxDuration { get; set; } = TimeSpan.FromMilliseconds(500);

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

		public static MonteCarloParameters Life
		{
			get
			{
				return new MonteCarloParameters()
				{
					MaxDuration = TimeSpan.FromMilliseconds(800),
					MoveCount = 100,
				};
			}
		}

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
		public TimeSpan SmartMoveDurationThreshold { get; set; } = TimeSpan.FromMilliseconds(80);

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
			sb.AppendLine($"SmartMoveSimulatorGenerationCount = {SmartMoveGenerationCount}");
			sb.AppendLine($"SmartMoveMinimumFieldCount = {SmartMoveMinimumFieldCount}");
			sb.AppendLine($"MinimumFieldCountForBirthMoves = {MinimumFieldCountForBirthMoves}");
			sb.AppendLine($"Debug = {Debug}");
			sb.AppendLine($"LogLevel = {LogLevel}");
			return sb.ToString();
		}
	}
}
