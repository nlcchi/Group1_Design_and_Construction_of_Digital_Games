using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Unity.Burst.Intrinsics.X86;
using System.Security.Cryptography;
using System;

public class Scene3Dialogue : MonoBehaviour
{
    public Sprite playerSprite;
    public Sprite BrutusSprite;
    public Sprite CassiusSprite;
    public Sprite MarkAntonySprite;
    public Sprite SenateSprite;
    public Sprite backgroundScene3;
    public AudioClip scene3Music;

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

        RegisterScene3();
    }

    void RegisterScene3()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager Instance is NULL in Scene3Dialogue!");
            return;
        }


        DialogueManager.DialogueData sceneDialogue = new DialogueManager.DialogueData
        {
            dialogues = new List<DialogueManager.Dialogue>
            {
                new DialogueManager.Dialogue { speaker = "Narrator", content = "Tensions rise within Rome. Your spies bring troubling news some senators are meeting in secret. Cassius has been seen whispering to Brutus. Discussions of 'saving the Republic' are growing louder." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "If you do nothing, the plot may strengthen. If you take action, you risk proving their fears right." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "Brutus remains distant. You don¡¯t know if he¡¯s involved¡­ yet." },
                new DialogueManager.Dialogue { speaker = "Cassius", portrait = CassiusSprite, content = "The Republic is dying, Brutus. Will you let it?" },
                new DialogueManager.Dialogue { speaker = "Brutus", portrait = BrutusSprite, content = "Caesar is my friend¡­ but Rome must come first." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "Will you spy on them, confront Brutus, or ignore the threat?" }
            },
            choices = new List<DialogueManager.Choice>
            {
                new DialogueManager.Choice { text = "Order Spies to Watch the Senate", nextScene = "Scene4",
                    followUpDialogues = new List<DialogueManager.Dialogue>
                {
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "You uncover the names of the conspirators, the truth now laid bare before you." },
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "You hold the power to decide¡ªarrest them, bribe them, or eliminate them in silence..." }

            } },
                new DialogueManager.Choice { text = "Ignore the Rumors", nextScene = "Scene4",
                    followUpDialogues = new List<DialogueManager.Dialogue>
                {
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "The assassination plot advances in the shadows, unnoticed and unchecked." }
                   
            } },
                new DialogueManager.Choice { text = "Confront Brutus About His Loyalty", nextScene = "Scene4",
                    followUpDialogues = new List<DialogueManager.Dialogue>
                {
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "Brutus wrestles with doubt¡ªhis convictions fragile, his loyalty still swayed... Maybe?" },
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "But if your relations are mishandled, Brutus will turn against you, his trust shattered beyond repair." }

            } },
            }

        };

        DialogueManager.Instance.RegisterScene("Scene3", backgroundScene3, sceneDialogue, scene3Music);
    }
}
