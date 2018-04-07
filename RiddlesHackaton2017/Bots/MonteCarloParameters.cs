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
			MaxWinBonus = original.MaxWinBonus;
			WinBonusDecrementFactor = original.WinBonusDecrementFactor;
			MaxWinBonus2 = original.MaxWinBonus2;
			WinBonusDecrementFactor2 = original.WinBonusDecrementFactor2;
			DoubleWinBonusCount = original.DoubleWinBonusCount;
			KillMovePercentage = original.KillMovePercentage;
			PassMovePercentage = original.PassMovePercentage;
			MinimumFieldCountForBirthMoves = original.MinimumFieldCountForBirthMoves;
			MaxDuration = original.MaxDuration;
			MaxRelativeDuration = original.MaxRelativeDuration;
			Debug = original.Debug;
			ValidateMoves = original.ValidateMoves;
			LogLevel = original.LogLevel;
			Throttle = original.Throttle;
			SimulationMaxGenerationCount = original.SimulationMaxGenerationCount;
			UseFastAndSmartMoveSimulator = original.UseFastAndSmartMoveSimulator;
			SmartMoveGenerationCount = original.SmartMoveGenerationCount;
			SmartMoveMinimumFieldCount = original.SmartMoveMinimumFieldCount;
			SmartMoveDurationThreshold = original.SmartMoveDurationThreshold;
			CellCountWeight = original.CellCountWeight;
			WinBonusWeight = original.WinBonusWeight;
			ParallelSimulation = original.ParallelSimulation;
			SimulationFactor = original.SimulationFactor;
			ScoreBasedOnWinBonus = original.ScoreBasedOnWinBonus;
			UseMoveGenerator2ForRed = original.UseMoveGenerator2ForRed;
			UseMoveGenerator2ForBlue = original.UseMoveGenerator2ForBlue;
			MoveGeneratorGenerationCount = original.MoveGeneratorGenerationCount;
			MoveGeneratorTopBirths = original.MoveGeneratorTopBirths;
			MoveGeneratorTopKills = original.MoveGeneratorTopKills;
			MoveGeneratorKeepFraction = original.MoveGeneratorKeepFraction;
		}

		/// <summary>Minimum number of simulations per move</summary>
		public int MinSimulationCount { get; set; } = 1;

		/// <summary>Maximum number of simulations per move</summary>
		public int MaxSimulationCount { get; set; } = 50;

		/// <summary>Initial number of simulations per move</summary>
		public int StartSimulationCount { get; set; } = 25;

		/// <summary>Multiplication factor to multiply available with before taking the square root to get simulation count</summary>
		public double SimulationFactor { get; set; } = 1.0;

		/// <summary>Number of moves</summary>
		public int MoveCount { get; set; } = 100;

		[XmlIgnore]
		public int[] WinBonus { get; set; }

		/// <summary>Maximum winbonus1 (for cellcount = 0)</summary>
		public int MaxWinBonus
		{
			get { return _MaxWinBonus; }
			set { _MaxWinBonus = value; CalculateWinBonus(); }
		}
		private int _MaxWinBonus = 128;

		/// <summary>
		/// Decrement factor of winbonus1 (winbonus1 for cellcount (n + 1) = WinBonusDecrementFactor1 * cellcount (n))
		/// </summary>
		/// <remarks>Typically between 0.0 and 1.0</remarks>
		public double WinBonusDecrementFactor
		{
			get { return _WinBonusDecrementFactor; }
			set { _WinBonusDecrementFactor = value; CalculateWinBonus(); }
		}
		private double _WinBonusDecrementFactor = 0.940;

		[XmlIgnore]
		public int[] WinBonus2 { get; set; }

		/// <summary>Maximum winbonus (for cellcount = 0)</summary>
		public int MaxWinBonus2
		{
			get { return _MaxWinBonus2; }
			set { _MaxWinBonus2 = value; CalculateWinBonus(); }
		}
		private int _MaxWinBonus2 = 128;

		/// <summary>
		/// Decrement factor of winbonus2 (winbonus2 for cellcount (n + 1) = WinBonusDecrementFactor2 * cellcount (n))
		/// </summary>
		/// <remarks>Typically between 0.0 and 1.0</remarks>
		public double WinBonusDecrementFactor2
		{
			get { return _WinBonusDecrementFactor2; }
			set { _WinBonusDecrementFactor2 = value; CalculateWinBonus(); }
		}
		private double _WinBonusDecrementFactor2 = 0.941;

		/// <summary>
		/// First number of win bonuses, which use double decrement factor
		/// </summary>
		public int DoubleWinBonusCount
		{
			get { return _DoubleWinBonusCount; }
			set { _DoubleWinBonusCount = value; CalculateWinBonus(); }
		}
		private int _DoubleWinBonusCount = 2;

		private void CalculateWinBonus()
		{
			CalculateWinBonus1();
			CalculateWinBonus2();
		}

		private void CalculateWinBonus1()
		{
			WinBonus = new int[Board.Size];
			double value = MaxWinBonus;
			for (int i = 0; i < WinBonus.Length; i++)
			{
				WinBonus[i] = (int)value;
				value *= WinBonusDecrementFactor;
			}
			for (int i = DoubleWinBonusCount - 1; i >= 0; i--)
			{
				WinBonus[i] = (int)(WinBonus[i + 1] / WinBonusDecrementFactor / WinBonusDecrementFactor);
			}
		}

		private void CalculateWinBonus2()
		{
			WinBonus2 = new int[Board.Size];
			double value = MaxWinBonus2;
			for (int i = 0; i < WinBonus2.Length; i++)
			{
				WinBonus2[i] = (int)value;
				value *= WinBonusDecrementFactor2;
			}
			for (int i = DoubleWinBonusCount - 1; i >= 0; i--)
			{
				WinBonus2[i] = (int)(WinBonus2[i + 1] / WinBonusDecrementFactor / WinBonusDecrementFactor);
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
		public TimeSpan MaxDuration
		{
			get { return Debug ? TimeSpan.FromDays(1) : _MaxDuration; }
			set { _MaxDuration = value; }
		}
		private TimeSpan _MaxDuration = TimeSpan.FromMilliseconds(400);

		/// <summary>
		/// MaxDuration in milliseconds: only for serialization purposes
		/// </summary>
		public int MaxDurationMs
		{
			get { return (int)MaxDuration.TotalMilliseconds; }
			set { MaxDuration = TimeSpan.FromMilliseconds(value); }
		}

		/// <summary>Maximum fraction of timelimit to be used</summary>
		public double MaxRelativeDuration
		{
			get { return Debug ? 1.0 : _MaxRelativeDuration; }
			set { _MaxRelativeDuration = value; }
		}
		private double _MaxRelativeDuration = 0.1;

		/// <summary>Set this to true for debugging the bot without any time limit constraints</summary>
		public bool Debug { get; set; } = false;

		public static MonteCarloParameters Life
		{
			get
			{
				return new MonteCarloParameters();
			}
		}

		public int LogLevel{ get; set; } = 0;

		/// <summary>Maximum number of generations during MonteCarlo simulation</summary>
		public int SimulationMaxGenerationCount { get; set; } = 8;

		/// <summary>
		/// Use FastAndSmartMoveSimulator instead of SimpleMoveSimulator
		/// </summary>
		public bool UseFastAndSmartMoveSimulator { get; set; } = false;

		/// <summary>Number of generations in which we use the smart move simulator</summary>
		/// <remarks>After this number of rounds, we switch to the simple move simulator</remarks>
		public int SmartMoveGenerationCount { get; set; } = 1;

		/// <summary>
		/// Minimum field count for smart move simulator: 
		/// below this number of field count for any of the players
		/// we switch to the smart move simulator
		/// </summary>
		public int SmartMoveMinimumFieldCount { get; set; } = 100;

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

		public bool ParallelSimulation
		{
			get { return !Debug && _ParallelSimulation; }
			set { _ParallelSimulation = value; }
		}
		private bool _ParallelSimulation = true;


		private bool _ValidateMoves = false;
		public bool ValidateMoves
		{
			get { return Debug || _ValidateMoves; }
			set { _ValidateMoves = value; }
		}

		/// <summary>
		/// Throttle factor for durations to simulate more realistically
		/// </summary>
		public double Throttle { get; set; } = 1;

		/// <summary>
		/// If true, then use winbonus for score calculation; if false, then use field counts for score calculation
		/// </summary>
		public bool ScoreBasedOnWinBonus { get; set; } = false;

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
			sb.AppendLine($"MaxWinBonus2 = {MaxWinBonus2}");
			sb.AppendLine($"WinBonusDecrementFactor2 = {WinBonusDecrementFactor2:0.000}");
			sb.AppendLine($"WinBonus2 = {string.Join(", ", WinBonus2.Where(i => i > 0))}");
			sb.AppendLine($"DoubleWinBonusCount = {DoubleWinBonusCount}");
			sb.AppendLine($"CellCountWeight = {CellCountWeight}");
			sb.AppendLine($"WinBonusWeight = {WinBonusWeight}");
			sb.AppendLine($"MaxDuration = {MaxDuration.TotalMilliseconds:0} ms");
			sb.AppendLine($"MaxRelativeDuration = {MaxRelativeDuration:P0}");
			sb.AppendLine($"PassMovePercentage = {PassMovePercentage}");
			sb.AppendLine($"KillMovePercentage = {KillMovePercentage}");
			sb.AppendLine($"BirthMovePercentage = {BirthMovePercentage}");
			sb.AppendLine($"MinimumFieldCountForBirthMoves = {MinimumFieldCountForBirthMoves}");
			sb.AppendLine($"SimulationMaxGenerationCount = {SimulationMaxGenerationCount}");
			sb.AppendLine($"UseFastAndSmartMoveSimulator = {UseFastAndSmartMoveSimulator}");
			sb.AppendLine($"SmartMoveGenerationCount = {SmartMoveGenerationCount}");
			sb.AppendLine($"SmartMoveMinimumFieldCount = {SmartMoveMinimumFieldCount}");
			sb.AppendLine($"SmartMoveDurationThreshold = {SmartMoveDurationThreshold.TotalMilliseconds:0} ms");
			sb.AppendLine($"ParallelSimulation = {ParallelSimulation}");
			sb.AppendLine($"SimulationFactor = {SimulationFactor}");
			sb.AppendLine($"ScoreBasedOnWinBonus = {ScoreBasedOnWinBonus}");
			sb.AppendLine($"UseMoveGenerator2Red = {UseMoveGenerator2ForRed}");
			sb.AppendLine($"UseMoveGenerator2Blue = {UseMoveGenerator2ForBlue}");
			sb.AppendLine($"MoveGeneratorGenerationCount = {MoveGeneratorGenerationCount}");
			sb.AppendLine($"MoveGeneratorTopBirths = {MoveGeneratorTopBirths}");
			sb.AppendLine($"MoveGeneratorTopKills = {MoveGeneratorTopKills}");
			sb.AppendLine($"MoveGeneratorKeepFraction = {MoveGeneratorKeepFraction}");
			sb.AppendLine($"Debug = {Debug}");
			sb.AppendLine($"ValidateMoves = {ValidateMoves}");
			sb.AppendLine($"LogLevel = {LogLevel}");
			sb.AppendLine($"Throttle = {Throttle}");
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
				&& MaxWinBonus2 == p.MaxWinBonus2
				&& WinBonusDecrementFactor2 == p.WinBonusDecrementFactor2
				&& DoubleWinBonusCount == p.DoubleWinBonusCount
				&& CellCountWeight == p.CellCountWeight
				&& WinBonusWeight == p.WinBonusWeight
				&& MaxDuration == p.MaxDuration
				&& MaxRelativeDuration == p.MaxRelativeDuration
				&& SmartMoveDurationThreshold == p.SmartMoveDurationThreshold
				&& PassMovePercentage == p.PassMovePercentage
				&& KillMovePercentage == p.KillMovePercentage
				&& SimulationMaxGenerationCount == p.SimulationMaxGenerationCount
				&& UseFastAndSmartMoveSimulator == p.UseFastAndSmartMoveSimulator
				&& SmartMoveGenerationCount == p.SmartMoveGenerationCount
				&& SmartMoveMinimumFieldCount == p.SmartMoveMinimumFieldCount
				&& MinimumFieldCountForBirthMoves == p.MinimumFieldCountForBirthMoves
				&& Debug == p.Debug
				&& LogLevel == p.LogLevel
				&& Throttle == p.Throttle
				&& ParallelSimulation == p.ParallelSimulation
				&& SimulationFactor == p.SimulationFactor
				&& ScoreBasedOnWinBonus == p.ScoreBasedOnWinBonus
				&& ValidateMoves == p.ValidateMoves
				&& UseMoveGenerator2ForRed == p.UseMoveGenerator2ForRed
				&& UseMoveGenerator2ForBlue == p.UseMoveGenerator2ForBlue
				&& MoveGeneratorGenerationCount == p.MoveGeneratorGenerationCount
				&& MoveGeneratorTopBirths == p.MoveGeneratorTopBirths
				&& MoveGeneratorTopKills == p.MoveGeneratorTopKills
				&& MoveGeneratorKeepFraction == p.MoveGeneratorKeepFraction;
		}

		public override int GetHashCode()
		{
			return MinSimulationCount.GetHashCode()
				^ MaxSimulationCount.GetHashCode()
				^ StartSimulationCount.GetHashCode()
				^ MoveCount.GetHashCode()
				^ MaxWinBonus.GetHashCode()
				^ WinBonusDecrementFactor.GetHashCode()
				^ MaxWinBonus2.GetHashCode()
				^ WinBonusDecrementFactor2.GetHashCode()
				^ DoubleWinBonusCount.GetHashCode()
				^ CellCountWeight.GetHashCode()
				^ WinBonusWeight.GetHashCode()
				^ MaxDuration.GetHashCode()
				^ MaxRelativeDuration.GetHashCode()
				^ SmartMoveDurationThreshold.GetHashCode()
				^ PassMovePercentage.GetHashCode()
				^ KillMovePercentage.GetHashCode()
				^ SimulationMaxGenerationCount.GetHashCode()
				^ UseFastAndSmartMoveSimulator.GetHashCode()
				^ SmartMoveGenerationCount.GetHashCode()
				^ SmartMoveMinimumFieldCount.GetHashCode()
				^ MinimumFieldCountForBirthMoves.GetHashCode()
				^ Debug.GetHashCode()
				^ LogLevel.GetHashCode()
				^ Throttle.GetHashCode()
				^ ParallelSimulation.GetHashCode()
				^ SimulationFactor.GetHashCode()
				^ ScoreBasedOnWinBonus.GetHashCode()
				^ ValidateMoves.GetHashCode()
				^ UseMoveGenerator2ForRed.GetHashCode()
				^ UseMoveGenerator2ForBlue.GetHashCode()
				^ MoveGeneratorGenerationCount.GetHashCode()
				^ MoveGeneratorTopBirths.GetHashCode()
				^ MoveGeneratorTopKills.GetHashCode()
				^ MoveGeneratorKeepFraction.GetHashCode();
		}

		private int _hashCode;
		public int HashCode { get { return GetHashCode(); } set { _hashCode = value; } }

		#region MoveGenerator2 properties

		/// <summary>Use MoveGenerator2 for Player1 (Red)</summary>
		public bool UseMoveGenerator2ForRed { get; set; } = true;

		/// <summary>Use MoveGenerator2 for Player2 (Blue)</summary>
		public bool UseMoveGenerator2ForBlue { get; set; } = false;

		/// <summary>Generation count in MoveGenerator: for births, kills and moves</summary>
		public int MoveGeneratorGenerationCount { get; set; } = 8;

		/// <summary>Initial number of births to keep</summary>
		public int MoveGeneratorTopBirths { get; set; } = 7;

		/// <summary>Initial number of kills to keep</summary>
		public int MoveGeneratorTopKills { get; set; } = 12;

		/// <summary>Fraction of moves to keep after each generation in MoveGenerator2</summary>
		public double MoveGeneratorKeepFraction { get; set; } = 0.8;

		#endregion
	}
}
