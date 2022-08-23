using UnityEngine;

public class CoinsDisplayManager : MonoBehaviour
{
	private UILabel m_pLabel;

	private static double m_fMoney;

	public double m_fInertia = 0.3;

	public AudioSource CoinsSound;

	private float m_fSoundRequest;

	public float SoundToPlay = 0.1f;

	private void Awake()
	{
		m_pLabel = GetComponent<UILabel>();
		m_fMoney = (float)Singleton<GameSaveManager>.Instance.GetCoins();
		DisplayMoney();
		m_fSoundRequest = 0f;
	}

	private void Update()
	{
		if (!m_pLabel)
		{
			return;
		}
		int coins = Singleton<GameSaveManager>.Instance.GetCoins();
		if (coins != (int)(m_fMoney + 0.5))
		{
			float deltaTime = Time.deltaTime;
			m_fMoney = Tricks.ComputeInertia(m_fMoney, coins, m_fInertia, deltaTime);
			m_fSoundRequest += deltaTime;
			DisplayMoney();
			if ((bool)CoinsSound && m_fSoundRequest > SoundToPlay)
			{
				CoinsSound.Play();
				m_fSoundRequest = 0f;
			}
		}
	}

	private void OnEnable()
	{
		DisplayMoney();
	}

	private void DisplayMoney()
	{
		if ((bool)m_pLabel)
		{
			if (m_fMoney == 0.0)
			{
				m_pLabel.text = "0";
			}
			else
			{
				m_pLabel.text = string.Format("{0:# ### ### ###}", (int)(m_fMoney + 0.5));
			}
		}
	}
}
