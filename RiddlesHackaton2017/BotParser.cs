using RiddlesHackaton2017.Models;
using System;

namespace RiddlesHackaton2017
{
	public static class BotParser
	{
		public static short[] ParseBoard(string boardString)
		{
			return ParseBoard(Board.Width, Board.Height, boardString);
		}

		public static short[] ParseBoard(int width, int height, string boardString)
		{
			var result = new short[Board.Size];
			int i = 0;
			foreach(var c in boardString)
			{
				switch (c)
				{
					case '0':
						result[new Position(i % width, i / width).Index] = 1;
						i++;
						break;
					case '1':
						result[new Position(i % width, i / width).Index] = 2;
						i++;
						break;
					case '.':
						result[new Position(i % width, i / width).Index] = 0;
						i++;
						break;
				}
			}
			return result;
		}
	}
}
