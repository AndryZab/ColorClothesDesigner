using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class initializeSlotsInventory : MonoBehaviour
{
    [System.Serializable]
    public class sectionsSlots
    {
        public int slotsAvalible;
        public TextMeshProUGUI textAvalible;
        public string savedSlots = "ShirtSlots_UnlockedSlots";
        public int countActiveClothes;
    }
    public List<sectionsSlots> slots = new List<sectionsSlots>();
    public GameObject panelForBlockPlay;

    public GameObject[] panelForBlockClothesObject;
    public IndexManager[] indexmanager;

    private void Start()
    {
        InitializeSlots();

    }

    private void Update()
    {
        foreach (var manager in indexmanager)
        {
            if (!manager.gameObject.activeInHierarchy)
            {
                manager.stop = true;
            }
        }
        bool shouldActivatePanel = false;

        foreach (sectionsSlots slot in slots)
        {

            if (slot.textAvalible != null)
            {
                slot.textAvalible.text = "Amount: " + slot.countActiveClothes + "/" + slot.slotsAvalible;
            }
            if (slot.slotsAvalible == slot.countActiveClothes)
            {
                shouldActivatePanel = true;
            }
        }
        if (panelForBlockPlay != null)
        {
            panelForBlockPlay.SetActive(shouldActivatePanel);
        }
        UpdateBlockClothesPanels();


    }
    public void InitializeSlots()
    {
        foreach (sectionsSlots slot in slots)
        {
            
            if (PlayerPrefs.HasKey(slot.savedSlots))
            {
                slot.slotsAvalible = PlayerPrefs.GetInt(slot.savedSlots);
            }
        }

    }
    private void UpdateBlockClothesPanels()
    {

        if (panelForBlockClothesObject == null || panelForBlockClothesObject.Length == 0)
        {
            return;
        }
        foreach (GameObject panel in panelForBlockClothesObject)
        {
            if (panel != null)
            {
                panel.SetActive(false);

            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < panelForBlockClothesObject.Length)
            {
                if (slots[i].slotsAvalible == slots[i].countActiveClothes)
                {
                    if (panelForBlockClothesObject[i] != null)
                    {
                        panelForBlockClothesObject[i].SetActive(true);

                    }
                }
            }
        }
    }

}
