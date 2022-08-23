using UnityEngine;

public class MenuOptionInput : AbstractMenu
{
	public UICheckbox m_pBtnGyro;

	public UICheckbox m_pBtnTouched;

	public UISlider m_pGyroSlider;

	private float m_fGyroSensibility;

	public override void OnEnter()
	{
		base.OnEnter();
		m_fGyroSensibility = Singleton<GameOptionManager>.Instance.GetGyroSensibility();
		E_InputType inputType = Singleton<GameOptionManager>.Instance.GetInputType();
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (inputType == E_InputType.Gyroscopic)
			{
				m_pBtnGyro.isChecked = true;
				m_pBtnTouched.isChecked = false;
			}
			else
			{
				m_pBtnTouched.isChecked = true;
				m_pBtnGyro.isChecked = false;
			}
			if ((bool)m_pGyroSlider)
			{
				m_pGyroSlider.sliderValue = m_fGyroSensibility;
			}
		}
	}

	public void OnSelectInput(int iInput)
	{
		Singleton<GameOptionManager>.Instance.SetInputType((E_InputType)iInput, true);
	}

	public void OnChangeSensibility(float fValue)
	{
		m_fGyroSensibility = fValue;
		Singleton<GameOptionManager>.Instance.SetGyroSensibility(m_fGyroSensibility, false);
	}

	public override void OnExit()
	{
		Singleton<GameOptionManager>.Instance.SetGyroSensibility(m_fGyroSensibility, true);
		base.OnExit();
	}
}
