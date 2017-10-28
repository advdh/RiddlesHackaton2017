using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Moves;

namespace RiddlesHackaton2017.Test.Moves
{
	[TestClass]
	public class PassMoveTest
	{
		[TestMethod]
		public void OutputString_Test()
		{
			Assert.AreEqual("pass", new PassMove().ToOutputString());
		}
	}
}
