using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private Image _targetImage;
    private bool _isLoading = false;


    private void Start() {
        // List를 받아와서 Json으로 도착하면
        // Json을 파싱해서 거기있는 count갯수만큼 Button을 Instantiate 해서
        // content에 넣어준다.
        // 이 때 해당 버튼을 클릭하면 (onClick 핸들러에 액션 등록)
        // 이미지를 로드해서 _targetImage에 뜨도록 한다.

        /*
        추가 : 가장 먼저 한 사람은 디코로 보내면 된다.
        이미지를 서버로 부터 받아오되, 한 번 맏으면 저장해서 클라이언트에 넣는다.
        클라이언트에 있는 이미지는 서버에 요청하지 않고, 클라이언트의 이미지로 서비스한다.
        */
    }

    private void Update() {
        if(Input.GetButtonDown("Jump"))
        {
            if(_isLoading)return;
            _isLoading = true;
            Debug.Log("Loading Data from server");
            StartCoroutine(GetDataFromServer());
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            if(_isLoading)return;
            _isLoading = true;
            Debug.Log("Loading Data from server");

            StartCoroutine(GetImageFromServer());
        }
    }

    private IEnumerator GetImageFromServer()
    {
        UnityWebRequest webReq = UnityWebRequestTexture.GetTexture("http://localhost:50000/image");

        yield return webReq.SendWebRequest();

        if(webReq.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)webReq.downloadHandler).texture as Texture2D;

            Debug.Log(texture);
            Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),new Vector2(0.5f, 0.5f));
            _targetImage.sprite = s;
            _targetImage.preserveAspect = true;
        }

        _isLoading = false;
        Debug.Log("Loading complete");
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

            DataVO m = JsonUtility.FromJson<DataVO>(msg);

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
class DataVO
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
