using System;

namespace RiddlesHackaton2017
{
	public static class Guard
	{
		public static T NotNull<T>(T value, string paramName)
		{
			if (value == null) throw new ArgumentNullException(paramName);
			return value;
		}
	}
}
