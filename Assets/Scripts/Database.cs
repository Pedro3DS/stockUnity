using Firebase.Database;
using Firebase.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;


public class DatabaseManager
{

    private DatabaseReference db;
    private string GloabalCpf;
    
    public void Start()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void getCpf(string cpf)
    {
        GloabalCpf = cpf;
    }

    public async Task<string> checkHierarchy()
    {

        DatabaseReference newUserData = db.Child("users").Child(GloabalCpf).Child("Hierarchy");
        DataSnapshot snapshot = await newUserData.GetValueAsync();
        if (snapshot.Exists)
        {
            return snapshot.Value.ToString();
        }
        return "null";
    }

    public DataSnapshot getUserData(string cpf)
    {
        DataSnapshot snapshot = db.Child("users").Child(cpf).GetValueAsync().Result;
        return snapshot;

        
    }
}
