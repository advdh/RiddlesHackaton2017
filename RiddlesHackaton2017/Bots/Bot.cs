using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Output;
using System;
using System.Diagnostics;
using System.Linq;

namespace RiddlesHackaton2017.Bots
{
	public class Bot
	{
		private readonly IConsole ConsoleError;

		private Board Board;

		/// <summary>Start time of my turn</summary>
		private DateTime StartTime;

		/// <summary>End time of my turn</summary>
		private DateTime EndTime;

		public Bot() : this(new ConsoleError())
		{
		}

		public Bot(IConsole consoleError)
		{
			ConsoleError = consoleError;
		}

		public string GetMove(Board board)
		{
			return GetMove(board, TimeSpan.FromSeconds(10));
		}

		public string GetMove(Board board, TimeSpan timeLimit)
		{
			int move = Move(board, timeLimit);
			throw new NotImplementedException();
		}

		public int Move(Board board, TimeSpan timeLimit)
		{
			StartTime = DateTime.UtcNow;
			Board = board;
			return GetMove(timeLimit);
		}

		private TimeSpan GetMaxDuration(int round, TimeSpan timeLimit)
		{
			int ms = (int)timeLimit.TotalMilliseconds;
			int maxMs = Math.Min(Math.Max(5 * round, 350), ms / 4);
			return TimeSpan.FromMilliseconds(maxMs);
		}

		public int GetMove(TimeSpan timeLimit)
		{
			var maxDuration = GetMaxDuration(Board.Round, timeLimit);
			var stopwatch = Stopwatch.StartNew();

			//Alpha-beta
			var alphaBetaNode = GetAlphaBetaMove(maxDuration);
			LogAlphaBetaMove(timeLimit, (int)stopwatch.ElapsedMilliseconds, alphaBetaNode);
			return alphaBetaNode.PlayFieldFromRoot;
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

		private void LogAlphaBetaMove(TimeSpan timeLimit, int elapsedMilliseconds, Node endNode)
		{
			EndTime = DateTime.UtcNow;
			ConsoleError.WriteLine("Round {0}: timelimit {1:0} ms, StartTime {7:ss.fff}, EndTime {8:ss.fff}, used {2:0} ms, depth {3}, max depth {4}, Evaluations: {5}: {6}",
				Board.Round, timeLimit.TotalMilliseconds, elapsedMilliseconds,
				endNode.Depth, endNode.Root.MaxDepth, endNode.Root.Evaluations, endNode,
				StartTime, EndTime);
		}
	}
}
