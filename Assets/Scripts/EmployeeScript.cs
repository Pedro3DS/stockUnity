using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using System;
using UnityEngine.Networking;

public class EmployeeScript : MonoBehaviour
{
    public TMP_InputField userName;
    public TMP_InputField userEmail;
    public TMP_Text userHierarchy;
    public RawImage userImg;

    private DataSnapshot userSnapshot;

    private DatabaseManager db;
    private string cpfT = "123";

    async void Start()
    {
        db = new DatabaseManager();
        db.Start();

        userSnapshot = await db.getUserData(cpfT);
    }

    public void setUserInformations()
    {
        userImg = gameObject.GetComponent<RawImage>();
        userName.text = userSnapshot.Child("Name").Value.ToString();
        userEmail.text = userSnapshot.Child("Email").Value.ToString();
        if (userSnapshot.Child("Hierarchy").Value.ToString() == "employee")
        {
            userHierarchy.text = "Funcionario Normal";
        }
        StartCoroutine(setUserPhotoProfile());
        
    }

    private IEnumerator setUserPhotoProfile()
    {
        Debug.Log(userSnapshot.Child("ProfilePhoto").Value.ToString());
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://firebasestorage.googleapis.com/v0/b/stockunity-46765.appspot.com/o/" + userSnapshot.Child("ProfilePhoto").Value.ToString() + "?alt=media&token=");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            userImg.texture = texture;
        }
    }

}
