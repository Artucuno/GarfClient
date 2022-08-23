using System.Collections.Generic;
using UnityEngine;

public class MenuTutorial : AbstractMenu
{
	public enum ETutoMode
	{
		MODE_DRVING,
		MODE_BONUS,
		MODE_CUSTOM
	}

	public UISprite m_oPicture;

	public UILocalize m_oCategory;

	public UILocalize m_oTitle;

	public UILocalize m_oDesc;

	public List<UILocalize> m_oLabel;

	public UILabel m_oPage;

	private List<TutorialData> m_oTutorialDrivingList = new List<TutorialData>();

	private List<TutorialData> m_oTutorialBonusList = new List<TutorialData>();

	private List<TutorialData> m_oTutorialCustomList = new List<TutorialData>();

	private List<TutorialData> m_oSelectedList;

	private int m_iPage;

	public override void Awake()
	{
		base.Awake();
		Object[] array = Resources.LoadAll("Tutorials", typeof(TutorialData));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			if (((TutorialData)@object).bShownOnPc)
			{
				switch (((TutorialData)@object).m_eTutoType)
				{
				case ETutoMode.MODE_DRVING:
					m_oSelectedList = m_oTutorialDrivingList;
					break;
				case ETutoMode.MODE_BONUS:
					m_oSelectedList = m_oTutorialBonusList;
					break;
				case ETutoMode.MODE_CUSTOM:
					m_oSelectedList = m_oTutorialCustomList;
					break;
				}
				Resources.UnloadAsset(((TutorialData)@object).atlas.spriteMaterial.mainTexture);
				m_oSelectedList.Add((TutorialData)@object);
			}
		}
	}

	public override void OnEnter(int iEntryPoint)
	{
		base.OnEnter();
		switch (iEntryPoint)
		{
		case 0:
			m_oSelectedList = m_oTutorialDrivingList;
			m_oCategory.key = "MENU_BT_TUTO_CONTROLE";
			break;
		case 1:
			m_oSelectedList = m_oTutorialBonusList;
			m_oCategory.key = "MENU_BT_TUTO_BONUS";
			break;
		case 2:
			m_oSelectedList = m_oTutorialCustomList;
			m_oCategory.key = "MENU_BT_TUTO_BONUS";
			break;
		}
		m_oCategory.Localize();
		m_iPage = 0;
		UpdatePanel();
	}

	public override void OnExit()
	{
		base.OnExit();
		if ((bool)m_oPicture.atlas)
		{
			Resources.UnloadAsset(m_oPicture.atlas.spriteMaterial.mainTexture);
		}
	}

	public void OnButtonLeft()
	{
		m_iPage = (m_iPage + m_oSelectedList.Count - 1) % m_oSelectedList.Count;
		UpdatePanel();
	}

	public void OnButtonRight()
	{
		m_iPage = (m_iPage + 1) % m_oSelectedList.Count;
		UpdatePanel();
	}

	public void UpdatePanel()
	{
		TutorialData tutorialData = m_oSelectedList[m_iPage];
		if ((bool)m_oTitle)
		{
			m_oTitle.key = tutorialData.m_TitleTextId;
			m_oTitle.Localize();
		}
		if ((bool)m_oDesc)
		{
			m_oDesc.key = tutorialData.m_InfoTextId;
			m_oDesc.Localize();
		}
		for (int i = 0; i < m_oLabel.Count; i++)
		{
			if ((bool)m_oLabel[i] && i < tutorialData.m_LabelId.Count)
			{
				if (tutorialData.m_LabelId[i] == string.Empty)
				{
					m_oLabel[i].gameObject.SetActive(false);
					continue;
				}
				m_oLabel[i].gameObject.SetActive(true);
				m_oLabel[i].key = tutorialData.m_LabelId[i];
				m_oLabel[i].Localize();
			}
		}
		if ((bool)m_oPicture)
		{
			if ((bool)m_oPicture.atlas)
			{
				Resources.UnloadAsset(m_oPicture.atlas.spriteMaterial.mainTexture);
			}
			m_oPicture.atlas = tutorialData.atlas;
			m_oPicture.atlas.spriteMaterial.mainTexture = Resources.Load(m_oPicture.atlas.name, typeof(Texture2D)) as Texture2D;
			m_oPicture.spriteName = tutorialData.spriteName;
		}
		if ((bool)m_oPage)
		{
			m_oPage.text = m_iPage + 1 + "/" + m_oSelectedList.Count;
		}
	}

	public override void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			ActSwapMenu(EMenus.MENU_TUTO_HUB);
		}
	}
}
