using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMotion : ProjectileMotionBase
{
	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		return true;
	}

	//new를 사용하면 수정하는 멤버가 기본 클래스에서 상속된 멤버를 숨김을 인식하고 있다고 어설션하는 것입니다
	//왜 가상함수로 SetInfo를 만들지 않는가? -> 원거리 타입에 따라 인자로 받는 종류가 달라질 수 있기 때문에 override보다 overload를 사용한다.
	public new void SetInfo(int dataTemplateID, Vector3 startPosition, Vector3 targetPosition, Action endCallback)
	{
		base.SetInfo(dataTemplateID, startPosition, targetPosition, endCallback);
	}

	protected override IEnumerator CoLaunchProjectile()
	{
		float journeyLength = Vector3.Distance(StartPosition, TargetPosition);
		float totalTime = journeyLength / ProjectileData.ProjSpeed;
		float elapsedTime = 0;

		while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;

			float normalizedTime = elapsedTime / totalTime;
			transform.position = Vector3.Lerp(StartPosition, TargetPosition, normalizedTime);

			if (LookAtTarget)
				LookAt2D(TargetPosition - transform.position);

			yield return null;
		}

		transform.position = TargetPosition;
		EndCallback?.Invoke();
	}
}
