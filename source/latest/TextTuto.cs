using UnityEngine;

public class TextTuto : MonoBehaviour
{
	public string TutoText = string.Empty;

	public GUIText toto;

	private void Update()
	{
		toto.text = TutoText;
	}
}
