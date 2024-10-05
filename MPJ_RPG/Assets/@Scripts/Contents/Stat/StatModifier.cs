using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

//스탯에 영향을 준 기록
public class StatModifier
{
	public readonly float Value;
	public readonly EStatModType Type;
	public readonly int Order; //연산의 우선순위
	public readonly object Source;

	public StatModifier(float value, EStatModType type, int order, object source)
	{
		Value = value;
		Type = type;
		Order = order;
		Source = source;
	}

	public StatModifier(float value, EStatModType type) : this(value, type, (int)type, null) { }

	public StatModifier(float value, EStatModType type, int order) : this(value, type, order, null) { }

	public StatModifier(float value, EStatModType type, object source) : this(value, type, (int)type, source) { }
}