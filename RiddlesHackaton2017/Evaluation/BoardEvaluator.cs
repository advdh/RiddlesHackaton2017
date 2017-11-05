﻿using RiddlesHackaton2017.Models;
using System.Collections.Generic;
using System;

namespace RiddlesHackaton2017.Evaluation
{
	public static class BoardEvaluator
	{
		/// <summary>
		/// Bonus for score if we are (pretty) sure that we're going to win
		/// </summary>
		public const int WinBonus = 999999;

		/// <summary>
		/// Threshold, above which we assume that we win
		/// </summary>
		public const int WinThreshold = 499999;

		public static BoardStatus Evaluate(Board board)
		{
			var status = board.MyPlayerFieldCount > 0 ?
				board.OpponentPlayerFieldCount > 0 ? GameStatus.Busy : GameStatus.Won :
				board.MyPlayerFieldCount > 0 ? GameStatus.Lost : GameStatus.Draw;
			return new BoardStatus(status, board.MyPlayerFieldCount - board.OpponentPlayerFieldCount);
		}

		/// <returns>Tuple (my score, opponent score)</returns>
		public static Tuple<int, int> Evaluate(Board board, IEnumerable<int> positions)
		{
			int score1 = 0;
			int score2 = 0;
			foreach (int i in positions)
			{
				switch (board.Field[i])
				{
					case 1: score1++; break;
					case 2: score2++; break;
				}
			}
			return new Tuple<int, int>(board.MyPlayer == Player.Player1 ? score1 : score2,
				board.MyPlayer == Player.Player2 ? score1 : score2);
		}
	}
}
