using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZoneUI : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    private Image _image;
    private RectTransform _rect;

    private void Awake() {
        _image = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        _image.color = Color.white;
        if(eventData.pointerDrag != null) // 드래그 되고 있던 녀석이 존재한다면
        {
            GameObject target = eventData.pointerDrag;
            // target.transform.SetParent(transform);

            target.GetComponent<ItemUI>().SetData(transform, target.transform.position); // target.Position으로 하면 Anchor가 이상하면 이상해질 수 있다
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _image.color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _image.color = Color.white;
        
    }

}
