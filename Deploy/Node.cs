using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiddlesHackaton2017.Models
{
	public class Node
	{
		public Board Board { get; set; }

		public Node Parent { get; set; }

		public int Score { get; set; }

		private List<Node> _children;

		public IEnumerable<Node> GetChildren()
		{
			return _children;
		}

		/// <summary>
		/// Collection of children: one node for every move of the specified player
		/// </summary>
		public IEnumerable<Node> GetChildren(Player player)
		{
			if (_children == null)
			{
				_children = new List<Node>();

				//Neighbour moves
				var feasibleMoves = Board.GetFeasibleMovesForPlayer(player);
				foreach (var move in feasibleMoves)
				{
					var newBoard = Board.CopyAndPlay(Board, player, move);
					_children.Add(new Node()
					{
						Parent = this,
						Board = newBoard,
					});
				}
			}
			return _children;
		}
		
		public override string ToString()
		{
			return string.Format("Score {0:0} - Me: {1}; Opponent: {2}", 
				Score, PathString(Board.MyPlayer), PathString(Board.OpponentPlayer));
		}

		public int Depth
		{
			get
			{
				var node = this;
				int result = 0;
				while (node != null)
				{
					node = node.Parent;
					result++;
				}
				return result - 1;
			}
		}

		public int MaxDepth
		{
			get
			{
				return CalculateDepth(this);
			}
		}

		private int CalculateDepth(Node node)
		{
			int result = 0;
			bool hasChildren = node.HasChildren();
			if (hasChildren || Score != 0)
			{
				result = 1;
				if (hasChildren)
				{
					var children = node.GetChildren();
					if (children.Any())
					{
						result += children.Max(c => CalculateDepth(c));
					}
				}
			}
			return result;
		}

		public Node Root
		{
			get
			{
				var node = this;
				while (node != null && node.Parent != null)
				{
					node = node.Parent;
				}
				return node;
			}
		}
		public int Evaluations
		{
			get
			{
				return CalculateEvaluationCount(this);
			}
		}

		public bool HasChildren()
		{
			return _children != null;
		}

		private int CalculateEvaluationCount(Node node)
		{
			int result = 0;
			if (node.HasChildren() || node.Score != 0)
			{
				result = 1;
			} 
			if (node.HasChildren())
			{
				//NB: does not matter which player we specify here
				foreach(var child in node.GetChildren(Player.Player1))
				{
					result += CalculateEvaluationCount(child);
				}
			}
			return result;
		}

		private int? _playField;

		/// <summary>The field to play if this node is going to be played</summary>
		/// <remarks>Only explicitly set in case of run-to-opponent moves</remarks>
		public int? PlayField
		{
			get
			{
				if (_playField.HasValue) return _playField.Value;
				throw new NotImplementedException();
			}
			set
			{
				_playField = value;
			}
		}

		/// <summary>The Field to play = first move from root in the direction of this node</summary>
		public int PlayFieldFromRoot
		{
			get
			{
				var node = this;
				while (node != null && node.Parent != null && node.Parent.Parent != null)
				{
					node = node.Parent;
				}
				return node.Parent == null ? 0 : node.PlayField.Value;
			}
		}

		public string PathString(Player player)
		{
			var sb = new StringBuilder();
			var node = this;
			int previousPosition = -1;
			while (node != null)
			{
				int position = 0; //player == Player.Player1 ? node.Board.Player1Position : node.Board.Player2Position;
				if (position != previousPosition)
				{
					if (sb.Length > 0)
					{
						sb.Insert(0, " - ");
					}
					sb.Insert(0, new Position(position));
				}
				node = node.Parent;
				previousPosition = position;
			}
			return sb.ToString();
		}

		public static Node AlphaNode = new Node() { Score = int.MinValue };
		public static Node BetaNode = new Node() { Score = int.MaxValue };
	}
}
