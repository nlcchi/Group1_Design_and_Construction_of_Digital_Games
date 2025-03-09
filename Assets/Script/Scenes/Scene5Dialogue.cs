using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Unity.Burst.Intrinsics.X86;
using System.Security.Cryptography;
using System;

public class Scene5Dialogue : MonoBehaviour
{
    public Sprite backgroundScene5;
    public AudioClip scene5Music;

    void Start()
    {
        if (DialogueManager.Instance == null)
        {
            RegisterScene5(); // ✅ 如果实例已存在，直接注册场景

        }
        else
        {
            StartCoroutine(WaitForDialogueManager());
        }
    }

    IEnumerator WaitForDialogueManager()
    {
        while (DialogueManager.Instance == null)
        {
            yield return null;
        }

        RegisterScene5();
    }

    void RegisterScene5()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager Instance is NULL in Scene5Dialogue!");
            return;
        }


        DialogueManager.DialogueData sceneDialogue = new DialogueManager.DialogueData
        {
            dialogues = new List<DialogueManager.Dialogue>
            {
                new DialogueManager.Dialogue { speaker = "Narrator", content = "The sun rises on the Ides of March. The city is tense. Your allies await your final decision." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "Mark Antony warns you not to go to the Senate, but Brutus and Cassius insist it is just another day. The Senate chambers are filled with men who may want you dead." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "The decision you make now will determine the fate of Rome—and your own survival." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "The moment has come. Will you walk into history, or shape your own destiny?" },
                new DialogueManager.Dialogue { speaker = "Narrator", content = " " }
            },
            choices = new List<DialogueManager.Choice>
            {
                new DialogueManager.Choice { text = "You make your way to the Senate", nextScene = "TemporaryFinalScene" },
                new DialogueManager.Choice { text = "Avoid the Senate", nextScene = "TemporaryFinalScene" },
                new DialogueManager.Choice { text = "Escape from Rome", nextScene = "TemporaryFinalScene", forcedEnding = "Exile Ending – Caesar Flees Before Assassination" },
                new DialogueManager.Choice { text = "Strike first: Conspirators fall", nextScene = "TemporaryFinalScene", forcedEnding = "The Bloody Tyrant Ending – Rule by Fear" }
            }


        };

        DialogueManager.Instance.RegisterScene("Scene5", backgroundScene5, sceneDialogue, scene5Music);
    }

}
