using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System.Linq;

public class LoyaltyManager : MonoBehaviour
{
    public static LoyaltyManager Instance; // Singleton for global access

    public TMP_Text[] textValues;
    private string textValue = "";

    public int IBrutus;
    public int ICassius;
    public int IMarkAntony;
    public int ISenate;


    // Dictionary to track NPC loyalty
    public Dictionary<string, int> npcLoyalty = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ LoyaltyManager initialized and set to DontDestroyOnLoad.");

            if (npcLoyalty == null || npcLoyalty.Count == 0)
            {
                InitializeLoyaltyValues();
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Duplicate LoyaltyManager found. Destroying the new instance.");
            Destroy(gameObject);
        }
    }


    private void InitializeLoyaltyValues()
    {
        if (npcLoyalty == null || npcLoyalty.Count == 0)
        {
            npcLoyalty = new Dictionary<string, int>
        {
            {"Brutus", 2},
            {"Cassius", -5},
            {"Mark Antony", 5},
            {"Senate", 0}
        };
            IBrutus = npcLoyalty["Brutus"];
            ICassius = npcLoyalty["Cassius"];
            IMarkAntony = npcLoyalty["Mark Antony"];
            ISenate = npcLoyalty["Senate"];
            
            Debug.Log("🏛 Loyalty values initialized to default.");
        }
        else
        {
            Debug.Log("🔍 Loyalty values already initialized, keeping current values.");
        }
    }



    // Function to modify loyalty based on player's choices
    public void ChangeLoyalty(string character, int amount)
    {
        if (npcLoyalty == null || npcLoyalty.Count == 0)
        {
            Debug.LogWarning("⚠️ Loyalty data not initialized. Initializing default values.");
            InitializeLoyaltyValues();
        }

        if (npcLoyalty.ContainsKey(character))
        {
            npcLoyalty[character] += amount;
            npcLoyalty[character] = Mathf.Clamp(npcLoyalty[character], -10, 10);
            textValue = npcLoyalty[character].ToString();
            Debug.Log($"🔄 {character} loyalty updated to: {npcLoyalty[character]}");
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
        else
        {
             Debug.LogWarning($"⚠️ Character {character} not found in loyalty dictionary.");
        }
        
    }

    public int GetLoyalty(string character)
    {
        return npcLoyalty.ContainsKey(character) ? npcLoyalty[character] : -11;
    }
    public Dictionary<string, int> GetAllLoyalty()
    {
        if (npcLoyalty == null || npcLoyalty.Count == 0)
        {
            Debug.LogWarning("⚠️ Loyalty data not initialized. Initializing default values.");
            InitializeLoyaltyValues();
        }
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
        Debug.Log("🧐 Checking loyalty values before determining ending...");

        //int brutusLoyalty = GetLoyalty("Brutus");
        //int cassiusLoyalty = GetLoyalty("Cassius");
        //int antonyLoyalty = GetLoyalty("Mark Antony");
        //int senateLoyalty = GetLoyalty("Senate");
        IBrutus = GetLoyalty("Brutus");
        ICassius = GetLoyalty("Cassius");
        IMarkAntony = GetLoyalty("Mark Antony");
        ISenate = GetLoyalty("Senate");

        int brutusLoyalty = IBrutus;
        int cassiusLoyalty = ICassius;
        int antonyLoyalty = IMarkAntony;
        int senateLoyalty = ISenate;

        Debug.Log($"📌 Brutus: {brutusLoyalty}, Cassius: {cassiusLoyalty}, Antony: {antonyLoyalty}, Senate: {senateLoyalty}");
        Debug.Log($"阿巴阿巴, Brutus: {brutusLoyalty}, Cassius: {cassiusLoyalty}, Antony: {antonyLoyalty}, Senate: {senateLoyalty}");

        // 结局1：凯撒被暗杀（Brutus 和 Cassius 的忠诚度低，Senate 反对）
        if (brutusLoyalty <= 0 && cassiusLoyalty <= -3 && senateLoyalty <= -3)
        {
            return "Caesar is assassinated by the Senate.";
        }

        // 结局2：凯撒掌控罗马（Mark Antony 和 Senate 的忠诚度较高）
        if (senateLoyalty >= -3 && antonyLoyalty >= -1)  // Senate 阈值降低，Antony 不必严格大于 0
        {
            return "Caesar consolidates power and controls Rome.";
        }

        // 结局3：罗马陷入内战（Brutus 模棱两可，Cassius 和 Senate 反对）
        if (brutusLoyalty > -2 && brutusLoyalty < 3 && cassiusLoyalty <= 4 && senateLoyalty <= 4)
        {
            return "Rome falls into chaos and civil war erupts.";
        }

        // 结局4：凯撒流亡（和刺杀条件类似，但可能有不同的分支）
        if (brutusLoyalty <= 0 && cassiusLoyalty <= -3 && senateLoyalty <= -3)
        {
            return "Exile Ending – Caesar Flees Before Assassination";
        }

        // 结局5：凯撒以恐怖统治（Cassius 和 Senate 忠诚度极低，Antony 依然支持）
        if (brutusLoyalty <= 0 && cassiusLoyalty <= -5 && senateLoyalty <= -5 && antonyLoyalty >= 3)
        {
            return "The Bloody Tyrant Ending – Rule by Fear";
        }


        // 默认结局
        return "Caesar's fate remains uncertain, with alliances shifting.";
    }
    // ✅ 重置所有角色的忠诚度
    public void ResetAllLoyalty()
    {
        npcLoyalty = npcLoyalty.ToDictionary(pair => pair.Key, pair => 0);
        Debug.Log("🧹 All character loyalties have been reset to 0.");
    }
}
