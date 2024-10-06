using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stun : CCBase
{
	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		EffectType = Define.EEffectType.CrowdControl;
		return true;
	}

}