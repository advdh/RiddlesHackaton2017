using RiddlesHackaton2017.Models;
using System;

namespace RiddlesHackaton2017
{
	public static class BotParser
	{
		public static Board ParseBoard(string[] words, Player player, int round)
		{
			string value = words[3];
			return ParseBoard(words[3], player, round);
		}

		public static Board ParseBoard(string boardString, Player player, int round)
		{
			string[] values = boardString.Split(',');
			var board = new Board() { MyPlayer = player, Round = round };
			for (int i = 0; i < values.Length; i++)
			{
				int x = i % 18;
				int y = i / 18;
				int ix = new Position(x, y).Index;
				switch (values[i])
				{
					case "0":
						board.Field[ix] = 1;
						break;
					case "1":
						board.Field[ix] = 2;
						break;
					case "x":
						break;
				}
			}

			return board;
		}
	}
}
