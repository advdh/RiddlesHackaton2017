using System;
using System.Text;

namespace RiddlesHackaton2017.Bots
{
	public class MonteCarloParameters
	{
		/// <summary>Minimum number of simulations per move</summary>
		public int MinSimulationCount { get; set; } = 5;

		/// <summary>Maximum number of simulations per move</summary>
		public int MaxSimulationCount { get; set; } = 25;

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

		/// <summary>If true, then use fixed time (the life version), if false, then use fixed number of moves (MoveCount)</summary>
		/// <remarks>
		/// For debugging purposes, timebound should be turned off
		/// Also for replaying, it's better to turn it off, because local bot is much faster than live bot
		/// </remarks>
		public TimeSpan MaxDuration { get; set; } = TimeSpan.FromMilliseconds(500);

		/// <summary>Maximum fraction of timelimit to be used</summary>
		public double MaxRelativeDuration { get; set; } = 0.2;

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

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"MinSimulationCount = {MinSimulationCount}");
			sb.AppendLine($"MaxSimulationCount = {MaxSimulationCount}");
			sb.AppendLine($"MoveCount = {MoveCount}");
			sb.AppendLine($"WinBonus = {string.Join(", ", WinBonus)}");
			sb.AppendLine($"MaxDuration = {MaxDuration.TotalMilliseconds:0} ms");
			sb.AppendLine($"MaxRelativeDuration = {MaxRelativeDuration:P0}");
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
