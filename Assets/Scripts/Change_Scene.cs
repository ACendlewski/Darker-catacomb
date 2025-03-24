using System.Collections;
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
            changeSceneButton.onClick.AddListener(() => StartCoroutine(ChangeScene(sceneToLoad)));
        }
    }

    IEnumerator ChangeScene(string sceneName)
    {
        if (sceneName.ToLower() == "quit")
        {
            Application.Quit();
            Debug.Log("Zamykanie gry...");
            yield break;
        }

        if (sceneName.ToLower() == "start")
        {
            yield return StartCoroutine(DestroyAllDontDestroyOnLoadObjects());
        }
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator DestroyAllDontDestroyOnLoadObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                Destroy(obj);
            }
        }

        yield return null;
    }
}
