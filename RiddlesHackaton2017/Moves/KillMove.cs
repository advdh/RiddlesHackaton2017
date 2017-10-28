using System;
using RiddlesHackaton2017.Models;

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
