using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class mainmenu: MonoBehaviour
{
    private bool isPlayingTranistions = false;
    private bool closeboard = false;
    private GameObject objectToClose;

    
    private void DeactivateObject()
    {
        if (objectToClose != null)
        {
            objectToClose.SetActive(false);
        }
    }
   
    public void pause()
    {
        Time.timeScale = 0f;
    }
    public void UnPause()
    {
        Time.timeScale = 1f;
    }
    public void loadmenu()
    {
        Time.timeScale = 1f;
        isPlayingTranistions = true;

        SceneManager.LoadScene(1);
        isPlayingTranistions = false;
    }
   
  
   
    public void ButtonPlay()
    {
        isPlayingTranistions = true;

        SceneManager.LoadScene("game");
        isPlayingTranistions = false;
    }
   
    
 

}
