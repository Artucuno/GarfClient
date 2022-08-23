using System;
using UnityEngine;

public class GkRacingAI : RacingAI
{
	public delegate void ManageDelegate(bool bCheckBonusBehind, bool bCheckBonusForward, bool bCheckForward, bool bCheckBehind, float fBonusDistance, float fVehicleForwardDistance, float fVehicleBehindDistance, float fItemColRadius, float fItemSpeed);

	private float _bonusDefenseDelay;

	private float _bonusAttackDelay;

	private float _bonusDelayMax;

	private float _bonusTimer;

	private float _bonusMaxTimer;

	private float _tryDetachDiamondTimer;

	private float _pieBeforeDistance;

	private float _pieBehindDistance;

	private float _diamondBeforeDistance;

	private float _diamondBehindDistance;

	private float _defenseZoneRadius;

	private float _magicDistance;

	private float _parfumeDistance;

	private float _shootChance;

	private int _layerMaskVehicle = 1 << LayerMask.NameToLayer("Vehicle");

	private int _layerMaskGlobal = 1;

	private bool m_bCanAttack;

	private bool m_bKeepItem;

	private float _keepItemChance;

	public Action<GkRacingAI> OnNeedPath;

	private float _RatioItemSphereCollider = 1.3f;

	private bool _wantDetachDiamond;

	private bool _DontUseItem;

	public ManageDelegate Manage;

	public override void Init()
	{
		base.Init();
		_bonusTimer = 0f;
		_bonusMaxTimer = 0f;
		BehaviourSettings behaviourSettings = Singleton<GameConfigurator>.Instance.AISettings.BehaviourSettings;
		switch (Singleton<GameConfigurator>.Instance.Difficulty)
		{
		case EDifficulty.EASY:
			_bonusDefenseDelay = behaviourSettings.EasyBonusDelay;
			break;
		case EDifficulty.NORMAL:
			_bonusDefenseDelay = behaviourSettings.NormalBonusDelay;
			break;
		case EDifficulty.HARD:
			_bonusDefenseDelay = behaviourSettings.HardBonusDelay;
			break;
		}
		_bonusDelayMax = behaviourSettings.BonusDelayMax + UnityEngine.Random.Range(behaviourSettings.BonusDelayBeforeUseMin, behaviourSettings.BonusDelayBeforeUseMax);
		_bonusAttackDelay = UnityEngine.Random.Range(behaviourSettings.BonusDelayBeforeUseMin, behaviourSettings.BonusDelayBeforeUseMax);
		_pieBeforeDistance = behaviourSettings.PieBeforeDistance;
		_pieBehindDistance = behaviourSettings.PieBehindDistance;
		_diamondBeforeDistance = behaviourSettings.DiamondBeforeDistance;
		_diamondBehindDistance = behaviourSettings.DiamondBehindDistance;
		_parfumeDistance = behaviourSettings.ParfumeDistance;
		_magicDistance = behaviourSettings.MagicDistance;
		_defenseZoneRadius = behaviourSettings.DefenseZoneRadius;
		_shootChance = behaviourSettings.ShootChance.GetChance(_level);
		_keepItemChance = behaviourSettings.KeepItemChance;
		_RatioItemSphereCollider = behaviourSettings.RatioSphereCollider;
		_tryDetachDiamondTimer = 0f;
		_wantDetachDiamond = false;
		_DontUseItem = false;
		Kart obj = (Kart)base.Vehicle;
		obj.OnHit = (Action)Delegate.Combine(obj.OnHit, new Action(base.Reset));
		if (_level == E_AILevel.BAD)
		{
			Manage = ManageAI;
		}
		else
		{
			Manage = ManageAI;
		}
		_layerMaskGlobal = _layerMaskVehicle;
	}

	public override void StartAntiJam()
	{
		base.StartAntiJam();
		if (OnNeedPath != null)
		{
			RcFastValuePath rcFastValuePath = (RcFastValuePath)base.CurrentPath;
			if (rcFastValuePath.PathType == E_PathType.SHORTCUT || rcFastValuePath.PathType == E_PathType.NONE)
			{
				OnNeedPath(this);
				_jamTime = 0f;
				_reverseTime = 0f;
				_antiJamTime = 0f;
				Reset();
			}
		}
	}

	public override void Update()
	{
		base.Update();
		Kart kart = (Kart)base.Vehicle;
		EITEM item = kart.GetBonusMgr().GetItem(0);
		if (item != 0 && Singleton<GameManager>.Instance.GameMode.State != E_GameState.Podium)
		{
			_bonusTimer += Time.deltaTime;
			if (!m_bKeepItem)
			{
				_bonusMaxTimer += Time.deltaTime;
			}
			else if (kart.GetBonusMgr().GetItem(1) != 0)
			{
				m_bKeepItem = false;
			}
			if (_bonusTimer > _bonusDefenseDelay)
			{
				m_bCanAttack = ((_bonusMaxTimer > _bonusAttackDelay) ? true : false);
				_bonusTimer -= _bonusDefenseDelay;
				switch (item)
				{
				case EITEM.ITEM_UFO:
				case EITEM.ITEM_NAP:
				{
					int num = Singleton<RandomManager>.Instance.Next(0, 100);
					if ((float)num < _shootChance && (item == EITEM.ITEM_NAP || (item == EITEM.ITEM_UFO && kart.RaceStats.GetRank() != 0)))
					{
						ActivateBonus(kart, false);
					}
					break;
				}
				case EITEM.ITEM_LASAGNA:
					if (kart.HasDiamondAttached && (float)Singleton<RandomManager>.Instance.Next(0, 100) < _shootChance)
					{
						ActivateBonus(kart, false);
					}
					break;
				case EITEM.ITEM_DIAMOND:
					Manage(true, false, true, true, _defenseZoneRadius, _diamondBeforeDistance, _diamondBehindDistance, Singleton<BonusMgr>.Instance.m_pDiamondEntities[0].MinDistance * _RatioItemSphereCollider, Singleton<GameConfigurator>.Instance.AISettings.BehaviourSettings.DiamondBeforeDistance / (Singleton<BonusMgr>.Instance.m_pDiamondEntities[0].LifeTime * 0.5f));
					break;
				case EITEM.ITEM_PIE:
					Manage(true, true, true, true, _defenseZoneRadius, _pieBeforeDistance, _pieBehindDistance, Singleton<BonusMgr>.Instance.m_pPieEntities[0].GetComponent<SphereCollider>().radius * _RatioItemSphereCollider, Singleton<BonusMgr>.Instance.m_pPieEntities[0].SpeedForward / 3.6f);
					break;
				case EITEM.ITEM_AUTOLOCK_PIE:
					if (kart.RaceStats.GetRank() == 0)
					{
						Manage(true, true, false, true, _defenseZoneRadius, 0f, _pieBehindDistance, Singleton<BonusMgr>.Instance.m_pAutolockPieEntities[0].GetComponent<SphereCollider>().radius * _RatioItemSphereCollider, Singleton<BonusMgr>.Instance.m_pAutolockPieEntities[0].SpeedForward / 3.6f);
					}
					else
					{
						Manage(true, true, true, true, _defenseZoneRadius, 0f, _pieBehindDistance, Singleton<BonusMgr>.Instance.m_pAutolockPieEntities[0].GetComponent<SphereCollider>().radius * _RatioItemSphereCollider, Singleton<BonusMgr>.Instance.m_pAutolockPieEntities[0].SpeedForward / 3.6f);
					}
					break;
				case EITEM.ITEM_MAGIC:
					Manage(false, false, true, false, 0f, _magicDistance, 0f, Singleton<BonusMgr>.Instance.m_pMagicEntities[0].GetComponent<SphereCollider>().radius * _RatioItemSphereCollider, Singleton<BonusMgr>.Instance.m_pMagicEntities[0].Speed / 3.6f);
					break;
				case EITEM.ITEM_PARFUME:
					if (kart.GetBonusMgr().GetBonusValue(EITEM.ITEM_PARFUME, EBonusCustomEffect.REPULSE) != 0f)
					{
						if (!m_bCanAttack)
						{
							break;
						}
						if ((float)Singleton<RandomManager>.Instance.Next(0, 100) < _shootChance)
						{
							Kart kart2 = (Kart)kart.RaceStats.GetPreceding();
							if (kart2 != null && Mathf.Abs(kart2.RaceStats.GetDistToEndOfRace() - kart.RaceStats.GetDistToEndOfRace()) < _defenseZoneRadius * 3f)
							{
								ActivateBonus(kart, false);
								break;
							}
							kart2 = (Kart)kart.RaceStats.GetPursuant();
							if (kart2 != null && Mathf.Abs(kart2.RaceStats.GetDistToEndOfRace() - kart.RaceStats.GetDistToEndOfRace()) < _defenseZoneRadius * 3f)
							{
								ActivateBonus(kart, false);
							}
						}
						else
						{
							BehaviourSettings behaviourSettings = Singleton<GameConfigurator>.Instance.AISettings.BehaviourSettings;
							_bonusAttackDelay += UnityEngine.Random.Range(behaviourSettings.BonusDelayBeforeUseMin, behaviourSettings.BonusDelayBeforeUseMax);
						}
						break;
					}
					if (kart.OnUfoCatchMe == null)
					{
						kart.OnUfoCatchMe = (Action<Kart>)Delegate.Combine(kart.OnUfoCatchMe, new Action<Kart>(UseParfume));
					}
					if (kart.IsSleeping() && (float)Singleton<RandomManager>.Instance.Next(0, 100) < _shootChance && !_DontUseItem)
					{
						ActivateBonus(kart, false);
						break;
					}
					if (kart.HasDiamondAttached && (float)Singleton<RandomManager>.Instance.Next(0, 100) < _shootChance && !_DontUseItem)
					{
						ActivateBonus(kart, false);
						break;
					}
					if (kart.HasDiamondAttached)
					{
						_DontUseItem = true;
					}
					Manage(true, true, false, false, _defenseZoneRadius, 0f, 0f, 0f, 0f);
					break;
				case EITEM.ITEM_SPRING:
					if ((kart.HasDiamondAttached || kart.HasNapEffect()) && (float)Singleton<RandomManager>.Instance.Next(0, 100) < _shootChance && !_DontUseItem)
					{
						ActivateBonus(kart, false);
						break;
					}
					if (kart.HasDiamondAttached || kart.HasNapEffect())
					{
						_DontUseItem = true;
					}
					Manage(true, true, false, false, _defenseZoneRadius, 0f, 0f, 0f, 0f);
					break;
				}
			}
			else
			{
				m_bKeepItem = (((float)Singleton<RandomManager>.Instance.Next(0, 100) < _keepItemChance) ? true : false);
			}
			if (_bonusMaxTimer > _bonusDelayMax)
			{
				_bonusMaxTimer = 0f;
				ForceThrow(item);
			}
		}
		if (kart.HasDiamondAttached)
		{
			if (_tryDetachDiamondTimer > 0f)
			{
				_tryDetachDiamondTimer -= Time.deltaTime;
			}
			if (!(_tryDetachDiamondTimer <= 0f))
			{
				return;
			}
			if (!_wantDetachDiamond)
			{
				if ((float)Singleton<RandomManager>.Instance.Next(0, 100) < _shootChance)
				{
					_wantDetachDiamond = true;
				}
				else
				{
					_tryDetachDiamondTimer = _bonusDefenseDelay * 10f;
				}
			}
			else if (kart.IsOnGround())
			{
				kart.Jump(0f, 0f);
				_tryDetachDiamondTimer = 0.5f;
			}
		}
		else
		{
			_wantDetachDiamond = false;
		}
	}

	public void ForceThrow(EITEM pItem)
	{
		switch (pItem)
		{
		case EITEM.ITEM_PIE:
		case EITEM.ITEM_AUTOLOCK_PIE:
		case EITEM.ITEM_SPRING:
		case EITEM.ITEM_DIAMOND:
		case EITEM.ITEM_PARFUME:
			ActivateBonus((Kart)base.Vehicle, true);
			break;
		case EITEM.ITEM_LASAGNA:
		case EITEM.ITEM_NAP:
		case EITEM.ITEM_MAGIC:
			ActivateBonus((Kart)base.Vehicle, false);
			break;
		case EITEM.ITEM_UFO:
			break;
		}
	}

	public void ActivateBonus(Kart pKart, bool pBehind)
	{
		BehaviourSettings behaviourSettings = Singleton<GameConfigurator>.Instance.AISettings.BehaviourSettings;
		pKart.GetBonusMgr().ActivateBonus(pBehind);
		_bonusTimer = 0f;
		_bonusMaxTimer = 0f;
		_bonusAttackDelay = UnityEngine.Random.Range(behaviourSettings.BonusDelayBeforeUseMin, behaviourSettings.BonusDelayBeforeUseMax);
		_bonusDelayMax = behaviourSettings.BonusDelayMax + UnityEngine.Random.Range(behaviourSettings.BonusDelayBeforeUseMin, behaviourSettings.BonusDelayBeforeUseMax);
		m_bKeepItem = false;
		_DontUseItem = false;
		if (pKart.OnUfoCatchMe != null)
		{
			pKart.OnUfoCatchMe = (Action<Kart>)Delegate.Remove(pKart.OnUfoCatchMe, new Action<Kart>(UseParfume));
		}
	}

	public override void DebugDrawGizmos()
	{
		Kart kart = (Kart)base.Vehicle;
		Vector3 forward = kart.transform.parent.forward;
		forward.Normalize();
		float num = kart.transform.parent.GetComponent<SphereCollider>().radius + 0.2f;
		Vector3 position = kart.GetPosition();
		Debug.DrawLine(position, position + forward * 10f, Color.white);
		Debug.DrawLine(position, position + kart.GetCameraAt() * 10f, Color.green);
	}

	public void ManageAI(bool bCheckBonusBehind, bool bCheckBonusForward, bool bCheckForward, bool bCheckBehind, float fBonusDistance, float fVehicleForwardDistance, float fVehicleBehindDistance, float fItemColRadius, float fItemSpeed)
	{
		if ((float)Singleton<RandomManager>.Instance.Next(0, 100) > _shootChance)
		{
			return;
		}
		Kart kart = (Kart)base.Vehicle;
		Vector3 launchHorizontalDirection = kart.LaunchHorizontalDirection;
		Vector2 vector = new Vector2(launchHorizontalDirection.x, launchHorizontalDirection.z);
		Vector2 vector2 = new Vector2(kart.transform.position.x, kart.transform.position.z);
		float radius = kart.transform.parent.GetComponent<SphereCollider>().radius;
		fBonusDistance *= fBonusDistance;
		fVehicleForwardDistance *= fVehicleForwardDistance;
		fVehicleBehindDistance *= fVehicleBehindDistance;
		if (bCheckBonusBehind)
		{
			AutolockPieBonusEntity[] pAutolockPieEntities = Singleton<BonusMgr>.Instance.m_pAutolockPieEntities;
			foreach (AutolockPieBonusEntity autolockPieBonusEntity in pAutolockPieEntities)
			{
				if (!autolockPieBonusEntity.Activate || autolockPieBonusEntity.IsStatic())
				{
					continue;
				}
				Vector2 rhs = vector2 - autolockPieBonusEntity.GetFlatPosition();
				if (rhs.sqrMagnitude < fBonusDistance && Vector2.Dot(vector, rhs) > 0f)
				{
					bool pBehind = true;
					if (fItemColRadius == 0f)
					{
						pBehind = false;
					}
					ActivateBonus(kart, pBehind);
					return;
				}
			}
			PieBonusEntity[] pPieEntities = Singleton<BonusMgr>.Instance.m_pPieEntities;
			foreach (PieBonusEntity pieBonusEntity in pPieEntities)
			{
				if (!pieBonusEntity.Activate || pieBonusEntity.IsStatic())
				{
					continue;
				}
				Vector2 rhs2 = vector2 - pieBonusEntity.GetFlatPosition();
				if (!(rhs2.sqrMagnitude < fBonusDistance))
				{
					continue;
				}
				if (fItemColRadius == 0f)
				{
					if (Vector2.Dot(rhs2.normalized, pieBonusEntity.GetFlatVelocity().normalized) > 0.707f)
					{
						ActivateBonus(kart, false);
						return;
					}
				}
				else if (Vector2.Dot(vector, pieBonusEntity.GetFlatVelocity()) / pieBonusEntity.GetFlatVelocity().magnitude > 0.707f && Vector2.Dot(vector, rhs2) / rhs2.magnitude > 0.707f)
				{
					ActivateBonus(kart, true);
					return;
				}
			}
		}
		if (bCheckBonusForward)
		{
			BonusEntity[][] array = new BonusEntity[4][]
			{
				Singleton<BonusMgr>.Instance.m_pAutolockPieEntities,
				Singleton<BonusMgr>.Instance.m_pPieEntities,
				Singleton<BonusMgr>.Instance.m_pDiamondEntities,
				Singleton<BonusMgr>.Instance.m_pSpringEntities
			};
			BonusEntity[][] array2 = array;
			foreach (BonusEntity[] array3 in array2)
			{
				float radius2 = array3[0].GetComponent<SphereCollider>().radius;
				BonusEntity[] array4 = array3;
				foreach (BonusEntity bonusEntity in array4)
				{
					if (!bonusEntity.Activate || !bonusEntity.IsStatic())
					{
						continue;
					}
					Vector2 rhs3 = bonusEntity.GetFlatPosition() - vector2;
					if (!(rhs3.sqrMagnitude < fBonusDistance))
					{
						continue;
					}
					float num = Vector2.Dot(vector, rhs3);
					float num2 = radius + radius2;
					if (num > 0f && rhs3.sqrMagnitude - num * num < num2 * num2)
					{
						num2 = radius2 + fItemColRadius;
						if (fItemColRadius == 0f || rhs3.sqrMagnitude - num * num < num2 * num2)
						{
							ActivateBonus(kart, false);
							return;
						}
					}
				}
			}
		}
		if (!m_bCanAttack)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		GameMode gameMode = Singleton<GameManager>.Instance.GameMode;
		int playerCount = gameMode.PlayerCount;
		int rank = kart.RaceStats.GetRank();
		if (bCheckForward && fVehicleForwardDistance == 0f)
		{
			ActivateBonus(kart, false);
		}
		for (int m = 0; m < playerCount; m++)
		{
			if (m == kart.Index)
			{
				continue;
			}
			Kart kart2 = gameMode.GetKart(m);
			if (!kart2.gameObject.activeSelf || !kart2.enabled)
			{
				continue;
			}
			RcVehiclePhysic vehiclePhysic = kart2.GetVehiclePhysic();
			Vector2 vector3 = new Vector2(vehiclePhysic.GetLinearVelocity().x, vehiclePhysic.GetLinearVelocity().z);
			float magnitude = vector3.magnitude;
			if (magnitude != 0f)
			{
				vector3 /= magnitude;
			}
			else
			{
				Vector2 vector4 = new Vector2(kart2.transform.forward.x, kart2.transform.forward.z);
				vector3 = vector4.normalized;
			}
			Vector2 vector5 = new Vector2(kart2.transform.position.x, kart2.transform.position.z);
			Vector2 lhs = new Vector2(vector3.y, 0f - vector3.x);
			float num3 = Vector2.SqrMagnitude(vector2 - vector5);
			if (bCheckForward && kart2.RaceStats.GetRank() < rank && num3 < fVehicleForwardDistance)
			{
				float num4 = Vector2.Dot(lhs, vector * fItemSpeed);
				if (num4 != 0f)
				{
					float num5 = Vector2.Dot(lhs, vector5 - vector2) / num4;
					Vector2 lhs2 = vector * fItemSpeed * num5;
					Vector2 rhs4 = vector5 + vector3 * magnitude * num5 - vector2;
					float num6 = Vector2.Dot(lhs2, rhs4);
					float num7 = rhs4.sqrMagnitude - num6 * num6 / lhs2.sqrMagnitude;
					float num8 = fItemColRadius + radius;
					if (num7 < num8 * num8)
					{
						flag2 = true;
						bCheckBehind = false;
						m = playerCount;
					}
				}
			}
			if (!bCheckBehind || flag || kart2.RaceStats.GetNbLapCompleted() < kart.RaceStats.GetNbLapCompleted() || !(num3 < fVehicleBehindDistance))
			{
				continue;
			}
			Vector2 rhs5 = vector2 - vector5;
			if (Vector2.Dot(vector, rhs5) > 0f)
			{
				float num9 = Vector2.Dot(vector3, rhs5);
				float num10 = fItemColRadius + radius;
				if (num9 > 0f && rhs5.sqrMagnitude - num9 * num9 < num10 * num10)
				{
					flag = true;
				}
			}
		}
		if (flag2)
		{
			ActivateBonus(kart, false);
		}
		else if (flag)
		{
			ActivateBonus(kart, true);
		}
	}

	public void UseParfume(Kart pKart)
	{
		if ((float)Singleton<RandomManager>.Instance.Next(0, 100) < _shootChance)
		{
			ActivateBonus(pKart, false);
		}
	}
}
