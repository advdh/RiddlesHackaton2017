using RiddlesHackaton2017.Models;
using System;
using System.Diagnostics;
using System.Linq;

namespace RiddlesHackaton2017.Evaluation
{
	public static class AlphaBetaEvaluator
	{
		/// <returns>Node with the best score</returns>
		public static Node AlphaBeta(Board board, TimeSpan maxDuration)
		{
			var root = new Node() { Board = board };
			return AlphaBeta(root, maxDuration);
		}

		/// <returns>Node with the best score</returns>
		public static Node AlphaBeta(Node root, TimeSpan maxDuration)
		{
			var stopwatch = Stopwatch.StartNew();
			int depth = Math.Max(root.MaxDepth - 1, 1);
			var stopTime = DateTime.UtcNow.Add(maxDuration);
			var board = root.Board;
			var endNode = AlphaBeta(root, depth, Node.AlphaNode, Node.BetaNode, board.MyPlayer, stopTime);

			while (stopwatch.ElapsedMilliseconds < maxDuration.TotalMilliseconds && depth < 100)
			{
				depth++;
				endNode = AlphaBeta(root, depth, Node.AlphaNode, Node.BetaNode, board.MyPlayer, stopTime);
			}

			return endNode;
		}

		public static Node AlphaBeta(Node node, int depth, Node alpha, Node beta, Player player, DateTime stopTime)
		{
			if (depth == 0 || (depth == 1 && DateTime.UtcNow > stopTime) || !node.GetChildren(player).Any())
			{
				if (node.Score == 0) node.Score = BoardEvaluator.Evaluate(node.Board);

				//Prefer to win in less turns, prefer to loose in more turns
				if (node.HasChildren() && !node.GetChildren(player).Any())
				{
					node.Score += 100 * Math.Sign(node.Score);
				}
				return node;
			}

			if (player == node.Board.MyPlayer)
			{
				foreach (Node child in node.GetChildren(player))
				{
					var childNode = AlphaBeta(child, depth - 1, alpha, beta, player.Opponent(), stopTime);
					child.Score = childNode.Score;
					if (childNode.Score > alpha.Score)
					{
						alpha = childNode;
					}
					if (beta.Score < alpha.Score)
					{
						break;
					}
				}
				return alpha;
			}
			else
			{
				foreach (Node child in node.GetChildren(player))
				{
					var childNode = AlphaBeta(child, depth - 1, alpha, beta, player.Opponent(), stopTime);
					child.Score = childNode.Score;
					if (childNode.Score < beta.Score)
					{
						beta = childNode;
					}
					if (beta.Score < alpha.Score)
					{
						break;
					}
				}
				return beta;
			}
		}
	}
}
