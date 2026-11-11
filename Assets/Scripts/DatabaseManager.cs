using Firebase.Database;
using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class DatabaseManager : MonoBehaviour
{
    public TMP_InputField Name;
    public TMP_InputField Passcode;

    public TMP_Text NameText;
    public TMP_Text MoneyText;

    private string userID;
    private DatabaseReference dbReference;

    void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseDatabase.GetInstance("https://advanced-technology-5452a-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;
    }

    public void CreateUser()
    {
        User newUser = new User(Name.text, int.Parse(Passcode.text));

        string json = JsonUtility.ToJson(newUser);

        dbReference.Child("users").Child(userID).SetRawJsonValueAsync(json);
    }

    public IEnumerator GetName(Action<string> onCallback)
    {
        var userNameData=dbReference.Child("users").Child(userID).Child("name").GetValueAsync();

        yield return new WaitUntil(predicate: () => userNameData.IsCompleted);

        if(userNameData!=null)
        {
            DataSnapshot snapshot = userNameData.Result;

            onCallback.Invoke(snapshot.Value.ToString());
        }
    }

    public IEnumerator GetMoney(Action<float> onCallback)
    {
        var moneyData = dbReference.Child("users").Child(userID).Child("money").GetValueAsync();

        yield return new WaitUntil(predicate: () => moneyData.IsCompleted);

        if (moneyData != null)
        {
            float money = float.Parse(moneyData.Result.Value.ToString());

            onCallback.Invoke(money);
        }
    }

    public void GetUserInfo()
    {
        StartCoroutine(GetName((string name) =>
        {
            NameText.text = "Name: "+name;

        }));

        StartCoroutine(GetMoney((float money) =>
        {
            MoneyText.text = "Current balance: £" +money;

        }));
    }
}
