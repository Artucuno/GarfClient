using System;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class BotControlScript : MonoBehaviour
{
	[NonSerialized]
	public float lookWeight;

	[NonSerialized]
	public Transform enemy;

	public float animSpeed = 1.5f;

	public float lookSmoother = 3f;

	public bool useCurves;

	private Animator anim;

	private AnimatorStateInfo currentBaseState;

	private AnimatorStateInfo layer2CurrentState;

	private CapsuleCollider col;

	private static int idleState = Animator.StringToHash("Base Layer.Idle");

	private static int locoState = Animator.StringToHash("Base Layer.Locomotion");

	private static int jumpState = Animator.StringToHash("Base Layer.Jump");

	private static int jumpDownState = Animator.StringToHash("Base Layer.JumpDown");

	private static int fallState = Animator.StringToHash("Base Layer.Fall");

	private static int rollState = Animator.StringToHash("Base Layer.Roll");

	private static int waveState = Animator.StringToHash("Layer2.Wave");

	private void Start()
	{
		anim = GetComponent<Animator>();
		col = GetComponent<CapsuleCollider>();
		enemy = GameObject.Find("Enemy").transform;
		if (anim.layerCount == 2)
		{
			anim.SetLayerWeight(1, 1f);
		}
	}

	private void FixedUpdate()
	{
		float axis = Input.GetAxis("Horizontal");
		float axis2 = Input.GetAxis("Vertical");
		anim.SetFloat("Speed", axis2);
		anim.SetFloat("Direction", axis);
		anim.speed = animSpeed;
		anim.SetLookAtWeight(lookWeight);
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
		if (anim.layerCount == 2)
		{
			layer2CurrentState = anim.GetCurrentAnimatorStateInfo(1);
		}
		if (Input.GetButton("Fire2"))
		{
			anim.SetLookAtPosition(enemy.position);
			lookWeight = Mathf.Lerp(lookWeight, 1f, Time.deltaTime * lookSmoother);
		}
		else
		{
			lookWeight = Mathf.Lerp(lookWeight, 0f, Time.deltaTime * lookSmoother);
		}
		if (currentBaseState.nameHash == locoState)
		{
			if (Input.GetButtonDown("Jump"))
			{
				anim.SetBool("Jump", true);
			}
		}
		else if (currentBaseState.nameHash == jumpState)
		{
			if (!anim.IsInTransition(0))
			{
				if (useCurves)
				{
					col.height = anim.GetFloat("ColliderHeight");
				}
				anim.SetBool("Jump", false);
			}
			Ray ray = new Ray(base.transform.position + Vector3.up, -Vector3.up);
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Raycast(ray, out hitInfo) && hitInfo.distance > 1.75f)
			{
				anim.MatchTarget(hitInfo.point, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(0f, 1f, 0f), 0f), 0.35f, 0.5f);
			}
		}
		else if (currentBaseState.nameHash == jumpDownState)
		{
			col.center = new Vector3(0f, anim.GetFloat("ColliderY"), 0f);
		}
		else if (currentBaseState.nameHash == fallState)
		{
			col.height = anim.GetFloat("ColliderHeight");
		}
		else if (currentBaseState.nameHash == rollState)
		{
			if (!anim.IsInTransition(0))
			{
				if (useCurves)
				{
					col.height = anim.GetFloat("ColliderHeight");
				}
				col.center = new Vector3(0f, anim.GetFloat("ColliderY"), 0f);
			}
		}
		else if (currentBaseState.nameHash == idleState && Input.GetButtonUp("Jump"))
		{
			anim.SetBool("Wave", true);
		}
		if (layer2CurrentState.nameHash == waveState)
		{
			anim.SetBool("Wave", false);
		}
	}
}
