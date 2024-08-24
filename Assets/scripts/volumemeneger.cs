using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class volumemeneger : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] public Slider musicslider;
    [SerializeField] private Slider musicSFXslider;
    public AudioSource musicSource;
    public AudioSource musicSourcedefault;
    public AudioSource soundSource;
    private bool soundMuted = true;
    private bool musicMuted = true;

    public GameObject[] buttonsMusicAndSound;
    public GameObject[] imageMusicAndSound;

    private void Start()
    {
        LoadSoundAndMusicState();
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusisVolume();
            SetSFXVolume();
        }
    }

    private void Update()
    {
        musicSource.mute = musicMuted;
        musicSourcedefault.mute = musicMuted;
        soundSource.mute = soundMuted;
    }

    public void SetMusisVolume()
    {
        float volume = musicslider.value;
        float maxVolume = Mathf.Lerp(-50f, 0f, volume);
        myMixer.SetFloat("music", maxVolume);
        PlayerPrefs.SetFloat("musicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume()
    {
        float volume = musicSFXslider.value;
        float maxVolume = Mathf.Lerp(-50f, 0f, volume);
        myMixer.SetFloat("Sound", maxVolume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void MuteSound()
    {
        soundMuted = false;
        SaveSoundState();
        buttonsMusicAndSound[1].SetActive(true);
        buttonsMusicAndSound[0].SetActive(false);
        imageMusicAndSound[1].SetActive(true);
        imageMusicAndSound[0].SetActive(false);

    }

    public void UnMuteSound()
    {
        soundMuted = true;
        SaveSoundState();
        buttonsMusicAndSound[1].SetActive(false);
        buttonsMusicAndSound[0].SetActive(true);
        imageMusicAndSound[1].SetActive(false);
        imageMusicAndSound[0].SetActive(true);
    }

    public void MuteMusic()
    {
        musicMuted = false;
        SaveMusicState();
        buttonsMusicAndSound[3].SetActive(true);
        buttonsMusicAndSound[2].SetActive(false);
        imageMusicAndSound[3].SetActive(true);
        imageMusicAndSound[2].SetActive(false);

    }

    public void UnMuteMusic()
    {
        musicMuted = true;
        SaveMusicState();
        buttonsMusicAndSound[3].SetActive(false);
        buttonsMusicAndSound[2].SetActive(true);
        imageMusicAndSound[3].SetActive(false);
        imageMusicAndSound[2].SetActive(true);
    }

    private void SaveSoundState()
    {
        PlayerPrefs.SetInt("SoundMuted", soundMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SaveMusicState()
    {
        PlayerPrefs.SetInt("MusicMuted", musicMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSoundAndMusicState()
    {
        int mutedSound = PlayerPrefs.GetInt("SoundMuted", 1);
        soundMuted = mutedSound == 1;
        if (soundMuted)
        {
            if (buttonsMusicAndSound.Length != 0 && imageMusicAndSound.Length != 0)
            {
               buttonsMusicAndSound[0].SetActive(true);
               buttonsMusicAndSound[1].SetActive(false);
               imageMusicAndSound[0].SetActive(true);
               imageMusicAndSound[1].SetActive(false);

            }
        }
        else
        {
            if (buttonsMusicAndSound.Length != 0 && imageMusicAndSound.Length != 0)
            {
               buttonsMusicAndSound[0].SetActive(false);
               buttonsMusicAndSound[1].SetActive(true);
               imageMusicAndSound[0].SetActive(false);
               imageMusicAndSound[1].SetActive(true);

            }
        }

        int mutedMusic = PlayerPrefs.GetInt("MusicMuted", 1);
        musicMuted = mutedMusic == 1;
        if (musicMuted)
        {
            if (buttonsMusicAndSound.Length != 0 && imageMusicAndSound.Length != 0)
            {
                buttonsMusicAndSound[2].SetActive(true);
                buttonsMusicAndSound[3].SetActive(false);
                imageMusicAndSound[2].SetActive(true);
                imageMusicAndSound[3].SetActive(false);
            }
        }
        else
        {
            if (buttonsMusicAndSound.Length != 0 && imageMusicAndSound.Length != 0)
            {
                buttonsMusicAndSound[2].SetActive(false);
                buttonsMusicAndSound[3].SetActive(true);
                imageMusicAndSound[2].SetActive(false);
                imageMusicAndSound[3].SetActive(true);
            }
        }

        Debug.Log("Loaded SoundMuted: " + soundMuted);
        Debug.Log("Loaded MusicMuted: " + musicMuted);
    }

    private void LoadVolume()
    {
        musicslider.value = PlayerPrefs.GetFloat("musicVolume");
        musicSFXslider.value = PlayerPrefs.GetFloat("SFXVolume");

        SetMusisVolume();
        SetSFXVolume();
    }
}
