using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Output;
using System;

namespace RiddlesHackaton2017.Bots
{
	public class AlphaBetaBot : BaseBot
	{
		public AlphaBetaBot() : this(new ConsoleError())
		{
		}

		public AlphaBetaBot(IConsole consoleError) : base(consoleError) { }

		/// <summary>
		/// Returns the maximum duration of one turn = max. 350 ms or 
		/// timeLimit / 4, if we have limited time
		/// </summary>
		private TimeSpan GetMaxDuration(TimeSpan timeLimit)
		{
			int ms = (int)timeLimit.TotalMilliseconds;
			int maxMs = Math.Min(350, ms / 4);
			return TimeSpan.FromMilliseconds(maxMs);
		}

		public override int GetMove()
		{
			var maxDuration = GetMaxDuration(TimeLimit);

			//Alpha-beta
			var endNode = GetAlphaBetaMove(maxDuration);
			LogMessage = string.Format("depth {0}, max depth {1}, Evaluations: {2}: {3}",
				endNode.Depth, endNode.Root.MaxDepth, endNode.Root.Evaluations, endNode);
			return endNode.PlayFieldFromRoot;
		}

		/// <summary>
		/// Gets best alpha beta move
		/// </summary>
		/// <returns>Alpha beta end node</returns>
		private Node GetAlphaBetaMove(TimeSpan maxDuration)
		{
			Node endNode = AlphaBetaEvaluator.AlphaBeta(Board, maxDuration);
			return endNode;
		}
	}
}
