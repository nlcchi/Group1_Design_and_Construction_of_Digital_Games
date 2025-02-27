using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class LoyaltyManager : MonoBehaviour
{
    public static LoyaltyManager Instance; // Singleton for global access

    public TMP_Text[] textValues;
    private string textValue = "";


    // Dictionary to track NPC loyalty
    public Dictionary<string, int> npcLoyalty = new Dictionary<string, int>
    {
        {"Brutus", 2},
        {"Cassius", -5},
        {"Mark Antony", 5},
        {"Cicero", -3},
        {"Senate", 0}
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
            Destroy(gameObject);
    }

    // Function to modify loyalty based on player's choices
    public void ChangeLoyalty(string character, int amount)
    {
        if (npcLoyalty.ContainsKey(character))
        {
            npcLoyalty[character] += amount;
            npcLoyalty[character] = Mathf.Clamp(npcLoyalty[character], -10, 10);
            textValue = npcLoyalty[character].ToString();
            Debug.Log(character + " loyalty is now: " + textValue);

            switch (character)
            {
                case "Brutus":
                    SetTextValue(0);
                    break;
                case "Cassius":
                    SetTextValue(1);
                    break;
                case "Mark Antony":
                    SetTextValue(2);
                    break;
                case "Senate":
                    SetTextValue(3);
                    break;
            }
        }
    }

    public int GetLoyalty(string character)
    {
        return npcLoyalty.ContainsKey(character) ? npcLoyalty[character] : 0;
    }
    public Dictionary<string, int> GetAllLoyalty()
    {
        return new Dictionary<string, int>(npcLoyalty); // 返回副本，防止外部修改数据
    }
    private void SetTextValue(int slot)
    {
        if (textValues.Length < slot)
        {
            Debug.Log("incorrect slot number does not exist");
            return;
        }
        //textValues[slot].text = textValue;
    }
    public string DetermineGameEnding()
    {
        int brutusLoyalty = GetLoyalty("Brutus");
        int cassiusLoyalty = GetLoyalty("Cassius");
        int antonyLoyalty = GetLoyalty("Mark Antony");
        int senateLoyalty = GetLoyalty("Senate");

        // 结局1：如果 Brutus 和 Cassius 的忠诚度高，凯撒被暗杀
        if (brutusLoyalty > 5 && cassiusLoyalty > 3)
        {
            return "Caesar is assassinated by the Senate.";
        }

        // 结局2：如果 Mark Antony 和 Cicero 忠诚度高，凯撒掌控罗马
        if (antonyLoyalty > 5 && senateLoyalty > 3)
        {
            return "Caesar consolidates power and controls Rome.";
        }

        // 结局3：如果所有人忠诚度都很低，罗马陷入内战
        if (brutusLoyalty < -5 && cassiusLoyalty < -5 && antonyLoyalty < -5 && senateLoyalty < -5)
        {
            return "Rome falls into chaos and civil war erupts.";
        }

        // 默认结局
        return "Caesar's fate remains uncertain, with alliances shifting.";
    }
}
