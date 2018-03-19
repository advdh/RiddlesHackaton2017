using RiddlesHackaton2017.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

		public override IEnumerable<int> AffectedFields
		{
			get
			{
				return new[] { Index }.Union(Board.NeighbourFields[Index]);
			}
		}

		public override string ToString()
		{
			return $"KillMove {Position}";
		}

		public override string ToOutputString()
		{
			return $"kill {Position.X},{Position.Y}";
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

		public override Board Apply(Board board, bool validate)
		{
			var result = new Board(board);

			var errorMessage = ValidateMove(board);
			if (errorMessage == null)
			{
				//Valid move
				switch (board.Field[Index])
				{
					case 1: result.Player1FieldCount--; break;
					case 2: result.Player2FieldCount--; break;
				}
				result.Field[Index] = 0;
			}
			else if (validate)
			{
				throw new InvalidKillMoveException(errorMessage);
			}
			//else silently apply pass move because invalid move

			return result;
		}

		public override void ApplyInline(Board board, bool validate)
		{
			var errorMessage = ValidateMove(board);
			if (errorMessage == null)
			{
				//Valid move
				board.ApplyKill(board.MyPlayer, Index);
			}
			else if (validate)
			{
				throw new InvalidKillMoveException(errorMessage);
			}
			//else silently apply pass move because invalid move
		}

		private string ValidateMove(Board board)
		{
			if (board.Field[Index] == 0)
			{
				return $"Kill move position must not be empty: {Position}";
			}
			return null;
		}
	}
}
