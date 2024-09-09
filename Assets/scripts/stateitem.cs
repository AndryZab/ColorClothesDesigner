using UnityEngine;

public class stateitem : MonoBehaviour
{
    public GameObject[] backgrounds;
    public AudioSource sourceMusicSave;
    public AudioSource defaultSource;

    public GameObject defaultbackground;

    public AudioClip[] musicClips;
    private int currentMusicIndex;
    private bool isPlaying;
    public bool musicfound = false;

    private void Start()
    {
        LoadBackgroundsState();
        LoadMusicState();
    }

    public void LoadMusicState()
    {
        bool musicFound = false;

        for (int i = 0; i < musicClips.Length; i++)
        {
            string key = "music_" + i;
            int musicIndex = PlayerPrefs.GetInt(key, -1);

            if (musicIndex == i)
            {
                if (sourceMusicSave.clip != musicClips[i] || !sourceMusicSave.isPlaying)
                {
                    sourceMusicSave.clip = musicClips[i];
                    sourceMusicSave.Play();
                    defaultSource.Stop();
                }
                musicFound = true;
                break;
            }
        }

        if (!musicFound && defaultSource.clip != null && !defaultSource.isPlaying)
        {
            defaultSource.Play();
            sourceMusicSave.Stop();
        }
    }

    public void LoadBackgroundsState()
    {
        bool hasSavedState = false;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (PlayerPrefs.HasKey("background_" + i))
            {
                hasSavedState = true;
                break;
            }

           
        }

        if (hasSavedState)
        {
            defaultbackground.SetActive(false);
            int activeIndex = -1;
            for (int i = 0; i < backgrounds.Length; i++)
            {
                int savedState = PlayerPrefs.GetInt("background_" + i, -1);
                if (savedState == i)
                {
                    activeIndex = i;
                }
                backgrounds[i].SetActive(savedState == i);
            }

            for (int i = 0; i < backgrounds.Length; i++)
            {
                if (i != activeIndex)
                {
                    backgrounds[i].SetActive(false);
                }
            }
        }
        else
        {
            if (defaultbackground != null)
            {
                defaultbackground.SetActive(true);
            }
            foreach (GameObject gameObject in backgrounds)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void StopAllMusic()
    {
        defaultSource.Stop();
        sourceMusicSave.Stop();
        isPlaying = false;
    }

    public void StopMusic()
    {
        StopAllMusic();
    }
}
