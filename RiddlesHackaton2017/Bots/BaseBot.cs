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
			ConsoleError.WriteLine("Round {0}: {1} - Used {2:0} ms, Timelimit {3:0} ms, Start {4:ss.fff}, End {5:ss.fff}",
				Board.Round, LogMessage, endTime.Subtract(StartTime).TotalMilliseconds, timeLimit.TotalMilliseconds, StartTime, endTime);

			return move.ToOutputString();
		}
	}
}
