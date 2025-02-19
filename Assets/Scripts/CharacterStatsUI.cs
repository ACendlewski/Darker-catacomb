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
    public GameObject healthBarPrefab;
    public Transform[] playerPositions;
    public Transform[] enemyPositions;

    public GameObject enemySelectionPanel;
    public GameObject enemyButtonPrefab;

    private Dictionary<string, GameObject> spawnedCharacters = new Dictionary<string, GameObject>();

    private void Start()
    {
        // Initialize any necessary components
    }

    public void ShowCharacterStats(Character character)
    {
        if (statsPanel == null || character == null) return;
        statsPanel.SetActive(true);

        characterNameText.text = character.name;
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
        UpdateEnemySelectionPanel();
    }

    private void SpawnCharacter(Character character)
    {
        if (character == null)
        {
            Debug.LogError("Attempted to spawn null character");
            return;
        }

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

        // Add health bar
        if (healthBarPrefab != null)
        {
            GameObject healthBarObject = Instantiate(healthBarPrefab, characterObject.transform);
            healthBarObject.SetActive(false);

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
            healthBarObject.transform.localPosition = new Vector3(0f, 2f, 0f);
            healthBarObject.SetActive(true);

            HealthBar healthBar = healthBarObject.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                StartCoroutine(DelayedHealthUpdate(healthBar, character.maxHealth, character.health));
            }
        }

        // Add button click handler for enemies
        if (character.isEnemy)
        {
            Button enemyButton = characterObject.GetComponent<Button>();
            if (enemyButton == null)
            {
                enemyButton = characterObject.AddComponent<Button>();
            }

            enemyButton.onClick.AddListener(() =>
            {
                TurnManager.Instance.SelectEnemy(character);
            });
        }

        spawnedCharacters[character.name] = characterObject;
    }

    private IEnumerator DelayedHealthUpdate(HealthBar healthBar, int maxHealth, int currentHealth)
    {
        yield return new WaitForEndOfFrame();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.UpdateHealth(currentHealth);
        }
    }

    public void SpawnAllCharacters(List<Character> playerTeam, List<Character> enemyTeam)
    {
        Debug.Log("Starting SpawnAllCharacters");

        if (playerTeam == null)
        {
            Debug.LogError("Player team list is null");
            return;
        }

        if (enemyTeam == null)
        {
            Debug.LogError("Enemy team list is null");
            return;
        }

        if (playerPositions == null)
        {
            Debug.LogError("Player positions array is not assigned!");
            return;
        }

        if (enemyPositions == null)
        {
            Debug.LogError("Enemy positions array is not assigned!");
            return;
        }

        Debug.Log($"Spawning {playerTeam.Count} player characters");
        for (int i = 0; i < playerTeam.Count; i++)
        {
            if (i < playerPositions.Length)
            {
                SpawnCharacter(playerTeam[i]);
                ShowCharacterStats(playerTeam[i]);
            }
            else
            {
                Debug.LogWarning($"Not enough player positions for character {playerTeam[i].name}");
            }
        }

        Debug.Log($"Spawning {enemyTeam.Count} enemy characters");
        for (int i = 0; i < enemyTeam.Count; i++)
        {
            if (i < enemyPositions.Length)
            {
                SpawnCharacter(enemyTeam[i]);
                ShowCharacterStats(enemyTeam[i]);
                TurnManager.Instance.AssignEnemySelectionTriggers(spawnedCharacters[enemyTeam[i].name]);


            }
            else
            {
                Debug.LogWarning($"Not enough enemy positions for character {enemyTeam[i].name}");
            }
        }
        foreach (Character enemy in enemyTeam)
        {
            TurnManager.Instance.AssignEnemySelectionTriggers(spawnedCharacters[enemy.name]);
        }

        Debug.Log("Finished SpawnAllCharacters");
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

    private void UpdateEnemySelectionPanel()
    {
        if (enemySelectionPanel == null || enemyButtonPrefab == null) return;

        foreach (Transform child in enemySelectionPanel.transform)
        {
            Destroy(child.gameObject);
        }

        enemySelectionPanel.SetActive(true);

        foreach (Character enemy in TurnManager.Instance.enemyTeam)
        {
            GameObject buttonObject = Instantiate(enemyButtonPrefab, enemySelectionPanel.transform);
            buttonObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Skalowanie przycisków

            TextMeshProUGUI buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = enemy.name;
            }

            Button button = buttonObject.GetComponent<Button>();
            if (button != null)
            {
                Character selectedEnemy = enemy;
                button.onClick.AddListener(() => TurnManager.Instance.SelectEnemy(selectedEnemy));
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(enemySelectionPanel.GetComponent<RectTransform>());
    }


    public void HideCharacterStats()
    {
        if (statsPanel != null)
        {
            statsPanel.SetActive(false);
        }
        if (enemySelectionPanel != null)
        {
            enemySelectionPanel.SetActive(false);
        }
    }

    public void DisplayActionFeedback(string message)
    {
        Debug.Log(message);
    }
}
