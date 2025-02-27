using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scene2Dialogue : MonoBehaviour
{
    public Sprite playerSprite;
    public Sprite BrutusSprite;
    public Sprite CassiusSprite;
    public Sprite MarkAntonySprite;
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
                new DialogueManager.Dialogue { speaker = "Narrator", content = "The festival of Lupercalia fills the streets with celebration. Mark Antony, your most loyal general, steps forward in the public square. He holds out a golden crown offering you the symbol of kingship before the people." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "A hush falls over the crowd. The people wait. The Senate watches in fear." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "Brutus remains still, unreadable. Cassius’ eyes narrow as he sees an opportunity. If you take the crown, We can't guarantee what he will do next..." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "But the people love you. Accepting might push Rome closer to the Empire but also make enemies." },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "A king of Rome? The moment hangs in the air. Will you take what is offered, or deny it?" },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "Antony holds out a golden crown before the crowd." },
                new DialogueManager.Dialogue { speaker = "Mark Antony", portrait = MarkAntonySprite, content = "Take it, Caesar! The people love you!" },
                new DialogueManager.Dialogue { speaker = "Brutus", portrait = BrutusSprite, content = "Rome has no king!" },
                new DialogueManager.Dialogue { speaker = "Cassius", portrait = CassiusSprite, content = "Oh Brutus, he wants it. Look at him." },
                new DialogueManager.Dialogue { speaker = "Player", portrait = playerSprite, content = "(Keep silence)" },
                new DialogueManager.Dialogue { speaker = "Narrator", content = "Seize the crown, uphold the Republic, or mock the Senate—what will you choose?" }
            },
            choices = new List<DialogueManager.Choice>
            {
                new DialogueManager.Choice { text = "Accept the Crown", nextScene = "Scene3",
                    followUpDialogues = new List<DialogueManager.Dialogue>
                {
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "Fear spreads among the senators, and Brutus's trust begins to crumble." },
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "Conspirators speed up the assassination plot." }

            }  },
                new DialogueManager.Choice { text = "Refuse the Crown", nextScene = "Scene3",
                    followUpDialogues = new List<DialogueManager.Dialogue>
                {
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "Brutus's resolve wavers on whether to join the conspiracy." },
                    new DialogueManager.Dialogue { speaker = "Narrator", content = " Cassius has to work harder to turn him against you." }

            } },
                new DialogueManager.Choice { text = "Publicly Condemn the Senate for Not Offering it", nextScene = "Scene3",
                    followUpDialogues = new List<DialogueManager.Dialogue>
                {
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "You are seen as power-hungry, your ambitions laid bare for all to judge." },
                    new DialogueManager.Dialogue { speaker = "Narrator", content = "The Senate grows fearful of a monarchy, and the shadow of assassination looms ever larger." }

            } },
            }

        };

        DialogueManager.Instance.RegisterScene("Scene2", backgroundScene2, sceneDialogue, scene2Music);
        Debug.Log("Scene2 successfully registered.");
    }
}
