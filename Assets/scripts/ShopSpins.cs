using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopSpins : MonoBehaviour
{
    private Audiomanager audiomanager;

    public int coinsBalance;

    [System.Serializable]
    class ShopItem
    {
        public string itemName;
        public int price;
        public GameObject BuyButton;
        public int countSpin;
    }
    public GameObject NotEnoughMoney;
    [SerializeField] private List<ShopItem> shopItemList;
    [SerializeField] private TextMeshProUGUI coinsText;

    private int currentEquippedIndex = -1;

   
    private void Start()
    {
        audiomanager = FindAnyObjectByType<Audiomanager>();


    }
    private void Update()
    {
        if (PlayerPrefs.HasKey("CoinsBalance"))
        {
            LoadCoinsBalance();
        }
    }

    public void BuyItem(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= shopItemList.Count)
        {
            return;
        }

        ShopItem item = shopItemList[itemIndex];

        if (coinsBalance <= item.price)
        {
            if (audiomanager != null)
            {
               audiomanager.PlaySFX(audiomanager.NotEnoughCoins);

            }
            NotEnoughMoney.SetActive(true);
        }

        if (coinsBalance >= item.price)
        {
            coinsBalance -= item.price;

            PlayerPrefs.SetInt("CoinsBalance", coinsBalance);
            PlayerPrefs.Save();

            if (item.BuyButton != null)
            {

                UpdateCoinsUI();

              
                int currentSpinCount = PlayerPrefs.GetInt(itemIndex + "_Spin") + item.countSpin;


                
                PlayerPrefs.SetInt(itemIndex + "_Spin", currentSpinCount);
                audiomanager.PlaySFX(audiomanager.buyitem);




            }
          

        }
    }



    public void UpdateCoinsUI()
    {
        if (coinsText != null)
        {
            coinsText.text = coinsBalance.ToString();
        }
    }

    private void LoadCoinsBalance()
    {
        coinsBalance = PlayerPrefs.GetInt("CoinsBalance");
    }
}
