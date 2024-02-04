using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Hierarchy;
using UnityEngine;

public class DatabaseManager
{

    private DatabaseReference db;
    private DataSnapshot userData;
    private string GloabalCpf;
    
    public void Start()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void getCpf(string cpf)
    {
        GloabalCpf = cpf;
    }

    public void checkHierarchy(Action<string> onCallBack)
    {
        string hierarchy = "";
        db.Child("users").Child(GloabalCpf).Child("Hierarchy").GetValueAsync().ContinueWith(result =>
        {
            if (result.IsCompleted)
            {
                Debug.Log("Successful");
                DataSnapshot snapshot = result.Result;
                hierarchy = snapshot.Value.ToString();
                onCallBack.Invoke(hierarchy);
            }
            else
            {
                Debug.Log("Unseccessful");
                onCallBack.Invoke(null);
            }
        });
    }
}
