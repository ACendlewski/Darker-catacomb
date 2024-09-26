using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Change_Scene : MonoBehaviour
{
    
    public Button changeSceneButton;  
    public string sceneToLoad;        

    void Start()
    {
        if (changeSceneButton != null)
        {
            changeSceneButton.onClick.AddListener(() => ChangeScene(sceneToLoad));
        }
    }
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
