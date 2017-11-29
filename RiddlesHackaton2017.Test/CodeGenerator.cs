using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiddlesHackaton2017.Test
{
	[TestClass]
	public class CodeGenerator
	{
		[TestMethod]
		public void GenerateNeighbours()
		{
			var sb = new StringBuilder();
			sb.AppendLine("public static int[][] NeighbourFields = new[]");
			sb.AppendLine("{");

			for (int i = 0; i < Board.Size; i++)
			{
				sb.Append("new int[] {");
				var position = new Position(i);
				bool first = true;
				foreach (var pair in RelativeNeighbours)
				{
					if (position.X + pair[0] >= 0
						&& position.X + pair[0] < Board.Width
						&& position.Y + pair[1] >= 0
						&& position.Y + pair[1] < Board.Height)
					{
						if (!first) sb.Append(",");
						sb.Append(new Position(position.X + pair[0], position.Y + pair[1]).Index);
						first = false;
					}
				}
				sb.AppendLine(" },");
			}
			sb.AppendLine("};");
			Console.WriteLine(sb.ToString());
		}

		[TestMethod]
		public void GenerateNeighboursAndThis()
		{
			var sb = new StringBuilder();
			sb.AppendLine("public static int[][] NeighbourFieldsAndThis = new[]");
			sb.AppendLine("{");

			for (int i = 0; i < Board.Size; i++)
			{
				sb.Append("new int[] {");
				var position = new Position(i);
				sb.Append(new Position(position.X, position.Y).Index);
				foreach (var pair in RelativeNeighbours)
				{
					if (position.X + pair[0] >= 0
						&& position.X + pair[0] < Board.Width
						&& position.Y + pair[1] >= 0
						&& position.Y + pair[1] < Board.Height)
					{
						sb.Append(",");
						sb.Append(new Position(position.X + pair[0], position.Y + pair[1]).Index);
					}
				}
				sb.AppendLine(" },");
			}
			sb.AppendLine("};");
			Console.WriteLine(sb.ToString());
		}

		[TestMethod]
		public void GenerateNeighbourNeighbours()
		{
			var sb = new StringBuilder();
			sb.AppendLine("public static int[][] NeighbourFields2 = new[]");
			sb.AppendLine("{");

			for (int i = 0; i < Board.Size; i++)
			{
				sb.Append("new int[] {");
				var neighbours2 = Board.NeighbourFields[i].SelectMany(j => Board.NeighbourFields[j]).Distinct().OrderBy(j => j);
				bool first = true;
				foreach (int j in neighbours2)
				{
					if (!first) sb.Append(",");
					sb.Append(j);
					first = false;
				}
				sb.AppendLine(" },");
			}
			sb.AppendLine("};");
			Console.WriteLine(sb.ToString());
		}


		[TestMethod]
		public void GenerateFieldBonus()
		{
			var sb = new StringBuilder();
			sb.AppendLine("public static int[] FieldBonus = new[]");
			sb.AppendLine("{");

			for (int y = 0; y < Board.Height; y++)
			{
				for (int x = 0; x < Board.Width; x++)
				{
					sb.Append($"{Math.Min(x, Board.Width - 1 - x) + Math.Min(y, Board.Height - 1 - y)}, ");
				}
				sb.AppendLine();
			}
			sb.AppendLine("};");
			Console.WriteLine(sb.ToString());
		}

		private int[][] RelativeNeighbours = new[]
		{
			new int[] {-1, -1 },
			new int[] {-1, 1 },
			new int[] {0, -1 },
			new int[] {0, 1 },
			new int[] {1, -1 },
			new int[] {1, 1 },
			new int[] {-1, 0 },
			new int[] {1, 0 },
		};
	}
}
