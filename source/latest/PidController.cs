using System;
using UnityEngine;

[Serializable]
public class PidController
{
	public const int NUM_ERROR_SLOTS = 40;

	public float IntegralDuration = 0.5f;

	private float[] _error = new float[40];

	private float[] _timeStep = new float[40];

	private int _currentIndex;

	private int _previousIndex;

	private int _numErrorsRecorded;

	private int _lastIndexIntegral;

	private float _currentIntegral;

	private float _errorSum;

	private float _timeSum;

	private BehaviourSettings pBehaviourSetting;

	public float PCoefficient
	{
		get
		{
			return pBehaviourSetting.PCoefficient;
		}
	}

	public float ICoefficient
	{
		get
		{
			return pBehaviourSetting.ICoefficient;
		}
	}

	public float DCoefficient
	{
		get
		{
			return pBehaviourSetting.DCoefficient;
		}
	}

	public float Error
	{
		get
		{
			if (_numErrorsRecorded >= 1)
			{
				return _error[_currentIndex];
			}
			return 0f;
		}
	}

	public float ErrorIntegral
	{
		get
		{
			return _currentIntegral;
		}
	}

	public PidController()
	{
		Clear();
		pBehaviourSetting = Singleton<GameConfigurator>.Instance.AISettings.BehaviourSettings;
	}

	public void Record(float pError, float pTimeStep)
	{
		if (!(pTimeStep <= 0f))
		{
			_previousIndex = _currentIndex;
			_currentIndex = (_currentIndex + 1) % 40;
			float num = _error[_currentIndex];
			float num2 = _timeStep[_currentIndex];
			_error[_currentIndex] = pError;
			_timeStep[_currentIndex] = pTimeStep;
			_numErrorsRecorded = Mathf.Min(_numErrorsRecorded + 1, 40);
			if (_currentIndex == _lastIndexIntegral)
			{
				_errorSum -= num * num2;
				_timeSum -= num2;
				_lastIndexIntegral = (_lastIndexIntegral + 1) % 40;
			}
			_errorSum += pError * pTimeStep;
			_timeSum += pTimeStep;
			while (_timeSum > IntegralDuration)
			{
				_errorSum -= _error[_lastIndexIntegral] * _timeStep[_lastIndexIntegral];
				_timeSum -= _timeStep[_lastIndexIntegral];
				_lastIndexIntegral = (_lastIndexIntegral + 1) % 40;
			}
			int num3 = (_lastIndexIntegral + 40 - 1) % 40;
			if (num3 != _currentIndex)
			{
				_currentIntegral = _error[num3] * (IntegralDuration - _timeSum) + _errorSum;
			}
			else
			{
				_currentIntegral = _errorSum * IntegralDuration / _timeSum;
			}
		}
	}

	public void Clear()
	{
		_currentIndex = 0;
		_previousIndex = 0;
		_numErrorsRecorded = 0;
		_currentIntegral = 0f;
		_errorSum = 0f;
		_timeSum = 0f;
		_lastIndexIntegral = _currentIndex;
		for (int i = 0; i < 40; i++)
		{
			_error[i] = 0f;
			_timeStep[i] = 0f;
		}
	}

	public void ResetErrorSum()
	{
		_errorSum = 0f;
	}

	public float GetErrorDerivative()
	{
		if (_numErrorsRecorded >= 2)
		{
			float num = _error[_currentIndex] - _error[_previousIndex];
			float num2 = _timeStep[_currentIndex];
			if (num2 > 0.001f)
			{
				return num / num2;
			}
			return 999999f;
		}
		return 0f;
	}

	public float GetOutput()
	{
		if (pBehaviourSetting == null)
		{
			return Error;
		}
		return PCoefficient * Error + ICoefficient * ErrorIntegral + DCoefficient * GetErrorDerivative();
	}
}
