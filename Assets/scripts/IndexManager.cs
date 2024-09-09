using UnityEngine;
using System.Collections;
using static initializeSlotsInventory;

public class IndexManager : MonoBehaviour
{
    private initializeSlotsInventory inventory;
    public GameObject parentObject;
    public string keyString;
    private int maxSlots = 15;
    public AutoProduction autoProduction;
    public string savedSlots = "ShirtSlots_UnlockedSlots";
    private int countProces = 0;
    private bool countAddProces;
    public bool stop = true;

    private bool ActiveChildrenCoroutine = false;
    private void Start()
    {
        inventory = FindObjectOfType<initializeSlotsInventory>();
        AssignIndexes();
    }

   
    private void LateUpdate()
    {
        if (gameObject.activeInHierarchy && stop)
        {
            autoProduction.productCompleteUpdateObjects = true;
            stop = false;

        }

        if (autoProduction.productCompleteUpdateObjects)
        {
             ActivateMatchingChildren();
        }

        if (ActiveChildrenCoroutine)
        {
          StartCoroutine(startActivate());

        }
      
    }

    private IEnumerator startActivate()
    {
        ActiveChildrenCoroutine = false;
        yield return new WaitForSeconds(5);
        if (autoProduction.countIndexForSaveProduct == 29)
        {
            ActivateMatchingChildren();
        }
        ActiveChildrenCoroutine = true;
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
        countAddProces = true;
        if (countAddProces)
        {
            countProces++;
        }

        if (countProces >= 5)
        {
            countAddProces = false;
            countProces = 0;
            autoProduction.productCompleteUpdateObjects = false;
        }
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
