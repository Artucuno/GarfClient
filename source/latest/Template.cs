using System;
using UnityEngine;

[Serializable]
public class Template
{
	public ECharacter Character = ECharacter.NONE;

	public Vector3 Position = Vector3.zero;

	public Vector3 Scale = Vector3.zero;

	public Template(ECharacter pCharacter)
	{
		Character = pCharacter;
	}
}
