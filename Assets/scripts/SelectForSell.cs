using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectForSell : MonoBehaviour
{
    public string sellObjects;
    public TextMeshProUGUI SelectedCountPanels;
    public GameObject[] panelsSelected;
    public GameObject objectForShowAvalible;
    private int countToDelete;
    public GameObject paneBlock;
    private Audiomanager audiomanager;

    private void Start()
    {
        audiomanager = FindAnyObjectByType<Audiomanager>();
    }
    private void Update()
   {
        countToDelete = 0;
        bool anyPanelActive = false;

        foreach (GameObject panel in panelsSelected)
        {
            if (panel.activeSelf)
            {
                anyPanelActive = true;
                countToDelete++;
                paneBlock.SetActive(false);
            }
        }

        if (!anyPanelActive)
        {
            countToDelete = 0;
            paneBlock.SetActive(true);
        }

        
        int activeChildCount = 0;
        foreach (Transform child in objectForShowAvalible.transform)
        {
            if (child.gameObject.activeSelf)
            {
                activeChildCount++;
            }
        }

        SelectedCountPanels.text = "Selected: " + countToDelete.ToString() + "/" + activeChildCount.ToString();
    }
    public void PlayGameForSell()
    {
        if (gameObject.activeInHierarchy)
        {
            PlayerPrefs.SetInt(sellObjects, countToDelete);
            SceneManager.LoadScene("SellMiniGame");
        }
        

    }
    
    public void ButtonActiveAllPanels()
    {
        foreach (GameObject panel in panelsSelected)
        {
            Transform parentTransform = panel.transform.parent;
            if (parentTransform != null && parentTransform.gameObject.activeSelf)
            {
                panel.SetActive(true);
            }
            else
            {
                panel.SetActive(false);
            }
        }
    }
    public void ButtonInactiveAllPanels()
    {
        foreach (GameObject panel in panelsSelected)
        {
            Transform parentTransform = panel.transform.parent;
            if (parentTransform != null && parentTransform.gameObject.activeSelf)
            {
                panel.SetActive(false); 
            }
           
        }
    }




}
