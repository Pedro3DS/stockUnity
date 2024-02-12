using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using System;
using UnityEngine.Networking;
using static NativeGallery;

public class EmployeeScript : MonoBehaviour
{
    public TMP_InputField userName;
    public TMP_InputField userEmail;
    public TMP_Text userHierarchy;
    public RawImage userImg;

    public RawImage userBtnImg;


    private DataSnapshot userSnapshot;

    private DatabaseManager db;

    async void Start()
    {
        db = new DatabaseManager();
        db.Start();

        userSnapshot = await db.getUserData(PlayerPrefs.GetString("Cpf"));
        StartCoroutine(setUserPhotoProfile());
    }

    public void setUserInformations()
    {
        userName.text = userSnapshot.Child("Name").Value.ToString();
        userEmail.text = userSnapshot.Child("Email").Value.ToString();
        if (userSnapshot.Child("Hierarchy").Value.ToString() == "employee")
        {
            userHierarchy.text = "Funcionario Normal";
        }
        StartCoroutine(setUserPhotoProfile());
        
    }

    public void changeUserProfilePhoto()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            Texture2D texture = NativeGallery.LoadImageAtPath(path);
            if (texture == null)
            {
                Debug.Log("Couldn't load texture from " + path);
                return;
            }
            userImg.texture = texture;


        });
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
            userBtnImg.texture = texture;
            userImg.texture = texture;
        }
    }

}
