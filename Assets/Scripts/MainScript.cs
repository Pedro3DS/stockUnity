using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        string hierarchyValue = "";

        database.checkHierarchy((string hierarchy) =>
        {
            Debug.Log(hierarchy);
            if (hierarchy == null)
            {
                Debug.Log("Cpf não cadastrado");
            }
            else
            {
                hierarchyValue = hierarchyValue + hierarchy; 
            }
        });
        loadSceneHierarchy(hierarchyValue);
    }

    private void loadSceneHierarchy(string hierarchyValue)
    {
         switch (hierarchyValue)
         {
            case "employee":
                SceneManager.LoadSceneAsync("EmployeeScene");
                break;
            case "manager":
                SceneManager.LoadSceneAsync("ManagerScene");
                break;
            case "director":
                SceneManager.LoadSceneAsync("DirectorScene");
                break;
            default:
                break;
         }
    }
}
