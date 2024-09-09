using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorUnlockScript : MonoBehaviour
{
    private Audiomanager audiomanager;
    private List<int> equippedIndices = new List<int>();

    public GameObject[] equipButtons;
    public GameObject[] unequipButtons;
    public GameObject[] panelforUnlock;
    public GameObject[] panelforUnlockUpgrade;

    public TextMeshProUGUI[] textShowProduction;
    private float updateInterval = 1.0f;
    private float timeSinceLastUpdate = 0.0f;
    private int productionCompleteCount;

    private HashSet<string> processedKeys = new HashSet<string>();

    public bool panelUnlockSucces = false;
    private void Start()
    {
        audiomanager = FindObjectOfType<Audiomanager>();
        LoadButtonStates();
        EnsureInitialColorUnlocks();
    }

    private void Update()
    {

        StartEnsuer();
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            timeSinceLastUpdate = 0.0f;

            UpdateProductionText();
            CheckAndUpdatePlayerPrefs();
            DeactivatePanelsBasedOnColorUnlock();


            bool unlockPanel = false;
            if (panelforUnlock[4].activeSelf)
            {
                unlockPanel = true;
              
            }
            else
            {
                unlockPanel = false;
            }

            for (int i = 0; i <= 3; i++)
            {
                panelforUnlock[i].SetActive(unlockPanel);

            }


        }
    }

    private void StartEnsuer()
    {
        
        for (int i = 5; i <= 9; i++)
        {
            if (i < panelforUnlock.Length && panelforUnlock[i].activeSelf)
            {
                string colorUnlockKey = "ColorUnlock_" + (i - 5); 
                if (PlayerPrefs.HasKey(colorUnlockKey) && !processedKeys.Contains(colorUnlockKey))
                {
                    processedKeys.Add(colorUnlockKey);
                    EnsureInitialColorUnlocks();
                }
            }
        }
    }

    private void UpdateProductionText()
    {
        int countProduct = PlayerPrefs.GetInt("CountsProduct", 0);
        productionCompleteCount = countProduct;
        int[] thresholds = { 50, 150, 350, 700, 1500 };

        for (int i = 0; i < textShowProduction.Length; i++)
        {
            if (i >= thresholds.Length)
                break;

            int threshold = thresholds[i];
            string displayText = $"Produce {productionCompleteCount}/{threshold} Clothes";
            textShowProduction[i].text = displayText;
        }
    }

    private void CheckAndUpdatePlayerPrefs()
    {
        int[] thresholds = { 50, 150, 350, 700, 1500 };
        string[] unlockKeys = { "ColorUnlock_0", "ColorUnlock_1", "ColorUnlock_2", "ColorUnlock_3", "ColorUnlock_4" };

        for (int i = 0; i < thresholds.Length; i++)
        {
            string productionKey = "ProductionComplete_" + thresholds[i];
            string unlockKey = unlockKeys[i];

            if (PlayerPrefs.HasKey(productionKey))
            {
                PlayerPrefs.SetString(unlockKey, unlockKey);
            }
        }
    }

    private void EnsureInitialColorUnlocks()
    {
        panelUnlockSucces = true;
        for (int i = 0; i < 5; i++)
        {
            bool allKeysPresent = true;
            for (int j = 0; j <= 5; j++)
            {
                if (!PlayerPrefs.HasKey("ColorUnlock_" + j))
                {
                    allKeysPresent = false;
                    break;
                }
            }

            if (allKeysPresent)
            {
                panelforUnlockUpgrade[i].SetActive(false);
            }
        }

        string[] colorUnlockKeys =
        {
            "ColorUnlock_0", "ColorUnlock_1", "ColorUnlock_2", "ColorUnlock_3", "ColorUnlock_4",
        };

        bool missingUnlocks = true;

        foreach (string key in colorUnlockKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                missingUnlocks = false;
                break;
            }
        }

        if (missingUnlocks)
        {
          
            for (int i = 0; i <= 3; i++)
            {
                string key1 = "ColorActivate_" + i;
                if (!PlayerPrefs.HasKey(key1))
                {
                    PlayerPrefs.SetString(key1, "active");
                }

                string key2 = "ButtonState_" + i;
                if (!PlayerPrefs.HasKey(key2))
                {
                    PlayerPrefs.SetString(key2, "1");
                }
                PlayerPrefs.Save();
            }
            equippedIndices.Clear();
            for (int i = 0; i < 4; i++)
            {
                equippedIndices.Add(i);
            }
        }
    }

    private void DeactivatePanelsBasedOnColorUnlock()
    {
        int startIndex = 4;

        for (int i = 0; i < panelforUnlock.Length - startIndex; i++)
        {
            string key = "ColorUnlock_" + i;
            if (PlayerPrefs.HasKey(key))
            {
                if (startIndex + i < panelforUnlock.Length)
                {
                    panelforUnlock[startIndex + i].SetActive(false);
                }
            }
            else
            {
                if (startIndex + i < panelforUnlock.Length)
                {
                    panelforUnlock[startIndex + i].SetActive(true);
                }
            }
        }
    }

    public void EquipButton(int index)
    {
        if (equippedIndices.Count >= 4)
        {
            int firstEquippedIndex = equippedIndices[0];
            UnequipButton(firstEquippedIndex);
        }
        if (audiomanager != null)
        {
            audiomanager.PlaySFX(audiomanager.equipitem);
        }

        unequipButtons[index].SetActive(true);
        equipButtons[index].SetActive(false);
        PlayerPrefs.SetString("ColorActivate_" + index, "active");
        SaveButtonState(index, true);

        equippedIndices.Add(index);
    }

    public void UnequipButton(int index)
    {
        if (audiomanager != null)
        {
            audiomanager.PlaySFX(audiomanager.unequipitem);
        }

        unequipButtons[index].SetActive(false);
        equipButtons[index].SetActive(true);
        PlayerPrefs.DeleteKey("ColorActivate_" + index);

        SaveButtonState(index, false);

        equippedIndices.Remove(index);
    }

    private void SaveButtonState(int index, bool isEquipped)
    {
        PlayerPrefs.SetString("ButtonState_" + index, isEquipped ? "1" : "0");
        PlayerPrefs.Save();
    }

    private void LoadButtonStates()
    {
        for (int i = 0; i < equipButtons.Length; i++)
        {
            if (PlayerPrefs.HasKey("ButtonState_" + i))
            {
                string state = PlayerPrefs.GetString("ButtonState_" + i);
                bool isEquipped = state == "1";

                equipButtons[i].SetActive(!isEquipped);
                unequipButtons[i].SetActive(isEquipped);

                if (isEquipped)
                {
                    equippedIndices.Add(i);
                }
            }
        }
    }
}
