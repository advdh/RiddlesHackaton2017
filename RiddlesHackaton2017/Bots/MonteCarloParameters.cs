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

		/// <summary>Percentage of kill moves in random move and simulation</summary>
		public int KillMovePercentage { get; internal set; } = 49;
		/// <summary>Percentage of pass moves in random move and simulation</summary>
		public int PassMovePercentage { get; internal set; } = 1;
		/// <summary>Percentage of birth moves in random move and simulation</summary>
		public int BirthMovePercentage { get { return 100 - KillMovePercentage - PassMovePercentage; } }

		/// <summary>Debug flag: if true, then output all attempted moves</summary>
		public bool LogAllMoves { get; set; } = false;

		/// <summary>If true, then use fixed time (the life version), if false, then use fixed number of moves (MoveCount)</summary>
		/// <remarks>
		/// For debugging purposes, timebound should be turned off
		/// Also for replaying, it's better to turn it off, because local bot is much faster than live bot
		/// </remarks>
		public TimeSpan MaxDuration { get; set; } = TimeSpan.FromMilliseconds(500);

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

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("SimulationCount = {0}", SimulationCount); sb.AppendLine();
			sb.AppendFormat("MoveCount = {0}", MoveCount); sb.AppendLine();
			sb.AppendFormat("MaxDuration = {0:0} ms", MaxDuration.TotalMilliseconds); sb.AppendLine();
			sb.AppendFormat("PassMovePercentage = {0}", PassMovePercentage); sb.AppendLine();
			sb.AppendFormat("KillMovePercentage = {0}", KillMovePercentage); sb.AppendLine();
			sb.AppendFormat("BirthMovePercentage = {0}", BirthMovePercentage); sb.AppendLine();
			sb.AppendFormat("LogAllMoves = {0}", LogAllMoves); sb.AppendLine();
			return sb.ToString();
		}
	}
}
