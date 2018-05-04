using System;
using System.Collections.Generic;
using System.Linq;

namespace RiddlesHackaton2017.Models
{
	public class RoundStatistic
	{
		public int Round { get; set; }
		public int MoveCount { get; set; }
		public int SimulationCount { get; set; }
		public TimeSpan UsedTime { get; set; }
	}

	public class RoundStatistics : List<RoundStatistic>
	{
		/// <summary>
		/// Measure for the current speed of the server
		/// </summary>
		public int SpeedIndex
		{
			get
			{
				int nominator = this.Sum(rs => rs.MoveCount * rs.SimulationCount);
				int denominator = (int)this.Sum(rs => rs.UsedTime.TotalMilliseconds);
				if (denominator == 0) return 0;
				return 1000 * nominator / denominator;
			}
		}

		public int GetSimulationCount(TimeSpan maxDuration, int maxSimulationCount, int startSimulationCount,
			 double simulationFactor)
		{
			if (Count == 0) return startSimulationCount;

			int total = this.Sum(rs => rs.MoveCount * rs.SimulationCount);
			int totalDurationMs = this.Sum(rs => rs.UsedTime.Milliseconds);
			double perMs = (double)total / totalDurationMs;

			int available = (int)(maxDuration.TotalMilliseconds * perMs);
			int result = (int)Math.Sqrt(simulationFactor * available); 
			if (result > maxSimulationCount) result = maxSimulationCount;
			return result;
		}
	}
}
