using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Victory;

public class CharacterChoiceClothes : MonoBehaviour
{
    [Header("Index Order")]
    public List<string> indexKeys = new List<string>();

    private Victory victoryScript;
    public GameObject PanelForActivate;
    public GameObject ParentObject;
    public TextMeshProUGUI[] Ratingtext;
    public GameObject[] ChildObjects;
    public int activateChildForWin = 1;
    private HashSet<GameObject> activatedChildObjects = new HashSet<GameObject>();
    private bool isCoroutineRunning = false;
    private int currentChildIndex = 0;

    public int PriceForSlots = 300;
    public string clothesElements = "ColorShirt_";
    public TextMeshProUGUI[] countForSell;
    public string slotsforSell = "Shirt_SellObjects";
    public int SellSlots;
    public int limitClothes = 7;
    private int countToDelete;
    private bool[] valueSetFlags;

    private int count1 = 0;
    private int count2 = 0;
    private int count3 = 0;
    private int coins;
    private int totalAdjustedPriceSlots = 0;
    void Awake()
    {
        coins = PlayerPrefs.GetInt("CoinsBalance", 0);
        victoryScript = FindAnyObjectByType<Victory>();
        SellSlots = PlayerPrefs.GetInt(slotsforSell, 0);
        valueSetFlags = new bool[countForSell.Length];
        for (int i = 0; i < valueSetFlags.Length; i++)
        {
            valueSetFlags[i] = false;
        }
       
    }

    private void Update()
    {
        UpdateTextMeshProUGUI(SellSlots);

       
    }
    public void textToPrice(string textPrize)
    {
        
        if (Ratingtext.Length == 0) return;

        
        for (int i = 0; i < Ratingtext.Length; i++)
        {
            TextMeshProUGUI text = Ratingtext[i];

            
            if (text.gameObject.activeSelf && string.IsNullOrEmpty(text.text))
            {
                
                text.text = textPrize;
                break;
            }
        }
    }

    public void ChildObject()
    {
        StartCoroutine(AnimateNextChild());
    }
   
    private IEnumerator AnimateNextChild()
    {
        
        if (ChildObjects.Length == 0) yield break;

        
        GameObject childToAnimate = ChildObjects[currentChildIndex];
        if (childToAnimate != null)
        {
            
            if (!childToAnimate.activeSelf)
            {
                childToAnimate.SetActive(true);
            }
            yield return StartCoroutine(AnimateObject(childToAnimate));

            
            childToAnimate.transform.SetParent(ParentObject.transform);

        }


        currentChildIndex = (currentChildIndex + 1) % ChildObjects.Length;
    }

    private IEnumerator AnimateObject(GameObject objectToAnimate)
    {
        Vector3 targetPosition1 = new Vector3(135.83f, 14061.12f, objectToAnimate.transform.localPosition.z);
        Vector3 targetPosition2 = new Vector3(4.70f, 135f, objectToAnimate.transform.localPosition.z);

        yield return AnimatePosition(objectToAnimate, targetPosition1, 0.3f);
        yield return AnimatePosition(objectToAnimate, targetPosition2, 1f);
        yield return new WaitForSeconds(0.1f);

    }

    private IEnumerator AnimatePosition(GameObject obj, Vector3 targetLocalPosition, float duration)
    {
        Vector3 startLocalPosition = obj.transform.localPosition; 

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            obj.transform.localPosition = Vector3.Lerp(startLocalPosition, targetLocalPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.localPosition = targetLocalPosition;
    }


    private void UpdateTextMeshProUGUI(int slotsForSell)
    {
        
        if (slotsForSell <= 7)
        {
            if (countForSell.Length > 0 && !valueSetFlags[0])
            {
                countForSell[0].text = slotsForSell.ToString();
                valueSetFlags[0] = true;
            }

            if (countForSell[0].gameObject.activeInHierarchy)
            {
                if (!isCoroutineRunning)
                {
                    foreach (objectsForWin obj in victoryScript.objects)
                    {
                        if (obj.name == activateChildForWin)
                        {
                            if (count1 <= 1)
                            {
                                count1++;
                            }

                            int priceSlots = slotsForSell * PriceForSlots;
                            int adjustedPriceSlots = priceSlots;

                            string rating = Ratingtext[0].text;

                            if (rating == "Bad")
                            {
                                adjustedPriceSlots = (int)(priceSlots * 0.85f);
                                obj.textShowPlusOrMinus[0].text = "(-15%)";
                                obj.textShowPlusOrMinus[0].color = Color.red;

                            }
                            else if (rating == "Very Bad")
                            {
                                adjustedPriceSlots = (int)(priceSlots * 0.70f); 
                                obj.textShowPlusOrMinus[0].text = "(-30%)";
                                obj.textShowPlusOrMinus[0].color = Color.red;

                            }
                            else if (rating == "Good")
                            {
                                adjustedPriceSlots = (int)(priceSlots * 1.20f); 
                                obj.textShowPlusOrMinus[0].text = "(+20%)";
                                obj.textShowPlusOrMinus[0].color = Color.green;

                            }
                            else if (rating == "Normal")
                            {
                                adjustedPriceSlots = (int)(adjustedPriceSlots * 1.1f);
                                obj.textShowPlusOrMinus[0].text = "(+10%)";
                                obj.textShowPlusOrMinus[0].color = Color.green;
                            }

                            obj.priceEarn[0].text = "= " + adjustedPriceSlots.ToString();
                            obj.countSellObjects[0].text = countForSell[0].text;
                            obj.ratingForObject[0].text = rating;
                            obj.ObjectsForWin[0].SetActive(true);
                            if (count1 <= 1)
                            {
                                totalAdjustedPriceSlots += adjustedPriceSlots;
                            }

                            if (count1 == 1)
                            {
                                coins += totalAdjustedPriceSlots;
                                PlayerPrefs.SetInt("CoinsBalance", coins);
                                PlayerPrefs.Save();
                                victoryScript.totalEarn.text = "Total: " + totalAdjustedPriceSlots.ToString();
                            }
                        }
                    }
                    StartCoroutine(victoryDelay());
                }
            }

        }
        else if (slotsForSell > 7 && slotsForSell <= 18)
        {
            if (countForSell.Length > 1 && !valueSetFlags[0] && !valueSetFlags[1])
            {
                int firstCount = Random.Range(1, slotsForSell);
                int secondCount = slotsForSell - firstCount;

                countForSell[0].text = firstCount.ToString();
                countForSell[1].text = secondCount.ToString();
                valueSetFlags[0] = true;
                valueSetFlags[1] = true;
            }

            if (countForSell[0].gameObject.activeInHierarchy && countForSell[1].gameObject.activeInHierarchy)
            {
                foreach (objectsForWin obj in victoryScript.objects)
                {

                    if (obj.name == activateChildForWin)
                    {
                        if (!isCoroutineRunning)
                        {
                            StartCoroutine(victoryDelay());
                        }
                        int priceSlots = slotsForSell * PriceForSlots;
                      

                        for (int i = 0; i < 2; i++)
                        {
                            if (count2 <= 2)
                            {
                                count2++;
                            }

                            string rating = Ratingtext[i].text;

                            int countForSellValue = int.Parse(countForSell[i].text); 
                            int adjustedPriceSlots = countForSellValue * PriceForSlots; 

                            
                            if (rating == "Bad")
                            {
                                adjustedPriceSlots = (int)(adjustedPriceSlots * 0.85f); 
                                obj.textShowPlusOrMinus[i].text = "(-15%)";
                                obj.textShowPlusOrMinus[i].color = Color.red;
                            }
                            else if (rating == "Very Bad")
                            {
                                adjustedPriceSlots = (int)(adjustedPriceSlots * 0.70f); 
                                obj.textShowPlusOrMinus[i].text = "(-30%)";
                                obj.textShowPlusOrMinus[i].color = Color.red;
                            }
                            else if (rating == "Good")
                            {
                                obj.textShowPlusOrMinus[i].text = "(+20%)";
                                obj.textShowPlusOrMinus[i].color = Color.green;
                                adjustedPriceSlots = (int)(adjustedPriceSlots * 1.20f); 
                            }
                            else if (rating == "Normal")
                            {
                                obj.textShowPlusOrMinus[i].text = "(+10%)";
                                obj.textShowPlusOrMinus[i].color = Color.green;
                                adjustedPriceSlots = (int)(adjustedPriceSlots * 1.1f);
                            }
                            obj.priceEarn[i].text = "= " + adjustedPriceSlots.ToString();
                            obj.countSellObjects[i].text = countForSell[i].text;
                            obj.ratingForObject[i].text = rating;
                            obj.ObjectsForWin[i].SetActive(true);

                            if (count2 <= 2)
                            {
                                totalAdjustedPriceSlots += adjustedPriceSlots;
                            }

                            if (count2 == 2)
                            {
                                coins += totalAdjustedPriceSlots;
                                PlayerPrefs.SetInt("CoinsBalance", coins);
                                PlayerPrefs.Save();
                                victoryScript.totalEarn.text = "Total: " + totalAdjustedPriceSlots.ToString();
                            }


                        }

                    }
                }

            }
        }
        else if (slotsForSell > 18 && slotsForSell <= 30)
        {
            if (countForSell.Length > 2 && !valueSetFlags[0] && !valueSetFlags[1] && !valueSetFlags[2])
            {
                int firstCount = Random.Range(1, slotsForSell - 1);
                int secondCount = Random.Range(1, slotsForSell - firstCount);
                int thirdCount = slotsForSell - firstCount - secondCount;

                countForSell[0].text = firstCount.ToString();
                countForSell[1].text = secondCount.ToString();
                countForSell[2].text = thirdCount.ToString();
                valueSetFlags[0] = true;
                valueSetFlags[1] = true;
                valueSetFlags[2] = true;
            }

            if (countForSell[0].gameObject.activeInHierarchy && countForSell[1].gameObject.activeInHierarchy && countForSell[2].gameObject.activeInHierarchy)
            {
                foreach (objectsForWin obj in victoryScript.objects)
                {
                    if (obj.name == activateChildForWin)
                    {
                        int priceSlots = slotsForSell * PriceForSlots;

                        for (int i = 0; i < 3; i++)
                        {
                            string rating = Ratingtext[i].text;
                            if (count3 <= 3)
                            {
                                count3++;
                            }
                            int countForSellValue = int.Parse(countForSell[i].text); 
                            int adjustedPriceSlots = countForSellValue * PriceForSlots; 

                            if (rating == "Bad")
                            {
                                adjustedPriceSlots = (int)(adjustedPriceSlots * 0.85f); 
                                obj.textShowPlusOrMinus[i].text = "(-15%)";
                                obj.textShowPlusOrMinus[i].color = Color.red;
                            }
                            else if (rating == "Very Bad")
                            {
                                obj.textShowPlusOrMinus[i].text = "(-30%)";
                                obj.textShowPlusOrMinus[i].color = Color.red;
                                adjustedPriceSlots = (int)(adjustedPriceSlots * 0.70f); 
                            }
                            else if (rating == "Good")
                            {
                                obj.textShowPlusOrMinus[i].text = "(+20%)";
                                obj.textShowPlusOrMinus[i].color = Color.green;
                                adjustedPriceSlots = (int)(adjustedPriceSlots * 1.20f); 
                            }
                            else if (rating == "Normal")
                            {
                                obj.textShowPlusOrMinus[i].text = "(+10%)";
                                obj.textShowPlusOrMinus[i].color = Color.green;
                                adjustedPriceSlots = (int)(adjustedPriceSlots * 1.1f); 
                            }

                            obj.priceEarn[i].text = "= " + adjustedPriceSlots.ToString();
                            obj.countSellObjects[i].text = countForSell[i].text;
                            obj.ratingForObject[i].text = rating;
                            obj.ObjectsForWin[i].SetActive(true);
                            if (count3 <= 3)
                            {
                                totalAdjustedPriceSlots += adjustedPriceSlots;
                            }

                            if (count3 == 3)
                            {
                                coins += totalAdjustedPriceSlots;
                                PlayerPrefs.SetInt("CoinsBalance", coins);
                                PlayerPrefs.Save();
                                victoryScript.totalEarn.text = "Total: " + totalAdjustedPriceSlots.ToString();
                            }
                        }

                        if (!isCoroutineRunning)
                        {
                            StartCoroutine(victoryDelay());
                        }
                    }
                }

            }
        }
    }



    private IEnumerator victoryDelay()
    {
        isCoroutineRunning = true;
        yield return new WaitForSeconds(1.37f);
        victoryScript.victory();
    }
    public void DeletePlayerPrefsKeys()
    {
        if (gameObject.activeInHierarchy)
        {
            int maxColorShirtIndex = SellSlots * limitClothes;

            
            int keysToDelete = maxColorShirtIndex;
            int currentIndex = 1;

            while (keysToDelete > 0)
            {
                string key = clothesElements + currentIndex;

                if (PlayerPrefs.HasKey(key))
                {
                    PlayerPrefs.DeleteKey(key);
                    keysToDelete--;
                }

                currentIndex++;
            }

            
            UpdateIndexKeys();

            
            if (PlayerPrefs.HasKey(slotsforSell))
            {
                PlayerPrefs.DeleteKey(slotsforSell);
            }
        }
    }

    private void UpdateIndexKeys()
    {
        int startIndex = 1;

        
        foreach (string indexKey in indexKeys)
        {
            if (PlayerPrefs.HasKey(indexKey))
            {
                PlayerPrefs.DeleteKey(indexKey);
            }

            
            startIndex++;

            
            if (startIndex > 7)
            {
                break;
            }
        }
    }




}
