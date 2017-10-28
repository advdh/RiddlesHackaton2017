using RiddlesHackaton2017.Moves;
using RiddlesHackaton2017.Output;

namespace RiddlesHackaton2017.Bots
{
	public class PassBot : BaseBot
	{
		public PassBot(IConsole consoleError) : base(consoleError)
		{
		}

		public override Move GetMove()
		{
			return new PassMove();
		}
	}
}
