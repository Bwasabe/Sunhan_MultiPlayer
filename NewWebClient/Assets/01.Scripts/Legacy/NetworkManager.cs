using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private Image _targetImage;
    [SerializeField]
    private Button _button;

    private Dictionary<string, Sprite> _spriteDict = new Dictionary<string, Sprite>();
    private bool _isLoading = false;

    private const string FILE_PATH = "/Images";

    private void Start()
    {
        StartCoroutine(GetAllImageFromServer());

        // TODO:
        // List를 받아와서 Json으로 도착하면
        // Json을 파싱해서 거기있는 count갯수만큼 Button을 Instantiate 해서
        // content에 넣어준다.
        // 이 때 해당 버튼을 클릭하면 (onClick 핸들러에 액션 등록)
        // 이미지를 로드해서 _targetImage에 뜨도록 한다.
        /*
        추가 : 가장 먼저 한 사람은 디코로 보내면 된다.
        이미지를 서버로 부터 받아오되, 한 번 받으면 저장해서 클라이언트에 넣는다.
        클라이언트에 있는 이미지는 서버에 요청하지 않고, 클라이언트의 이미지로 서비스한다.
        */
    }


    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (_isLoading) return;
            _isLoading = true;
            Debug.Log("Loading Data from server");
            StartCoroutine(GetDataFromServer());
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (_isLoading) return;
            _isLoading = true;
            Debug.Log("Loading Data from server");

            StartCoroutine(GetImageFromServer());
        }
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     if (_isLoading) return;
        //     _isLoading = true;
        //     Debug.Log("Loading Data from server");

        //     StartCoroutine(GetAllImageFromServer());
        // }
    }

    private IEnumerator GetAllImageFromServer()
    {
        UnityWebRequest webReq = UnityWebRequest.Get("http://localhost:50000/imageList");

        yield return webReq.SendWebRequest();

        if (webReq.result == UnityWebRequest.Result.Success)
        {
            string msg = webReq.downloadHandler.text;
            // Debug.Log(msg);

            TextureVO vo = JsonUtility.FromJson<TextureVO>(msg);

            // vo.list.ForEach(x => Debug.Log(x));
            for (int i = 0; i < vo.count; ++i)
            {
                string filename = vo.list[i];
                string path = Path.Combine(Application.dataPath, FILE_PATH);

                Debug.Log(path);
                Texture2D texture = default(Texture2D);
                string filePath =  Path.Combine(path, filename);
                if (Directory.Exists(path))
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    Texture2D texture2D = null;
                    texture2D.LoadImage(bytes);

                    // byte[] bytes = texture.EncodeToPNG();
                    // //TODO : 경로 설정해주기
                    // File.WriteAllBytes("", bytes);
                }
                else
                {
                    UnityWebRequest req = UnityWebRequestTexture.GetTexture($"http://localhost:50000/image/{filename}");

                    yield return req.SendWebRequest();

                    texture = ((DownloadHandlerTexture)req.downloadHandler).texture as Texture2D;

                    Directory.CreateDirectory(path);

                    File.WriteAllBytes(path, texture.EncodeToPNG());
                }
                Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                _spriteDict[filename] = s;

                Button b = Instantiate(_button, _button.transform.parent);
                b.onClick.AddListener(() => _targetImage.sprite = _spriteDict[filename]);
                b.gameObject.SetActive(true);
                int findIndex = filename.LastIndexOf('.');
                b.transform.GetChild(0).GetComponent<Text>().text = filename.Substring(0, filename.Length - findIndex);
            }
        }

        _targetImage.preserveAspect = true;
    }

    private IEnumerator GetImageFromServer()
    {
        UnityWebRequest webReq = UnityWebRequestTexture.GetTexture("http://localhost:50000/image/V한민영.png");

        yield return webReq.SendWebRequest();

        if (webReq.result == UnityWebRequest.Result.Success)
        {
            // Debug.Log(webReq.downloadHandler.data);
            Texture2D texture = ((DownloadHandlerTexture)webReq.downloadHandler).texture as Texture2D;

            // Debug.Log(texture);
            Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            _targetImage.sprite = s;
            _targetImage.preserveAspect = true;
        }

        _isLoading = false;
        Debug.Log("Loading complete");
    }

    private IEnumerator GetDataFromServer()
    {
        // Post, Get, Put, Delete
        // CRUD(Create, Read, Update, Delete)
        UnityWebRequest webReq = UnityWebRequest.Get("http://localhost:50000/");

        yield return webReq.SendWebRequest();

        // ProtocolError : 서버에서 Get을 만들지 않았을 때
        // DataProcessingError : 서버에서 문제가 생겼을 때(거의 안보임)
        // ConnectionError : 연결에 실패했을 때
        if (webReq.result == UnityWebRequest.Result.Success)
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


