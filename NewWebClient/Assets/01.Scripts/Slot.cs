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

    // (이러면 안됨) -> 내 의견
    public ItemUI slotItem = null;
    public ItemSO SlotItem {
      get =>slotItem != null ? slotItem.Item : null;  
    } 

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();
    }

    

    public void RemoveItem()
    {
        _hasItem = false;
        slotItem = null;
    }

    public void SetItem(ItemUI item)
    {
        slotItem = item;
        _hasItem = true;
        slotItem.SetData(transform, _rect.position);
    }

    public void OnDrop(PointerEventData eventData)
    {
        _image.color = Color.white;

        if (eventData.pointerDrag != null && _hasItem == false) //드래그 중인 아이템이 있었다면
        {
            GameObject target = eventData.pointerDrag;

            slotItem = target.GetComponent<ItemUI>();

            if(slotItem == null)return;

            slotItem.SetData(transform, _rect.position);

            _hasItem = true;
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