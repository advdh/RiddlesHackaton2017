using System;
using RiddlesHackaton2017.Models;
using System.Collections.Generic;
using System.Linq;

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

		public override IEnumerable<int> AffectedFields
		{
			get
			{
				return new[] { BirthIndex }
					.Union(Board.NeighbourFields[BirthIndex])
					.Union(new[] { SacrificeIndex1 })
					.Union(Board.NeighbourFields[SacrificeIndex1])
					.Union(new[] { SacrificeIndex2 })
					.Union(Board.NeighbourFields[SacrificeIndex2]);
			}
		}

		public override string ToString()
		{
			return $"Birthmove {BirthPosition}, sacrifice = {SacrificePosition1} and {SacrificePosition2}";
		}

		public override Board Apply(Board board, Player player, bool validate = true)
		{
			var result = new Board(board);

			string errorMessage = ValidateMove(board, player);
			if (errorMessage == null)
			{
				result.Field[BirthIndex] = (short)player;
				result.Field[SacrificeIndex1] = 0;
				result.Field[SacrificeIndex2] = 0;
				result.MyPlayerFieldCount--;
			}
			else if (validate)
			{
				throw new InvalidBirthMoveException(errorMessage);
			}
			//else silently apply pass move because invalid move
			return result;
		}

		public override void ApplyInline(Board board, Player player, bool validate = true)
		{
			string errorMessage = ValidateMove(board, player);
			if (errorMessage == null)
			{
				board.Field[BirthIndex] = (short)player;
				board.Field[SacrificeIndex1] = 0;
				board.Field[SacrificeIndex2] = 0;
				board.MyPlayerFieldCount--;
				board.ResetNextGeneration();
			}
			else if (validate)
			{
				throw new InvalidBirthMoveException(errorMessage);
			}
			//else silently apply pass move because invalid move
		}

		/// <summary>
		/// Returns errorstring if move is invalid, null if valid
		/// </summary>
		private string ValidateMove(Board board, Player player)
		{
			if (board.Field[BirthIndex] != 0)
			{
				return $"Birth position must be empty: {BirthPosition}";
			}
			if (board.Field[SacrificeIndex1] != (short)player)
			{
				return $"SacrificeIndex1 position must be owned by you: {SacrificePosition1}";
			}
			if (board.Field[SacrificeIndex2] != (short)player)
			{
				return $"SacrificeIndex2 position must be owned by you: {SacrificePosition2}";
			}
			if (SacrificeIndex1 == SacrificeIndex2)
			{
				return $"SacrificeIndex2 position must not be equals to Sacrifice1 position: {SacrificePosition1}";
			}
			return null;
		}

		public override string ToOutputString()
		{
			return $"birth {BirthPosition.X},{BirthPosition.Y} {SacrificePosition1.X},{SacrificePosition1.Y} {SacrificePosition2.X},{SacrificePosition2.Y}";
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
