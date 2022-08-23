using System;
using UnityEngine;

[ExecuteInEditMode]
public class BRDFLookupTexture : MonoBehaviour
{
	public float intensity = 1f;

	public float diffuseIntensity = 1f;

	public Color keyColor = ColorRGB(188, 158, 118);

	public Color fillColor = ColorRGB(86, 91, 108);

	public Color backColor = ColorRGB(44, 54, 57);

	public float wrapAround;

	public float metalic;

	public float specularIntensity = 1f;

	public float specularShininess = 5f / 64f;

	public float translucency;

	public Color translucentColor = ColorRGB(255, 82, 82);

	public int lookupTextureWidth = 128;

	public int lookupTextureHeight = 128;

	public bool fastPreview = true;

	public Texture2D lookupTexture;

	private void Awake()
	{
		if (!lookupTexture)
		{
			Bake();
		}
	}

	private static Color ColorRGB(int r, int g, int b)
	{
		return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, 0f);
	}

	private void CheckConsistency()
	{
		intensity = Mathf.Max(0f, intensity);
		wrapAround = Mathf.Clamp(wrapAround, -1f, 1f);
		metalic = Mathf.Clamp(metalic, 0f, 12f);
		diffuseIntensity = Mathf.Max(0f, diffuseIntensity);
		specularIntensity = Mathf.Max(0f, specularIntensity);
		specularShininess = Mathf.Clamp(specularShininess, 0.01f, 1f);
		translucency = Mathf.Clamp01(translucency);
	}

	private Color PixelFunc(float ndotl, float ndoth)
	{
		ndotl *= Mathf.Pow(ndoth, metalic);
		float num = (1f + metalic * 0.25f) * Mathf.Max(0f, diffuseIntensity - (1f - ndoth) * metalic);
		float t = Mathf.Clamp01(Mathf.InverseLerp(0f - wrapAround, 1f, ndotl * 2f - 1f));
		float t2 = Mathf.Clamp01(Mathf.InverseLerp(-1f, Mathf.Max(-0.99f, 0f - wrapAround), ndotl * 2f - 1f));
		Color color = num * Color.Lerp(backColor, Color.Lerp(fillColor, keyColor, t), t2);
		color += backColor * (1f - num) * Mathf.Clamp01(diffuseIntensity);
		float num2 = specularShininess * 128f;
		float num3 = (num2 + 2f) * (num2 + 4f) / ((float)Math.PI * 8f * (Mathf.Pow(2f, (0f - num2) / 2f) + num2));
		float a = specularIntensity * num3 * Mathf.Pow(ndoth, num2);
		float num4 = ndotl + 0.1f;
		float num5 = 0.5f * translucency * Mathf.Clamp01(1f - num4 * ndoth) * Mathf.Clamp01(1f - ndotl);
		Color color2 = color * intensity + translucentColor * num5 + new Color(0f, 0f, 0f, a);
		return color2 * intensity;
	}

	private void TextureFunc(Texture2D tex)
	{
		for (int i = 0; i < tex.height; i++)
		{
			for (int j = 0; j < tex.width; j++)
			{
				float num = tex.width;
				float num2 = tex.height;
				float num3 = (float)j / num;
				float num4 = (float)i / num2;
				float ndotl = num3;
				float ndoth = num4;
				Color color = PixelFunc(ndotl, ndoth);
				tex.SetPixel(j, i, color);
			}
		}
	}

	private void GenerateLookupTexture(int width, int height)
	{
		Texture2D texture2D = ((!lookupTexture || lookupTexture.width != width || lookupTexture.height != height) ? new Texture2D(width, height, TextureFormat.ARGB32, false) : lookupTexture);
		CheckConsistency();
		TextureFunc(texture2D);
		texture2D.Apply();
		texture2D.wrapMode = TextureWrapMode.Clamp;
		if (lookupTexture != texture2D)
		{
			UnityEngine.Object.DestroyImmediate(lookupTexture);
		}
		lookupTexture = texture2D;
	}

	public void Preview()
	{
		GenerateLookupTexture(32, 64);
	}

	public void Bake()
	{
		GenerateLookupTexture(lookupTextureWidth, lookupTextureHeight);
	}
}
