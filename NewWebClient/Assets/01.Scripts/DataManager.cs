using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public const string URL = "http://localhost:50000";
    public int user_id = 1;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple DataManager is running");
        }
        Instance = this;
    }

    public void SaveData(WWWForm form, string uri, Action<string, bool> Callback)
    {
        StartCoroutine(SendData("", uri, Callback, form));
    }

    public void SaveData(string json, string uri, Action<string, bool> Callback)
    {
        StartCoroutine(SendData(json, uri, Callback));
    }

    // 재활용 하기 위해서는 string json 이 아닌 WWWForm으로 받아야 한다
    IEnumerator SendData(string json, string uri, Action<string, bool> Callback, WWWForm form = null) // 원래는 ErrorCallBack을 만들어 둬야하나 너무 복잡하기 때문에 생략
    {
        if (form == null)
        {
            form = new WWWForm();
            form.AddField("user_id", user_id);
            form.AddField("json", json);
        }
        

        UnityWebRequest req = UnityWebRequest.Post(URL + uri, form);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Callback(req.downloadHandler.text, true);
        }
        else
        {
            Callback(req.error, false);
        }
    }

    public void LoadData(string uri, Action<string, bool> CallBack)
    {
        StartCoroutine(LoadCoroutine(uri, CallBack));
    }

    private IEnumerator LoadCoroutine(string uri, Action<string, bool> CallBack)
    {
        UnityWebRequest req = UnityWebRequest.Get($"{URL}{uri}");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            CallBack(req.downloadHandler.text, true);
        }
        else
        {
            CallBack(req.error, false);
        }
    }
}
