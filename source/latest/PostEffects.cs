using UnityEngine;

public class PostEffects : MonoBehaviour
{
	public static Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		if ((bool)m2Create && m2Create.shader == s)
		{
			return m2Create;
		}
		if (!s)
		{
			return null;
		}
		if (!s.isSupported)
		{
			return null;
		}
		m2Create = new Material(s);
		m2Create.hideFlags = HideFlags.DontSave;
		return m2Create;
	}

	public static bool CheckSupport(bool needDepth)
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			return false;
		}
		return true;
	}
}
