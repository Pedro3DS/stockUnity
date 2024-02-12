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
using Firebase.Storage;
using System.Threading;
using System.IO;

public class EmployeeScript : MonoBehaviour
{
    public TMP_InputField userName;
    public TMP_InputField userEmail;
    public TMP_Text userHierarchy;
    public RawImage userImg;

    public RawImage userBtnImg;


    private FirebaseStorage storage;
    private StorageReference storageReference;

    private string userAvatar;

    private DataSnapshot userSnapshot;

    private DatabaseManager db;
    private string cpfT = "123";

    async void Start()
    {
        db = new DatabaseManager();
        db.Start();

        userSnapshot = await db.getUserData(cpfT);
        StartCoroutine(setUserPhotoProfile());

        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://stockunity-46765.appspot.com");
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
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                string fileName = System.IO.Path.GetFileName(path);
                userAvatar = path;
                Debug.Log(userAvatar);
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path);
                userImg.texture = texture;
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
            }
        }, "Select a PNG image", "image/png");

        Debug.Log("Permission result: " + permission);
       
    }

    public async void updateUserInformations()
    {
        db.updateUserInformations(cpfT, userName.text, userEmail.text, userAvatar);

        StorageReference uploadRef = storageReference.Child(userAvatar);
        await uploadRef.PutFileAsync(userAvatar);

        userSnapshot = await db.getUserData(cpfT);
        StartCoroutine(setUserPhotoProfile());

    }

    private IEnumerator setUserPhotoProfile()
    {
        Debug.Log(userSnapshot.Child("ProfilePhoto").Value.ToString());
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(userSnapshot.Child("ProfilePhoto").Value.ToString());
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
