using RiddlesHackaton2017.Models;
using RiddlesHackaton2017.Moves;
using System;

namespace RiddlesHackaton2017.MonteCarlo
{
	public interface IMoveSimulator
	{
		Tuple<Move, Board> GetRandomMove(Board board);
	}
}
