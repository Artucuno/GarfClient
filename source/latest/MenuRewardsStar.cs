using UnityEngine;

public class MenuRewardsStar : MenuRewards
{
	public static int StarCount = 5;

	public string MainKey;

	public string[] StarTypeKeys = new string[StarCount];

	public string[] StarNumberKeys = new string[StarCount];

	public GameObject[] Stars = new GameObject[StarCount];

	public override void OnEnter()
	{
		base.OnEnter();
		int num = (int)Singleton<RewardManager>.Instance.PopStar();
		string format = Localization.instance.Get(MainKey);
		string arg = Localization.instance.Get(StarTypeKeys[num]);
		int num2 = Singleton<GameConfigurator>.Instance.PlayerConfig.NbStars - 1;
		string arg2 = Localization.instance.Get(StarNumberKeys[num2]);
		for (int i = 0; i < StarCount; i++)
		{
			if ((bool)Stars[i])
			{
				Stars[i].SetActive(i == num2);
			}
		}
		LbMessage.text = string.Format(format, arg, arg2);
	}
}
