using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CharacterStatsUI : MonoBehaviour
{
    public TextMeshProUGUI characterNameText;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI speedText;

    public GameObject statsPanel;
    public GameObject healthBarPrefab; // 🔥 Teraz pasek zdrowia przypisujemy w edytorze!

    public Transform[] playerPositions;
    public Transform[] enemyPositions;

    private Dictionary<string, GameObject> spawnedCharacters = new Dictionary<string, GameObject>();

    public void ShowCharacterStats(Character character)
    {
        if (statsPanel == null || character == null) return;
        statsPanel.SetActive(true);

        characterNameText.text = character.name;

        Debug.Log($"[CharacterStatsUI] Pokazuję statystyki dla: {character.name}");


        healthSlider.maxValue = character.maxHealth;
        healthSlider.value = character.health;
        healthText.text = $"{character.health} / {character.maxHealth}";

        attackText.text = "ATT: " + character.attack;
        defenseText.text = "DEF: " + character.defense;
        speedText.text = "SPD: " + character.speed;

        if (!spawnedCharacters.ContainsKey(character.name))
        {
            SpawnCharacter(character);
        }
        else
        {
            Debug.Log($"Character {character.name} already instantiated.");
        }
    }

    private void SpawnCharacter(Character character)
    {
        string prefabName = character.name.Replace("(Enemy) ", "").Trim();
        string path = character.isEnemy ? "Prefabs/Enemies/" + prefabName : "Prefabs/" + prefabName;

        GameObject characterPrefab = Resources.Load<GameObject>(path);
        if (characterPrefab == null)
        {
            Debug.LogError("Character prefab not found at: " + path);
            return;
        }

        Transform spawnPoint = GetSpawnPoint(character);
        if (spawnPoint == null)
        {
            Debug.LogError("No spawn position available for " + character.name);
            return;
        }

        Quaternion rotation = character.isEnemy ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        GameObject characterObject = Instantiate(characterPrefab, spawnPoint.position, rotation);
        characterObject.name = character.name;

        // 🔥 Tworzenie paska zdrowia z przypisanego prefabryktu
        // 🔥 Tworzenie paska zdrowia z przypisanego prefabryktu
        if (healthBarPrefab != null)
        {
            GameObject healthBarObject = Instantiate(healthBarPrefab, characterObject.transform);
            healthBarObject.SetActive(false); // Początkowo wyłączony

            // Dodaj Canvas, jeśli go nie ma
            Canvas canvas = healthBarObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = healthBarObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                CanvasScaler scaler = healthBarObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
            }

            healthBarObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            healthBarObject.transform.localPosition = new Vector3(0f, 2f, 0f); // Przesunięcie nad postacią
            healthBarObject.SetActive(true); // Aktywujemy pasek po skonfigurowaniu

            HealthBar healthBar = healthBarObject.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                Debug.Log($"[CharacterStatsUI] {character.name}: ustawiam maxHealth = {character.maxHealth}"); // 🔍 Debug

                StartCoroutine(DelayedHealthUpdate(healthBar, character.maxHealth, character.health));
            }
            else
            {
                Debug.LogError($"{character.name} - HealthBar script missing in prefab!");
            }
        }

        else
        {
            Debug.LogError("HealthBar prefab is not assigned in the Inspector!");
        }

        // Animator
        Animator animator = characterObject.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"{character.name} has no Animator component!");
        }
        else
        {
            animator.enabled = true;
        }

        spawnedCharacters[character.name] = characterObject;
    }

    private IEnumerator DelayedHealthUpdate(HealthBar healthBar, int maxHealth, int currentHealth)
    {
        yield return new WaitForEndOfFrame(); // Poczekaj do następnej klatki

        if (healthBar == null)
        {
            Debug.LogError("[CharacterStatsUI] HealthBar is NULL! Sprawdź, czy komponent został dodany do prefabu!");
            yield break;
        }

        Debug.Log($"[CharacterStatsUI] Aktualizacja zdrowia: maxHealth={maxHealth}, currentHealth={currentHealth}");

        healthBar.SetMaxHealth(maxHealth); // 🔥 Ustawia maxHealth tylko raz
        healthBar.UpdateHealth(currentHealth);
    }


    public void SpawnAllCharacters(List<Character> playerTeam, List<Character> enemyTeam)
    {
        for (int i = 0; i < playerTeam.Count; i++)
        {
            SpawnCharacter(playerTeam[i]);
            ShowCharacterStats(playerTeam[i]);
        }

        for (int i = 0; i < enemyTeam.Count; i++)
        {
            SpawnCharacter(enemyTeam[i]);
            ShowCharacterStats(enemyTeam[i]);
        }
    }

    private Transform GetSpawnPoint(Character character)
    {
        if (character.isEnemy)
        {
            int index = TurnManager.Instance.enemyTeam.IndexOf(character);
            if (index >= 0 && index < enemyPositions.Length)
            {
                return enemyPositions[index];
            }
        }
        else
        {
            int index = TurnManager.Instance.playerTeam.IndexOf(character);
            if (index >= 0 && index < playerPositions.Length)
            {
                return playerPositions[index];
            }
        }
        return null;
    }

    public void HideCharacterStats()
    {
        if (statsPanel != null)
        {
            statsPanel.SetActive(false);
        }
    }

    public void DisplayActionFeedback(string message)
    {
        Debug.Log(message);
    }
}
