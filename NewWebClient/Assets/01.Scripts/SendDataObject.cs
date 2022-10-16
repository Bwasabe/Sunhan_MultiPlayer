using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
class ReturnMsg
{
    public string msg;
    public int incrementID;
}

[Serializable]
class ScoreVO
{
    public int id;
    public int score;
    public string username;
    public string time;
}

[Serializable]
class RecordList
{
    public string msg;
    public int count;
    public List<ScoreVO> data;

}

public class SendDataObject : MonoBehaviour
{
    private InputField _inputScore;
    private InputField _inputUsername;

    private Button _sendBtn;
    private void Awake()
    {
        _inputScore = transform.Find("InputScore").GetComponent<InputField>();
        _inputUsername = transform.Find("InputUsername").GetComponent<InputField>();
        _sendBtn = transform.Find("Button").GetComponent<Button>();

        _sendBtn.onClick.AddListener(() =>
        {
            StartCoroutine(SendRecordData());
        });
    }



    IEnumerator SendRecordData()
    {
        int score = int.Parse(_inputScore.text);
        string username = _inputUsername.text;

        RecordVO vo = new RecordVO
        {
            score = score,
            username = username
        };

        string json = JsonUtility.ToJson(vo);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();


        UnityWebRequest req = new UnityWebRequest("http://localhost:50000/insert", "POST");

        byte[] dataByte = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(dataByte);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-type", "application/json");

        // MultipartFormDataSection a; // 데이터 받을 때
        // MultipartFormFileSection b; // 데이터 보낼 때
        // formData.Add(new MultipartFormDataSection($"score={score}&username={username}"));

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string result = req.downloadHandler.text;

            ReturnMsg msg = JsonUtility.FromJson<ReturnMsg>(result);

            Debug.Log(msg.incrementID);

            // ResultVO resultVO = JsonUtility.FromJson<ResultVO>(result);
            // Debug.Log(resultVO.incrementID);
        }
        else
        {
            Debug.LogError("전송 에러");
        }
    }

    private IEnumerator GetScoreData()
    {
        UnityWebRequest req = UnityWebRequest.Get("http://localhost:50000/record");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string msg = req.downloadHandler.text;

            RecordList recordList = JsonUtility.FromJson<RecordList>(msg);

            Debug.Log(recordList.msg);
            
            foreach(ScoreVO vo in recordList.data)
            {
                DateTime dt = DateTime.Parse(vo.time);
                Debug.Log($"{vo.username} : {vo.score} - {dt}");
            }
        }
        else
        {
            Debug.LogError("전송 에러");
        }
    }

    private void Update() {
        if(Input.GetButtonDown("Jump"))
        {
            StartCoroutine(GetScoreData());
        }
    }
}
