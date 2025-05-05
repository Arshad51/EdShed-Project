using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI highScoreText;

    public void Start()
    {
        startButton.onClick.AddListener(SceneLoader.instance.LoadGameplayScene);

        //load data from local (PlayerPrefs)
        var score = UserDataManager.instance.GetHighScore();

        if (score == null)
        {
            score = "0";
        }

        highScoreText.text = "HighScore: " + score;
;
    }
}
