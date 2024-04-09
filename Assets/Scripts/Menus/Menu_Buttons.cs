using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Buttons : MonoBehaviour
{
    public void StartGame()
    {

        SceneManager.LoadScene("Start");
    }
    // Start is called before the first frame update
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}
    
