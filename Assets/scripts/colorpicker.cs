using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public Button[] colorButtons; 
    public Color selectedColor = Color.white;
    private Audiomanager audiomanager;

    private void Start()
    {
        audiomanager = FindObjectOfType<Audiomanager>();
    }
    public void colorSelect(int index)
    {
        selectedColor = colorButtons[index].GetComponent<Image>().color;
        audiomanager.PlaySFX(audiomanager.ColorPicker);
    }

}
