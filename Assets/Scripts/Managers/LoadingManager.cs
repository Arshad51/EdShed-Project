using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject connectionLostScreen;
    [SerializeField] private Button retryButton;

    //Add a function to display what text to display while loading... 

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        retryButton.onClick.AddListener(RetryButton);
        StartCoroutine(CheckInternetConnectionFirstTimeCoroutine());

        InvokeRepeating("CheckInternetConnection", 15f, 15f); // Check every 5 seconds

        ShowLoadingScreen();

        UpdateText("Loading Content...");

        DataLoaderManager.instance.Initialize();
    }

    private void CheckInternetConnection()
    {
        StartCoroutine(CheckInternetConnectionCoroutine());
    }

    public void UpdateText(string text)
    {
        loadingText.text = text;
    }

    public void ShowLoadingScreen()
    {
        loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        loadingScreen.SetActive(false);
    }

    public void RetryButton()
    {
        connectionLostScreen.SetActive(false);

        //Router.LoginState();

        ShowLoadingScreen();

        UpdateText("Checking Internet Connection...!");

        StartCoroutine(RetryCheckInternetConnectionCoroutine());
    }

    IEnumerator CheckInternetConnectionCoroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                connectionLostScreen.SetActive(true);

                Debug.Log("No internet connection: " + request.error);
            }
            else
            {
                Debug.Log("Internet is available");
            }
        }
    }

    IEnumerator RetryCheckInternetConnectionCoroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                connectionLostScreen.SetActive(true);

                Debug.Log("No internet connection: " + request.error);
            }
            else
            {
                Debug.Log("Internet is available");

                HideLoadingScreen();
            }
        }
    }

    IEnumerator CheckInternetConnectionFirstTimeCoroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                connectionLostScreen.SetActive(true);

                Debug.Log("No internet connection: " + request.error);
            }
            else
            {
                Debug.Log("Internet is available");
            }
        }
    }
}
