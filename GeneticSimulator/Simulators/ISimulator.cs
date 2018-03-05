using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticSimulator.Simulators
{
	public interface ISimulator
	{
		/// <summary>
		/// Generates a set of configurations
		/// </summary>
		/// <param name="generator">The configuration generator</param>
		/// <param name="populationSize">Population size</param>
		Configurations Generate(ConfigurationGenerator generator, int populationSize);

		/// <summary>
		/// Simulates the specified configurations
		/// </summary>
		/// <param name="configurations">Set of configurations</param>
		void Simulate(Configurations configurations, int simulationCount);
	}
}
