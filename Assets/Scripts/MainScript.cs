using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour
{
    public TMP_InputField inpCpf;

    private DatabaseManager database;
    // Start is called before the first frame update
    void Start()
    {
        database = new DatabaseManager();
        database.Start();
    }

    public void getHierarchy()
    {
        var cpf = inpCpf.text.Replace(".", "").Replace("-", "");
        database.getCpf(cpf);

        string hierarchy = database.checkHierarchy();
        Debug.Log(hierarchy);
        if (hierarchy == null)
        {
            Debug.Log("CPF não encotrado");
        }
        else if(hierarchy == "employee")
        {
            SceneManager.LoadScene("EmployeeScene");
        }
        else if (hierarchy == "manager")
        {
            SceneManager.LoadScene("ManagerScene");
        }
        else if (hierarchy == "director")
        {
            SceneManager.LoadScene("DirectorScene");
        }
    }
}
