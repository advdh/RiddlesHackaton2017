﻿using RiddlesHackaton2017.MonteCarlo;
using RiddlesHackaton2017.Moves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RiddlesHackaton2017.Models
{
	public class Board
	{
		public const int Width = 18;
		public const int Height = 16;
		public const int Size = Width * Height;
		public const int MaxRounds = 100;

		public int Round { get; set; }

		#region Fields

		public int[] PlayerFieldCount = new int[3];

		public int Player1FieldCount {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return PlayerFieldCount[1]; }
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set { PlayerFieldCount[1] = value; }
		}
		public int Player2FieldCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return PlayerFieldCount[2]; }
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set { PlayerFieldCount[2] = value; }
		}

		public int MyPlayerFieldCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return PlayerFieldCount[MyPlayer.Value()];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				PlayerFieldCount[MyPlayer.Value()] = value;
			}
		}

		public int OpponentPlayerFieldCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return PlayerFieldCount[OpponentPlayer.Value()];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				PlayerFieldCount[OpponentPlayer.Value()] = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UpdateFieldCounts()
		{
			foreach(var player in AllPlayers)
			{
				PlayerFieldCount[player.Value()] = GetCalculatedPlayerFieldCount(player);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetCalculatedPlayerFieldCount(Player player)
		{
			return AllCells.Count(i => Field[i] == player.Value());
		}

		#endregion

		#region Cells

		public IEnumerable<int> GetCells(Player player)
		{
			int playerId = player.Value();
			for (int i = 0; i < Size; i++)
			{
				if (Field[i] == playerId) yield return i;
			}
		}

		public static IEnumerable<int> AllCells { get { return Enumerable.Range(0, Size); } }

		public void ValidateFieldCounts(bool validate)
		{
			if (validate &&
				(PlayerFieldCount[1] != GetCalculatedPlayerFieldCount(Player.Player1)
					|| PlayerFieldCount[2] != GetCalculatedPlayerFieldCount(Player.Player2)))
					{
						Console.WriteLine($"Incorrect field count: 1: {Player1FieldCount} / {GetCalculatedPlayerFieldCount(Player.Player1)}; 2: {Player2FieldCount} / {GetCalculatedPlayerFieldCount(Player.Player2)}");
					}
		}

		public IEnumerable<int> MyCells { get { return AllCells.Where(i => Field[i] == MyPlayer.Value()); } }
		public IEnumerable<int> OpponentCells { get { return AllCells.Where(i => Field[i] == OpponentPlayer.Value()); } }
		public IEnumerable<int> EmptyCells { get { return AllCells.Where(i => Field[i] == 0); } }

		#endregion

		#region Players

		public IEnumerable<Player> AllPlayers
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return new[] { Player.Player1, Player.Player2 }; }
		}

		public short[] Field = new short[Size];

		/// <summary>Me</summary>
		public Player MyPlayer { get; set; }

		/// <summary>Opponent</summary>
		public Player OpponentPlayer { get { return MyPlayer.Opponent(); } }

		#endregion

		#region Constructors and next generation

		static Board()
		{
			InitializeFieldCountChanges();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Board()
		{
			MyPlayer = Player.Player1;
		}

		/// <summary>Copy constructor</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Board(Board board)
		{
			for (int i = 0; i < Size; i++)
			{
				Field[i] = board.Field[i];
			}
			MyPlayer = board.MyPlayer;
			Round = board.Round;
			PlayerFieldCount[1] = board.PlayerFieldCount[1];
			PlayerFieldCount[2] = board.PlayerFieldCount[2];
			_mykills = board._mykills;
			_opponentKills = board._opponentKills;
			_myBirths = board._myBirths;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResetNextGeneration()
		{
			_mykills = null;
			_opponentKills = null;
			_myBirths = null;
			_NextGeneration = null;
			Neighbours = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetField(short[] v)
		{
			Field = v;
			ResetNextGeneration();
		}

		/// <summary>
		/// Applies move for player and applies next generation
		/// </summary>
		/// <returns>New board</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Board ApplyMoveAndNext(Move move, bool validateMove)
		{
			//Apply move and next generation
			var newBoard = move.Apply(this, validateMove).NextGeneration;

			return newBoard;
		}

		/// <summary>
		/// Moves to the next generation for only the specified fields
		/// and return the next generation board in board
		/// </summary>
		/// <returns>New board</returns>
		/// <remarks>TODO: get rid of this method, and refactor calling methods</remarks>
		public void GetNextGeneration(Board board, IEnumerable<int> affectedFields)
		{
			foreach (int i in affectedFields)
			{
				board.PlayerFieldCount[board.Field[i]]--;
				board.Field[i] = NextGenerationForField(i);
				board.PlayerFieldCount[board.Field[i]]++;
			}
		}

		private short NextGenerationForField(int i)
		{
			int count = 0;
			int count1 = 0;
			foreach (int j in NeighbourFields[i])
			{
				if (Field[j] != 0)
				{
					count++;
					if (Field[j] == 1)
					{
						count1++;
					}
				}
			}
			if (Field[i] != 0)
			{
				//Current cell is living
				switch (count)
				{
					case 2:
					case 3:
						//Live on
						return Field[i];
					default:
						//Die
						break;
				}
			}
			else if (count == 3)
			{
				//Get born
				return (count1 >= 2 ? Player.Player1 : Player.Player2).Value();
			}
			return 0;
		}

		public Board OpponentBoard
		{
			get
			{
				var copy = new Board() { Round = Round, MyPlayer = OpponentPlayer };
				for (int i = 0; i < Size; i++)
				{
					if (Field[i] != 0)
					{
						copy.Field[i] = (short)(3 - Field[i]);
					}
				}
				copy.PlayerFieldCount[2] = PlayerFieldCount[1];
				copy.PlayerFieldCount[1] = PlayerFieldCount[2];
				return copy;
			}
		}

		#endregion

		#region Apply moves

		Dictionary<int, int> _mykills;
		Dictionary<int, int> _opponentKills;
		Dictionary<int, int> _myBirths;

		public Dictionary<int, int> MyKills { get { return _mykills; } }
		public Dictionary<int, int> OpponentKills { get { return _opponentKills; } }

		/// <summary>
		/// Apply kill to this board, and to the NextGeneration, if it exists
		/// </summary>
		/// <param name="player">Player of which the cell is killed</param>
		/// <param name="index">Index which is killed</param>
		internal void ApplyKill(int index)
		{
			int playerValue = Field[index];
			PlayerFieldCount[playerValue]--;
			Field[index] = 0;
			if (Neighbours != null)
			{
				foreach (int i in Board.NeighbourFields[index])
				{
					Neighbours[playerValue, i]--;
					SetNextGenerationField(this, NextGeneration, i);
				}
			}
		}

		/// Apply birth to this board, and to the NextGeneration, if it exists
		/// </summary>
		/// <param name="player">Player for which the cell is born</param>
		/// <param name="index">Index which is born</param>
		internal void ApplyBirth(Player player, int index)
		{
			int playerId = player.Value();
			PlayerFieldCount[playerId]++;
			Field[index] = (short)playerId;
			if (Neighbours != null)
			{
				foreach (int i in Board.NeighbourFields[index])
				{
					Neighbours[playerId, i]++;
					SetNextGenerationField(this, NextGeneration, i);
				}
			}
		}

		public Dictionary<int, int> MyBirths { get { return _myBirths; } }

		/// <remarks>MyKills gets kills for the opponent because we use it in the context of the opponent player</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InitializeSmartMoves()
		{
			_mykills = SmartMoveSimulator.GetKills(this, MyPlayer);
			_opponentKills = SmartMoveSimulator.GetKills(this, OpponentPlayer);
			_myBirths = SmartMoveSimulator.GetBirths(this, MyPlayer);
		}

		#endregion

		#region NextGeneration

		/// <summary>
		/// Returns the next generation of the current board
		/// </summary>
		public Board NextGeneration
		{
			get
			{
				if (_NextGeneration == null)
				{
					CalculateNeighbours();

					_NextGeneration = new Board()
					{
						MyPlayer = MyPlayer.Opponent(),
						Round = Round + (MyPlayer == Player.Player2 ? 1 : 0),
					};
					for (int i = 0; i < Size; i++)
					{
						SetNextGenerationField(this, _NextGeneration, i);
					}
				}
				return _NextGeneration;
			}
		}

		/// <summary>
		/// Sets next generation field for the new board for position i
		/// </summary>
		/// <param name="board">Current board</param>
		/// <param name="nextBoard">Next board</param>
		/// <param name="i">Position</param>
		private static void SetNextGenerationField(Board board, Board nextBoard, int i)
		{
			var values = FieldCountChange[board.Field[i], board.Neighbours[1, i], board.Neighbours[2, i]];
			nextBoard.Field[i] = (short)values[5];
			nextBoard.PlayerFieldCount[1] += values[6];
			nextBoard.PlayerFieldCount[2] += values[7];
		}

		/// <summary>
		/// Returns the change in field count of player -/- the field count of opponent player
		/// in the next generation after a kill on field i (which is owned by fieldOwner) 
		/// in the current generation, by the player
		/// </summary>
		/// <param name="i">Cell index</param>
		/// <param name="player">Current player</param>
		/// <param name="fieldOwner">Current owner of the field</param>
		/// <param name="validate">If true, then validate fieldOwner</param>
		public int GetDeltaFieldCountForKill(int i, Player player, Player fieldOwner, bool validate = false)
		{
			if (validate && Field[i] != fieldOwner.Value())
			{
				throw new ArgumentException($"Field {i} is not owned by expected field owner {fieldOwner}, but by {Field[i]}");
			}

			int result = NextGeneration.Field[i] == 0 ? 0 : -1;

			foreach (int j in NeighbourFields[i])
			{
				result += FieldCountChange[Field[j], Neighbours[1, j], Neighbours[2, j]][2 + fieldOwner.Value()];
			}

			return (player == Player.Player1 ? 1 : -1) * result;
		}

		/// <summary>
		/// Returns the change in field count of player -/- the field count of opponent player
		/// in the next generation after a birth on field i (which will be owned by fieldOwner) 
		/// in the current generation, by the player
		/// </summary>
		/// <param name="i">Cell index</param>
		/// <param name="player">Current player</param>
		/// <param name="fieldOwner">Owner of the field after the birth</param>
		/// <param name="validate">If true, then validate that field is currently empty</param>
		public int GetDeltaFieldCountForBirth(int i, Player player, Player fieldOwner, bool validate = false)
		{
			if (validate && Field[i] != 0)
			{
				throw new ArgumentException($"Field {i} is not empty but owned by {Field[i]}");
			}

			int result = 0;
			switch(NextGeneration.Field[i])
			{	case 1: result = fieldOwner == Player.Player1 ? 0 : -2; break;
				case 2: result = fieldOwner == Player.Player1 ? 2 : 0; break;
				default: result = fieldOwner == Player.Player1 ? 1 : -1; break;
			}
			foreach (int j in NeighbourFields[i])
			{
				result += FieldCountChange[Field[j], Neighbours[1, j], Neighbours[2, j]][fieldOwner.Value()];
			}
			return (player == Player.Player1 ? 1 : -1) * result;
		}

		/// <summary>
		/// FieldCountChange[a, b, c] = array of 5 elements:
		/// if the current field value is a (0, 1, or 2),
		/// and the number of Neighbours of player 1 is b
		/// and the number of Neighbours of player 2 is c
		/// Value is the field count change for player 1 (field count change for player  is -Value)
		/// index 0: the relative additional field count for player 1
		/// index 1: Field count changes for player1 births
		/// index 2: Field count changes for player2 births
		/// index 3: Field count changes for player1 kills (field owner is player1)
		/// index 4: Field count changes for player2 kills (field owner is player1)
		/// index 5: Next field value
		/// index 6: Absolute additional field count for player 1
		/// index 7: Absolute additional field count for player 2
		/// </summary>
		private static int[,,][] FieldCountChange = new int[3, 9, 9][];

		public static void InitializeFieldCountChanges()
		{
			//Basic field count changes
			for(int a = 0; a < 3; a++)
			{
				for (int b = 0; b < 9; b++)
				{
					for(int c = 0; c < 9; c++)
					{
						FieldCountChange[a, b, c] = new int[8];
						FieldCountChange[a, b, c][6] = GetAbsoluteFieldCountChangeForPlayer1(a, b, c);
						FieldCountChange[a, b, c][7] = GetAbsoluteFieldCountChangeForPlayer2(a, b, c);
						FieldCountChange[a, b, c][0] = FieldCountChange[a, b, c][6] - FieldCountChange[a, b, c][7];
						FieldCountChange[a, b, c][5] = GetNextFieldValue(a, b, c);
					}
				}
			}

			//Field count changes for player1 births (index = 1) and player2 births (index = 2)
			//Loops of b and c go only to 8 because b + 1 cannot be greater than 8 (anyway all FieldCountChange with b or c > 5 are always 0)
			for (int a = 0; a < 3; a++)
			{
				for (int b = 0; b < 8; b++)
				{
					for (int c = 0; c < 8; c++)
					{
						FieldCountChange[a, b, c][1] = FieldCountChange[a, b + 1, c][0] - FieldCountChange[a, b, c][0];
						FieldCountChange[a, b, c][2] = FieldCountChange[a, b, c + 1][0] - FieldCountChange[a, b, c][0];
					}
				}
			}

			//Field count changes for player1 kills (index = 3) and player2 kills (index = 4)
			for (int a = 0; a < 3; a++)
			{
				for (int b = 0; b < 9; b++)
				{
					for (int c = 0; c < 9; c++)
					{
						FieldCountChange[a, b, c][3] = FieldCountChange[a, Math.Max(0, b - 1), c][0] - FieldCountChange[a, b, c][0];
						FieldCountChange[a, b, c][4] = FieldCountChange[a, b, Math.Max(0, c - 1)][0] - FieldCountChange[a, b, c][0];
					}
				}
			}
		}

		public static int GetNextFieldValue(int a, int b, int c)
		{
			switch (b + c)
			{
				case 2:
					return a;
				case 3:
					if (a == 0)
						return b >= 2 ? Player.Player1.Value() : Player.Player2.Value();
					else
						return a;
			}
			return 0;
		}

		public static int GetAbsoluteFieldCountChangeForPlayer1(int a, int b, int c)
		{
			switch (b + c)
			{
				case 2:
					return a == 1 ? 1 : 0;
				case 3:
					if (a == 0)
						return b >= 2 ? 1 : 0;
					else
						return a == 1 ? 1 : 0;
			}
			return 0;
		}

		public static int GetAbsoluteFieldCountChangeForPlayer2(int a, int b, int c)
		{
			switch (b + c)
			{
				case 2:
					return a == 2 ? 1 : 0;
				case 3:
					if (a == 0)
						return c >= 2 ? 1 : 0;
					else
						return a == 2 ? 1 : 0;
			}
			return 0;
		}

		private Board _NextGeneration;

		/// <summary>
		/// Neighbours1[i] = number of neighbours of player1 of Field[i]
		/// </summary>
		public short[,] Neighbours = null;

		public void CalculateNeighbours()
		{
			Neighbours = new short[3, Size];

			foreach (var player in AllPlayers)
			{
				int playerIndex = player.Value();
				foreach(int i in GetCells(player))
				{
					for(int j = 0; j < NeighbourFields[i].Length; j++)
					{
						Neighbours[playerIndex, NeighbourFields[i][j]]++;
					}
				}
			}
		}

		#endregion

		#region ToString

		public override string ToString() => $"Round {Round}, {MyPlayer}: my count: {MyPlayerFieldCount}; his count: {OpponentPlayerFieldCount}";

		public string BoardString()
		{
			return BoardString(Width, Height);
		}

		public string BoardString(int width, int height)
		{
			var result = new StringBuilder();
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					if (result.Length > 0) result.Append(",");
					int ix = new Position(x, y).Index;
					switch (Field[ix])
					{
						case 0: result.Append("."); break;
						case 1: result.Append("0"); break;
						case 2: result.Append("1"); break;
					}
				}
			}
			return result.ToString();
		}

		public string HumanBoardString()
		{
			return HumanBoardString(Width, Height);
		}

		public string HumanBoardString(int width, int height)
		{
			var result = new StringBuilder();
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int ix = new Position(x, y).Index;
					switch (Field[ix])
					{
						case 0: result.Append("."); break;
						case 1: result.Append("0"); break;
						case 2: result.Append("1"); break;
					}
				}
				result.Append(Environment.NewLine);
			}
			return result.ToString();
		}

		#endregion

		#region Helper arrays

		public static int[][] NeighbourFields = new[]
		{
			new int[] {1,17,16 },
			new int[] {0,2,16,18,17 },
			new int[] {1,3,17,19,18 },
			new int[] {2,4,18,20,19 },
			new int[] {3,5,19,21,20 },
			new int[] {4,6,20,22,21 },
			new int[] {5,7,21,23,22 },
			new int[] {6,8,22,24,23 },
			new int[] {7,9,23,25,24 },
			new int[] {8,10,24,26,25 },
			new int[] {9,11,25,27,26 },
			new int[] {10,12,26,28,27 },
			new int[] {11,13,27,29,28 },
			new int[] {12,14,28,30,29 },
			new int[] {13,15,29,31,30 },
			new int[] {14,30,31 },
			new int[] {1,17,33,0,32 },
			new int[] {0,2,16,18,32,34,1,33 },
			new int[] {1,3,17,19,33,35,2,34 },
			new int[] {2,4,18,20,34,36,3,35 },
			new int[] {3,5,19,21,35,37,4,36 },
			new int[] {4,6,20,22,36,38,5,37 },
			new int[] {5,7,21,23,37,39,6,38 },
			new int[] {6,8,22,24,38,40,7,39 },
			new int[] {7,9,23,25,39,41,8,40 },
			new int[] {8,10,24,26,40,42,9,41 },
			new int[] {9,11,25,27,41,43,10,42 },
			new int[] {10,12,26,28,42,44,11,43 },
			new int[] {11,13,27,29,43,45,12,44 },
			new int[] {12,14,28,30,44,46,13,45 },
			new int[] {13,15,29,31,45,47,14,46 },
			new int[] {14,30,46,15,47 },
			new int[] {17,33,49,16,48 },
			new int[] {16,18,32,34,48,50,17,49 },
			new int[] {17,19,33,35,49,51,18,50 },
			new int[] {18,20,34,36,50,52,19,51 },
			new int[] {19,21,35,37,51,53,20,52 },
			new int[] {20,22,36,38,52,54,21,53 },
			new int[] {21,23,37,39,53,55,22,54 },
			new int[] {22,24,38,40,54,56,23,55 },
			new int[] {23,25,39,41,55,57,24,56 },
			new int[] {24,26,40,42,56,58,25,57 },
			new int[] {25,27,41,43,57,59,26,58 },
			new int[] {26,28,42,44,58,60,27,59 },
			new int[] {27,29,43,45,59,61,28,60 },
			new int[] {28,30,44,46,60,62,29,61 },
			new int[] {29,31,45,47,61,63,30,62 },
			new int[] {30,46,62,31,63 },
			new int[] {33,49,65,32,64 },
			new int[] {32,34,48,50,64,66,33,65 },
			new int[] {33,35,49,51,65,67,34,66 },
			new int[] {34,36,50,52,66,68,35,67 },
			new int[] {35,37,51,53,67,69,36,68 },
			new int[] {36,38,52,54,68,70,37,69 },
			new int[] {37,39,53,55,69,71,38,70 },
			new int[] {38,40,54,56,70,72,39,71 },
			new int[] {39,41,55,57,71,73,40,72 },
			new int[] {40,42,56,58,72,74,41,73 },
			new int[] {41,43,57,59,73,75,42,74 },
			new int[] {42,44,58,60,74,76,43,75 },
			new int[] {43,45,59,61,75,77,44,76 },
			new int[] {44,46,60,62,76,78,45,77 },
			new int[] {45,47,61,63,77,79,46,78 },
			new int[] {46,62,78,47,79 },
			new int[] {49,65,81,48,80 },
			new int[] {48,50,64,66,80,82,49,81 },
			new int[] {49,51,65,67,81,83,50,82 },
			new int[] {50,52,66,68,82,84,51,83 },
			new int[] {51,53,67,69,83,85,52,84 },
			new int[] {52,54,68,70,84,86,53,85 },
			new int[] {53,55,69,71,85,87,54,86 },
			new int[] {54,56,70,72,86,88,55,87 },
			new int[] {55,57,71,73,87,89,56,88 },
			new int[] {56,58,72,74,88,90,57,89 },
			new int[] {57,59,73,75,89,91,58,90 },
			new int[] {58,60,74,76,90,92,59,91 },
			new int[] {59,61,75,77,91,93,60,92 },
			new int[] {60,62,76,78,92,94,61,93 },
			new int[] {61,63,77,79,93,95,62,94 },
			new int[] {62,78,94,63,95 },
			new int[] {65,81,97,64,96 },
			new int[] {64,66,80,82,96,98,65,97 },
			new int[] {65,67,81,83,97,99,66,98 },
			new int[] {66,68,82,84,98,100,67,99 },
			new int[] {67,69,83,85,99,101,68,100 },
			new int[] {68,70,84,86,100,102,69,101 },
			new int[] {69,71,85,87,101,103,70,102 },
			new int[] {70,72,86,88,102,104,71,103 },
			new int[] {71,73,87,89,103,105,72,104 },
			new int[] {72,74,88,90,104,106,73,105 },
			new int[] {73,75,89,91,105,107,74,106 },
			new int[] {74,76,90,92,106,108,75,107 },
			new int[] {75,77,91,93,107,109,76,108 },
			new int[] {76,78,92,94,108,110,77,109 },
			new int[] {77,79,93,95,109,111,78,110 },
			new int[] {78,94,110,79,111 },
			new int[] {81,97,113,80,112 },
			new int[] {80,82,96,98,112,114,81,113 },
			new int[] {81,83,97,99,113,115,82,114 },
			new int[] {82,84,98,100,114,116,83,115 },
			new int[] {83,85,99,101,115,117,84,116 },
			new int[] {84,86,100,102,116,118,85,117 },
			new int[] {85,87,101,103,117,119,86,118 },
			new int[] {86,88,102,104,118,120,87,119 },
			new int[] {87,89,103,105,119,121,88,120 },
			new int[] {88,90,104,106,120,122,89,121 },
			new int[] {89,91,105,107,121,123,90,122 },
			new int[] {90,92,106,108,122,124,91,123 },
			new int[] {91,93,107,109,123,125,92,124 },
			new int[] {92,94,108,110,124,126,93,125 },
			new int[] {93,95,109,111,125,127,94,126 },
			new int[] {94,110,126,95,127 },
			new int[] {97,113,129,96,128 },
			new int[] {96,98,112,114,128,130,97,129 },
			new int[] {97,99,113,115,129,131,98,130 },
			new int[] {98,100,114,116,130,132,99,131 },
			new int[] {99,101,115,117,131,133,100,132 },
			new int[] {100,102,116,118,132,134,101,133 },
			new int[] {101,103,117,119,133,135,102,134 },
			new int[] {102,104,118,120,134,136,103,135 },
			new int[] {103,105,119,121,135,137,104,136 },
			new int[] {104,106,120,122,136,138,105,137 },
			new int[] {105,107,121,123,137,139,106,138 },
			new int[] {106,108,122,124,138,140,107,139 },
			new int[] {107,109,123,125,139,141,108,140 },
			new int[] {108,110,124,126,140,142,109,141 },
			new int[] {109,111,125,127,141,143,110,142 },
			new int[] {110,126,142,111,143 },
			new int[] {113,129,145,112,144 },
			new int[] {112,114,128,130,144,146,113,145 },
			new int[] {113,115,129,131,145,147,114,146 },
			new int[] {114,116,130,132,146,148,115,147 },
			new int[] {115,117,131,133,147,149,116,148 },
			new int[] {116,118,132,134,148,150,117,149 },
			new int[] {117,119,133,135,149,151,118,150 },
			new int[] {118,120,134,136,150,152,119,151 },
			new int[] {119,121,135,137,151,153,120,152 },
			new int[] {120,122,136,138,152,154,121,153 },
			new int[] {121,123,137,139,153,155,122,154 },
			new int[] {122,124,138,140,154,156,123,155 },
			new int[] {123,125,139,141,155,157,124,156 },
			new int[] {124,126,140,142,156,158,125,157 },
			new int[] {125,127,141,143,157,159,126,158 },
			new int[] {126,142,158,127,159 },
			new int[] {129,145,161,128,160 },
			new int[] {128,130,144,146,160,162,129,161 },
			new int[] {129,131,145,147,161,163,130,162 },
			new int[] {130,132,146,148,162,164,131,163 },
			new int[] {131,133,147,149,163,165,132,164 },
			new int[] {132,134,148,150,164,166,133,165 },
			new int[] {133,135,149,151,165,167,134,166 },
			new int[] {134,136,150,152,166,168,135,167 },
			new int[] {135,137,151,153,167,169,136,168 },
			new int[] {136,138,152,154,168,170,137,169 },
			new int[] {137,139,153,155,169,171,138,170 },
			new int[] {138,140,154,156,170,172,139,171 },
			new int[] {139,141,155,157,171,173,140,172 },
			new int[] {140,142,156,158,172,174,141,173 },
			new int[] {141,143,157,159,173,175,142,174 },
			new int[] {142,158,174,143,175 },
			new int[] {145,161,177,144,176 },
			new int[] {144,146,160,162,176,178,145,177 },
			new int[] {145,147,161,163,177,179,146,178 },
			new int[] {146,148,162,164,178,180,147,179 },
			new int[] {147,149,163,165,179,181,148,180 },
			new int[] {148,150,164,166,180,182,149,181 },
			new int[] {149,151,165,167,181,183,150,182 },
			new int[] {150,152,166,168,182,184,151,183 },
			new int[] {151,153,167,169,183,185,152,184 },
			new int[] {152,154,168,170,184,186,153,185 },
			new int[] {153,155,169,171,185,187,154,186 },
			new int[] {154,156,170,172,186,188,155,187 },
			new int[] {155,157,171,173,187,189,156,188 },
			new int[] {156,158,172,174,188,190,157,189 },
			new int[] {157,159,173,175,189,191,158,190 },
			new int[] {158,174,190,159,191 },
			new int[] {161,177,193,160,192 },
			new int[] {160,162,176,178,192,194,161,193 },
			new int[] {161,163,177,179,193,195,162,194 },
			new int[] {162,164,178,180,194,196,163,195 },
			new int[] {163,165,179,181,195,197,164,196 },
			new int[] {164,166,180,182,196,198,165,197 },
			new int[] {165,167,181,183,197,199,166,198 },
			new int[] {166,168,182,184,198,200,167,199 },
			new int[] {167,169,183,185,199,201,168,200 },
			new int[] {168,170,184,186,200,202,169,201 },
			new int[] {169,171,185,187,201,203,170,202 },
			new int[] {170,172,186,188,202,204,171,203 },
			new int[] {171,173,187,189,203,205,172,204 },
			new int[] {172,174,188,190,204,206,173,205 },
			new int[] {173,175,189,191,205,207,174,206 },
			new int[] {174,190,206,175,207 },
			new int[] {177,193,209,176,208 },
			new int[] {176,178,192,194,208,210,177,209 },
			new int[] {177,179,193,195,209,211,178,210 },
			new int[] {178,180,194,196,210,212,179,211 },
			new int[] {179,181,195,197,211,213,180,212 },
			new int[] {180,182,196,198,212,214,181,213 },
			new int[] {181,183,197,199,213,215,182,214 },
			new int[] {182,184,198,200,214,216,183,215 },
			new int[] {183,185,199,201,215,217,184,216 },
			new int[] {184,186,200,202,216,218,185,217 },
			new int[] {185,187,201,203,217,219,186,218 },
			new int[] {186,188,202,204,218,220,187,219 },
			new int[] {187,189,203,205,219,221,188,220 },
			new int[] {188,190,204,206,220,222,189,221 },
			new int[] {189,191,205,207,221,223,190,222 },
			new int[] {190,206,222,191,223 },
			new int[] {193,209,225,192,224 },
			new int[] {192,194,208,210,224,226,193,225 },
			new int[] {193,195,209,211,225,227,194,226 },
			new int[] {194,196,210,212,226,228,195,227 },
			new int[] {195,197,211,213,227,229,196,228 },
			new int[] {196,198,212,214,228,230,197,229 },
			new int[] {197,199,213,215,229,231,198,230 },
			new int[] {198,200,214,216,230,232,199,231 },
			new int[] {199,201,215,217,231,233,200,232 },
			new int[] {200,202,216,218,232,234,201,233 },
			new int[] {201,203,217,219,233,235,202,234 },
			new int[] {202,204,218,220,234,236,203,235 },
			new int[] {203,205,219,221,235,237,204,236 },
			new int[] {204,206,220,222,236,238,205,237 },
			new int[] {205,207,221,223,237,239,206,238 },
			new int[] {206,222,238,207,239 },
			new int[] {209,225,241,208,240 },
			new int[] {208,210,224,226,240,242,209,241 },
			new int[] {209,211,225,227,241,243,210,242 },
			new int[] {210,212,226,228,242,244,211,243 },
			new int[] {211,213,227,229,243,245,212,244 },
			new int[] {212,214,228,230,244,246,213,245 },
			new int[] {213,215,229,231,245,247,214,246 },
			new int[] {214,216,230,232,246,248,215,247 },
			new int[] {215,217,231,233,247,249,216,248 },
			new int[] {216,218,232,234,248,250,217,249 },
			new int[] {217,219,233,235,249,251,218,250 },
			new int[] {218,220,234,236,250,252,219,251 },
			new int[] {219,221,235,237,251,253,220,252 },
			new int[] {220,222,236,238,252,254,221,253 },
			new int[] {221,223,237,239,253,255,222,254 },
			new int[] {222,238,254,223,255 },
			new int[] {225,241,257,224,256 },
			new int[] {224,226,240,242,256,258,225,257 },
			new int[] {225,227,241,243,257,259,226,258 },
			new int[] {226,228,242,244,258,260,227,259 },
			new int[] {227,229,243,245,259,261,228,260 },
			new int[] {228,230,244,246,260,262,229,261 },
			new int[] {229,231,245,247,261,263,230,262 },
			new int[] {230,232,246,248,262,264,231,263 },
			new int[] {231,233,247,249,263,265,232,264 },
			new int[] {232,234,248,250,264,266,233,265 },
			new int[] {233,235,249,251,265,267,234,266 },
			new int[] {234,236,250,252,266,268,235,267 },
			new int[] {235,237,251,253,267,269,236,268 },
			new int[] {236,238,252,254,268,270,237,269 },
			new int[] {237,239,253,255,269,271,238,270 },
			new int[] {238,254,270,239,271 },
			new int[] {241,257,273,240,272 },
			new int[] {240,242,256,258,272,274,241,273 },
			new int[] {241,243,257,259,273,275,242,274 },
			new int[] {242,244,258,260,274,276,243,275 },
			new int[] {243,245,259,261,275,277,244,276 },
			new int[] {244,246,260,262,276,278,245,277 },
			new int[] {245,247,261,263,277,279,246,278 },
			new int[] {246,248,262,264,278,280,247,279 },
			new int[] {247,249,263,265,279,281,248,280 },
			new int[] {248,250,264,266,280,282,249,281 },
			new int[] {249,251,265,267,281,283,250,282 },
			new int[] {250,252,266,268,282,284,251,283 },
			new int[] {251,253,267,269,283,285,252,284 },
			new int[] {252,254,268,270,284,286,253,285 },
			new int[] {253,255,269,271,285,287,254,286 },
			new int[] {254,270,286,255,287 },
			new int[] {257,273,256 },
			new int[] {256,258,272,274,257 },
			new int[] {257,259,273,275,258 },
			new int[] {258,260,274,276,259 },
			new int[] {259,261,275,277,260 },
			new int[] {260,262,276,278,261 },
			new int[] {261,263,277,279,262 },
			new int[] {262,264,278,280,263 },
			new int[] {263,265,279,281,264 },
			new int[] {264,266,280,282,265 },
			new int[] {265,267,281,283,266 },
			new int[] {266,268,282,284,267 },
			new int[] {267,269,283,285,268 },
			new int[] {268,270,284,286,269 },
			new int[] {269,271,285,287,270 },
			new int[] {270,286,271 },
		};

		public static int[][] NeighbourFieldsAndThis = new[]
		{
			new int[] {0,1,17,16 },
			new int[] {1,0,2,16,18,17 },
			new int[] {2,1,3,17,19,18 },
			new int[] {3,2,4,18,20,19 },
			new int[] {4,3,5,19,21,20 },
			new int[] {5,4,6,20,22,21 },
			new int[] {6,5,7,21,23,22 },
			new int[] {7,6,8,22,24,23 },
			new int[] {8,7,9,23,25,24 },
			new int[] {9,8,10,24,26,25 },
			new int[] {10,9,11,25,27,26 },
			new int[] {11,10,12,26,28,27 },
			new int[] {12,11,13,27,29,28 },
			new int[] {13,12,14,28,30,29 },
			new int[] {14,13,15,29,31,30 },
			new int[] {15,14,30,31 },
			new int[] {16,1,17,33,0,32 },
			new int[] {17,0,2,16,18,32,34,1,33 },
			new int[] {18,1,3,17,19,33,35,2,34 },
			new int[] {19,2,4,18,20,34,36,3,35 },
			new int[] {20,3,5,19,21,35,37,4,36 },
			new int[] {21,4,6,20,22,36,38,5,37 },
			new int[] {22,5,7,21,23,37,39,6,38 },
			new int[] {23,6,8,22,24,38,40,7,39 },
			new int[] {24,7,9,23,25,39,41,8,40 },
			new int[] {25,8,10,24,26,40,42,9,41 },
			new int[] {26,9,11,25,27,41,43,10,42 },
			new int[] {27,10,12,26,28,42,44,11,43 },
			new int[] {28,11,13,27,29,43,45,12,44 },
			new int[] {29,12,14,28,30,44,46,13,45 },
			new int[] {30,13,15,29,31,45,47,14,46 },
			new int[] {31,14,30,46,15,47 },
			new int[] {32,17,33,49,16,48 },
			new int[] {33,16,18,32,34,48,50,17,49 },
			new int[] {34,17,19,33,35,49,51,18,50 },
			new int[] {35,18,20,34,36,50,52,19,51 },
			new int[] {36,19,21,35,37,51,53,20,52 },
			new int[] {37,20,22,36,38,52,54,21,53 },
			new int[] {38,21,23,37,39,53,55,22,54 },
			new int[] {39,22,24,38,40,54,56,23,55 },
			new int[] {40,23,25,39,41,55,57,24,56 },
			new int[] {41,24,26,40,42,56,58,25,57 },
			new int[] {42,25,27,41,43,57,59,26,58 },
			new int[] {43,26,28,42,44,58,60,27,59 },
			new int[] {44,27,29,43,45,59,61,28,60 },
			new int[] {45,28,30,44,46,60,62,29,61 },
			new int[] {46,29,31,45,47,61,63,30,62 },
			new int[] {47,30,46,62,31,63 },
			new int[] {48,33,49,65,32,64 },
			new int[] {49,32,34,48,50,64,66,33,65 },
			new int[] {50,33,35,49,51,65,67,34,66 },
			new int[] {51,34,36,50,52,66,68,35,67 },
			new int[] {52,35,37,51,53,67,69,36,68 },
			new int[] {53,36,38,52,54,68,70,37,69 },
			new int[] {54,37,39,53,55,69,71,38,70 },
			new int[] {55,38,40,54,56,70,72,39,71 },
			new int[] {56,39,41,55,57,71,73,40,72 },
			new int[] {57,40,42,56,58,72,74,41,73 },
			new int[] {58,41,43,57,59,73,75,42,74 },
			new int[] {59,42,44,58,60,74,76,43,75 },
			new int[] {60,43,45,59,61,75,77,44,76 },
			new int[] {61,44,46,60,62,76,78,45,77 },
			new int[] {62,45,47,61,63,77,79,46,78 },
			new int[] {63,46,62,78,47,79 },
			new int[] {64,49,65,81,48,80 },
			new int[] {65,48,50,64,66,80,82,49,81 },
			new int[] {66,49,51,65,67,81,83,50,82 },
			new int[] {67,50,52,66,68,82,84,51,83 },
			new int[] {68,51,53,67,69,83,85,52,84 },
			new int[] {69,52,54,68,70,84,86,53,85 },
			new int[] {70,53,55,69,71,85,87,54,86 },
			new int[] {71,54,56,70,72,86,88,55,87 },
			new int[] {72,55,57,71,73,87,89,56,88 },
			new int[] {73,56,58,72,74,88,90,57,89 },
			new int[] {74,57,59,73,75,89,91,58,90 },
			new int[] {75,58,60,74,76,90,92,59,91 },
			new int[] {76,59,61,75,77,91,93,60,92 },
			new int[] {77,60,62,76,78,92,94,61,93 },
			new int[] {78,61,63,77,79,93,95,62,94 },
			new int[] {79,62,78,94,63,95 },
			new int[] {80,65,81,97,64,96 },
			new int[] {81,64,66,80,82,96,98,65,97 },
			new int[] {82,65,67,81,83,97,99,66,98 },
			new int[] {83,66,68,82,84,98,100,67,99 },
			new int[] {84,67,69,83,85,99,101,68,100 },
			new int[] {85,68,70,84,86,100,102,69,101 },
			new int[] {86,69,71,85,87,101,103,70,102 },
			new int[] {87,70,72,86,88,102,104,71,103 },
			new int[] {88,71,73,87,89,103,105,72,104 },
			new int[] {89,72,74,88,90,104,106,73,105 },
			new int[] {90,73,75,89,91,105,107,74,106 },
			new int[] {91,74,76,90,92,106,108,75,107 },
			new int[] {92,75,77,91,93,107,109,76,108 },
			new int[] {93,76,78,92,94,108,110,77,109 },
			new int[] {94,77,79,93,95,109,111,78,110 },
			new int[] {95,78,94,110,79,111 },
			new int[] {96,81,97,113,80,112 },
			new int[] {97,80,82,96,98,112,114,81,113 },
			new int[] {98,81,83,97,99,113,115,82,114 },
			new int[] {99,82,84,98,100,114,116,83,115 },
			new int[] {100,83,85,99,101,115,117,84,116 },
			new int[] {101,84,86,100,102,116,118,85,117 },
			new int[] {102,85,87,101,103,117,119,86,118 },
			new int[] {103,86,88,102,104,118,120,87,119 },
			new int[] {104,87,89,103,105,119,121,88,120 },
			new int[] {105,88,90,104,106,120,122,89,121 },
			new int[] {106,89,91,105,107,121,123,90,122 },
			new int[] {107,90,92,106,108,122,124,91,123 },
			new int[] {108,91,93,107,109,123,125,92,124 },
			new int[] {109,92,94,108,110,124,126,93,125 },
			new int[] {110,93,95,109,111,125,127,94,126 },
			new int[] {111,94,110,126,95,127 },
			new int[] {112,97,113,129,96,128 },
			new int[] {113,96,98,112,114,128,130,97,129 },
			new int[] {114,97,99,113,115,129,131,98,130 },
			new int[] {115,98,100,114,116,130,132,99,131 },
			new int[] {116,99,101,115,117,131,133,100,132 },
			new int[] {117,100,102,116,118,132,134,101,133 },
			new int[] {118,101,103,117,119,133,135,102,134 },
			new int[] {119,102,104,118,120,134,136,103,135 },
			new int[] {120,103,105,119,121,135,137,104,136 },
			new int[] {121,104,106,120,122,136,138,105,137 },
			new int[] {122,105,107,121,123,137,139,106,138 },
			new int[] {123,106,108,122,124,138,140,107,139 },
			new int[] {124,107,109,123,125,139,141,108,140 },
			new int[] {125,108,110,124,126,140,142,109,141 },
			new int[] {126,109,111,125,127,141,143,110,142 },
			new int[] {127,110,126,142,111,143 },
			new int[] {128,113,129,145,112,144 },
			new int[] {129,112,114,128,130,144,146,113,145 },
			new int[] {130,113,115,129,131,145,147,114,146 },
			new int[] {131,114,116,130,132,146,148,115,147 },
			new int[] {132,115,117,131,133,147,149,116,148 },
			new int[] {133,116,118,132,134,148,150,117,149 },
			new int[] {134,117,119,133,135,149,151,118,150 },
			new int[] {135,118,120,134,136,150,152,119,151 },
			new int[] {136,119,121,135,137,151,153,120,152 },
			new int[] {137,120,122,136,138,152,154,121,153 },
			new int[] {138,121,123,137,139,153,155,122,154 },
			new int[] {139,122,124,138,140,154,156,123,155 },
			new int[] {140,123,125,139,141,155,157,124,156 },
			new int[] {141,124,126,140,142,156,158,125,157 },
			new int[] {142,125,127,141,143,157,159,126,158 },
			new int[] {143,126,142,158,127,159 },
			new int[] {144,129,145,161,128,160 },
			new int[] {145,128,130,144,146,160,162,129,161 },
			new int[] {146,129,131,145,147,161,163,130,162 },
			new int[] {147,130,132,146,148,162,164,131,163 },
			new int[] {148,131,133,147,149,163,165,132,164 },
			new int[] {149,132,134,148,150,164,166,133,165 },
			new int[] {150,133,135,149,151,165,167,134,166 },
			new int[] {151,134,136,150,152,166,168,135,167 },
			new int[] {152,135,137,151,153,167,169,136,168 },
			new int[] {153,136,138,152,154,168,170,137,169 },
			new int[] {154,137,139,153,155,169,171,138,170 },
			new int[] {155,138,140,154,156,170,172,139,171 },
			new int[] {156,139,141,155,157,171,173,140,172 },
			new int[] {157,140,142,156,158,172,174,141,173 },
			new int[] {158,141,143,157,159,173,175,142,174 },
			new int[] {159,142,158,174,143,175 },
			new int[] {160,145,161,177,144,176 },
			new int[] {161,144,146,160,162,176,178,145,177 },
			new int[] {162,145,147,161,163,177,179,146,178 },
			new int[] {163,146,148,162,164,178,180,147,179 },
			new int[] {164,147,149,163,165,179,181,148,180 },
			new int[] {165,148,150,164,166,180,182,149,181 },
			new int[] {166,149,151,165,167,181,183,150,182 },
			new int[] {167,150,152,166,168,182,184,151,183 },
			new int[] {168,151,153,167,169,183,185,152,184 },
			new int[] {169,152,154,168,170,184,186,153,185 },
			new int[] {170,153,155,169,171,185,187,154,186 },
			new int[] {171,154,156,170,172,186,188,155,187 },
			new int[] {172,155,157,171,173,187,189,156,188 },
			new int[] {173,156,158,172,174,188,190,157,189 },
			new int[] {174,157,159,173,175,189,191,158,190 },
			new int[] {175,158,174,190,159,191 },
			new int[] {176,161,177,193,160,192 },
			new int[] {177,160,162,176,178,192,194,161,193 },
			new int[] {178,161,163,177,179,193,195,162,194 },
			new int[] {179,162,164,178,180,194,196,163,195 },
			new int[] {180,163,165,179,181,195,197,164,196 },
			new int[] {181,164,166,180,182,196,198,165,197 },
			new int[] {182,165,167,181,183,197,199,166,198 },
			new int[] {183,166,168,182,184,198,200,167,199 },
			new int[] {184,167,169,183,185,199,201,168,200 },
			new int[] {185,168,170,184,186,200,202,169,201 },
			new int[] {186,169,171,185,187,201,203,170,202 },
			new int[] {187,170,172,186,188,202,204,171,203 },
			new int[] {188,171,173,187,189,203,205,172,204 },
			new int[] {189,172,174,188,190,204,206,173,205 },
			new int[] {190,173,175,189,191,205,207,174,206 },
			new int[] {191,174,190,206,175,207 },
			new int[] {192,177,193,209,176,208 },
			new int[] {193,176,178,192,194,208,210,177,209 },
			new int[] {194,177,179,193,195,209,211,178,210 },
			new int[] {195,178,180,194,196,210,212,179,211 },
			new int[] {196,179,181,195,197,211,213,180,212 },
			new int[] {197,180,182,196,198,212,214,181,213 },
			new int[] {198,181,183,197,199,213,215,182,214 },
			new int[] {199,182,184,198,200,214,216,183,215 },
			new int[] {200,183,185,199,201,215,217,184,216 },
			new int[] {201,184,186,200,202,216,218,185,217 },
			new int[] {202,185,187,201,203,217,219,186,218 },
			new int[] {203,186,188,202,204,218,220,187,219 },
			new int[] {204,187,189,203,205,219,221,188,220 },
			new int[] {205,188,190,204,206,220,222,189,221 },
			new int[] {206,189,191,205,207,221,223,190,222 },
			new int[] {207,190,206,222,191,223 },
			new int[] {208,193,209,225,192,224 },
			new int[] {209,192,194,208,210,224,226,193,225 },
			new int[] {210,193,195,209,211,225,227,194,226 },
			new int[] {211,194,196,210,212,226,228,195,227 },
			new int[] {212,195,197,211,213,227,229,196,228 },
			new int[] {213,196,198,212,214,228,230,197,229 },
			new int[] {214,197,199,213,215,229,231,198,230 },
			new int[] {215,198,200,214,216,230,232,199,231 },
			new int[] {216,199,201,215,217,231,233,200,232 },
			new int[] {217,200,202,216,218,232,234,201,233 },
			new int[] {218,201,203,217,219,233,235,202,234 },
			new int[] {219,202,204,218,220,234,236,203,235 },
			new int[] {220,203,205,219,221,235,237,204,236 },
			new int[] {221,204,206,220,222,236,238,205,237 },
			new int[] {222,205,207,221,223,237,239,206,238 },
			new int[] {223,206,222,238,207,239 },
			new int[] {224,209,225,241,208,240 },
			new int[] {225,208,210,224,226,240,242,209,241 },
			new int[] {226,209,211,225,227,241,243,210,242 },
			new int[] {227,210,212,226,228,242,244,211,243 },
			new int[] {228,211,213,227,229,243,245,212,244 },
			new int[] {229,212,214,228,230,244,246,213,245 },
			new int[] {230,213,215,229,231,245,247,214,246 },
			new int[] {231,214,216,230,232,246,248,215,247 },
			new int[] {232,215,217,231,233,247,249,216,248 },
			new int[] {233,216,218,232,234,248,250,217,249 },
			new int[] {234,217,219,233,235,249,251,218,250 },
			new int[] {235,218,220,234,236,250,252,219,251 },
			new int[] {236,219,221,235,237,251,253,220,252 },
			new int[] {237,220,222,236,238,252,254,221,253 },
			new int[] {238,221,223,237,239,253,255,222,254 },
			new int[] {239,222,238,254,223,255 },
			new int[] {240,225,241,257,224,256 },
			new int[] {241,224,226,240,242,256,258,225,257 },
			new int[] {242,225,227,241,243,257,259,226,258 },
			new int[] {243,226,228,242,244,258,260,227,259 },
			new int[] {244,227,229,243,245,259,261,228,260 },
			new int[] {245,228,230,244,246,260,262,229,261 },
			new int[] {246,229,231,245,247,261,263,230,262 },
			new int[] {247,230,232,246,248,262,264,231,263 },
			new int[] {248,231,233,247,249,263,265,232,264 },
			new int[] {249,232,234,248,250,264,266,233,265 },
			new int[] {250,233,235,249,251,265,267,234,266 },
			new int[] {251,234,236,250,252,266,268,235,267 },
			new int[] {252,235,237,251,253,267,269,236,268 },
			new int[] {253,236,238,252,254,268,270,237,269 },
			new int[] {254,237,239,253,255,269,271,238,270 },
			new int[] {255,238,254,270,239,271 },
			new int[] {256,241,257,273,240,272 },
			new int[] {257,240,242,256,258,272,274,241,273 },
			new int[] {258,241,243,257,259,273,275,242,274 },
			new int[] {259,242,244,258,260,274,276,243,275 },
			new int[] {260,243,245,259,261,275,277,244,276 },
			new int[] {261,244,246,260,262,276,278,245,277 },
			new int[] {262,245,247,261,263,277,279,246,278 },
			new int[] {263,246,248,262,264,278,280,247,279 },
			new int[] {264,247,249,263,265,279,281,248,280 },
			new int[] {265,248,250,264,266,280,282,249,281 },
			new int[] {266,249,251,265,267,281,283,250,282 },
			new int[] {267,250,252,266,268,282,284,251,283 },
			new int[] {268,251,253,267,269,283,285,252,284 },
			new int[] {269,252,254,268,270,284,286,253,285 },
			new int[] {270,253,255,269,271,285,287,254,286 },
			new int[] {271,254,270,286,255,287 },
			new int[] {272,257,273,256 },
			new int[] {273,256,258,272,274,257 },
			new int[] {274,257,259,273,275,258 },
			new int[] {275,258,260,274,276,259 },
			new int[] {276,259,261,275,277,260 },
			new int[] {277,260,262,276,278,261 },
			new int[] {278,261,263,277,279,262 },
			new int[] {279,262,264,278,280,263 },
			new int[] {280,263,265,279,281,264 },
			new int[] {281,264,266,280,282,265 },
			new int[] {282,265,267,281,283,266 },
			new int[] {283,266,268,282,284,267 },
			new int[] {284,267,269,283,285,268 },
			new int[] {285,268,270,284,286,269 },
			new int[] {286,269,271,285,287,270 },
			new int[] {287,270,286,271 },
		};

		public static int[][] NeighbourFields2 = new[]
		{
			new int[] {0,1,2,16,17,18,32,33,34 },
			new int[] {0,1,2,3,16,17,18,19,32,33,34,35 },
			new int[] {0,1,2,3,4,16,17,18,19,20,32,33,34,35,36 },
			new int[] {1,2,3,4,5,17,18,19,20,21,33,34,35,36,37 },
			new int[] {2,3,4,5,6,18,19,20,21,22,34,35,36,37,38 },
			new int[] {3,4,5,6,7,19,20,21,22,23,35,36,37,38,39 },
			new int[] {4,5,6,7,8,20,21,22,23,24,36,37,38,39,40 },
			new int[] {5,6,7,8,9,21,22,23,24,25,37,38,39,40,41 },
			new int[] {6,7,8,9,10,22,23,24,25,26,38,39,40,41,42 },
			new int[] {7,8,9,10,11,23,24,25,26,27,39,40,41,42,43 },
			new int[] {8,9,10,11,12,24,25,26,27,28,40,41,42,43,44 },
			new int[] {9,10,11,12,13,25,26,27,28,29,41,42,43,44,45 },
			new int[] {10,11,12,13,14,26,27,28,29,30,42,43,44,45,46 },
			new int[] {11,12,13,14,15,27,28,29,30,31,43,44,45,46,47 },
			new int[] {12,13,14,15,28,29,30,31,44,45,46,47 },
			new int[] {13,14,15,29,30,31,45,46,47 },
			new int[] {0,1,2,16,17,18,32,33,34,48,49,50 },
			new int[] {0,1,2,3,16,17,18,19,32,33,34,35,48,49,50,51 },
			new int[] {0,1,2,3,4,16,17,18,19,20,32,33,34,35,36,48,49,50,51,52 },
			new int[] {1,2,3,4,5,17,18,19,20,21,33,34,35,36,37,49,50,51,52,53 },
			new int[] {2,3,4,5,6,18,19,20,21,22,34,35,36,37,38,50,51,52,53,54 },
			new int[] {3,4,5,6,7,19,20,21,22,23,35,36,37,38,39,51,52,53,54,55 },
			new int[] {4,5,6,7,8,20,21,22,23,24,36,37,38,39,40,52,53,54,55,56 },
			new int[] {5,6,7,8,9,21,22,23,24,25,37,38,39,40,41,53,54,55,56,57 },
			new int[] {6,7,8,9,10,22,23,24,25,26,38,39,40,41,42,54,55,56,57,58 },
			new int[] {7,8,9,10,11,23,24,25,26,27,39,40,41,42,43,55,56,57,58,59 },
			new int[] {8,9,10,11,12,24,25,26,27,28,40,41,42,43,44,56,57,58,59,60 },
			new int[] {9,10,11,12,13,25,26,27,28,29,41,42,43,44,45,57,58,59,60,61 },
			new int[] {10,11,12,13,14,26,27,28,29,30,42,43,44,45,46,58,59,60,61,62 },
			new int[] {11,12,13,14,15,27,28,29,30,31,43,44,45,46,47,59,60,61,62,63 },
			new int[] {12,13,14,15,28,29,30,31,44,45,46,47,60,61,62,63 },
			new int[] {13,14,15,29,30,31,45,46,47,61,62,63 },
			new int[] {0,1,2,16,17,18,32,33,34,48,49,50,64,65,66 },
			new int[] {0,1,2,3,16,17,18,19,32,33,34,35,48,49,50,51,64,65,66,67 },
			new int[] {0,1,2,3,4,16,17,18,19,20,32,33,34,35,36,48,49,50,51,52,64,65,66,67,68 },
			new int[] {1,2,3,4,5,17,18,19,20,21,33,34,35,36,37,49,50,51,52,53,65,66,67,68,69 },
			new int[] {2,3,4,5,6,18,19,20,21,22,34,35,36,37,38,50,51,52,53,54,66,67,68,69,70 },
			new int[] {3,4,5,6,7,19,20,21,22,23,35,36,37,38,39,51,52,53,54,55,67,68,69,70,71 },
			new int[] {4,5,6,7,8,20,21,22,23,24,36,37,38,39,40,52,53,54,55,56,68,69,70,71,72 },
			new int[] {5,6,7,8,9,21,22,23,24,25,37,38,39,40,41,53,54,55,56,57,69,70,71,72,73 },
			new int[] {6,7,8,9,10,22,23,24,25,26,38,39,40,41,42,54,55,56,57,58,70,71,72,73,74 },
			new int[] {7,8,9,10,11,23,24,25,26,27,39,40,41,42,43,55,56,57,58,59,71,72,73,74,75 },
			new int[] {8,9,10,11,12,24,25,26,27,28,40,41,42,43,44,56,57,58,59,60,72,73,74,75,76 },
			new int[] {9,10,11,12,13,25,26,27,28,29,41,42,43,44,45,57,58,59,60,61,73,74,75,76,77 },
			new int[] {10,11,12,13,14,26,27,28,29,30,42,43,44,45,46,58,59,60,61,62,74,75,76,77,78 },
			new int[] {11,12,13,14,15,27,28,29,30,31,43,44,45,46,47,59,60,61,62,63,75,76,77,78,79 },
			new int[] {12,13,14,15,28,29,30,31,44,45,46,47,60,61,62,63,76,77,78,79 },
			new int[] {13,14,15,29,30,31,45,46,47,61,62,63,77,78,79 },
			new int[] {16,17,18,32,33,34,48,49,50,64,65,66,80,81,82 },
			new int[] {16,17,18,19,32,33,34,35,48,49,50,51,64,65,66,67,80,81,82,83 },
			new int[] {16,17,18,19,20,32,33,34,35,36,48,49,50,51,52,64,65,66,67,68,80,81,82,83,84 },
			new int[] {17,18,19,20,21,33,34,35,36,37,49,50,51,52,53,65,66,67,68,69,81,82,83,84,85 },
			new int[] {18,19,20,21,22,34,35,36,37,38,50,51,52,53,54,66,67,68,69,70,82,83,84,85,86 },
			new int[] {19,20,21,22,23,35,36,37,38,39,51,52,53,54,55,67,68,69,70,71,83,84,85,86,87 },
			new int[] {20,21,22,23,24,36,37,38,39,40,52,53,54,55,56,68,69,70,71,72,84,85,86,87,88 },
			new int[] {21,22,23,24,25,37,38,39,40,41,53,54,55,56,57,69,70,71,72,73,85,86,87,88,89 },
			new int[] {22,23,24,25,26,38,39,40,41,42,54,55,56,57,58,70,71,72,73,74,86,87,88,89,90 },
			new int[] {23,24,25,26,27,39,40,41,42,43,55,56,57,58,59,71,72,73,74,75,87,88,89,90,91 },
			new int[] {24,25,26,27,28,40,41,42,43,44,56,57,58,59,60,72,73,74,75,76,88,89,90,91,92 },
			new int[] {25,26,27,28,29,41,42,43,44,45,57,58,59,60,61,73,74,75,76,77,89,90,91,92,93 },
			new int[] {26,27,28,29,30,42,43,44,45,46,58,59,60,61,62,74,75,76,77,78,90,91,92,93,94 },
			new int[] {27,28,29,30,31,43,44,45,46,47,59,60,61,62,63,75,76,77,78,79,91,92,93,94,95 },
			new int[] {28,29,30,31,44,45,46,47,60,61,62,63,76,77,78,79,92,93,94,95 },
			new int[] {29,30,31,45,46,47,61,62,63,77,78,79,93,94,95 },
			new int[] {32,33,34,48,49,50,64,65,66,80,81,82,96,97,98 },
			new int[] {32,33,34,35,48,49,50,51,64,65,66,67,80,81,82,83,96,97,98,99 },
			new int[] {32,33,34,35,36,48,49,50,51,52,64,65,66,67,68,80,81,82,83,84,96,97,98,99,100 },
			new int[] {33,34,35,36,37,49,50,51,52,53,65,66,67,68,69,81,82,83,84,85,97,98,99,100,101 },
			new int[] {34,35,36,37,38,50,51,52,53,54,66,67,68,69,70,82,83,84,85,86,98,99,100,101,102 },
			new int[] {35,36,37,38,39,51,52,53,54,55,67,68,69,70,71,83,84,85,86,87,99,100,101,102,103 },
			new int[] {36,37,38,39,40,52,53,54,55,56,68,69,70,71,72,84,85,86,87,88,100,101,102,103,104 },
			new int[] {37,38,39,40,41,53,54,55,56,57,69,70,71,72,73,85,86,87,88,89,101,102,103,104,105 },
			new int[] {38,39,40,41,42,54,55,56,57,58,70,71,72,73,74,86,87,88,89,90,102,103,104,105,106 },
			new int[] {39,40,41,42,43,55,56,57,58,59,71,72,73,74,75,87,88,89,90,91,103,104,105,106,107 },
			new int[] {40,41,42,43,44,56,57,58,59,60,72,73,74,75,76,88,89,90,91,92,104,105,106,107,108 },
			new int[] {41,42,43,44,45,57,58,59,60,61,73,74,75,76,77,89,90,91,92,93,105,106,107,108,109 },
			new int[] {42,43,44,45,46,58,59,60,61,62,74,75,76,77,78,90,91,92,93,94,106,107,108,109,110 },
			new int[] {43,44,45,46,47,59,60,61,62,63,75,76,77,78,79,91,92,93,94,95,107,108,109,110,111 },
			new int[] {44,45,46,47,60,61,62,63,76,77,78,79,92,93,94,95,108,109,110,111 },
			new int[] {45,46,47,61,62,63,77,78,79,93,94,95,109,110,111 },
			new int[] {48,49,50,64,65,66,80,81,82,96,97,98,112,113,114 },
			new int[] {48,49,50,51,64,65,66,67,80,81,82,83,96,97,98,99,112,113,114,115 },
			new int[] {48,49,50,51,52,64,65,66,67,68,80,81,82,83,84,96,97,98,99,100,112,113,114,115,116 },
			new int[] {49,50,51,52,53,65,66,67,68,69,81,82,83,84,85,97,98,99,100,101,113,114,115,116,117 },
			new int[] {50,51,52,53,54,66,67,68,69,70,82,83,84,85,86,98,99,100,101,102,114,115,116,117,118 },
			new int[] {51,52,53,54,55,67,68,69,70,71,83,84,85,86,87,99,100,101,102,103,115,116,117,118,119 },
			new int[] {52,53,54,55,56,68,69,70,71,72,84,85,86,87,88,100,101,102,103,104,116,117,118,119,120 },
			new int[] {53,54,55,56,57,69,70,71,72,73,85,86,87,88,89,101,102,103,104,105,117,118,119,120,121 },
			new int[] {54,55,56,57,58,70,71,72,73,74,86,87,88,89,90,102,103,104,105,106,118,119,120,121,122 },
			new int[] {55,56,57,58,59,71,72,73,74,75,87,88,89,90,91,103,104,105,106,107,119,120,121,122,123 },
			new int[] {56,57,58,59,60,72,73,74,75,76,88,89,90,91,92,104,105,106,107,108,120,121,122,123,124 },
			new int[] {57,58,59,60,61,73,74,75,76,77,89,90,91,92,93,105,106,107,108,109,121,122,123,124,125 },
			new int[] {58,59,60,61,62,74,75,76,77,78,90,91,92,93,94,106,107,108,109,110,122,123,124,125,126 },
			new int[] {59,60,61,62,63,75,76,77,78,79,91,92,93,94,95,107,108,109,110,111,123,124,125,126,127 },
			new int[] {60,61,62,63,76,77,78,79,92,93,94,95,108,109,110,111,124,125,126,127 },
			new int[] {61,62,63,77,78,79,93,94,95,109,110,111,125,126,127 },
			new int[] {64,65,66,80,81,82,96,97,98,112,113,114,128,129,130 },
			new int[] {64,65,66,67,80,81,82,83,96,97,98,99,112,113,114,115,128,129,130,131 },
			new int[] {64,65,66,67,68,80,81,82,83,84,96,97,98,99,100,112,113,114,115,116,128,129,130,131,132 },
			new int[] {65,66,67,68,69,81,82,83,84,85,97,98,99,100,101,113,114,115,116,117,129,130,131,132,133 },
			new int[] {66,67,68,69,70,82,83,84,85,86,98,99,100,101,102,114,115,116,117,118,130,131,132,133,134 },
			new int[] {67,68,69,70,71,83,84,85,86,87,99,100,101,102,103,115,116,117,118,119,131,132,133,134,135 },
			new int[] {68,69,70,71,72,84,85,86,87,88,100,101,102,103,104,116,117,118,119,120,132,133,134,135,136 },
			new int[] {69,70,71,72,73,85,86,87,88,89,101,102,103,104,105,117,118,119,120,121,133,134,135,136,137 },
			new int[] {70,71,72,73,74,86,87,88,89,90,102,103,104,105,106,118,119,120,121,122,134,135,136,137,138 },
			new int[] {71,72,73,74,75,87,88,89,90,91,103,104,105,106,107,119,120,121,122,123,135,136,137,138,139 },
			new int[] {72,73,74,75,76,88,89,90,91,92,104,105,106,107,108,120,121,122,123,124,136,137,138,139,140 },
			new int[] {73,74,75,76,77,89,90,91,92,93,105,106,107,108,109,121,122,123,124,125,137,138,139,140,141 },
			new int[] {74,75,76,77,78,90,91,92,93,94,106,107,108,109,110,122,123,124,125,126,138,139,140,141,142 },
			new int[] {75,76,77,78,79,91,92,93,94,95,107,108,109,110,111,123,124,125,126,127,139,140,141,142,143 },
			new int[] {76,77,78,79,92,93,94,95,108,109,110,111,124,125,126,127,140,141,142,143 },
			new int[] {77,78,79,93,94,95,109,110,111,125,126,127,141,142,143 },
			new int[] {80,81,82,96,97,98,112,113,114,128,129,130,144,145,146 },
			new int[] {80,81,82,83,96,97,98,99,112,113,114,115,128,129,130,131,144,145,146,147 },
			new int[] {80,81,82,83,84,96,97,98,99,100,112,113,114,115,116,128,129,130,131,132,144,145,146,147,148 },
			new int[] {81,82,83,84,85,97,98,99,100,101,113,114,115,116,117,129,130,131,132,133,145,146,147,148,149 },
			new int[] {82,83,84,85,86,98,99,100,101,102,114,115,116,117,118,130,131,132,133,134,146,147,148,149,150 },
			new int[] {83,84,85,86,87,99,100,101,102,103,115,116,117,118,119,131,132,133,134,135,147,148,149,150,151 },
			new int[] {84,85,86,87,88,100,101,102,103,104,116,117,118,119,120,132,133,134,135,136,148,149,150,151,152 },
			new int[] {85,86,87,88,89,101,102,103,104,105,117,118,119,120,121,133,134,135,136,137,149,150,151,152,153 },
			new int[] {86,87,88,89,90,102,103,104,105,106,118,119,120,121,122,134,135,136,137,138,150,151,152,153,154 },
			new int[] {87,88,89,90,91,103,104,105,106,107,119,120,121,122,123,135,136,137,138,139,151,152,153,154,155 },
			new int[] {88,89,90,91,92,104,105,106,107,108,120,121,122,123,124,136,137,138,139,140,152,153,154,155,156 },
			new int[] {89,90,91,92,93,105,106,107,108,109,121,122,123,124,125,137,138,139,140,141,153,154,155,156,157 },
			new int[] {90,91,92,93,94,106,107,108,109,110,122,123,124,125,126,138,139,140,141,142,154,155,156,157,158 },
			new int[] {91,92,93,94,95,107,108,109,110,111,123,124,125,126,127,139,140,141,142,143,155,156,157,158,159 },
			new int[] {92,93,94,95,108,109,110,111,124,125,126,127,140,141,142,143,156,157,158,159 },
			new int[] {93,94,95,109,110,111,125,126,127,141,142,143,157,158,159 },
			new int[] {96,97,98,112,113,114,128,129,130,144,145,146,160,161,162 },
			new int[] {96,97,98,99,112,113,114,115,128,129,130,131,144,145,146,147,160,161,162,163 },
			new int[] {96,97,98,99,100,112,113,114,115,116,128,129,130,131,132,144,145,146,147,148,160,161,162,163,164 },
			new int[] {97,98,99,100,101,113,114,115,116,117,129,130,131,132,133,145,146,147,148,149,161,162,163,164,165 },
			new int[] {98,99,100,101,102,114,115,116,117,118,130,131,132,133,134,146,147,148,149,150,162,163,164,165,166 },
			new int[] {99,100,101,102,103,115,116,117,118,119,131,132,133,134,135,147,148,149,150,151,163,164,165,166,167 },
			new int[] {100,101,102,103,104,116,117,118,119,120,132,133,134,135,136,148,149,150,151,152,164,165,166,167,168 },
			new int[] {101,102,103,104,105,117,118,119,120,121,133,134,135,136,137,149,150,151,152,153,165,166,167,168,169 },
			new int[] {102,103,104,105,106,118,119,120,121,122,134,135,136,137,138,150,151,152,153,154,166,167,168,169,170 },
			new int[] {103,104,105,106,107,119,120,121,122,123,135,136,137,138,139,151,152,153,154,155,167,168,169,170,171 },
			new int[] {104,105,106,107,108,120,121,122,123,124,136,137,138,139,140,152,153,154,155,156,168,169,170,171,172 },
			new int[] {105,106,107,108,109,121,122,123,124,125,137,138,139,140,141,153,154,155,156,157,169,170,171,172,173 },
			new int[] {106,107,108,109,110,122,123,124,125,126,138,139,140,141,142,154,155,156,157,158,170,171,172,173,174 },
			new int[] {107,108,109,110,111,123,124,125,126,127,139,140,141,142,143,155,156,157,158,159,171,172,173,174,175 },
			new int[] {108,109,110,111,124,125,126,127,140,141,142,143,156,157,158,159,172,173,174,175 },
			new int[] {109,110,111,125,126,127,141,142,143,157,158,159,173,174,175 },
			new int[] {112,113,114,128,129,130,144,145,146,160,161,162,176,177,178 },
			new int[] {112,113,114,115,128,129,130,131,144,145,146,147,160,161,162,163,176,177,178,179 },
			new int[] {112,113,114,115,116,128,129,130,131,132,144,145,146,147,148,160,161,162,163,164,176,177,178,179,180 },
			new int[] {113,114,115,116,117,129,130,131,132,133,145,146,147,148,149,161,162,163,164,165,177,178,179,180,181 },
			new int[] {114,115,116,117,118,130,131,132,133,134,146,147,148,149,150,162,163,164,165,166,178,179,180,181,182 },
			new int[] {115,116,117,118,119,131,132,133,134,135,147,148,149,150,151,163,164,165,166,167,179,180,181,182,183 },
			new int[] {116,117,118,119,120,132,133,134,135,136,148,149,150,151,152,164,165,166,167,168,180,181,182,183,184 },
			new int[] {117,118,119,120,121,133,134,135,136,137,149,150,151,152,153,165,166,167,168,169,181,182,183,184,185 },
			new int[] {118,119,120,121,122,134,135,136,137,138,150,151,152,153,154,166,167,168,169,170,182,183,184,185,186 },
			new int[] {119,120,121,122,123,135,136,137,138,139,151,152,153,154,155,167,168,169,170,171,183,184,185,186,187 },
			new int[] {120,121,122,123,124,136,137,138,139,140,152,153,154,155,156,168,169,170,171,172,184,185,186,187,188 },
			new int[] {121,122,123,124,125,137,138,139,140,141,153,154,155,156,157,169,170,171,172,173,185,186,187,188,189 },
			new int[] {122,123,124,125,126,138,139,140,141,142,154,155,156,157,158,170,171,172,173,174,186,187,188,189,190 },
			new int[] {123,124,125,126,127,139,140,141,142,143,155,156,157,158,159,171,172,173,174,175,187,188,189,190,191 },
			new int[] {124,125,126,127,140,141,142,143,156,157,158,159,172,173,174,175,188,189,190,191 },
			new int[] {125,126,127,141,142,143,157,158,159,173,174,175,189,190,191 },
			new int[] {128,129,130,144,145,146,160,161,162,176,177,178,192,193,194 },
			new int[] {128,129,130,131,144,145,146,147,160,161,162,163,176,177,178,179,192,193,194,195 },
			new int[] {128,129,130,131,132,144,145,146,147,148,160,161,162,163,164,176,177,178,179,180,192,193,194,195,196 },
			new int[] {129,130,131,132,133,145,146,147,148,149,161,162,163,164,165,177,178,179,180,181,193,194,195,196,197 },
			new int[] {130,131,132,133,134,146,147,148,149,150,162,163,164,165,166,178,179,180,181,182,194,195,196,197,198 },
			new int[] {131,132,133,134,135,147,148,149,150,151,163,164,165,166,167,179,180,181,182,183,195,196,197,198,199 },
			new int[] {132,133,134,135,136,148,149,150,151,152,164,165,166,167,168,180,181,182,183,184,196,197,198,199,200 },
			new int[] {133,134,135,136,137,149,150,151,152,153,165,166,167,168,169,181,182,183,184,185,197,198,199,200,201 },
			new int[] {134,135,136,137,138,150,151,152,153,154,166,167,168,169,170,182,183,184,185,186,198,199,200,201,202 },
			new int[] {135,136,137,138,139,151,152,153,154,155,167,168,169,170,171,183,184,185,186,187,199,200,201,202,203 },
			new int[] {136,137,138,139,140,152,153,154,155,156,168,169,170,171,172,184,185,186,187,188,200,201,202,203,204 },
			new int[] {137,138,139,140,141,153,154,155,156,157,169,170,171,172,173,185,186,187,188,189,201,202,203,204,205 },
			new int[] {138,139,140,141,142,154,155,156,157,158,170,171,172,173,174,186,187,188,189,190,202,203,204,205,206 },
			new int[] {139,140,141,142,143,155,156,157,158,159,171,172,173,174,175,187,188,189,190,191,203,204,205,206,207 },
			new int[] {140,141,142,143,156,157,158,159,172,173,174,175,188,189,190,191,204,205,206,207 },
			new int[] {141,142,143,157,158,159,173,174,175,189,190,191,205,206,207 },
			new int[] {144,145,146,160,161,162,176,177,178,192,193,194,208,209,210 },
			new int[] {144,145,146,147,160,161,162,163,176,177,178,179,192,193,194,195,208,209,210,211 },
			new int[] {144,145,146,147,148,160,161,162,163,164,176,177,178,179,180,192,193,194,195,196,208,209,210,211,212 },
			new int[] {145,146,147,148,149,161,162,163,164,165,177,178,179,180,181,193,194,195,196,197,209,210,211,212,213 },
			new int[] {146,147,148,149,150,162,163,164,165,166,178,179,180,181,182,194,195,196,197,198,210,211,212,213,214 },
			new int[] {147,148,149,150,151,163,164,165,166,167,179,180,181,182,183,195,196,197,198,199,211,212,213,214,215 },
			new int[] {148,149,150,151,152,164,165,166,167,168,180,181,182,183,184,196,197,198,199,200,212,213,214,215,216 },
			new int[] {149,150,151,152,153,165,166,167,168,169,181,182,183,184,185,197,198,199,200,201,213,214,215,216,217 },
			new int[] {150,151,152,153,154,166,167,168,169,170,182,183,184,185,186,198,199,200,201,202,214,215,216,217,218 },
			new int[] {151,152,153,154,155,167,168,169,170,171,183,184,185,186,187,199,200,201,202,203,215,216,217,218,219 },
			new int[] {152,153,154,155,156,168,169,170,171,172,184,185,186,187,188,200,201,202,203,204,216,217,218,219,220 },
			new int[] {153,154,155,156,157,169,170,171,172,173,185,186,187,188,189,201,202,203,204,205,217,218,219,220,221 },
			new int[] {154,155,156,157,158,170,171,172,173,174,186,187,188,189,190,202,203,204,205,206,218,219,220,221,222 },
			new int[] {155,156,157,158,159,171,172,173,174,175,187,188,189,190,191,203,204,205,206,207,219,220,221,222,223 },
			new int[] {156,157,158,159,172,173,174,175,188,189,190,191,204,205,206,207,220,221,222,223 },
			new int[] {157,158,159,173,174,175,189,190,191,205,206,207,221,222,223 },
			new int[] {160,161,162,176,177,178,192,193,194,208,209,210,224,225,226 },
			new int[] {160,161,162,163,176,177,178,179,192,193,194,195,208,209,210,211,224,225,226,227 },
			new int[] {160,161,162,163,164,176,177,178,179,180,192,193,194,195,196,208,209,210,211,212,224,225,226,227,228 },
			new int[] {161,162,163,164,165,177,178,179,180,181,193,194,195,196,197,209,210,211,212,213,225,226,227,228,229 },
			new int[] {162,163,164,165,166,178,179,180,181,182,194,195,196,197,198,210,211,212,213,214,226,227,228,229,230 },
			new int[] {163,164,165,166,167,179,180,181,182,183,195,196,197,198,199,211,212,213,214,215,227,228,229,230,231 },
			new int[] {164,165,166,167,168,180,181,182,183,184,196,197,198,199,200,212,213,214,215,216,228,229,230,231,232 },
			new int[] {165,166,167,168,169,181,182,183,184,185,197,198,199,200,201,213,214,215,216,217,229,230,231,232,233 },
			new int[] {166,167,168,169,170,182,183,184,185,186,198,199,200,201,202,214,215,216,217,218,230,231,232,233,234 },
			new int[] {167,168,169,170,171,183,184,185,186,187,199,200,201,202,203,215,216,217,218,219,231,232,233,234,235 },
			new int[] {168,169,170,171,172,184,185,186,187,188,200,201,202,203,204,216,217,218,219,220,232,233,234,235,236 },
			new int[] {169,170,171,172,173,185,186,187,188,189,201,202,203,204,205,217,218,219,220,221,233,234,235,236,237 },
			new int[] {170,171,172,173,174,186,187,188,189,190,202,203,204,205,206,218,219,220,221,222,234,235,236,237,238 },
			new int[] {171,172,173,174,175,187,188,189,190,191,203,204,205,206,207,219,220,221,222,223,235,236,237,238,239 },
			new int[] {172,173,174,175,188,189,190,191,204,205,206,207,220,221,222,223,236,237,238,239 },
			new int[] {173,174,175,189,190,191,205,206,207,221,222,223,237,238,239 },
			new int[] {176,177,178,192,193,194,208,209,210,224,225,226,240,241,242 },
			new int[] {176,177,178,179,192,193,194,195,208,209,210,211,224,225,226,227,240,241,242,243 },
			new int[] {176,177,178,179,180,192,193,194,195,196,208,209,210,211,212,224,225,226,227,228,240,241,242,243,244 },
			new int[] {177,178,179,180,181,193,194,195,196,197,209,210,211,212,213,225,226,227,228,229,241,242,243,244,245 },
			new int[] {178,179,180,181,182,194,195,196,197,198,210,211,212,213,214,226,227,228,229,230,242,243,244,245,246 },
			new int[] {179,180,181,182,183,195,196,197,198,199,211,212,213,214,215,227,228,229,230,231,243,244,245,246,247 },
			new int[] {180,181,182,183,184,196,197,198,199,200,212,213,214,215,216,228,229,230,231,232,244,245,246,247,248 },
			new int[] {181,182,183,184,185,197,198,199,200,201,213,214,215,216,217,229,230,231,232,233,245,246,247,248,249 },
			new int[] {182,183,184,185,186,198,199,200,201,202,214,215,216,217,218,230,231,232,233,234,246,247,248,249,250 },
			new int[] {183,184,185,186,187,199,200,201,202,203,215,216,217,218,219,231,232,233,234,235,247,248,249,250,251 },
			new int[] {184,185,186,187,188,200,201,202,203,204,216,217,218,219,220,232,233,234,235,236,248,249,250,251,252 },
			new int[] {185,186,187,188,189,201,202,203,204,205,217,218,219,220,221,233,234,235,236,237,249,250,251,252,253 },
			new int[] {186,187,188,189,190,202,203,204,205,206,218,219,220,221,222,234,235,236,237,238,250,251,252,253,254 },
			new int[] {187,188,189,190,191,203,204,205,206,207,219,220,221,222,223,235,236,237,238,239,251,252,253,254,255 },
			new int[] {188,189,190,191,204,205,206,207,220,221,222,223,236,237,238,239,252,253,254,255 },
			new int[] {189,190,191,205,206,207,221,222,223,237,238,239,253,254,255 },
			new int[] {192,193,194,208,209,210,224,225,226,240,241,242,256,257,258 },
			new int[] {192,193,194,195,208,209,210,211,224,225,226,227,240,241,242,243,256,257,258,259 },
			new int[] {192,193,194,195,196,208,209,210,211,212,224,225,226,227,228,240,241,242,243,244,256,257,258,259,260 },
			new int[] {193,194,195,196,197,209,210,211,212,213,225,226,227,228,229,241,242,243,244,245,257,258,259,260,261 },
			new int[] {194,195,196,197,198,210,211,212,213,214,226,227,228,229,230,242,243,244,245,246,258,259,260,261,262 },
			new int[] {195,196,197,198,199,211,212,213,214,215,227,228,229,230,231,243,244,245,246,247,259,260,261,262,263 },
			new int[] {196,197,198,199,200,212,213,214,215,216,228,229,230,231,232,244,245,246,247,248,260,261,262,263,264 },
			new int[] {197,198,199,200,201,213,214,215,216,217,229,230,231,232,233,245,246,247,248,249,261,262,263,264,265 },
			new int[] {198,199,200,201,202,214,215,216,217,218,230,231,232,233,234,246,247,248,249,250,262,263,264,265,266 },
			new int[] {199,200,201,202,203,215,216,217,218,219,231,232,233,234,235,247,248,249,250,251,263,264,265,266,267 },
			new int[] {200,201,202,203,204,216,217,218,219,220,232,233,234,235,236,248,249,250,251,252,264,265,266,267,268 },
			new int[] {201,202,203,204,205,217,218,219,220,221,233,234,235,236,237,249,250,251,252,253,265,266,267,268,269 },
			new int[] {202,203,204,205,206,218,219,220,221,222,234,235,236,237,238,250,251,252,253,254,266,267,268,269,270 },
			new int[] {203,204,205,206,207,219,220,221,222,223,235,236,237,238,239,251,252,253,254,255,267,268,269,270,271 },
			new int[] {204,205,206,207,220,221,222,223,236,237,238,239,252,253,254,255,268,269,270,271 },
			new int[] {205,206,207,221,222,223,237,238,239,253,254,255,269,270,271 },
			new int[] {208,209,210,224,225,226,240,241,242,256,257,258,272,273,274 },
			new int[] {208,209,210,211,224,225,226,227,240,241,242,243,256,257,258,259,272,273,274,275 },
			new int[] {208,209,210,211,212,224,225,226,227,228,240,241,242,243,244,256,257,258,259,260,272,273,274,275,276 },
			new int[] {209,210,211,212,213,225,226,227,228,229,241,242,243,244,245,257,258,259,260,261,273,274,275,276,277 },
			new int[] {210,211,212,213,214,226,227,228,229,230,242,243,244,245,246,258,259,260,261,262,274,275,276,277,278 },
			new int[] {211,212,213,214,215,227,228,229,230,231,243,244,245,246,247,259,260,261,262,263,275,276,277,278,279 },
			new int[] {212,213,214,215,216,228,229,230,231,232,244,245,246,247,248,260,261,262,263,264,276,277,278,279,280 },
			new int[] {213,214,215,216,217,229,230,231,232,233,245,246,247,248,249,261,262,263,264,265,277,278,279,280,281 },
			new int[] {214,215,216,217,218,230,231,232,233,234,246,247,248,249,250,262,263,264,265,266,278,279,280,281,282 },
			new int[] {215,216,217,218,219,231,232,233,234,235,247,248,249,250,251,263,264,265,266,267,279,280,281,282,283 },
			new int[] {216,217,218,219,220,232,233,234,235,236,248,249,250,251,252,264,265,266,267,268,280,281,282,283,284 },
			new int[] {217,218,219,220,221,233,234,235,236,237,249,250,251,252,253,265,266,267,268,269,281,282,283,284,285 },
			new int[] {218,219,220,221,222,234,235,236,237,238,250,251,252,253,254,266,267,268,269,270,282,283,284,285,286 },
			new int[] {219,220,221,222,223,235,236,237,238,239,251,252,253,254,255,267,268,269,270,271,283,284,285,286,287 },
			new int[] {220,221,222,223,236,237,238,239,252,253,254,255,268,269,270,271,284,285,286,287 },
			new int[] {221,222,223,237,238,239,253,254,255,269,270,271,285,286,287 },
			new int[] {224,225,226,240,241,242,256,257,258,272,273,274 },
			new int[] {224,225,226,227,240,241,242,243,256,257,258,259,272,273,274,275 },
			new int[] {224,225,226,227,228,240,241,242,243,244,256,257,258,259,260,272,273,274,275,276 },
			new int[] {225,226,227,228,229,241,242,243,244,245,257,258,259,260,261,273,274,275,276,277 },
			new int[] {226,227,228,229,230,242,243,244,245,246,258,259,260,261,262,274,275,276,277,278 },
			new int[] {227,228,229,230,231,243,244,245,246,247,259,260,261,262,263,275,276,277,278,279 },
			new int[] {228,229,230,231,232,244,245,246,247,248,260,261,262,263,264,276,277,278,279,280 },
			new int[] {229,230,231,232,233,245,246,247,248,249,261,262,263,264,265,277,278,279,280,281 },
			new int[] {230,231,232,233,234,246,247,248,249,250,262,263,264,265,266,278,279,280,281,282 },
			new int[] {231,232,233,234,235,247,248,249,250,251,263,264,265,266,267,279,280,281,282,283 },
			new int[] {232,233,234,235,236,248,249,250,251,252,264,265,266,267,268,280,281,282,283,284 },
			new int[] {233,234,235,236,237,249,250,251,252,253,265,266,267,268,269,281,282,283,284,285 },
			new int[] {234,235,236,237,238,250,251,252,253,254,266,267,268,269,270,282,283,284,285,286 },
			new int[] {235,236,237,238,239,251,252,253,254,255,267,268,269,270,271,283,284,285,286,287 },
			new int[] {236,237,238,239,252,253,254,255,268,269,270,271,284,285,286,287 },
			new int[] {237,238,239,253,254,255,269,270,271,285,286,287 },
			new int[] {240,241,242,256,257,258,272,273,274 },
			new int[] {240,241,242,243,256,257,258,259,272,273,274,275 },
			new int[] {240,241,242,243,244,256,257,258,259,260,272,273,274,275,276 },
			new int[] {241,242,243,244,245,257,258,259,260,261,273,274,275,276,277 },
			new int[] {242,243,244,245,246,258,259,260,261,262,274,275,276,277,278 },
			new int[] {243,244,245,246,247,259,260,261,262,263,275,276,277,278,279 },
			new int[] {244,245,246,247,248,260,261,262,263,264,276,277,278,279,280 },
			new int[] {245,246,247,248,249,261,262,263,264,265,277,278,279,280,281 },
			new int[] {246,247,248,249,250,262,263,264,265,266,278,279,280,281,282 },
			new int[] {247,248,249,250,251,263,264,265,266,267,279,280,281,282,283 },
			new int[] {248,249,250,251,252,264,265,266,267,268,280,281,282,283,284 },
			new int[] {249,250,251,252,253,265,266,267,268,269,281,282,283,284,285 },
			new int[] {250,251,252,253,254,266,267,268,269,270,282,283,284,285,286 },
			new int[] {251,252,253,254,255,267,268,269,270,271,283,284,285,286,287 },
			new int[] {252,253,254,255,268,269,270,271,284,285,286,287 },
			new int[] {253,254,255,269,270,271,285,286,287 },
		};

		#endregion

	}
}
