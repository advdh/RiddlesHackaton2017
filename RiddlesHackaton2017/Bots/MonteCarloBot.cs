using RiddlesHackaton2017.Evaluation;
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

			var candidateMoves = GetCandidateMoves(Parameters.MoveCount).ToArray();

			if (Parameters.LogAllMoves)
			{
				int ix = 0;
				foreach (var moveScore in candidateMoves)
				{
					ConsoleError.WriteLine($"  {ix}: {moveScore}");
					ix++;
				}
			}

			int count = 0;
			int bestGain2 = 0;
			while (goOn)
			{
				//Get random move and simulate rest of the game several times
				var moveScore = candidateMoves[count];
				var move = moveScore.Move;
				var result = SimulateMove(Board, move);

				if (Parameters.LogAllMoves)
				{
					ConsoleError.WriteLine($"     Move {count}: move gain2: {moveScore.Gain2} - {move} - score = {result.Score:P0}, win in {result.AverageWinRounds}, loose in {result.AverageLooseRounds}");
				}

				if (result.Score > bestResult.Score)
				{
					//Prefer higher score
					bestResult = result;
					bestMove = move;
					bestGain2 = moveScore.Gain2;
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
							bestGain2 = moveScore.Gain2;
						}
					}
					else
					{
						//Prefer to loose in more rounds
						if (result.AverageLooseRounds > bestResult.AverageLooseRounds)
						{
							bestResult = result;
							bestMove = move;
							bestGain2 = moveScore.Gain2;
						}
					}
				}

				count++;

				goOn = stopwatch.Elapsed < duration && count < candidateMoves.Length;
			}

			//Log and return
			LogMessage = $"{bestMove} (gain2 = {bestGain2}): score = {bestResult.Score:P0}, moves = {count}, win in {bestResult.AverageWinRounds}, loose in {bestResult.AverageLooseRounds}";

			return bestMove;
		}

		/// <summary>
		/// Generates birth moves and kill moves in such a way that two birth moves 
		/// have never more than 1 move part (birth/kill) in common
		/// </summary>
		/// <returns>Sorted collection of moves</returns>
		private IEnumerable<MoveScore> GetCandidateMoves(int maxCount)
		{
			var result = new List<MoveScore>();

			var board1 = Board.NextGeneration;
			var board2 = board1.NextGeneration;
			var afterMoveBoard = new Board(Board);
			var afterMoveBoard1 = new Board(board1);
			var afterMoveBoard2 = new Board(board2);

			var myKills = GetMyKills(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2).OrderByDescending(kvp => kvp.Value);
			var opponentKills = GetOpponentKills(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2).OrderByDescending(kvp => kvp.Value);
			var myBirths = GetBirths(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2).OrderByDescending(kvp => kvp.Value);

			result.Add(new MoveScore(new PassMove(), 0));
			for (int i = 1; i < Math.Min(myBirths.Count(), myKills.Count()); i++)
			{
				for (int b = 0; b < i && b < myBirths.Count(); b++)
				{
					var birth = myBirths.ElementAt(b);
					for (int k1 = 0; k1 < i && k1 < myKills.Count(); k1++)
					{
						var kill1 = myKills.ElementAt(k1);
						for (int k2 = k1 + 1; k2 < i + 1 && k2 < myKills.Count(); k2++)
						{
							if (b == i - 1 || k1 == i - 1 || k2 == i)
							{
								//Else already done
								var kill2 = myKills.ElementAt(k2);

								//int score = birth.Value.Score + kill1.Value.Score + kill2.Value.Score;
								//Calculate real score
								var birthMove = new BirthMove(birth.Key, kill1.Key, kill2.Key);

								var neighbours1 = Board.NeighbourFields[birth.Key].Union(Board.NeighbourFields[kill1.Key]).Union(Board.NeighbourFields[kill2.Key]);
								var neighbours2 = Board.NeighbourFields2[birth.Key].Union(Board.NeighbourFields2[kill1.Key]).Union(Board.NeighbourFields2[kill2.Key]);
								afterMoveBoard.Field[birth.Key] = (short)Board.MyPlayer;
								afterMoveBoard.Field[kill1.Key] = 0;
								afterMoveBoard.Field[kill2.Key] = 0;
								var r = CalculateBoardStatus(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
								afterMoveBoard.Field[birth.Key] = 0;
								afterMoveBoard.Field[kill1.Key] = (short)Board.MyPlayer;
								afterMoveBoard.Field[kill2.Key] = (short)Board.MyPlayer;
								//var realScore = GetRealScore(birthMove, BoardEvaluator.Evaluate(board2).Score);
								result.Add(new MoveScore(birthMove, r.Score));
								if (result.Count >= maxCount) break;
							}
						}
						if (result.Count >= maxCount) break;
					}
					if (result.Count >= maxCount) break;
				}
				if (result.Count >= maxCount) break;
			}

			//Kill moves
			foreach (var killMove in myKills)
			{
				result.Add(new MoveScore(new KillMove(killMove.Key), killMove.Value.Score));
			}

			//Kill moves
			foreach (var killMove in opponentKills)
			{
				result.Add(new MoveScore(new KillMove(killMove.Key), killMove.Value.Score));
			}

			return result.OrderByDescending(r => r.Gain2).Take(maxCount);
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
		public Dictionary<int, BoardStatus> GetMyKills(Board board1, Board board2,
			Board afterMoveBoard, Board afterMoveBoard1, Board afterMoveBoard2)
		{
			var result = new Dictionary<int, BoardStatus>();

			foreach (int i in Board.MyCells)
			{
				var neighbours1 = Board.NeighbourFields[i];
				var neighbours2 = Board.NeighbourFields2[i];
				afterMoveBoard.Field[i] = 0;
				var r = CalculateBoardStatus(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
				result.Add(i, r);
				afterMoveBoard.Field[i] = Board.Field[i];
			}
			return result;
		}

		public int GetRealScore(BirthMove move, int baseScore)
		{
			return BoardEvaluator.Evaluate(move.Apply(Board, Board.MyPlayer).NextGeneration.NextGeneration).Score - baseScore;
		}

		/// <summary>Gets a dictionary of kills on opponent's cells with their scores</summary>
		public Dictionary<int, BoardStatus> GetOpponentKills(Board board1, Board board2,
			Board afterMoveBoard, Board afterMoveBoard1, Board afterMoveBoard2)
		{
			var result = new Dictionary<int, BoardStatus>();

			foreach (int i in Board.OpponentCells)
			{
				var neighbours1 = Board.NeighbourFields[i];
				var neighbours2 = Board.NeighbourFields2[i];
				afterMoveBoard.Field[i] = 0;
				var r = CalculateBoardStatus(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
				result.Add(i, r);
				afterMoveBoard.Field[i] = Board.Field[i];
			}
			return result;
		}

		/// <summary>Gets a dictionary of births (not birth moves) with their scores</summary>
		public Dictionary<int, BoardStatus> GetBirths(Board board1, Board board2,
			Board afterMoveBoard, Board afterMoveBoard1, Board afterMoveBoard2)
		{
			var result = new Dictionary<int, BoardStatus>();

			foreach (int i in Board.EmptyCells)
			{
				var neighbours1 = Board.NeighbourFields[i];
				var neighbours2 = Board.NeighbourFields2[i];
				afterMoveBoard.Field[i] = (short)Board.MyPlayer;
				var r = CalculateBoardStatus(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
				result.Add(i, r);
				afterMoveBoard.Field[i] = 0;
			}
			return result;
		}

		/// <summary>
		/// Calculates board status
		/// </summary>
		/// <param name="board1">Next generation board after pass move</param>
		/// <param name="board2">Next-next generation board after two pass moves</param>
		/// <param name="afterMoveBoard">Board after specific move</param>
		/// <param name="afterMoveBoard1">Next generation board after specific move</param>
		/// <param name="afterMoveBoard2">Next-next generation board after specific move</param>
		/// <param name="neighbours1">Neighbours of i</param>
		/// <param name="neighbours2">Neighbours of neighbours of i</param>
		private BoardStatus CalculateBoardStatus(Board board1, Board board2,
			Board afterMoveBoard, Board afterMoveBoard1, Board afterMoveBoard2,
			IEnumerable<int> neighbours1, IEnumerable<int> neighbours2)
		{
			afterMoveBoard.GetNextGeneration(afterMoveBoard1, neighbours1);
			afterMoveBoard1.GetNextGeneration(afterMoveBoard2, neighbours2);

			//Calculate
			var moveScore = BoardEvaluator.Evaluate(afterMoveBoard2, neighbours2);
			int myMoveScore = moveScore.Item1;
			int opponentMoveScore = moveScore.Item2;
			var score = BoardEvaluator.Evaluate(board2, neighbours2);
			int myScore = score.Item1;
			int opponentScore = score.Item2;

			//Calculate status
			GameStatus newGameStatus = GameStatus.Busy;
			bool opponent0 = board2.OpponentPlayerFieldCount + opponentMoveScore - opponentScore == 0;
			bool me0 = board2.MyPlayerFieldCount + myMoveScore - myScore == 0;
			if (opponent0)
			{
				newGameStatus = me0 ? GameStatus.Draw : GameStatus.Won;
			}
			else if (me0)
			{
				newGameStatus = GameStatus.Lost;
			}

			//Reset after move boards
			foreach (int j in neighbours1)
			{
				afterMoveBoard1.Field[j] = board1.Field[j];
			}
			afterMoveBoard1.MyPlayerFieldCount = board1.MyPlayerFieldCount;
			afterMoveBoard1.OpponentPlayerFieldCount = board1.OpponentPlayerFieldCount;
			foreach (int j in neighbours2)
			{
				afterMoveBoard2.Field[j] = board2.Field[j];
			}
			afterMoveBoard2.MyPlayerFieldCount = board2.MyPlayerFieldCount;
			afterMoveBoard2.OpponentPlayerFieldCount = board2.OpponentPlayerFieldCount;
			return new BoardStatus(newGameStatus, myMoveScore - opponentMoveScore - (myScore - opponentScore));
		}
	}
}
