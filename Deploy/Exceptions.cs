using System;

namespace RiddlesHackaton2017
{
	public class GameOfLifeException : ApplicationException
	{
		public GameOfLifeException(string message) : base(message) { }
		public GameOfLifeException(string message, params object[] args) : base(string.Format(message, args)) { }
	}

	public class InvalidMoveException : GameOfLifeException
	{
		public InvalidMoveException(string message) : base(message) { }
		public InvalidMoveException(string message, params object[] args) : base(message, args) { }
	}

	public class InvalidKillMoveException : InvalidMoveException
	{
		public InvalidKillMoveException(string message) : base(message) { }
		public InvalidKillMoveException(string message, params object[] args) : base(message, args) { }
	}

	public class InvalidBirthMoveException : InvalidMoveException
	{
		public InvalidBirthMoveException(string message) : base(message) { }
		public InvalidBirthMoveException(string message, params object[] args) : base(message, args) { }
	}
}
