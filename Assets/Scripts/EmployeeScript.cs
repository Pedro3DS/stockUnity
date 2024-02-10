using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Firebase.Extensions;
using Firebase.Storage;
using TMPro;
using System;

public class EmployeeScript : MonoBehaviour
{
    public TMP_Text userName;
    public TMP_Text userEmail;
    public TMP_Text userHierarchy;
    public RawImage userImg;

    private FirebaseStorage storage;
    private StorageReference storageReference;

    private DataSnapshot userSnapshot;

    private DatabaseManager db;
    async void Start()
    {
        db = new DatabaseManager();

        string userCpf = PlayerPrefs.GetString("Cpf");

        string cpfTest = new string("123");

        Debug.Log(cpfTest);

        db.getCpf("123");

        userSnapshot = await db.getUserData();

        Debug.Log(userSnapshot);

        setUserInformations();
    }

    private void setUserInformations()
    {
        userName.text = userSnapshot.Child("Name").Value.ToString();
        userEmail.text = userSnapshot.Child("Email").Value.ToString();
        userHierarchy.text = userSnapshot.Child("Hierarchy").Value.ToString();

    }

}
