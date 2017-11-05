using System;

namespace RiddlesHackaton2017.Models
{
	public class Position
	{
		private int _x;
		public int X
		{
			get
			{
				return _x;
			}
			set
			{
				if (value < 0 || value >= Board.Width) throw new ArgumentOutOfRangeException("X");
				_x = value;
			}
		}

		private int _y;
		public int Y
		{
			get
			{
				return _y;
			}
			set
			{
				if (value < 0 || value >= Board.Height) throw new ArgumentOutOfRangeException("Y");
				_y = value;
			}
		}

		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}

		public Position(int ix)
		{
			X = ix / Board.Height;
			Y = ix % Board.Height;
		}

		public static Position Parse(string s)
		{
			string raw = s.Replace("(", "").Replace(")", "").Trim();
			string[] pos = raw.Split(',');
			return new Position(int.Parse(pos[0]), int.Parse(pos[1]));
		}

		public int Index {
			get
			{
				return Y + Board.Height * X;
			}
		}

		public override string ToString()
		{
			return $"({X},{Y})";
		}

		public override bool Equals(object obj)
		{
			var position = obj as Position;
			if (position == null) return false;
			return this.X == position.X && this.Y == position.Y;
		}

		public override int GetHashCode()
		{
			return Index;
		}
	}
}
