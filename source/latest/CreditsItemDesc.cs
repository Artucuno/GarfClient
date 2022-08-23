using UnityEngine;

public struct CreditsItemDesc
{
	public float FontSize;

	public float Offset;

	public Color FontColor;

	private GameObject FontPrefab;

	private string SavedFontName;

	public string FontName
	{
		get
		{
			return SavedFontName;
		}
		set
		{
			SavedFontName = value;
		}
	}

	public UIFont ItemFont
	{
		get
		{
			if (FontPrefab == null)
			{
				FontPrefab = Resources.Load(SavedFontName) as GameObject;
			}
			if (FontPrefab != null)
			{
				return FontPrefab.GetComponent<UIFont>();
			}
			return null;
		}
	}
}
