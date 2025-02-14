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
        {"Cicero", -3}    
    };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
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
        
            switch(character){                
                case "Brutus":
                    SetTextValue(0);
                    break;
                case "Cassius":
                    SetTextValue(1);
                    break;
                case "Mark Antony":
                    SetTextValue(2);
                    break;
                case "Cicero":
                    SetTextValue(3);
                    break;
            }
        }
    }

    public int GetLoyalty(string character)
    {
        return npcLoyalty.ContainsKey(character) ? npcLoyalty[character] : 0;
    }

    private void SetTextValue(int slot){
        if(textValues.Length < slot) {
            Debug.Log("incorrect slot number does not exist");
            return;
        }
        textValues[slot].text = textValue;
    }
}
