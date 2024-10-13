using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Item
{
    public ItemSaveData SaveData { get; set; }

	public int InstanceId
	{
		get { return SaveData.InstanceId; }
		set { SaveData.InstanceId = value; }
	}

	public long DbId
	{
		get { return SaveData.DbId; }
	}

	public int TemplateId
	{
		get { return SaveData.TemplateId; }
		set { SaveData.TemplateId = value; }
	}

	public int Count
	{
		get { return SaveData.Count; }
		set { SaveData.Count = value; }
	}

	public int EquipSlot
	{
		get { return SaveData.EquipSlot; }
		set { SaveData.EquipSlot = value; }
	}

	public Data.ItemData TemplateData
	{
		get
		{
			return Managers.Data.ItemDic[TemplateId];
		}
	}

	public EItemType ItemType { get; private set; }
	public EItemSubType SubType { get; private set; }

	public Item(int templateId)
	{
		TemplateId = templateId;
		ItemType = TemplateData.Type;
		SubType = TemplateData.SubType;
	}

	public virtual bool Init()
	{
		return true;
	}

	public static Item MakeItem(ItemSaveData itemInfo)
	{
		if (Managers.Data.ItemDic.TryGetValue(itemInfo.TemplateId, out ItemData itemData) == false)
			return null;

		Item item = null;

		switch (itemData.Type)
		{
			case EItemType.Weapon:
				item = new Equipment(itemInfo.TemplateId);
				break;
			case EItemType.Armor:
				item = new Equipment(itemInfo.TemplateId);
				break;
			case EItemType.Potion:
				item = new Consumable(itemInfo.TemplateId);
				break;
			case EItemType.Scroll:
				item = new Consumable(itemInfo.TemplateId);
				break;
		}

		if (item != null)
		{
			item.SaveData = itemInfo;
			item.InstanceId = itemInfo.InstanceId;
			item.Count = itemInfo.Count;
		}

		return item;
	}

	#region Helpers
	public bool IsEquippable()
	{
		return GetEquipItemEquipSlot() != EEquipSlotType.None;
	}

	//장착 슬롯 위치
	public EEquipSlotType GetEquipItemEquipSlot()
	{
		if (ItemType == EItemType.Weapon)
			return EEquipSlotType.Weapon;

		if (ItemType == EItemType.Armor)
		{
			switch (SubType)
			{
				case EItemSubType.Helmet:
					return EEquipSlotType.Helmet;
				case EItemSubType.Armor:
					return EEquipSlotType.Armor;
				case EItemSubType.Shield:
					return EEquipSlotType.Shield;
				case EItemSubType.Gloves:
					return EEquipSlotType.Gloves;
				case EItemSubType.Shoes:
					return EEquipSlotType.Shoes;
			}
		}

		return EEquipSlotType.None;
	}

	public bool IsEquippedItem()
	{
		return EquipSlot > (int)EEquipSlotType.None && EquipSlot < (int)EEquipSlotType.EquipMax;
	}

	public bool IsInInventory()
	{
		return EquipSlot == (int)EEquipSlotType.Inventory;
	}

	public bool IsInWarehouse()
	{
		return EquipSlot == (int)EEquipSlotType.WareHouse;
	}
	#endregion
}

public class Equipment : Item
{
	public int Damage { get; private set; }
	public int Defence { get; private set; }
	public double Speed { get; private set; }

	protected Data.EquipmentData EquipmentData { get { return (Data.EquipmentData)TemplateData; } }

	public Equipment(int templateId) : base(templateId)
	{
		Init();
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		if (TemplateData == null)
			return false;

		if (TemplateData.Type != EItemType.Armor || TemplateData.Type != EItemType.Weapon)
			return false;

		EquipmentData data = (EquipmentData)TemplateData;
		{
			Damage = data.Damage;
			Defence = data.Defence;
			Speed = data.Speed;
		}

		return true;
	}
}

public class Consumable : Item
{
	public double Value { get; private set; }

	public Consumable(int templateId) : base(templateId)
	{
		Init();
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		if (TemplateData == null)
			return false;

		if (TemplateData.Type != EItemType.Potion || TemplateData.Type != EItemType.Scroll)
			return false;

		ConsumableData data = (ConsumableData)TemplateData;
		{
			Value = data.Value;
		}

		return true;
	}
}
