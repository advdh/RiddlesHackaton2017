using RiddlesHackaton2017.Bots;
using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Output;
using RiddlesHackaton2017.RandomGeneration;
using System;
using System.IO;
using System.Reflection;

namespace RiddlesHackaton2017
{
	static class Program
	{
		static Board Board = new Board();
		//static KillMoveBot Bot = new KillMoveBot(new ConsoleError());
		//static PassBot Bot = new PassBot(new ConsoleError());

		static MonteCarloParameters Parameters = MonteCarloParameters.Life;
		static MonteCarloBot Bot = new MonteCarloBot(new ConsoleError(), new RandomGenerator(new Random()))
		{
			Parameters = Parameters,
		};

		static void Main(string[] args)
		{
			Console.SetIn(new StreamReader(Console.OpenStandardInput(512)));
			Console.Error.WriteLine("Version {0}, UTC: {1}",
				Assembly.GetExecutingAssembly().GetName().Version,
				DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
			Console.Error.WriteLine("Parameters:");
			Console.Error.WriteLine(Parameters);

			while (true)
			{
				var command = Console.ReadLine();
				if (command == null) return;
				ExecuteCommand(command);
			}
		}

		private static void ExecuteCommand(string command)
		{
			string[] words = command.Split(' ');
			switch (words[0])
			{
				case "settings":
					ParseSettings(words);
					break;

				case "update":
					ParseBoard(words);
					break;

				case "action":
					int timelimit = int.Parse(words[2]);
					Console.WriteLine(Bot.GetMove(Board, TimeSpan.FromMilliseconds(timelimit)));
					break;
			}
		}

		private static void ParseBoard(string[] words)
		{
			switch (words[2])
			{
				case "field":
					Board.Field = BotParser.ParseBoard(words[3]);
					break;
				case "round":
					Board.Round = int.Parse(words[3]);
					break;
				case "living_cells":
					if (words[1] == "player0")
					{
						Board.Player1FieldCount = int.Parse(words[3]);
					}
					else
					{
						Board.Player2FieldCount = int.Parse(words[3]);
					}
					break;
			}
		}

		private static void ParseSettings(string[] words)
		{
			switch (words[1])
			{
				case "your_botid":
					int value = int.Parse(words[2]);
					Board.MyPlayer = (Player)Enum.Parse(typeof(Player), (value + 1).ToString());
					break;
			}
		}
	}
}
