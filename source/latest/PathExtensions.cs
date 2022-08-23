using System.IO;

public static class PathExtensions
{
	public static string Combine(params string[] pPaths)
	{
		string text = null;
		if (pPaths != null)
		{
			text = pPaths[0];
			for (int i = 1; i < pPaths.Length; i++)
			{
				text = Path.Combine(text, pPaths[i]);
			}
		}
		return text;
	}
}
