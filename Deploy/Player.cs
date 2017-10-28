namespace RiddlesHackaton2017.Models
{
	public enum Player
	{
		Player1 = 1,
		Player2 = 2,
	}

	public static class PlayerExtensions
	{
		public static Player Opponent(this Player player)
		{
			return 3 - player;
		}
	}
}
