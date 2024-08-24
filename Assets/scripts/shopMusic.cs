using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.Build.Content;

public class shopMusic : MonoBehaviour
{
    public int coinsBalance;
    private Audiomanager audiomanager;

    [System.Serializable]
    class ShopItem
    {
        public string itemName;
        public int price;
        public bool isPurchased = false;
        public GameObject BuyButton;
        public GameObject purchasedPanel;
        public Slider musicSlider;
        public AudioClip musicClip;
        public TextMeshProUGUI pricetext;
    }

    private stateitem stateitem;
    public GameObject[] buttonStop;
    public GameObject[] buttonPlay;
    public GameObject NotEnoughMoney;
    [SerializeField] private List<ShopItem> shopItemList;
    [SerializeField] private TextMeshProUGUI coinsText;

    public GameObject[] equipButtons;
    public GameObject[] unequipButtons;
    private int currentEquippedIndex = -1;

    public AudioSource musicSource;
    public AudioSource musicSourceDefault;
    private int currentMusicIndex = -1;

    private void Start()
    {
        audiomanager = FindAnyObjectByType<Audiomanager>();
        stateitem = FindAnyObjectByType<stateitem>();
       
        foreach (var item in shopItemList)
        {
            if (item.pricetext != null)
            {
                item.pricetext.text = item.price.ToString();
            }
            if (item.musicClip != null)
            {
                item.musicClip.LoadAudioData();
            }
        }
        
        LoadPurchasedItems();
        SetupButtons();
        LoadButtonStates();

    }

    private void Update()
    {
        if (currentMusicIndex >= 0 && currentMusicIndex < shopItemList.Count)
        {
            ShopItem currentItem = shopItemList[currentMusicIndex];
            if (currentItem.musicSlider != null && musicSource.clip != null)
            {
                currentItem.musicSlider.value = musicSource.time / musicSource.clip.length;
            }
        }
        if (PlayerPrefs.HasKey("CoinsBalance"))
        {
            LoadCoinsBalance();
        }
    }

    public void buttonBackStopMusic()
    {
        foreach (GameObject buttonStop in buttonStop)
        {
            buttonStop.SetActive(false);

        }
        foreach (GameObject buttonPlay in buttonPlay)
        {
            buttonPlay.SetActive(true);
        }
       
        stateitem.LoadMusicState();


    }
    public void TogglePlayStopButtons(int index)
    {
       foreach (GameObject buttonStop in buttonStop)
       {
            if (buttonStop.activeSelf)
            {
                buttonStop.SetActive(false);

            }
       }
        foreach (GameObject buttonPlay in buttonPlay)
        {
           
                buttonPlay.SetActive(true);

            
        }

        buttonPlay[index].SetActive(false);
        buttonStop[index].SetActive(true);
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

    public void ChangeAudioTime(int index)
    {
        if (index >= 0 && index < shopItemList.Count)
        {
            ShopItem currentItem = shopItemList[index];
            if (currentItem.musicSlider != null)
            {
                musicSource.time = musicSource.clip.length * currentItem.musicSlider.value;
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

                if (item.musicSlider != null)
                {
                    item.musicSlider.onValueChanged.AddListener(delegate { ChangeAudioTime(itemIndex); });
                }
            }
        }
    }

    public void equipButton(int index)
    {
        audiomanager.PlaySFX(audiomanager.equipitem);

        // Hide the previous unequip button if it exists
        if (currentEquippedIndex != -1 && currentEquippedIndex != index)
        {
            unequipButtons[currentEquippedIndex].SetActive(false);
            equipButtons[currentEquippedIndex].SetActive(true);
            PlayerPrefs.SetInt("MusicEquipButtonState_" + currentEquippedIndex, 1);
            PlayerPrefs.SetInt("MusicUnequipButtonState_" + currentEquippedIndex, 0);
        }
        for (int i = 0; i < equipButtons.Length; i++)
        {
            if (i != index && PlayerPrefs.HasKey("music_" + i))
            {
                PlayerPrefs.DeleteKey("music_" + i);
            }
        }
        PlayerPrefs.SetInt("music_" + index, index);

        // Equip the new item
        currentEquippedIndex = index;
        unequipButtons[index].SetActive(true);
        equipButtons[index].SetActive(false);
        stateitem.LoadMusicState();
        PlayerPrefs.SetInt("MusicEquipButtonState_" + index, 0);
        PlayerPrefs.SetInt("MusicUnequipButtonState_" + index, 1);
        PlayerPrefs.Save();
    }

    public void unequipButton(int index)
    {

        ShopItem item = shopItemList[index];

        if (item.isPurchased)
        {
            audiomanager.PlaySFX(audiomanager.unequipitem);

            for (int i = 0; i < equipButtons.Length; i++)
            {
                if (PlayerPrefs.HasKey("music_" + i))
                {
                    PlayerPrefs.DeleteKey("music_" + i);
                }

            }

            unequipButtons[index].SetActive(false);
            equipButtons[index].SetActive(true);

            if (currentEquippedIndex == index)
            {
                currentEquippedIndex = -1;
            }
            stateitem.LoadMusicState();

            PlayerPrefs.SetInt("MusicEquipButtonState_" + index, 1);
            PlayerPrefs.SetInt("MusicUnequipButtonState_" + index, 0);
            PlayerPrefs.Save();
        }

    }

   

    private void LoadButtonStates()
    {
        for (int i = 0; i < equipButtons.Length; i++)
        {
            int equipState = PlayerPrefs.GetInt("MusicEquipButtonState_" + i, equipButtons[i].activeSelf ? 1 : 0);
            int unequipState = PlayerPrefs.GetInt("MusicUnequipButtonState_" + i, unequipButtons[i].activeSelf ? 1 : 0);

            equipButtons[i].SetActive(equipState == 1);
            unequipButtons[i].SetActive(unequipState == 1);

            if (unequipState == 1)
            {
                currentEquippedIndex = i;
            }
        }
    }

    public void PlayMusic(int index)
    {
        if (index >= 0 && index < shopItemList.Count)
        {
            ShopItem item = shopItemList[index];
            if (item.musicClip != null)
            {
                musicSource.clip = item.musicClip;
                musicSource.Play();
                musicSourceDefault.Stop();

                
                currentMusicIndex = index;
            }
        }
    }


    public void StopMusic()
    {
        musicSource.Stop();
        musicSourceDefault.Play();
        musicSource.clip = null;
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
            PlayerPrefs.SetInt("MusicIsPurchased_" + i, shopItemList[i].isPurchased ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    private void LoadPurchasedItems()
    {
        for (int i = 0; i < shopItemList.Count; i++)
        {
            int isPurchased = PlayerPrefs.GetInt("MusicIsPurchased_" + i, 0);
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
}
