using System.Runtime.CompilerServices;

namespace RiddlesHackaton2017.Models
{
	public enum Player
	{
		Player1 = 1,
		Player2 = 2,
	}

	public static class PlayerExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Player Opponent(this Player player)
		{
			return 3 - player;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short Value(this Player player)
		{
			return (short)player;
		}
	}
}
