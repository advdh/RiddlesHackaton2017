namespace RiddlesHackaton2017.RandomGeneration
{
	/// <summary>
	/// Random generator, which generates always 0 as nex value (so not random at all)
	/// </summary>
	public class FirstIndexGenerator : IRandomGenerator
	{
		public int Next(int maxValue)
		{
			return 0;
		}
	}
}
