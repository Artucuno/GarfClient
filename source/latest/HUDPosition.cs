using System;
using System.Collections.Generic;
using UnityEngine;

public class HUDPosition : MonoBehaviour
{
	public GameObject Lap;

	public GameObject Position;

	public GameObject LapTime;

	public GameObject WrongWay;

	public UILabel Coins;

	public List<GameObject> PuzzleGO = new List<GameObject>();

	protected List<string> mSpriteNames = new List<string>();

	protected UISprite mPositionSprite;

	protected UILabel mLabelLap;

	protected UILabel mLabelLapTime;

	private int m_iCurrentIndex = -1;

	private int m_iCurrentLap = -1;

	protected List<Animation> m_cPuzzlesAnimation = new List<Animation>();

	protected List<UITexturePattern> m_cPuzzlesSprites = new List<UITexturePattern>();

	public bool StopTimer;

	public float LastLapPitchGain = 0.1f;

	protected int m_RaceTimeMs;

	private int m_iLogPuzzle;

	public int LogPuzzle
	{
		get
		{
			return m_iLogPuzzle;
		}
	}

	private void OnDestroy()
	{
		RewardManager instance = Singleton<RewardManager>.Instance;
		instance.OnEarnCoins = (Action<int>)Delegate.Remove(instance.OnEarnCoins, new Action<int>(EarnCoins));
	}

	private void EarnCoins(int pTotalCoins)
	{
		Coins.text = pTotalCoins.ToString();
	}

	public void Start()
	{
		Coins.text = "0";
		RewardManager instance = Singleton<RewardManager>.Instance;
		instance.OnEarnCoins = (Action<int>)Delegate.Combine(instance.OnEarnCoins, new Action<int>(EarnCoins));
		if (mPositionSprite == null && Position != null)
		{
			mPositionSprite = Position.GetComponent<UISprite>();
		}
		if (mLabelLap == null && Lap != null)
		{
			mLabelLap = Lap.GetComponent<UILabel>();
			if ((bool)mLabelLap)
			{
				mLabelLap.text = Localization.instance.Get("HUD_INGAME_LAP") + " : " + 0 + " / " + 3;
			}
		}
		if (mLabelLapTime == null && LapTime != null)
		{
			mLabelLapTime = LapTime.GetComponent<UILabel>();
		}
		mSpriteNames.Clear();
		if (mPositionSprite != null && mPositionSprite.atlas != null)
		{
			List<UIAtlas.Sprite> spriteList = mPositionSprite.atlas.spriteList;
			int i = 0;
			for (int count = spriteList.Count; i < count; i++)
			{
				UIAtlas.Sprite sprite = spriteList[i];
				if (string.IsNullOrEmpty("GK_HUD_Place") || sprite.name.StartsWith("GK_HUD_Place"))
				{
					mSpriteNames.Add(sprite.name);
				}
			}
		}
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			mPositionSprite.gameObject.SetActive(false);
			Coins.gameObject.transform.parent.gameObject.SetActive(false);
		}
		else if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
		{
			foreach (GameObject item in PuzzleGO)
			{
				item.SetActive(false);
			}
			mLabelLap.gameObject.SetActive(false);
			Coins.gameObject.transform.parent.gameObject.SetActive(false);
			mLabelLapTime.gameObject.SetActive(false);
		}
		else
		{
			mLabelLapTime.gameObject.SetActive(false);
		}
		foreach (GameObject item2 in PuzzleGO)
		{
			Animation componentInChildren = item2.GetComponentInChildren<Animation>();
			m_cPuzzlesAnimation.Add(componentInChildren);
			UITexturePattern componentInChildren2 = item2.GetComponentInChildren<UITexturePattern>();
			m_cPuzzlesSprites.Add(componentInChildren2);
			if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
			{
				item2.SetActive(false);
			}
		}
		for (int j = 0; j < 3; j++)
		{
			string pPiece = Singleton<GameConfigurator>.Instance.StartScene + "_" + j;
			if (Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(pPiece))
			{
				TakePuzzlePiece(j);
			}
		}
	}

	public void TakePuzzlePiece(int iIndex)
	{
		if (iIndex < 0 || iIndex >= PuzzleGO.Count)
		{
			return;
		}
		bool flag = true;
		for (int i = 0; i < 2; i++)
		{
			if (i != iIndex)
			{
				string pPiece = Singleton<GameConfigurator>.Instance.StartScene + "_" + i;
				if (!Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(pPiece))
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			foreach (Animation item in m_cPuzzlesAnimation)
			{
				item.Play("Sprite_Turn");
			}
		}
		else if (m_cPuzzlesAnimation[iIndex] != null)
		{
			m_cPuzzlesAnimation[iIndex].Play("Sprite_Turn");
		}
		if (m_cPuzzlesSprites[iIndex] != null)
		{
			m_cPuzzlesSprites[iIndex].ChangeTexture(1);
			if (LogManager.Instance != null)
			{
				m_iLogPuzzle++;
			}
		}
	}

	public void StopTimerAt(int raceTime)
	{
		if (!StopTimer)
		{
			m_RaceTimeMs = raceTime;
			StopTimer = true;
		}
	}

	public void DisplayRaceStats(RcVehicleRaceStats _Stats)
	{
		int rank = _Stats.GetRank();
		int raceNbLap = _Stats.GetRaceNbLap();
		int nbLapCompleted = _Stats.GetNbLapCompleted();
		int currentLapTime = _Stats.GetCurrentLapTime();
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			if (!StopTimer)
			{
				m_RaceTimeMs = _Stats.GetRaceTime();
			}
			TimeSpan tS = TimeSpan.FromMilliseconds(m_RaceTimeMs);
			if (!StopTimer && nbLapCompleted > 1 && TimeSpan.FromMilliseconds(currentLapTime).Seconds < 3)
			{
				tS = TimeSpan.FromMilliseconds(_Stats.GetLastLapTime());
			}
			mLabelLapTime.text = tS.FormatRaceTime();
		}
		else if (m_iCurrentIndex != rank && rank >= 0 && rank < mSpriteNames.Count)
		{
			mPositionSprite.spriteName = mSpriteNames[rank];
			mPositionSprite.MakePixelPerfect();
			m_iCurrentIndex = rank;
		}
		if (m_iCurrentLap == nbLapCompleted)
		{
			return;
		}
		if ((bool)mLabelLap)
		{
			mLabelLap.text = Localization.instance.Get("HUD_INGAME_LAP") + " : " + nbLapCompleted + " / " + raceNbLap;
		}
		m_iCurrentLap = nbLapCompleted;
		if (m_iCurrentLap >= 2 && m_iCurrentLap <= 3)
		{
			Singleton<GameManager>.Instance.GameMode.Hud.HUDFinish.ShowLap(m_iCurrentLap);
		}
		if ((bool)Singleton<GameManager>.Instance.SoundManager && m_iCurrentLap > 1)
		{
			if (m_iCurrentLap < raceNbLap)
			{
				Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.NewLap);
				return;
			}
			Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.LastLap);
			Singleton<GameManager>.Instance.GameMode.MainMusic.pitch += LastLapPitchGain;
		}
	}
}
