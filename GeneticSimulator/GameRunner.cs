﻿using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.Output;
using System;
using System.Diagnostics;
using System.Threading;

namespace GeneticSimulator
{
	public class GameRunner
	{
		private TimeSpan StartTimeBank = TimeSpan.FromMilliseconds(10000);
		private const int RoundTimeBank = 100;
		private readonly Random Random;

		public GameRunner(Random random)
		{
			Random = random;
		}

		public GameResult Run(MonteCarloParameters bot1Parameters, MonteCarloParameters bot2Parameters)
		{
			//Initialize bots
			var bot1 = new Anila8Bot(new NullConsole()) { Parameters = bot1Parameters };
			TimeSpan timeLimit1 = StartTimeBank;
			bool bot1Timeout = false;

			//Wait a while before initializing bot2 in order to have indepedent random objects
			Thread.Sleep(100);

			var bot2 = new Anila8Bot(new NullConsole()) { Parameters = bot2Parameters };
			TimeSpan timeLimit2 = StartTimeBank;
			bool bot2Timeout = false;

			//Total number of fields of players of all rounds
			int score1 = 0;
			int score2 = 0;

			//Initialize board
			Board board = GetRandomBoard();

			//Play
			bool goOn = true;
			while (goOn)
			{
				//Player 1
				board.MyPlayer = Player.Player1;
				PlayerPlay(ref board, bot1, Player.Player1, ref timeLimit1, ref bot1Timeout);
				board.UpdateFieldCounts();
				score1 += board.Player1FieldCount;
				score2 += board.Player2FieldCount;
				goOn = board.Player1FieldCount > 0 && board.Player2FieldCount > 0;
				if (!goOn)
				{
					return GetResult(board, timeLimit1, timeLimit2, bot1Timeout, bot2Timeout, 
						bot1Parameters.GetHashCode(), bot2Parameters.GetHashCode(),
						score1, score2);
				}

				//Player 2
				board.MyPlayer = Player.Player2;
				PlayerPlay(ref board, bot2, Player.Player2, ref timeLimit2, ref bot2Timeout);
				board.UpdateFieldCounts();
				score1 += board.Player1FieldCount;
				score2 += board.Player2FieldCount;
				goOn = board.Player1FieldCount > 0 && board.Player2FieldCount > 0 && board.Round <= Board.MaxRounds;
			}

			return GetResult(board, timeLimit1, timeLimit2, bot1Timeout, bot2Timeout,
						bot1Parameters.GetHashCode(), bot2Parameters.GetHashCode(),
						score1, score2);
		}

		private void PlayerPlay(ref Board board, Anila8Bot bot, Player player, ref TimeSpan playerTimeBank, ref bool playerTimedOut)
		{
			Move move = new PassMove();
			if (!playerTimedOut)
			{
				var stopwatch = Stopwatch.StartNew();
				playerTimeBank += TimeSpan.FromMilliseconds(RoundTimeBank);
				if (playerTimeBank > StartTimeBank) playerTimeBank = StartTimeBank;
				move = Move.Parse(bot.GetMove(new Board(board), playerTimeBank));
				playerTimeBank -= stopwatch.Elapsed;
				if (playerTimeBank < TimeSpan.Zero)
				{
					playerTimedOut = true;
					move = new PassMove();
				}
			}
			board = board.ApplyMoveAndNext(player, move, true);
		}

		private GameResult GetResult(Board board, TimeSpan timeBank1, TimeSpan timeBank2, bool bot1TimedOut, bool bot2TimedOut,
			int hash1, int hash2, int score1, int score2)
		{
			var result = new GameResult()
			{
				Winner = null,
				Rounds = Math.Min(board.Round, Board.MaxRounds),
				TimeBank1 = timeBank1,
				TimeBank2 = timeBank2,
				Bot1TimedOut = bot1TimedOut,
				Bot2TimedOut = bot2TimedOut,
				Parameters1Hash = hash1,
				Parameters2Hash = hash2,
				Player1Score = 2.0 * score1 / (score1 + score2) - 1.0,
				Player2Score = 2.0 * score2 / (score1 + score2) - 1.0,
			};
			if (board.Player1FieldCount == 0 && board.Player2FieldCount > 0)
			{
				result.Winner = Player.Player2;
				result.Player2Score = 1.0;
				result.Player1Score = -1.0;
			}
			else if (board.Player2FieldCount == 0 && board.Player1FieldCount > 0)
			{
				result.Winner = Player.Player1;
				result.Player1Score = 1.0;
				result.Player2Score = -1.0;
			}
			if (board.Player2FieldCount == 0 && board.Player1FieldCount == 0)
			{
				result.Player1Score = 0.0;
				result.Player2Score = 0.0;
			}
			return result;
		}

		private Board GetRandomBoard()
		{
			var board = new Board() { Round = 1 };
			for (int i = 0; i < 40; i++)
			{
				Position position;
				do
				{
					position = new Position(Random.Next(Board.Width), Random.Next(Board.Height / 2));
				} while (board.Field[position.Index] != 0);
				board.Field[position.Index] = 1;
				board.Field[Board.Size - 1 - position.Index] = 2;
			}
			board.Player1FieldCount = board.CalculatedPlayer1FieldCount;
			board.Player2FieldCount = board.CalculatedPlayer2FieldCount;
			return board;
		}
	}
}
