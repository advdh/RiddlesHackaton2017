using RiddlesHackaton2017.Models;
using System;

namespace RiddlesHackaton2017
{
	public static class BotParser
	{
		public static short[] ParseBoard(string boardString)
		{
			var result = new short[Board.Size];
			string[] values = boardString.Split(',');
			for (int i = 0; i < values.Length; i++)
			{
				int x = i % Board.Width;
				int y = i / Board.Width;
				int ix = new Position(x, y).Index;
				switch (values[i])
				{
					case "0":
						result[ix] = 1;
						break;
					case "1":
						result[ix] = 2;
						break;
					case "x":
						break;
				}
			}

			return result;
		}
	}
}
