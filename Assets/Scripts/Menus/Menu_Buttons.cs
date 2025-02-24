using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Buttons : MonoBehaviour
{
    public Button Quitbutton;

    private void Start()
    {
        Quitbutton.onClick.AddListener(OnApplicationQuit);
    }
    public void StartGame()
    {

        SceneManager.LoadScene("Start");
    }
    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}
    
