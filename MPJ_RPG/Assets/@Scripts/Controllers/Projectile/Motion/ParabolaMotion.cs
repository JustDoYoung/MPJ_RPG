using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaMotion : ProjectileMotionBase
{
	public float HeightArc { get; protected set; } = 2;

	public new void SetInfo(int dataTemplateID, Vector3 startPosition, Vector3 targetPosition, Action endCallback)
	{
		base.SetInfo(dataTemplateID, startPosition, targetPosition, endCallback);
	}

	protected override IEnumerator CoLaunchProjectile()
	{
		float journeyLength = Vector2.Distance(StartPosition, TargetPosition);
		float totalTime = journeyLength / ProjectileData.ProjSpeed;
		float elapsedTime = 0;

		while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;

			float normalizedTime = elapsedTime / totalTime;

			// 포물선 모양으로 이동
			float x = Mathf.Lerp(StartPosition.x, TargetPosition.x, normalizedTime);
			float baseY = Mathf.Lerp(StartPosition.y, TargetPosition.y, normalizedTime);
			float arc = HeightArc * Mathf.Sin(normalizedTime * Mathf.PI);

			float y = baseY + arc;

			var nextPos = new Vector3(x, y);

			if (LookAtTarget)
				LookAt2D(nextPos - transform.position);

			transform.position = nextPos;

			yield return null;
		}

		EndCallback?.Invoke();
	}
}
