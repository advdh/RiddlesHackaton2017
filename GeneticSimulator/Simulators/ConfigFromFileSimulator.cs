using System;
using System.IO;

namespace GeneticSimulator.Simulators
{
	public class ConfigFromFileSimulator : BaseSimulator
	{
		public const string BasePath = @"d:\temp\GeneticSimulator";

		private string ConfigurationsFilename { get; set; }

		public ConfigFromFileSimulator(string commandLine) : base(commandLine)
		{
			string[] words = commandLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			if (words.Length > 3)
			{
				ConfigurationsFilename = Path.Combine(BasePath, words[3]);
			}
		}

		public override Configurations Generate(ConfigurationGenerator generator, int populationSize)
		{
			return Configurations.Load(ConfigurationsFilename);
		}
	}
}
