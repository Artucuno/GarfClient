using System;
using System.IO;
using UnityEngine;

internal class ModGUIHelper
{
	static ModGUIHelper()
	{
		ModGUIHelper.tickerArrowTextures = new Tuple<Texture2D, Texture2D>(ModGUIHelper.getTexture("arrow_left.png", 77, 78), ModGUIHelper.getTexture("arrow_right.png", 77, 78));
		ModGUIHelper.tickerGUIStyle = new GUIStyle(GUI.skin.button);
		ModGUIHelper.tickerGUIStyle.fontSize = 24;
		ModGUIHelper.tickerGUIStyle.alignment = TextAnchor.MiddleCenter;
		Texture2D texture = ModGUIHelper.getTexture("ticker_texture.png", (int)ModGUIHelper.tickerSize.x, (int)ModGUIHelper.tickerSize.y);
		ModGUIHelper.tickerGUIStyle.normal.background = texture;
		ModGUIHelper.tickerGUIStyle.hover.background = texture;
		ModGUIHelper.tickerGUIStyle.active.background = texture;
		ModGUIHelper.buttonGUIStyle = new GUIStyle(GUI.skin.button);
		ModGUIHelper.buttonGUIStyle.fontSize = ((Screen.width < 1000) ? 20 : 28);
		ModGUIHelper.buttonGUIStyle.alignment = TextAnchor.MiddleCenter;
		ModGUIHelper.buttonGUIStyle.normal.background = ModGUIHelper.getTexture("button_texture.png", 12, 12);
		ModGUIHelper.buttonGUIStyle.hover.background = ModGUIHelper.getTexture("button_hover_texture.png", 12, 12);
		ModGUIHelper.buttonGUIStyle.active.background = ModGUIHelper.getTexture("button_active_texture.png", 12, 12);
	}

	public static bool CenteredButton(Vector3 center, Vector2 size, string text, string tooltip)
	{
		GUIContent guicontent = new GUIContent();
		guicontent.text = text;
		guicontent.tooltip = tooltip;
		size /= 2f;
		return GUI.Button(new Rect(center.x - size.x, center.y - size.y, size.x * 2f, size.y * 2f), guicontent, ModGUIHelper.buttonGUIStyle);
	}

	public static bool CenteredTickerButton(Vector3 center, ModGUIHelper.TickerDirection direction)
	{
		GUIContent guicontent = new GUIContent();
		guicontent.image = ((direction == ModGUIHelper.TickerDirection.LEFT) ? ModGUIHelper.tickerArrowTextures.Item1 : ModGUIHelper.tickerArrowTextures.Item2);
		Vector2 vector = new Vector2(ModGUIHelper.tickerSize.x, ModGUIHelper.tickerSize.y) / 2f;
		return GUI.Button(new Rect(center.x - vector.x, center.y - vector.y, vector.x * 2f, vector.y * 2f), guicontent, ModGUIHelper.tickerGUIStyle);
	}

	private static Texture2D getTexture(string fileName, int width, int height)
	{
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
		WWW www = new WWW(ModGUIHelper.baseResourcesURL + fileName);
		www.LoadImageIntoTexture(texture2D);
		while (!www.isDone)
		{
		}
		return texture2D;
	}

	public static void CenteredLabel(Vector3 center, Vector2 size, string text)
	{
		ModGUIHelper.CenteredLabel(center, size, text, GUI.skin.label.fontSize);
	}

	public static string CenteredTextField(Vector3 center, Vector2 size, string defaultText, FontStyle textStyle = FontStyle.Normal, int fontSize = 32)
	{
		GUIStyle guistyle = new GUIStyle(GUI.skin.textField);
		guistyle.alignment = TextAnchor.MiddleCenter;
		guistyle.fontSize = fontSize;
		guistyle.fontStyle = textStyle;
		size /= 2f;
		return GUI.TextField(new Rect(center.x - size.x, center.y - size.y, size.x * 2f, size.y * 2f), defaultText, guistyle);
	}

	public static bool RectButton(Rect rect, string text, string tooltip = "")
	{
		return GUI.Button(rect, new GUIContent
		{
			text = text,
			tooltip = tooltip
		}, ModGUIHelper.buttonGUIStyle);
	}

	public static bool CenteredButton(Vector3 center, Vector2 size, string text, string tooltip, bool isActive)
	{
		GUIContent guicontent = new GUIContent();
		guicontent.text = text;
		guicontent.tooltip = tooltip;
		size /= 2f;
		GUIStyle guistyle = ModGUIHelper.buttonGUIStyle;
		if (isActive)
		{
			guistyle = new GUIStyle(ModGUIHelper.buttonGUIStyle);
			guistyle.normal = guistyle.active;
			guistyle.hover = guistyle.active;
		}
		return GUI.Button(new Rect(center.x - size.x, center.y - size.y, size.x * 2f, size.y * 2f), guicontent, guistyle);
	}

	public static void CenteredLabel(Vector3 center, Vector2 size, string text, int fontSize)
	{
		GUIStyle guistyle = new GUIStyle(GUI.skin.label);
		guistyle.alignment = TextAnchor.UpperCenter;
		guistyle.fontSize = fontSize;
		GUI.Label(new Rect(center.x - size.x, center.y - size.y, size.x * 2f, size.y * 2f), text, guistyle);
	}

	private static GUIStyle buttonGUIStyle;

	private static string baseResourcesURL = "file://" + new DirectoryInfo(Application.dataPath + "/Resources/").FullName;

	private static GUIStyle tickerGUIStyle;

	private static Tuple<Texture2D, Texture2D> tickerArrowTextures;

	private static Vector2 tickerSize = new Vector2(96f, 99f);

	public enum TickerDirection
	{
		LEFT,
		RIGHT
	}
}