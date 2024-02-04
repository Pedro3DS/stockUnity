using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DatabaseManager : MonoBehaviour
{
    public TMP_InputField inpCpf;
    public string employeeScene;
    public string managerScene;
    public string directorScene;


    private DatabaseReference db;
    private DataSnapshot userData;
    void Start()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void checkHierarchy()
    {
        string fCpf = inpCpf.text.Replace(".", "").Replace("-", "");
        db.Child("users").Child(fCpf).GetValueAsync().ContinueWith(result =>
        {
            if (result.Result.Value == null)
            {
                Debug.Log("CPF não encontrado");
            }
            else if (result.IsCompleted)
            {
                userData = result.Result;
                
            }
            else if (result.IsFaulted)
            {
                Debug.Log("Erro na coneccao");
            }
        });
        string hierarchy = userData.Child("Hierarchy").Value.ToString();
        if (hierarchy == "employee")
        {
            SceneManager.LoadScene(employeeScene);

        }
        else if (hierarchy == "manager")
        {
            SceneManager.LoadScene(managerScene);
        }
        else if (hierarchy == "director")
        {
            SceneManager.LoadScene(directorScene);
        }
    }

    public void getUserData()
    {
        string fCpf = inpCpf.text.Replace(".", "").Replace("-", "");
        db.Child("users").Child(fCpf).GetValueAsync().ContinueWith(result =>
        {
            if (result.IsCompleted)
            {
                DataSnapshot userData = result.Result;
                Debug.Log("asfdghjklçjhgfdsfgjkhgfdsfghjgfd");
                
            }
            else
            {
                Debug.Log("Error");
            }
        });
    }
}
