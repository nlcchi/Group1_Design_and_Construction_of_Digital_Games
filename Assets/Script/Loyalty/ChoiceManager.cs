using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    public void ChooseOption(int option)
    {
        switch (option)
        {
            case 1:
                LoyaltyManager.Instance.ChangeLoyalty("Brutus", 2);
                LoyaltyManager.Instance.ChangeLoyalty("Cassius", -1);
                break;

            case 2:
                LoyaltyManager.Instance.ChangeLoyalty("Brutus", -4);
                LoyaltyManager.Instance.ChangeLoyalty("Cassius", -2);
                break;

            case 3:
                LoyaltyManager.Instance.ChangeLoyalty("Mark Antony", 3);
                LoyaltyManager.Instance.ChangeLoyalty("Cicero", -3);
                break;
        }
    }
}
