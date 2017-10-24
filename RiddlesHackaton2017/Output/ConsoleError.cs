using System;

namespace RiddlesHackaton2017.Output
{
	public class ConsoleError : IConsole
	{
		public void Dispose()
		{
			//Nothing do dispose
		}

		public void WriteLine(string format, params object[] arg)
		{
			Console.Error.WriteLine(format, arg);
		}

		public void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			Console.Error.WriteLine(format, arg0, arg1, arg2);
		}
	}
}
