using System;
using UnityEngine;

public class RacePuzzlePiece : RaceItem
{
	public int Index;

	public bool m_bAlreadyTaken;

	public Material TransparentMaterial;

	protected override void Awake()
	{
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL || Network.peerType != 0)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		base.Awake();
		string startScene = Singleton<GameConfigurator>.Instance.StartScene;
		string pPiece = startScene + "_" + Index;
		m_bAlreadyTaken = Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(pPiece);
		if (m_bAlreadyTaken)
		{
			Material[] materials = base.renderer.materials;
			Material[] array = new Material[1];
			if (materials.Length == 2)
			{
				materials[0] = TransparentMaterial;
				materials[1] = TransparentMaterial;
				Array.Copy(materials, array, 1);
			}
			base.renderer.materials = array;
		}
	}

	protected override void DoTrigger(RcVehicle pVehicle)
	{
		if (m_bAlreadyTaken)
		{
			return;
		}
		base.DoTrigger(pVehicle);
		if ((bool)pVehicle && pVehicle.GetControlType() == RcVehicle.ControlType.Human)
		{
			Kart kart = (Kart)pVehicle;
			kart.TakePuzzlePiece(Index);
		}
		bool flag = true;
		for (int i = 0; i < 2; i++)
		{
			if (i != Index)
			{
				string pPiece = Singleton<GameConfigurator>.Instance.StartScene + "_" + i;
				if (!Singleton<GameSaveManager>.Instance.IsPuzzlePieceUnlocked(pPiece))
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.FullPuzzle);
		}
		else
		{
			Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.TakePuzzle);
		}
		string pPiece2 = Singleton<GameConfigurator>.Instance.StartScene + "_" + Index;
		Singleton<RewardManager>.Instance.UnlockPuzzlePiece(Index);
		Singleton<GameSaveManager>.Instance.UnlockPuzzlePiece(pPiece2, false);
	}
}
