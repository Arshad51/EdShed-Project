using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayController : MonoBehaviour
{
    [System.Serializable]
    public class GameStats
    {
        public string word;
        public string sentence;
        public float difficulty;
        public List<Phoneme> phonics;
    }

    private List<Word> WordsList; // ← Assign this in the inspector or from your API
    private Word currentWord;
    private GameStats currentGameStats;

    private int currentWordIndex = -1;
    private bool isGameActive = false;
    private int currentLevel = 0;

    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Button showStatsButton;
    [SerializeField] private Button closeStatsButton;
    [SerializeField] private GameObject statsContainer;
    [SerializeField] private TextMeshProUGUI levelText;

    void Start()
    {
        statsContainer.SetActive(false);
        showStatsButton.onClick.AddListener(() => { statsContainer.SetActive(true); });
        closeStatsButton.onClick.AddListener(() => { statsContainer.SetActive(false); });

        InitializeGame();
    }

    public void InitializeGame()
    {
        WordsList = DataLoaderManager.instance.GetRootData().list.words;

        if (WordsList == null || WordsList.Count == 0)
        {
            Debug.LogError("WordsList is empty!");
            return;
        }

        isGameActive = true;

        currentWordIndex = Random.Range(0, WordsList.Count);
        currentWord = WordsList[currentWordIndex];
        currentLevel++;

        // Parse sentence JSON string to List
        List<string> sentences = ParseJsonArray(currentWord.sentences);
        string chosenSentence = sentences[Random.Range(0, sentences.Count)];

        // Create game stats
        currentGameStats = new GameStats
        {
            word = currentWord.text,
            sentence = chosenSentence,
            difficulty = currentWord.difficulty_index,
            phonics = currentWord.dictionary.phonics
        };

        DisplayStats();
    }

    public void CheckAnswer(string playerInput)
    {
        if (!isGameActive) return;

        if (playerInput.Trim().ToLower() == currentGameStats.word.Trim().ToLower())
        {
            CorrectAnswer();
        }
        else
        {
            WrongAnswer();
        }
    }

    private void CorrectAnswer()
    {
        Debug.Log("Correct!");
        DisplayReward();
        ProgressToNextWord();
    }

    private void WrongAnswer()
    {
        Debug.Log("Wrong! Showing reward screen anyway.");
        DisplayReward();
        isGameActive = false;
    }

    private void DisplayReward()
    {
        // Replace this with your reward UI
        Debug.Log("🎉 Displaying reward screen!");
    }

    private void DisplayStats()
    {
        if (statsText == null)
        {
            Debug.LogWarning("Stats Text UI is not assigned!");
            return;
        }

        string sentenceWithBlank = currentGameStats.sentence.Replace("*", "_____");
        string phonicsText = "";

        foreach (var p in currentGameStats.phonics)
        {
            phonicsText += $"\n - Phoneme: {p.phoneme}, Grapheme: {p.grapheme}";
        }

        string finalText =
            $"<b>Word:</b> {currentGameStats.word}\n" +
            $"<b>Sentence:</b> {sentenceWithBlank}\n" +
            $"<b>Difficulty:</b> {currentGameStats.difficulty}\n" +
            $"<b>Phonics:</b>{phonicsText}";

        statsText.text = finalText;
    }

    private void ProgressToNextWord()
    {
        InitializeGame();
    }

    // Helper to parse stringified JSON arrays like ["sentence1","sentence2"]
    private List<string> ParseJsonArray(string jsonArray)
    {
        string wrapped = "{\"array\":" + jsonArray + "}";
        SentenceWrapper wrapper = JsonUtility.FromJson<SentenceWrapper>(wrapped);
        return wrapper.array;
    }

    [System.Serializable]
    private class SentenceWrapper
    {
        public List<string> array;
    }
}
