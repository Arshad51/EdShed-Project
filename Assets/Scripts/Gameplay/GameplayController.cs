using System.Collections.Generic;
using System.Collections;
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
    [SerializeField] private GameObject snapSlotObject;
    [SerializeField] private GameObject draggableObject;
    [SerializeField] private Transform snapSlotContainer;

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

        snapSlotContainer.GetComponent<GridLayoutGroup>().enabled = true;

        foreach (var character in currentWord.text)
        {
            Instantiate(snapSlotObject, snapSlotContainer);
        }

        StartCoroutine(SpawnDraggables());

        DisplayStats();
    }

    private IEnumerator SpawnDraggables()
    {
        yield return new WaitForSeconds(1);

        snapSlotContainer.GetComponent<GridLayoutGroup>().enabled = false;

        // Set anchor to middle center for each snapSlotObject
        foreach (Transform child in snapSlotContainer)
        {
            RectTransform rect = child.GetComponent<RectTransform>();

            // Store the world position before anchor change
            Vector3 worldPos = rect.position;

            // Change anchor preset to Middle Center
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            // Apply world position again to maintain visual location
            rect.position = worldPos;

        }

        foreach (var character in currentWord.text)
        {
            Vector2 spawnPos;

            do
            {
                float x = Random.Range(-490f, 490f);
                float y = Random.Range(-240f, 191f);
                spawnPos = new Vector2(x, y);
            }
            while (spawnPos.x >= -362f && spawnPos.x <= 362f &&
                   spawnPos.y >= -102f && spawnPos.y <= 102f);

            GameObject obj = Instantiate(draggableObject, snapSlotContainer);
            obj.GetComponent<RectTransform>().anchoredPosition = spawnPos;
        }
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
