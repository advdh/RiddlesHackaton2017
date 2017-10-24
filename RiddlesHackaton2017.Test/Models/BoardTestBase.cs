using RiddlesHackaton2017.Models;
using System;
using System.Linq;

namespace RiddlesHackaton2017.Test.Models
{
	public class BoardTestBase
	{
		protected static Board InitBoard()
		{
			return InitBoard(Player.Player1, StartBoardString);
		}

		protected static Board InitBoard(Position player1Position)
		{
			var boardStringChars = StartBoardString.Replace('1','.').Replace('2','.').ToCharArray();
			boardStringChars[2 + player1Position.X + 18 * player1Position.Y] = '1';
			boardStringChars[2 + 15 - player1Position.X + 18 * player1Position.Y] = '2';
			return InitBoard(Player.Player1, new string(boardStringChars));
		}

		private static string StartBoardString = @"
................
................
................
................
................
................
................
...1........2...
................
................
................
................
................
................
................
................
";

		protected static Board InitBoard(string boardString)
		{
			return InitBoard(16, Player.Player1, boardString);
		}

		protected static Board InitBoard(Player player, string boardString)
		{
			return InitBoard(16, player, boardString);
		}

		protected static Board InitBoard(int size, Player player, string boardString)
		{
			var board = new Board() { MyPlayer = player };
			if (size < Board.Size)
			{
				for (int i = 0; i < size; i++)
				{
					//board.SetField(i, size);
					//board.SetField(size, i);
				}
			}
			string[] lines = boardString.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Skip(1).ToArray();
			for (int y = 0; y < Math.Min(size, lines.Length); y++)
			{
				for (int x = 0; x < Math.Min(size, lines[y].Length); x++)
				{
					switch (lines[y][x])
					{
						case '1':
//							board.SetField(x, y);
							board.Player1Position = new Position(x, y).Index;
							break;
						case '2':
//							board.SetField(x, y);
							board.Player2Position = new Position(x, y).Index;
							break;
						case 'x':
//							board.SetField(x, y);
							break;
						default:
							break;
					}
				}
			}

			return board;
		}
	}
}
