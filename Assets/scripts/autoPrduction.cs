using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static initializeSlotsInventory;

public class AutoProduction : MonoBehaviour
{
    public float defaultSaveInterval = 1.0f;
    public int startIndex = 210;
    public string prefsKeyPrefix = "ColorShirt_";
    public Image[] clothesElements;

    public TextMeshProUGUI timeTextShow;
    public Image fillImage;

    private initializeSlotsInventory inventory;

    public string savedSlots = "ShirtSlots_UnlockedSlots";
    private string savedCounts = "CountsProduct";
    public string savedFillBar = "Shirt_decraseTimeFill_";
    public string savedTimeProduct = "Shirt_decraseTime_";
    public float fillDuration = 2.0f;

    private Color[] allColors = new Color[6];
    private bool hasUnlockedColors = false;
    public float saveInterval;
    private float fillAmount = 0f;
    private float elapsedTime = 0f;
    public int maxSlots = 15;
    private int countProductionForActiveObjects;
    public int countIndexForSaveProduct;

    public GameObject activeObjects;

    private bool isCoroutineRunningStart = false;
    private bool isCoroutineRunningUpdate = false;

    public bool productCompleteUpdateObjects = true;
    private Coroutine fillCoroutine;

    private void Start()
    {
        saveInterval = PlayerPrefs.GetFloat(savedTimeProduct, defaultSaveInterval);
        fillDuration = PlayerPrefs.GetFloat(savedFillBar, fillDuration);

        inventory = FindAnyObjectByType<initializeSlotsInventory>();
        InitializeColors();
        if (!isCoroutineRunningStart && maxSlots > countProductionForActiveObjects) 
        {
            StartCoroutine(SaveIndicesCoroutine());
        }
        UpdateTimeText();
        
    }
   
    public void UpdateTimeText()
    {
        fillDuration = PlayerPrefs.GetFloat(savedFillBar, fillDuration);

        int minutes = Mathf.FloorToInt(fillDuration / 60f);
        int seconds = Mathf.FloorToInt(fillDuration % 60f);

        if (timeTextShow != null)
        {
            timeTextShow.text = $"Time Production: {minutes:D2}:{seconds:D2}";
        }
    }

    private void saveCompleteProduct()
    {
        int highestExistingIndex = -1;

        
        for (int i = 0; i <= 1501; i++)  
        {
            string key = "ProductionComplete_" + i;
            if (PlayerPrefs.HasKey(key))
            {
                highestExistingIndex = i;
            }
        }

        
        int nextIndex = highestExistingIndex + 1;
        string nextKey = "ProductionComplete_" + nextIndex;
        PlayerPrefs.SetInt(nextKey, 0);
        PlayerPrefs.Save();
    }
   

    private void LateUpdate()
    {

        foreach (sectionsSlots inv in inventory.slots)
        {
            if (inv.savedSlots == savedSlots && gameObject.activeSelf)
            {
                maxSlots = inv.slotsAvalible;
                countProductionForActiveObjects = inv.countActiveClothes;
            }
            
        }
        if (maxSlots > countProductionForActiveObjects && !isCoroutineRunningStart && isCoroutineRunningUpdate)
        {
            StartCoroutine(SaveIndicesCoroutine());
        }




       if (maxSlots == countProductionForActiveObjects && fillImage != null)
       {
            fillImage.fillAmount = 1f;
       }

       if (maxSlots != countProductionForActiveObjects && fillImage != null)
        {
            elapsedTime += Time.deltaTime;
            float newFillAmount = Mathf.Clamp01(elapsedTime / fillDuration);


            if (Mathf.Abs(newFillAmount - fillImage.fillAmount) > 0.001f)
            {
                fillImage.fillAmount = newFillAmount;
            }


            if (newFillAmount >= 1f)
            {
                elapsedTime = 0f;
                fillImage.fillAmount = 0f;
            }
        }
       


    }
   
    private IEnumerator SaveIndicesCoroutine()
    {
        isCoroutineRunningStart = true;
        isCoroutineRunningUpdate = false;

        int startIndexs = startIndex;
        int endIndex = 1;

        for (int i = startIndexs; i >= endIndex; i--)
        {
            if (maxSlots > countProductionForActiveObjects)
            {
                string key = prefsKeyPrefix + i;
                if (!PlayerPrefs.HasKey(key))
                {
                    PlayerPrefs.SetInt(key, i);
                    int value = PlayerPrefs.GetInt(key, i);

                   

                    PlayerPrefs.Save();

                    if (i % countIndexForSaveProduct == 0)
                    {
                        saveCompleteProduct();

                        int countProduction = 0;

                        if (PlayerPrefs.HasKey(savedCounts))
                        {
                            countProduction = PlayerPrefs.GetInt(savedCounts, 0);
                        }

                        countProduction++;


                        PlayerPrefs.SetInt(savedCounts, countProduction);
                        PlayerPrefs.Save();
                        productCompleteUpdateObjects = true;
                    }

                    int colorIndex = i;
                    if (colorIndex < clothesElements.Length && clothesElements[colorIndex] != null)
                    {
                        if (hasUnlockedColors)
                        {
                            clothesElements[colorIndex].color = GetRandomUnlockedColor();

                            
                        }
                    }

                    if (countProductionForActiveObjects >= maxSlots)
                    {
                        isCoroutineRunningStart = false;
                        isCoroutineRunningUpdate = true;
                        yield break;
                    }

                    yield return new WaitForSeconds(saveInterval);

                }
            }
        }

        isCoroutineRunningStart = false;
        PlayerPrefs.Save();
        isCoroutineRunningUpdate = true;
    }





    private Color GetRandomUnlockedColor()
    {
        int randomIndex = Random.Range(0, allColors.Length);
        return allColors[randomIndex];
    }

    private Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }

    private void InitializeColors()
    {
        allColors[0] = HexToColor("7C00FF");
        allColors[1] = HexToColor("FF0000");
        allColors[2] = HexToColor("FF00D6");
        allColors[3] = HexToColor("6495ED");
        allColors[4] = HexToColor("DC143C");

        for (int i = 5; i <= 9; i++)
        {
            if (PlayerPrefs.HasKey("UnlockColor_" + (i + 1)) &&
                PlayerPrefs.GetInt("UnlockColor_" + (i + 1), 0) == 1)
            {
                hasUnlockedColors = true;
            }
        }
    }
}
