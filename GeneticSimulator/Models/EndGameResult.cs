using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GeneticSimulator.Models
{
	public class EndGameResult
	{
		public int WinningId { get; set; }
		public int LoosingId { get; set; }
		public string GameId { get; set; }
		public bool? Won { get; set; }
		public int StartRound { get; set; }
		public int EndRound { get; set; }

		public override string ToString()
		{
			string wonString = Won.HasValue && Won.Value ? "Won" : "Lost/Draw";
			return $"{WinningId} - {LoosingId}: {wonString} in {EndRound - StartRound} rounds";
		}
	}

	public class EndGameResults
	{
		public Configurations Configurations { get; set; } = new Configurations();

		public List<EndGameResult> Results { get; set; } = new List<EndGameResult>();

		public void Save(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Create))
			{
				var serializer = new XmlSerializer(typeof(EndGameResults));
				serializer.Serialize(stream, this);
			}
		}

		public static EndGameResults Load(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Open))
			{
				var serializer = new XmlSerializer(typeof(EndGameResults));
				return (EndGameResults)serializer.Deserialize(stream);
			}
		}
	}

}
