using System.Collections.Generic;
using UnityEngine;

public class UITexturePattern : MonoBehaviour
{
	public List<string> m_sTextureName = new List<string>();

	public void ChangeTexture(int iNum)
	{
		iNum %= m_sTextureName.Count;
		UISprite component = GetComponent<UISprite>();
		if ((bool)component)
		{
			component.spriteName = m_sTextureName[iNum];
		}
	}
}
