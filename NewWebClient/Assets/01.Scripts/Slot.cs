using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    private Image _image;
    private RectTransform _rect;
    private bool _hasItem = false;

    public int slotNumber = 0;

    public ItemUI _slotItem = null;
    public ItemSO SlotItem => _slotItem != null ? _slotItem.item : null;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();
    }

    public void SetItem(GameObject target)
    {
        _hasItem = true;
        _slotItem = target.GetComponent<ItemUI>();
        target.transform.SetParent(transform); //부모를 나로 설정
        target.GetComponent<RectTransform>().position = _rect.position;

        // 여기는 정말 이렇게 짜면 안된다.
        Slot prevSlot = _slotItem._prevParent.GetComponent<Slot>();
        prevSlot?.RemoveItem();
    }

    public void RemoveItem()
    {
        _hasItem = false;
        _slotItem = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        _image.color = Color.white;

        if (eventData.pointerDrag != null && _hasItem == false) //드래그 중인 아이템이 있었다면
        {
            SetItem(eventData.pointerDrag);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _image.color = _hasItem ? Color.red : Color.yellow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _image.color = Color.white;
    }
}