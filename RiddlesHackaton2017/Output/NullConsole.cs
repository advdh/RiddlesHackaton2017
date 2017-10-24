namespace RiddlesHackaton2017.Output
{
	public class NullConsole : IConsole
	{
		public void Dispose()
		{
			//Do nothing
		}

		public void WriteLine(string format, params object[] arg)
		{
			//Do nothing
		}

		public void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			//Do nothing
		}
	}
}
