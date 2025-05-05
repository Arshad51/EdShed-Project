using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager instance;

    //Add a function to display what text to display while loading... 

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void SaveHighScore(string score)
    {
        PlayerPrefs.SetString("score", score);
    }
    public string GetHighScore()
    {
        return PlayerPrefs.GetString("score");
    }
}
