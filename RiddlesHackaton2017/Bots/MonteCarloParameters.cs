using RiddlesHackaton2017.Models;
using System;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RiddlesHackaton2017.Bots
{
	public class MonteCarloParameters
	{
		public MonteCarloParameters()
		{
			CalculateWinBonus();
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		public MonteCarloParameters(MonteCarloParameters original)
		{
			MinSimulationCount = original.MinSimulationCount;
			MaxSimulationCount = original.MaxSimulationCount;
			StartSimulationCount = original.StartSimulationCount;
			MoveCount = original.MoveCount;
			WinBonusDecrementFactor = original.WinBonusDecrementFactor;
			MaxWinBonus = original.MaxWinBonus;
			KillMovePercentage = original.KillMovePercentage;
			PassMovePercentage = original.PassMovePercentage;
			MinimumFieldCountForBirthMoves = original.MinimumFieldCountForBirthMoves;
			MaxDuration = original.MaxDuration;
			MaxRelativeDuration = original.MaxRelativeDuration;
			Debug = original.Debug;
			LogLevel = original.LogLevel;
			SimulationMaxGenerationCount = original.SimulationMaxGenerationCount;
			SmartMoveGenerationCount = original.SmartMoveGenerationCount;
			SmartMoveMinimumFieldCount = original.SmartMoveMinimumFieldCount;
			SmartMoveDurationThreshold = original.SmartMoveDurationThreshold;
			CellCountWeight = original.CellCountWeight;
			WinBonusWeight = original.WinBonusWeight;
			BinarySimulationResult = original.BinarySimulationResult;
			SimulationDecrementScore2Factor = original.SimulationDecrementScore2Factor;
			ParallelSimulation = original.ParallelSimulation;
		}

		/// <summary>Minimum number of simulations per move</summary>
		public int MinSimulationCount { get; set; } = 5;

		/// <summary>Maximum number of simulations per move</summary>
		public int MaxSimulationCount { get; set; } = 50;

		/// <summary>Initial number of simulations per move</summary>
		public int StartSimulationCount { get; set; } = 25;


		/// <summary>Number of moves</summary>
		public int MoveCount { get; set; } = 100;

		[XmlIgnore]
		public int[] WinBonus { get; set; }

		/// <summary>Maximum winbonus (for cellcount = 0)</summary>
		public int MaxWinBonus
		{
			get { return _MaxWinBonus; }
			set { _MaxWinBonus = value; CalculateWinBonus(); }
		}
		private int _MaxWinBonus = 128;

		/// <summary>
		/// Decrement factor of winbonus (winbonus for cellcount (n + 1) = WinBonusDecrementFactor * cellcount (n))
		/// </summary>
		/// <remarks>Typically between 0.0 and 1.0</remarks>
		public double WinBonusDecrementFactor
		{
			get { return _WinBonusDecrementFactor; }
			set { _WinBonusDecrementFactor = value; CalculateWinBonus();}
		}
		private double _WinBonusDecrementFactor = 0.916;

		private void CalculateWinBonus()
		{
			WinBonus = new int[Board.Size];
			double value = MaxWinBonus;
			for (int i = 0; i < WinBonus.Length; i++)
			{
				WinBonus[i] = (int)value;
				value *= WinBonusDecrementFactor;
			}
		}

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
		public int SimulationMaxGenerationCount { get; set; } = 8;

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

		/// <summary>Relative weight of absolute cellcounts in score calcultion</summary>
		public int WinBonusWeight { get; set; } = 10;


		/// <summary>
		/// If true, then simulation result is based on won / lost games only;
		/// If false, then simulation result is based on number of fields owned by both players during simulated rounds
		/// </summary>
		public bool BinarySimulationResult { get; set; } = false;

		/// <summary>
		/// Decrement factor, with which the number of fields adds to the score.
		/// Example if factor = 0.9 then: generation 0: 100 fields = score 100; generation 1: 100 fields = score 90; generation 2; 100 fields = sore 81, etc.
		/// </summary>
		public double SimulationDecrementScore2Factor { get; set; } = 1.0;

		public bool ParallelSimulation { get; set; } = true;

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"MinSimulationCount = {MinSimulationCount}");
			sb.AppendLine($"MaxSimulationCount = {MaxSimulationCount}");
			sb.AppendLine($"StartSimulationCount = {StartSimulationCount}");
			sb.AppendLine($"MoveCount = {MoveCount}");
			sb.AppendLine($"MaxWinBonus = {MaxWinBonus}");
			sb.AppendLine($"WinBonusDecrementFactor = {WinBonusDecrementFactor:0.000}");
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
			sb.AppendLine($"BinarySimulationResult = {BinarySimulationResult}");
			sb.AppendLine($"SimulationDecrementScore2Factor = {SimulationDecrementScore2Factor}");
			sb.AppendLine($"ParallelSimulation = {ParallelSimulation}");
			sb.AppendLine($"Debug = {Debug}");
			sb.AppendLine($"LogLevel = {LogLevel}");
			return sb.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) return false;

			var p = (MonteCarloParameters)obj;
			return MinSimulationCount == p.MinSimulationCount
				&& MaxSimulationCount == p.MaxSimulationCount
				&& StartSimulationCount == p.StartSimulationCount
				&& MoveCount == p.MoveCount
				&& MaxWinBonus == p.MaxWinBonus
				&& WinBonusDecrementFactor == p.WinBonusDecrementFactor
				&& CellCountWeight == p.CellCountWeight
				&& WinBonusWeight == p.WinBonusWeight
				&& MaxDuration == p.MaxDuration
				&& MaxRelativeDuration == p.MaxRelativeDuration
				&& SmartMoveDurationThreshold == p.SmartMoveDurationThreshold
				&& PassMovePercentage == p.PassMovePercentage
				&& KillMovePercentage == p.KillMovePercentage
				&& SimulationMaxGenerationCount == p.SimulationMaxGenerationCount
				&& SmartMoveGenerationCount == p.SmartMoveGenerationCount
				&& SmartMoveMinimumFieldCount == p.SmartMoveMinimumFieldCount
				&& MinimumFieldCountForBirthMoves == p.MinimumFieldCountForBirthMoves
				&& Debug == p.Debug
				&& LogLevel == p.LogLevel
				&& BinarySimulationResult == p.BinarySimulationResult
				&& SimulationDecrementScore2Factor == p.SimulationDecrementScore2Factor
				&& ParallelSimulation == p.ParallelSimulation;
		}

		public override int GetHashCode()
		{
			return MinSimulationCount.GetHashCode()
				^ MaxSimulationCount.GetHashCode()
				^ StartSimulationCount.GetHashCode()
				^ MoveCount.GetHashCode()
				^ MaxWinBonus.GetHashCode()
				^ WinBonusDecrementFactor.GetHashCode()
				^ CellCountWeight.GetHashCode()
				^ WinBonusWeight.GetHashCode()
				^ MaxDuration.GetHashCode()
				^ MaxRelativeDuration.GetHashCode()
				^ SmartMoveDurationThreshold.GetHashCode()
				^ PassMovePercentage.GetHashCode()
				^ KillMovePercentage.GetHashCode()
				^ SimulationMaxGenerationCount.GetHashCode()
				^ SmartMoveGenerationCount.GetHashCode()
				^ SmartMoveMinimumFieldCount.GetHashCode()
				^ MinimumFieldCountForBirthMoves.GetHashCode()
				^ Debug.GetHashCode()
				^ LogLevel.GetHashCode()
				^ BinarySimulationResult.GetHashCode()
				^ SimulationDecrementScore2Factor.GetHashCode()
				^ ParallelSimulation.GetHashCode();
		}

		private int _hashCode;
		public int HashCode { get { return GetHashCode(); } set { _hashCode = value; } }
	}
}
