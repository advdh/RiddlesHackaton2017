using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using System;
using System.Linq;

namespace GeneticSimulator
{
	[TestClass]
	public class Analysis
	{
		[TestMethod]
		public void AnalyzeResults()
		{
			var path = @"d:\temp\GeneticSimulator_0.xml";
			var results = Generations.Load(path)
				.OrderByDescending(r => r.Won)
				.ThenByDescending(r => r.Draw);
			int ix = 1;
			foreach(var result in results)
			{
				Console.WriteLine($"{ix}.\tCount={result.Count}\tWon={result.Won}\tDraw={result.Draw}\tLost={result.Lost}\tParameters={result.Parameters}");
				ix++;
			}
		}
	}
}
