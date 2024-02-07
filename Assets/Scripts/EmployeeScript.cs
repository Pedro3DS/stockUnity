using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EmployeeScript : MonoBehaviour
{
    //Buttons
    public GameObject homeButton;
    public GameObject managementButton;
    public GameObject listButton;
    public GameObject employeesButton;
    public GameObject userButton;

    //Activated Buttons Sprites
    public GameObject homeButtonActivated;
    public GameObject listButtonActivated;
    public GameObject managementButtonActivated;
    public GameObject employeesButtonActivated;
    public GameObject userButtonActivated;

    private string userCpf;
    void Start()
    {
        userCpf = PlayerPrefs.GetString("Cpf");
    }


    public void actuvatedButtonEffect(string btnName)
    {
        Debug.Log("ç,çl,çl");
        switch (btnName)
        {
            case "home":
                homeButton.SetActive(false);
                homeButtonActivated.SetActive(true);
                break;
            default:
                Debug.Log("sdfghjk");
                break;
        }
            
    }
}
