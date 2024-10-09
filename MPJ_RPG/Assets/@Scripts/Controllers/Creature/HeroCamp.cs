using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCamp : BaseObject
{
	Vector2 _moveDir = Vector2.zero;

	public float Speed { get; set; } = 5.0f;

	public Transform Pivot { get; private set; }
	public Transform Destination { get; private set; }

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
		Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;

		Collider.includeLayers = (1 << (int)Define.ELayer.Obstacle);
		Collider.excludeLayers = (1 << (int)Define.ELayer.Monster) | (1 << (int)Define.ELayer.Hero);

		ObjectType = Define.EObjectType.HeroCamp;

		Pivot = Util.FindChild<Transform>(gameObject, "Pivot", true);
		Destination = Util.FindChild<Transform>(gameObject, "Destination", true);

		return true;
	}

	private void Update()
	{
		Vector3 dir = _moveDir * Time.deltaTime * Speed;
		Vector3 newPos = transform.position + dir;

		if (Managers.Map == null)
			return;
		if (Managers.Map.CanGo(null, newPos, ignoreObjects: true, ignoreSemiWall: true) == false)
			return;

		transform.position = newPos;

		// Map Transition
		Managers.Map.StageTransition.CheckMapChanged(newPos);
	}

	private void HandleOnMoveDirChanged(Vector2 dir)
	{
		_moveDir = dir;

		if (dir != Vector2.zero)
		{
			float angle = Mathf.Atan2(-dir.x, +dir.y) * 180 / Mathf.PI;
			Pivot.eulerAngles = new Vector3(0, 0, angle);
		}
	}
}
