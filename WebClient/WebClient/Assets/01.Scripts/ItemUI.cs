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
    public ItemSO item;

    private void Awake()
    {
        _canvas = FindObjectOfType<Canvas>().transform;
        _rect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _image = GetComponent<Image>();
    }

    private void Start() {
        _image.sprite = item.sprite;
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