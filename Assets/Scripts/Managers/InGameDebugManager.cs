using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameDebugManager : MonoBehaviour
{
    public static InGameDebugManager instance;

    [SerializeField] GameObject DebugWindow;
    [SerializeField] Button CloseButton;

    [SerializeField] private Text logText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] VerticalLayoutGroup verticalLayout;

    private List<string> logMessages = new List<string>();

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
        CloseButton.onClick.AddListener(() => { DebugWindow.SetActive(false); });
    }

    void Update()
    {
        if (Input.touchCount >= 3 || Input.GetKeyDown(KeyCode.H))
        {
            DebugWindow.SetActive(true);
            verticalLayout.enabled = false;
            verticalLayout.enabled = true;
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type != LogType.Error || type != LogType.Exception)
        {
            logMessages.Add($"{type}: {logString}\n");
        }

        UpdateLogText();
    }

    void UpdateLogText()
    {
        logText.text = string.Join("", logMessages.ToArray());
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f; // Scroll to bottom
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
}
