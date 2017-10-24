using RiddlesHackaton2017.Models;
using System;

namespace RiddlesHackaton2017
{
	public static class BotParser
	{
		public static Board ParseBoard(string[] words, Player player, int round)
		{
			string value = words[3];
			string[] values = value.Split(',');
			var board = new Board() { MyPlayer = player, Round = round };
			for (int i = 0; i < values.Length; i++)
			{
				switch (values[i])
				{
					case "0":
						board.Player1Position = i;
						throw new NotImplementedException();
						break;
					case "1":
						board.Player2Position = i;
						throw new NotImplementedException();
						break;
					case "x":
						throw new NotImplementedException();
						break;
				}
			}

			return board;
		}
	}
}
