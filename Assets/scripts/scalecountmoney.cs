using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scalecountmoney : MonoBehaviour
{
    public TextMeshProUGUI textmoney;
    private void Update()
    {
        if (PlayerPrefs.HasKey("CoinsBalance"))
        {
            int coins = PlayerPrefs.GetInt("CoinsBalance");
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
