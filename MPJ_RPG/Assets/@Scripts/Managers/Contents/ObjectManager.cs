using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
	public HashSet<Hero> Heroes { get; } = new HashSet<Hero>();
	public HashSet<Monster> Monsters { get; } = new HashSet<Monster>();
	public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>();
	public HashSet<Env> Envs { get; } = new HashSet<Env>();
	public HeroCamp Camp { get; private set; }

	#region Roots
	public Transform GetRootTransform(string name)
	{
		GameObject root = GameObject.Find(name);
		if (root == null)
			root = new GameObject { name = name };

		return root.transform;
	}

	public Transform HeroRoot { get { return GetRootTransform("@Heroes"); } }
	public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }
	public Transform ProjectileRoot { get { return GetRootTransform("@Projectiles"); } }
	public Transform EnvRoot { get { return GetRootTransform("@Envs"); } }
	#endregion

	public T Spawn<T>(Vector3 position, int templateID) where T : BaseObject
	{
		string prefabName = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate(prefabName);
		go.name = prefabName;
		go.transform.position = position;

		BaseObject obj = go.GetComponent<BaseObject>();

		if (obj.ObjectType == EObjectType.Creature)
		{
			Creature creature = go.GetComponent<Creature>();
			switch (creature.CreatureType)
			{
				case ECreatureType.Hero:
					obj.transform.parent = HeroRoot;
					Hero hero = creature as Hero;
					Heroes.Add(hero);
					break;
				case ECreatureType.Monster:
					obj.transform.parent = MonsterRoot;
					Monster monster = creature as Monster;
					Monsters.Add(monster);
					break;
			}

			creature.SetInfo(templateID);
		}
		else if (obj.ObjectType == EObjectType.Projectile)
		{
			obj.transform.parent = ProjectileRoot;

			Projectile projectile = go.GetComponent<Projectile>();
			Projectiles.Add(projectile);

			projectile.SetInfo(templateID);
		}
		else if (obj.ObjectType == EObjectType.Env)
		{
			obj.transform.parent = EnvRoot;

			Env env = go.GetComponent<Env>();
			Envs.Add(env);

			env.SetInfo(templateID);
		}
		else if (obj.ObjectType == EObjectType.HeroCamp)
		{
			Camp = go.GetComponent<HeroCamp>();
		}

		return obj as T;
	}

	public void Despawn<T>(T obj) where T : BaseObject
	{
		EObjectType objectType = obj.ObjectType;

		if (obj.ObjectType == EObjectType.Creature)
		{
			Creature creature = obj.GetComponent<Creature>();
			switch (creature.CreatureType)
			{
				case ECreatureType.Hero:
					Hero hero = creature as Hero;
					Heroes.Remove(hero);
					break;
				case ECreatureType.Monster:
					Monster monster = creature as Monster;
					Monsters.Remove(monster);
					break;
			}
		}
		else if (obj.ObjectType == EObjectType.Projectile)
		{
			Projectile projectile = obj as Projectile;
			Projectiles.Remove(projectile);
		}
		else if (obj.ObjectType == EObjectType.Env)
		{
			Env env = obj as Env;
			Envs.Remove(env);
		}
		else if (obj.ObjectType == EObjectType.HeroCamp)
		{
			Camp = null;
		}

		Managers.Resource.Destroy(obj.gameObject);
	}
}
