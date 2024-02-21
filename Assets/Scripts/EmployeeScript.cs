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
using Unity.VisualScripting;

public class EmployeeScript : MonoBehaviour
{
    /*User Panel*/
    [Header("User Panel")]
    public TMP_InputField userName;
    public TMP_InputField userEmail;
    public TMP_Text userHierarchy;
    public RawImage userImg;


    /*Employees Panel*/
    [Header("Employees Panel")]
    public GameObject usersBtnModel;
    public Transform usersAreaPanel;
    public Dropdown usersHierarchyChoices;
    public GameObject removeUserConfirm;
    public UnityEngine.UI.Button removeUserBtnConfirm;
    private GameObject userToRemove;

    public TMP_Text newUserNameAlert;
    public TMP_Text newUserEmailAlert;
    public TMP_Text newUserCpfAlert;

    public TMP_InputField newUserName;
    public TMP_InputField newUserEmail;
    public TMP_InputField newUserCpf;
    public TMP_Dropdown newUserHierarchy;

    /*List Panel*/
    [Header("List Panel")]
    public GameObject productBtnModel;
    public Transform productAreaPanel;
    private GameObject selectedProduct;
    public GameObject editProductPanel;
    public TMP_Text editProductCode;
    public DataSnapshot selectedProductInfo;
    public UnityEngine.UI.Button removeProductBtnConfirm;
    public GameObject removeProductPanel;

    public GameObject addProductPanel;
    public TMP_Text addProductCode;
    private string newProductCode;
    public TMP_InputField addProductName;
    public TMP_InputField addProductValue;
    public TMP_InputField addProductQuantity;
    public TMP_Text addProductNameAlert;

    [Header("Home Panel")]
    public GameObject lowProductBtnModel;
    public Transform lowProductAreaPanel;

    /*All Panel*/
    [Header("Geral Panel's")]
    public RawImage userBtnImg;


    private FirebaseStorage storage;
    private StorageReference storageReference;

    private DataSnapshot userSnapshot;

    private DatabaseManager db;

    public UnityEvent Function_onPicked_Return; // Visual Scripting trigger [On Unity Event]
    public UnityEvent Function_onSaved_Return; // Visual Scripting trigger [On Unity Event]

    public NativeFilePicker.Permission permission; // Permission to access Camera Roll


    async void Start()
    {
        
        db = new DatabaseManager();
        db.Start();

        userSnapshot = await db.getUserData(PlayerPrefs.GetString("Cpf"));
        StartCoroutine(setUserPhotoProfile());

        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://stockunity-46765.appspot.com");

        homePanel();

    }

    public void setUserInformations()
    {
        userName.text = userSnapshot.Child("Name").Value.ToString();
        userEmail.text = userSnapshot.Child("Email").Value.ToString();
        if (userSnapshot.Child("Hierarchy").Value.ToString() == "employee")
        {
            userHierarchy.text = "Gerente";
        }
        else
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
                StorageReference uploadRef = storageReference.Child("uploads/" + PlayerPrefs.GetString("Cpf") + ".jpeg");
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
        db.updateUserInformations(PlayerPrefs.GetString("Cpf"), userName.text, userEmail.text);

        userSnapshot = await db.getUserData(PlayerPrefs.GetString("Cpf"));
    }

    public async void usersPanel()
    {
        clearUsersAreaPanel();

        DataSnapshot usersSnapshots = await db.getUsersDatas();

        foreach (var usersKeys in usersSnapshots.Children) 
        {
            createUsersInfos(usersKeys.Key, usersSnapshots.Child(usersKeys.Key).Child("Name").Value.ToString(), usersSnapshots.Child(usersKeys.Key).Child("Hierarchy").Value.ToString());
        }

    }

    private void clearUsersAreaPanel()
    {
        Transform childTransforms = usersAreaPanel.GetComponentInChildren<Transform>();

        foreach (Transform childTransform in childTransforms)
        {
            Destroy(childTransform.gameObject);
        }
    }

    private void createUsersInfos(string cpf, string name, string hierarchy)
    {
        GameObject newUserInfotmation = Instantiate(usersBtnModel, usersAreaPanel);
        newUserInfotmation.SetActive(true);
        newUserInfotmation.name = cpf;
        if (hierarchy == "employee")
        {
            hierarchy = "Gerente";
        }
        else
        {
            hierarchy = "Funcionario Normal";
        }
        newUserInfotmation.GetComponentInChildren<TMP_Text>().text = $"{name}\n{hierarchy}";
        StartCoroutine(setEmployeesPhotoProfile(cpf, newUserInfotmation));
        
    }

    public async void setRemoveUserName(GameObject thisObject)
    {
        removeUserConfirm.SetActive(true);
        DataSnapshot removeUserSnapshot = await db.getUserData(thisObject.name);
        if (userSnapshot.Key == thisObject.name)
        {
            removeUserConfirm.GetComponentInChildren<TMP_Text>().text = $"Você não pode se deletar";
            removeUserBtnConfirm.gameObject.SetActive(false);
        }
        else
        {
            removeUserConfirm.GetComponentInChildren<TMP_Text>().text = $"Deseja remover o usuario {removeUserSnapshot.Child("Name").Value.ToString()}";
            removeUserBtnConfirm.gameObject.SetActive(true);
        } 
        Debug.Log(thisObject);
        userToRemove = thisObject.gameObject;

    }

    public void removeUser()
    {
        db.removeUser(userToRemove.name);
        Destroy(userToRemove);
        removeUserConfirm.SetActive(false);
    }

    public void createNewUser()
    {
        string name = newUserName.text;
        string email = newUserEmail.text;
        string cpf = newUserCpf.text;
        int hierarchyNum = newUserHierarchy.value;
        string hierarchy = "";

        if (hierarchyNum == 0)
        {
            hierarchy = "employee";
        }
        else
        {
            hierarchy = "normalEmployee";
        }

        if (name == "" || email == "" || cpf == "")
        {
            Debug.Log("Preencha todos os campos");
            newUserCpfAlert.gameObject.SetActive(true);
            newUserNameAlert.gameObject.SetActive(true);
            newUserEmailAlert.gameObject.SetActive(true);

            newUserEmailAlert.text = "Todos os campos precisam ser preencidos.";
            newUserCpfAlert.text = "Todos os campos precisam ser preencidos.";

        }
        else if (!email.Contains("@"))
        {
            newUserEmailAlert.gameObject.SetActive(true);
            newUserEmailAlert.text = "O Email está no formato errado (falta do @)";
            newUserCpfAlert.gameObject.SetActive(false);
            newUserNameAlert.gameObject.SetActive(false);
        }
        else if (cpf.Length < 11)
        {
            newUserCpfAlert.gameObject.SetActive(true);
            newUserCpfAlert.text = "O CPf não pode ter menos de 11 caracteres.";
            newUserEmailAlert.gameObject.SetActive(false);
            newUserNameAlert.gameObject.SetActive(false);
        }
        else
        {
            db.createNewUser(cpf, name, email, hierarchy);
            newUserEmailAlert.gameObject.SetActive(false);
            newUserNameAlert.gameObject.SetActive(false);
            newUserCpfAlert.gameObject.SetActive(false);

            newUserName.text = "";
            newUserEmail.text = "";
            newUserCpf.text = "";
        }

    }

    public async void listPanel()
    {
        clearListAreaPanel();

        DataSnapshot productsSnapshots = await db.getProductsData();

        foreach (var productKeys in productsSnapshots.Children)
        {
            string code = productKeys.Key;
            string name = productsSnapshots.Child(productKeys.Key).Child("Name").Value.ToString();
            string value = productsSnapshots.Child(productKeys.Key).Child("Value").Value.ToString();
            string quantity = productsSnapshots.Child(productKeys.Key).Child("Quantity").Value.ToString();
            createListInfos(code, name, value, quantity);
        }

    }

    public void searchProducts(TMP_InputField searchInp)
    {
        string searchString = searchInp.text.FirstCharacterToUpper();
        Transform childTransforms = productAreaPanel.GetComponentInChildren<Transform>();
        
        if (searchString != "")
        {
            foreach (Transform childTransform in childTransforms)
            {
                if (!childTransform.name.Contains(searchString))
                {
                    childTransform.gameObject.SetActive(false);
                }
                else
                {
                    childTransform.gameObject.SetActive(true);
                }

            }
        }
        else
        {
            foreach (Transform childTransform in childTransforms)
            {
                childTransform.gameObject.SetActive(true);

            }
        }
    }

    public async void setSelectedProduct(GameObject thisProduct)
    {
        editProductPanel.SetActive(true);
        DataSnapshot products = await db.getProductsData();

        selectedProductInfo = products.Child(thisProduct.GetComponentsInChildren<TMP_Text>()[0].text);

        editProductCode.text = "Código do produto: " + selectedProductInfo.Key;
        editProductPanel.GetComponentsInChildren<TMP_InputField>()[0].text = selectedProductInfo.Child("Name").Value.ToString();
        editProductPanel.GetComponentsInChildren<TMP_InputField>()[1].text = selectedProductInfo.Child("Value").Value.ToString();
        editProductPanel.GetComponentsInChildren<TMP_InputField>()[2].text = selectedProductInfo.Child("Quantity").Value.ToString();

        StartCoroutine(setProductPhoto(selectedProductInfo.Child("ProductPhoto").Value.ToString())); ;

        selectedProduct = editProductPanel.gameObject;
    }

    public void changeProductPhoto()
    {
        StartCoroutine(ShowLoadDialogCoroutineProduct());
    }

    public void updateProductInfotmation()
    {
        string code = selectedProductInfo.Key;
        string Name = selectedProduct.GetComponentsInChildren<TMP_InputField>()[0].text;
        string Value = selectedProduct.GetComponentsInChildren<TMP_InputField>()[1].text;
        string Quantity = selectedProduct.GetComponentsInChildren<TMP_InputField>()[2].text;
        db.updateProduct(code, Name, Value, Quantity);
        listPanel();
    }

    public void removeProduct()
    {
        db.removeProduct(selectedProductInfo.Key);
        selectedProduct.SetActive(false);
        removeProductPanel.SetActive(false);
        listPanel();
    }

    public void createProductArea()
    {
        System.Random rand = new System.Random();
        int num1 = rand.Next(1, 9);
        int num2 = rand.Next(1, 9);
        int num3 = rand.Next(1, 9);
        newProductCode = $"78940028{num1}{num2}{num3}{num1 + num2 + num3}";
        addProductPanel.SetActive(true);
        addProductCode.text = "O código do produto irá ser: " + newProductCode;
    }

    public void createNewProduct()
    {
        string name = addProductName.text.FirstCharacterToUpper();
        string value = addProductValue.text.FirstCharacterToUpper();
        string quantity = addProductQuantity.text.FirstCharacterToUpper();

        if (value == "")
        {
            value = "0";
        }
        if (quantity == "")
        {
            quantity = "0";
        }
        if (name == "")
        {
            addProductNameAlert.text = "O produto nescessita de um nome";
        }
        else
        {
            db.createProduct(newProductCode, name, value, quantity);
            addProductNameAlert.text = "";
            addProductName.text = "";
            addProductValue.text = "";
            addProductQuantity.text = "";

            listPanel();
        }   
    }

    public async void homePanel()
    {
        clearHomeAreaPanel();

        DataSnapshot productsSnapshots = await db.getProductsData();

        foreach (var productKeys in productsSnapshots.Children)
        {
            if (Int32.Parse(productsSnapshots.Child(productKeys.Key).Child("Quantity").Value.ToString()) <= 50)
            {
                string code = productKeys.Key;
                string name = productsSnapshots.Child(productKeys.Key).Child("Name").Value.ToString();
                string value = productsSnapshots.Child(productKeys.Key).Child("Value").Value.ToString();
                string quantity = productsSnapshots.Child(productKeys.Key).Child("Quantity").Value.ToString();
                createHomeInfos(code, name, value, quantity);
            }
            
        }

    }

    private void clearHomeAreaPanel()
    {
        Transform childTransforms = lowProductAreaPanel.GetComponentInChildren<Transform>();

        foreach (Transform childTransform in childTransforms)
        {
            Destroy(childTransform.gameObject);
        }
    }

    private void createHomeInfos(string code, string name, string value, string quantity)
    {
        GameObject newUserInfotmation = Instantiate(lowProductBtnModel, lowProductAreaPanel);
        newUserInfotmation.SetActive(true);
        newUserInfotmation.name = name;
        newUserInfotmation.GetComponentsInChildren<TMP_Text>()[0].text = code;
        newUserInfotmation.GetComponentsInChildren<TMP_Text>()[1].text = name;
        newUserInfotmation.GetComponentsInChildren<TMP_Text>()[2].text = "R$ " + value;
        newUserInfotmation.GetComponentsInChildren<TMP_Text>()[3].text = "Quantidade: " + quantity;

        StartCoroutine(setProductsPhotos(code, newUserInfotmation));

    }

    private void clearListAreaPanel()
    {
        Transform childTransforms = productAreaPanel.GetComponentInChildren<Transform>();

        foreach (Transform childTransform in childTransforms)
        {
            Destroy(childTransform.gameObject);
        }
    }

    private void createListInfos(string code, string name, string value, string quantity)
    {
        GameObject newUserInfotmation = Instantiate(productBtnModel, productAreaPanel);
        newUserInfotmation.SetActive(true);
        newUserInfotmation.name = name;
        newUserInfotmation.GetComponentsInChildren<TMP_Text>()[0].text = code;
        newUserInfotmation.GetComponentsInChildren<TMP_Text>()[1].text = name;
        newUserInfotmation.GetComponentsInChildren<TMP_Text>()[2].text = "R$ " + value;
        newUserInfotmation.GetComponentsInChildren<TMP_Text>()[3].text = "Quantidade: " + quantity;

        StartCoroutine(setProductsPhotos(code, newUserInfotmation));

    }

    IEnumerator ShowLoadDialogCoroutineProduct()
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
                StorageReference uploadRef = storageReference.Child("products/" + selectedProductInfo.Child("ProductPhoto").Value.ToString());
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
                        StartCoroutine(setProductPhoto(selectedProductInfo.Child("ProductPhoto").Value.ToString()));
                    }
                });
            }
            else
            {
                Debug.Log("File selection canceled");
            }
        });
    }

    private IEnumerator setProductPhoto(string productPhoto)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://firebasestorage.googleapis.com/v0/b/stockunity-46765.appspot.com/o/products%2F" + productPhoto + "?alt=media&token=");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            selectedProduct.gameObject.GetComponentInChildren<RawImage>().texture = texture;
        }
    }

    private IEnumerator setProductsPhotos(string productCode, GameObject productObject)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://firebasestorage.googleapis.com/v0/b/stockunity-46765.appspot.com/o/products%2F" + productCode + ".jpeg?alt=media&token=");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            productObject.gameObject.GetComponentInChildren<RawImage>().texture = texture;
        }
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

    private IEnumerator setEmployeesPhotoProfile(string cpf, GameObject employeeProfilePhoto)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://firebasestorage.googleapis.com/v0/b/stockunity-46765.appspot.com/o/uploads%2F" + cpf + ".jpeg?alt=media&token=");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            employeeProfilePhoto.GetComponentInChildren<RawImage>().texture = texture;

        }
    }




}
