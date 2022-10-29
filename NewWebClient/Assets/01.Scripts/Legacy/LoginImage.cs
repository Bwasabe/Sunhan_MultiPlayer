using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct MsgVO
{
    public bool success;
    public string msg;
    public string token;
}

public class LoginImage : MonoBehaviour
{
    private InputField _accountInput;
    private InputField _passInput;
    private Button _loginBtn;

    private void Awake() {
        _accountInput = transform.Find("AccountInputField").GetComponent<InputField>();
        _passInput = transform.Find("PassInputField").GetComponent<InputField>();
        _loginBtn = transform.Find("Button").GetComponent<Button>();

        _loginBtn.onClick.AddListener(() =>
        {
            WWWForm form = new WWWForm();
            form.AddField("account", _accountInput.text);
            form.AddField("pass", _passInput.text);
            string id = SystemInfo.deviceUniqueIdentifier;
            form.AddField("deviceID", id);

            DataManager.Instance.SaveData(form, "/login", (json, success) =>
            {
                Debug.Log(json);
                MsgVO msg = JsonUtility.FromJson<MsgVO>(json);

                if(msg.success)
                {
                    PlayerPrefs.SetString("token", msg.token);
                }
                else
                {
                    Debug.LogError("아이디와 비밀번호가 불일치");
                }
            });
        });
    }
}
