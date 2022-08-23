using UnityEngine;

public class KartHumanController : RcHumanController
{
	public float Influence;

	private int m_iLogJump;

	public int jumpBoost1;

	public int jumpBoost2;

	public bool jumpBoostToggle = true;

	public int LogJump
	{
		get
		{
			return m_iLogJump;
		}
	}

	public override void Start()
	{
		base.Start();
	}

	public void Update()
	{
		if (GetVehicle().GetControlType() != 0 || Time.timeScale == 0f || GetVehicle().IsAutoPilot())
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
        {
			if (jumpBoostToggle == false)
            {
				jumpBoostToggle = true;
            }
            else
            {
				jumpBoostToggle = false;
            }
        }
		if (jumpBoostToggle == false)
		{
			jumpBoost1 = 0;
			jumpBoost2 = 0;
			jumpBoostToggle = true;
		}
		else
		{
			jumpBoost1 = 10;
			jumpBoost2 = 20;
			jumpBoostToggle = false;
		}
		if (GetKart().IsOnGround() && !GetKart().IsLocked())
		{
			float action = Singleton<InputManager>.Instance.GetAction(EAction.DriftJump);
			if (action != 0f && GetKart().Jump(jumpBoost1, jumpBoost2) && LogManager.Instance != null)
			{
				m_iLogJump++;
			}
		}
		if (Singleton<InputManager>.Instance.GetAction(EAction.LaunchBonus) == 1f)
		{
			GetKart().GetBonusMgr().ActivateBonus(false);
		}
		else if (Singleton<InputManager>.Instance.GetAction(EAction.DropBonus) == 1f)
		{
			GetKart().GetBonusMgr().ActivateBonus(true);
		}
	}

	public override void Turn(float _Steer)
	{
		base.Turn(_Steer + Influence);
	}

	public Kart GetKart()
	{
		return (Kart)m_pVehicle;
	}
}
