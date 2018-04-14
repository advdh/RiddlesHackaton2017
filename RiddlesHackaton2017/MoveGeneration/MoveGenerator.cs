using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Evaluation;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiddlesHackaton2017.MoveGeneration
{
	public class MoveGenerator
	{
		public MonteCarloParameters Parameters { get; private set; }
		public Board Board { get; private set; }

		public MoveGenerator(Board board, MonteCarloParameters parameters)
		{
			Board = Guard.NotNull(board, nameof(board));
			Parameters = Guard.NotNull(parameters, nameof(parameters));
		}

		/// <summary>
		/// Generates birth moves and kill moves in such a way that two birth moves 
		/// have never more than 1 move part (birth/kill) in common
		/// </summary>
		/// <returns>Sorted collection of moves</returns>
		public IEnumerable<MoveScore> GetCandidateMoves(int maxCount)
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

			if (Parameters.LogLevel >= 4)
			{
				Console.WriteLine("MyKills:");
				int ix = 0;
				foreach (var kill in myKills)
				{
					Console.WriteLine($"  {ix} ({kill.Value}): {new Position(kill.Key)}");
					ix++;
				}
				Console.WriteLine("OpponentKills:");
				ix = 0;
				foreach (var kill in opponentKills)
				{
					Console.WriteLine($"  {ix} ({kill.Value}): {new Position(kill.Key)}");
					ix++;
				}
				Console.WriteLine("Births:");
				ix = 0;
				foreach (var birth in myBirths)
				{
					Console.WriteLine($"  {ix} ({birth.Value}): {new Position(birth.Key)}");
					ix++;
				}
			}

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

			//Own kill moves
			foreach (var killMove in myKills)
			{
				result.Add(new MoveScore(new KillMove(killMove.Key), killMove.Value));
			}

			//Opponent kill moves
			foreach (var killMove in opponentKills)
			{
				result.Add(new MoveScore(new KillMove(killMove.Key), killMove.Value));
			}
			return result.OrderByDescending(r => r.Gain2).Take(maxCount);
		}


		/// <summary>Gets a dictionary of kills on one of my cells with their scores after two generations</summary>
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


		/// <summary>Gets a dictionary of kills on opponent's cells with their scores after two generations</summary>
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

		/// <summary>Gets a dictionary of births (not birth moves) with their scores after two generations</summary>
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
				result.Add(i, score);
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
			int moveScore = BoardEvaluator.Evaluate(afterMoveBoard2, neighbours2, Parameters.CellCountWeight);
			int score = BoardEvaluator.Evaluate(board2, neighbours2, Parameters.CellCountWeight);

			//Win bonus (NB: afterMoveBoard1 has players switched!)
			int winBonus = -Parameters.WinBonus[afterMoveBoard1.OpponentPlayerFieldCount]
				+ Parameters.WinBonus[afterMoveBoard1.MyPlayerFieldCount]
				+ Parameters.WinBonus2[afterMoveBoard2.OpponentPlayerFieldCount]
				- Parameters.WinBonus2[afterMoveBoard2.MyPlayerFieldCount];

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
			return moveScore - score + Parameters.WinBonusWeight * winBonus;
		}
	}
}
