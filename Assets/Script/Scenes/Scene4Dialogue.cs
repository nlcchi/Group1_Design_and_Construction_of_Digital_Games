using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Unity.Burst.Intrinsics.X86;
using System.Security.Cryptography;
using System;

public class Scene4Dialogue : MonoBehaviour
{
    public Sprite playerSprite;
    public Sprite BrutusSprite;
    public Sprite CassiusSprite;
    public Sprite MarkAntonySprite;
    public Sprite SenateSprite;
    public Sprite CalpurniaSprite;
    public Sprite backgroundScene4;
    public AudioClip scene4Music;

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

        RegisterScene4();
    }

    void RegisterScene4()
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
                new DialogueManager.Dialogue { speaker = "Narrator", content = "A storm brews over Rome. A soothsayer warns you: 'Beware the Ides of March.' That night, Calpurnia, your wife, wakes in terror from a dream of your blood spilling in the Senate." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "Your closest allies react differently: \n  Mark Antony urges you to ignore superstition.\n  Brutus watches carefully, weighing his fears.\n  Cassius laughs, dismissing it as nonsense." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "Are these mere superstitions, or signs of real danger?" },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "Fate whispers its warning. Will you listen?" },
                new DialogueManager.Dialogue { speaker = "Player", portrait = playerSprite,  content ="You Romans love your omens. Thunder rumbles, a dog howls, and suddenly the gods are speaking?" },
                new DialogueManager.Dialogue { speaker = "Calpurnia", portrait = CalpurniaSprite, content = "I dreamt of blood, Caesar. Your blood, spilling in the Senate! The gods are warning us¡ªstay home today!" },
                new DialogueManager.Dialogue { speaker = "Player", portrait = playerSprite,  content ="A dream, a warning¡­ tell me, my friends, should I cower in my chambers like a frightened child?" },
                new DialogueManager.Dialogue { speaker = "Mark Antony", portrait = MarkAntonySprite, content = "Superstitions! You are Caesar. Go and show them your power. If the gods sent omens for every man¡¯s death, no one would ever leave their home!" },
                new DialogueManager.Dialogue { speaker = "Brutus", portrait = BrutusSprite, content = "Perhaps fate does not whisper in vain. But then¡­ what is a king if he bends to fear?" },
                new DialogueManager.Dialogue { speaker = "Cassius", portrait = CassiusSprite, content = "Bah! If we all feared every bad dream, Rome would be ruled by old women. The Senate awaits, Caesar. Let them see the lion, not the lamb!" },
                new DialogueManager.Dialogue { speaker = "Player", portrait = playerSprite, content = "A lion does not flinch at shadows¡­ yet even lions fall when they walk into a hunter¡¯s trap." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "Heed the warning, ignore it, or fake your death? Fate is watching. Choose wisely." },
            },
            choices = new List<DialogueManager.Choice>
            {
                new DialogueManager.Choice { text = "Stay Home on March 15", nextScene = "Scene5",
                    followUpDialogues = new List<DialogueManager.Dialogue>
                {
            } },
                new DialogueManager.Choice { text = "Go to the Senate", nextScene = "Scene5",
                    followUpDialogues = new List<DialogueManager.Dialogue>
                {
            }  },
                new DialogueManager.Choice { text = "Fake Your Death to Catch the Conspirators", nextScene = "",
                    followUpDialogues = new List<DialogueManager.Dialogue>
                {
            } }
            }

        };

        DialogueManager.Instance.RegisterScene("Scene4", backgroundScene4, sceneDialogue, scene4Music);
    }
}
