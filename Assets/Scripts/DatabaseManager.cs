using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;

public class DatabaseManager
{

    private DatabaseReference db;
    private DataSnapshot userData;
    private string GloabalCpf;
    
    public void Start()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void getCpf(string cpf)
    {
        GloabalCpf = cpf;
    }

    public async Task<string> checkHierarchy()
    {

        DatabaseReference newUserData = db.Child("users").Child(GloabalCpf).Child("Hierarchy");
        DataSnapshot snapshot = await newUserData.GetValueAsync();
        if (snapshot.Exists)
        {
            return snapshot.Value.ToString();
        }
        return null;
    }
}
