using RiddlesHackaton2017.Models;
using System;
using System.Diagnostics;

namespace RiddlesHackaton2017.Moves
{
	//[DebuggerDisplay("{DebuggerDisplay}")]
	public class PassMove : Move
	{
		public override Board Apply(Board board, Player player)
		{
			throw new NotImplementedException();
		}

		public override string ToOutputString()
		{
			return "pass";
		}

		public override string ToString()
		{
			return "PassMove";
		}

		//private string DebuggerDisplay
		//{
		//	get
		//	{
		//		return "PassMove";
		//	}
		//}
	}
}
