using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiddlesHackaton2017.Models
{
	public class Board
	{
		public const int Size = 16;
		public const int Size2 = Size * Size;

		#region Properties

		public int Round { get; set; }

		/// <summary>Me</summary>
		public Player MyPlayer { get; set; }

		/// <summary>Opponent</summary>
		public Player OpponentPlayer { get { return MyPlayer.Opponent(); } }

		/// <summary>Position of player 1</summary>
		public int Player1Position { get; set; }
		/// <summary>Position of player 2</summary>
		public int Player2Position { get; set; }

		/// <summary>My position</summary>
		public int MyPosition
		{
			get
			{
				return MyPlayer == Player.Player1 ? Player1Position : Player2Position;
			}
			set
			{
				if (MyPlayer == Player.Player1)
					Player1Position = value;
				else
					Player2Position = value;
			}
		}

		public IEnumerable<int> GetFeasibleMovesForPlayer(Player player)
		{
			throw new NotImplementedException();
		}

		/// <summary>Opponent's position</summary>
		public int OpponentPosition
		{
			get
			{
				return MyPlayer == Player.Player1 ? Player2Position : Player1Position;
			}
			set
			{
				if (MyPlayer == Player.Player1)
					Player2Position = value;
				else
					Player1Position = value;
			}
		}

		#endregion

		#region Constructors and static Creaate methods

		public Board()
		{
			MyPlayer = Player.Player1;
		}

		/// <summary>Copy constructor</summary>
		public Board(Board board)
		{
			//for (int i = 0; i < FieldSize; i++)
			//{
			//	Fields[i] = board.Fields[i];
			//}
			MyPlayer = board.MyPlayer;
			Player1Position = board.Player1Position;
			Player2Position = board.Player2Position;
			Round = board.Round;
		}

		public static Board CopyAndPlay(Board board, Player player, int move)
		{
			var newBoard = new Board(board);

			throw new NotImplementedException();
			//newBoard.Fields[FieldBits[move]] |= (BitPositions[move]);

			if (player == Player.Player1)
			{
				newBoard.Player1Position = move;
				newBoard.Player2Position = board.Player2Position;
			}
			else
			{
				newBoard.Player2Position = move;
				newBoard.Player1Position = board.Player1Position;
				newBoard.Round = board.Round + 1;
			}

			return newBoard;
		}

		#endregion

		#region Helper methods

		public Board OpponentBoard
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string ToString()
		{
			return string.Format("Round {0}; player1: {1}, player2: {2}",
				Round, new Position(Player1Position), new Position(Player2Position));
		}

		public string BoardString()
		{
			var result = new StringBuilder();
			for (int y = 0; y < Size; y++)
			{
				for (int x = 0; x < Size; x++)
				{
					int ix = new Position(x, y).Index;
					if (Player1Position == ix)
					{
						result.Append("0");
					}
					else if (Player2Position == ix)
					{
						result.Append("1");
					}
					//else if (Field(ix))
					//{
					//	result.Append("x");
					//}
					else
					{
						result.Append(".");
					}
				}
				result.AppendLine();
			}
			return result.ToString();
		}

		#endregion
	}
}
