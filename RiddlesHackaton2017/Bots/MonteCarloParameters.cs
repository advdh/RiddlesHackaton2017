using System;
using System.Text;

namespace RiddlesHackaton2017.Bots
{
	public class MonteCarloParameters
	{
		/// <summary>Number of simulations per move</summary>
		public int SimulationCount { get; set; } = 25;

		/// <summary>Number of moves</summary>
		public int MoveCount { get; set; } = 100;

		public int[] WinBonus { get; set; } = new[] { 128, 64, 32, 16, 8, 4, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

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

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"SimulationCount = {SimulationCount}");
			sb.AppendLine($"MoveCount = {MoveCount}");
			sb.AppendLine($"WinBonus = {string.Join(", ", WinBonus)}");
			sb.AppendLine($"MaxDuration = {MaxDuration.TotalMilliseconds:0} ms");
			sb.AppendLine($"PassMovePercentage = {PassMovePercentage}");
			sb.AppendLine($"KillMovePercentage = {KillMovePercentage}");
			sb.AppendLine($"BirthMovePercentage = {BirthMovePercentage}");
			sb.AppendLine($"MinimumFieldCountForBirthMoves = {MinimumFieldCountForBirthMoves}");
			sb.AppendLine($"Debug = {Debug}");
			sb.AppendLine($"LogLevel = {LogLevel}");
			return sb.ToString();
		}
	}
}
