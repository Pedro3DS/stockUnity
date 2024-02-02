using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DatabaseManager : MonoBehaviour
{
    public TMP_InputField Name;
    public TMP_InputField Email;
    public TMP_InputField Password;

    private DatabaseReference dbRederence;

    void Start()
    {
        
        dbRederence = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CreateUser()
    {
        string fEmail = Regex.Replace(Email.text, "[@.]", "");
        User newUser = new User(Name.text, Email.text, Password.text);
        string json = JsonUtility.ToJson(newUser);

        dbRederence.Child("users").Child(fEmail).SetRawJsonValueAsync(json);
    }
}
