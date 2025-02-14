using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保不会被销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class Dialogue
    {
        public string speaker;  // Speaker's name
        public Sprite portrait; // Character portrait
        [TextArea(3, 10)] public string content; // Dialogue text
    }

    [System.Serializable]
    public class Choice
    {
        public string text; // Choice text
        public string nextScene; // Name of the next scene
        public List<Dialogue> followUpDialogues; // Follow-up dialogues
    }

    [System.Serializable]
    public class DialogueData
    {
        public List<Dialogue> dialogues; // List of dialogues
        public List<Choice> choices; // List of choices
    }
    public Image backgroundPanel;
    public Image playerPortrait;
    public Image npcPortrait;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;

    private DialogueData currentDialogueData;
    private int currentDialogueIndex;

    private float normalOpacity = 1f;
    private float dimOpacity = 0.5f;
    private string lastNpcName = ""; 

    // Stores all virtual scenes
    private Dictionary<string, (Sprite, DialogueData)> sceneDatabase = new Dictionary<string, (Sprite, DialogueData)>();

    public static DialogueManager Instance;

    // Register a virtual scene
    public void RegisterScene(string sceneName, Sprite background, DialogueData dialogueData)
    {
        if (!sceneDatabase.ContainsKey(sceneName))
        {
            sceneDatabase.Add(sceneName, (background, dialogueData));
            Debug.Log($"Scene {sceneName} successfully registered.");
        }
        else
        {
            Debug.LogWarning($" Scene {sceneName} is already registered!");
        }
    }

    // Load a virtual scene (only change background & dialogue, not Unity scenes)
    public void LoadVirtualScene(string sceneName)
    {
        if (sceneDatabase.ContainsKey(sceneName))
        {
            Debug.Log($"Switching to scene: {sceneName}");

            (Sprite newBackground, DialogueData newDialogueData) = sceneDatabase[sceneName];

            if (newBackground != null)
            {
                SetBackground(newBackground);
            }
            StartDialogue(newDialogueData);
        }
        else
        {
            Debug.LogError($"Scene {sceneName} is not registered!");
        }
    }

    public void SetBackground(Sprite newBackground)
    {
        if (newBackground != null)
        {
            backgroundPanel.sprite = newBackground;
        }
    }

    public void StartDialogue(DialogueData dialogueData)
    {
        currentDialogueData = dialogueData;
        currentDialogueIndex = 0;
        choicePanel.SetActive(false);
        ShowDialogue();
    }

    private void ShowDialogue()
    {
        if (currentDialogueIndex < currentDialogueData.dialogues.Count)
        {
            Dialogue dialogue = currentDialogueData.dialogues[currentDialogueIndex];
            dialogueText.text = dialogue.content;

            if (dialogue.speaker == "Player")
            {
                // 玩家发言，保持上一个 NPC 名字
                playerPortrait.sprite = dialogue.portrait;
                playerPortrait.gameObject.SetActive(true);
                npcPortrait.gameObject.SetActive(true); // NPC 头像仍然可见

                playerNameText.text = "Caesar"; // 玩家名字固定
                npcNameText.text = lastNpcName; // 保持上次 NPC 名字

                // 设置透明度：玩家 100%，NPC 50%
                SetPortraitOpacity(playerPortrait, normalOpacity);
                SetPortraitOpacity(npcPortrait, dimOpacity);
            }
            else
            {
                // NPC 说话，更新 NPC 名字，并存储为 `lastNpcName`
                npcPortrait.sprite = dialogue.portrait;
                npcPortrait.gameObject.SetActive(true);
                playerPortrait.gameObject.SetActive(true); // 玩家头像始终可见

                playerNameText.text = "Caesar"; // 玩家名字固定
                npcNameText.text = dialogue.speaker; // 显示 NPC 名字
                lastNpcName = dialogue.speaker; // 记录 NPC 名字，供下次使用

                // 设置透明度：NPC 100%，玩家 50%
                SetPortraitOpacity(npcPortrait, normalOpacity);
                SetPortraitOpacity(playerPortrait, dimOpacity);
            }

            nextButton.gameObject.SetActive(true);
            choicePanel.SetActive(false);
        }
        else
        {
            ShowChoices();
        }
    }


    private void SetPortraitOpacity(Image portrait, float opacity)
    {
        if (portrait != null)
        {
            Color color = portrait.color;
            color.a = opacity;
            portrait.color = color;
        }
    }

    public void NextDialogue()
    {
        currentDialogueIndex++;
        ShowDialogue();
    }

    private void ShowChoices()
    {
        nextButton.gameObject.SetActive(false);
        choicePanel.SetActive(true);

        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Choice choice in currentDialogueData.choices)
        {
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choicePanel.transform);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            choiceButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnChoiceSelected(choice);
            });
        }
    }

    private void OnChoiceSelected(Choice choice)
    {
        if (!string.IsNullOrEmpty(choice.nextScene))
        {
            Debug.Log($"Attempting to switch to: {choice.nextScene}");

            if (sceneDatabase.ContainsKey(choice.nextScene))
            {
                LoadVirtualScene(choice.nextScene);
            }
            else
            {
                Debug.LogError($"Scene {choice.nextScene} is NOT registered!");
            }
        }
        else if (choice.followUpDialogues != null && choice.followUpDialogues.Count > 0)
        {
            currentDialogueData.dialogues = choice.followUpDialogues;
            currentDialogueIndex = 0;
            ShowDialogue();
        }
        else
        {
            choicePanel.SetActive(false);
        }
    }

}

