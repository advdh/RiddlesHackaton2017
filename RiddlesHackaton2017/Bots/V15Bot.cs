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
	public class V15Bot : BaseBot
	{
		public MonteCarloParameters Parameters { get; set; } = MonteCarloParameters.Default;

		private readonly IRandomGenerator Random;

		public V15Bot(IConsole consoleError, IRandomGenerator random) : base(consoleError)
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
			if (Parameters.Debug) return Parameters.MaxDuration;
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

			var moveGeneratorStopwatch = Stopwatch.StartNew();
			var candidateMoves = GetCandidateMoves(Parameters.MoveCount).ToArray();
			if (Parameters.LogLevel >= 1)
			{
				ConsoleError.WriteLine($"MoveGeneration: {moveGeneratorStopwatch.ElapsedMilliseconds:0} ms");
			}

			if (Parameters.LogLevel >= 2)
			{
				int ix = 0;
				foreach (var move in candidateMoves)
				{
					ConsoleError.WriteLine($"  {ix}: {move}");
					ix++;
				}
			}

			int count = 0;
			while (goOn)
			{
				//Get random move and simulate rest of the game several times
				var move = candidateMoves[count];
				var result = SimulateMove(Board, move);
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

			var myKills1 = GetMyKills().OrderByDescending(kvp => kvp.Value);
			var myKills = myKills1.Select(k => k.Key).ToArray();
			var opponentKills1 = GetOpponentKills().OrderByDescending(kvp => kvp.Value);
			var opponentKills = opponentKills1.Select(k => k.Key).ToArray();
			var myBirths1 = GetBirths().OrderByDescending(kvp => kvp.Value);
			var myBirths = myBirths1.Select(b => b.Key).ToArray();

			if (Parameters.LogLevel >= 3)
			{
				ConsoleError.WriteLine("MyKills:");
				int ix = 0;
				foreach (var kill in myKills1)
				{
					ConsoleError.WriteLine($"  {ix} ({kill.Value.Score}): {new Position(kill.Key)}");
					ix++;
				}
				ConsoleError.WriteLine("OpponentKills:");
				ix = 0;
				foreach (var kill in opponentKills1)
				{
					ConsoleError.WriteLine($"  {ix} ({kill.Value.Score}): {new Position(kill.Key)}");
					ix++;
				}
				ConsoleError.WriteLine("Births:");
				ix = 0;
				foreach (var birth in myBirths1)
				{
					ConsoleError.WriteLine($"  {ix} ({birth.Value.Score}): {new Position(birth.Key)}");
					ix++;
				}
			}

			var bkHashes = new HashSet<int>();
			var kkHashes = new HashSet<int>();

			result.Add(new PassMove());
			for (int k2 = 1; k2 < Math.Min(myKills.Length, myBirths.Length); k2++)
			{
				for (int b = 0; b < k2 && b < myBirths.Length; b++)
				{
					if (!bkHashes.Contains(b + 256 * k2))
					{
						for (int k1 = 0; k1 < k2 && k1 < myKills.Length - 1; k1++)
						{
							if (!bkHashes.Contains(b + 256 * k1)
								&& !kkHashes.Contains(k1 + 256 + k2))
							{
								bkHashes.Add(b + 256 * k1);
								bkHashes.Add(b + 256 * k2);
								kkHashes.Add(k1 + 256 + k2);
								result.Add(new BirthMove(myBirths[b], myKills[k1], myKills[k2]));
								if (result.Count >= maxCount) return result;
								break;
							}
						}
					}
				}
				if (k2 <= opponentKills.Length)
				{
					result.Add(new KillMove(opponentKills[k2 - 1]));
					if (result.Count >= maxCount) return result;
				}
			}
			for (int k2 = Math.Min(myKills.Length, myBirths.Length) - 1; k2 < opponentKills.Length; k2++)
			{
				result.Add(new KillMove(opponentKills[k2]));
				if (result.Count >= maxCount) return result;
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
			var startBoard = board.ApplyMoveAndNext(board.MyPlayer, move);
			if (startBoard.OpponentPlayerFieldCount == 0)
			{
				if (startBoard.MyPlayerFieldCount == 0)
				{
					//Draw in 1
					statistic.Count = Parameters.SimulationCount;
					return statistic;
				}
				else
				{
					//Won in 1
					statistic.Count = Parameters.SimulationCount;
					statistic.Won = Parameters.SimulationCount;
					statistic.WonInRounds = Parameters.SimulationCount;
					return statistic;
				}
			}
			if (startBoard.MyPlayerFieldCount == 0)
			{
				//Lost in 1
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
				board = board.ApplyMoveAndNext(player, move);
				if (board.OpponentPlayerFieldCount == 0) return new SimulationResult(won: true, round: board.Round);
				if (board.MyPlayerFieldCount == 0) return new SimulationResult(won: false, round: board.Round);

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
			//If player has only a few cells left, then do only kill moves
			if (board.GetFieldCount(player) < Parameters.MinimumFieldCountForBirthMoves)
			{
				return GetRandomKillMove(board, player);
			}

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
		public Dictionary<int, BoardStatus> GetMyKills()
		{
			var result = new Dictionary<int, BoardStatus>();

			foreach (int i in Board.MyCells)
			{
				var newBoard = new Board(Board);
				newBoard.Field[i] = 0;
				newBoard.MyPlayerFieldCount--;
				newBoard = newBoard.NextGeneration.NextGeneration;
				var score = BoardEvaluator.Evaluate(newBoard);
				result.Add(i, score);
			}
			return result;
		}

		/// <summary>Gets a dictionary of kills on opponent's cells with their scores</summary>
		public Dictionary<int, BoardStatus> GetOpponentKills()
		{
			var result = new Dictionary<int, BoardStatus>();

			foreach (int i in Board.OpponentCells)
			{
				var newBoard = new Board(Board);
				newBoard.Field[i] = 0;
				newBoard.OpponentPlayerFieldCount--;
				newBoard = newBoard.NextGeneration.NextGeneration;
				var score = BoardEvaluator.Evaluate(newBoard);
				result.Add(i, score);
			}
			return result;
		}

		/// <summary>Gets a dictionary of births (not birth moves) with their scores</summary>
		public Dictionary<int, BoardStatus> GetBirths()
		{
			var result = new Dictionary<int, BoardStatus>();

			foreach (int i in Board.EmptyCells)
			{
				var newBoard = new Board(Board);
				newBoard.Field[i] = (short)Board.MyPlayer;
				newBoard.MyPlayerFieldCount++;
				newBoard = newBoard.NextGeneration.NextGeneration;
				var score = BoardEvaluator.Evaluate(newBoard);
				result.Add(i, score);
			}
			return result;
		}
	}
}
