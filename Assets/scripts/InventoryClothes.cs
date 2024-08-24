using UnityEngine;
using UnityEngine.UI;

public class InventoryClothes : MonoBehaviour
{
    private Image sectionImage;
    public string indexColor;
    public string nameClothes;

    private bool colorApplied = false; // Додаємо змінну для перевірки

    private void Awake()
    {
        sectionImage = GetComponent<Image>();
    }

    private void Update()
    {
        ApplySavedColor();
    }

    private void ApplySavedColor()
    {
        if (colorApplied) // Якщо колір вже застосовано, виходимо з методу
        {
            return;
        }

        if (sectionImage.color == Color.white) // Перевірка, чи потрібно застосовувати колір
        {
            string key = nameClothes + indexColor;
            if (PlayerPrefs.HasKey(key))
            {
                string colorHex = PlayerPrefs.GetString(key);
                Color color;
                if (ColorUtility.TryParseHtmlString("#" + colorHex, out color))
                {
                    sectionImage.color = color;
                    colorApplied = true; // Встановлюємо прапорець, що колір був застосований
                }
            }
        }
    }
}
