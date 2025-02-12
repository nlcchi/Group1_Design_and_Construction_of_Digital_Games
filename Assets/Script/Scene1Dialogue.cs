using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scene1Dialogue : MonoBehaviour
{
    public Sprite playerSprite;
    public Sprite BrutusSprite;
    public Sprite CassiusSprite;
    public Sprite backgroundScene1;

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
                new DialogueManager.Dialogue { speaker = "Brutus", portrait = BrutusSprite, content = "Caesar, the people of Rome admire you, but some fear your power." },
                new DialogueManager.Dialogue { speaker = "Player", portrait = playerSprite, content = "Rome needs strong leadership." },
                new DialogueManager.Dialogue { speaker = "Cassius", portrait = CassiusSprite, content = "Not everyone agrees. Beware of those who smile in your presence but plot behind your back." }
            },
                choices = new List<DialogueManager.Choice>
            {
                new DialogueManager.Choice { text = "Go to the Senate", nextScene = "Scene2" }
            },

        };

        DialogueManager.Instance.RegisterScene("Scene1", backgroundScene1, sceneDialogue);
        DialogueManager.Instance.LoadVirtualScene("Scene1"); // Enter the default scene
    }
}

