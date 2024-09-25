using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileMotionBase : InitBase
{
	Coroutine _coLaunchProjectile;

	public Vector3 StartPosition { get; private set; }
	public Vector3 TargetPosition { get; private set; }
	public bool LookAtTarget { get; private set; }
	public Data.ProjectileData ProjectileData { get; private set; }
	protected Action EndCallback { get; private set; }

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		return true;
	}

	protected void SetInfo(int projectileTemplateID, Vector3 spawnPosition, Vector3 targetPosition, Action endCallback = null)
	{
		ProjectileData = Managers.Data.ProjectileDic[projectileTemplateID];
		StartPosition = spawnPosition;
		TargetPosition = targetPosition;
		EndCallback = endCallback;

		LookAtTarget = true; // TEMP

		if (_coLaunchProjectile != null)
			StopCoroutine(_coLaunchProjectile);

		_coLaunchProjectile = StartCoroutine(CoLaunchProjectile());
	}

	protected void LookAt2D(Vector2 forward)
	{
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
	}

	protected abstract IEnumerator CoLaunchProjectile();
}
