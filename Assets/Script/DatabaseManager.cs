using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour
{
    public TMP_InputField Name;
    public TMP_InputField Email;
    public TMP_InputField Password;

    private DatabaseReference dbRederence;
    // Start is called before the first frame update
    void Start()
    {
        dbRederence = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void CreateUser()
    {
        User newUser = new User(Name.text, Email.text, Password.text);
        string json = JsonUtility.ToJson(newUser);

        dbRederence.Child("users").Child(Email.text).SetRawJsonValueAsync(json);
    }
}
