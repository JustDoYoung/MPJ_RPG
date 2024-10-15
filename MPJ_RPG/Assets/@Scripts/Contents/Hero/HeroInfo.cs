using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HeroInfo
{
	public HeroSaveData SaveData { get; set; }

	public int TemplateId
	{
		get { return SaveData.DataId; }
		set { SaveData.DataId = value; }
	}

	public int Level
	{
		get { return SaveData.Level; }
		set { SaveData.Level = value; }
	}

	public int Exp
	{
		get { return SaveData.Exp; }
		set { SaveData.Exp = value; }
	}

	public EHeroOwningState OwningState
	{
		get { return SaveData.OwningState; }
		set { SaveData.OwningState = value; }
	}

	public HeroData HeroData { get; private set; } //히어로 능력치, 스킬..
	public HeroInfoData HeroInfoData { get; set; } //등급 ...

	public int ASkillDataId { get { return HeroData.SkillAId; } }
	public int BSkillDataId { get { return HeroData.SkillBId; } }

	public bool IsPicked()
	{
		return OwningState == EHeroOwningState.Picked;
	}

	public HeroInfo(HeroSaveData saveData)
	{
		SaveData = saveData;

		if (Managers.Data.HeroDic.TryGetValue(saveData.DataId, out HeroData data))
			HeroData = data;

		if (Managers.Data.HeroInfoDic.TryGetValue(saveData.DataId, out HeroInfoData infoData))
			HeroInfoData = infoData;

		OwningState = saveData.OwningState;
	}

	public static HeroInfo MakeHeroInfo(HeroSaveData saveData)
	{
		HeroInfo heroInfo = new HeroInfo(saveData);
		return heroInfo;
	}
}
