using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform _canvas;
    public Transform _prevParent;

    private RectTransform _rect;
    private CanvasGroup _canvasGroup;
    private Vector3 _prevPos;

    private Image _image;
    [SerializeField]
    private ItemSO item;

    public ItemSO Item
    {
        get => item;
        set{
            item = value;
            LoadSprite();
        }
    }

    private void Awake()
    {
        _canvas = FindObjectOfType<Canvas>().transform;
        _rect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _image = GetComponent<Image>();
    }

    private void Start() {
        // _image.sprite = item.sprite;
        LoadSprite();
    }

    private async void LoadSprite()
    {
        if(item.sprite == null)
        {
            item.sprite = await item._assetSprite.LoadAssetAsync<Sprite>().Task;
            _image.sprite = item.sprite;
        }
        else
        {
            _image.sprite = item.sprite;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _prevParent = transform.parent;
        _prevPos =  _rect.position;

        transform.SetParent(_canvas);
        transform.SetAsLastSibling(); //맨 마지막 자식으로 보내져 맨 앞에 보이도록 조정한다.

        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void SetData(Transform parent, Vector3 rectPos)
    {
        transform.SetParent(parent);
        _rect.position = rectPos;
        // 이전에 만약 슬롯에 있었다면 이전 슬롯에서 이 녀석이 존재했음을 제거해줘야해
        if(_prevParent == null)return;
        Slot slot = _prevParent.GetComponent<Slot>();
        if(slot != null)
        {
            slot.RemoveItem();
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //슬롯이 드랍이 안된거
        if(transform.parent == _canvas)
        {
            transform.SetParent(_prevParent);
            _rect.position = _prevPos;
        }

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }
}