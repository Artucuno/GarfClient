using UnityEngine;

public class MenuBestOfGarfield : AbstractMenu
{
	public UISprite m_oSpriteBg;

	public GameObject[] m_oPuzzles = new GameObject[3];

	public UILocalize m_oTrackName;

	public string[] Atlases = new string[4];

	private int m_iCurrentPuzzle;

	public override void Awake()
	{
		base.Awake();
	}

	public override void OnEnter()
	{
		base.OnEnter();
		UpdatePanel();
	}

	public void OnButtonLeft()
	{
		m_iCurrentPuzzle = (m_iCurrentPuzzle + 15) & 0xF;
		UpdatePanel();
	}

	public void OnButtonRight()
	{
		m_iCurrentPuzzle = (m_iCurrentPuzzle + 1) & 0xF;
		UpdatePanel();
	}

	private void UpdatePanel()
	{
		int num = m_iCurrentPuzzle >> 2;
		int num2 = m_iCurrentPuzzle & 3;
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		for (int i = 0; i < 3; i++)
		{
			bool flag = instance.IsPuzzlePieceUnlocked(string.Format("E{0}C{1}_{2}", num2 + 1, num + 1, i));
			m_oPuzzles[i].SetActive(!flag);
		}
		if ((bool)m_oSpriteBg)
		{
			if (m_oSpriteBg.atlas != null)
			{
				Resources.UnloadAsset(m_oSpriteBg.atlas.spriteMaterial.mainTexture);
			}
			UIAtlas atlas = (UIAtlas)Resources.Load(Atlases[num], typeof(UIAtlas));
			m_oSpriteBg.atlas = atlas;
			m_oSpriteBg.atlas.spriteMaterial.mainTexture = Resources.Load(m_oSpriteBg.atlas.spriteMaterial.mainTexture.name, typeof(Texture2D)) as Texture2D;
			m_oSpriteBg.spriteName = "Puzzle_" + ((m_iCurrentPuzzle + 1 >= 10) ? string.Empty : "0") + (m_iCurrentPuzzle + 1);
		}
		m_oTrackName.key = string.Format("TRACK_NAME_E{0}C{1}", num2 + 1, num + 1);
		m_oTrackName.Localize();
	}

	public override void OnExit()
	{
		Resources.UnloadAsset(m_oSpriteBg.atlas.spriteMaterial.mainTexture);
		base.OnExit();
	}
}
