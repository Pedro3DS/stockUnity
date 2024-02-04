using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UIElements.VisualElement;

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

    public string checkHierarchy()
    {
        string hierarchy = "";

        try
        {
            DataSnapshot snapshot = db.Child("users").Child(GloabalCpf).Child("Hierarchy").GetValueAsync().Result;
            hierarchy = snapshot.Value.ToString();
            Debug.Log("Successful");
        }
        catch (Exception e)
        {
            Debug.LogError($"Unsuccessful: {e.Message}");
        }

        return hierarchy;
    }


}
