﻿using RiddlesHackaton2017.Moves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiddlesHackaton2017.Models
{
	public class Board
	{
		public const int Width = 18;
		public const int Height = 16;
		public const int Size = Width * Height;
		public const int MaxRounds = 100;

		#region Properties

		public int Round { get; set; }

		public short[] Field = new short[Size];

		/// <summary>Me</summary>
		public Player MyPlayer { get; set; }

		/// <summary>Opponent</summary>
		public Player OpponentPlayer { get { return MyPlayer.Opponent(); } }

		public IEnumerable<Move> GetFeasibleMovesForPlayer(Player player)
		{
			var result = new List<Move>();

			//Pass move
			result.Add(new PassMove());

			//Kill moves
			for (int i = 0; i < Size; i++)
			{
				if (Field[i] != 0)
				{
					result.Add(new KillMove(i));
				}
			}

			//Birth moves
			var myFields = Enumerable.Range(0, Size).Where(i => Field[i] == (short)MyPlayer);
			//var opponentFields = Enumerable.Range(0, Size).Where(i => Field[i] == (short)OpponentPlayer);
			var emptyFields = Enumerable.Range(0, Size).Where(i => Field[i] == 0);

			foreach(int b in emptyFields)
			{
				foreach(int s1 in myFields)
				{
					foreach(int s2 in myFields.Except(new[] { s1 }))
					{
						result.Add(new BirthMove(b, s1, s2));
					}
				}
			}
			return result;
		}

		#endregion

		#region Constructors and static Creaate methods

		public Board()
		{
			MyPlayer = Player.Player1;
		}

		/// <summary>Copy constructor</summary>
		public Board(Board board)
		{
			for (int i = 0; i < Size; i++)
			{
				Field[i] = board.Field[i];
			}
			MyPlayer = board.MyPlayer;
			Round = board.Round;
		}

		public static Board CopyAndPlay(Board board, Player player, Move move)
		{
			//Apply move
			var newBoard = move.Apply(board, player);

			//Apply next generation
			newBoard = NextGeneration(newBoard);

			//Increment round
			if (player == Player.Player2)
			{
				newBoard.Round = board.Round + 1;
			}

			return newBoard;
		}

		public static Board NextGeneration(Board board)
		{
			var newBoard = new Board() { MyPlayer = board.MyPlayer };

			for(int i = 0; i < Size; i++)
			{
				newBoard.Field[i] = NextGenerationForField(board, i);
			}

			return newBoard;
		}

		private static short NextGenerationForField(Board board, int i)
		{
			int count = 0;
			int count1 = 0;
			foreach (int j in NeighbourFields[i])
			{
				if (board.Field[j] != 0)
				{
					count++;
					if (board.Field[j] == 1)
					{
						count1++;
					}
				}
			}
			if (board.Field[i] != 0)
			{
				//Current cell is living
				switch (count)
				{
					case 0:
					case 1:
						//Die
						break;
					case 2:
					case 3:
						//Live on
						return board.Field[i];
					default:
						//Die
						break;
				}
			}
			else
			{
				//Current cell dead
				if (count == 3)
				{
					if (count1 >= 2)
					{
						//Player1 born
						return 1;
					}
					else
					{
						//Player2 born
						return 2;
					}
				}
			}
			return 0;
		}

		#endregion

		#region Helper methods

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
				return copy;
			}
		}

		/// <remarks>TODO: cache</remarks>
		public IEnumerable<int> MyCells { get { return Enumerable.Range(0, Size).Where(i => Field[i] == (short)MyPlayer); } }

		/// <remarks>TODO: cache</remarks>
		public IEnumerable<int> GetCells(Player player)
		{
			return Enumerable.Range(0, Size).Where(i => Field[i] == (short)player);
		}

		/// <remarks>TODO: cache</remarks>
		public IEnumerable<int> OpponentCells { get { return Enumerable.Range(0, Size).Where(i => Field[i] == (short)OpponentPlayer); } }
		/// <remarks>TODO: cache</remarks>
		public IEnumerable<int> EmptyCells { get { return Enumerable.Range(0, Size).Where(i => Field[i] == 0); } }

		public override string ToString()
		{
			return string.Format("Round {0}, {3}: my count: {1}; his count: {2}", 
				Round, MyCells.Count(), OpponentCells.Count(), MyPlayer);
		}

		public string BoardString()
		{
			var result = new StringBuilder();
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
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
			var result = new StringBuilder();
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
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

		#endregion

	}
}
