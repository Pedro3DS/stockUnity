using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using System;

public class EmployeeScript : MonoBehaviour
{
    public TMP_Text userName;
    public TMP_Text userEmail;
    public TMP_Text userHierarchy;
    public RawImage userImg;

    private DataSnapshot userSnapshot;

    private DatabaseManager db;
    private string cpfT = "123";
    void Start()
    {
        Debug.Log("hbhbhjbhj");
        db = new DatabaseManager();
        db.Start();

        userSnapshot = db.getUserData(cpfT);
    }

    public void setUserInformations()
    {
        userName.text = userSnapshot.Child("Name").Value.ToString();
        userEmail.text = userSnapshot.Child("Email").Value.ToString();
        userHierarchy.text = userSnapshot.Child("Hierarchy").Value.ToString();

        Debug.Log("ae poraaaaaa");

    }

}
