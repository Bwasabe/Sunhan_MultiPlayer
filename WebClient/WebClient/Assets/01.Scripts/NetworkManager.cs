using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    private bool _isLoading = false;

    private void Update() {
        if(Input.GetButtonDown("Jump"))
        {
            if(_isLoading)return;
            _isLoading = true;
            Debug.Log("Loading Data from server");
            StartCoroutine(GetDataFromServer());
        }
    }

    private IEnumerator GetDataFromServer()
    {
        // Post, Get, Put, Delete
        // CRUD(Create, Request, Update, Delete)
        UnityWebRequest webReq = UnityWebRequest.Get("http://localhost:50000/");

        yield return webReq.SendWebRequest();

        // ProtocolError : 서버에서 Get을 만들지 않았을 때
        // DataProcessingError : 서버에서 문제가 생겼을 때(거의 안보임)
        // ConnectionError : 연결에 실패했을 때
        if(webReq.result == UnityWebRequest.Result.Success)
        {
            string msg = webReq.downloadHandler.text;

            Debug.Log(msg);
            Msg m = JsonUtility.FromJson<Msg>(msg);

            for (int i = 0; i < m.users.Count; ++i)
            {
                Debug.Log(m.users[i].name);
            }

            Debug.Log(msg);
        }

        _isLoading = false;
        Debug.Log("Loading complete");
    }
}

[System.Serializable]
class Msg
{
    public string name;
    public List<User> users;
}

[System.Serializable]
class User
{
    public int id;
    public string name;
}
