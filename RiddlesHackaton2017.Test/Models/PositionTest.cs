using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiddlesHackaton2017.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiddlesHackaton2017.Test.Models
{
	[TestClass]
	public class PositionTest
	{
		[TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void X_Test()
		{
			new Position(-1, 0);
		}

		[TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void X_Test2()
		{
			new Position(18, 0);
		}

		[TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Y_Test()
		{
			new Position(0, -1);
		}

		[TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Y_Test2()
		{
			new Position(0, 16);
		}

		[TestMethod]
		public void Index_Test()
		{
			Assert.AreEqual(0, new Position(0, 0).Index);
			Assert.AreEqual(1, new Position(0, 1).Index);
			Assert.AreEqual(15, new Position(0, 15).Index);
			Assert.AreEqual(16, new Position(1, 0).Index);
			Assert.AreEqual(287, new Position(17, 15).Index);
		}
	}
}
