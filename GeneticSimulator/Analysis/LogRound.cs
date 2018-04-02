using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using System;
using System.Text.RegularExpressions;

namespace GeneticSimulator.Analysis
{
	public class LogRound
	{
		//Round 1: Birthmove (15,9), sacrifice = (14,0) and (0,5) (40-40, gain2 = 150): score = 62 %, 
		//score2 = 1552, moves = 100 (67), simulations = 25, win in 8.00, loose in Infinity - 
		//Used 751 ms, Timelimit 10000 ms, Start 45.129, End 45.880
		public bool DirectWinMove { get; set; }
		public int Round { get; set; }
		public Move move { get; set; }
		public int MyPlayerFieldCount { get; set; }
		public int OpponentPlayerFieldCount { get; set; }
		public int Gain2 { get; set; }
		public double Score { get; set; }
		public int Score2 { get; set; }
		public int MoveCount { get; set; }
		public int BestMoveIndex { get; set; }
		public int SimulationCount { get; set; }
		public double WinRounds { get; set; }
		public double LooseRounds { get; set; }
		public TimeSpan UsedTime { get; set; }
		public TimeSpan TimeLimit { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		public static LogRound ParseV52(string s)
		{
			var result = ParseDirectWinMoveV52(s);
			if (result == null)
			{
				result = ParseMoveV52(s);
			}
			if (result == null)
			{
				throw new ArgumentException($"ParseV52: No match: {s}");
			}
			return result;
		}

	private static LogRound ParseDirectWinMoveV52(string s)
		{
			string patternDirectWinMove = @"Round \s (?<round>\d+): \s Direct \s win \s move \s-\s Used \s (?<usedms>\d+) \s ms, \s Timelimit \s (?<timelimitms>\d+) \s ms, \s Start \s (?<start>\d+\.\d+), \s End \s (?<end>\d+\.\d+)";
			var regexDirectWinMove = new Regex(patternDirectWinMove, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
			var match = regexDirectWinMove.Match(s);
			if (match.Success)
			{
				var logRound = new LogRound() { DirectWinMove = true };
				logRound.Round = int.Parse(match.Groups["round"].Value);
				logRound.UsedTime = TimeSpan.FromMilliseconds(int.Parse(match.Groups["usedms"].Value));
				logRound.TimeLimit = TimeSpan.FromMilliseconds(int.Parse(match.Groups["timelimitms"].Value));
				logRound.StartTime = DateTime.MinValue.AddSeconds(double.Parse(match.Groups["start"].Value));
				logRound.EndTime = DateTime.MinValue.AddSeconds(double.Parse(match.Groups["end"].Value));
				return logRound;
			}
			return null;
		}

		private static LogRound ParseMoveV52(string s)
		{

			string pattern = @"Round \s (?<round>\d+): \s (?<move>Birthmove \s \(\d+,\d+\), \s sacrifice \s = \s \(\d+,\d+\) \s and \s \(\d+,\d+\) | Killmove \s \(\d+,\d+\) | Passmove) \s \((?<myfieldcount>\d+)-(?<opponentfieldcount>\d+), \s gain2 \s=\s (?<gain2>(-)?\d+)\): \s score \s=\s (?<score>\d+)\s%, \s score2 \s=\s (?<score2>(-)?\d+), \s moves \s=\s (?<movecount>\d+) \s \((?<bestmoveindex>\d+)\), \s simulations \s=\s (?<simulationcount>\d+), \s win \s in \s (?<winmovecount>\d+\.\d+ | Infinity), \s loose \s in \s (?<loosemovecount>\d+\.\d+ | Infinity) \s-\s Used \s (?<usedms>\d+) \s ms, \s Timelimit \s (?<timelimitms>\d+) \s ms, \s Start \s (?<start>\d+\.\d+), \s End \s (?<end>\d+\.\d+)";
			var regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
			var match = regex.Match(s);
			if (match.Success)
			{
				var logRound = new LogRound();
				logRound.Round = int.Parse(match.Groups["round"].Value);
				logRound.move = ParseMove(match.Groups["move"].Value);
				logRound.MyPlayerFieldCount = int.Parse(match.Groups["myfieldcount"].Value);
				logRound.OpponentPlayerFieldCount = int.Parse(match.Groups["opponentfieldcount"].Value);
				logRound.Gain2 = int.Parse(match.Groups["gain2"].Value);
				logRound.Score = int.Parse(match.Groups["score"].Value) / 100.0;
				logRound.Score2 = int.Parse(match.Groups["score2"].Value);
				logRound.MoveCount = int.Parse(match.Groups["movecount"].Value);
				logRound.BestMoveIndex = int.Parse(match.Groups["bestmoveindex"].Value);
				logRound.SimulationCount = int.Parse(match.Groups["simulationcount"].Value);
				logRound.WinRounds = ParseDouble(match.Groups["winmovecount"].Value);
				logRound.LooseRounds = ParseDouble(match.Groups["loosemovecount"].Value);
				logRound.UsedTime = TimeSpan.FromMilliseconds(int.Parse(match.Groups["usedms"].Value));
				logRound.TimeLimit = TimeSpan.FromMilliseconds(int.Parse(match.Groups["timelimitms"].Value));
				logRound.StartTime = DateTime.MinValue.AddSeconds(double.Parse(match.Groups["start"].Value));
				logRound.EndTime = DateTime.MinValue.AddSeconds(double.Parse(match.Groups["end"].Value));
				return logRound;
			}
			return null;
		}

		private static double ParseDouble(string value)
		{
			if (value.Equals("Infinity", StringComparison.InvariantCultureIgnoreCase))
			{
				return double.PositiveInfinity;
			}
			else
			{
				return double.Parse(value);
			}
		}

		private static Move ParseMove(string value)
		{
			if (value.StartsWith("Birth", StringComparison.InvariantCultureIgnoreCase))
			{
				string pattern = @"Birthmove \s \( (?<bx>\d+),(?<by>\d+) \), \s sacrifice \s=\s \( (?<k1x>\d+), (?<k1y>\d+) \) \s and \s \( (?<k2x>\d+), (?<k2y>\d+) \)";
				var regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
				var match = regex.Match(value);
				if (match.Success)
				{
					var b = new Position(int.Parse(match.Groups["bx"].Value), int.Parse(match.Groups["by"].Value));
					var k1 = new Position(int.Parse(match.Groups["k1x"].Value), int.Parse(match.Groups["k1y"].Value));
					var k2 = new Position(int.Parse(match.Groups["k2x"].Value), int.Parse(match.Groups["k2y"].Value));
					return new BirthMove(b, k1, k2);
				}
				else
				{
					throw new ArgumentException($"ParseBirthMove: No match: {value}");
				}
			}
			else if (value.StartsWith("Pass", StringComparison.InvariantCultureIgnoreCase))
			{
				return new PassMove();
			}
			else if (value.StartsWith("Kill", StringComparison.InvariantCultureIgnoreCase))
			{
				string pattern = @"Killmove \s \( (?<x>\d+),(?<y>\d+) \)";
				var regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
				var match = regex.Match(value);
				if (match.Success)
				{
					var kill = new Position(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value));
					return new KillMove(kill);
				}
				else
				{
					throw new ArgumentException($"ParseBirthMove: No match: {value}");
				}
			}
			else
			{
				throw new ArgumentException($"ParseMove: No match: {value}");
			}
		}
	}
}
