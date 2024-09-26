using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitBase : MonoBehaviour
{
	protected bool _init = false;

	public virtual bool Init()
	{
		if (_init)
			return false;

		_init = true;
		return true;
	}

	private void Awake()
	{
		Init();
	}
}
