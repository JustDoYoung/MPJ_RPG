using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseObject : InitBase
{
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public  CircleCollider2D Collider { get; private set; }
    public SkeletonAnimation SkeletonAnim { get; private set; }
    public Rigidbody2D Rigidbody { get; private set; }

	/// <summary>
	/// (병합연산자) public float ColliderRadius { get { return Collider != null ? Collider.radius : 0.0f; } }
	/// </summary>
	public float ColliderRadius { get { return Collider?.radius ?? 0.0f; } }

	bool _lookLeft = true;
	public bool LookLeft
    {
        get { return _lookLeft; }
        set
        {
			_lookLeft = value;
			Flip(!value);
        }
    }

    public override bool Init()
    {
        if (base.Init() == false) return false;

        Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
        SkeletonAnim = GetComponent<SkeletonAnimation>();
        Rigidbody = GetComponent<Rigidbody2D>();

        return true;
    }

	#region Spine
	protected virtual void UpdateAnimation()
	{

	}

	public void PlayAnimation(int trackIndex, string AnimName, bool loop)
	{
		if (SkeletonAnim == null)
			return;

		SkeletonAnim.AnimationState.SetAnimation(trackIndex, AnimName, loop);
	}

	public void AddAnimation(int trackIndex, string AnimName, bool loop, float delay)
	{
		if (SkeletonAnim == null)
			return;

		SkeletonAnim.AnimationState.AddAnimation(trackIndex, AnimName, loop, delay);
	}

	//기본값은 왼쪽을 바라보는 상태
	public void Flip(bool flag)
	{
		if (SkeletonAnim == null)
			return;

		SkeletonAnim.Skeleton.ScaleX = flag ? -1 : 1;
	}
	#endregion
}
