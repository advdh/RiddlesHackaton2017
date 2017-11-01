﻿using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
			MonteCarloStatistics bestResult = new MonteCarloStatistics()
			{
				Count = Parameters.SimulationCount,
				Won = -1,
				Lost = Parameters.SimulationCount,
				LostInRounds = Parameters.SimulationCount,
			};
			Move bestMove = GetDirectWinMove();
			if (bestMove != null)
			{
				LogMessage = "Direct win move";
				return bestMove;
			}

			var stopwatch = Stopwatch.StartNew();
			TimeSpan duration = GetMaxDuration(TimeLimit);
			bool goOn = true;

			var candidateMoves = GetCandidateMoves(Parameters.MoveCount).ToArray();

			int count = 0;
			while (goOn)
			{
				//Get random move and simulate rest of the game several times
				var move = candidateMoves[count];
				var result = SimulateMove(Board, move);
				if (Parameters.LogAllMoves)
				{
					ConsoleError.WriteLine("{0}: {1:P0}: direct impact: {2}, win in {3}, loose in {4}", 
						move, result.Score, move.DirectImpactForBoard(Board),
						result.AverageWinRounds, result.AverageLooseRounds);
				}
				if (result.Score > bestResult.Score)
				{
					//Prefer higher score
					bestResult = result;
					bestMove = move;
				}
				else if (result.Score == bestResult.Score)
				{
					//Same score
					if (result.Score >= 0.50)
					{
						//Prefer to win in less rounds
						if (result.AverageWinRounds < bestResult.AverageWinRounds)
						{
							bestResult = result;
							bestMove = move;
						}
					}
					else
					{
						//Prefer to loose in more rounds
						if (result.AverageLooseRounds > bestResult.AverageLooseRounds)
						{
							bestResult = result;
							bestMove = move;
						}
					}
				}

				count++;

				goOn = stopwatch.Elapsed < duration && count < candidateMoves.Length;
			}

			//Log and return
			LogMessage = string.Format("{0}: score = {1:P0}, evaluated moves = {2}, win in {3}, loose in {4}", 
				bestMove, bestResult.Score, count, bestResult.AverageWinRounds, bestResult.AverageLooseRounds);

			return bestMove;
		}

		/// <summary>
		/// Generates birth moves and kill moves in such a way that two birth moves 
		/// have never more than 1 move part (birth/kill) in common
		/// </summary>
		/// <returns>Sorted collection of moves</returns>
		private IEnumerable<Move> GetCandidateMoves(int maxCount)
		{
			var result = new List<Move>();

			var myKills = GetMyKillMoves().OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
			var opponentKills = GetOpponentKills().OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
			var myBirths = GetBirths().OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();

			var bkHashes = new HashSet<int>();
			var kkHashes = new HashSet<int>();

			result.Add(new PassMove());
			int count = 1;

			bool quit = false;
			for (int i = 0; i < Math.Max(myKills.Length, myBirths.Length); i++)
			{
				quit = true;
				for (int b = 0; b <= i && b < myBirths.Length; b++)
				{
					for (int k1 = 0; k1 <= i && k1 < myKills.Length; k1++)
					{
						for (int k2 = k1 + 1; k2 <= i + 1 && k2 < myKills.Length; k2++)
						{
							if (!bkHashes.Contains(b + 256 * k1)
								&& !bkHashes.Contains(b + 256 * k2)
								&& !kkHashes.Contains(k1 + 256 + k2))
							{
								bkHashes.Add(b + 256 * k1);
								bkHashes.Add(b + 256 * k2);
								kkHashes.Add(k1 + 256 + k2);
								result.Add(new BirthMove(myBirths[b], myKills[k1], myKills[k2]));
								count++;
								quit = false;
								if (count >= maxCount) return result;
							}
						}
					}
				}
				if (i < opponentKills.Length)
				{
					result.Add(new KillMove(opponentKills[i]));
					count++;
					quit = false;
					if (count >= maxCount) return result;
				}
				if (quit) break;
			}

			return result;
		}

		/// <summary>
		/// Tries to get a direct win move
		/// </summary>
		/// <returns>Direct win move, if there is any, otherwise null</returns>
		/// <remarks>TODO: Move out of this class; it has nothing to do with Monte Carlo</remarks>
		private Move GetDirectWinMove()
		{
			var opponentCells = Board.OpponentCells;
			if (opponentCells.Count() == 1)
			{
				return new KillMove(opponentCells.First());
			}
			return null;
		}

		/// <summary>
		/// Simulates the specified move a number of times using MonteCarlo simulation
		/// </summary>
		/// <returns>Score for this move</returns>
		private MonteCarloStatistics SimulateMove(Board board, Move move)
		{
			var statistic = new MonteCarloStatistics() { Move = move };
			var startBoard = Board.CopyAndPlay(board, board.MyPlayer, move);
			bool anyHis = Enumerable.Range(0, Board.Size).Any(i => startBoard.Field[i] == (short)Board.OpponentPlayer);
			if (!anyHis)
			{
				statistic.Count = Parameters.SimulationCount;
				statistic.Won = Parameters.SimulationCount;
				statistic.WonInRounds = Parameters.SimulationCount;
				return statistic;
			}
			bool anyMine = Enumerable.Range(0, Board.Size).Any(i => startBoard.Field[i] == (short)Board.MyPlayer);
			if (!anyMine)
			{
				statistic.Count = Parameters.SimulationCount;
				statistic.Lost = Parameters.SimulationCount;
				statistic.LostInRounds = Parameters.SimulationCount;
				return statistic;
			}

			for (int i = 0; i < Parameters.SimulationCount; i++)
			{
				var myBoard = new Board(startBoard);
				var result = SimulateRestOfGame(myBoard);

				statistic.Count++;
				if (result.Won.HasValue && result.Won.Value)
				{
					statistic.Won++;
					statistic.WonInRounds += (result.Round - startBoard.Round);
				}
				if (result.Won.HasValue && !result.Won.Value)
				{
					statistic.Lost++;
					statistic.LostInRounds += (result.Round - startBoard.Round);
				}
			}

			return statistic;
		}

		/// <summary>
		/// Simulates one game to the end
		/// </summary>
		/// <returns>true if won, false if lost, null if draw</returns>
		private SimulationResult SimulateRestOfGame(Board board)
		{
			var player = board.OpponentPlayer;
			while (board.Round < Board.MaxRounds)
			{
				//Bot play
				Move move = GetRandomMove(board, player);
				board = Board.CopyAndPlay(board, player, move);
				bool anyHis = Enumerable.Range(0, Board.Size).Any(i => board.Field[i] == (short)Board.OpponentPlayer);
				if (!anyHis) return new SimulationResult(won: true, round: board.Round);
				bool anyMine = Enumerable.Range(0, Board.Size).Any(i => board.Field[i] == (short)Board.MyPlayer);
				if (!anyMine) return new SimulationResult(won: false, round: board.Round);

				//Next player
				player = player.Opponent();
			}

			return new SimulationResult(won: null, round: Board.MaxRounds);
		}

		private class SimulationResult
		{
			public bool? Won { get; set; }

			/// <summary>Round number in which we win or loose, or MaxRounds if draw</summary>
			public int Round { get; set; }

			public SimulationResult(bool? won, int round)
			{
				Won = won;
				Round = round;
			}
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
			var opponentCells = board.GetCells(player.Opponent()).ToArray();
			return new KillMove(opponentCells[Random.Next(opponentCells.Length)]);
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
			//Don't pick an empty cell without any neighbours
			var empty = board.EmptyCells
				.Where(c => Board.NeighbourFields[c]
					.Any(nc => board.Field[nc] != 0))
				.ToArray();
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
		
		/// <summary>Gets a dictionary of kills on one of my cells with their scores</summary>
		public Dictionary<int, int> GetMyKillMoves()
		{
			var result = new Dictionary<int, int>();
			
			var mine = Enumerable.Range(0, Board.Size).Where(i => Board.Field[i] == (short)Board.MyPlayer);

			foreach (int i in mine)
			{
				var move = new KillMove(i);
				var newBoard = Models.Board.CopyAndPlay(Board, Board.MyPlayer, move);
				int score = BoardEvaluator.Evaluate(newBoard);
				result.Add(move.Index, score);
			}
			return result;
		}

		/// <summary>Gets a dictionary of kills on opponent's cells with their scores</summary>
		public Dictionary<int, int> GetOpponentKills()
		{
			var result = new Dictionary<int, int>();
			
			var his = Enumerable.Range(0, Models.Board.Size).Where(i => Board.Field[i] == (short)Board.OpponentPlayer);

			foreach (int i in his)
			{
				var move = new KillMove(i);
				var newBoard = Models.Board.CopyAndPlay(Board, Board.MyPlayer, move);
				int score = BoardEvaluator.Evaluate(newBoard);
				result.Add(move.Index, score);
			}
			return result;
		}

		/// <summary>Gets a dictionary of births (not birth moves) with their scores</summary>
		public Dictionary<int, int> GetBirths()
		{
			var result = new Dictionary<int, int>();
			
			var empty = Enumerable.Range(0, Models.Board.Size).Where(i => Board.Field[i] == 0);

			foreach (int i in empty)
			{
				var newBoard = new Models.Board(Board);
				newBoard.Field[i] = (short)Board.MyPlayer;
				newBoard = Board.NextGeneration(newBoard);
				int score = BoardEvaluator.Evaluate(newBoard);
				result.Add(i, score);
			}
			return result;
		}
	}
}
