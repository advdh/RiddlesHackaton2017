using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using System;
using System.Linq;

namespace RiddlesHackaton2017.IntegrationTest
{
	public class IntegrationTestBase
	{
		protected Board GetBoardFromDatabase(string gameId, int round, Player player)
		{
			using (var database = new Database())
			{
				database.Connect();
				var game = database.GetGameById(gameId);
				var rawLines = game.GameData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				var boardString = rawLines.First();
				var board = new Board() { Round = 1 };
				board.SetField(BotParser.ParseBoard(boardString));
				board.UpdateFieldCounts();
				int ix = 1;
				while (ix < rawLines.Length)
				{
					if (board.Round == round && board.MyPlayer == player) return board;
					var line = rawLines[ix];
					if (line != "kill -1,-1")
					{
						var move = Move.Parse(rawLines[ix]);
						board = board.ApplyMoveAndNext(move, validateMove: false);
					}
					ix++;
				}
			}
			throw new ArgumentException($"game {gameId}, round {round}, {player}");
		}

	}
}
