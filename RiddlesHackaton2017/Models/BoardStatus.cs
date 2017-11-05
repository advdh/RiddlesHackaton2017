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

		public override bool Equals(object obj)
		{
			var other = obj as BoardStatus;
			return (object)other != null
				&& other.Score == Score
				&& other.Status == Status;
		}

		public override int GetHashCode()
		{
			return Score.GetHashCode() % Status.GetHashCode();
		}

		public static bool operator ==(BoardStatus boardStatus1, BoardStatus boardStatus2)
		{
			return boardStatus1.Score == boardStatus2.Score
				&& boardStatus1.Status == boardStatus2.Status;
		}

		public static bool operator !=(BoardStatus boardStatus1, BoardStatus boardStatus2)
		{
			return boardStatus1.Score != boardStatus2.Score
				|| boardStatus1.Status != boardStatus2.Status;
		}

		public override string ToString()
		{
			return $"{Status}: score = {Score}";
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
