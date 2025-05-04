using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager instance;

    public string highScore;

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
        highScore = PlayerPrefs.GetString("score");
    }


    public void SaveHighScore(string score)
    {
        PlayerPrefs.SetString("score", score);
    }
    public string GetHighScore()
    {
        return highScore;
    }
}
