using System.Collections.Generic;

namespace Tsukuru.Core.SourceEngine;

internal class SkyboxMaterialHelper
{
	public static IEnumerable<string> GetFilePaths(string skyboxName)
	{
		// LDR skybox material filenames
		yield return $"materials/skybox/{skyboxName}lf.vmt";
		yield return $"materials/skybox/{skyboxName}rt.vmt";
		yield return $"materials/skybox/{skyboxName}bk.vmt";
		yield return $"materials/skybox/{skyboxName}dn.vmt";
		yield return $"materials/skybox/{skyboxName}up.vmt";
		yield return $"materials/skybox/{skyboxName}ft.vmt";

		// HDR skybox material filenames
		yield return $"materials/skybox/{skyboxName}_hdrlf.vmt";
		yield return $"materials/skybox/{skyboxName}_hdrrt.vmt";
		yield return $"materials/skybox/{skyboxName}_hdrbk.vmt";
		yield return $"materials/skybox/{skyboxName}_hdrdn.vmt";
		yield return $"materials/skybox/{skyboxName}_hdrup.vmt";
		yield return $"materials/skybox/{skyboxName}_hdrft.vmt";
	}
}