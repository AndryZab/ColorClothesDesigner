using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UpgradeProductionAndWarehouse;

public class UpgradeProductionAndWarehouse : MonoBehaviour
{
    [Serializable]
    public class ProductUpgrade
    {
        public AutoProduction autoProductionScript;
        public string name;
        public TextMeshProUGUI coinsbar1Text;
        public int upgradeCostbar1;
        public TextMeshProUGUI TextNeedColorUnlockCounts;
        public int countColorUnlocks;
        public int NeedColorsForUnlockProduct;
        public GameObject PanelMaxUpgrade;
        public GameObject PanelLock;
        public float multiplier = 1.0f;
        public string IndexForUnlockColors;
        public float newFloatInverval;

        [Header("InitializeSettings")]
        public float saveIntervalMAX = 0;
        public float fillDurationMaxed;

    }

    [Serializable]
    public class warehouseUpgrade
    {
        public string name;
        public TextMeshProUGUI coinsbarTextSlots;
        public int upgradeCostbarSlots;
        public TextMeshProUGUI countSlots;
        public GameObject panelMaxUpgrades;
        public int maxSlots;
    }

    private ColorUnlockScript colorUnlockScript;
    public TextMeshProUGUI coinsBalanceText;
    public List<ProductUpgrade> Product = new List<ProductUpgrade>();
    public List<warehouseUpgrade> warehouses = new List<warehouseUpgrade>();
    private int coinsBalance;
    private Audiomanager audiomanager;
    private HashSet<string> processedKeys = new HashSet<string>();

    private initializeSlotsInventory inventoryslots;

    private void Start()
    {
        inventoryslots = FindObjectOfType<initializeSlotsInventory>();
        colorUnlockScript = FindObjectOfType<ColorUnlockScript>();
        audiomanager = FindAnyObjectByType<Audiomanager>();
        if (PlayerPrefs.HasKey("CoinsBalance"))
        {
            coinsBalance = PlayerPrefs.GetInt("CoinsBalance");
        }

        if (coinsBalanceText != null)
        {
            UpdateCoinsBalanceDisplay();
        }

        foreach (ProductUpgrade autoProduct in Product)
        {
            if (autoProduct.autoProductionScript != null)
            {
                autoProduct.autoProductionScript.saveInterval = PlayerPrefs.GetFloat(autoProduct.name + "_decraseTime_", autoProduct.autoProductionScript.saveInterval);
                autoProduct.autoProductionScript.fillDuration = PlayerPrefs.GetFloat(autoProduct.name + "_decraseTimeFill_", autoProduct.autoProductionScript.fillDuration);
            }

            if (autoProduct.PanelMaxUpgrade != null && autoProduct.autoProductionScript != null && autoProduct.autoProductionScript.fillDuration <= autoProduct.fillDurationMaxed)
            {
                autoProduct.PanelMaxUpgrade.SetActive(true);
                if (autoProduct.coinsbar1Text != null)
                {
                    autoProduct.coinsbar1Text.gameObject.SetActive(false);
                }
            }

            if (PlayerPrefs.HasKey(autoProduct.name + "_UpgradeCostbar1"))
            {
                autoProduct.upgradeCostbar1 = PlayerPrefs.GetInt(autoProduct.name + "_UpgradeCostbar1");
            }

            if (autoProduct.coinsbar1Text != null)
            {
                autoProduct.coinsbar1Text.text = "- 0.5 seconds for " + autoProduct.upgradeCostbar1.ToString();
            }

            string key = PlayerPrefs.GetString(autoProduct.IndexForUnlockColors);
            if (key == autoProduct.IndexForUnlockColors)
            {
                if (autoProduct.PanelLock != null)
                {
                    autoProduct.PanelLock.SetActive(false);
                }
                if (autoProduct.autoProductionScript != null)
                {
                    autoProduct.autoProductionScript.gameObject.SetActive(true);
                }
            }
        }



        foreach (warehouseUpgrade warehouse in warehouses)
        {
            if (PlayerPrefs.HasKey(warehouse.name + "_UnlockedSlots"))
            {
                warehouse.maxSlots = PlayerPrefs.GetInt(warehouse.name + "_UnlockedSlots", 15);
            }

            if (PlayerPrefs.HasKey(warehouse.name + "_UpgradeCostbarSlots"))
            {
                warehouse.upgradeCostbarSlots = PlayerPrefs.GetInt(warehouse.name + "_UpgradeCostbarSlots");
            }

            if (warehouse.countSlots != null)
            {
                warehouse.countSlots.text = "Slots Available " + warehouse.maxSlots.ToString() + "/30";
            }

            if (warehouse.maxSlots >= 30 && warehouse.panelMaxUpgrades != null)
            {
                warehouse.panelMaxUpgrades.SetActive(true);
                warehouse.coinsbarTextSlots.gameObject.SetActive(false);

            }

            if (warehouse.coinsbarTextSlots != null)
            {
                warehouse.coinsbarTextSlots.text = "+ 1 slot for " + warehouse.upgradeCostbarSlots.ToString();
            }
        }

        coinsbalancecheck();

        foreach (ProductUpgrade colorsUnlock in Product)
        {
            if (colorsUnlock.PanelLock != null && !colorsUnlock.PanelLock.activeSelf)
            {
                if (colorsUnlock.autoProductionScript != null)
                {
                    colorsUnlock.autoProductionScript.gameObject.SetActive(true);
                }
            }

            colorsUnlock.countColorUnlocks = 0;

            if (colorUnlockScript != null)
            {
                foreach (GameObject unlockColors in colorUnlockScript.panelforUnlock)
                {
                    if (unlockColors != null && !unlockColors.activeSelf)
                    {
                        colorsUnlock.countColorUnlocks++;
                    }
                }
            }

            if (colorsUnlock.TextNeedColorUnlockCounts != null)
            {
                colorsUnlock.TextNeedColorUnlockCounts.text = "You need unlock " + colorsUnlock.countColorUnlocks.ToString() + "/" + colorsUnlock.NeedColorsForUnlockProduct.ToString() + " colors";
            }
        }

    }

    private void LateUpdate()
    {
       

        

        foreach (ProductUpgrade autoProduct in Product)
        {
            if (autoProduct.PanelMaxUpgrade != null && autoProduct.autoProductionScript != null && autoProduct.autoProductionScript.fillDuration <= autoProduct.fillDurationMaxed)
            {
                autoProduct.PanelMaxUpgrade.SetActive(true);
                if (autoProduct.coinsbar1Text != null)
                {
                    autoProduct.coinsbar1Text.gameObject.SetActive(false);
                }
            }



            if (colorUnlockScript != null && colorUnlockScript.panelUnlockSucces)
            {
                string key = PlayerPrefs.GetString(autoProduct.IndexForUnlockColors);
                if (key == autoProduct.IndexForUnlockColors)
                {
                    if (autoProduct.PanelLock != null)
                    {
                        autoProduct.PanelLock.SetActive(false);
                    }
                    if (autoProduct.autoProductionScript != null)
                    {
                        autoProduct.autoProductionScript.gameObject.SetActive(true);
                    }
                    colorUnlockScript.panelUnlockSucces = false;
                }
            }


        }

        foreach (warehouseUpgrade warehouse in warehouses)
        {
            
            if (warehouse.maxSlots >= 30 && warehouse.panelMaxUpgrades != null)
            {
                warehouse.panelMaxUpgrades.SetActive(true);
                warehouse.coinsbarTextSlots.gameObject.SetActive(false);

            }

            
        }


        foreach (ProductUpgrade colorsUnlock in Product)
        {
            if (colorsUnlock.PanelLock != null && !colorsUnlock.PanelLock.activeSelf)
            {
                if (colorsUnlock.autoProductionScript != null)
                {
                    colorsUnlock.autoProductionScript.gameObject.SetActive(true);
                }
            }

            colorsUnlock.countColorUnlocks = 0;

            if (colorUnlockScript != null)
            {
                foreach (GameObject unlockColors in colorUnlockScript.panelforUnlock)
                {
                    if (unlockColors != null && !unlockColors.activeSelf)
                    {
                        colorsUnlock.countColorUnlocks++;
                    }
                }
            }

            if (colorsUnlock.TextNeedColorUnlockCounts != null)
            {
                colorsUnlock.TextNeedColorUnlockCounts.text = "You need unlock " + colorsUnlock.countColorUnlocks.ToString() + "/" + colorsUnlock.NeedColorsForUnlockProduct.ToString() + " colors";
            }
        }
    }

    void coinsbalancecheck()
    {
        coinsBalance = PlayerPrefs.GetInt("CoinsBalance");
    }

    void UpdateCoinsBalanceDisplay()
    {
        coinsBalanceText.text = coinsBalance.ToString();
    }

    public void UpgradeDecrase(string ProductName)
    {
        coinsbalancecheck();
        ProductUpgrade AutoProduct = Product.Find(x => x.name == ProductName);
        if (AutoProduct != null)
        {
            if (coinsBalance >= AutoProduct.upgradeCostbar1)
            {
                if (audiomanager != null)
                {
                    audiomanager.PlaySFX(audiomanager.upgradebutton);
                }

                coinsBalance -= AutoProduct.upgradeCostbar1;
                PlayerPrefs.SetInt("CoinsBalance", coinsBalance);
                PlayerPrefs.Save();
                UpdateCoinsBalanceDisplay();

                float currentSaveInterval = PlayerPrefs.GetFloat(AutoProduct.name + "_decraseTime_", AutoProduct.autoProductionScript.saveInterval);
                float currentFillDuration = PlayerPrefs.GetFloat(AutoProduct.name + "_decraseTimeFill_", AutoProduct.autoProductionScript.fillDuration);

                float newSaveInterval = Mathf.Max(currentSaveInterval - AutoProduct.newFloatInverval, AutoProduct.newFloatInverval);
                float newFillDuration = Mathf.Max(currentFillDuration - 0.5f, 0.5f);

                AutoProduct.autoProductionScript.saveInterval = newSaveInterval;
                AutoProduct.autoProductionScript.fillDuration = newFillDuration;

                PlayerPrefs.SetFloat(AutoProduct.name + "_decraseTime_", newSaveInterval);
                PlayerPrefs.SetFloat(AutoProduct.name + "_decraseTimeFill_", newFillDuration);
                PlayerPrefs.Save();

                AutoProduct.upgradeCostbar1 += 170;

                PlayerPrefs.SetInt(AutoProduct.name + "_UpgradeCostbar1", AutoProduct.upgradeCostbar1);
                PlayerPrefs.Save();

                if (AutoProduct.autoProductionScript != null)
                {
                    AutoProduct.autoProductionScript.saveInterval = PlayerPrefs.GetFloat(AutoProduct.name + "_decraseTime_", AutoProduct.autoProductionScript.saveInterval);

                    AutoProduct.autoProductionScript.fillDuration = PlayerPrefs.GetFloat(AutoProduct.name + "_decraseTimeFill_", AutoProduct.autoProductionScript.fillDuration);


                }
                AutoProduct.coinsbar1Text.text = "- 0.5 seconds for " + AutoProduct.upgradeCostbar1.ToString();
                AutoProduct.autoProductionScript.saveInterval = PlayerPrefs.GetFloat(AutoProduct.autoProductionScript.savedTimeProduct, AutoProduct.autoProductionScript.defaultSaveInterval);
                AutoProduct.autoProductionScript.UpdateTimeText();
            }
        }
    }

    public void UpgradeSlots(string warehouseName)
    {
        coinsbalancecheck();
        warehouseUpgrade WarehouseUpgrade = warehouses.Find(x => x.name == warehouseName);
        if (WarehouseUpgrade != null)
        {
            if (coinsBalance >= WarehouseUpgrade.upgradeCostbarSlots)
            {
                if (audiomanager != null)
                {
                    audiomanager.PlaySFX(audiomanager.upgradebutton);
                }

                coinsBalance -= WarehouseUpgrade.upgradeCostbarSlots;
                PlayerPrefs.SetInt("CoinsBalance", coinsBalance);
                PlayerPrefs.Save();
                UpdateCoinsBalanceDisplay();

                int currentMaxSlots = PlayerPrefs.GetInt(WarehouseUpgrade.name + "_UnlockedSlots", WarehouseUpgrade.maxSlots) + 1;

                PlayerPrefs.SetInt(WarehouseUpgrade.name + "_UnlockedSlots", currentMaxSlots);
                PlayerPrefs.Save();

              

                WarehouseUpgrade.upgradeCostbarSlots += 110;

                PlayerPrefs.SetInt(WarehouseUpgrade.name + "_UpgradeCostbarSlots", WarehouseUpgrade.upgradeCostbarSlots);
                PlayerPrefs.Save();

                if (PlayerPrefs.HasKey(WarehouseUpgrade.name + "_UpgradeCostbarSlots"))
                {
                    WarehouseUpgrade.upgradeCostbarSlots = PlayerPrefs.GetInt(WarehouseUpgrade.name + "_UpgradeCostbarSlots");
                }
                if (PlayerPrefs.HasKey(WarehouseUpgrade.name + "_UnlockedSlots"))
                {
                    WarehouseUpgrade.maxSlots = PlayerPrefs.GetInt(WarehouseUpgrade.name + "_UnlockedSlots", 15);
                }

                WarehouseUpgrade.coinsbarTextSlots.text = "+ 1 slot for " + WarehouseUpgrade.upgradeCostbarSlots.ToString();
                WarehouseUpgrade.countSlots.text = "Slots Available " + WarehouseUpgrade.maxSlots.ToString() + "/30";
                


                inventoryslots.InitializeSlots();
            }
        }
    }
}
