using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scene2Dialogue : MonoBehaviour
{
    public Sprite playerSprite;
    public Sprite CiceroSprite;
    public Sprite backgroundScene2;
    public AudioClip scene2Music;

    void Start()
    {
        StartCoroutine(WaitForDialogueManager());
    }

    IEnumerator WaitForDialogueManager()
    {
        // 等待 `DialogueManager` 加载
        while (DialogueManager.Instance == null)
        {
            yield return null;
        }

        RegisterScene2();
    }

    void RegisterScene2()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager Instance is NULL in Scene2Dialogue!");
            return;
        }

        DialogueManager.DialogueData sceneDialogue = new DialogueManager.DialogueData
        {
            dialogues = new List<DialogueManager.Dialogue>
            {
                new DialogueManager.Dialogue { speaker = "Cicero", portrait = CiceroSprite, content = "Welcome to the Senate, Caesar." },
                new DialogueManager.Dialogue { speaker = "Player", portrait = playerSprite, content = "Let history be made today." }
            },
            choices = new List<DialogueManager.Choice>
            {
                new DialogueManager.Choice { text = "Side with Mark Antony", nextScene = "Scene3" },
                new DialogueManager.Choice { text = "Remain neutral", nextScene = "Scene3" }
            }
        };

        DialogueManager.Instance.RegisterScene("Scene2", backgroundScene2, sceneDialogue, scene2Music);
        Debug.Log("Scene2 successfully registered.");
    }
}
