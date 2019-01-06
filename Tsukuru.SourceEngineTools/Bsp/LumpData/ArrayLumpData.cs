using System.IO;

namespace Tsukuru.SourceEngineTools.Bsp.LumpData
{
	public class ArrayLumpData<T>: LumpData where T: struct
	{
		public T[] Elements
		{
			get; protected set;
		}
		public ArrayLumpData(BinaryReader reader, int length)
		{
			if (reader == null) { return; }
			Elements = reader.ReadStructArray<T>(length);
		}

		public override string ToString() => $"{typeof(T).ToString()}[{Elements.Length}";
	}
}