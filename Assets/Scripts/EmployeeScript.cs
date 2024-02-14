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
using UnityEngine.Events;
using System.Globalization;
using Firebase.Extensions;
using NativeFilePickerNamespace;
using Unity.VisualScripting;

public class EmployeeScript : MonoBehaviour
{
    public TMP_InputField userName;
    public TMP_InputField userEmail;
    public TMP_Text userHierarchy;
    public RawImage userImg;

    public GameObject usersAreaPanel;

    public RawImage userBtnImg;


    private FirebaseStorage storage;
    private StorageReference storageReference;


    private DataSnapshot userSnapshot;

    private DatabaseManager db;
    private string cpfT = "123";

    public UnityEvent Function_onPicked_Return; // Visual Scripting trigger [On Unity Event]
    public UnityEvent Function_onSaved_Return; // Visual Scripting trigger [On Unity Event]

    public NativeFilePicker.Permission permission; // Permission to access Camera Roll


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

    public void changeUserPhoto()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {


        yield return NativeFilePicker.PickFile((paths) =>
        {
            if (paths != null && paths.Length > 0)
            {
                Debug.Log("File Selected: " + paths[0]);

                // Read the selected file into bytes
                byte[] bytes = File.ReadAllBytes(paths);

                // Editing Metadata
                var newMetadata = new MetadataChange();
                newMetadata.ContentType = "image/jpeg";

                // Create a reference to where the file needs to be uploaded
                StorageReference uploadRef = storageReference.Child("uploads/" + cpfT + ".jpeg");
                Debug.Log("File upload started");

                // Upload the file to Firebase Storage
                uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError(task.Exception.ToString());
                    }
                    else
                    {
                        Debug.Log("File Uploaded Successfully!");
                        StartCoroutine(setUserPhotoProfile());
                    }
                });
            }
            else
            {
                Debug.Log("File selection canceled");
            }
        });
    }

    public async void updateUserInformations()
    {
        db.updateUserInformations(cpfT, userName.text, userEmail.text);

        userSnapshot = await db.getUserData(cpfT);
        StartCoroutine(setUserPhotoProfile());

    }

    public void createUsersButtonUsersArea()
    {
    }

    private IEnumerator setUserPhotoProfile()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://firebasestorage.googleapis.com/v0/b/stockunity-46765.appspot.com/o/uploads%2F" + userSnapshot.Child("ProfilePhoto").Value.ToString() + "?alt=media&token=");
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
