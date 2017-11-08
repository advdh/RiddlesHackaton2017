using System.Collections.Generic;
using System.Linq;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Evaluation;

namespace RiddlesHackaton2017.Bots
{
	public class CheatBot : BaseBot
	{
		public CheatBot(IConsole consoleError) : base(consoleError)
		{
		}

		public override Move GetMove()
		{
			var board1 = Board.NextGeneration;
			var board2 = board1.NextGeneration;
			var killBoard = new Board(Board);
			var killBoard1 = new Board(board1);
			var killBoard2 = new Board(board2);

			var myKills = GetMyKills(board1, board2, killBoard, killBoard1, killBoard2).OrderByDescending(kvp => kvp.Value);
			var opponentKills = GetOpponentKills(board1, board2, killBoard, killBoard1, killBoard2).OrderByDescending(kvp => kvp.Value);
			var myBirths = GetBirths(board1, board2, killBoard, killBoard1, killBoard2).OrderByDescending(kvp => kvp.Value);

			Move move;
			if (myKills.Any())
			{
				move = new BirthMove(myBirths.First().Key, myKills.First().Key, myKills.First().Key);
			}
			else
			{
				move = new KillMove(opponentKills.First().Key);
			}
			LogMessage = move.ToString();
			return move;
		}

		/// <summary>Gets a dictionary of kills on one of my cells with their scores</summary>
		public Dictionary<int, BoardStatus> GetMyKills(Board board1, Board board2,
			Board killBoard, Board killBoard1, Board killBoard2)
		{
			var result = new Dictionary<int, BoardStatus>();

			foreach (int i in Board.MyCells)
			{
				var neighbours1 = Board.NeighbourFields[i];
				var neighbours2 = Board.NeighbourFields2[i];
				killBoard.Field[i] = 0;
				var r = CalculateBoardStatus(board1, board2, killBoard, killBoard1, killBoard2, neighbours1, neighbours2);
				result.Add(i, r);
				killBoard.Field[i] = Board.Field[i];
			}
			return result;
		}

		/// <summary>Gets a dictionary of kills on opponent's cells with their scores</summary>
		public Dictionary<int, BoardStatus> GetOpponentKills(Board board1, Board board2,
			Board killBoard, Board killBoard1, Board killBoard2)
		{
			var result = new Dictionary<int, BoardStatus>();

			foreach (int i in Board.OpponentCells)
			{
				var neighbours1 = Board.NeighbourFields[i];
				var neighbours2 = Board.NeighbourFields2[i];
				killBoard.Field[i] = 0;
				var r = CalculateBoardStatus(board1, board2, killBoard, killBoard1, killBoard2, neighbours1, neighbours2);
				result.Add(i, r);
				killBoard.Field[i] = Board.Field[i];
			}
			return result;
		}

		/// <summary>Gets a dictionary of births (not birth moves) with their scores</summary>
		public Dictionary<int, BoardStatus> GetBirths(Board board1, Board board2,
			Board killBoard, Board killBoard1, Board killBoard2)
		{
			var result = new Dictionary<int, BoardStatus>();

			foreach (int i in Board.EmptyCells)
			{
				var neighbours1 = Board.NeighbourFields[i];
				var neighbours2 = Board.NeighbourFields2[i];
				killBoard.Field[i] = (short)Board.MyPlayer;
				var r = CalculateBoardStatus(board1, board2, killBoard, killBoard1, killBoard2, neighbours1, neighbours2);
				result.Add(i, r);
				killBoard.Field[i] = 0;
			}
			return result;
		}

		/// <summary>
		/// Calculates board status
		/// </summary>
		/// <param name="board1">Next generation board after pass move</param>
		/// <param name="board2">Next-next generation board after two pass moves</param>
		/// <param name="killBoard">Board after specific move</param>
		/// <param name="killBoard1">Next generation board after specific move</param>
		/// <param name="killBoard2">Next-next generation board after specific move</param>
		/// <param name="neighbours1">Neighbours of i</param>
		/// <param name="neighbours2">Neighbours of neighbours of i</param>
		private BoardStatus CalculateBoardStatus(Board board1, Board board2,
			Board killBoard, Board killBoard1, Board killBoard2,
			int[] neighbours1, IEnumerable<int> neighbours2)
		{
			killBoard.GetNextGeneration(killBoard1, neighbours1);
			killBoard1.GetNextGeneration(killBoard2, neighbours2);

			//Calculate
			var killScore = BoardEvaluator.Evaluate(killBoard2, neighbours2);
			int myKillScore = killScore.Item1;
			int opponentKillScore = killScore.Item2;
			var score = BoardEvaluator.Evaluate(board2, neighbours2);
			int myScore = score.Item1;
			int opponentScore = score.Item2;

			//Calculate status
			GameStatus newGameStatus = GameStatus.Busy;
			bool opponent0 = board2.OpponentPlayerFieldCount + opponentKillScore - opponentScore == 0;
			bool me0 = board2.MyPlayerFieldCount + myKillScore - myScore == 0;
			if (opponent0)
			{
				newGameStatus = me0 ? GameStatus.Draw : GameStatus.Won;
			}
			else if (me0)
			{
				newGameStatus = GameStatus.Lost;
			}

			//Reset kill boards
			foreach (int j in neighbours1)
			{
				killBoard1.Field[j] = board1.Field[j];
			}
			foreach (int j in neighbours2)
			{
				killBoard2.Field[j] = board2.Field[j];
			}
			return new BoardStatus(newGameStatus, myKillScore - opponentKillScore - (myScore - opponentScore));
		}
	}
}
