using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace GeneticSimulator
{
	public class Generations : List<Generation>
	{
		public void Save(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Create))
			{
				var serializer = new XmlSerializer(typeof(Generations));
				serializer.Serialize(stream, this);
			}
		}

		public static Generations Load(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Open))
			{
				var serializer = new XmlSerializer(typeof(Generations));
				return (Generations)serializer.Deserialize(stream);
			}
		}
	}
}
