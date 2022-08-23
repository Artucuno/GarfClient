using UnityEngine;

public class HUDOptions : MonoBehaviour
{
	public UICheckbox BtnGyro;

	public UICheckbox BtnTouched;

	public UISlider GyroSlider;

	private float m_fGyroSensibility;

	public void Start()
	{
		m_fGyroSensibility = Singleton<GameOptionManager>.Instance.GetGyroSensibility();
		E_InputType inputType = Singleton<GameOptionManager>.Instance.GetInputType();
		if (inputType == E_InputType.Gyroscopic)
		{
			BtnGyro.isChecked = true;
			BtnTouched.isChecked = false;
		}
		else
		{
			BtnTouched.isChecked = true;
			BtnGyro.isChecked = false;
		}
		if ((bool)GyroSlider)
		{
			GyroSlider.sliderValue = m_fGyroSensibility;
		}
	}

	public void OnSelectGyro()
	{
		SelectInput(E_InputType.Gyroscopic);
	}

	public void OnSelectArrow()
	{
		SelectInput(E_InputType.Touched);
	}

	public void SelectInput(E_InputType eInput)
	{
		Singleton<GameOptionManager>.Instance.SetInputType(eInput, true);
		if (eInput == E_InputType.Gyroscopic)
		{
			BtnGyro.isChecked = true;
		}
		else
		{
			BtnTouched.isChecked = true;
		}
	}

	public void OnChangeSensibility(float fValue)
	{
		m_fGyroSensibility = fValue;
		Singleton<GameOptionManager>.Instance.SetGyroSensibility(m_fGyroSensibility, false);
	}

	public void SaveSensibility()
	{
		Singleton<GameOptionManager>.Instance.SetGyroSensibility(m_fGyroSensibility, true);
	}
}
