using UnityEngine;

public class MenuMulti : AbstractMenu
{
	private NetworkMgr networkMgr;

	public int nameLengthLimit = 8;

	public UIInput Input;

	public override void Awake()
	{
		base.Awake();
		networkMgr = (NetworkMgr)Object.FindObjectOfType(typeof(NetworkMgr));
	}

	public override void OnEnter()
	{
		base.OnEnter();
		if (Input != null)
		{
			Input.text = Singleton<GameSaveManager>.Instance.GetPseudo();
			Input.defaultText = Localization.instance.Get("MENU_PLAYER");
			Input.maxChars = 8;
			Input.selected = false;
		}
		StartCoroutine(networkMgr.CheckIP());
	}

	public override void OnExit()
	{
		OnSubmit();
		base.OnExit();
	}

	public void OnButtonLocal()
	{
		OnSubmit();
		networkMgr.BLanOnly = true;
		ActSwapMenu(EMenus.MENU_MULTI_JOIN);
	}

	public void OnButtonOnLine()
	{
		OnSubmit();
		networkMgr.BLanOnly = false;
		ActSwapMenu(EMenus.MENU_MULTI_JOIN);
	}

	private void OnSubmit()
	{
		if (Input != null)
		{
			string text = NGUITools.StripSymbols(Input.text);
			string text2 = text.Trim();
			if (!string.IsNullOrEmpty(text2) && text2 != Localization.instance.Get("MENU_PLAYER"))
			{
				Singleton<GameSaveManager>.Instance.SetPseudo(text2, true);
			}
			else if (Input.text.Equals(string.Empty))
			{
				Singleton<GameSaveManager>.Instance.SetPseudo(string.Empty, true);
			}
			Input.selected = false;
		}
	}

	public override void Update()
	{
		if (Application.platform == RuntimePlatform.Android && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			ActSwapMenu(EMenus.MENU_WELCOME);
		}
	}
}
