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
    public Sprite defaultPortrait;
    //public Image dialogueBackground;

    private DialogueData currentDialogueData;
    private int currentDialogueIndex;

    private float normalOpacity = 1f;
    private float dimOpacity = 0.5f;
    private string lastNpcName = "";
    private string currentNextScene = ""; // 存储选择后的目标场景

    public Image fadePanel;

    public AudioSource audioSource; // 用于播放所有音效
    public AudioSource bgmSource;   // 用于播放背景音乐

    public AudioClip nextDialogueSound; // 点击 Next 按钮时的音效
    public AudioClip choiceClickSound;  // 点击选项时的音效

    private Dictionary<string, AudioClip> sceneMusic = new Dictionary<string, AudioClip>(); // 存储场景背景音乐

    // Stores all virtual scenes
    private Dictionary<string, (Sprite, DialogueData)> sceneDatabase = new Dictionary<string, (Sprite, DialogueData)>();

    public static DialogueManager Instance;

    // Register a virtual scene
    public void RegisterScene(string sceneName, Sprite background, DialogueData dialogueData, AudioClip bgmClip)
    {
        if (!sceneDatabase.ContainsKey(sceneName))
        {
            sceneDatabase.Add(sceneName, (background, dialogueData));

            if (bgmClip != null)
            {
                sceneMusic[sceneName] = bgmClip; // 绑定场景音乐
                Debug.Log($"Scene music {bgmClip} successfully registered.");
            }

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
                StartCoroutine(SceneTransition(newBackground, newDialogueData)); // ✅ 执行渐变
            }
            else
            {
                StartDialogue(newDialogueData);
            }
            PlayBGM(sceneName);
        }
        else
        {
            Debug.LogError($"Scene {sceneName} is not registered!");
        }
    }
    private IEnumerator SceneTransition(Sprite newBackground, DialogueData newDialogueData)
    {
        yield return StartCoroutine(FadeToBlack()); // ✅ 变黑
        SetBackground(newBackground); // ✅ 切换背景
        yield return StartCoroutine(FadeToClear()); // ✅ 变亮

        StartDialogue(newDialogueData);
    }
    private IEnumerator FadeToBlack()
    {
        float duration = 0.5f; // 变黑时间
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            SetFadePanelAlpha(alpha);
            yield return null;
        }

        SetFadePanelAlpha(1f); // 确保最终完全黑
    }

    private IEnumerator FadeToClear()
    {
        float duration = 0.5f; // 变亮时间
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            SetFadePanelAlpha(alpha);
            yield return null;
        }

        SetFadePanelAlpha(0f); // 确保最终完全透明
    }

    private void SetFadePanelAlpha(float alpha)
    {
        if (fadePanel != null)
        {
            Color color = fadePanel.color;
            color.a = alpha;
            fadePanel.color = color;
        }
    }

    private void PlayBGM(string sceneName)
    {
        if (sceneMusic.ContainsKey(sceneName))
        {
            AudioClip newBgm = sceneMusic[sceneName];

            if (bgmSource == null)
            {
                Debug.LogError("❌ BGM AudioSource is NULL! Make sure it is assigned in the Inspector.");
                return;
            }

            if (bgmSource.clip != newBgm) // ✅ 防止重复播放相同音乐
            {
                Debug.Log($"🎵 Attempting to play BGM: {newBgm.name} for {sceneName}");

                bgmSource.clip = newBgm;
                bgmSource.loop = true; // ✅ 确保背景音乐循环播放
                bgmSource.volume = 1.0f; // ✅ 确保音量正常
                bgmSource.mute = false;  // ✅ 确保未静音
                bgmSource.Play();

                Debug.Log($"🎶 Now playing: {sceneName} BGM -> {newBgm.name}");
            }
            else
            {
                Debug.Log($"🔄 BGM for {sceneName} is already playing.");
            }
        }
        else
        {
            Debug.LogWarning($"⚠ No BGM found for {sceneName}");
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
            //AdjustDialogueBackground();

            if (nextDialogueSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(nextDialogueSound); // ✅ 播放对话推进音效
            }

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
            else if(dialogue.speaker == "Narrator")
            {
                playerNameText.text = "";
                npcNameText.text = "";
                playerPortrait.sprite = defaultPortrait;
                npcPortrait.sprite = defaultPortrait;
                SetPortraitOpacity(playerPortrait, dimOpacity);
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
        {        // ✅ 处理 followUpDialogues 结束后是否切换场景
            if (!string.IsNullOrEmpty(currentNextScene))
            {
                Debug.Log($"Follow-up dialogues finished. Switching to {currentNextScene}");
                LoadVirtualScene(currentNextScene); // ✅ 结束后自动切换
                currentNextScene = ""; // ✅ 清空，防止错误调用
            }
            else
            {
                ShowChoices();
            }
        }
    }
    //private void AdjustDialogueBackground()
    //{
    //    if (dialogueBackground != null && dialogueText != null)
    //    {
    //        // 获取文本的 `RectTransform`
    //        RectTransform textRect = dialogueText.GetComponent<RectTransform>();
    //        RectTransform bgRect = dialogueBackground.GetComponent<RectTransform>();

    //        // 让背景大小适应文本
    //        bgRect.sizeDelta = new Vector2(textRect.sizeDelta.x + 40f, textRect.sizeDelta.y + 20f);
    //    }
    //}


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
        if (choiceClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(choiceClickSound); // ✅ 播放选项点击音效
        }

        Debug.Log($"Player selected: {choice.text}"); // ✅ 记录玩家选项

        // 修改忠诚度（如果选项影响忠诚度）
        ApplyLoyaltyEffects(choice.text);
        currentNextScene = choice.nextScene;

        // ✅ 先播放 followUpDialogues，而不是直接切换场景
        if (choice.followUpDialogues != null && choice.followUpDialogues.Count > 0)
        {
            Debug.Log(" Playing follow-up dialogues before scene switch.");
            currentDialogueData.dialogues = choice.followUpDialogues;
            currentDialogueIndex = 0;
            ShowDialogue(); // ✅ 进入 follow-up 对话模式
            return; // ✅ 先播放对话，暂时不切换场景
        }

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

    private void ApplyLoyaltyEffects(string choiceText)
    {
        if (LoyaltyManager.Instance == null)
        {
            Debug.LogError("LoyaltyManager is NULL! Make sure it is in the scene.");
            return;
        }

        switch (choiceText)
        {
            case "Humble Reject":
                LoyaltyManager.Instance.ChangeLoyalty("Brutus", 2);
                LoyaltyManager.Instance.ChangeLoyalty("Cassius", 2);
                LoyaltyManager.Instance.ChangeLoyalty("Mark Antony", -2);
                break;

            case "Humble Acceptance":
                LoyaltyManager.Instance.ChangeLoyalty("Brutus", -1);
                LoyaltyManager.Instance.ChangeLoyalty("Cassius", -2);
                LoyaltyManager.Instance.ChangeLoyalty("Senate", -2);
                break;

            case "Arrogant Declaration ('I am Rome!')":
                LoyaltyManager.Instance.ChangeLoyalty("Brutus", -4);
                LoyaltyManager.Instance.ChangeLoyalty("Cassius", -2);
                break;

            case "Accept the Crown":
                LoyaltyManager.Instance.ChangeLoyalty("Brutus", -2);
                LoyaltyManager.Instance.ChangeLoyalty("Cassius", -2);
                LoyaltyManager.Instance.ChangeLoyalty("Senate", -2);
                LoyaltyManager.Instance.ChangeLoyalty("Mark Antony", 2);
                break;

            case "Refuse the Crown":
                LoyaltyManager.Instance.ChangeLoyalty("Brutus", 2);
                LoyaltyManager.Instance.ChangeLoyalty("Cassius", 1);
                LoyaltyManager.Instance.ChangeLoyalty("Senate", 2);
                LoyaltyManager.Instance.ChangeLoyalty("Mark Antony", -2);
                break;

            case "Publicly Condemn the Senate for Not Offering it":
                LoyaltyManager.Instance.ChangeLoyalty("Brutus", -4);
                LoyaltyManager.Instance.ChangeLoyalty("Cassius", -4);
                LoyaltyManager.Instance.ChangeLoyalty("Senate", -4);
                LoyaltyManager.Instance.ChangeLoyalty("Mark Antony", 3);
                break;

            case "Order Spies to Watch the Senate":
                LoyaltyManager.Instance.ChangeLoyalty("Brutus", -2);
                LoyaltyManager.Instance.ChangeLoyalty("Cassius", -2);
                LoyaltyManager.Instance.ChangeLoyalty("Senate", -2);
                LoyaltyManager.Instance.ChangeLoyalty("Mark Antony", 2);
                break;

            case "Ignore the Rumors":
                LoyaltyManager.Instance.ChangeLoyalty("Mark Antony", -2);
                break;

            case "Confront Brutus About His Loyalty":
                LoyaltyManager.Instance.ChangeLoyalty("Senate", -1);
                LoyaltyManager.Instance.ChangeLoyalty("Mark Antony", -1);
                break;

            case "Stay Home on March 15":
                LoyaltyManager.Instance.ChangeLoyalty("Senate", -1);
                break;

            default:
                Debug.Log("No loyalty change for this choice.");
                break;
        }
    }
    private void ShowGameEnding()
    {
        if (LoyaltyManager.Instance == null)
        {
            Debug.LogError(" LoyaltyManager is NULL! Cannot determine game ending.");
            return;
        }

        string ending = LoyaltyManager.Instance.DetermineGameEnding();
        Debug.Log(" Game Ending: " + ending);

        // 在对话框里显示游戏结局
        dialogueText.text = ending;

        // 禁用 "Next" 按钮（游戏结束）
        nextButton.gameObject.SetActive(false);
    }

}

