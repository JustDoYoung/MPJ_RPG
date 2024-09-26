using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public event Action<PointerEventData> OnClickHandler = null;
	public event Action<PointerEventData> OnPointerDownHandler = null;
	public event Action<PointerEventData> OnPointerUpHandler = null;
	public event Action<PointerEventData> OnDragHandler = null;

	public void OnPointerClick(PointerEventData eventData)
	{
		OnClickHandler?.Invoke(eventData);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		OnPointerDownHandler?.Invoke(eventData);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		OnPointerUpHandler?.Invoke(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		OnDragHandler?.Invoke(eventData);
	}
}
