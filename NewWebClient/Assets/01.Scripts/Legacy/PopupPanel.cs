using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PopupCategory
{
    REGISTER = 0,
    LOGIN = 1,

    LENGTH
}

public class PopupPanel : MonoBehaviour
{
    public static PopupPanel Instance; // 원래는 UIManager가 있고, 걔가 Popup을 관리해야 함

    private CanvasGroup _canvasGroup;


    private void Awake() {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OpenPopup(PopupCategory category)
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    public void ClosePopup()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }
}
