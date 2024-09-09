using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public Button[] colorButtons; 
    public Color selectedColor = Color.white;
    private Audiomanager audiomanager;

    private void Start()
    {
        LoadColorButtonStates();
        audiomanager = FindObjectOfType<Audiomanager>();
    }
    public void colorSelect(int index)
    {
        selectedColor = colorButtons[index].GetComponent<Image>().color;
        audiomanager.PlaySFX(audiomanager.ColorPicker);
    }
    private void LoadColorButtonStates()
    {
        for (int i = 0; i < colorButtons.Length; i++)
        {
            string key = "ColorActivate_" + i;

            
            if (PlayerPrefs.HasKey(key) && PlayerPrefs.GetString(key) == "active")
            {
                colorButtons[i].gameObject.SetActive(true);
            }
            else
            {

                colorButtons[i].gameObject.SetActive(false);
            }
        }
    }

}
