using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.Output;
using System;

namespace RiddlesHackaton2017.Bots
{
	public abstract class BaseBot
	{
		protected IConsole ConsoleError;
		protected Board Board;
		protected TimeSpan TimeLimit;

		protected DateTime StartTime { get; set; }
		protected string LogMessage { get; set; }

		protected BaseBot(IConsole consoleError)
		{
			ConsoleError = Guard.NotNull(consoleError, nameof(consoleError));
		}

		public abstract Move GetMove();

		public string GetMove(Board board)
		{
			return GetMove(board, TimeSpan.FromSeconds(10));
		}

		public string GetMove(Board board, TimeSpan timeLimit)
		{
			Board = Guard.NotNull(board, nameof(board));
			TimeLimit = timeLimit;
			StartTime = DateTime.UtcNow;
			LogMessage = null;
			var move = GetMove();
			DateTime endTime = DateTime.UtcNow;
			ConsoleError.WriteLine("Round {0}: timelimit {1:0} ms, StartTime {2:ss.fff}, EndTime {3:ss.fff}, used {4:0} ms: {5}",
				Board.Round, timeLimit.TotalMilliseconds, StartTime, endTime, endTime.Subtract(StartTime).TotalMilliseconds, LogMessage);

			return move.ToOutputString();
		}
	}
}
