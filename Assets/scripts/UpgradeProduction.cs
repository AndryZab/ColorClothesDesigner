using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        public int fillDurationMaxed;
        public string IndexForUnlockColors;
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

    private void Start()
    {
        colorUnlockScript = FindObjectOfType<ColorUnlockScript>();
        audiomanager = FindAnyObjectByType<Audiomanager>();
    }

    private void Update()
    {
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

            if (autoProduct.PanelMaxUpgrade != null && autoProduct.autoProductionScript != null && autoProduct.autoProductionScript.fillDuration < autoProduct.fillDurationMaxed)
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

                float decreaseAmount = 0.01f * AutoProduct.multiplier;
                float newSaveInterval = Mathf.Max(Mathf.Round((currentSaveInterval - decreaseAmount) * 10) / 10, 0.01f);
                float newFillDuration = Mathf.Max(Mathf.Round((currentFillDuration - decreaseAmount * AutoProduct.multiplier) * 10) / 10, 1f);

                AutoProduct.autoProductionScript.saveInterval = newSaveInterval;
                AutoProduct.autoProductionScript.fillDuration = newFillDuration;

                PlayerPrefs.SetFloat(AutoProduct.name + "_decraseTime_", newSaveInterval);
                PlayerPrefs.SetFloat(AutoProduct.name + "_decraseTimeFill_", newFillDuration);
                PlayerPrefs.Save();

                AutoProduct.upgradeCostbar1 += 50;

                PlayerPrefs.SetInt(AutoProduct.name + "_UpgradeCostbar1", AutoProduct.upgradeCostbar1);
                PlayerPrefs.Save();

                AutoProduct.coinsbar1Text.text = "- 0.5 seconds for " + AutoProduct.upgradeCostbar1.ToString();
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

                if (WarehouseUpgrade.maxSlots >= 30)
                {
                    WarehouseUpgrade.panelMaxUpgrades.SetActive(true);
                    WarehouseUpgrade.coinsbarTextSlots.gameObject.SetActive(false);
                }

                WarehouseUpgrade.upgradeCostbarSlots += 30;

                PlayerPrefs.SetInt(WarehouseUpgrade.name + "_UpgradeCostbarSlots", WarehouseUpgrade.upgradeCostbarSlots);
                PlayerPrefs.Save();

                WarehouseUpgrade.coinsbarTextSlots.text = "+ 1 slot for " + WarehouseUpgrade.upgradeCostbarSlots.ToString();
                WarehouseUpgrade.countSlots.text = "Slots Available " + WarehouseUpgrade.maxSlots.ToString() + "/30";
            }
        }
    }
}
