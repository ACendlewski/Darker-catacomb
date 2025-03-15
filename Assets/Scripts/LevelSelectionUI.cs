using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionUI : MonoBehaviour
{
    public void SetDifficultyAndGoToCharacterSelection(int level)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.difficultyLevel = level;
            Debug.Log("Wybrany poziom trudności: " + level);
        }
        // Przejdź do sceny wyboru postaci
        SceneManager.LoadScene("CharSel");
    }
}
