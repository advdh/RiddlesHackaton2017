using System;
using System.Data;

namespace RiddlesHackaton2017.IntegrationTest
{
	public static class DataReaderExtensions
	{
		public static Byte? GetNullableInt16(this IDataReader dataReader, int index)
		{
			if (dataReader.IsDBNull(index)) return null;
			return dataReader.GetByte(index);
		}

		public static string GetNullableString(this IDataReader dataReader, int index)
		{
			if (dataReader.IsDBNull(index)) return null;
			return dataReader.GetString(index);
		}
	}
}
