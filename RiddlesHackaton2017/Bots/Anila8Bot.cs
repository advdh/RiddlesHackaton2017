using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.MonteCarlo;
using RiddlesHackaton2017.MoveGeneration;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RiddlesHackaton2017.Bots
{
	public class Anila8Bot : BaseBot
	{
		public MonteCarloParameters Parameters { get; set; } = MonteCarloParameters.Life;

		private readonly IRandomGenerator Random;

		private RoundStatistics RoundStatistics = new RoundStatistics();

		public Anila8Bot(IConsole consoleError) : this(consoleError, new TroschuetzRandomGenerator(new Random()))
		{

		}

		private Anila8Bot(IConsole consoleError, IRandomGenerator random) : base(consoleError)
		{
			Random = Guard.NotNull(random, nameof(random));
		}

		/// <summary>
		/// Returns the maximum allowed duration for simulations
		/// </summary>
		private TimeSpan GetMaxDuration(TimeSpan timeLimit)
		{
			if (Parameters.Debug) return Parameters.MaxDuration;
			return new TimeSpan((int)(Math.Min((long)(Parameters.MaxRelativeDuration * timeLimit.Ticks), 
				Parameters.MaxDuration.Ticks) / Parameters.Throttle));
		}

		private Simulator _Simulator;
		private Simulator Simulator
		{
			get
			{
				if (_Simulator == null)
				{
					_Simulator = new Simulator(randomGenerator: Random, monteCarloParameters: Parameters);
				}
				return _Simulator;
			}
		}

		private bool UseMoveGenerator2
		{
			get
			{
				return ((Parameters.UseMoveGenerator2ForRed && Board.MyPlayer == Player.Player1) ||
					(Parameters.UseMoveGenerator2ForBlue && Board.MyPlayer == Player.Player2))
					&& TimeLimit > Parameters.MoveGenerator2MinimumTimebank;
			}
		}

		public override Move GetMove()
		{
			var bestMove = GetDirectWinMove();
			if (bestMove != null)
			{
				LogMessage = "Direct win move";
				return bestMove;
			}

			var stopwatch = Stopwatch.StartNew();
			MoveScore[] candidateMoves;
			if (UseMoveGenerator2)
			{
				var moveGenerator2 = new MoveGenerator2(Board, Parameters);
				candidateMoves = moveGenerator2.GetMoves().ToArray();
			}
			else
			{
				var moveGenerator = new MoveGenerator(Board, Parameters);
				candidateMoves = moveGenerator.GetCandidateMoves(Parameters.MoveCount).ToArray();
			}
			if (Parameters.LogLevel >= 1)
			{
				Console.WriteLine($"GetCandidateMoves = {stopwatch.ElapsedMilliseconds}");
			}
			return GetSimulationMove(candidateMoves, stopwatch);
		}

		/// <summary>
		/// Get best simulation move from candidateMoves within limited time
		/// </summary>
		public Move GetSimulationMove(MoveScore[] candidateMoves, Stopwatch stopwatch)
		{
			if (stopwatch == null) stopwatch = Stopwatch.StartNew();
			int GetMoveCandidatesMs = (int)stopwatch.ElapsedMilliseconds;
			TimeSpan maxDuration = GetMaxDuration(TimeLimit);
			int simulationCount = RoundStatistics.GetSimulationCount(maxDuration, Parameters.MinSimulationCount, Parameters.MaxSimulationCount, Parameters.StartSimulationCount
				, Parameters.SimulationFactor);

			List<MonteCarloStatistics> results = SimulateMoves(candidateMoves, stopwatch, maxDuration, simulationCount);

			RoundStatistics.Add(new RoundStatistic() { MaxDuration = stopwatch.Elapsed, MoveCount = results.Count, SimulationCount = simulationCount, Round = Board.Round });

			var orderedResults = results.OrderByDescending(r => r.Score2);
			LogSimulatedMoves(orderedResults);

			var best = orderedResults.First();

			//Log and return
			LogMessage = $"{best.Move} ({Board.MyPlayerFieldCount}-{Board.OpponentPlayerFieldCount}, gain2 = {best.Gain2}): score = {best.Score:P0}, score2 = {best.Score2}, moves = {results.Count} ({best.Index}), simulations = {simulationCount}, win in {best.AverageWinGenerations:0.00}, loose in {best.AverageLooseGenerations:0.00}, GetMoveCandidates = {GetMoveCandidatesMs} ms";

			return best.Move;
		}

		/// <summary>
		/// Logs simulated moves, if loglevel is 2 or more
		/// </summary>
		private void LogSimulatedMoves(IOrderedEnumerable<MonteCarloStatistics> orderedResults)
		{
			if (Parameters.LogLevel >= 2)
			{
				int rank = 1;
				foreach (var result in orderedResults)
				{
					ConsoleError.WriteLine($"     Move {rank} ({result.Index}): move gain2: {result.Gain2} - {result.Move} - score = {result.Score:P0}, score2 = {result.Score2}, win in {result.AverageWinGenerations:0.00}, loose in {result.AverageLooseGenerations:0.00}");
					rank++;
				}
			}
		}

		public List<MonteCarloStatistics> SimulateMoves(MoveScore[] candidateMoves, Stopwatch stopwatch, TimeSpan maxDuration, int simulationCount)
		{
			if (stopwatch == null) stopwatch = Stopwatch.StartNew();

			var results = new List<MonteCarloStatistics>();
			int i = 0;
			bool goOn = true;
			while (goOn)
			{
				//Get random move and simulate rest of the game several times
				var stopWatchSimulation = Stopwatch.StartNew();
				var moveScore = candidateMoves[i];
				var move = moveScore.Move;

				var result = TryMove(move, moveScore.Gain2, maxDuration, simulationCount);

				result.Index = i;
				results.Add(result);

				i++;

				goOn = stopwatch.Elapsed < maxDuration && i < candidateMoves.Length;
			}

			return results;
		}

		public MonteCarloStatistics TryMove(Move move, int gain2, TimeSpan maxDuration, int simulationCount)
		{
			var startBoard = Board.ApplyMoveAndNext(move, Parameters.ValidateMoves);
			return Simulator.SimulateMove(startBoard, maxDuration, move, gain2, simulationCount);
		}

		/// <remarks>TODO: remove this: only used by test code</remarks>
		public int GetMoveScore(Move move)
		{
			var moveGenerator = new MoveGenerator(Board, Parameters);
			var board1 = Board.NextGeneration;
			var board2 = board1.NextGeneration;
			var afterMoveBoard = new Board(Board);
			var afterMoveBoard1 = new Board(board1);
			var afterMoveBoard2 = new Board(board2);
			var birthMove = move as BirthMove;
			if (birthMove != null)
			{
				afterMoveBoard.Field[birthMove.BirthIndex] = (short)Board.MyPlayer;
				afterMoveBoard.Field[birthMove.SacrificeIndex1] = 0;
				afterMoveBoard.Field[birthMove.SacrificeIndex2] = 0;
				var neighbours1 = Board.NeighbourFieldsAndThis[birthMove.BirthIndex].Union(Board.NeighbourFieldsAndThis[birthMove.SacrificeIndex1]).Union(Board.NeighbourFieldsAndThis[birthMove.SacrificeIndex2]);
				var neighbours2 = Board.NeighbourFields2[birthMove.BirthIndex].Union(Board.NeighbourFields2[birthMove.SacrificeIndex1]).Union(Board.NeighbourFields2[birthMove.SacrificeIndex2]);
				return moveGenerator.CalculateMoveScore(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
			}

			var killMove = move as KillMove;
			if (killMove != null)
			{
				afterMoveBoard.Field[killMove.Index] = 0;
				var neighbours1 = Board.NeighbourFieldsAndThis[killMove.Index];
				var neighbours2 = Board.NeighbourFields2[killMove.Index];
				return moveGenerator.CalculateMoveScore(board1, board2, afterMoveBoard, afterMoveBoard1, afterMoveBoard2, neighbours1, neighbours2);
			}

			//PassMove
			return 0;
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

	}
}
