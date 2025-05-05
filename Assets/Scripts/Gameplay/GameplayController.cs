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

    private List<Word> WordsList;
    private Word currentWord;
    private GameStats currentGameStats;
    private List<SnapSlot> snapSlots = new List<SnapSlot>();

    private int currentWordIndex = -1;
    private bool isGameActive = false;
    private int currentLevel = -1;
    private int lives = 3;


    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI rewardText;

    [SerializeField] private Button showStatsButton;
    [SerializeField] private Button closeStatsButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button tryAgainButton;

    [SerializeField] private GameObject statsContainer;
    [SerializeField] private GameObject rewardContainer;
    [SerializeField] private GameObject snapSlotObject;
    [SerializeField] private GameObject draggableObject;
    [SerializeField] private Transform snapSlotContainer;

    void Start()
    {
        statsContainer.SetActive(false);
        rewardContainer.SetActive(false);
        showStatsButton.onClick.AddListener(() => { statsContainer.SetActive(true); });
        closeStatsButton.onClick.AddListener(() => { statsContainer.SetActive(false); });
        backButton.onClick.AddListener(() => { SceneLoader.instance.LoadMainScene(); });
        confirmButton.onClick.AddListener(() => { CheckAnswer(); });
        tryAgainButton.onClick.AddListener(() => 
        {
            currentLevel = -1;
            statsContainer.SetActive(false);
            rewardContainer.SetActive(false);
            InitializeGame();
        });

        InitializeGame();
    }

    public void InitializeGame()
    {
        if (currentLevel == -1)
        {
            lives = 3;
        }

        UpdateLifeText();

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
        confirmButton.interactable = true;

        List<string> sentences = ParseJsonArray(currentWord.sentences);
        string chosenSentence = sentences[Random.Range(0, sentences.Count)];

        currentGameStats = new GameStats
        {
            word = currentWord.text,
            sentence = chosenSentence,
            difficulty = currentWord.difficulty_index,
            phonics = currentWord.dictionary.phonics
        };

        snapSlotContainer.GetComponent<GridLayoutGroup>().enabled = true;

        foreach (Transform child in snapSlotContainer)
        {
            Destroy(child.gameObject);
        }

        snapSlots.Clear();

        foreach (var character in currentWord.text)
        {
            var snapSlotGO = Instantiate(snapSlotObject, snapSlotContainer);
            var snapSlot = snapSlotGO.GetComponent<SnapSlot>();
            snapSlot.Value = "";
            snapSlot.CorrectValue = character.ToString();
            snapSlots.Add(snapSlot);
        }

        StartCoroutine(SpawnDraggables());

        UpdateStats();
    }

    private IEnumerator SpawnDraggables()
    {
        yield return new WaitForSeconds(1);

        snapSlotContainer.GetComponent<GridLayoutGroup>().enabled = false;

        foreach (Transform child in snapSlotContainer)
        {
            RectTransform rect = child.GetComponent<RectTransform>();
            Vector3 worldPos = rect.position;

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

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
            obj.GetComponent<Draggable>().Initialize(character.ToString());
        }
    }

    public void CheckAnswer()
    {
        confirmButton.interactable = false;

        if (!isGameActive) return;

        string playerInput = "";
        bool allCorrect = true;

        for (int i = 0; i < snapSlots.Count; i++)
        {
            string correctChar = currentGameStats.word[i].ToString().ToLower();
            string userChar = snapSlots[i].Value.ToLower();

            bool isCorrect = correctChar == userChar;
            snapSlots[i].SetColor(isCorrect ? Color.green : Color.red);

            if (!isCorrect) allCorrect = false;

            playerInput += userChar;
        }

        Debug.Log($"Player input: {playerInput} | Correct word: {currentGameStats.word}");

        if (allCorrect)
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
        StartCoroutine(ShowCorrectFeedbackAndContinue());
    }

    private void WrongAnswer()
    {
        lives--;
        UpdateLifeText();

        Debug.Log("Wrong!");

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
            isGameActive = false;
            DisplayReward(); // Show reward ONLY when out of lives
        }
        else
        {
            // Re-enable confirm button so the player can try again
            confirmButton.interactable = true;
        }
    }


    private void DisplayReward()
    {
        Debug.Log("🎉 Showing reward screen because all lives are used.");
        // TODO: Show your end screen or celebration UI here

        var prevHighScoreStr = UserDataManager.instance.GetHighScore();
        int prevHighScore = 0;
        int.TryParse(prevHighScoreStr, out prevHighScore); // safely convert string to int

        if (currentLevel > prevHighScore)
        {
            UserDataManager.instance.SaveHighScore(currentLevel.ToString());
        }

        rewardContainer.SetActive(true);
        rewardText.text = "Here's your score: " + currentLevel;
    }

    private IEnumerator ShowCorrectFeedbackAndContinue()
    {
        // Wait 3 seconds to show green slots
        yield return new WaitForSeconds(2f);

        // Show the correct word in the sentence
        string filledSentence = currentGameStats.sentence.Replace("*", $"<b><color=green>{currentGameStats.word}</color></b>");
        questionText.text = filledSentence;

        // Wait 2 more seconds to show updated sentence
        yield return new WaitForSeconds(3f);

        ProgressToNextWord();
    }

    private void ProgressToNextWord()
    {
        InitializeGame();
    }

    private void UpdateStats()
    {
        if (statsText == null) return;

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
        questionText.text = sentenceWithBlank;
        levelText.text = "Level: " + currentLevel.ToString();
    }

    private void UpdateLifeText()
    {
        if (lifeText != null)
        {
            lifeText.text = $"Lives: {lives}";
        }
    }

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
