using TMPro;
using UnityEngine;

public class scalecountmoney : MonoBehaviour
{
    public TextMeshProUGUI textmoney;
    private int coins;
    private void LateUpdate()
    {
        coins = PlayerPrefs.GetInt("CoinsBalance", 0);
        if (coins < 10000 && textmoney.fontSize == 73)
        {
            return;
        }
        else if (coins >= 10000 && textmoney.fontSize == 57)
        {
            return;
        }
        else if (coins >= 100000 && textmoney.fontSize == 53)
        {
            return;
        }

        if (PlayerPrefs.HasKey("CoinsBalance"))
        {
            if (coins < 10000)
            {
                textmoney.fontSize = 73;
            }
            if (coins >= 10000)
            {
                textmoney.fontSize = 57;
            }
            if (coins >= 100000)
            {
                textmoney.fontSize = 53;
            }
        }

    }
}
