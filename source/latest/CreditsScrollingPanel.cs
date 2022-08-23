using System.Collections.Generic;
using UnityEngine;

public class CreditsScrollingPanel : MonoBehaviour
{
	public Color _colorTI1 = Color.white;

	public Color _colorTI2 = Color.white;

	public Color _colorITM = Color.white;

	public GameObject CreditsPanel;

	public GameObject CreditsFont;

	public float ScrollingSpeed = 5f;

	public TextAsset CreditsFile;

	public int ItemCount = 10;

	public AbstractMenu CreditsMenu;

	public EMenus BackMenu = EMenus.MENU_WELCOME;

	private float _startingOffset = -200f;

	private float _endOffset = 200f;

	private ByteReader Reader;

	private List<CreditsScrollingLabel> ScrollingLabels = new List<CreditsScrollingLabel>();

	private Dictionary<string, CreditsItemDesc> CreditsItemDataBase;

	private UISprite m_pAnimSprite;

	private int m_iEndedLabels;

	private void Awake()
	{
		if (!CreditsMenu)
		{
			CreditsMenu = GameObject.Find("MENU_CREDITS").GetComponent<AbstractMenu>();
		}
		CreditsItemDataBase = new Dictionary<string, CreditsItemDesc>
		{
			{
				"[TI1]",
				new CreditsItemDesc
				{
					FontName = "Chinacat",
					FontSize = 64f,
					Offset = 145f,
					FontColor = _colorTI1
				}
			},
			{
				"[TI2]",
				new CreditsItemDesc
				{
					FontName = "Chinacat",
					FontSize = 64f,
					Offset = 100f,
					FontColor = _colorTI2
				}
			},
			{
				"[ITM]",
				new CreditsItemDesc
				{
					FontName = "Chinacat",
					FontSize = 56f,
					Offset = 55f,
					FontColor = _colorITM
				}
			}
		};
		_endOffset = CreditsPanel.transform.localScale.y / 2f + 50f;
		_startingOffset = 0f - _endOffset;
		GameObject gameObject = null;
		for (int i = 0; i < ItemCount; i++)
		{
			gameObject = new GameObject();
			gameObject.AddComponent<UILabel>();
			gameObject.GetComponent<UILabel>().text = "Bob";
			gameObject.transform.parent = base.transform.GetChild(0);
			gameObject.transform.localPosition = new Vector3(465.5f, _startingOffset - (float)i * 45f, -72f);
			gameObject.transform.localScale = new Vector3(64f, 64f, 1f);
			gameObject.AddComponent<CreditsScrollingLabel>();
			gameObject.GetComponent<CreditsScrollingLabel>().Speed = ScrollingSpeed;
			gameObject.GetComponent<CreditsScrollingLabel>().EndOffset = _endOffset;
			gameObject.GetComponent<CreditsScrollingLabel>().OnLabelEnd += ScrollingLabelEndHandler;
			ScrollingLabels.Add(gameObject.GetComponent<CreditsScrollingLabel>());
		}
		GameObject gameObject2 = GameObject.Find("PanelAnimation");
		if (gameObject2 != null)
		{
			m_pAnimSprite = gameObject2.GetComponentInChildren<UISprite>();
		}
		if (m_pAnimSprite != null)
		{
			m_pAnimSprite.atlas.spriteMaterial.mainTexture = Resources.Load("ANIM", typeof(Texture2D)) as Texture2D;
		}
	}

	private void OnEnable()
	{
		Reset();
		Load();
	}

	private void Load()
	{
		if ((bool)CreditsFile)
		{
			Reader = new ByteReader(CreditsFile);
			for (int i = 0; i < ScrollingLabels.Count; i++)
			{
				UpdateLabel(ScrollingLabels[i]);
			}
		}
	}

	private void UpdateLabelsSpeed()
	{
		if (ScrollingLabels != null)
		{
			for (int i = 0; i < ScrollingLabels.Count; i++)
			{
				ScrollingLabels[i].Speed = ScrollingSpeed;
			}
		}
	}

	private void ScrollingLabelEndHandler(CreditsScrollingLabel Label)
	{
		m_iEndedLabels++;
		if (m_iEndedLabels == ScrollingLabels.Count)
		{
			Reset();
			CreditsMenu.ActSwapMenu(BackMenu);
		}
	}

	private void UpdateLabel(CreditsScrollingLabel Label)
	{
		string empty = string.Empty;
		CreditsScrollingLabel creditsScrollingLabel = FindPreviousLabel(Label);
		float num = _startingOffset;
		if (creditsScrollingLabel != null)
		{
			num = FindPreviousLabel(Label).gameObject.transform.localPosition.y;
		}
		empty = Reader.ReadLine();
		while (empty == null)
		{
			Reader = new ByteReader(CreditsFile);
			empty = Reader.ReadLine();
			num -= 450f;
		}
		string key = empty.Substring(0, 5);
		if (CreditsItemDataBase.ContainsKey(key))
		{
			num -= CreditsItemDataBase[key].Offset;
			Label.transform.localPosition = new Vector3(465.5f, num, -72f);
			Label.SetItem(empty.Substring(5), CreditsItemDataBase[key], CreditsFont);
		}
	}

	private CreditsScrollingLabel FindPreviousLabel(CreditsScrollingLabel Label)
	{
		int num = -1;
		for (int i = 0; i < ScrollingLabels.Count; i++)
		{
			if (ScrollingLabels[i] == Label)
			{
				num = i;
				break;
			}
		}
		int num2 = num - 1;
		if (num2 < 0)
		{
			num2 = ScrollingLabels.Count - 1;
		}
		if (ScrollingLabels[num2].Updating)
		{
			return ScrollingLabels[num2];
		}
		return null;
	}

	private void Reset()
	{
		m_iEndedLabels = 0;
		for (int i = 0; i < ScrollingLabels.Count; i++)
		{
			ScrollingLabels[i].Reset();
		}
	}

	public void OnDestroy()
	{
		Resources.UnloadAsset(m_pAnimSprite.atlas.spriteMaterial.mainTexture);
	}
}
