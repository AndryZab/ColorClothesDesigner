using UnityEngine;
using UnityEngine.UI;

public class InventoryClothes : MonoBehaviour
{
    private Image sectionImage;
    public string indexColor;
    public string nameClothes;


    private void Awake()
    {
        sectionImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        ApplySavedColor();
    }

    private void ApplySavedColor()
    {

        string key = nameClothes + indexColor;
        if (PlayerPrefs.HasKey(key))
        {
            string colorHex = PlayerPrefs.GetString(key);
            Color color;
            if (ColorUtility.TryParseHtmlString("#" + colorHex, out color))
            {
                sectionImage.color = color;
            }
        }

    }
}
