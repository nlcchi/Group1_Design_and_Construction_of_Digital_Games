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
        StartCoroutine(WaitForDialogueManager());
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
                new DialogueManager.Dialogue { speaker = "Narrator", content = "The decision you make now will determine the fate of Rome¡ªand your own survival." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "The moment has come. Will you walk into history, or shape your own destiny?" }
            },
            choices = new List<DialogueManager.Choice>
            {
                new DialogueManager.Choice { text = "You make your way to the Senate", nextScene = "sceneFinal" },
                new DialogueManager.Choice { text = "Avoid the Senate", nextScene = "sceneFinal" },
                new DialogueManager.Choice { text = "Turn Brutus Against the Conspirators", nextScene = "sceneFinal"},
                new DialogueManager.Choice { text = "Strike first: Conspirators fall before any action.", nextScene = "sceneFinal"}
            }

        };

        DialogueManager.Instance.RegisterScene("Scene5", backgroundScene5, sceneDialogue, scene5Music);
    }
}
