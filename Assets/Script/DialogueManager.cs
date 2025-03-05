using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public Button replayButton; // ✅ Replay 按钮
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
    private void Start()
    {
        // ✅ 确保 Replay 按钮在开始时隐藏
        if (replayButton != null)
        {
            replayButton.gameObject.SetActive(false);
            Debug.Log("🔒 Replay button is hidden at the start.");
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
        public string forcedEnding = ""; // ✅ 新增：如果这个选项强制指定结局，则存储结局名称
    }

    [System.Serializable]
    public class DialogueData
    {
        public List<Dialogue> dialogues; // List of dialogues
        public List<Choice> choices; // List of choices
    }

    // ✅ 修改对话历史的存储结构，保存头像和角色信息
    [System.Serializable]
    public class DialogueHistoryEntry
    {
        public string speaker;
        public string content;
        public Sprite portrait;
    }

    private List<DialogueHistoryEntry> dialogueHistory = new List<DialogueHistoryEntry>();

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

    public Button reviewButton; // ✅ 回看按钮

    //private List<string> dialogueHistory = new List<string>(); // ✅ 存储对话历史
    private int reviewIndex = -1; // ✅ 当前回看的对话索引，-1 表示未处于回看模式

    private DialogueData currentDialogueData;
    private int currentDialogueIndex;

    private float normalOpacity = 1f;
    private float dimOpacity = 0.1f;
    private string lastNpcName = "";
    private string currentNextScene = ""; // 存储选择后的目标场景
    private string lastSceneName = ""; // ✅ 存储上一个场景名称
    public string currentForcedEnding { get; private set; }  // ✅ 存储强制结局

    public Image fadePanel;

    public AudioSource audioSource; // 用于播放所有音效
    public AudioSource bgmSource;   // 用于播放背景音乐

    public AudioClip nextDialogueSound; // 点击 Next 按钮时的音效
    public AudioClip choiceClickSound;  // 点击选项时的音效

    private Dictionary<string, AudioClip> sceneMusic = new Dictionary<string, AudioClip>(); // 存储场景背景音乐

    // Stores all virtual scenes
    private Dictionary<string, (Sprite, DialogueData)> sceneDatabase = new Dictionary<string, (Sprite, DialogueData)>();

    public static DialogueManager Instance;
    public string GetCurrentSceneName()
    {
        return lastSceneName; // 使用 lastSceneName 作为当前虚拟场景名称
    }

    public int GetCurrentDialogueIndex()
    {
        return currentDialogueIndex;
    }
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
        if (lastSceneName == sceneName)
        {
            Debug.LogWarning($"⚠️ Scene '{sceneName}' is already active. Skipping redundant load.");
            // 激活replay按钮（这里利用了一个bug，是取巧的做法）
            if (sceneName == "TemporaryFinalScene")
            {
                if (replayButton != null)
                {
                    replayButton.gameObject.SetActive(true);
                    Debug.Log("🎯 Replay button is now visible.");
                }
                else
                {
                    Debug.LogWarning("⚠️ Replay button is not assigned in the Inspector.");
                }
            }
            return;
        }

        lastSceneName = sceneName;
        Debug.Log($"🔄 Loading virtual scene: {sceneName}");

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
            // ✅ 清除历史对话记录，防止跨场景查看旧对话
            dialogueHistory.Clear();
            reviewIndex = -1;
            Debug.Log("🧹 Cleared dialogue history after scene switch.");
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
        // ✅ 处理“回看模式”
        if (reviewIndex >= 0)
        {
            if (reviewIndex < dialogueHistory.Count)
            {
                DialogueHistoryEntry entry = dialogueHistory[reviewIndex];
                dialogueText.text = $"{entry.speaker}: {entry.content}";
                Debug.Log($"📜 Reviewing dialogue: {entry.speaker}: {entry.content}");

                UpdatePortraitsAndNames(entry.speaker, entry.portrait);
                PlayClickSound();
                reviewIndex++; // 在“回看模式”中前进
                return; // ✅ 在“回看模式”中，阻止正常对话逻辑执行
            }
            else
            {
                reviewIndex = -1; // ✅ 自动退出“回看模式”
                currentDialogueIndex = Mathf.Min(currentDialogueIndex, dialogueHistory.Count);
                Debug.Log("🔄 Exiting review mode, resuming normal dialogue flow.");
            }
        }

        // ✅ 正常对话模式
        if (currentDialogueIndex < currentDialogueData.dialogues.Count)
        {
            Dialogue dialogue = currentDialogueData.dialogues[currentDialogueIndex];
            dialogueText.text = dialogue.content;

            dialogueHistory.Add(new DialogueHistoryEntry
            {
                speaker = dialogue.speaker,
                content = dialogue.content,
                portrait = dialogue.portrait
            });
            Debug.Log($"New dialogue shown: {dialogue.speaker}: {dialogue.content}");
            Debug.Log(" currentDialogueData.dialogues.Count:" + currentDialogueData.dialogues.Count);

            UpdatePortraitsAndNames(dialogue.speaker, dialogue.portrait);
            PlayClickSound();
            nextButton.gameObject.SetActive(true);
            choicePanel.SetActive(false);
        }
        else
        {
            if (!string.IsNullOrEmpty(currentNextScene))
            {
                Debug.Log($"Follow-up dialogues finished. Switching to {currentNextScene}");
                LoadVirtualScene(currentNextScene);
                currentNextScene = "";

                dialogueHistory.Clear();
                reviewIndex = -1;
                Debug.Log("🧹 Cleared dialogue history after scene switch.");
            }
            else
            {
                ShowChoices();


            }
        }
    }


    // ✅ 进入“回看模式”，从最后一条对话开始回看
    public void ReviewPreviousDialogue()
    {
        if (dialogueHistory.Count == 0)
        {
            Debug.LogWarning("⚠️ No dialogue history to review.");
            return;
        }

        if (choicePanel.activeSelf)
        {
            choicePanel.SetActive(false);
            nextButton.gameObject.SetActive(true);
        }

        if (reviewIndex == -1)
        {
            reviewIndex = Mathf.Max(0, dialogueHistory.Count - 1); // ✅ 从最后一条开始回看
        }
        else if (reviewIndex > 0)
        {
            reviewIndex--;
        }

        if (reviewIndex >= 0 && reviewIndex < dialogueHistory.Count)
        {
            DialogueHistoryEntry entry = dialogueHistory[reviewIndex];
            dialogueText.text = $"{entry.speaker}: {entry.content}";
            Debug.Log($"📜 Reviewing previous dialogue: {entry.speaker}: {entry.content}");

            // ✅ 同步显示头像和名字
            UpdatePortraitsAndNames(entry.speaker, entry.portrait);
            PlayClickSound();
        }
    }

    // ✅ 在正常对话模式和回看模式之间切换
    public void ShowNextDialogue()
    {
        if (reviewIndex >= 0)
        {
            reviewIndex++; // 退出回看模式
            if (reviewIndex >= dialogueHistory.Count)
            {
                reviewIndex = -1; // 结束回看，恢复正常对话播放
                ShowDialogue();
            }
            else
            {
                DialogueHistoryEntry entry = dialogueHistory[reviewIndex];
                dialogueText.text = $"{entry.speaker}: {entry.content}";
                Debug.Log($"📜 Forward reviewing dialogue: {entry.speaker}: {entry.content}");

                // ✅ 同步显示头像和名字
                UpdatePortraitsAndNames(entry.speaker, entry.portrait);
            }
        }
        else
        {
            ShowDialogue();
        }
    }
    // ✅ 统一处理头像和名字的显示
    private void UpdatePortraitsAndNames(string speaker, Sprite portrait)
    {
        if (speaker == "Player")
        {
            playerPortrait.sprite = portrait;
            playerPortrait.gameObject.SetActive(true);
            npcPortrait.gameObject.SetActive(true);

            playerNameText.text = "Caesar";
            npcNameText.text = lastNpcName;

            SetPortraitOpacity(playerPortrait, normalOpacity);
            SetPortraitOpacity(npcPortrait, dimOpacity);
        }
        else if (speaker == "Narrator")
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
            npcPortrait.sprite = portrait;
            npcPortrait.gameObject.SetActive(true);
            playerPortrait.gameObject.SetActive(true);

            playerNameText.text = "Caesar";
            npcNameText.text = speaker;
            lastNpcName = speaker;

            SetPortraitOpacity(npcPortrait, normalOpacity);
            SetPortraitOpacity(playerPortrait, dimOpacity);
        }
    }

    private void PlayClickSound()
    {
        if (nextDialogueSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(nextDialogueSound);
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
        if (choiceClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(choiceClickSound); // ✅ 播放选项点击音效
        }

        Debug.Log($"Player selected: {choice.text}"); // ✅ 记录玩家选项
                                                      // ✅ 存储强制结局，如果该选项指定了 `forcedEnding`
        if (!string.IsNullOrEmpty(choice.forcedEnding))
        {
            currentForcedEnding = choice.forcedEnding;
            Debug.Log($"🎭 Forced Ending Set: {currentForcedEnding}");
        }
        else
        {
            currentForcedEnding = ""; // ✅ 清空之前的强制结局
        }
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
                LoyaltyManager.Instance.ChangeLoyalty("Mark Antony", 3);
                break;

            case "Refuse the Crown":
                LoyaltyManager.Instance.ChangeLoyalty("Brutus", 2);
                LoyaltyManager.Instance.ChangeLoyalty("Cassius", 1);
                LoyaltyManager.Instance.ChangeLoyalty("Senate", 3);
                LoyaltyManager.Instance.ChangeLoyalty("Mark Antony", -2);
                break;

            case "Condemn the Senate for Not Offering it":
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

            case "Fake Your Death to Catch the Conspirators":
                LoyaltyManager.Instance.ChangeLoyalty("Senate", 1);
                LoyaltyManager.Instance.ChangeLoyalty("Mark Antony", 1);
                break;

            default:
                Debug.Log("No loyalty change for this choice.");
                break;
        }
    }


}

