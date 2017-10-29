using System;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using RiddlesHackaton2017.Models;
using System.Linq;
using RiddlesHackaton2017.Moves;
using System.Diagnostics;

namespace RiddlesHackaton2017.Bots
{
	public class MonteCarloBot : BaseBot
	{
		public MonteCarloParameters Parameters { get; set; } = MonteCarloParameters.Default;

		private readonly IRandomGenerator Random;

		public MonteCarloBot(IConsole consoleError, IRandomGenerator random) : base(consoleError)
		{
			Random = Guard.NotNull(random, nameof(random));
		}

		/// <summary>
		/// Returns the maximum allowed duration for simulations 
		/// = minimum of "according to parameter" and
		/// timeLimit / 4
		/// </summary>
		private TimeSpan GetMaxDuration(TimeSpan timeLimit)
		{
			return new TimeSpan(Math.Min(timeLimit.Ticks / 4, Parameters.MaxDuration.Ticks));
		}

		public override Move GetMove()
		{
			int count = 0;
			double bestScore = double.MinValue;
			Move bestMove = null;

			var stopwatch = Stopwatch.StartNew();
			TimeSpan duration = GetMaxDuration(TimeLimit);
			bool goOn = true;

			while (goOn)
			{
				//Get random move and simulate rest of the game several times
				var move = GetRandomMove(Board, Board.MyPlayer);
				double score = SimulateMove(Board, move);
				if (Parameters.LogAllMoves)
				{
					ConsoleError.WriteLine("{0}: {1:P0}: direct impact: {2}", 
						move, score, move.DirectImpactForBoard(Board));
				}
				if (score > bestScore)
				{
					bestScore = score;
					bestMove = move;
				}

				count++;

				goOn = stopwatch.Elapsed < duration && count < Parameters.MoveCount;
			}

			//Log and return
			LogMessage = string.Format("Move {0}: score = {1:P0}, evaluated moves = {2}", 
				bestMove, bestScore, count);

			return bestMove;
		}

		/// <summary>
		/// Simulates the specified move a number of times using MonteCarlo simulation
		/// </summary>
		/// <returns>Score for this move</returns>
		private double SimulateMove(Board board, Move move)
		{
			var statistic = new MonteCarloStatistics() { Move = move };

			for (int i = 0; i < Parameters.SimulationCount; i++)
			{
				var myBoard = Board.CopyAndPlay(board, board.MyPlayer, move);
				bool? won = SimulateRestOfGame(myBoard);

				statistic.Count++;
				if (won.HasValue && won.Value) statistic.Won++;
				if (won.HasValue && !won.Value) statistic.Lost++;
			}

			return statistic.Score;
		}

		/// <summary>
		/// Simulates one game to the end
		/// </summary>
		/// <returns>true if won, false if lost, null if draw</returns>
		private bool? SimulateRestOfGame(Board board)
		{
			var player = board.OpponentPlayer;
			while (board.Round < Board.MaxRounds)
			{
				//Bot play
				Move move = GetRandomMove(board, player);
				board = Board.CopyAndPlay(board, player, move);
				bool anyHis = Enumerable.Range(0, Models.Board.Size).Any(i => board.Field[i] == (short)Board.OpponentPlayer);
				if (!anyHis) return true;
				bool anyMine = Enumerable.Range(0, Models.Board.Size).Any(i => board.Field[i] == (short)Board.MyPlayer);
				if (!anyMine) return false;

				//Next player
				player = player.Opponent();
			}

			return null;
		}

		private Move GetRandomMove(Board board, Player player)
		{
			int rnd = Random.Next(100);
			if (rnd < Parameters.PassMovePercentage)
			{
				//With probability 1% we do a pass move
				return new PassMove();
			}
			else if (rnd < Parameters.PassMovePercentage + Parameters.KillMovePercentage)
			{
				//With probability 49% we do a kill move
				return GetRandomKillMove(board, player);
			}
			else
			{
				//With probability 50% we do a birth move
				return GetRandomBirthMove(board, player);
			}
		}

		public KillMove GetRandomKillMove(Board board, Player player)
		{
			var mine = board.GetCells(player);
			var his = board.GetCells(player.Opponent());
			var possible = mine.Union(his).ToArray();
			return new KillMove(possible[Random.Next(possible.Length)]);
		}

		public Move GetRandomBirthMove(Board board, Player player)
		{
			var mine = board.GetCells(player).ToArray();
			if (mine.Count() < 2)
			{
				//Only one cell left: cannot do a birth move
				//Switch to pass move
				return new PassMove();
			}

			//Pick one empty cell for birth
			var empty = board.EmptyCells.ToArray();
			int b = empty[Random.Next(empty.Length)];

			//Pick two cells of my own to sacrifice
			int s1, s2;
			if (mine.Length == 2)
			{
				s1 = mine.First();
				s2 = mine.Last();
			}
			else
			{
				s1 = mine[Random.Next(mine.Length)];
				do
				{
					s2 = mine[Random.Next(mine.Length)];
				}
				while (s2 == s1);
			}

			return new BirthMove(b, s1, s2);
		}
	}
}
