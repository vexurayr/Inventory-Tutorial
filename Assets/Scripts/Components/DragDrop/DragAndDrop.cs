using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] protected Canvas canvas;

    protected RectTransform rectTransform;

    public virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {}

    public virtual void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {}

    public virtual void OnEndDrag(PointerEventData eventData)
    {}

    public virtual void OnDrop(PointerEventData eventData)
    {}

    public virtual RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    public virtual void SetRectTransform(RectTransform newTransform)
    {
        rectTransform = newTransform;
    }
}