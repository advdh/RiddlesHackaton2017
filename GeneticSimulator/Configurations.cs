using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace GeneticSimulator
{
	public class Configurations : List<Configuration>
	{
		public void Save(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Create))
			{
				var serializer = new XmlSerializer(typeof(Configurations));
				serializer.Serialize(stream, this);
			}
		}

		public static Configurations Load(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Open))
			{
				var serializer = new XmlSerializer(typeof(Configurations));
				return (Configurations)serializer.Deserialize(stream);
			}
		}
	}
}
