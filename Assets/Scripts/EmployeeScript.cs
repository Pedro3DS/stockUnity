using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EmployeeScript : MonoBehaviour
{


    private string userCpf;
    void Start()
    {
        userCpf = PlayerPrefs.GetString("Cpf");
    }


}
