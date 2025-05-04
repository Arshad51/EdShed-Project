using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DataLoaderManager : MonoBehaviour
{
    public static DataLoaderManager instance;

    public RootObject rootData;

    private static string API = "https://api.edshed.com/lists/Y12";

    //Add a function to display what text to display while loading... 

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void Initialize()
    {
        StartCoroutine(FetchJson(API));
    }

    IEnumerator FetchJson(string url)
    {
        yield return new WaitForSeconds(0.5f);

        LoadingManager.instance.UpdateText("Sending Request...");

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load data: " + request.error);
            LoadingManager.instance.UpdateText("Failed to load data");
        }
        else
        {
            LoadingManager.instance.UpdateText("Data Received Successfully");

            string json = request.downloadHandler.text;
            RootObject data = JsonUtility.FromJson<RootObject>(json);

            if (data != null)
            {
                Debug.Log("Loaded list title: " + data.list.title);
                Debug.Log("First word: " + data.list.words[0].text);
                LoadingManager.instance.UpdateText("Data Loaded Successfully");

                rootData = data;
            }
            else
            {
                Debug.LogError("Failed to parse JSON.");
                LoadingManager.instance.UpdateText("Failed to load data");
            }
        }

        yield return new WaitForSeconds(0.5f);

        LoadingManager.instance.HideLoadingScreen();
    }

    public RootObject GetRootData()
    {
        return rootData;
    }
}
