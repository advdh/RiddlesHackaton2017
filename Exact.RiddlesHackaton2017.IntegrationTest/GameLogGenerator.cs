using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiddlesHackaton2017.IntegrationTest
{
	public static class GameLogGenerator
	{
		/// <summary>
		/// Generates collection of command lines, which correspond to the gameData and player
		/// </summary>
		/// <remarks>Timebank is estimated</remarks>
		public static IEnumerable<string> GenerateCommandLines(string gameData, Player player)
		{
			var rawLines = gameData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			var boardString = rawLines.First();

			var result = new List<string>();
			result.AddRange(new[]
			{
				"settings player_names player0,player1",
				$"settings your_bot player{(int)player - 1}",
				"settings timebank 10000",
				"settings time_per_move 100",
				$"settings your_botid {(int)player - 1}",
				"settings field_width 18",
				"settings field_height 16",
				"settings max_rounds 100",
			});

			var board = new Board() { Field = BotParser.ParseBoard(boardString) };
			int timeBank = 10000;
			int ix = 1;
			int round;
			while (ix < rawLines.Length)
			{
				round = (ix + 1) / 2;
				result.Add($"update game round {round}");
				result.Add($"update game field {board.BoardString()}");
				result.Add($"update player0 living_cells {board.GetCalculatedPlayerFieldCount(Player.Player1)}");
				var move = Move.Parse(rawLines[ix]);

				result.AddRange(MoveString(move, Player.Player1, player, timeBank));

				board = board.ApplyMoveAndNext(move, validateMove:false);
				result.Add($"update player1 living_cells {board.GetCalculatedPlayerFieldCount(Player.Player2)}");
				if (ix + 1 < rawLines.Length)
				{
					move = Move.Parse(rawLines[ix + 1]);
					result.AddRange(MoveString(move, Player.Player2, player, timeBank));
					board = board.ApplyMoveAndNext(move, validateMove: false);
				}

				int usedTime = Math.Min(800, Math.Max(100, timeBank / 10));
				timeBank -= usedTime + 100;
				ix += 2;
			}

			return result;
		}

		private static string[] MoveString(Move move, Player playerToPlay, Player myPlayer, int timeBank)
		{
			if (playerToPlay == myPlayer)
			{
				return PlayerMoveString(move, timeBank);
			}
			else
			{
				return OpponentMoveString(move);
			}
		}

		private static string[] PlayerMoveString(Move move, int timeBank)
		{
			return new[]
			{
				$"action move {timeBank}",
				string.Format("Output from your bot: \"{0}\"", move.ToOutputString()),
			};
		}

		private static string[] OpponentMoveString(Move move)
		{
			var opponentMoveString = move.ToOutputString().Replace(" ", "_");
			return new[] { $"update player0 move {opponentMoveString}" };
		}
	}
}
