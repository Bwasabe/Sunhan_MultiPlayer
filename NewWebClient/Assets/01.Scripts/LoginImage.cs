using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

            DataManager.Instance.SaveData(form, "/login", (json, success) =>
            {
                Debug.Log(json);
            });
        });
    }
}
