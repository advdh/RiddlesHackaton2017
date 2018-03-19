using GeneticSimulator.Models;
using RiddlesHackaton2017;
using RiddlesHackaton2017.IntegrationTest;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneticSimulator.Simulators
{
	public class EndGameSimulator : BaseSimulator
	{
		public const string BasePath = @"d:\temp\GeneticSimulator";

		private string ConfigurationsFilename { get; set; }

		/// <summary>Number of fields that the loosing player has exactly when starting the simulation</summary>
		private int LoosingFieldCount { get; set; }

		public EndGameSimulator(string commandLine) : base(commandLine)
		{
			string[] words = commandLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			if (words.Length > 1)
			{
				LoosingFieldCount = int.Parse(words[1]);
			}
			if (words.Length > 3)
			{
				ConfigurationsFilename = Path.Combine(BasePath, words[3]);
			}
		}

		public override Configurations Generate(ConfigurationGenerator generator, int populationSize)
		{
			return Configurations.Load(ConfigurationsFilename);
		}

		/// <summary>
		/// Simulates <paramref name="simlationCount"/> simulations of each of the configurations against each other,
		/// where each configuration is both winning player and loosing player
		/// </summary>
		public override void Simulate(Configurations configurations, int simulationCount)
		{
			var endgameResults = new EndGameResults();
			endgameResults.Configurations.AddRange(configurations);
			endgameResults.Save(Filename);

			int populationSize = configurations.Count;

			//Get relevant games from the database
			var games = GetGames(10000);
			var loosingPlayer = Player.Player2;
			var relevantGames = games.Where(g => GetStartBoard(g, loosingPlayer) != null).ToList();
			if (relevantGames.Count < simulationCount) simulationCount = relevantGames.Count;

			//Play
			var gameRunner = new GameRunner(new Random());
			for (int n = 0; n < simulationCount; n++)
			{
				//Get board and start player from the game
				var game = relevantGames[n];
				var startBoard = GetStartBoard(game, loosingPlayer);

				string wonString = "Won by player1";
				Console.WriteLine($"Start simulation {n + 1} / {simulationCount}: game {game.Id}, {game.Player0} ({game.Version0}) - {game.Player1} ({game.Version1}), {game.PlayedDate:dd-MM-yyyy}, starting at round {startBoard.Round}, {wonString} in {game.Rounds}");

				for (int i = 0; i < populationSize; i++)
				{
					for (int j = 0; j < populationSize; j++)
					{
						//For the moment we will always start with a start board where player1 moves first
						var result = gameRunner.Run(configurations[i].Parameters, 
							configurations[j].Parameters,
							startBoard: new Board(startBoard), 
							startPlayer: Player.Player1, 
							startTimeBank: TimeSpan.FromSeconds(1));
						var endBoard = result.Board;
						var endgameResult = new EndGameResult()
						{
							Won = endBoard.Player2FieldCount == 0 && endBoard.Player1FieldCount > 0,
							EndRound = endBoard.Round,
							GameId = game.Id,
							StartRound = startBoard.Round,
							WinningId = i,
							LoosingId = j,
						};
						endgameResults.Results.Add(endgameResult);
						endgameResults.Save(Filename);
						Console.WriteLine(endgameResult);
					}
				}
			}
		}

		/// <summary>
		/// Returns the first board from the game where the loosing player has exactly the LoosingPlayerFields fields left
		/// </summary>
		/// <param name="maxLooseCount">Maximum number of fields which the looser owns</param>
		/// <returns>Start board</returns>
		private Board GetStartBoard(Game game, Player loosingPlayer)
		{
			var board = new Board();
			var rawLines = game.GameData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			var boardString = rawLines.First();
			board.SetField(BotParser.ParseBoard(boardString));
			board.UpdateFieldCounts();
			int ix = 1;
			var player = Player.Player1;
			while (ix < rawLines.Length)
			{
				var line = rawLines[ix];
				if (line != "kill -1,-1")
				{
					var move = Move.Parse(rawLines[ix]);
					board = board.ApplyMoveAndNext(move, validateMove: false);
					if (player == Player.Player2)
					{
						//For the moment we will always start with a start board where player1 moves first
						if (loosingPlayer == Player.Player1 && board.Player1FieldCount == LoosingFieldCount) return board;
						if (loosingPlayer == Player.Player2 && board.Player2FieldCount == LoosingFieldCount) return board;
					}
					player = player.Opponent();
				}
				ix++;
			}
			return null;
		}

		/// <summary>
		/// Returns top count games that player0 (red) won, ordered by played date desc
		/// </summary>
		private IEnumerable<Game> GetGames(int count)
		{
			string where = string.Format(@"Player0 IN (SELECT TOP {0} BotName FROM LeaderBoard WHERE Rank <= 10)
				AND Player1 IN (SELECT TOP {0} BotName FROM LeaderBoard WHERE Rank <= 10)
				AND Winner = 0", count);
			string orderBy = "PlayedDate DESC";

			using (var database = new Database())
			{
				database.Connect();
				return database.GetGames(count, where, orderBy).ToList();
			}
		}
	}
}
