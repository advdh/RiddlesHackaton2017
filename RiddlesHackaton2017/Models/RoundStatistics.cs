using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiddlesHackaton2017.Models
{
	public class RoundStatistic
	{
		public int Round { get; set; }
		public int MoveCount { get; set; }
		public int SimulationCount { get; set; }
		public TimeSpan MaxDuration { get; set; }
	}

	public class RoundStatistics : List<RoundStatistic>
	{
		public int GetSimulationCount(TimeSpan maxDuration, int minSimulationCount, int maxSimulationCount, int startSimulationCount,
			 double simulationFactor)
		{
			if (Count == 0) return startSimulationCount;

			int total = this.Sum(rs => rs.MoveCount * rs.SimulationCount);
			int totalDurationMs = this.Sum(rs => rs.MaxDuration.Milliseconds);
			double perMs = (double)total / totalDurationMs;

			int available = (int)(maxDuration.TotalMilliseconds * perMs);
			int result = (int)Math.Sqrt(simulationFactor * available); 
			if (result < minSimulationCount) result = minSimulationCount;
			if (result > maxSimulationCount) result = maxSimulationCount;
			return result;
		}
	}
}
