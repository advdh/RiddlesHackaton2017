using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GeneticSimulator
{
	public class GameResults : List<GameResult>
	{
		public void Save(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Create))
			{
				var serializer = new XmlSerializer(typeof(GameResults));
				serializer.Serialize(stream, this);
			}
		}

		public static GameResults Load(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Open))
			{
				var serializer = new XmlSerializer(typeof(GameResults));
				return (GameResults)serializer.Deserialize(stream);
			}
		}
	}
}
