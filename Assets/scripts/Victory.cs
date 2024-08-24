using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    public GameObject winBoard;
    private Audiomanager audiomanager;
    private bool allowWin = true;
    private bool increaseMusic = true;
    private volumemeneger Volumemeneger;
    [System.Serializable]
    public class objectsForWin
    {
        public int  name;
        public GameObject parentObject;
        public GameObject[] ObjectsForWin;
        public TextMeshProUGUI[] ratingForObject;
        public TextMeshProUGUI[] countSellObjects;
        public TextMeshProUGUI[] priceEarn;
        public TextMeshProUGUI[] textShowPlusOrMinus;


    }
    private float musicSave;
    public Button buttonclaim;
    public TextMeshProUGUI totalEarn;
    public List<objectsForWin> objects;
    void Start()
    {
        musicSave = PlayerPrefs.GetFloat("musicVolume"); 
        audiomanager = FindAnyObjectByType<Audiomanager>();
        Volumemeneger = FindObjectOfType<volumemeneger>();
    }


    private void Update()
    {
        if (!increaseMusic)
        {
            Volumemeneger.musicslider.value = musicSave;
            increaseMusic = true;
            buttonclaim.interactable = true;
        }
    }

    public void victory()
    {
        if (allowWin)
        {
            if (audiomanager != null)
            {
                if (increaseMusic)
                {
                   Volumemeneger.musicslider.value = 0.3f;
                   StartCoroutine(delayForBool());
                }
                
                audiomanager.PlaySFX(audiomanager.Victory);
                audiomanager.PlaySFX(audiomanager.coins);

            }

            winBoard.SetActive(true);
            allowWin = false;
        }
    }
    private IEnumerator delayForBool()
    {
        yield return new WaitForSeconds(3f);
        increaseMusic = false;
    }
   
}
