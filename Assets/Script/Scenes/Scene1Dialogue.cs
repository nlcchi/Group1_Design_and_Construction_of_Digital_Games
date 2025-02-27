using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Unity.Burst.Intrinsics.X86;
using System.Security.Cryptography;
using System;

public class Scene1Dialogue : MonoBehaviour
{
    public Sprite playerSprite;
    public Sprite BrutusSprite;
    public Sprite CassiusSprite;
    public Sprite MarkAntonySprite;
    public Sprite SenateSprite;
    public Sprite backgroundScene1;
    public AudioClip scene1Music;

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

        RegisterScene1();
    }

    void RegisterScene1()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager Instance is NULL in Scene1Dialogue!");
            return;
        }


        DialogueManager.DialogueData sceneDialogue = new DialogueManager.DialogueData
        {
            dialogues = new List<DialogueManager.Dialogue>
            {
                new DialogueManager.Dialogue { speaker = "Narrator", content = "Rome stands at a crossroads. The Senate, once the heart of the Republic, has just granted Julius Caesar the title of Dictator for Life. Some senators murmur uneasily, while others begrudgingly accept his rule." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "In the streets, the people rejoice. They see Caesar as a hero who ended the civil war and brought stability to Rome." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "But behind closed doors, whispers of conspiracy grow louder. Some fear that Caesar¡¯s ambition threatens the Republic itself. Brutus watches in silence, torn between admiration and duty. Cassius smiles knowingly, already planning his next move." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "The Republic is slipping away. Will you reassure the Senate, or cement your power?" },
                new DialogueManager.Dialogue { speaker = "Brutus", portrait = BrutusSprite, content = "Caesar, Rome is victorious. But tell me¡­ what¡¯s next?" },
                new DialogueManager.Dialogue { speaker = "Cassius", portrait = CassiusSprite, content = "Hail, Caesar! Enjoy the cheers while they last. Power is a fickle thing in Rome." },
                new DialogueManager.Dialogue { speaker = "Player", portrait = playerSprite, content = "Long time no see, guys. Now is the time to enjoy the power of the lifelong consul! but I am still worried about the old guys in the Senate..." },
                new DialogueManager.Dialogue { speaker = "Mark Antony", portrait = MarkAntonySprite, content = "Forget them! Rome stands strong because of you. Rule as you see fit!" },
                new DialogueManager.Dialogue { speaker = "Senators ", portrait = SenateSprite, content = "A dictator today¡­ what about tomorrow?" },
                new DialogueManager.Dialogue { speaker = "Brutus", portrait = BrutusSprite, content = "A great honor, Caesar. But the Senate watches closely. What will you do?" },
                new DialogueManager.Dialogue { speaker = "Cassius ", portrait = CassiusSprite , content = "See? He¡¯s already a king. Soon, he won¡¯t need us." },
                new DialogueManager.Dialogue { speaker = "Mark Antony", portrait = MarkAntonySprite, content = "The Republic is weak. You are Rome¡¯s strength!" },
            },
            choices = new List<DialogueManager.Choice>
            {
                new DialogueManager.Choice { text = "Humble Reject", nextScene = "Scene2",            
                    followUpDialogues = new List<DialogueManager.Dialogue>
                {
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "Gradually earns Brutus' trust, slowing down the conspiracy." },
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "Cassius still plots but struggles to convince Brutus." }

            } },
                new DialogueManager.Choice { text = "Humble Acceptance", nextScene = "Scene2",
                    followUpDialogues = new List<DialogueManager.Dialogue>
                {
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "Slowing down the conspiracy¡ªbut doubts begin to creep into Brutus' mind." },
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "Cassius begins to believe that Caesar harbors great ambition, gradually growing suspicious of him." }

            } },
                new DialogueManager.Choice { text = "Arrogant Declaration ('I am Rome!')", nextScene = "Scene2",
                    followUpDialogues = new List<DialogueManager.Dialogue>
                {
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "Brutus begins doubting Caesar." },
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "After that, cassius easily convinces Brutus to join the conspiracy." }

            } },
            }

        };

        DialogueManager.Instance.RegisterScene("Scene1", backgroundScene1, sceneDialogue, scene1Music);
        DialogueManager.Instance.LoadVirtualScene("Scene1"); // Enter the default scene
    }
}

