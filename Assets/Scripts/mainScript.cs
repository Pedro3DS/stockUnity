using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mainScript : MonoBehaviour
{

    public Button btnLogin;
    public Text txtAlert;
    // Start is called before the first frame update
    void Start()
    {
        btnLogin.onClick.AddListener(showCpfAlert);
    }

    // Update is called once per frame
    public void showCpfAlert()
    {
        txtAlert.text = "asdfgbhnjmk,l.";
    }
}
