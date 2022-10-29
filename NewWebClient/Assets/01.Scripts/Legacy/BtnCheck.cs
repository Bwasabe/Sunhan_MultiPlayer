using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnCheck : MonoBehaviour
{
    private void Start() {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            DataManager.Instance.LoadData("/check", (msg, success) =>
            {
                Debug.Log(msg);
            });
        });
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteKey("token");
        }
    }
}
