using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public string sceneToLoad;
    public Image progressBar;
    public TextMeshProUGUI statusText;

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            progressBar.fillAmount = progress;

            
            statusText.text = (progress * 100).ToString("F0") + "%";

            
            if (asyncOperation.progress >= 0.9f)
            {
                statusText.text = "100%";
                
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
