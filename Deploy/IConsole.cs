using System;

namespace RiddlesHackaton2017.Output
{
	public interface IConsole : IDisposable
	{
		/// <summary> Summary:
		/// Writes the text representation of the specified objects, followed by the current
		/// line terminator, to the standard output stream using the specified format information.
		/// </summary>
		void WriteLine(string format, object arg0, object arg1, object arg2);

		/// <summary> Summary:
		/// Writes the text representation of the specified objects, followed by the current
		/// line terminator, to the standard output stream using the specified format information.
		/// </summary>
		void WriteLine(string format, params object[] arg);
	}
}
