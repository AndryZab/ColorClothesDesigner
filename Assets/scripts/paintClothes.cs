using UnityEngine;
using UnityEngine.UI;

public class paintClothes : MonoBehaviour
{
    public Camera mainCamera;
    private ColorPicker colorPicker;
    private Audiomanager audioManager;
    [System.Serializable]
    public class Section
    {
        public string index;
        public Image sectionImage;
        public Image[] neighbors;
        public string nameClothes;
    }

    public Section sections;

    private void Start()
    {
        audioManager = FindObjectOfType<Audiomanager>();
        colorPicker = FindObjectOfType<ColorPicker>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = mainCamera.ScreenPointToRay(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                Image image = hit.collider.GetComponent<Image>();
                if (image != null)
                {
                    PaintSection(image);
                }
            }
        }
        
    }

    private void PaintSection(Image image)
    {
        if (colorPicker == null)
        {
            return;
        }
        
        Color selectedColor = colorPicker.selectedColor;
        Section section = FindSection(image);

        if (section != null && CanPaintSection(section, selectedColor))
        {
            if (image.color != selectedColor)
            {
               audioManager.PlaySFX(audioManager.paintButton);

            }
            image.color = selectedColor;
            
            SaveColorAndIndex(section, image.color);


        }
    }

    private Section FindSection(Image image)
    {
        
        if (sections.sectionImage == image)
        {
                return sections;
        }
        
        return null;
    }

    private bool CanPaintSection(Section section, Color selectedColor)
    {
        
        foreach (Image neighbor in section.neighbors)
        {
            if (neighbor != null && neighbor.color == selectedColor)
            {
                return false; 
            }
        }
        return true; 
    }
    private void SaveColorAndIndex(Section section, Color color)
    {
        string colorHex = ColorUtility.ToHtmlStringRGB(color);
        string key = section.nameClothes + section.index;

        PlayerPrefs.SetString(key, colorHex);
        PlayerPrefs.Save();
    }





}
