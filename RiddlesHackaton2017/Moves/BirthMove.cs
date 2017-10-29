using System;
using RiddlesHackaton2017.Models;

namespace RiddlesHackaton2017.Moves
{
	public class BirthMove : Move
	{
		public int BirthIndex { get; private set; }
		public Position BirthPosition { get { return new Position(BirthIndex); } }
		public int SacrificeIndex1 { get; private set; }
		public Position SacrificePosition1 { get { return new Position(SacrificeIndex1); } }
		public int SacrificeIndex2 { get; private set; }
		public Position SacrificePosition2 { get { return new Position(SacrificeIndex2); } }

		public BirthMove(Position birthPosition, Position sacrificePosition1, Position sacrificePosition2)
			: this(birthPosition.Index, sacrificePosition1.Index, sacrificePosition2.Index)
		{

		}

		public BirthMove(int birthIndex, int sacrificeIndex1, int sacrificeIndex2)
		{
			BirthIndex = birthIndex;
			SacrificeIndex1 = sacrificeIndex1;
			SacrificeIndex2 = sacrificeIndex2;
		}

		public override string ToString()
		{
			return string.Format("Birthmove {0}, sacrifice = {1} and {2}", 
				BirthPosition, SacrificePosition1, SacrificePosition2);
		}

		public override Board Apply(Board board, Player player)
		{
			if (board.Field[BirthIndex] != 0)
			{
				throw new InvalidBirthMoveException("Birth position must be empty: {0}", BirthPosition);
			}
			if (board.Field[SacrificeIndex1] != (short)player)
			{
				throw new InvalidBirthMoveException("SacrificeIndex1 position must be owned by you: {0}", SacrificePosition1);
			}
			if (board.Field[SacrificeIndex2] != (short)player)
			{
				throw new InvalidBirthMoveException("SacrificeIndex2 position must be owned by you: {0}", SacrificePosition2);
			}
			if (SacrificeIndex1 == SacrificeIndex2)
			{
				throw new InvalidBirthMoveException("SacrificeIndex2 position must not be equals to Sacrifice1 position: {0}", SacrificePosition1);
			}

			var result = new Board(board);
			result.Field[BirthIndex] = (short)player;
			result.Field[SacrificeIndex1] = 0;
			result.Field[SacrificeIndex2] = 0;
			return result;
		}

		public override string ToOutputString()
		{
			return string.Format("birth {0},{1} {2},{3} {4},{5}",
				BirthPosition.X, BirthPosition.Y,
				SacrificePosition1.X, SacrificePosition1.Y,
				SacrificePosition2.X, SacrificePosition2.Y);
		}

		public static bool TryParse(string moveString, out Move move)
		{
			if (moveString.StartsWith("birth", StringComparison.InvariantCulture)
				&& moveString.Length > 6)
			{
				string[] ss = moveString.Substring(6).Split(' ');
				bool valid = ss.Length == 3;
				var positions = new Position[3];
				if (valid)
				{
					for (int i = 0; i < 3; i++)
					{
						string[] s = ss[i].Split(',');
						int x, y;
						if (!int.TryParse(s[0], out x) || !int.TryParse(s[1], out y))
						{
							valid = false;
							break;
						}
						positions[i] = new Position(x, y);
					}
				}
				if (valid)
				{
					move = new BirthMove(positions[0], positions[1], positions[2]);
					return true;
				}
			}
			move = new NullMove();
			return false;
		}
	}
}
