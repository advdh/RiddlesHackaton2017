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
	public class BotParserTest
	{
		[TestMethod]
		public void Parse_Test()
		{
			var s = "update game board .,.,.,1,.,0,.,.,.,.,.,0,.,1,.,.,1,.,.,.,.,.,.,.,0,.,.,1,1,.,.,.,1,1,.,.,0,.,.,.,0,.,.,0,1,0,1,.,.,.,1,.,.,.,1,.,.,.,.,1,.,0,.,.,1,.,0,.,1,.,1,1,.,1,.,1,.,.,0,1,.,0,.,0,.,0,.,.,.,1,1,.,.,.,.,0,.,.,1,.,.,.,.,.,0,.,0,1,0,.,.,.,.,.,.,1,.,1,0,.,1,0,.,.,.,.,.,0,.,.,.,.,.,.,1,.,.,.,.,.,1,.,0,1,0,1,.,0,.,.,.,.,.,0,.,.,.,.,.,.,1,.,.,.,.,.,1,0,.,1,0,.,0,.,.,.,.,.,.,1,0,1,.,1,.,.,.,.,.,0,.,.,1,.,.,.,.,0,0,.,.,.,1,.,1,.,1,.,0,1,.,.,0,.,0,.,0,0,.,0,.,1,.,0,.,.,1,.,0,.,.,.,.,0,.,.,.,0,.,.,.,0,1,0,1,.,.,1,.,.,.,1,.,.,0,0,.,.,.,0,0,.,.,1,.,.,.,.,.,.,.,0,.,.,0,.,1,.,.,.,.,.,1,.,0,.,.,.";
			var words = s.Split(' ');
			var board = BotParser.ParseBoard(words, Player.Player1, 0);

			Assert.AreEqual(0, board.Field[new Position(0, 0).Index]);
			Assert.AreEqual(2, board.Field[new Position(3, 0).Index]);
			Assert.AreEqual(1, board.Field[new Position(5, 0).Index]);
			Assert.AreEqual(1, board.Field[new Position(17, 12).Index]);
			Assert.AreEqual(0, board.Field[new Position(17, 15).Index]);
		}
	}
}
