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
class ReturnMsg // struct로 하면 비용이 더 싸진다
{
    public string msg;
    public string data;
}

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private Slot _slotPrefab;
    [SerializeField]
    private ItemUI _itemPrefab;

    [SerializeField]
    private List<ItemSO> _itemList;  //SerializedField는 UnityEditor에서 집어넣기 때문에 new로 인스턴싱 해줄 필요가 없다

    private Dictionary<int, ItemSO> _itemDictionary = new Dictionary<int, ItemSO>();

    private Dictionary<int, Slot> _slotDictionary = new Dictionary<int, Slot>();

    // private List<Slot> _inventoryList = new List<Slot>();

    private void Awake()
    {
        foreach (ItemSO so in _itemList)
        {
            _itemDictionary[so.code] = so;
        }

        for (int i = 0; i < 8 * 7; ++i)
        {
            Slot slot = Instantiate(_slotPrefab, transform) as Slot;
            slot.slotNumber = i;
            // _slotDictionary.Add(i,slot);
            _slotDictionary[i] = slot;
        }

        // getcomponent같은건 Awake를 권장한다

        // _slotPrefab = transform.Find("Slot").gameObject; // 코드몽키가 자주 사용하는 방식

        // for (int i = 0; i < 8 * 7; ++i)
        // {
        //     Slot slot = Instantiate(_slotPrefab, transform) as Slot;
        //     slot.slotNumber = i;
        //     _inventoryList.Add(slot);
        // }

        // _slotPrefab.SetActive(false); // 코드몽키가 자주 사용하는 방식
    }

    private void Start()
    {
        // 서버로부터 데이터를 받아서 파싱한 다음에
        // 그거에 맞춰서 슬롯에 넣는다.
        // StartCoroutine(GetInventory());

        LoadInven();
    }


    public void SaveInven()
    {
        // string data = GetSlotDataJson();

        // int id = 1;

        // InventoryUserVO vo = new InventoryUserVO { user_id = id, json = data };

        // string msg = JsonUtility.ToJson(vo);

        // StartCoroutine(SendInventoryData(msg));


        // Debug.Log(data);

        InventoryVO saveData = new InventoryVO();
        saveData.list = new List<SlotVO>();

        foreach (int key in _slotDictionary.Keys)
        {
            if (_slotDictionary[key].SlotItem != null)
            {
                SlotVO vo = new SlotVO
                {
                    code = _slotDictionary[key].SlotItem.code,
                    slotNumber = _slotDictionary[key].slotNumber
                };
                saveData.list.Add(vo);
            }
        }
        saveData.count = saveData.list.Count;

        DataManager.Instance.SaveData(JsonUtility.ToJson(saveData), "/inven", (json, success) =>
        {
            Debug.Log(json);
        });
        //SaveData를 쏴주면 된다
    }

    public void LoadInven()
    {
        DataManager.Instance.LoadData("/inven", (json, succenss) =>
        {
            if(succenss)
            {
                ReturnMsg msg = JsonUtility.FromJson<ReturnMsg>(json);

                if(msg.data != "")
                {
                    InventoryVO inveoVO = JsonUtility.FromJson<InventoryVO>(msg.data);

                    foreach (SlotVO slot in inveoVO.list)
                    {
                        _slotDictionary[slot.slotNumber].RemoveItem(); // 기존 슬롯 초기화
                        ItemUI item = Instantiate(_itemPrefab);

                        // (이렇게 하지 말자...) -> 내 의견
                        item.Item = _itemDictionary[slot.code];
                        _slotDictionary[slot.slotNumber].SetItem(item);
                        // item.SetData(_slotDictionary[slot.slotNumber].transform, )
                    }
                }
            }
        });
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            SaveInven();
        }

    }

}
