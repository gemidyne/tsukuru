using System.IO;
using System.Linq;
using System.Text;

namespace Tsukuru.Core.SourceEngine.Bsp.LumpData
{
	public class EntitiesString : LumpData
	{
		public string[] All { get; }

		public EntitiesString(BinaryReader reader, int length)
		{
			var bytes = reader.ReadBytes(length);
			string full = Encoding.ASCII.GetString(bytes);

			All = full.Split('}').Select(e => e.Trim('\n')).ToArray();
		}

		private string GetPart(int offset)
		{
			return All[offset] + "}";

			//var builder = new StringBuilder();

			//for (var i = offset; i < All.Length; i++)
			//{
			//	builder.Append(All[i]);

			//	if (All[i] == '}')
			//	{
			//		break;
			//	}
			//}
			//return builder.ToString();
		}

		public string this[int i]
		{
			get { return GetPart(i); }
		}
	}
}