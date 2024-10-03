using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class BaseObject : InitBase
{
	public EObjectType ObjectType { get; protected set; } = EObjectType.None;
	public CircleCollider2D Collider { get; private set; }
	public SkeletonAnimation SkeletonAnim { get; private set; }
	public Rigidbody2D RigidBody { get; private set; }

	public float ColliderRadius { get { return Collider != null ? Collider.radius : 0.0f; } }
	public Vector3 CenterPosition { get { return transform.position + Vector3.up * ColliderRadius; } }

	public int DataTemplateID { get; set; }

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
		if (base.Init() == false)
			return false;

		Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
		SkeletonAnim = GetComponent<SkeletonAnimation>();
		RigidBody = GetComponent<Rigidbody2D>();

		return true;
	}

	public void LookAtTarget(BaseObject target)
	{
		Vector2 dir = target.transform.position - transform.position;
		if (dir.x < 0)
			LookLeft = true;
		else
			LookLeft = false;
	}

	#region Battle
	public virtual void OnDamaged(BaseObject attacker, SkillBase skill)
	{

	}

	public virtual void OnDead(BaseObject attacker, SkillBase skill)
	{

	}
	#endregion

	#region Spine
	protected virtual void SetSpineAnimation(string dataLabel, int sortingOrder)
	{
		if (SkeletonAnim == null)
			return;

		SkeletonAnim.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(dataLabel);
		SkeletonAnim.Initialize(true);

		// Spine SkeletonAnimation은 SpriteRenderer 를 사용하지 않고 MeshRenderer을 사용함
		// 그렇기떄문에 2D Sort Axis가 안먹히게 되는데 SortingGroup을 SpriteRenderer,MeshRenderer을 같이 계산함.
		SortingGroup sg = Util.GetOrAddComponent<SortingGroup>(gameObject);
		sg.sortingOrder = sortingOrder;
	}

	protected virtual void UpdateAnimation()
	{
	}

	public TrackEntry PlayAnimation(int trackIndex, string animName, bool loop)
	{
		if (SkeletonAnim == null)
			return null;

		TrackEntry entry = SkeletonAnim.AnimationState.SetAnimation(trackIndex, animName, loop);

		//애니메이션 블렌딩
		if (animName == AnimName.DEAD)
			entry.MixDuration = 0;
		else
			entry.MixDuration = 0.2f;

		return entry;
	}

	public void AddAnimation(int trackIndex, string AnimName, bool loop, float delay)
	{
		if (SkeletonAnim == null)
			return;

		SkeletonAnim.AnimationState.AddAnimation(trackIndex, AnimName, loop, delay);
	}

	public void Flip(bool flag)
	{
		if (SkeletonAnim == null)
			return;

		SkeletonAnim.Skeleton.ScaleX = flag ? -1 : 1;
	}

	public virtual void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
	{
		Debug.Log("OnAnimEventHandler");
	}
	#endregion

	#region Map
	public bool LerpCellPosCompleted { get; protected set; }

	Vector3Int _cellPos;
	public Vector3Int CellPos
	{
		get { return _cellPos; }
		protected set
		{
			_cellPos = value;
			LerpCellPosCompleted = false;
		}
	}

	public void SetCellPos(Vector3Int cellPos, bool forceMove = false)
	{
		CellPos = cellPos;
		LerpCellPosCompleted = false;

		if (forceMove)
		{
			transform.position = Managers.Map.Cell2World(CellPos);
			LerpCellPosCompleted = true;
		}
	}

	/// <summary>
	/// 실제 이동을 담당.
	/// </summary>
	/// <param name="moveSpeed"></param>
	public void LerpToCellPos(float moveSpeed)
	{
		if (LerpCellPosCompleted)
			return;

		Vector3 destPos = Managers.Map.Cell2World(CellPos);
		Vector3 dir = destPos - transform.position;

		if (dir.x < 0)
			LookLeft = true;
		else
			LookLeft = false;

		if (dir.magnitude < 0.01f)
		{
			transform.position = destPos;
			LerpCellPosCompleted = true;
			return;
		}

		float moveDist = Mathf.Min(dir.magnitude, moveSpeed * Time.deltaTime);
		transform.position += dir.normalized * moveDist;
	}
	#endregion
}
