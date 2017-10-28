using System.IO;

namespace RiddlesHackaton2017.Output
{
	public class FileConsole : IConsole
	{
		private readonly Stream stream;
		private readonly StreamWriter writer;

		public FileConsole(string filename, FileMode fileMode = FileMode.Create)
		{
			stream = new FileStream(filename, fileMode);
			writer = new StreamWriter(stream);
		}

		public void WriteLine(string format, params object[] arg)
		{
			writer.WriteLine(format, arg);
		}

		public void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			writer.WriteLine(format, arg0, arg1, arg2);
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					writer.Dispose();
					stream.Dispose();
				}

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
