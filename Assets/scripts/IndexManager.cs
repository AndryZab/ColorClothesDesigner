using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using static initializeSlotsInventory;

public class IndexManager : MonoBehaviour
{
    private initializeSlotsInventory inventory;
    public GameObject parentObject;
    public string keyString;
    private int maxSlots = 15;
    public AutoProduction autoProduction;
    public string savedSlots = "ShirtSlots_UnlockedSlots";

    private void Start()
    {
        inventory = FindObjectOfType<initializeSlotsInventory>();
        AssignIndexes();
        StartCoroutine(UpdateRoutine()); 
    }

    private IEnumerator UpdateRoutine()
    {
        while (true)
        {
            ActivateMatchingChildren();
            yield return new WaitForSeconds(1f); 
        }
    }

    private void AssignIndexes()
    {
        if (parentObject == null)
        {
            return;
        }

        int index = 1;

        foreach (Transform child in parentObject.GetComponentsInChildren<Transform>(true))
        {
            InventoryClothes[] inventoryClothesComponents = child.GetComponents<InventoryClothes>();

            if (inventoryClothesComponents.Length > 0)
            {
                foreach (InventoryClothes inventoryClothesComponent in inventoryClothesComponents)
                {
                    if (string.IsNullOrEmpty(inventoryClothesComponent.indexColor) || string.IsNullOrEmpty(inventoryClothesComponent.nameClothes))
                    {
                        inventoryClothesComponent.indexColor = index.ToString();
                        inventoryClothesComponent.nameClothes = keyString;
                        index++;
                    }
                }
            }
        }
    }

    private void ActivateMatchingChildren()
    {
        int activeCount = 0;

        foreach (Transform frameShirt in parentObject.transform)
        {
            bool shouldActivate = true;

            foreach (Transform shirt in frameShirt)
            {
                InventoryClothes[] inventoryClothesComponents = shirt.GetComponents<InventoryClothes>();

                foreach (InventoryClothes inventoryClothesComponent in inventoryClothesComponents)
                {
                    if (string.IsNullOrEmpty(inventoryClothesComponent.indexColor))
                    {
                        shouldActivate = false;
                        break;
                    }

                    string key = keyString + inventoryClothesComponent.indexColor;
                    if (!PlayerPrefs.HasKey(key))
                    {
                        shouldActivate = false;
                        break;
                    }
                }

                if (!shouldActivate)
                {
                    break;
                }
            }

            frameShirt.gameObject.SetActive(shouldActivate);

            if (shouldActivate)
            {
                activeCount++;
            }
        }

        foreach (sectionsSlots active in inventory.slots)
        {
            if (active.savedSlots == savedSlots)
            {
                active.countActiveClothes = activeCount;
            }
        }
    }
}
