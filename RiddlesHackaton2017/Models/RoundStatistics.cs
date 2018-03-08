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
		public int GetSimulationCount(TimeSpan maxDuration, int minSimulationCount, int maxSimulationCount, int startSimulationCount)
		{
			if (Count == 0) return startSimulationCount;

			int total = this.Sum(rs => rs.MoveCount * rs.SimulationCount);
			int totalDurationMs = this.Sum(rs => rs.MaxDuration.Milliseconds);
			double perMs = (double)total / totalDurationMs;

			int result = GetSimulationCount((int)(maxDuration.TotalMilliseconds * perMs));
			if (result < minSimulationCount) result = minSimulationCount;
			if (result > maxSimulationCount) result = maxSimulationCount;
			return result;
		}

		public int GetSimulationCount(int total)
		{
			//18: 9 moves, 2 simulations
			if (total < 20) return 2;
			//36: ±4 simulations
			//72: ±4 simulations
			if (total < 100) return 4;
			//144: ±8 simulations
			if (total < 200) return 6;
			//216: ±9 simulations
			if (total < 300) return 9;
			//324: ±9 simulations
			if (total < 400) return 10;
			//676: ±10-13 simulations
			//1200: ±12-24 simulations
			//2500: ±17-25 simulations

			return Math.Max(13, total / 100);
		}
	}
}
