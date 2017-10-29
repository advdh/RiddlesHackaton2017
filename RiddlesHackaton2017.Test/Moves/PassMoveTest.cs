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

		[TestMethod]
		public void TryParse_Parsable_Test()
		{
			Move move;
			Assert.IsTrue(PassMove.TryParse("pass", out move));
			Assert.AreEqual(new PassMove(), move);
		}

		[TestMethod]
		public void TryParse_NotParsable_Test()
		{
			Move move;
			Assert.IsFalse(PassMove.TryParse("foo", out move));
			Assert.AreEqual(new NullMove(), move);
		}

	}
}
