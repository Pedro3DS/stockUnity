using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour
{
    public TMP_InputField inpCpf;
    public TMP_Text txtAlert;

    private DatabaseManager database;
    void Start()
    {
        database = new DatabaseManager();
        database.Start();
    }

    public async void starByHierarchy()
    {
        var cpf = inpCpf.text.Replace(".", "").Replace("-", "");
        PlayerPrefs.SetString("Cpf" ,"123");
        database.getCpf(cpf);

        string hierarchy = await database.checkHierarchy();
        if (hierarchy != "null")
        {
            switch (hierarchy)
            {
                case "employee":
                    SceneManager.LoadScene("EmployeeScene");
                    break;
                case "manager":
                    SceneManager.LoadScene("ManagerScene");
                    break;
                case "director":
                    SceneManager.LoadScene("DirectorScene");
                    break;
                default:
                    break;
            }
        }
        else
        {
            txtAlert.enabled = true;
            txtAlert.text = "CPF n�o encontrado";
        }
    }    
}
