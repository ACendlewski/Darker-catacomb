using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Poziom trudności od 1 do 10 – ustawiany przez przyciski w scenie wyboru poziomów
    public int difficultyLevel = 1;

    // Przykładowy multiplikator – poziom 1 daje 1.0, a poziom 10 daje 1 + 9*0.5 = 5.5
    public float DifficultyMultiplier => 1f + (difficultyLevel - 1) * 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
