using System;
using System.Data;

namespace RiddlesHackaton2017.IntegrationTest
{
	public static class DataReaderExtensions
	{
		public static bool? GetNullableBool(this IDataReader dataReader, int index)
		{
			if (dataReader.IsDBNull(index)) return null;
			return dataReader.GetBoolean(index);
		}

		public static int? GetNullableInt32(this IDataReader dataReader, int index)
		{
			if (dataReader.IsDBNull(index)) return null;
			return dataReader.GetInt32(index);
		}

		public static string GetNullableString(this IDataReader dataReader, int index)
		{
			if (dataReader.IsDBNull(index)) return null;
			return dataReader.GetString(index);
		}
	}
}
