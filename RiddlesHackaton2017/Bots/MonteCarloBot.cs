using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.MonteCarlo;
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

			var moveGeneratorStopwatch = Stopwatch.StartNew();
			var candidateMoves = GetCandidateMoves(Parameters.MoveCount).ToArray();
			if (Parameters.LogLevel >= 1)
			{
				ConsoleError.WriteLine($"MoveGeneration: {moveGeneratorStopwatch.ElapsedMilliseconds:0} ms");
			}

			if (Parameters.LogLevel >= 2)
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
				var startBoard = Board.ApplyMoveAndNext(Board.MyPlayer, move);
				var simulator = new Simulator(startBoard, Random, Parameters);
				var result = simulator.SimulateMove(move);

				if (Parameters.LogLevel >= 2)
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

			if (Parameters.LogLevel >= 3)
			{
				ConsoleError.WriteLine("MyKills:");
				int ix = 0;
				foreach (var kill in myKills)
				{
					ConsoleError.WriteLine($"  {ix} ({kill.Value}): {new Position(kill.Key)}");
					ix++;
				}
				ConsoleError.WriteLine("OpponentKills:");
				ix = 0;
				foreach (var kill in opponentKills)
				{
					ConsoleError.WriteLine($"  {ix} ({kill.Value}): {new Position(kill.Key)}");
					ix++;
				}
				ConsoleError.WriteLine("Births:");
				ix = 0;
				foreach (var birth in myBirths)
				{
					ConsoleError.WriteLine($"  {ix} ({birth.Value}): {new Position(birth.Key)}");
					ix++;
				}
			}

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

								//Calculate real score
								var birthMove = new BirthMove(birth.Key, kill1.Key, kill2.Key);

								var neighbours1 = Board.NeighbourFieldsAndThis[birth.Key].Union(Board.NeighbourFieldsAndThis[kill1.Key]).Union(Board.NeighbourFieldsAndThis[kill2.Key]);
								var neighbours2 = Board.NeighbourFields2[birth.Key].Union(Board.NeighbourFields2[kill1.Key]).Union(Board.NeighbourFields2[kill2.Key]);
								afterMoveBoard.Field[birth.Key] = (short)Board.MyPlayer;
								afterMoveBoard.Field[kill1.Key] = 0;
								afterMoveBoard.Field[kill2.Key] = 0;
								var score = CalculateMoveScore(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
								afterMoveBoard.Field[birth.Key] = 0;
								afterMoveBoard.Field[kill1.Key] = (short)Board.MyPlayer;
								afterMoveBoard.Field[kill2.Key] = (short)Board.MyPlayer;
								result.Add(new MoveScore(birthMove, score));
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
				result.Add(new MoveScore(new KillMove(killMove.Key), killMove.Value));
			}

			//Kill moves
			foreach (var killMove in opponentKills)
			{
				result.Add(new MoveScore(new KillMove(killMove.Key), killMove.Value));
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

		/// <summary>Gets a dictionary of kills on one of my cells with their scores</summary>
		public Dictionary<int, int> GetMyKills(Board board1, Board board2,
			Board afterMoveBoard, Board afterMoveBoard1, Board afterMoveBoard2)
		{
			var result = new Dictionary<int, int>();

			foreach (int i in Board.MyCells)
			{
				var neighbours1 = Board.NeighbourFieldsAndThis[i];
				var neighbours2 = Board.NeighbourFields2[i];
				afterMoveBoard.Field[i] = 0;
				var score = CalculateMoveScore(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
				result.Add(i, score);
				afterMoveBoard.Field[i] = Board.Field[i];
			}
			return result;
		}

		/// <summary>Gets a dictionary of kills on opponent's cells with their scores</summary>
		public Dictionary<int, int> GetOpponentKills(Board board1, Board board2,
			Board afterMoveBoard, Board afterMoveBoard1, Board afterMoveBoard2)
		{
			var result = new Dictionary<int, int>();

			foreach (int i in Board.OpponentCells)
			{
				var neighbours1 = Board.NeighbourFieldsAndThis[i];
				var neighbours2 = Board.NeighbourFields2[i];
				afterMoveBoard.Field[i] = 0;
				var score = CalculateMoveScore(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
				result.Add(i, score);
				afterMoveBoard.Field[i] = Board.Field[i];
			}
			return result;
		}

		/// <summary>Gets a dictionary of births (not birth moves) with their scores</summary>
		public Dictionary<int, int> GetBirths(Board board1, Board board2,
			Board afterMoveBoard, Board afterMoveBoard1, Board afterMoveBoard2)
		{
			var result = new Dictionary<int, int>();

			var candidates = Board.EmptyCells;

			foreach (int i in candidates)
			{
				var neighbours1 = Board.NeighbourFieldsAndThis[i];
				var neighbours2 = Board.NeighbourFields2[i];
				afterMoveBoard.Field[i] = (short)Board.MyPlayer;
				var score = CalculateMoveScore(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
				if (score > 0)
				{
					result.Add(i, score);
				}
				afterMoveBoard.Field[i] = 0;
			}
			return result;
		}

		/// <summary>
		/// Calculates score after second move
		/// </summary>
		/// <param name="board1">Next generation board after pass move</param>
		/// <param name="board2">Next-next generation board after two pass moves</param>
		/// <param name="afterMoveBoard">Board after specific move</param>
		/// <param name="afterMoveBoard1">Next generation board after specific move</param>
		/// <param name="afterMoveBoard2">Next-next generation board after specific move</param>
		/// <param name="neighbours1">Neighbours of i</param>
		/// <param name="neighbours2">Neighbours of neighbours of i</param>
		public int CalculateMoveScore(Board board1, Board board2,
			Board afterMoveBoard, Board afterMoveBoard1, Board afterMoveBoard2,
			IEnumerable<int> neighbours1AndThis, IEnumerable<int> neighbours2)
		{
			afterMoveBoard.GetNextGeneration(afterMoveBoard1, neighbours1AndThis);
			afterMoveBoard1.GetNextGeneration(afterMoveBoard2, neighbours2);

			//Calculate
			var moveScore = BoardEvaluator.Evaluate(afterMoveBoard2, neighbours2);
			int myMoveScore = moveScore.Item1;
			int opponentMoveScore = moveScore.Item2;
			var score = BoardEvaluator.Evaluate(board2, neighbours2);
			int myScore = score.Item1;
			int opponentScore = score.Item2;

			//Win bonus
			int winBonus = Parameters.WinBonus[afterMoveBoard1.OpponentPlayerFieldCount]
				- Parameters.WinBonus[afterMoveBoard1.MyPlayerFieldCount]
				+ Parameters.WinBonus[afterMoveBoard2.OpponentPlayerFieldCount]
				- Parameters.WinBonus[afterMoveBoard2.MyPlayerFieldCount];

			//Reset after move boards
			foreach (int j in neighbours1AndThis)
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
			return myMoveScore - opponentMoveScore - (myScore - opponentScore) + winBonus;
		}
	}
}
