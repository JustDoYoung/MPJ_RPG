using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
	public enum EScene
	{
		Unknown,
		TitleScene,
		GameScene,
	}

	public enum EUIEvent
	{
		Click,
		PointerDown,
		PointerUp,
		Drag,
	}

	public enum EJoystickState
	{
		PointerDown,
		PointerUp,
		Drag,
	}

	public enum ESound
	{
		Bgm,
		Effect,
		Max,
	}

	public enum EObjectType
	{
		None,
		HeroCamp,
		Creature,
		Projectile,
		Env,
	}

	public enum ECreatureType
	{
		None,
		Hero,
		Monster,
		Npc,
	}

	public enum ECreatureState
	{
		None,
		Idle,
		Move,
		Skill,
		Dead
	}

	public enum EHeroMoveState
	{
		None,
		TargetMonster,
		CollectEnv,
		ReturnToCamp,
		ForceMove,
		ForcePath
	}

	public enum EEnvState
	{
		Idle,
		OnDamaged,
		Dead
	}

	public enum ELayer
	{
		Default = 0,
		TransparentFX = 1,
		IgnoreRaycast = 2,
		Dummy1 = 3,
		Water = 4,
		UI = 5,
		Hero = 6,
		Monster = 7,
		Env = 8,
		Obstacle = 9,
		Projectile = 10,
	}

	public enum EColliderSize
	{
		Small,
		Normal,
		Big
	}

	public enum EFindPathResult
	{
		Fail_LerpCell,
		Fail_NoPath,
		Fail_MoveTo,
		Success,
	}

	public enum ECellCollisionType
	{
		None, //갈 수 있음
		SemiWall, //카메라만
		Wall, //갈 수 없음
	}

	public enum ESkillSlot
	{
		Default,
		Env,
		A,
		B
	}

	//공격 모양
	public enum EIndicatorType
	{
		None,
		Cone,
		Rectangle,
	}

	//공격 범위
	public enum EEffectSize
	{
		CircleSmall,
		CircleNormal,
		CircleBig,
		ConeSmall,
		ConeNormal,
		ConeBig,
	}

	public enum EStatModType
	{
		Add,
		PercentAdd,
		PercentMult,
	}

	public const float EFFECT_SMALL_RADIUS = 2.5f;
	public const float EFFECT_NORMAL_RADIUS = 4.5f;
	public const float EFFECT_BIG_RADIUS = 5.5f;

	public const int CAMERA_PROJECTION_SIZE = 12;

	// HARD CODING
	public const float HERO_SEARCH_DISTANCE = 8.0f;
	public const float MONSTER_SEARCH_DISTANCE = 8.0f;
	public const int HERO_DEFAULT_MELEE_ATTACK_RANGE = 1;
	public const int HERO_DEFAULT_RANGED_ATTACK_RANGE = 5;
	public const float HERO_DEFAULT_STOP_RANGE = 1.5f;

	public const int HERO_DEFAULT_MOVE_DEPTH = 5;
	public const int MONSTER_DEFAULT_MOVE_DEPTH = 3;

	public const int HERO_WIZARD_ID = 201000;
	public const int HERO_KNIGHT_ID = 201001;

	public const int MONSTER_SLIME_ID = 202001;
	public const int MONSTER_SPIDER_COMMON_ID = 202002;
	public const int MONSTER_WOOD_COMMON_ID = 202004;
	public const int MONSTER_GOBLIN_ARCHER_ID = 202005;
	public const int MONSTER_BEAR_ID = 202006;

	public const int ENV_TREE1_ID = 300001;
	public const int ENV_TREE2_ID = 301000;

	public const char MAP_TOOL_WALL = '0';
	public const char MAP_TOOL_NONE = '1';
	public const char MAP_TOOL_SEMI_WALL = '2'; //카메라만
}

public static class AnimName
{
	public const string ATTACK_A = "attack";
	public const string ATTACK_B = "attack";
	public const string SKILL_A = "skill";
	public const string SKILL_B = "skill";
	public const string IDLE = "idle";
	public const string MOVE = "move";
	public const string DAMAGED = "hit";
	public const string DEAD = "dead";
	public const string EVENT_ATTACK_A = "event_attack";
	public const string EVENT_ATTACK_B = "event_attack";
	public const string EVENT_SKILL_A = "event_attack";
	public const string EVENT_SKILL_B = "event_attack";
}

public static class SortingLayers
{
	public const int SPELL_INDICATOR = 200;
	public const int CREATURE = 300;
	public const int ENV = 300;
	public const int PROJECTILE = 310;
	public const int SKILL_EFFECT = 310;
	public const int DAMAGE_FONT = 410;
}
