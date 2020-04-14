using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tsukuru.Core.SourceEngine
{
	public class SourceMaterial
	{
		private const string ShaderLightMappedGeneric = "lightmappedGeneric";

		public string ShaderName { get; }

		public Dictionary<string, string> Values { get; } = new Dictionary<string, string>();

		public string BaseTexture
		{
			get
			{
				string name;

				if (Values.TryGetValue("basetexture", out name))
				{
					return name;
				}

				if (Values.TryGetValue("phong", out name))
				{
					return name;
				}

				return null;
			}
		}

		public SourceMaterial(string textureName)
		{
			Values.Add("basetexture", textureName);
			ShaderName = ShaderLightMappedGeneric;
		}

		public SourceMaterial(BinaryReader reader, int length)
		{
			var text = Encoding.ASCII.GetString(reader.ReadBytes(length));
			var blockStart = text.IndexOf('{');
			ShaderName = text.Substring(0, blockStart).Replace("\"", "").Trim();
			var blockEnd = text.IndexOf('}');
			var block = text.Substring(blockStart, blockEnd - blockStart).Trim();
			var lines = block.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var line in lines)
			{
				var trimmedLine = line.Replace("$", "").Replace("%", "").Replace("\"", "").Replace("'", "").Trim();
				var keyEnd = trimmedLine.IndexOf(' ');
				if (keyEnd == -1)
				{
					keyEnd = trimmedLine.IndexOf('\t');

					if (keyEnd == -1)
					{
						continue;
					}
				}
				var key = trimmedLine.Substring(0, keyEnd).ToLower();
				var value = trimmedLine.Substring(keyEnd + 1).Replace("\t", "");
				Values[key] = value;
			}
		}

		public IEnumerable<string> GetTextures()
		{
			yield return TryGetValue("basetexture");
			yield return TryGetValue("texture2");
			yield return TryGetValue("phong");
			yield return TryGetValue("envmap");
			yield return TryGetValue("tooltexture");
			yield return TryGetValue("bumpmap");
			yield return TryGetValue("normalmap");
			yield return TryGetValue("bottommaterial");
		}

		private string TryGetValue(string key)
		{
			string value;
			Values.TryGetValue(key, out value);
			return value;
		}
	}
}