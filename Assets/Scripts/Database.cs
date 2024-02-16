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

    public async Task<DataSnapshot> getUserData(string cpf)
    {
        DatabaseReference newUserData = db.Child("users").Child(cpf);
        DataSnapshot snapshot = await newUserData.GetValueAsync();
        if (snapshot.Exists)
        {
            return snapshot;
        }
        return null;
        
    }

    public void updateUserInformations(string cpf, string name, string email)
    {
        db.Child("users").Child(cpf).Child("Name").SetValueAsync(name);
        db.Child("users").Child(cpf).Child("Email").SetValueAsync(email);
    }

    public async Task<DataSnapshot> getUsersDatas()
    {
        DatabaseReference newUsersDatas = db.Child("users");
        DataSnapshot snapshot = await newUsersDatas.GetValueAsync();
        if (snapshot.Exists)
        {
            return snapshot;
        }
        return null;

    }

    public void removeUser(string cpf)
    {
        db.Child("users").Child(cpf).RemoveValueAsync();
    }

    public void createNewUser(string cpf, string Name, string Email, string Hierarchy)
    {
        db.Child("users").Child(cpf).Child("Name").SetValueAsync(Name);
        db.Child("users").Child(cpf).Child("Email").SetValueAsync(Email);
        db.Child("users").Child(cpf).Child("Hierarchy").SetValueAsync(Hierarchy);
        db.Child("users").Child(cpf).Child("ProfilePhoto").SetValueAsync(cpf + ".jpeg");
    }
}
