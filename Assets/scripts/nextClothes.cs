using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckColorsAndAnimate : MonoBehaviour
{
    [System.Serializable]
    public class NextClotheSection
    {
        public string indexAdd;
        public Image[] images;
        public GameObject objectToAnimate;
        public paintClothes[] paintclothes;
        public string savedForCounts = "CountsProduct";
        public int addSlots;
        public float targetPositionX1;
    }

    private int currentIndex = 1;

    [SerializeField] public List<NextClotheSection> nextClotheSection;
    public GameObject[] scriptsAutoProduct;
    private bool animationDefaultTriggered = false;
    private bool animationActiveObject = true;
    private bool animationRandomTriggered = false;

    private void Start()
    {
        int[] unlockThresholds = { 3, 7 };

        for (int i = 0; i < unlockThresholds.Length; i++)
        {
            bool allKeysPresent = true;
            for (int j = 0; j <= unlockThresholds[i]; j++)
            {
                if (!PlayerPrefs.HasKey("ColorUnlock_" + j))
                {
                    allKeysPresent = false;
                    break;
                }
            }

            if (allKeysPresent && i < scriptsAutoProduct.Length)
            {
                scriptsAutoProduct[i].SetActive(true);
            }
        }
        SaveCurrentIndexes();
        LoadIndexes();
    }

    private void SaveCurrentIndexes()
    {
        foreach (var section in nextClotheSection)
        {
            if (int.TryParse(section.indexAdd, out int indexAddValue))
            {
                bool anyKeyExists = false;

                for (int i = 0; i < section.paintclothes.Length && i < indexAddValue; i++)
                {
                    var clotheScript = section.paintclothes[i];
                    if (clotheScript == null) continue;

                    string key = "CurrentIndex_" + clotheScript.name;

                    if (PlayerPrefs.HasKey(key))
                    {
                        anyKeyExists = true;
                        break;
                    }
                }

                if (!anyKeyExists)
                {
                    for (int i = 0; i < section.paintclothes.Length && i < indexAddValue; i++)
                    {
                        var clotheScript = section.paintclothes[i];
                        if (clotheScript == null) continue;

                        string key = "CurrentIndex_" + clotheScript.name;

                        PlayerPrefs.SetString(key, (i + 1).ToString());
                    }

                    PlayerPrefs.Save();
                }
            }
        }
    }

    private void indexApperAfterAnim()
    {
        foreach (var section in nextClotheSection)
        {
            if (section.objectToAnimate != null && section.objectToAnimate.activeSelf)
            {
                foreach (var clotheScript in section.paintclothes)
                {
                    if (clotheScript == null) continue;

                    if (int.TryParse(clotheScript.sections.index, out int currentIndex) &&
                        int.TryParse(section.indexAdd, out int indexAddValue))
                    {
                        currentIndex += indexAddValue;
                        clotheScript.sections.index = currentIndex.ToString();
                        string key = GetClotheScriptKey(clotheScript);

                        PlayerPrefs.SetString(key, clotheScript.sections.index);

                        PlayerPrefs.Save();
                    }
                }
            }
        }
    }

    private int GetHighestExistingValue(string baseKey)
    {
        if (PlayerPrefs.HasKey(baseKey))
        {
            return PlayerPrefs.GetInt(baseKey);
        }
        return 0;
    }

    private string GetClotheScriptKey(paintClothes clotheScript)
    {
        return "CurrentIndex_" + clotheScript.name;
    }

    private void LoadIndexes()
    {
        foreach (var section in nextClotheSection)
        {
            foreach (var clotheScript in section.paintclothes)
            {
                if (clotheScript == null) continue;

                string key = GetClotheScriptKey(clotheScript);
                if (PlayerPrefs.HasKey(key))
                {
                    string savedIndex = PlayerPrefs.GetString(key);
                    clotheScript.sections.index = savedIndex;
                }
            }
        }
    }

    private void Update()
    {
        LoadIndexes();
        if (currentIndex == 5)
        {
            currentIndex = 0;
        }
        if (!animationDefaultTriggered && AllImagesNonWhite())
        {
            StartCoroutine(AnimateObject());
        }
        if (!animationRandomTriggered && !animationActiveObject)
        {
            StartCoroutine(AnimateObjectRandom());
        }

        foreach (var section in nextClotheSection)
        {
            if (section.objectToAnimate != null && !section.objectToAnimate.activeSelf)
            {
                foreach (Image image in section.images)
                {
                    if (image != null && image.color != Color.white)
                    {
                        image.color = Color.white;
                    }
                }
            }
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

    private IEnumerator AnimateObject()
    {
        indexApperAfterAnim();
        NextClotheSection activeSection = null;
        foreach (var section in nextClotheSection)
        {
            if (section.objectToAnimate != null && section.objectToAnimate.activeSelf)
            {
                activeSection = section;
                break;
            }
        }

        if (activeSection == null)
        {
            yield break;
        }

        foreach (var section in nextClotheSection)
        {
            if (section.objectToAnimate != null && section.objectToAnimate.activeSelf)
            {
                int highestExistingValue = GetHighestExistingValue(section.savedForCounts);
                section.addSlots = highestExistingValue + 1;

                PlayerPrefs.SetInt(section.savedForCounts, section.addSlots);

                break;
            }
        }

        GameObject objectToAnimate = activeSection.objectToAnimate;

        animationDefaultTriggered = true;
        saveCompleteProduct();

        Vector3 targetPosition1 = new Vector3(881, objectToAnimate.transform.localPosition.y, objectToAnimate.transform.localPosition.z);
        Vector3 targetPosition2 = new Vector3(-1012, objectToAnimate.transform.localPosition.y, objectToAnimate.transform.localPosition.z);

        yield return MoveObjectToPosition(objectToAnimate, targetPosition1, 1f);
        objectToAnimate.SetActive(false);

        yield return MoveObjectToPosition(objectToAnimate, targetPosition2, 1f);
        objectToAnimate.SetActive(true);

        animationDefaultTriggered = false;
        animationActiveObject = false;

        yield return new WaitForSeconds(0.5f);
        objectToAnimate.SetActive(false);
    }

    private IEnumerator MoveObjectToPosition(GameObject obj, Vector3 targetLocalPosition, float duration)
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

    private IEnumerator AnimateObjectRandom()
    {
        animationRandomTriggered = true;

        if (nextClotheSection.Count == 0)
        {
            animationRandomTriggered = false;
            yield break;
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, nextClotheSection.Count - 1);

        NextClotheSection sectionToAnimate = nextClotheSection[currentIndex];
        GameObject objectToAnimate = sectionToAnimate.objectToAnimate;

        if (objectToAnimate == null)
        {
            animationRandomTriggered = false;
            yield break;
        }

        Vector3 targetPosition1 = new Vector3(-1012, objectToAnimate.transform.localPosition.y, objectToAnimate.transform.localPosition.z);
        Vector3 targetPosition2 = new Vector3(sectionToAnimate.targetPositionX1, objectToAnimate.transform.localPosition.y, objectToAnimate.transform.localPosition.z);

        yield return MoveObjectToPosition(objectToAnimate, targetPosition1, 1f);
        objectToAnimate.SetActive(true);

        yield return MoveObjectToPosition(objectToAnimate, targetPosition2, 1f);

        animationRandomTriggered = false;
        animationActiveObject = true;

        currentIndex = (currentIndex + 1) % nextClotheSection.Count;
    }

    private bool AllImagesNonWhite()
    {
        foreach (var section in nextClotheSection)
        {
            if (section.images == null || section.images.Length == 0)
            {
                continue;
            }
            if (section.objectToAnimate != null && section.objectToAnimate.activeSelf)
            {
                foreach (Image image in section.images)
                {
                    if (image == null)
                    {
                        continue;
                    }
                    if (image.color == Color.white)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}
