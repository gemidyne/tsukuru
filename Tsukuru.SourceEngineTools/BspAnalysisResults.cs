using System.Collections.Generic;

namespace Tsukuru.SourceEngineTools
{
	public class BspAnalysisResults
	{
		public IEnumerable<string> CustomModels { get; }

		public IEnumerable<string> CustomMaterials { get; }

		public IEnumerable<string> CustomSounds { get; }

		internal BspAnalysisResults(
			IEnumerable<string> customModels, 
			IEnumerable<string> customMaterials, 
			IEnumerable<string> customSounds)
		{
			CustomModels = customModels;
			CustomMaterials = customMaterials;
			CustomSounds = customSounds;
		}
	}
}
