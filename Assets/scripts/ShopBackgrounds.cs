using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class shop : MonoBehaviour
{

    public int coinsBalance;

    [System.Serializable]
    class ShopItem
    {
        public string itemName;
        public int price;
        public bool isPurchased = false;
        public GameObject BuyButton;
        public GameObject purchasedPanel;
        public TextMeshProUGUI pricetext;
    }
    public GameObject NotEnoughMoney;
    [SerializeField] private List<ShopItem> shopItemList;
    [SerializeField] private TextMeshProUGUI coinsText;
    private stateitem Stateitem;
    public GameObject[] equipButtons;
    public GameObject[] unequipButtons;
    private int currentEquippedIndex = -1;
    private Audiomanager audiomanager;
    private void Start()
    {
        Stateitem = FindFirstObjectByType<stateitem>();
        audiomanager = FindAnyObjectByType<Audiomanager>();
        
        LoadPurchasedItems();
       
        UpdateCoinsUI();
        SetupButtons();
        LoadButtonStates();
        foreach (var item in shopItemList)
        {
            if (item.pricetext != null)
            {
                item.pricetext.text = item.price.ToString();
            }
        }
    }
    private void Update()
    {
        if (PlayerPrefs.HasKey("CoinsBalance"))
        {
            LoadCoinsBalance();
            UpdateCoinsUI();
        }
    }
    private void SetupButtons()
    {
        foreach (var item in shopItemList)
        {
            if (item.BuyButton != null && item.isPurchased)
            {
                item.BuyButton.SetActive(false);
                item.purchasedPanel.SetActive(true);
            }
        }
    }
    public void BuyItem(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= shopItemList.Count)
        {
            return;
        }

        ShopItem item = shopItemList[itemIndex];
        if (item.isPurchased)
        {
            return;
        }

        if (coinsBalance <= item.price)
        {
            audiomanager.PlaySFX(audiomanager.NotEnoughCoins);
            NotEnoughMoney.SetActive(true);

        }

        if (coinsBalance >= item.price)
        {
            coinsBalance -= item.price;
            item.isPurchased = true;

            if (item.BuyButton != null)
            {
                SavePurchasedItems();
                SaveCoinsBalance();
                UpdateCoinsUI();
                item.BuyButton.SetActive(false);
                item.purchasedPanel.SetActive(true);
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

    private void SavePurchasedItems()
    {
        for (int i = 0; i < shopItemList.Count; i++)
        {
            PlayerPrefs.SetInt("BackIsPurchased_" + i, shopItemList[i].isPurchased ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    private void LoadPurchasedItems()
    {
        for (int i = 0; i < shopItemList.Count; i++)
        {
            int isPurchased = PlayerPrefs.GetInt("BackIsPurchased_" + i, 0);
            shopItemList[i].isPurchased = isPurchased == 1;
        }
    }

    public void SaveCoinsBalance()
    {
        PlayerPrefs.SetInt("CoinsBalance", coinsBalance);
        PlayerPrefs.Save();
    }

    private void LoadCoinsBalance()
    {
        coinsBalance = PlayerPrefs.GetInt("CoinsBalance");
    }

    public void equipButton(int index)
    {
        audiomanager.PlaySFX(audiomanager.equipitem);


        if (currentEquippedIndex != -1 && currentEquippedIndex != index)
        {
            unequipButtons[currentEquippedIndex].SetActive(false);
            equipButtons[currentEquippedIndex].SetActive(true);
            PlayerPrefs.SetInt("BackEquipButtonState_" + currentEquippedIndex, 1);
            PlayerPrefs.SetInt("BackUnequipButtonState_" + currentEquippedIndex, 0);
        }
        for (int i = 0; i < equipButtons.Length; i++)
        {
            if (i != index && PlayerPrefs.HasKey("background_" + i))
            {
                PlayerPrefs.DeleteKey("background_" + i);
            }
        }
        PlayerPrefs.SetInt("background_" + index, index);

        
        currentEquippedIndex = index;
        unequipButtons[index].SetActive(true);
        equipButtons[index].SetActive(false);
        PlayerPrefs.SetInt("BackEquipButtonState_" + index, 0);
        PlayerPrefs.SetInt("BackUnequipButtonState_" + index, 1);
        PlayerPrefs.Save();
        Stateitem.LoadBackgroundsState();

    }

    public void unequipButton(int index)
    {

        ShopItem item = shopItemList[index];

        if (item.isPurchased)
        {
            audiomanager.PlaySFX(audiomanager.unequipitem);

            for (int i = 0; i < equipButtons.Length; i++)
            {
                if (PlayerPrefs.HasKey("background_" + i))
                {
                    PlayerPrefs.DeleteKey("background_" + i);
                }
            }

            unequipButtons[index].SetActive(false);
            equipButtons[index].SetActive(true);

            if (currentEquippedIndex == index)
            {
                currentEquippedIndex = -1;
            }

            PlayerPrefs.SetInt("BackEquipButtonState_" + index, 1);
            PlayerPrefs.SetInt("BackUnequipButtonState_" + index, 0);
            PlayerPrefs.Save();
            Stateitem.LoadBackgroundsState();

        }

    }

   

    private void LoadButtonStates()
    {
        for (int i = 0; i < equipButtons.Length; i++)
        {
            int equipState = PlayerPrefs.GetInt("BackEquipButtonState_" + i, equipButtons[i].activeSelf ? 1 : 0);
            int unequipState = PlayerPrefs.GetInt("BackUnequipButtonState_" + i, unequipButtons[i].activeSelf ? 1 : 0);

            equipButtons[i].SetActive(equipState == 1);
            unequipButtons[i].SetActive(unequipState == 1);

            if (unequipState == 1)
            {
                currentEquippedIndex = i;
            }
        }
    }

}
