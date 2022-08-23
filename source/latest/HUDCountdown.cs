using System.Collections;
using UnityEngine;

public class HUDCountdown : MonoBehaviour
{
	public UISprite Sprite3;

	public UISprite Sprite2;

	public UISprite Sprite1;

	public UISprite SpriteGo;

	public float GoHideDelay = 1f;

	private void Awake()
	{
		Sprite3.gameObject.SetActive(false);
		Sprite2.gameObject.SetActive(false);
		Sprite1.gameObject.SetActive(false);
		SpriteGo.gameObject.SetActive(false);
	}

	public void SetCountdown(int countdown)
	{
		base.gameObject.SetActive(countdown >= 0);
		if (countdown > 0)
		{
			Sprite3.gameObject.SetActive(countdown == 3);
			Sprite2.gameObject.SetActive(countdown == 2);
			Sprite1.gameObject.SetActive(countdown == 1);
			SpriteGo.gameObject.SetActive(false);
		}
		else if (countdown == 0)
		{
			Sprite1.gameObject.SetActive(false);
			SpriteGo.gameObject.SetActive(true);
			StartCoroutine(DelayedHideGo());
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	protected IEnumerator DelayedHideGo()
	{
		yield return new WaitForSeconds(GoHideDelay);
		SetCountdown(-1);
	}
}
