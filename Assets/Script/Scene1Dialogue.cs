using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scene1Dialogue : MonoBehaviour
{
    public Sprite playerSprite;
    public Sprite BrutusSprite;
    public Sprite CassiusSprite;
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
                new DialogueManager.Dialogue { speaker = "Brutus", portrait = BrutusSprite, content = "Caesar, the people of Rome admire you, but some fear your power." },
                new DialogueManager.Dialogue { speaker = "Player", portrait = playerSprite, content = "Rome needs strong leadership." },
                new DialogueManager.Dialogue { speaker = "Cassius", portrait = CassiusSprite, content = "Not everyone agrees. Beware of those who smile in your presence but plot behind your back." }
            },
            choices = new List<DialogueManager.Choice>
            {
                new DialogueManager.Choice { text = "Go to the Senate", nextScene = "Scene2" },
            },

        };

        DialogueManager.Instance.RegisterScene("Scene1", backgroundScene1, sceneDialogue, scene1Music);
        DialogueManager.Instance.LoadVirtualScene("Scene1"); // Enter the default scene
    }
}

