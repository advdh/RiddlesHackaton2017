using System;

namespace RiddlesHackaton2017.Models
{
	public class BoardStatus : IComparable<BoardStatus>
	{
		public GameStatus Status { get; private set;}
		public int Score { get; private set; }
		public static BoardStatus MinValue
		{
			get
			{
				return new BoardStatus(GameStatus.Lost, int.MinValue);
			}
		}

		public BoardStatus(GameStatus status, int score)
		{
			Status = status;
			Score = score;
		}

		public static bool operator >(BoardStatus boardStatus1, BoardStatus boardStatus2)
		{
			return boardStatus1.CompareTo(boardStatus2) == 1;
		}

		public static bool operator <(BoardStatus boardStatus1, BoardStatus boardStatus2)
		{
			return boardStatus1.CompareTo(boardStatus2) == -1;
		}

		public static bool operator >=(BoardStatus boardStatus1, BoardStatus boardStatus2)
		{
			return boardStatus1.CompareTo(boardStatus2) >= 0;
		}

		public static bool operator <=(BoardStatus boardStatus1, BoardStatus boardStatus2)
		{
			return boardStatus1.CompareTo(boardStatus2) <= 0;
		}

		public override string ToString()
		{
			return string.Format("{0}: score = {1}", Status, Score);
		}

		public int CompareTo(BoardStatus other)
		{
			if (Status == other.Status) return Score.CompareTo(other.Score);
			if (Status == GameStatus.Won) return 1;
			if (Status == GameStatus.Lost) return -1;
			return 0;
		}
	}
}
