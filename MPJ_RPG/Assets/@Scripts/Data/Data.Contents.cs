using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


namespace Data
{
	#region CreatureData
	[Serializable]
	public class CreatureData
	{
		public int DataId;
		public string DescriptionTextID;
		public string PrefabLabel;
		public float ColliderOffsetX;
		public float ColliderOffsetY;
		public float ColliderRadius;
		public float MaxHp;
		public float UpMaxHpBonus;
		public float Atk;
		public float AtkRange;
		public float AtkBonus;
		public float MoveSpeed;
		public float CriRate;
		public float CriDamage;
		public string IconImage;
		public string SkeletonDataID;
		public int DefaultSkillId;
		public int EnvSkillId;
		public int SkillAId;
		public int SkillBId;
	}
	#endregion

	#region MonsterData
	[Serializable]
	public class MonsterData : CreatureData
	{
		public int DropItemId;
	}

	[Serializable]
	public class MonsterDataLoader : ILoader<int, MonsterData>
	{
		public List<MonsterData> monsters = new List<MonsterData>();
		public Dictionary<int, MonsterData> MakeDict()
		{
			Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
			foreach (MonsterData monster in monsters)
				dict.Add(monster.DataId, monster);
			return dict;
		}
	}
	#endregion

	#region HeroData
	[Serializable]
	public class HeroData : CreatureData
	{
	}

	[Serializable]
	public class HeroDataLoader : ILoader<int, HeroData>
	{
		public List<HeroData> heroes = new List<HeroData>();
		public Dictionary<int, HeroData> MakeDict()
		{
			Dictionary<int, HeroData> dict = new Dictionary<int, HeroData>();
			foreach (HeroData hero in heroes)
				dict.Add(hero.DataId, hero);
			return dict;
		}
	}
	#endregion

	#region HeroInfoData
	[Serializable]
	public class HeroInfoData
	{
		public int DataId;
		public string NameTextId;
		public string DescriptionTextId;
		public string Rarity;
		public float GachaSpawnWeight;
		public float GachaWeight;
		public int GachaExpCount;
		public string IconImage;
	}

	[Serializable]
	public class HeroInfoDataLoader : ILoader<int, HeroInfoData>
	{
		public List<HeroInfoData> heroInfo = new List<HeroInfoData>();
		public Dictionary<int, HeroInfoData> MakeDict()
		{
			Dictionary<int, HeroInfoData> dict = new Dictionary<int, HeroInfoData>();
			foreach (HeroInfoData info in heroInfo)
				dict.Add(info.DataId, info);
			return dict;
		}
	}
	#endregion

	#region SkillData
	[Serializable]
	public class SkillData
	{
		public int DataId;
		public string Name;
		public string ClassName;
		public string Description;
		public int ProjectileId;
		public string PrefabLabel;
		public string IconLabel;
		public string AnimName;
		public float CoolTime;
		public float DamageMultiplier;
		public float Duration;
		public float AnimImpactDuration;
		public string CastingSound;
		public float SkillRange;
		public float ScaleMultiplier;
		public int TargetCount;
		public List<int> EffectIds = new List<int>();
		public int NextLevelId;
		public int AoEId;
		public EEffectSize EffectSize;
	}

	[Serializable]
	public class SkillDataLoader : ILoader<int, SkillData>
	{
		public List<SkillData> skills = new List<SkillData>();

		public Dictionary<int, SkillData> MakeDict()
		{
			Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
			foreach (SkillData skill in skills)
				dict.Add(skill.DataId, skill);
			return dict;
		}
	}
	#endregion

	#region ProjectileData
	[Serializable]
	public class ProjectileData
	{
		public int DataId;
		public string Name;
		public string ClassName;
		public string ComponentName;
		public string ProjectileSpriteName;
		public string PrefabLabel;
		public float Duration;
		public float HitSound;
		public float ProjRange;
		public float ProjSpeed;
	}

	[Serializable]
	public class ProjectileDataLoader : ILoader<int, ProjectileData>
	{
		public List<ProjectileData> projectiles = new List<ProjectileData>();

		public Dictionary<int, ProjectileData> MakeDict()
		{
			Dictionary<int, ProjectileData> dict = new Dictionary<int, ProjectileData>();
			foreach (ProjectileData projectile in projectiles)
				dict.Add(projectile.DataId, projectile);
			return dict;
		}
	}
	#endregion

	#region Env
	[Serializable]
	public class EnvData
	{
		public int DataId;
		public string DescriptionTextID;
		public string PrefabLabel;
		public float MaxHp;
		public int ResourceAmount;
		public float RegenTime;
		public List<String> SkeletonDataIDs = new List<String>();
		public int DropItemId;
	}

	[Serializable]
	public class EnvDataLoader : ILoader<int, EnvData>
	{
		public List<EnvData> envs = new List<EnvData>();
		public Dictionary<int, EnvData> MakeDict()
		{
			Dictionary<int, EnvData> dict = new Dictionary<int, EnvData>();
			foreach (EnvData env in envs)
				dict.Add(env.DataId, env);
			return dict;
		}
	}
	#endregion

	#region EffectData
	[Serializable]
	public class EffectData
	{
		public int DataId;
		public string Name;
		public string ClassName;
		public string DescriptionTextID;
		public string SkeletonDataID;
		public string IconLabel;
		public string SoundLabel;
		public float Amount;
		public float PercentAdd;
		public float PercentMult;
		public float TickTime;
		public float TickCount;
		public EEffectType EffectType;
	}

	[Serializable]
	public class EffectDataLoader : ILoader<int, EffectData>
	{
		public List<EffectData> effects = new List<EffectData>();
		public Dictionary<int, EffectData> MakeDict()
		{
			Dictionary<int, EffectData> dict = new Dictionary<int, EffectData>();
			foreach (EffectData effect in effects)
				dict.Add(effect.DataId, effect);
			return dict;
		}
	}
	#endregion

	#region AoEData
	[Serializable]
	public class AoEData
	{
		public int DataId;
		public string Name;
		public string ClassName;
		public string SkeletonDataID;
		public string SoundLabel;
		public float Duration;
		public List<int> AllyEffects = new List<int>();
		public List<int> EnemyEffects = new List<int>();
		public string AnimName;
	}

	[Serializable]
	public class AoEDataLoader : ILoader<int, AoEData>
	{
		public List<AoEData> aoes = new List<AoEData>();
		public Dictionary<int, AoEData> MakeDict()
		{
			Dictionary<int, AoEData> dict = new Dictionary<int, AoEData>();
			foreach (AoEData aoe in aoes)
				dict.Add(aoe.DataId, aoe);
			return dict;
		}
	}
	#endregion

	#region NPC
	[Serializable]
	public class NpcData
	{
		public int DataId;
		public string Name;
		public string DescriptionTextID;
		public ENpcType NpcType;
		public string PrefabLabel;
		public string SpriteName;
		public string SkeletonDataID;
	}

	[Serializable]
	public class NpcDataLoader : ILoader<int, NpcData>
	{
		public List<NpcData> creatures = new List<NpcData>();
		public Dictionary<int, NpcData> MakeDict()
		{
			Dictionary<int, NpcData> dict = new Dictionary<int, NpcData>();
			foreach (NpcData creature in creatures)
				dict.Add(creature.DataId, creature);
			return dict;
		}
	}
	#endregion

	#region TextData
	[Serializable]
	public class TextData
	{
		public string DataId;
		public string KOR;
	}

	[Serializable]
	public class TextDataLoader : ILoader<string, TextData>
	{
		public List<TextData> texts = new List<TextData>();
		public Dictionary<string, TextData> MakeDict()
		{
			Dictionary<string, TextData> dict = new Dictionary<string, TextData>();
			foreach (TextData text in texts)
				dict.Add(text.DataId, text);
			return dict;
		}
	}
	#endregion
}