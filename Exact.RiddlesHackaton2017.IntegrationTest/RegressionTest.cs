using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.MoveGeneration;
using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.Output;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiddlesHackaton2017.IntegrationTest
{
	[TestClass]
	public class RegressionTest : IntegrationTestBase
	{
		[TestMethod]
		public void CompareMoves()
		{
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4, Player.Player1);
			var birthPosition = new Position(5, 9);
			var killPosition1 = new Position(10, 6);
			var killPosition2 = new Position(2, 6);
			var oIomove = new BirthMove(birthPosition, killPosition1, killPosition2);

			////Gain1
			//var birthGain1 = board.GetDeltaFieldCountForBirth(birthPosition.Index, Player.Player1, Player.Player1, validate: true);
			//var kill1Gain1 = board.GetDeltaFieldCountForKill(killPosition1.Index, Player.Player1, Player.Player1, validate: true);
			//var kill2Gain1 = board.GetDeltaFieldCountForKill(killPosition2.Index, Player.Player1, Player.Player1, validate: true);
			//Console.WriteLine($"BirthGain1: {birthGain1}, Kill1Gain: {kill1Gain1}, Kill2Gain: {kill2Gain1}");

			var bot = new Anila8Bot(new NullConsole());
			var parameters = MonteCarloParameters.Life;
			parameters.UseMoveGenerator2 = true;
			bot.Parameters = parameters;
			bot.Board = board;
			bot.TimeLimit = TimeSpan.FromMilliseconds(7892);
			var candidateMoves = bot.GetCandidateMoves(100).ToArray();
			var moveGenerator = new MoveGenerator2(board, parameters);
			var candidateMoves2 = moveGenerator.GetMoves().ToArray();
			var moves = new List<Move>();
			moves.Add(new PassMove());
			moves.Add(oIomove);
			moves.AddRange(candidateMoves2.Select(m => m.Move));

			Console.WriteLine($"Generation 0: pass = {board.Player1FieldCount} - {board.Player2FieldCount}");

			foreach (var move in moves)
			{
				int generation = 0;
				var board1 = new Board(board).ApplyMoveAndNext(move, validateMove: true);
				var score = board1.Player1FieldCount - board1.Player2FieldCount;

				generation++;
				Console.WriteLine($"Generation {generation}: {board1.Player1FieldCount} - {board1.Player2FieldCount}");
				while (generation < 8)
				{
					board1 = board1.NextGeneration;
					score += board1.Player1FieldCount - board1.Player2FieldCount;
					Console.WriteLine($"	Generation {generation}: {board1.Player1FieldCount} - {board1.Player2FieldCount}");
					generation++;
				}

				//Console.WriteLine($"{move}: Score = {score}");
			}
		}

		[TestMethod]
		public void CompareBirths()
		{
			Board.InitializeFieldCountChanges();
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4, Player.Player1);
			var birthPosition = new Position(5, 9);
			var killPosition1 = new Position(10, 6);
			var killPosition2 = new Position(2, 6);
			var oIomove = new BirthMove(birthPosition, killPosition1, killPosition2);

			Console.WriteLine($"Generation 0: pass = {board.Player1FieldCount} - {board.Player2FieldCount}");

			var births = new Dictionary<int, int>();
			foreach (var birth in board.EmptyCells)
			{
				int generation = 0;
				var board1 = new Board(board);
				board1.Field[birth] = 1;
				board1.MyPlayerFieldCount++;
				board1 = board1.NextGeneration;
				var score = board1.Player1FieldCount - board1.Player2FieldCount;

				generation++;
				while (generation < 8)
				{
					board1 = board1.NextGeneration;
					score += board1.Player1FieldCount - board1.Player2FieldCount;
					generation++;
				}
				births.Add(birth, score);
			}
			foreach (var birth in births.OrderByDescending(b => b.Value))
			{
				Console.WriteLine($"{new Position(birth.Key)}: Score = {birth.Value}");
			}
		}

		[TestMethod]
		public void CompareBirths2()
		{
			Board.InitializeFieldCountChanges();
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4, Player.Player1);
			int generationCount = 16;

			//Header
			Console.Write("Move");
			for (int generation = 1; generation < generationCount; generation++)
			{
				Console.Write($"\tg{generation}");
			}
			Console.WriteLine();

			foreach (var birth in board.EmptyCells)
			{
				int generation = 0;
				var board1 = new Board(board);
				board1.Field[birth] = 1;
				board1.MyPlayerFieldCount++;
				board1 = board1.NextGeneration;
				var score = board1.Player1FieldCount - board1.Player2FieldCount;

				generation++;
				Console.Write(new Position(birth));
				while (generation < generationCount)
				{
					board1 = board1.NextGeneration;
					score += board1.Player1FieldCount - board1.Player2FieldCount;
					generation++;
					Console.Write('\t');
					Console.Write(score);
				}
				Console.WriteLine();
			}
		}

		[TestMethod]
		public void CompareKills()
		{
			Board.InitializeFieldCountChanges();
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4, Player.Player1);
			var birthPosition = new Position(5, 9);
			var killPosition1 = new Position(10, 6);
			var killPosition2 = new Position(2, 6);
			var oIomove = new BirthMove(birthPosition, killPosition1, killPosition2);

			var kills = new Dictionary<int, int>();
			foreach (var kill in board.MyCells)
			{
				int generation = 0;
				var board1 = new Board(board);
				board1.Field[kill] = 0;
				board1.MyPlayerFieldCount--;
				board1 = board1.NextGeneration;
				var score = board1.Player1FieldCount - board1.Player2FieldCount;

				generation++;
				while (generation < 8)
				{
					board1 = board1.NextGeneration;
					score += board1.Player1FieldCount - board1.Player2FieldCount;
					generation++;
				}
				kills.Add(kill, score);
			}
			foreach (var kill in kills.OrderByDescending(b => b.Value))
			{
				Console.WriteLine($"{new Position(kill.Key)}: Score = {kill.Value}");
			}
		}

		[TestMethod]
		public void CompareKills2()
		{
			Board.InitializeFieldCountChanges();
			var board = GetBoardFromDatabase("84f2e021-78df-4c1b-80bc-ae62ca233ac3", 4, Player.Player1);
			int generationCount = 16;

			//Header
			Console.Write("Move");
			for (int generation = 1; generation < generationCount; generation++)
			{
				Console.Write($"\tg{generation}");
			}
			Console.WriteLine();

			foreach (var kill in board.MyCells)
			{
				int generation = 0;
				var board1 = new Board(board);
				board1.Field[kill] = 0;
				board1.MyPlayerFieldCount--;
				board1 = board1.NextGeneration;
				var score = board1.Player1FieldCount - board1.Player2FieldCount;

				generation++;
				Console.Write(new Position(kill));
				while (generation < generationCount)
				{
					board1 = board1.NextGeneration;
					score += board1.Player1FieldCount - board1.Player2FieldCount;
					generation++;
					Console.Write('\t');
					Console.Write(score);
				}
				Console.WriteLine();
			}
		}
	}
}
