using UnityEngine;

public class HUDResultsChallenge : MonoBehaviour
{
	public UILabel FirstObjective;

	public UILabel SecondObjective;

	public UILabel State;

	public UILabel Summary;

	public UITexturePattern ChampionshipIcon;

	public UITexturePattern GameModeIcon;

	public UITexturePattern DifficultyIcon;

	public UITexturePattern ValidFirstObjective;

	public UITexturePattern ValidSecondObjective;

	private void Start()
	{
		string _First = string.Empty;
		string _Second = string.Empty;
		Singleton<ChallengeManager>.Instance.GetLocalizedObjectives(out _First, out _Second);
		FirstObjective.text = _First;
		SecondObjective.text = _Second;
		State.text = ((!Singleton<ChallengeManager>.Instance.Success) ? Localization.instance.Get("HUD_RESULTS_FAILED") : Localization.instance.Get("HUD_RESULTS_CONGRATS"));
		Summary.text = ((!Singleton<ChallengeManager>.Instance.Success) ? Localization.instance.Get("HUD_CHALLENGE_FAILED") : Localization.instance.Get("HUD_CHALLENGE_SUCCESS"));
		ChampionshipIcon.ChangeTexture(Singleton<GameConfigurator>.Instance.ChampionShipData.Index);
		DifficultyIcon.ChangeTexture((int)Singleton<GameConfigurator>.Instance.Difficulty);
		E_GameModeType gameMode = Singleton<ChallengeManager>.Instance.GameMode;
		int iNum = 0;
		switch (gameMode)
		{
		case E_GameModeType.TIME_TRIAL:
			iNum = 2;
			break;
		case E_GameModeType.SINGLE:
			iNum = 0;
			break;
		case E_GameModeType.CHAMPIONSHIP:
			iNum = 1;
			break;
		}
		GameModeIcon.ChangeTexture(iNum);
		ValidFirstObjective.ChangeTexture(Singleton<ChallengeManager>.Instance.SuccessFirstObjective ? 1 : 0);
		if (Singleton<ChallengeManager>.Instance.GameMode == E_GameModeType.TIME_TRIAL)
		{
			ValidSecondObjective.gameObject.SetActive(false);
		}
		else
		{
			ValidSecondObjective.ChangeTexture(Singleton<ChallengeManager>.Instance.SuccessSecondObjective ? 1 : 0);
		}
	}
}
