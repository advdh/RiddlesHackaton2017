using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;

namespace RiddlesHackaton2017.MoveGeneration
{
	public class MoveGenerator2
	{
		private readonly Board Board;
		private readonly MonteCarloParameters Parameters;

		public MoveGenerator2(Board board, MonteCarloParameters parameters)
		{
			Board = Guard.NotNull(board, nameof(board));
			Parameters = Guard.NotNull(parameters, nameof(parameters));
		}

		public IEnumerable<MoveScore> GetMoves()
		{
			var births = GetBirths(generationCount: Parameters.MoveGeneratorGenerationCount, top: Parameters.MoveGeneratorTopBirths).ToList();
			var kills = GetKills(generationCount: Parameters.MoveGeneratorGenerationCount, top: Parameters.MoveGeneratorTopKills).ToList();
			var opponentKills = GetOpponentKills(generationCount: Parameters.MoveGeneratorGenerationCount, top: Parameters.MoveGeneratorTopKills).ToList();
			var birthMoves = CombineBirthsAndMoves(births, kills);
			var killMoves = MakeKillMoves(opponentKills);
			return SimulateMoves(birthMoves.Union(killMoves), generationCount: Parameters.MoveGeneratorGenerationCount, keepPercentage: Parameters.MoveGeneratorKeepFraction);
		}

		/// <summary>
		/// For each move, apply the move and a number of pass moves
		/// </summary>
		/// <param name="moves"></param>
		/// <param name="generationCount">Number of generations to apply to each board</param>
		/// <param name="keepPercentage">Percentage of moves to keep after each generation</param>
		/// <returns>Best moves after a number of generations</returns>
		private IEnumerable<MoveScore> SimulateMoves(IEnumerable<Move> moves, int generationCount, double keepPercentage)
		{
			int sign = Board.MyPlayer == Player.Player1 ? 1 : -1;

			List<MoveBoard> moveBoards = InitializeMoveBoards(moves, sign);
			for (int i = 1; i <= generationCount; i++)
			{
				ApplyNext(moveBoards, sign);
				int keep = Math.Max(1, (int)(keepPercentage * moveBoards.Count + 0.5));
				moveBoards = moveBoards
					.OrderByDescending(mb => mb.Score)
					.Take(keep).ToList();
			}

			return moveBoards.Select(mb => new MoveScore(mb.Move, mb.Score));
		}

		private List<MoveBoard> InitializeMoveBoards(IEnumerable<Move> moves, int sign)
		{
			var moveBoards = new List<MoveBoard>();
			foreach (var move in moves)
			{
				var nextBoard = new Board(Board.ApplyMoveAndNext(move, Parameters.ValidateMoves));

				//For first board include winbonus
				int score = nextBoard.Player1FieldCount - nextBoard.Player2FieldCount
					+ Parameters.WinBonus[nextBoard.Player2FieldCount] - Parameters.WinBonus[nextBoard.Player1FieldCount];

				var moveBoard = new MoveBoard()
				{
					Move = move,
					Board = nextBoard,
					Score = sign * score,
				};
				moveBoards.Add(moveBoard);
			}

			return moveBoards;
		}

		private void ApplyNext(List<MoveBoard> moveBoards, int sign)
		{
			Parallel.ForEach(moveBoards, moveBoard =>
			{
				var board = moveBoard.Board;
				board = board.NextGeneration;
				moveBoard.Score += sign * (board.Player1FieldCount - board.Player2FieldCount);
			});
		}

		private class MoveBoard
		{
			public Move Move { get; set; }
			public Board Board { get; set; }
			public int Score { get; set; }

			public override string ToString()
			{
				return $"Score: {Score} - {Move} - {Board}";
			}
		}

		/// <summary>
		/// Returns top births, based on cumulative difference between player field counts after generationCount generations
		/// </summary>
		public IEnumerable<int> GetBirths(int generationCount, int top)
		{
			var scores = new ConcurrentDictionary<int, int>();
			int sign = Board.MyPlayer == Player.Player1 ? 1 : -1;
			Parallel.ForEach(Board.EmptyCells, birth =>
			{
				int generation = 0;
				var board1 = new Board(Board);
				board1.Field[birth] = 1;
				board1.MyPlayerFieldCount++;
				board1 = board1.NextGeneration;
				var score = board1.Player1FieldCount - board1.Player2FieldCount;

				generation++;
				while (generation <= generationCount)
				{
					board1 = board1.NextGeneration;
					score += board1.Player1FieldCount - board1.Player2FieldCount;
					generation++;
				}
				scores.TryAdd(birth, sign * score);
			});

			return scores
				.OrderByDescending(r => r.Value)
				.Take(top)
				.Select(r => r.Key);
		}

		private IEnumerable<Move> MakeKillMoves(IEnumerable<int> opponentKills)
		{
			return opponentKills.Select(k => new KillMove(k));
		}

		/// <summary>
		/// Returns top my kills, based on cumulative difference between player field counts after generationCount generations
		/// </summary>
		public IEnumerable<int> GetKills(int generationCount, int top)
		{
			var scores = new ConcurrentDictionary<int, int>();
			int sign = Board.MyPlayer == Player.Player1 ? 1 : -1;
			Parallel.ForEach(Board.MyCells, kill =>
			{
				int generation = 0;
				var board1 = new Board(Board);
				board1.Field[kill] = 0;
				board1.MyPlayerFieldCount--;
				board1 = board1.NextGeneration;
				var score = board1.Player1FieldCount
					- board1.Player2FieldCount;

				generation++;
				while (generation <= generationCount)
				{
					board1 = board1.NextGeneration;
					score += board1.Player1FieldCount - board1.Player2FieldCount;
					generation++;
				}
				scores.TryAdd(kill, sign * score);
			});

			return scores
				.OrderByDescending(r => r.Value)
				.Take(top)
				.Select(r => r.Key);
		}

		/// <summary>
		/// Returns top opponent kills, based on cumulative difference between player field counts after generationCount generations
		/// </summary>
		public IEnumerable<int> GetOpponentKills(int generationCount, int top)
		{
			var scores = new ConcurrentDictionary<int, int>();
			int sign = Board.MyPlayer == Player.Player1 ? 1 : -1;
			Parallel.ForEach(Board.OpponentCells, kill =>
			{
				int generation = 0;
				var board1 = new Board(Board);
				board1.Field[kill] = 0;
				board1.OpponentPlayerFieldCount--;
				board1 = board1.NextGeneration;
				var score = board1.Player1FieldCount - board1.Player2FieldCount;

				generation++;
				while (generation <= generationCount)
				{
					board1 = board1.NextGeneration;
					score += board1.Player1FieldCount - board1.Player2FieldCount;
					generation++;
				}
				scores.TryAdd(kill, sign * score);
			});

			return scores
				.OrderByDescending(r => r.Value)
				.Take(top)
				.Select(r => r.Key);
		}

		/// <summary>
		/// Combines the births and kills to birth moves
		/// </summary>
		public IEnumerable<Move> CombineBirthsAndMoves(List<int> births, List<int> kills)
		{
			foreach (var birth in births)
			{
				for (int i = 0; i < births.Count; i++)
				{
					for (int j = i + 1; j < kills.Count; j++)
					{
						yield return new BirthMove(birth, kills[i], kills[j]);
					}
				}
			}
		}
	}
}
