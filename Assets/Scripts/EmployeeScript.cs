using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EmployeeScript : MonoBehaviour
{
    public GameObject buttonActivated;
    public Image iconToDisapear;

    private string userCpf;
    void Start()
    {
        userCpf = PlayerPrefs.GetString("Cpf");
    }

    public void actuvatedButtonEffect()
    {
        iconToDisapear.SetEnabled(false);
        buttonActivated.SetActive(true);
    }
}
