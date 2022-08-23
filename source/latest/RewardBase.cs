using UnityEngine;

public abstract class RewardBase : MonoBehaviour
{
	public E_ConditionOperator ConditionOperator;

	public void GiveReward()
	{
		RewardConditionBase[] components = base.gameObject.GetComponents<RewardConditionBase>();
		if (components.Length <= 0)
		{
			return;
		}
		bool flag = components[0].CanGiveReward();
		if (ConditionOperator == E_ConditionOperator.AND)
		{
			for (int i = 1; i < components.Length; i++)
			{
				flag = flag && components[i].CanGiveReward();
			}
		}
		else
		{
			for (int j = 1; j < components.Length; j++)
			{
				flag = flag || components[j].CanGiveReward();
			}
		}
		if (flag)
		{
			GetReward();
		}
	}

	protected abstract void GetReward();
}
