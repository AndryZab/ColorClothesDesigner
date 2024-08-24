using UnityEngine;
using UnityEngine.UI;

public class Audiomanager : MonoBehaviour
{
    public AudioSource musicSource; 
    public AudioSource musicsourcedefault;  
    public AudioSource SFXsource;
    public Slider[] sliders; 

    private int currentMusicIndex = -1;
    private bool isPlaying = false;

    public AudioClip NotEnoughCoins;
    public AudioClip WheelFortune;
    public AudioClip rewardcommon;
    public AudioClip rewardrare;
    public AudioClip rewardlegendary;
    public AudioClip equipitem;
    public AudioClip unequipitem;
    public AudioClip buyitem;
    public AudioClip upgradebutton;
    public AudioClip timer;
    public AudioClip Victory;
    public AudioClip coins;
    public AudioClip Select;
    public AudioClip Deselect;
    public AudioClip paintButton;
    public AudioClip ColorPicker;
    public AudioClip button;
    private stateitem stateitem;
   
    public void PlaySFX(AudioClip clip)
    {
        if (SFXsource != null && clip != null)
        {
            SFXsource.PlayOneShot(clip);
        }
    }
  
    

    public void StopMusic()
    {
        if (isPlaying)
        {
            musicSource.Stop();
            isPlaying = false;
            currentMusicIndex = -1;
        }
    }
}
