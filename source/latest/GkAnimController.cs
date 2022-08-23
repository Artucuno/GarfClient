using System;
using UnityEngine;

[Serializable]
public class GkAnimController
{
	private Animator _animator;

	private int _bonusParameter;

	private int _bonusState;

	private bool _canStop;

	public Animator Animator
	{
		get
		{
			return _animator;
		}
	}

	public GkAnimController(Animator pAnimator)
	{
		_animator = pAnimator;
		_bonusParameter = -1;
		_bonusState = -1;
		_canStop = true;
	}

	public void Update()
	{
		AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		if (_bonusState != -1 && currentAnimatorStateInfo.nameHash == _bonusState && _canStop && !_animator.IsInTransition(0))
		{
			_animator.SetBool(_bonusParameter, false);
			_bonusParameter = -1;
			_bonusState = -1;
		}
	}

	public void Play(int pAnimParameter, int pAnimState, bool pCanStop)
	{
		if (_bonusState != -1 && _bonusParameter != -1)
		{
			_animator.SetBool(_bonusParameter, false);
		}
		_bonusParameter = pAnimParameter;
		_bonusState = pAnimState;
		_canStop = pCanStop;
		_animator.SetBool(pAnimParameter, true);
	}

	public void Stop(int pAnimState)
	{
		if (_bonusState == pAnimState)
		{
			_canStop = true;
		}
	}

	public void ForceStop(int pAnimParameter)
	{
		_animator.SetBool(pAnimParameter, false);
	}

	public void StopAll()
	{
		_canStop = true;
	}
}
