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
