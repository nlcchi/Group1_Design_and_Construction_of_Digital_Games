using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class TemporaryFinalScene : MonoBehaviour
{
    public static TemporaryFinalScene Instance; // ✅ 添加单例

    public UnityEngine.UI.Button replayButton; // ✅ 通过命名空间明确指定 UI Button
    public Sprite AssassinationEndingbackground;
    public Sprite RomeUnderCaesarbackground;
    public Sprite BrutusTurnsAgainstYoubackground;
    public Sprite ExileEndingbackground;
    public Sprite TheBloodyTyrantEndingbackground;
    public AudioClip AssassinationMusic;
    public AudioClip RomeUnderCaesarMusic;
    public AudioClip BrutusTurnsAgainstYouMusic;
    public AudioClip ExileMusic;
    public AudioClip TheBloodyTyrantMusic;

    void Awake()
    {
        // ✅ 确保 `Instance` 是唯一的
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 防止重复
        }

        enabled = false; // 默认关闭
    }
    void Start()
    {
        enabled = false;    //
        StartCoroutine(WaitForDialogueManager());
    }

    IEnumerator WaitForDialogueManager()
    {
        while (DialogueManager.Instance == null || LoyaltyManager.Instance == null)
        {
            yield return null;
        }
        Debug.LogError("Check RegisterTemporaryFinalScene");
        RegisterTemporaryFinalScene();
    }

    void RegisterTemporaryFinalScene()
    {
        Debug.Log("Current forced ending from DialogueManager: " + DialogueManager.Instance.currentForcedEnding);
        if (DialogueManager.Instance == null)
        {
            Debug.LogError(" DialogueManager Instance is NULL in TemporaryFinalScene!");
            return;
        }

        if (LoyaltyManager.Instance == null)
        {
            Debug.LogError(" LoyaltyManager Instance is NULL! Cannot determine ending.");
            return;
        }

        string finalEnding = DialogueManager.Instance.currentForcedEnding;
        if (!string.IsNullOrEmpty(finalEnding))
        {
            Debug.Log($"✅ Forced Ending Applied: {finalEnding}");
        }
        else
        {
            finalEnding = LoyaltyManager.Instance.DetermineGameEnding();
            Debug.Log($"🎭 Using loyalty-based ending: {finalEnding}");
        }

        Sprite finalBackground = AssassinationEndingbackground; // 默认背景
        AudioClip finalMusic = AssassinationMusic; // 默认音乐
        List<DialogueManager.Dialogue> endingDialogues = new List<DialogueManager.Dialogue>();

        // ✅ 根据结局选择不同的背景和文本
        if (finalEnding == "Caesar is assassinated by the Senate.")
        {
            finalBackground = AssassinationEndingbackground;
            finalMusic = AssassinationMusic;
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "The Senate chambers are filled with whispers, tension thick in the air." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "Then, chaos erupts. Daggers flash. The Ides of March ends in blood." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "Caesar, betrayed by those he called friends, collapses onto the cold marble floor." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "Rome mourns—or celebrates. The Republic breathes anew, but at what cost?" });
        }
        else if (finalEnding == "Caesar consolidates power and controls Rome.")
        {
            finalBackground = RomeUnderCaesarbackground;
            finalMusic = RomeUnderCaesarMusic;
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "The Senate halls are silent as Caesar takes his place upon the seat of power." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "The Republic, weakened and fractured, bends to his will." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "Mark Antony stands by his side, the legions sworn to their command." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "Rome has a ruler once more, but is it salvation or the birth of an empire of tyranny?" });
        }
        else if (finalEnding == "Rome falls into chaos and civil war erupts.")
        {
            finalBackground = BrutusTurnsAgainstYoubackground;
            finalMusic = BrutusTurnsAgainstYouMusic;
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "Without a strong guiding hand, the Republic fractures into warring factions." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "Brutus, Cassius, and Antony carve Rome into their own dominions." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "The streets of the Eternal City run red as legions turn against their own." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "The Senate’s dream of democracy is buried beneath the cries of the dying." });
        }
        else if (finalEnding == "Exile Ending – Caesar Flees Before Assassination")
        {
            finalBackground = ExileEndingbackground;
            finalMusic = ExileMusic;
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "A storm brews over Rome, but Caesar is nowhere to be found." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "Betrayed, hunted, and knowing his fate, he flees into the wilderness beyond the Republic's reach." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "From afar, he watches as Rome tears itself apart, each faction claiming legitimacy." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "Perhaps one day, he will return to reclaim what was his." });
        }
        else
        {
            finalBackground = TheBloodyTyrantEndingbackground;
            finalMusic = TheBloodyTyrantMusic;
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "The Senate chambers are emptied, its members either in chains or dead." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "The people of Rome whisper in fear, for their ruler brooks no opposition." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "Brutus and Cassius lie broken, and Antony ensures loyalty through the might of the legions." });
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = "The Republic is no more. Only Caesar remains, ruling with an iron fist." });
        }

        // ✅ Narrator 最后播报所有角色的忠诚度
        if (LoyaltyManager.Instance != null)
        {
            string loyaltySummary = "As the dust settles, the loyalty of those around Caesar is revealed: ";

            int brutusLoyalty = LoyaltyManager.Instance.GetLoyalty("Brutus");
            int cassiusLoyalty = LoyaltyManager.Instance.GetLoyalty("Cassius");
            int markAntonyLoyalty = LoyaltyManager.Instance.GetLoyalty("Mark Antony");
            int senateLoyalty = LoyaltyManager.Instance.GetLoyalty("Senate");

            loyaltySummary += $"Brutus: {brutusLoyalty}, ";
            loyaltySummary += $"Cassius: {cassiusLoyalty}, ";
            loyaltySummary += $"Mark Antony: {markAntonyLoyalty}, ";
            loyaltySummary += $"Senate: {senateLoyalty}";

            //if (replayButton != null
            //    && DialogueManager.Instance != null
            //    && DialogueManager.Instance.GetCurrentSceneName() == "TemporaryFinalScene"
            //    && DialogueManager.Instance.GetCurrentDialogueIndex() >= endingDialogues.Count - 1)
            //{
            //    replayButton.gameObject.SetActive(true);
            //    Debug.Log("🎯 Replay button is now visible.");
            //}

            Debug.Log($"📊 Final Loyalty Summary: {loyaltySummary}");
            endingDialogues.Add(new DialogueManager.Dialogue { speaker = "Narrator", content = loyaltySummary });
        }
        else
        {
            Debug.LogError("❌ LoyaltyManager.Instance is NULL in TemporaryFinalScene!");
        }


        DialogueManager.DialogueData sceneDialogue = new DialogueManager.DialogueData
        {
            dialogues = endingDialogues
        };

        DialogueManager.Instance.RegisterScene("TemporaryFinalScene", finalBackground, sceneDialogue, finalMusic);
    }

    // ✅ 重播游戏的方法
    public void ReplayGame()
    {
        Debug.Log("🔄 Replay button clicked. Resetting game state...");

        // ✅ 停止所有正在播放的音乐
        if (DialogueManager.Instance != null && DialogueManager.Instance.audioSource != null)
        {
            DialogueManager.Instance.audioSource.Stop();
            Debug.Log("🔇 All background music and sounds have been stopped.");
        }
        else
        {
            Debug.LogWarning("⚠️ AudioSource is NULL. Cannot stop sounds.");
        }

        // ✅ 重置所有角色的忠诚度
        if (LoyaltyManager.Instance != null)
        {
            LoyaltyManager.Instance.ResetAllLoyalty();
            Debug.Log("🧹 All character loyalties have been reset.");
        }
        else
        {
            Debug.LogWarning("⚠️ LoyaltyManager instance is NULL. Cannot reset loyalties.");
        }

        // ✅ 跳转到主菜单场景 (真实的 Unity Scene)
        SceneManager.LoadScene("Menu");
    }

    public void ActivateFinalSceneScript()
    {
        enabled = true;
        Debug.Log("✅ TemporaryFinalScene 已激活！");
    }
}

