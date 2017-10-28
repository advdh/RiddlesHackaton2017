namespace RiddlesHackaton2017.Output
{
	public class TheConsole : IConsole
	{
		public void WriteLine(string format, params object[] arg)
		{
			System.Console.WriteLine(format, arg);
		}

		public void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			System.Console.WriteLine(format, arg0, arg1, arg2);
		}

		public void Dispose()
		{
			//Nothing to dispose
		}
	}
}
