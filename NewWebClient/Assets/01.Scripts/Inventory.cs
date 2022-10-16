using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

[Serializable]
class SlotVO
{
    public bool isEmpty;
    public int slotNumber; // -> 어떤 슬롯에
    public int code;       // -> 어떤 아이템이 있는지
    // 수량도 나중엔 들어가야 함
}

[Serializable]
class InventoryVO
{
    public int count;
    public List<SlotVO> list;
}

[Serializable]
class InventoryUserVO
{
    public int user_id;
    public string json;
}

[Serializable]
class InventorySendResultVO
{
    public string msg;
    public int user_id;
}

[Serializable]
class InventoryGetResultVO
{
    public string msg;
    public string json;
}


public class Inventory : MonoBehaviour
{
    [SerializeField]
    private Slot _slotPrefab;

    [SerializeField]
    private List<ItemSO> _codeToItem = new List<ItemSO>();
    [SerializeField]
    private ItemUI _itemUIPrefab;

    private List<Slot> _inventoryList = new List<Slot>();


    private void Awake()
    {
        // getcomponent같은건 Awake를 권장한다

        // _slotPrefab = transform.Find("Slot").gameObject; // 코드몽키가 자주 사용하는 방식

        for (int i = 0; i < 8 * 7; ++i)
        {
            Slot slot = Instantiate(_slotPrefab, transform) as Slot;
            slot.slotNumber = i;
            _inventoryList.Add(slot);
        }

        // _slotPrefab.SetActive(false); // 코드몽키가 자주 사용하는 방식
    }

    private void Start()
    {
        // 서버로부터 데이터를 받아서 파싱한 다음에
        // 그거에 맞춰서 슬롯에 넣는다.
        // StartCoroutine(GetInventory());
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            string data = GetSlotDataJson();

            int id = 1;

            InventoryUserVO vo = new InventoryUserVO { user_id = id, json = data };

            string msg = JsonUtility.ToJson(vo);

            StartCoroutine(SendInventoryData(msg));


            // 이제 이 데이터를 서버로 전송한다. 뭐랑같이?
            // userID랑 같이
            // 1번 회원이라고 가정하고 보낸다.
            // {user_id:1, json:data}
            // post로 쏘면 된다.

            // 서버는
            // 받은 거를 Insert 구문을 이용해서 해당 테이블에 넣는다.


            // 원래는 서버도 Item의 구조를 다 가지고 있어야하며,
            // 바뀐 데이터만 다시 전송해서 서버는 바뀐 데이터만 갈아끼는 방식이다.


            Debug.Log(data);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(GetInventory());
        }
    }

    private IEnumerator SendInventoryData(string data)
    {
        // UnityWebRequest req = UnityWebRequest.Post("http://localhost:50000/insert/Inventory", data);

        UnityWebRequest req = new UnityWebRequest("http://localhost:50000/insert/Inventory", "POST");

        byte[] dataByte = new System.Text.UTF8Encoding().GetBytes(data);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(dataByte);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string result = req.downloadHandler.text;

            InventorySendResultVO resultVO = JsonUtility.FromJson<InventorySendResultVO>(result);

            Debug.Log($"{resultVO.user_id}에 {resultVO.msg}");
        }
        else
        {
            Debug.LogError("결과 에러");
        }
    }

    private IEnumerator GetInventory()
    {
        UnityWebRequest req = UnityWebRequest.Get("http://localhost:50000/get/Inventory");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string result = req.downloadHandler.text;
            Debug.Log(result);
            InventoryGetResultVO resultVO = JsonUtility.FromJson<InventoryGetResultVO>(result);

            Debug.Log(resultVO.msg);
            Debug.Log(resultVO.json);

            InventoryVO inventoryVO = JsonUtility.FromJson<InventoryVO>(resultVO.json);
            
            for (int i = 0; i < inventoryVO.count; ++i)
            {
                SlotVO vo = inventoryVO.list[i];
                Slot s = transform.GetChild(vo.slotNumber).GetComponent<Slot>();
                ItemUI g = Instantiate(_itemUIPrefab, s.transform);
                g.item = _codeToItem[vo.code];
                s.SetItem(g.gameObject);
                // s._slotItem = new ItemSO { code = vo.code , sprite = _codeToSprite[vo.code]};
            }
        }
    }

    private string GetSlotDataJson()
    {
        InventoryVO sendData = new InventoryVO();
        sendData.list = new List<SlotVO>();
        foreach (Slot s in _inventoryList)
        {
            ItemSO so = s.SlotItem;
            if (so != null)
            {
                SlotVO vo = new SlotVO { code = so.code, isEmpty = false, slotNumber = s.slotNumber };
                sendData.list.Add(vo);
            }
        }

        sendData.count = sendData.list.Count;

        string json = JsonUtility.ToJson(sendData);

        return json;
    }
}
