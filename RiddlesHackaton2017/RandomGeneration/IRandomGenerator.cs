namespace RiddlesHackaton2017.RandomGeneration
{
	public interface IRandomGenerator
	{
		/// <summary>
		/// Returns a non-negative random value less than the specified maximum value
		/// </summary>
		/// <param name="maxValue"></param>
		int Next(int maxValue);
		IRandomGenerator Clone(int i);
	}
}
