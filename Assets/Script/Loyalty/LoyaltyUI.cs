using UnityEngine;
using TMPro;

public class LoyaltyUI : MonoBehaviour
{
    public TextMeshProUGUI brutusText, cassiusText, antonyText, ciceroText;

    private void Update()
    {
        brutusText.text = "Brutus: " + LoyaltyManager.Instance.GetLoyalty("Brutus");
        cassiusText.text = "Cassius: " + LoyaltyManager.Instance.GetLoyalty("Cassius");
        antonyText.text = "Mark Antony: " + LoyaltyManager.Instance.GetLoyalty("Mark Antony");
        ciceroText.text = "Cicero: " + LoyaltyManager.Instance.GetLoyalty("Cicero");
    }
}
