using System;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using System.Collections.Generic;
using System.Diagnostics;
using RiddlesHackaton2017.Models;
using System.Linq;

namespace RiddlesHackaton2017.Bots
{
	public class MonteCarloBot : BaseBot
	{
		private readonly IRandomGenerator Random;

		public MonteCarloBot(IConsole consoleError, IRandomGenerator random) : base(consoleError)
		{
			Random = Guard.NotNull(random, nameof(random));
		}

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
			var statistics = new Dictionary<int, MonteCarloStatistics>();
			var stopwatch = Stopwatch.StartNew();
			int count = 0;

			var allMoves = Board.GetFeasibleMovesForPlayer(Board.MyPlayer).ToArray();
			if (allMoves.Length == 1) return allMoves[0];

			TimeSpan duration = GetMaxDuration(TimeLimit);
			while (stopwatch.Elapsed < duration)
			//while (count < 2000)
			{
				//Play move and simulate rest of game
				var myBoard = new Board(Board);

				int move = allMoves[count % allMoves.Length];

				myBoard = Board.CopyAndPlay(myBoard, myBoard.MyPlayer, move);
				bool won = SimulateRestOfGame(myBoard);

				//Update statistics
				MonteCarloStatistics statistic = null;
				if (!statistics.ContainsKey(move))
				{
					statistic = new MonteCarloStatistics() { Move = move };
					statistics.Add(move, statistic);
				}
				else
				{
					statistic = statistics[move];
				}

				statistic.Count++;
				if (won) statistic.Won++;
				//statistic.SumMyBearedOff += board.BearedOff[0];
				//statistic.SumOpponentBearedOff += board.BearedOff[1];

				count++;
			}

			//Select moves with best results
			//Currently best probability to win
			var orderedStatistics = statistics
				.OrderByDescending(s => (double)s.Value.Won / s.Value.Count);
			var best = orderedStatistics.FirstOrDefault().Value;
			Console.WriteLine("Board: {0}, Move {1} ({2:P0}), {3} games simulated, {4} options",
				Board, best.Move, (double)best.Won / best.Count, count, statistics.Count);
			return best.Move;
		}

		private bool SimulateRestOfGame(Board board)
		{
			bool endOfGame = false; //TODO
			var bot1 = new MonteCarloRestOfGameBot(new NullConsole(), new RandomGenerator(new Random()));
			var bot2 = new MonteCarloRestOfGameBot(new NullConsole(), new RandomGenerator(new Random()));
			board = board.OpponentBoard;

			Player myPlayer = board.OpponentPlayer;
			while (!endOfGame)
			{
				var bot = board.MyPlayer == Player.Player1 ? bot1 : bot2;
				//var dice = Dice.Roll(Random);

				//Bot play
				int move = int.Parse(bot.GetMove(new Board(board), TimeSpan.Zero));
				board = Board.CopyAndPlay(board, board.MyPlayer, move);
				endOfGame = false; //TODO
				if (endOfGame) break;

				//Next player
				myPlayer = myPlayer.Opponent();
				board = board.OpponentBoard;
			}

			return true; //TODO: return winner
		}
	}
}
