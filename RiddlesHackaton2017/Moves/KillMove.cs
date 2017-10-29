using RiddlesHackaton2017.Models;
using System;

namespace RiddlesHackaton2017.Moves
{
	public class KillMove : Move
	{
		public int Index { get; private set; }
		public Position Position { get { return new Position(Index); } }

		public KillMove(Position position) : this(position.Index) { }
		public KillMove(int x, int y) : this(new Position(x, y).Index) { }

		public KillMove(int index)
		{
			Index = index;
		}

		public override string ToString()
		{
			return string.Format("KillMove {0}", Position);
		}

		public override string ToOutputString()
		{
			return string.Format("kill {0},{1}", Position.X, Position.Y);
		}

		public static bool TryParse(string moveString, out Move move)
		{
			if (moveString.StartsWith("kill", StringComparison.InvariantCulture)
				&& moveString.Length > 5)
			{
				string[] s = moveString.Substring(5).Split(',');
				if (s.Length == 2)
				{
					int x, y;
					if (int.TryParse(s[0], out x) 
						&& int.TryParse(s[1], out y))
					{
						move = new KillMove(x, y);
						return true;
					}
				}
			}
			move = new NullMove();
			return false;
		}

		public override Board Apply(Board board, Player player)
		{
			if (board.Field[Index] == 0)
			{
				throw new InvalidKillMoveException("Kill move position must not be empty: {0}", Position);
			}

			var result = new Board(board);
			result.Field[Index] = 0;
			return result;
		}
	}
}
